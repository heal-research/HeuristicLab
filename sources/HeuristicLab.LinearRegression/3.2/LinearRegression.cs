#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using System.Diagnostics;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.Modeling;
using HeuristicLab.GP;
using HeuristicLab.Random;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.LinearRegression {
  public class LinearRegression : ItemBase, IEditable, IAlgorithm {

    public string Name { get { return "LinearRegression"; } }
    public string Description { get { return "TODO"; } }

    private SequentialEngine.SequentialEngine engine;
    public IEngine Engine {
      get { return engine; }
    }

    public Dataset Dataset {
      get { return ProblemInjector.GetVariableValue<Dataset>("Dataset", null, false); }
      set { ProblemInjector.GetVariable("Dataset").Value = value; }
    }

    public int TargetVariable {
      get { return ProblemInjector.GetVariableValue<IntData>("TargetVariable", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TargetVariable", null, false).Data = value; }
    }

    public IOperator ProblemInjector {
      get {
        IOperator main = GetMainOperator();
        return main.SubOperators[1];
      }
      set {
        IOperator main = GetMainOperator();
        main.RemoveSubOperator(1);
        main.AddSubOperator(value, 1);
      }
    }

    public IAnalyzerModel Model {
      get {
        if (!engine.Terminated) throw new InvalidOperationException("The algorithm is still running. Wait until the algorithm is terminated to retrieve the result.");
        IScope bestModelScope = engine.GlobalScope;
        return CreateLRModel(bestModelScope);
      }
    }

    public LinearRegression() {
      engine = new SequentialEngine.SequentialEngine();
      CombinedOperator algo = CreateAlgorithm();
      engine.OperatorGraph.AddOperator(algo);
      engine.OperatorGraph.InitialOperator = algo;
    }

    private CombinedOperator CreateAlgorithm() {
      CombinedOperator algo = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      algo.Name = "LinearRegression";
      seq.Name = "LinearRegression";

      var randomInjector = new RandomInjector();
      randomInjector.Name = "Random Injector";
      IOperator globalInjector = CreateGlobalInjector();
      ProblemInjector problemInjector = new ProblemInjector();
      problemInjector.GetVariableInfo("MaxNumberOfTrainingSamples").Local = true;
      problemInjector.AddVariable(new HeuristicLab.Core.Variable("MaxNumberOfTrainingSamples", new IntData(5000)));

      IOperator shuffler = new DatasetShuffler();
      shuffler.GetVariableInfo("ShuffleStart").ActualName = "TrainingSamplesStart";
      shuffler.GetVariableInfo("ShuffleEnd").ActualName = "TrainingSamplesEnd";

      LinearRegressionOperator lrOperator = new LinearRegressionOperator();
      lrOperator.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      lrOperator.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";

      seq.AddSubOperator(randomInjector);
      seq.AddSubOperator(problemInjector);
      seq.AddSubOperator(globalInjector);
      seq.AddSubOperator(shuffler);
      seq.AddSubOperator(lrOperator);
      seq.AddSubOperator(CreateModelAnalyser());

      algo.OperatorGraph.InitialOperator = seq;
      algo.OperatorGraph.AddOperator(seq);

      return algo;
    }

    private IOperator CreateGlobalInjector() {
      VariableInjector injector = new VariableInjector();
      injector.AddVariable(new HeuristicLab.Core.Variable("PunishmentFactor", new DoubleData(10)));
      injector.AddVariable(new HeuristicLab.Core.Variable("TotalEvaluatedNodes", new DoubleData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("TreeEvaluator", new HL2TreeEvaluator()));
      injector.AddVariable(new HeuristicLab.Core.Variable("UseEstimatedTargetValue", new BoolData(false)));

      return injector;
    }

    private IOperator CreateModelAnalyser() {
      CombinedOperator modelAnalyser = new CombinedOperator();
      modelAnalyser.Name = "Model Analyzer";
      SequentialProcessor seqProc = new SequentialProcessor();
      #region MSE
      MeanSquaredErrorEvaluator trainingMSE = new MeanSquaredErrorEvaluator();
      trainingMSE.Name = "TrainingMseEvaluator";
      trainingMSE.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      trainingMSE.GetVariableInfo("MSE").ActualName = "TrainingQuality";
      trainingMSE.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      trainingMSE.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      MeanSquaredErrorEvaluator validationMSE = new MeanSquaredErrorEvaluator();
      validationMSE.Name = "ValidationMseEvaluator";
      validationMSE.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      validationMSE.GetVariableInfo("MSE").ActualName = "ValidationQuality";
      validationMSE.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationMSE.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      MeanSquaredErrorEvaluator testMSE = new MeanSquaredErrorEvaluator();
      testMSE.Name = "TestMseEvaluator";
      testMSE.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      testMSE.GetVariableInfo("MSE").ActualName = "TestQuality";
      testMSE.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testMSE.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      #endregion

      #region R2
      CoefficientOfDeterminationEvaluator trainingR2 = new CoefficientOfDeterminationEvaluator();
      trainingR2.Name = "TrainingR2Evaluator";
      trainingR2.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      trainingR2.GetVariableInfo("R2").ActualName = "TrainingR2";
      trainingR2.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      trainingR2.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      CoefficientOfDeterminationEvaluator validationR2 = new CoefficientOfDeterminationEvaluator();
      validationR2.Name = "ValidationR2Evaluator";
      validationR2.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      validationR2.GetVariableInfo("R2").ActualName = "ValidationR2";
      validationR2.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationR2.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      CoefficientOfDeterminationEvaluator testR2 = new CoefficientOfDeterminationEvaluator();
      testR2.Name = "TestR2Evaluator";
      testR2.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      testR2.GetVariableInfo("R2").ActualName = "TestR2";
      testR2.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testR2.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      #endregion

      #region MAPE
      MeanAbsolutePercentageErrorEvaluator trainingMAPE = new MeanAbsolutePercentageErrorEvaluator();
      trainingMAPE.Name = "TrainingMapeEvaluator";
      trainingMAPE.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      trainingMAPE.GetVariableInfo("MAPE").ActualName = "TrainingMAPE";
      trainingMAPE.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      trainingMAPE.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      MeanAbsolutePercentageErrorEvaluator validationMAPE = new MeanAbsolutePercentageErrorEvaluator();
      validationMAPE.Name = "ValidationMapeEvaluator";
      validationMAPE.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      validationMAPE.GetVariableInfo("MAPE").ActualName = "ValidationMAPE";
      validationMAPE.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationMAPE.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      MeanAbsolutePercentageErrorEvaluator testMAPE = new MeanAbsolutePercentageErrorEvaluator();
      testMAPE.Name = "TestMapeEvaluator";
      testMAPE.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      testMAPE.GetVariableInfo("MAPE").ActualName = "TestMAPE";
      testMAPE.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testMAPE.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      #endregion

      #region MAPRE
      MeanAbsolutePercentageOfRangeErrorEvaluator trainingMAPRE = new MeanAbsolutePercentageOfRangeErrorEvaluator();
      trainingMAPRE.Name = "TrainingMapreEvaluator";
      trainingMAPRE.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      trainingMAPRE.GetVariableInfo("MAPRE").ActualName = "TrainingMAPRE";
      trainingMAPRE.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      trainingMAPRE.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      MeanAbsolutePercentageOfRangeErrorEvaluator validationMAPRE = new MeanAbsolutePercentageOfRangeErrorEvaluator();
      validationMAPRE.Name = "ValidationMapreEvaluator";
      validationMAPRE.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      validationMAPRE.GetVariableInfo("MAPRE").ActualName = "ValidationMAPRE";
      validationMAPRE.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationMAPRE.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      MeanAbsolutePercentageOfRangeErrorEvaluator testMAPRE = new MeanAbsolutePercentageOfRangeErrorEvaluator();
      testMAPRE.Name = "TestMapreEvaluator";
      testMAPRE.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      testMAPRE.GetVariableInfo("MAPRE").ActualName = "TestMAPRE";
      testMAPRE.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testMAPRE.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      #endregion

      #region VAF
      VarianceAccountedForEvaluator trainingVAF = new VarianceAccountedForEvaluator();
      trainingVAF.Name = "TrainingVafEvaluator";
      trainingVAF.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      trainingVAF.GetVariableInfo("VAF").ActualName = "TrainingVAF";
      trainingVAF.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      trainingVAF.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      VarianceAccountedForEvaluator validationVAF = new VarianceAccountedForEvaluator();
      validationVAF.Name = "ValidationVafEvaluator";
      validationVAF.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      validationVAF.GetVariableInfo("VAF").ActualName = "ValidationVAF";
      validationVAF.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationVAF.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      VarianceAccountedForEvaluator testVAF = new VarianceAccountedForEvaluator();
      testVAF.Name = "TestVafEvaluator";
      testVAF.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      testVAF.GetVariableInfo("VAF").ActualName = "TestVAF";
      testVAF.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testVAF.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      #endregion

      HeuristicLab.GP.StructureIdentification.VariableEvaluationImpactCalculator evalImpactCalc = new HeuristicLab.GP.StructureIdentification.VariableEvaluationImpactCalculator();
      evalImpactCalc.GetVariableInfo("TrainingSamplesStart").ActualName = "ActualTrainingSamplesStart";
      evalImpactCalc.GetVariableInfo("TrainingSamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      evalImpactCalc.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      HeuristicLab.Modeling.VariableQualityImpactCalculator qualImpactCalc = new HeuristicLab.GP.StructureIdentification.VariableQualityImpactCalculator();
      qualImpactCalc.GetVariableInfo("TrainingSamplesStart").ActualName = "ActualTrainingSamplesStart";
      qualImpactCalc.GetVariableInfo("TrainingSamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      qualImpactCalc.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      seqProc.AddSubOperator(trainingMSE);
      seqProc.AddSubOperator(validationMSE);
      seqProc.AddSubOperator(testMSE);
      seqProc.AddSubOperator(trainingR2);
      seqProc.AddSubOperator(validationR2);
      seqProc.AddSubOperator(testR2);
      seqProc.AddSubOperator(trainingMAPE);
      seqProc.AddSubOperator(validationMAPE);
      seqProc.AddSubOperator(testMAPE);
      seqProc.AddSubOperator(trainingMAPRE);
      seqProc.AddSubOperator(validationMAPRE);
      seqProc.AddSubOperator(testMAPRE);
      seqProc.AddSubOperator(trainingVAF);
      seqProc.AddSubOperator(validationVAF);
      seqProc.AddSubOperator(testVAF);
      seqProc.AddSubOperator(qualImpactCalc);
      seqProc.AddSubOperator(evalImpactCalc);
      modelAnalyser.OperatorGraph.InitialOperator = seqProc;
      modelAnalyser.OperatorGraph.AddOperator(seqProc);
      return modelAnalyser;
    }


    protected internal virtual IAnalyzerModel CreateLRModel(IScope bestModelScope) {
      IGeneticProgrammingModel tree = bestModelScope.GetVariableValue<IGeneticProgrammingModel>("LinearRegressionModel", false);
      ITreeEvaluator evaluator = bestModelScope.GetVariableValue<ITreeEvaluator>("TreeEvaluator", true);
      IAnalyzerModel model = new AnalyzerModel();
      model.Predictor = new Predictor(evaluator, tree);
      model.TrainingMeanSquaredError = bestModelScope.GetVariableValue<DoubleData>("TrainingQuality", false).Data;
      model.ValidationMeanSquaredError = bestModelScope.GetVariableValue<DoubleData>("ValidationQuality", false).Data;
      model.TestMeanSquaredError = bestModelScope.GetVariableValue<DoubleData>("TestQuality", false).Data;
      model.TrainingCoefficientOfDetermination = bestModelScope.GetVariableValue<DoubleData>("TrainingR2", false).Data;
      model.ValidationCoefficientOfDetermination = bestModelScope.GetVariableValue<DoubleData>("ValidationR2", false).Data;
      model.TestCoefficientOfDetermination = bestModelScope.GetVariableValue<DoubleData>("TestR2", false).Data;
      model.TrainingMeanAbsolutePercentageError = bestModelScope.GetVariableValue<DoubleData>("TrainingMAPE", false).Data;
      model.ValidationMeanAbsolutePercentageError = bestModelScope.GetVariableValue<DoubleData>("ValidationMAPE", false).Data;
      model.TestMeanAbsolutePercentageError = bestModelScope.GetVariableValue<DoubleData>("TestMAPE", false).Data;
      model.TrainingMeanAbsolutePercentageOfRangeError = bestModelScope.GetVariableValue<DoubleData>("TrainingMAPRE", false).Data;
      model.ValidationMeanAbsolutePercentageOfRangeError = bestModelScope.GetVariableValue<DoubleData>("ValidationMAPRE", false).Data;
      model.TestMeanAbsolutePercentageOfRangeError = bestModelScope.GetVariableValue<DoubleData>("TestMAPRE", false).Data;
      model.TrainingVarianceAccountedFor = bestModelScope.GetVariableValue<DoubleData>("TrainingVAF", false).Data;
      model.ValidationVarianceAccountedFor = bestModelScope.GetVariableValue<DoubleData>("ValidationVAF", false).Data;
      model.TestVarianceAccountedFor = bestModelScope.GetVariableValue<DoubleData>("TestVAF", false).Data;

      HeuristicLab.DataAnalysis.Dataset ds = bestModelScope.GetVariableValue<Dataset>("Dataset", true);
      model.Dataset = ds;
      model.TargetVariable = ds.GetVariableName(bestModelScope.GetVariableValue<IntData>("TargetVariable", true).Data);
      model.TrainingSamplesStart = bestModelScope.GetVariableValue<IntData>("TrainingSamplesStart", true).Data;
      model.TrainingSamplesEnd = bestModelScope.GetVariableValue<IntData>("TrainingSamplesEnd", true).Data;
      model.ValidationSamplesStart = bestModelScope.GetVariableValue<IntData>("ValidationSamplesStart", true).Data;
      model.ValidationSamplesEnd = bestModelScope.GetVariableValue<IntData>("ValidationSamplesEnd", true).Data;
      model.TestSamplesStart = bestModelScope.GetVariableValue<IntData>("TestSamplesStart", true).Data;
      model.TestSamplesEnd = bestModelScope.GetVariableValue<IntData>("TestSamplesEnd", true).Data;

      ItemList evaluationImpacts = bestModelScope.GetVariableValue<ItemList>("VariableEvaluationImpacts", false);
      ItemList qualityImpacts = bestModelScope.GetVariableValue<ItemList>("VariableQualityImpacts", false);
      foreach (ItemList row in evaluationImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[1]).Data;
        model.SetVariableEvaluationImpact(variableName, impact);
        model.AddInputVariable(variableName);
      }
      foreach (ItemList row in qualityImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[1]).Data;
        model.SetVariableQualityImpact(variableName, impact);
        model.AddInputVariable(variableName);
      }

      return model;
    }

    private IOperator GetMainOperator() {
      CombinedOperator lr = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
      return lr.OperatorGraph.InitialOperator;
    }

    public override IView CreateView() {
      return engine.CreateView();
    }

    #region IEditable Members

    public IEditor CreateEditor() {
      return engine.CreateEditor();
    }

    #endregion
  }
}
