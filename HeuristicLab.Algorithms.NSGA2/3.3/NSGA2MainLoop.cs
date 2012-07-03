#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.NSGA2 {
  /// <summary>
  /// An operator that represents the mainloop of the NSGA-II
  /// </summary>
  [Item("NSGA2MainLoop", "An operator which represents the main loop of the NSGA-II algorithm.")]
  [StorableClass]
  public class NSGA2MainLoop : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolArray> MaximizationParameter {
      get { return (ValueLookupParameter<BoolArray>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleArray> QualitiesParameter {
      get { return (ScopeTreeLookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }
    public ValueLookupParameter<IntValue> PopulationSizeParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    public ValueLookupParameter<IOperator> SelectorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Selector"]; }
    }
    public ValueLookupParameter<PercentValue> CrossoverProbabilityParameter {
      get { return (ValueLookupParameter<PercentValue>)Parameters["CrossoverProbability"]; }
    }
    public ValueLookupParameter<IOperator> CrossoverParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Crossover"]; }
    }
    public ValueLookupParameter<PercentValue> MutationProbabilityParameter {
      get { return (ValueLookupParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    public ValueLookupParameter<IOperator> MutatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Mutator"]; }
    }
    public ValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    public ValueLookupParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    public LookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (LookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    #endregion

    [StorableConstructor]
    protected NSGA2MainLoop(bool deserializing) : base(deserializing) { }
    protected NSGA2MainLoop(NSGA2MainLoop original, Cloner cloner) : base(original, cloner) { }
    public NSGA2MainLoop()
      : base() {
      Initialize();
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolArray>("Maximization", "True if an objective should be maximized, or false if it should be minimized."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("Qualities", "The vector of quality values."));
      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The population size."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("CrossoverProbability", "The probability that the crossover operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze each generation."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of times solutions have been evaluated."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      Placeholder analyzer1 = new Placeholder();
      Placeholder selector = new Placeholder();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      ChildrenCreator childrenCreator = new ChildrenCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      StochasticBranch crossoverStochasticBranch = new StochasticBranch();
      Placeholder crossover = new Placeholder();
      ParentCopyCrossover noCrossover = new ParentCopyCrossover();
      StochasticBranch mutationStochasticBranch = new StochasticBranch();
      Placeholder mutator = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      UniformSubScopesProcessor uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      Placeholder evaluator = new Placeholder();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      MergingReducer mergingReducer = new MergingReducer();
      RankAndCrowdingSorter rankAndCrowdingSorter = new RankAndCrowdingSorter();
      LeftSelector leftSelector = new LeftSelector();
      RightReducer rightReducer = new RightReducer();
      IntCounter intCounter = new IntCounter();
      Comparator comparator = new Comparator();
      Placeholder analyzer2 = new Placeholder();
      ConditionalBranch conditionalBranch = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));

      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      analyzer1.Name = "Analyzer";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      selector.Name = "Selector";
      selector.OperatorParameter.ActualName = SelectorParameter.Name;

      childrenCreator.ParentsPerChild = new IntValue(2);

      crossoverStochasticBranch.ProbabilityParameter.ActualName = CrossoverProbabilityParameter.Name;
      crossoverStochasticBranch.RandomParameter.ActualName = RandomParameter.Name;

      crossover.Name = "Crossover";
      crossover.OperatorParameter.ActualName = CrossoverParameter.Name;

      noCrossover.Name = "Clone parent";
      noCrossover.RandomParameter.ActualName = RandomParameter.Name;

      mutationStochasticBranch.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mutationStochasticBranch.RandomParameter.ActualName = RandomParameter.Name;

      mutator.Name = "Mutator";
      mutator.OperatorParameter.ActualName = MutatorParameter.Name;

      subScopesRemover.RemoveAllSubScopes = true;

      uniformSubScopesProcessor2.Parallel.Value = true;

      evaluator.Name = "Evaluator";
      evaluator.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesCounter.Name = "Increment EvaluatedSolutions";
      subScopesCounter.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      rankAndCrowdingSorter.CrowdingDistanceParameter.ActualName = "CrowdingDistance";
      rankAndCrowdingSorter.RankParameter.ActualName = "Rank";

      leftSelector.CopySelected = new BoolValue(false);
      leftSelector.NumberOfSelectedSubScopesParameter.ActualName = PopulationSizeParameter.Name;

      intCounter.Increment = new IntValue(1);
      intCounter.ValueParameter.ActualName = "Generations";

      comparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      comparator.LeftSideParameter.ActualName = "Generations";
      comparator.ResultParameter.ActualName = "Terminate";
      comparator.RightSideParameter.ActualName = MaximumGenerationsParameter.Name;

      analyzer2.Name = "Analyzer";
      analyzer2.OperatorParameter.ActualName = "Analyzer";

      conditionalBranch.ConditionParameter.ActualName = "Terminate";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = resultsCollector1;
      resultsCollector1.Successor = analyzer1;
      analyzer1.Successor = selector;
      selector.Successor = subScopesProcessor1;
      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(childrenCreator);
      subScopesProcessor1.Successor = mergingReducer;
      childrenCreator.Successor = uniformSubScopesProcessor1;
      uniformSubScopesProcessor1.Operator = crossoverStochasticBranch;
      uniformSubScopesProcessor1.Successor = uniformSubScopesProcessor2;
      crossoverStochasticBranch.FirstBranch = crossover;
      crossoverStochasticBranch.SecondBranch = noCrossover;
      crossoverStochasticBranch.Successor = mutationStochasticBranch;
      crossover.Successor = null;
      noCrossover.Successor = null;
      mutationStochasticBranch.FirstBranch = mutator;
      mutationStochasticBranch.SecondBranch = null;
      mutationStochasticBranch.Successor = subScopesRemover;
      mutator.Successor = null;
      subScopesRemover.Successor = null;
      uniformSubScopesProcessor2.Operator = evaluator;
      uniformSubScopesProcessor2.Successor = subScopesCounter;
      evaluator.Successor = null;
      subScopesCounter.Successor = null;
      mergingReducer.Successor = rankAndCrowdingSorter;
      rankAndCrowdingSorter.Successor = leftSelector;
      leftSelector.Successor = rightReducer;
      rightReducer.Successor = intCounter;
      intCounter.Successor = comparator;
      comparator.Successor = analyzer2;
      analyzer2.Successor = conditionalBranch;
      conditionalBranch.FalseBranch = selector;
      conditionalBranch.TrueBranch = null;
      conditionalBranch.Successor = null;
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NSGA2MainLoop(this, cloner);
    }
  }
}
