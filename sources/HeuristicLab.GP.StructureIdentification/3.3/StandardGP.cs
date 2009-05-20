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
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Selection;
using HeuristicLab.Logging;
using HeuristicLab.Data;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Modeling;

namespace HeuristicLab.GP.StructureIdentification {
  public class StandardGP : AlgorithmBase, IEditable {

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

    public StandardGP()
      : base() {
      PopulationSize = 10000;
      MaxGenerations = 100;
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
      return new ProblemInjector();
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
      crossover.GetVariableInfo("OperatorLibrary").ActualName = "FunctionLibrary";
      return crossover;
    }

    protected internal override IOperator CreateTreeCreator() {
      ProbabilisticTreeCreator treeCreator = new ProbabilisticTreeCreator();
      treeCreator.Name = "Tree generator";
      treeCreator.GetVariableInfo("OperatorLibrary").ActualName = "FunctionLibrary";
      treeCreator.GetVariableInfo("MinTreeSize").ActualName = "MinInitialTreeSize";
      return treeCreator;
    }

    protected internal override IOperator CreateFunctionLibraryInjector() {
      return new FunctionLibraryInjector();
    }

    protected internal override IOperator CreateManipulator() {
      CombinedOperator manipulator = new CombinedOperator();
      manipulator.Name = "Manipulator";
      StochasticMultiBranch multibranch = new StochasticMultiBranch();
      FullTreeShaker fullTreeShaker = new FullTreeShaker();
      fullTreeShaker.GetVariableInfo("OperatorLibrary").ActualName = "FunctionLibrary";
      fullTreeShaker.GetVariableInfo("ShakingFactor").ActualName = "FullTreeShakingFactor";

      OnePointShaker onepointShaker = new OnePointShaker();
      onepointShaker.GetVariableInfo("OperatorLibrary").ActualName = "FunctionLibrary";
      onepointShaker.GetVariableInfo("ShakingFactor").ActualName = "OnePointShakingFactor";
      ChangeNodeTypeManipulation changeNodeTypeManipulation = new ChangeNodeTypeManipulation();
      changeNodeTypeManipulation.GetVariableInfo("OperatorLibrary").ActualName = "FunctionLibrary";
      CutOutNodeManipulation cutOutNodeManipulation = new CutOutNodeManipulation();
      cutOutNodeManipulation.GetVariableInfo("OperatorLibrary").ActualName = "FunctionLibrary";
      DeleteSubTreeManipulation deleteSubTreeManipulation = new DeleteSubTreeManipulation();
      deleteSubTreeManipulation.GetVariableInfo("OperatorLibrary").ActualName = "FunctionLibrary";
      SubstituteSubTreeManipulation substituteSubTreeManipulation = new SubstituteSubTreeManipulation();
      substituteSubTreeManipulation.GetVariableInfo("OperatorLibrary").ActualName = "FunctionLibrary";

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
      MeanSquaredErrorEvaluator testMseEvaluator = new MeanSquaredErrorEvaluator();
      testMseEvaluator.Name = "TestMeanSquaredErrorEvaluator";
      testMseEvaluator.GetVariableInfo("MSE").ActualName = "TestQuality";
      testMseEvaluator.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testMseEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      MeanAbsolutePercentageErrorEvaluator trainingMapeEvaluator = new MeanAbsolutePercentageErrorEvaluator();
      trainingMapeEvaluator.Name = "TrainingMapeEvaluator";
      trainingMapeEvaluator.GetVariableInfo("MAPE").ActualName = "TrainingMAPE";
      trainingMapeEvaluator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      trainingMapeEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
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
      MeanAbsolutePercentageOfRangeErrorEvaluator trainingMapreEvaluator = new MeanAbsolutePercentageOfRangeErrorEvaluator();
      trainingMapreEvaluator.Name = "TrainingMapreEvaluator";
      trainingMapreEvaluator.GetVariableInfo("MAPRE").ActualName = "TrainingMAPRE";
      trainingMapreEvaluator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      trainingMapreEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
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
      CoefficientOfDeterminationEvaluator trainingR2Evaluator = new CoefficientOfDeterminationEvaluator();
      trainingR2Evaluator.Name = "TrainingR2Evaluator";
      trainingR2Evaluator.GetVariableInfo("R2").ActualName = "TrainingR2";
      trainingR2Evaluator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      trainingR2Evaluator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
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

    public virtual IEditor CreateEditor() {
      return new StandardGpEditor(this);
    }

    public override IView CreateView() {
      return new StandardGpEditor(this);
    }
  }
}
