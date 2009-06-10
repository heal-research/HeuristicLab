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
using HeuristicLab.Evolutionary;
using HeuristicLab.Modeling;

namespace HeuristicLab.GP.StructureIdentification {
  public abstract class AlgorithmBase : ItemBase, IAlgorithm, IStochasticAlgorithm {
    public virtual string Name { get { return "GP"; } }
    public virtual string Description { get { return "TODO"; } }

    public abstract Dataset Dataset { get; set; }
    public abstract int TargetVariable { get; set; }

    public virtual double MutationRate {
      get { return GetVariableInjector().GetVariable("MutationRate").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("MutationRate").GetValue<DoubleData>().Data = value; }
    }
    public virtual int PopulationSize {
      get { return GetVariableInjector().GetVariable("PopulationSize").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("PopulationSize").GetValue<IntData>().Data = value; }
    }

    public virtual bool SetSeedRandomly {
      get { return GetRandomInjector().GetVariable("SetSeedRandomly").GetValue<BoolData>().Data; }
      set { GetRandomInjector().GetVariable("SetSeedRandomly").GetValue<BoolData>().Data = value; }
    }

    public virtual int RandomSeed {
      get { return GetRandomInjector().GetVariable("Seed").GetValue<IntData>().Data; }
      set { GetRandomInjector().GetVariable("Seed").GetValue<IntData>().Data = value; }
    }

    public virtual IOperator ProblemInjector {
      get { return algorithm.SubOperators[0]; }
      set {
        value.Name = "ProblemInjector";
        algorithm.RemoveSubOperator(0);
        algorithm.AddSubOperator(value, 0);
      }
    }

    public virtual IModel Model {
      get {
        if (!engine.Terminated) throw new InvalidOperationException("The algorithm is still running. Wait until the algorithm is terminated to retrieve the result.");
        IScope bestModelScope = engine.GlobalScope.GetVariableValue<IScope>("BestValidationSolution", false);
        return CreateGPModel(bestModelScope);
      }
    }

    public virtual int Elites {
      get { return GetVariableInjector().GetVariable("Elites").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("Elites").GetValue<IntData>().Data = value; }
    }

