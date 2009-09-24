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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Evolutionary;
using HeuristicLab.Logging;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Selection;
using HeuristicLab.GP.Operators;

namespace HeuristicLab.GP.Algorithms {
  public abstract class AlgorithmBase : ItemBase {
    public virtual string Name { get { return "GP"; } }
    public virtual string Description { get { return "TODO"; } }

    public virtual double MutationRate {
      get { return GetVariableInjector().GetVariable("MutationRate").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("MutationRate").GetValue<DoubleData>().Data = value; }
    }
    public virtual int PopulationSize {
      get { return GetVariableInjector().GetVariable("PopulationSize").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("PopulationSize").GetValue<IntData>().Data = value; }
    }

    public virtual int MaxGenerations {
      get { return GetVariableInjector().GetVariable("MaxGenerations").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxGenerations").GetValue<IntData>().Data = value; }
    }

    public virtual int Elites {
      get { return GetVariableInjector().GetVariable("Elites").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("Elites").GetValue<IntData>().Data = value; }
    }

    public virtual int MaxTreeSize {
      get { return GetVariableInjector().GetVariable("MaxTreeSize").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxTreeSize").GetValue<IntData>().Data = value; }
    }

    public virtual int MinTreeSize {
      get { return GetVariableInjector().GetVariable("MinTreeSize").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MinTreeSize").GetValue<IntData>().Data = value; }
    }

    public virtual int MaxTreeHeight {
      get { return GetVariableInjector().GetVariable("MaxTreeHeight").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxTreeHeight").GetValue<IntData>().Data = value; }
    }

    public virtual int Parents {
      get { return GetVariableInjector().GetVariable("Parents").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("Parents").GetValue<IntData>().Data = value; }
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
      get { return GetInitializationOperator().SubOperators[0]; }
      set {
        value.Name = "ProblemInjector";
        IOperator init = GetInitializationOperator();
        init.RemoveSubOperator(0);
        init.AddSubOperator(value, 0);
      }
    }

    public virtual IOperator FunctionLibraryInjector {
      get { return GetInitializationOperator().SubOperators[1]; }
      set {
        value.Name = "FunctionLibraryInjector";
        IOperator init = GetInitializationOperator();
        init.RemoveSubOperator(1);
        init.AddSubOperator(value, 1);
      }
    }

    private IEngine engine;
    public IEngine Engine {
      get { return engine; }
      protected set { engine = value; }
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
      MaxGenerations = 100;
      MaxTreeSize = 100;
      MinTreeSize = 1;
      MaxTreeHeight = 10;
      Parents = 2000;
    }

    protected internal virtual CombinedOperator CreateAlgorithm() {
      CombinedOperator algo = new CombinedOperator();
      algo.Name = Name;
      SequentialProcessor seq = new SequentialProcessor();
      seq.Name = Name;
      IOperator init = CreateInitializationOperator();
      init.AddSubOperator(CreateProblemInjector());
      init.AddSubOperator(CreateFunctionLibraryInjector());
      seq.AddSubOperator(init);

      IOperator initPopulation = CreateInitialPopulationGenerator();
      initPopulation.AddSubOperator(CreateRandomSolutionGenerator());
      initPopulation.AddSubOperator(CreateInitialPopulationEvaluator());
      seq.AddSubOperator(initPopulation);

      IOperator mainLoop = CreateMainLoop();
      mainLoop.AddSubOperator(CreateSelectionOperator());
      mainLoop.AddSubOperator(CreateCrossoverOperator());
      mainLoop.AddSubOperator(CreateManipulationOperator());
      mainLoop.AddSubOperator(CreateEvaluationOperator());
      mainLoop.AddSubOperator(CreateTerminationCondition());
      seq.AddSubOperator(mainLoop);

      IOperator postProcess = CreatePostProcessingOperator();
      seq.AddSubOperator(postProcess);

      algo.OperatorGraph.AddOperator(seq);
      algo.OperatorGraph.InitialOperator = seq;
      return algo;
    }

    #region global init
    protected virtual IOperator CreateInitializationOperator() {
      CombinedOperator init = new CombinedOperator();
      init.Name = "Initialization";
      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(CreateRandomInjector());
      seq.AddSubOperator(CreateGlobalInjector());

      OperatorExtractor probInjectorExtractor = new OperatorExtractor();
      probInjectorExtractor.Name = "ProblemInjector";
      probInjectorExtractor.GetVariableInfo("Operator").ActualName = "ProblemInjector";
      seq.AddSubOperator(probInjectorExtractor);

      OperatorExtractor funLibInjectorExtractor = new OperatorExtractor();
      funLibInjectorExtractor.Name = "FunctionLibraryInjector";
      funLibInjectorExtractor.GetVariableInfo("Operator").ActualName = "FunctionLibraryInjector";
      seq.AddSubOperator(funLibInjectorExtractor);

      init.OperatorGraph.AddOperator(seq);
      init.OperatorGraph.InitialOperator = seq;
      return init;
    }

    protected virtual IOperator CreateRandomInjector() {
      RandomInjector randomInjector = new RandomInjector();
      randomInjector.Name = "Random Injector";
      return randomInjector;
    }

    protected virtual VariableInjector CreateGlobalInjector() {
      VariableInjector injector = new VariableInjector();
      injector.Name = "Global Injector";
      injector.AddVariable(new HeuristicLab.Core.Variable("Generations", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxGenerations", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("MutationRate", new DoubleData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("PopulationSize", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("Elites", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("Maximization", new BoolData(false)));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxTreeHeight", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxTreeSize", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("MinTreeSize", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("EvaluatedSolutions", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("TotalEvaluatedNodes", new DoubleData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("Parents", new IntData()));
      return injector;
    }


    protected virtual IOperator CreateProblemInjector() {
      return new EmptyOperator();
    }

    protected virtual IOperator CreateFunctionLibraryInjector() {
      return new EmptyOperator();
    }
    #endregion

    #region population init
    private IOperator CreateInitialPopulationGenerator() {
      CombinedOperator initPopulation = new CombinedOperator();
      initPopulation.Name = "Init population";
      SequentialProcessor seq = new SequentialProcessor();
      SubScopesCreater subScopesCreater = new SubScopesCreater();
      subScopesCreater.GetVariableInfo("SubScopes").ActualName = "PopulationSize";
      UniformSequentialSubScopesProcessor subScopesProc = new UniformSequentialSubScopesProcessor();
      SequentialProcessor individualSeq = new SequentialProcessor();
      OperatorExtractor treeCreater = new OperatorExtractor();
      treeCreater.Name = "Tree generator (extr.)";
      treeCreater.GetVariableInfo("Operator").ActualName = "Solution generator";

      OperatorExtractor evaluator = new OperatorExtractor();
      evaluator.Name = "Evaluator (extr.)";
      evaluator.GetVariableInfo("Operator").ActualName = "Evaluator";
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
      individualSeq.AddSubOperator(evalCounter);

      initPopulation.OperatorGraph.AddOperator(seq);
      initPopulation.OperatorGraph.InitialOperator = seq;
      return initPopulation;
    }

    protected virtual IOperator CreateRandomSolutionGenerator() {
      ProbabilisticTreeCreator treeCreator = new ProbabilisticTreeCreator();
      treeCreator.Name = "Solution generator";
      return treeCreator;
    }

    protected virtual IOperator CreateInitialPopulationEvaluator() {
      return new EmptyOperator();
    }
    #endregion

    #region mainloop
    protected virtual IOperator CreateMainLoop() {
      CombinedOperator main = new CombinedOperator();
      main.Name = "Main";
      SequentialProcessor seq = new SequentialProcessor();
      IOperator childCreater = CreateChildCreater();
      IOperator replacement = CreateReplacement();

      BestAverageWorstQualityCalculator qualityCalculator = new BestAverageWorstQualityCalculator();
      IOperator loggingOperator = CreateLoggingOperator();
      Counter counter = new Counter();
      counter.GetVariableInfo("Value").ActualName = "Generations";

      OperatorExtractor terminationCriterionExtractor = new OperatorExtractor();
      terminationCriterionExtractor.Name = "TerminationCondition (extr.)";
      terminationCriterionExtractor.GetVariableInfo("Operator").ActualName = "TerminationCondition";

      ConditionalBranch loop = new ConditionalBranch();
      loop.Name = "Main loop";
      loop.GetVariableInfo("Condition").ActualName = "TerminationCriterion";
      loop.AddSubOperator(new EmptyOperator());
      loop.AddSubOperator(seq);

      seq.AddSubOperator(childCreater);
      seq.AddSubOperator(replacement);
      seq.AddSubOperator(qualityCalculator);
      seq.AddSubOperator(CreateGenerationStepHook());
      seq.AddSubOperator(loggingOperator);
      seq.AddSubOperator(counter);
      seq.AddSubOperator(terminationCriterionExtractor);
      seq.AddSubOperator(loop);

      main.OperatorGraph.AddOperator(seq);
      main.OperatorGraph.InitialOperator = seq;
      return main;
    }

    protected virtual IOperator CreateChildCreater() {
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
      individualSeqProc.AddSubOperator(evalCounter);
      individualSeqProc.AddSubOperator(parentRefRemover);
      selectedProc.AddSubOperator(sorter);

      childCreater.OperatorGraph.AddOperator(seq);
      childCreater.OperatorGraph.InitialOperator = seq;
      return childCreater;
    }

    protected virtual IOperator CreateReplacement() {
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

    protected virtual IOperator CreateLoggingOperator() {
      CombinedOperator loggingOperator = new CombinedOperator();
      loggingOperator.Name = "Logging";
      SequentialProcessor seq = new SequentialProcessor();

      DataCollector collector = new DataCollector();
      ItemList<StringData> names = collector.GetVariable("VariableNames").GetValue<ItemList<StringData>>();
      names.Add(new StringData("BestQuality"));
      names.Add(new StringData("AverageQuality"));
      names.Add(new StringData("WorstQuality"));
      LinechartInjector lineChartInjector = new LinechartInjector();
      lineChartInjector.GetVariableInfo("Linechart").ActualName = "Quality Linechart";
      lineChartInjector.GetVariable("NumberOfLines").GetValue<IntData>().Data = 3;
      QualityLogger qualityLogger = new QualityLogger();

      seq.AddSubOperator(collector);
      seq.AddSubOperator(lineChartInjector);
      seq.AddSubOperator(qualityLogger);

      loggingOperator.OperatorGraph.AddOperator(seq);
      loggingOperator.OperatorGraph.InitialOperator = seq;
      return loggingOperator;
    }

    protected virtual IOperator CreateGenerationStepHook() {
      return new EmptyOperator();
    }

    protected virtual IOperator CreateTerminationCondition() {
      CombinedOperator terminationCriterion = new CombinedOperator();
      terminationCriterion.Name = "TerminationCondition";
      SequentialProcessor seq = new SequentialProcessor();
      GreaterThanComparator comparator = new GreaterThanComparator();
      comparator.GetVariableInfo("LeftSide").ActualName = "Generations";
      comparator.GetVariableInfo("RightSide").ActualName = "MaxGenerations";
      comparator.GetVariableInfo("Result").ActualName = "TerminationCriterion";

      seq.AddSubOperator(comparator);
      terminationCriterion.OperatorGraph.AddOperator(seq);
      terminationCriterion.OperatorGraph.InitialOperator = seq;
      return terminationCriterion;
    }

    protected virtual IOperator CreateEvaluationOperator() {
      return new EmptyOperator();
    }

    protected virtual IOperator CreateManipulationOperator() {
      ChangeNodeTypeManipulation manipulator = new ChangeNodeTypeManipulation();
      manipulator.Name = "Manipulator";
      return manipulator;
    }

    protected virtual IOperator CreateCrossoverOperator() {
      StandardCrossOver crossover = new StandardCrossOver();
      crossover.Name = "Crossover";
      return crossover;
    }

    protected virtual IOperator CreateSelectionOperator() {
      TournamentSelector selector = new TournamentSelector();
      selector.GetVariableInfo("Selected").ActualName = "Parents";
      selector.GetVariable("GroupSize").Value = new IntData(7);
      selector.Name = "Selector";
      return selector;
    }

    #endregion

    protected virtual IOperator CreatePostProcessingOperator() {
      return new EmptyOperator();
    }

    protected virtual IOperator GetVariableInjector() {
      CombinedOperator init = (CombinedOperator)GetInitializationOperator();
      return init.OperatorGraph.InitialOperator.SubOperators[1];
    }

    protected virtual IOperator GetRandomInjector() {
      CombinedOperator init = (CombinedOperator)GetInitializationOperator();
      return init.OperatorGraph.InitialOperator.SubOperators[0];
    }

    protected virtual IOperator GetInitializationOperator() {
      CombinedOperator algo = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
      return algo.OperatorGraph.InitialOperator.SubOperators[0];
    }

    #region Persistence Methods
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      AlgorithmBase clone = (AlgorithmBase)base.Clone(clonedObjects);
      clone.engine = (IEngine)Auxiliary.Clone(Engine, clonedObjects);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Engine", Engine, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      engine = (IEngine)PersistenceManager.Restore(node.SelectSingleNode("Engine"), restoredObjects);
    }
    #endregion


    public static IOperator CombineTerminationCriterions(IOperator criterion1, IOperator criterion2) {
      ConditionalBranch branch = new ConditionalBranch();
      branch.GetVariableInfo("Condition").ActualName = "TerminationCriterion";
      branch.AddSubOperator(new EmptyOperator());
      branch.AddSubOperator(criterion2);

      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(criterion1);
      seq.AddSubOperator(branch);

      return seq;
    }
  }
}
