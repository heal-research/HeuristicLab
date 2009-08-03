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

using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Operators;
using HeuristicLab.Selection;
using HeuristicLab.Logging;
using HeuristicLab.Data;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Modeling;
using HeuristicLab.GP.Operators;

namespace HeuristicLab.GP.StructureIdentification {
  public class StandardGP : AlgorithmBase, IEditable {

    public override string Name { get { return "StandardGP"; } }

    public override int TargetVariable {
      get { return ProblemInjector.GetVariableValue<IntData>("TargetVariable", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TargetVariable", null, false).Data = value; }
    }

    public override Dataset Dataset {
      get { return ProblemInjector.GetVariableValue<Dataset>("Dataset", null, false); }
      set { ProblemInjector.GetVariable("Dataset").Value = value; }
    }

    public virtual int MaxGenerations {
      get { return GetVariableInjector().GetVariable("MaxGenerations").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxGenerations").GetValue<IntData>().Data = value; }
    }

    public virtual int TournamentSize {
      get { return GetVariableInjector().GetVariable("TournamentSize").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("TournamentSize").GetValue<IntData>().Data = value; }
    }

    public double FullTreeShakingFactor {
      get { return GetVariableInjector().GetVariable("FullTreeShakingFactor").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("FullTreeShakingFactor").GetValue<DoubleData>().Data = value; }
    }

    public double OnePointShakingFactor {
      get { return GetVariableInjector().GetVariable("OnePointShakingFactor").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("OnePointShakingFactor").GetValue<DoubleData>().Data = value; }
    }

    public int MinInitialTreeSize {
      get { return GetVariableInjector().GetVariable("MinInitialTreeSize").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MinInitialTreeSize").GetValue<IntData>().Data = value; }
    }

    public override int MaxTreeSize {
      get {
        return base.MaxTreeSize;
      }
      set {
        base.MaxTreeSize = value;
        MinInitialTreeSize = value / 2;
      }
    }

    public override int PopulationSize {
      get {
        return base.PopulationSize;
      }
      set {
        base.PopulationSize = value;
        Parents = 2 * value;
      }
    }

    public override IOperator ProblemInjector {
      get { return base.ProblemInjector.SubOperators[0]; }
      set {
        value.Name = "ProblemInjector";
        base.ProblemInjector.RemoveSubOperator(0);
        base.ProblemInjector.AddSubOperator(value, 0);
      }
    }

    public StandardGP()
      : base() {
      PopulationSize = 10000;
      MaxGenerations = 500;
      TournamentSize = 7;
      MutationRate = 0.15;
      Elites = 1;
      MaxTreeSize = 100;
      MaxTreeHeight = 10;
      FullTreeShakingFactor = 0.1;
      OnePointShakingFactor = 1.0;
      PunishmentFactor = 10.0;
      UseEstimatedTargetValue = false;
      SetSeedRandomly = true;
    }

    protected internal override IOperator CreateProblemInjector() {
      SequentialProcessor seq = new SequentialProcessor();      
      var probInject = new ProblemInjector();
      probInject.GetVariableInfo("MaxNumberOfTrainingSamples").Local = true;
      probInject.AddVariable(new HeuristicLab.Core.Variable("MaxNumberOfTrainingSamples", new IntData(5000)));

      var shuffler = new DatasetShuffler();
      shuffler.GetVariableInfo("ShuffleStart").ActualName = "TrainingSamplesStart";
      shuffler.GetVariableInfo("ShuffleEnd").ActualName = "TrainingSamplesEnd";

      seq.AddSubOperator(probInject);
      seq.AddSubOperator(shuffler);
      return seq;
    }

    protected internal override IOperator CreateSelector() {
      TournamentSelector selector = new TournamentSelector();
      selector.Name = "Selector";
      selector.GetVariableInfo("Selected").ActualName = "Parents";
      selector.GetVariableInfo("GroupSize").Local = false;
      selector.RemoveVariable("GroupSize");
      selector.GetVariableInfo("GroupSize").ActualName = "TournamentSize";
      return selector;
    }

    protected internal override IOperator CreateGlobalInjector() {
      VariableInjector globalInjector = (VariableInjector)base.CreateGlobalInjector();
      globalInjector.AddVariable(new HeuristicLab.Core.Variable("TournamentSize", new IntData()));
      globalInjector.AddVariable(new HeuristicLab.Core.Variable("MaxGenerations", new IntData()));
      globalInjector.AddVariable(new HeuristicLab.Core.Variable("FullTreeShakingFactor", new DoubleData()));
      globalInjector.AddVariable(new HeuristicLab.Core.Variable("OnePointShakingFactor", new DoubleData()));
      globalInjector.AddVariable(new HeuristicLab.Core.Variable("MinInitialTreeSize", new IntData()));
      return globalInjector;
    }

    protected internal override IOperator CreateCrossover() {
      StandardCrossOver crossover = new StandardCrossOver();
      crossover.Name = "Crossover";
      return crossover;
    }

    protected internal override IOperator CreateTreeCreator() {
      ProbabilisticTreeCreator treeCreator = new ProbabilisticTreeCreator();
      treeCreator.Name = "Tree generator";
      treeCreator.GetVariableInfo("MinTreeSize").ActualName = "MinInitialTreeSize";
      return treeCreator;
    }

    protected internal override IOperator CreateFunctionLibraryInjector() {
      FunctionLibraryInjector funLibInjector = new FunctionLibraryInjector();
      funLibInjector.GetVariableValue<BoolData>("Xor", null, false).Data = false;
      funLibInjector.GetVariableValue<BoolData>("Average", null, false).Data = false;
      return funLibInjector;
    }

    protected internal override IOperator CreateManipulator() {
      CombinedOperator manipulator = new CombinedOperator();
      manipulator.Name = "Manipulator";
      StochasticMultiBranch multibranch = new StochasticMultiBranch();
      FullTreeShaker fullTreeShaker = new FullTreeShaker();
      fullTreeShaker.GetVariableInfo("ShakingFactor").ActualName = "FullTreeShakingFactor";

      OnePointShaker onepointShaker = new OnePointShaker();
      onepointShaker.GetVariableInfo("ShakingFactor").ActualName = "OnePointShakingFactor";
      ChangeNodeTypeManipulation changeNodeTypeManipulation = new ChangeNodeTypeManipulation();
      CutOutNodeManipulation cutOutNodeManipulation = new CutOutNodeManipulation();
      DeleteSubTreeManipulation deleteSubTreeManipulation = new DeleteSubTreeManipulation();
      SubstituteSubTreeManipulation substituteSubTreeManipulation = new SubstituteSubTreeManipulation();

      IOperator[] manipulators = new IOperator[] {
        onepointShaker, fullTreeShaker,
        changeNodeTypeManipulation,
        cutOutNodeManipulation,
        deleteSubTreeManipulation,
        substituteSubTreeManipulation};

      DoubleArrayData probabilities = new DoubleArrayData(new double[manipulators.Length]);
      for (int i = 0; i < manipulators.Length; i++) {
        probabilities.Data[i] = 1.0;
        multibranch.AddSubOperator(manipulators[i]);
      }
      multibranch.GetVariableInfo("Probabilities").Local = true;
      multibranch.AddVariable(new HeuristicLab.Core.Variable("Probabilities", probabilities));

      manipulator.OperatorGraph.AddOperator(multibranch);
      manipulator.OperatorGraph.InitialOperator = multibranch;
      return manipulator;
    }

    protected internal override IOperator CreateBestSolutionProcessor() {
      SequentialProcessor bestSolutionProcessor = new SequentialProcessor();
      #region MSE
      MeanSquaredErrorEvaluator testMseEvaluator = new MeanSquaredErrorEvaluator();
      testMseEvaluator.Name = "TestMeanSquaredErrorEvaluator";
      testMseEvaluator.GetVariableInfo("MSE").ActualName = "TestQuality";
      testMseEvaluator.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testMseEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      #endregion
      #region MAPE
      MeanAbsolutePercentageErrorEvaluator trainingMapeEvaluator = new MeanAbsolutePercentageErrorEvaluator();
      trainingMapeEvaluator.Name = "TrainingMapeEvaluator";
      trainingMapeEvaluator.GetVariableInfo("MAPE").ActualName = "TrainingMAPE";
      trainingMapeEvaluator.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      trainingMapeEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      MeanAbsolutePercentageErrorEvaluator validationMapeEvaluator = new MeanAbsolutePercentageErrorEvaluator();
      validationMapeEvaluator.Name = "ValidationMapeEvaluator";
      validationMapeEvaluator.GetVariableInfo("MAPE").ActualName = "ValidationMAPE";
      validationMapeEvaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationMapeEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      MeanAbsolutePercentageErrorEvaluator testMapeEvaluator = new MeanAbsolutePercentageErrorEvaluator();
      testMapeEvaluator.Name = "TestMapeEvaluator";
      testMapeEvaluator.GetVariableInfo("MAPE").ActualName = "TestMAPE";
      testMapeEvaluator.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testMapeEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      #endregion
      #region MAPRE
      MeanAbsolutePercentageOfRangeErrorEvaluator trainingMapreEvaluator = new MeanAbsolutePercentageOfRangeErrorEvaluator();
      trainingMapreEvaluator.Name = "TrainingMapreEvaluator";
      trainingMapreEvaluator.GetVariableInfo("MAPRE").ActualName = "TrainingMAPRE";
      trainingMapreEvaluator.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      trainingMapreEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      MeanAbsolutePercentageOfRangeErrorEvaluator validationMapreEvaluator = new MeanAbsolutePercentageOfRangeErrorEvaluator();
      validationMapreEvaluator.Name = "ValidationMapreEvaluator";
      validationMapreEvaluator.GetVariableInfo("MAPRE").ActualName = "ValidationMAPRE";
      validationMapreEvaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationMapreEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      MeanAbsolutePercentageOfRangeErrorEvaluator testMapreEvaluator = new MeanAbsolutePercentageOfRangeErrorEvaluator();
      testMapreEvaluator.Name = "TestMapreEvaluator";
      testMapreEvaluator.GetVariableInfo("MAPRE").ActualName = "TestMAPRE";
      testMapreEvaluator.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testMapreEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      #endregion MAPRE
      #region R2
      CoefficientOfDeterminationEvaluator trainingR2Evaluator = new CoefficientOfDeterminationEvaluator();
      trainingR2Evaluator.Name = "TrainingR2Evaluator";
      trainingR2Evaluator.GetVariableInfo("R2").ActualName = "TrainingR2";
      trainingR2Evaluator.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      trainingR2Evaluator.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      CoefficientOfDeterminationEvaluator validationR2Evaluator = new CoefficientOfDeterminationEvaluator();
      validationR2Evaluator.Name = "ValidationR2Evaluator";
      validationR2Evaluator.GetVariableInfo("R2").ActualName = "ValidationR2";
      validationR2Evaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationR2Evaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      CoefficientOfDeterminationEvaluator testR2Evaluator = new CoefficientOfDeterminationEvaluator();
      testR2Evaluator.Name = "TestR2Evaluator";
      testR2Evaluator.GetVariableInfo("R2").ActualName = "TestR2";
      testR2Evaluator.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testR2Evaluator.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      #endregion
      #region VAF
      VarianceAccountedForEvaluator trainingVAFEvaluator = new VarianceAccountedForEvaluator();
      trainingVAFEvaluator.Name = "TrainingVAFEvaluator";
      trainingVAFEvaluator.GetVariableInfo("VAF").ActualName = "TrainingVAF";
      trainingVAFEvaluator.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      trainingVAFEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      VarianceAccountedForEvaluator validationVAFEvaluator = new VarianceAccountedForEvaluator();
      validationVAFEvaluator.Name = "ValidationVAFEvaluator";
      validationVAFEvaluator.GetVariableInfo("VAF").ActualName = "ValidationVAF";
      validationVAFEvaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationVAFEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      VarianceAccountedForEvaluator testVAFEvaluator = new VarianceAccountedForEvaluator();
      testVAFEvaluator.Name = "TestVAFEvaluator";
      testVAFEvaluator.GetVariableInfo("VAF").ActualName = "TestVAF";
      testVAFEvaluator.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testVAFEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      #endregion
      ProgrammableOperator progOperator = new ProgrammableOperator();
      progOperator.RemoveVariableInfo("Result");
      progOperator.AddVariableInfo(new HeuristicLab.Core.VariableInfo("EvaluatedSolutions", "", typeof(IntData), VariableKind.In));
      progOperator.Code = @"
int evalSolutions = EvaluatedSolutions.Data;
scope.AddVariable(new Variable(""EvaluatedSolutions"", new IntData(evalSolutions)));
";
      bestSolutionProcessor.AddSubOperator(testMseEvaluator);
      bestSolutionProcessor.AddSubOperator(trainingMapeEvaluator);
      bestSolutionProcessor.AddSubOperator(validationMapeEvaluator);
      bestSolutionProcessor.AddSubOperator(testMapeEvaluator);
      bestSolutionProcessor.AddSubOperator(trainingMapreEvaluator);
      bestSolutionProcessor.AddSubOperator(validationMapreEvaluator);
      bestSolutionProcessor.AddSubOperator(testMapreEvaluator);
      bestSolutionProcessor.AddSubOperator(trainingR2Evaluator);
      bestSolutionProcessor.AddSubOperator(validationR2Evaluator);
      bestSolutionProcessor.AddSubOperator(testR2Evaluator);
      bestSolutionProcessor.AddSubOperator(trainingVAFEvaluator);
      bestSolutionProcessor.AddSubOperator(validationVAFEvaluator);
      bestSolutionProcessor.AddSubOperator(testVAFEvaluator);
      bestSolutionProcessor.AddSubOperator(progOperator);
      return bestSolutionProcessor;
    }

    protected internal override IOperator CreateLoggingOperator() {
      CombinedOperator loggingOperator = new CombinedOperator();
      loggingOperator.Name = "Logging";
      SequentialProcessor seq = new SequentialProcessor();

      DataCollector collector = new DataCollector();
      ItemList<StringData> names = collector.GetVariable("VariableNames").GetValue<ItemList<StringData>>();
      names.Add(new StringData("BestQuality"));
      names.Add(new StringData("AverageQuality"));
      names.Add(new StringData("WorstQuality"));
      names.Add(new StringData("BestValidationQuality"));
      names.Add(new StringData("AverageValidationQuality"));
      names.Add(new StringData("WorstValidationQuality"));
      LinechartInjector lineChartInjector = new LinechartInjector();
      lineChartInjector.GetVariableInfo("Linechart").ActualName = "Quality Linechart";
      lineChartInjector.GetVariable("NumberOfLines").GetValue<IntData>().Data = 6;
      QualityLogger qualityLogger = new QualityLogger();
      QualityLogger validationQualityLogger = new QualityLogger();
      validationQualityLogger.Name = "ValidationQualityLogger";
      validationQualityLogger.GetVariableInfo("Quality").ActualName = "ValidationQuality";
      validationQualityLogger.GetVariableInfo("QualityLog").ActualName = "ValidationQualityLog";

      seq.AddSubOperator(collector);
      seq.AddSubOperator(lineChartInjector);
      seq.AddSubOperator(qualityLogger);
      seq.AddSubOperator(validationQualityLogger);

      loggingOperator.OperatorGraph.AddOperator(seq);
      loggingOperator.OperatorGraph.InitialOperator = seq;
      return loggingOperator;
    }

    protected internal override Model CreateGPModel(IScope bestModelScope) {
      Model model = base.CreateGPModel(bestModelScope);
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

      return model;
    }

    public virtual IEditor CreateEditor() {
      return new StandardGpEditor(this);
    }

    public override IView CreateView() {
      return new StandardGpEditor(this);
    }
  }
}
