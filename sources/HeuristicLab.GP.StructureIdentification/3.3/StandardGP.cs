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