    public virtual int MaxTreeSize {
      get { return GetVariableInjector().GetVariable("MaxTreeSize").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxTreeSize").GetValue<IntData>().Data = value; }
    }

    public virtual int MaxTreeHeight {
      get { return GetVariableInjector().GetVariable("MaxTreeHeight").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxTreeHeight").GetValue<IntData>().Data = value; }
    }

    public virtual int Parents {
      get { return GetVariableInjector().GetVariable("Parents").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("Parents").GetValue<IntData>().Data = value; }
    }

    public virtual double PunishmentFactor {
      get { return GetVariableInjector().GetVariable("PunishmentFactor").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("PunishmentFactor").GetValue<DoubleData>().Data = value; }
    }

    public virtual bool UseEstimatedTargetValue {
      get { return GetVariableInjector().GetVariable("UseEstimatedTargetValue").GetValue<BoolData>().Data; }
      set { GetVariableInjector().GetVariable("UseEstimatedTargetValue").GetValue<BoolData>().Data = value; }
    }

    private IOperator algorithm;

    private SequentialEngine.SequentialEngine engine;
    public IEngine Engine {
      get { return engine; }
      protected set { engine = (SequentialEngine.SequentialEngine)value; }
    }

    public AlgorithmBase() {
      engine = new SequentialEngine.SequentialEngine();
      CombinedOperator algo = CreateAlgorithm();
      engine.OperatorGraph.AddOperator(algo);
      engine.OperatorGraph.InitialOperator = algo;
      SetSeedRandomly = true;
      Elites = 1;
      MutationRate = 0.15;
      PopulationSize = 1000;
      MaxTreeSize = 100;
      MaxTreeHeight = 10;
      Parents = 2000;
      PunishmentFactor = 10;
      UseEstimatedTargetValue = false;
    }

    protected internal virtual CombinedOperator CreateAlgorithm() {
      CombinedOperator algo = new CombinedOperator();
      algo.Name = "GP";
      SequentialProcessor seq = new SequentialProcessor();
      IOperator problemInjector = CreateProblemInjector();

      RandomInjector randomInjector = new RandomInjector();
      randomInjector.Name = "Random Injector";

      IOperator globalInjector = CreateGlobalInjector();
      IOperator initialization = CreateInitialization();
      IOperator funLibInjector = CreateFunctionLibraryInjector();

      IOperator mainLoop = CreateMainLoop();
      mainLoop.Name = "Main loop";

      IOperator treeCreator = CreateTreeCreator();

      MeanSquaredErrorEvaluator evaluator = new MeanSquaredErrorEvaluator();
      evaluator.GetVariableInfo("MSE").ActualName = "Quality";
      evaluator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      evaluator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
      evaluator.Name = "Evaluator";

      IOperator crossover = CreateCrossover();
      IOperator manipulator = CreateManipulator();

      IOperator selector = CreateSelector();
      LeftReducer cleanUp = new LeftReducer();

      seq.AddSubOperator(problemInjector);
      seq.AddSubOperator(randomInjector);
      seq.AddSubOperator(globalInjector);
      seq.AddSubOperator(funLibInjector);
      seq.AddSubOperator(initialization);
      seq.AddSubOperator(mainLoop);
      seq.AddSubOperator(cleanUp);

      initialization.AddSubOperator(treeCreator);
      initialization.AddSubOperator(evaluator);

      mainLoop.AddSubOperator(selector);
      mainLoop.AddSubOperator(crossover);
      mainLoop.AddSubOperator(manipulator);
      mainLoop.AddSubOperator(evaluator);
      algo.OperatorGraph.AddOperator(seq);
      algo.OperatorGraph.InitialOperator = seq;
      this.algorithm = seq;
      return algo;
    }

    protected internal virtual IOperator CreateProblemInjector() {
      return new EmptyOperator();
    }

    protected internal abstract IOperator CreateSelector();

    protected internal abstract IOperator CreateCrossover();

    protected internal abstract IOperator CreateTreeCreator();

    protected internal abstract IOperator CreateFunctionLibraryInjector();

    protected internal virtual IOperator CreateGlobalInjector() {
      VariableInjector injector = new VariableInjector();
      injector.Name = "Global Injector";
      injector.AddVariable(new HeuristicLab.Core.Variable("Generations", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("MutationRate", new DoubleData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("PopulationSize", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("Elites", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("Maximization", new BoolData(false)));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxTreeHeight", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxTreeSize", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("EvaluatedSolutions", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("TotalEvaluatedNodes", new DoubleData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("Parents", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("PunishmentFactor", new DoubleData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("UseEstimatedTargetValue", new BoolData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("TreeEvaluator", new HL2TreeEvaluator()));
      return injector;
    }

    protected internal abstract IOperator CreateManipulator();

    protected internal virtual IOperator CreateInitialization() {
      CombinedOperator init = new CombinedOperator();
      init.Name = "Initialization";
      SequentialProcessor seq = new SequentialProcessor();
      SubScopesCreater subScopesCreater = new SubScopesCreater();
      subScopesCreater.GetVariableInfo("SubScopes").ActualName = "PopulationSize";
      UniformSequentialSubScopesProcessor subScopesProc = new UniformSequentialSubScopesProcessor();
      SequentialProcessor individualSeq = new SequentialProcessor();
      OperatorExtractor treeCreater = new OperatorExtractor();
      treeCreater.Name = "Tree generator (extr.)";
      treeCreater.GetVariableInfo("Operator").ActualName = "Tree generator";
      OperatorExtractor evaluator = new OperatorExtractor();
      evaluator.Name = "Evaluator (extr.)";
      evaluator.GetVariableInfo("Operator").ActualName = "Evaluator";
      MeanSquaredErrorEvaluator validationEvaluator = new MeanSquaredErrorEvaluator();
      validationEvaluator.GetVariableInfo("MSE").ActualName = "ValidationQuality";
      validationEvaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      Counter evalCounter = new Counter();
      evalCounter.GetVariableInfo("Value").ActualName = "EvaluatedSolutions";
      Sorter sorter = new Sorter();
      sorter.GetVariableInfo("Descending").ActualName = "Maximization";
      sorter.GetVariableInfo("Value").ActualName = "Quality";

      seq.AddSubOperator(subScopesCreater);
      seq.AddSubOperator(subScopesProc);
      seq.AddSubOperator(sorter);

      subScopesProc.AddSubOperator(individualSeq);
      individualSeq.AddSubOperator(treeCreater);
      individualSeq.AddSubOperator(evaluator);
      individualSeq.AddSubOperator(validationEvaluator);
      individualSeq.AddSubOperator(evalCounter);

      init.OperatorGraph.AddOperator(seq);
      init.OperatorGraph.InitialOperator = seq;
      return init;
    }

    protected internal virtual IOperator CreateMainLoop() {
      CombinedOperator main = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      IOperator childCreater = CreateChildCreater();
      IOperator replacement = CreateReplacement();

      BestSolutionStorer solutionStorer = new BestSolutionStorer();
      solutionStorer.GetVariableInfo("Quality").ActualName = "ValidationQuality";
      solutionStorer.GetVariableInfo("BestSolution").ActualName = "BestValidationSolution";
      solutionStorer.AddSubOperator(CreateBestSolutionProcessor());

      BestAverageWorstQualityCalculator qualityCalculator = new BestAverageWorstQualityCalculator();
      BestAverageWorstQualityCalculator validationQualityCalculator = new BestAverageWorstQualityCalculator();
      validationQualityCalculator.Name = "BestAverageWorstValidationQualityCalculator";
      validationQualityCalculator.GetVariableInfo("Quality").ActualName = "ValidationQuality";
      validationQualityCalculator.GetVariableInfo("BestQuality").ActualName = "BestValidationQuality";
      validationQualityCalculator.GetVariableInfo("AverageQuality").ActualName = "AverageValidationQuality";
      validationQualityCalculator.GetVariableInfo("WorstQuality").ActualName = "WorstValidationQuality";
      IOperator loggingOperator = CreateLoggingOperator();
      Counter counter = new Counter();
      counter.GetVariableInfo("Value").ActualName = "Generations";
      IOperator loopCondition = CreateLoopCondition(seq);

      seq.AddSubOperator(childCreater);
      seq.AddSubOperator(replacement);
      seq.AddSubOperator(solutionStorer);
      seq.AddSubOperator(qualityCalculator);
      seq.AddSubOperator(validationQualityCalculator);
      seq.AddSubOperator(loggingOperator);
      seq.AddSubOperator(counter);
      seq.AddSubOperator(loopCondition);

      main.OperatorGraph.AddOperator(seq);
      main.OperatorGraph.InitialOperator = seq;
      return main;
    }

    protected internal virtual IOperator CreateLoggingOperator() {
      return new EmptyOperator();
    }

    protected internal virtual IOperator CreateLoopCondition(IOperator loop) {
      SequentialProcessor seq = new SequentialProcessor();
      seq.Name = "Loop Condition";
      LessThanComparator comparator = new LessThanComparator();
      comparator.GetVariableInfo("LeftSide").ActualName = "Generations";
      comparator.GetVariableInfo("RightSide").ActualName = "MaxGenerations";
      comparator.GetVariableInfo("Result").ActualName = "GenerationsCondition";
      ConditionalBranch cond = new ConditionalBranch();
      cond.GetVariableInfo("Condition").ActualName = "GenerationsCondition";

      seq.AddSubOperator(comparator);
      seq.AddSubOperator(cond);

      cond.AddSubOperator(loop);
      return seq;
    }

    protected internal virtual IOperator CreateBestSolutionProcessor() {
      return new EmptyOperator();
    }

    protected internal virtual IOperator CreateReplacement() {
      CombinedOperator replacement = new CombinedOperator();
      replacement.Name = "Replacement";
      SequentialProcessor seq = new SequentialProcessor();
      SequentialSubScopesProcessor seqScopeProc = new SequentialSubScopesProcessor();
      SequentialProcessor selectedProc = new SequentialProcessor();
      LeftSelector leftSelector = new LeftSelector();
      leftSelector.GetVariableInfo("Selected").ActualName = "Elites";
      RightReducer rightReducer = new RightReducer();

      SequentialProcessor remainingProc = new SequentialProcessor();
      RightSelector rightSelector = new RightSelector();
      rightSelector.GetVariableInfo("Selected").ActualName = "Elites";
      LeftReducer leftReducer = new LeftReducer();
      MergingReducer mergingReducer = new MergingReducer();
      Sorter sorter = new Sorter();
      sorter.GetVariableInfo("Descending").ActualName = "Maximization";
      sorter.GetVariableInfo("Value").ActualName = "Quality";

      seq.AddSubOperator(seqScopeProc);
      seqScopeProc.AddSubOperator(selectedProc);
      selectedProc.AddSubOperator(leftSelector);
      selectedProc.AddSubOperator(rightReducer);

      seqScopeProc.AddSubOperator(remainingProc);
      remainingProc.AddSubOperator(rightSelector);
      remainingProc.AddSubOperator(leftReducer);
      seq.AddSubOperator(mergingReducer);
      seq.AddSubOperator(sorter);
      replacement.OperatorGraph.AddOperator(seq);
      replacement.OperatorGraph.InitialOperator = seq;
      return replacement;
    }

    protected internal virtual IOperator CreateChildCreater() {
      CombinedOperator childCreater = new CombinedOperator();
      childCreater.Name = "Create children";
      SequentialProcessor seq = new SequentialProcessor();
      OperatorExtractor selector = new OperatorExtractor();
      selector.Name = "Selector (extr.)";
      selector.GetVariableInfo("Operator").ActualName = "Selector";

      SequentialSubScopesProcessor seqScopesProc = new SequentialSubScopesProcessor();
      EmptyOperator emptyOpt = new EmptyOperator();
      SequentialProcessor selectedProc = new SequentialProcessor();
      ChildrenInitializer childInitializer = new ChildrenInitializer();
      ((IntData)childInitializer.GetVariable("ParentsPerChild").Value).Data = 2;

      OperatorExtractor crossover = new OperatorExtractor();
      crossover.Name = "Crossover (extr.)";
      crossover.GetVariableInfo("Operator").ActualName = "Crossover";
      UniformSequentialSubScopesProcessor individualProc = new UniformSequentialSubScopesProcessor();
      SequentialProcessor individualSeqProc = new SequentialProcessor();
      StochasticBranch cond = new StochasticBranch();
      cond.GetVariableInfo("Probability").ActualName = "MutationRate";
      OperatorExtractor manipulator = new OperatorExtractor();
      manipulator.Name = "Manipulator (extr.)";
      manipulator.GetVariableInfo("Operator").ActualName = "Manipulator";
      OperatorExtractor evaluator = new OperatorExtractor();
      evaluator.Name = "Evaluator (extr.)";
      evaluator.GetVariableInfo("Operator").ActualName = "Evaluator";
      MeanSquaredErrorEvaluator validationEvaluator = new MeanSquaredErrorEvaluator();
      validationEvaluator.GetVariableInfo("MSE").ActualName = "ValidationQuality";
      validationEvaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      Counter evalCounter = new Counter();
      evalCounter.GetVariableInfo("Value").ActualName = "EvaluatedSolutions";
      SubScopesRemover parentRefRemover = new SubScopesRemover();

      Sorter sorter = new Sorter();
      sorter.GetVariableInfo("Descending").ActualName = "Maximization";
      sorter.GetVariableInfo("Value").ActualName = "Quality";


      seq.AddSubOperator(selector);
      seq.AddSubOperator(seqScopesProc);
      seqScopesProc.AddSubOperator(emptyOpt);
      seqScopesProc.AddSubOperator(selectedProc);
      selectedProc.AddSubOperator(childInitializer);
      selectedProc.AddSubOperator(individualProc);
      individualProc.AddSubOperator(individualSeqProc);
      individualSeqProc.AddSubOperator(crossover);
      individualSeqProc.AddSubOperator(cond);
      cond.AddSubOperator(manipulator);
      individualSeqProc.AddSubOperator(evaluator);
      individualSeqProc.AddSubOperator(validationEvaluator);
      individualSeqProc.AddSubOperator(evalCounter);
      individualSeqProc.AddSubOperator(parentRefRemover);
      selectedProc.AddSubOperator(sorter);

      childCreater.OperatorGraph.AddOperator(seq);
      childCreater.OperatorGraph.InitialOperator = seq;
      return childCreater;
    }

    protected internal virtual Model CreateGPModel(IScope bestModelScope) {
      Model model = new Model();
      Dataset ds = bestModelScope.GetVariableValue<Dataset>("Dataset", true);
      model.Data = bestModelScope.GetVariableValue<IFunctionTree>("FunctionTree", false);
      model.Dataset = ds;
      model.TargetVariable = ds.GetVariableName(bestModelScope.GetVariableValue<IntData>("TargetVariable", true).Data);
      model.TrainingMeanSquaredError = bestModelScope.GetVariableValue<DoubleData>("Quality", false).Data;
      model.ValidationMeanSquaredError = bestModelScope.GetVariableValue<DoubleData>("ValidationQuality", false).Data;
      // calculate and set variable impacts
      VariableEvaluationImpactCalculator evaluationImpactCalculator = new VariableEvaluationImpactCalculator();
      VariableQualityImpactCalculator qualityImpactCalculator = new VariableQualityImpactCalculator();

      evaluationImpactCalculator.Apply(bestModelScope);
      qualityImpactCalculator.Apply(bestModelScope);

      ItemList evaluationImpacts = bestModelScope.GetVariableValue<ItemList>("VariableEvaluationImpacts", false);
      ItemList qualityImpacts = bestModelScope.GetVariableValue<ItemList>("VariableQualityImpacts", false);
      foreach (ItemList row in evaluationImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[0]).Data;
        model.SetVariableEvaluationImpact(variableName, impact);
      }
      foreach (ItemList row in qualityImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[0]).Data;
        model.SetVariableQualityImpact(variableName, impact);
      }
      return model;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      AlgorithmBase clone = (AlgorithmBase)base.Clone(clonedObjects);
      clonedObjects.Add(Guid, clone);
      clone.engine = (SequentialEngine.SequentialEngine)Auxiliary.Clone(Engine, clonedObjects);
      return clone;
    }

    protected VariableInjector GetVariableInjector() {
      CombinedOperator co1 = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
      // SequentialProcessor in GP
      algorithm = (SequentialProcessor)co1.OperatorGraph.InitialOperator;
      return (VariableInjector)algorithm.SubOperators[2];
    }

    protected RandomInjector GetRandomInjector() {
      CombinedOperator co1 = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
      // SequentialProcessor in GP
      algorithm = (SequentialProcessor)co1.OperatorGraph.InitialOperator;
      return (RandomInjector)algorithm.SubOperators[1];
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Engine", Engine, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      engine = (SequentialEngine.SequentialEngine)PersistenceManager.Restore(node.SelectSingleNode("Engine"), restoredObjects);
    }
    #endregion


  }
}
