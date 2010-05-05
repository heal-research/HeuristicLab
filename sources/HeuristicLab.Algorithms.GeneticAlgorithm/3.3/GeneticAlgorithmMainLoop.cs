#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.GeneticAlgorithm {
  /// <summary>
  /// An operator which represents the main loop of a genetic algorithm.
  /// </summary>
  [Item("GeneticAlgorithmMainLoop", "An operator which represents the main loop of a genetic algorithm.")]
  [StorableClass]
  public sealed class GeneticAlgorithmMainLoop : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<IOperator> SelectorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Selector"]; }
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
    public ValueLookupParameter<IntValue> ElitesParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["Elites"]; }
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
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private GeneticAlgorithmMainLoop(bool deserializing) : base() { }
    public GeneticAlgorithmMainLoop()
      : base() {
      Initialize();
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze each generation."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents a population of solutions on which the genetic algorithm should be applied."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      Placeholder analyzer1 = new Placeholder();
      Placeholder selector = new Placeholder();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      ChildrenCreator childrenCreator = new ChildrenCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor = new UniformSubScopesProcessor();
      Placeholder crossover = new Placeholder();
      StochasticBranch stochasticBranch = new StochasticBranch();
      Placeholder mutator = new Placeholder();
      Placeholder evaluator = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      SubScopesProcessor subScopesProcessor2 = new SubScopesProcessor();
      BestSelector bestSelector = new BestSelector();
      RightReducer rightReducer = new RightReducer();
      MergingReducer mergingReducer = new MergingReducer();
      IntCounter intCounter = new IntCounter();
      Comparator comparator = new Comparator();
      ResultsCollector resultsCollector2 = new ResultsCollector();
      Placeholder analyzer2 = new Placeholder();
      ConditionalBranch conditionalBranch = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));

      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector1.ResultsParameter.ActualName = "Results";

      analyzer1.Name = "Analyzer";
      analyzer1.OperatorParameter.ActualName = "Analyzer";

      selector.Name = "Selector";
      selector.OperatorParameter.ActualName = "Selector";

      childrenCreator.ParentsPerChild = new IntValue(2);

      crossover.Name = "Crossover";
      crossover.OperatorParameter.ActualName = "Crossover";

      stochasticBranch.ProbabilityParameter.ActualName = "MutationProbability";
      stochasticBranch.RandomParameter.ActualName = "Random";

      mutator.Name = "Mutator";
      mutator.OperatorParameter.ActualName = "Mutator";

      evaluator.Name = "Evaluator";
      evaluator.OperatorParameter.ActualName = "Evaluator";

      subScopesRemover.RemoveAllSubScopes = true;

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = "Maximization";
      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = "Elites";
      bestSelector.QualityParameter.ActualName = "Quality";

      intCounter.Increment = new IntValue(1);
      intCounter.ValueParameter.ActualName = "Generations";

      comparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      comparator.LeftSideParameter.ActualName = "Generations";
      comparator.ResultParameter.ActualName = "Terminate";
      comparator.RightSideParameter.ActualName = "MaximumGenerations";

      resultsCollector2.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector2.ResultsParameter.ActualName = "Results";

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
      subScopesProcessor1.Successor = subScopesProcessor2;
      childrenCreator.Successor = uniformSubScopesProcessor;
      uniformSubScopesProcessor.Operator = crossover;
      uniformSubScopesProcessor.Successor = null;
      crossover.Successor = stochasticBranch;
      stochasticBranch.FirstBranch = mutator;
      stochasticBranch.SecondBranch = null;
      stochasticBranch.Successor = evaluator;
      mutator.Successor = null;
      evaluator.Successor = subScopesRemover;
      subScopesRemover.Successor = null;
      subScopesProcessor2.Operators.Add(bestSelector);
      subScopesProcessor2.Operators.Add(new EmptyOperator());
      subScopesProcessor2.Successor = mergingReducer;
      bestSelector.Successor = rightReducer;
      rightReducer.Successor = null;
      mergingReducer.Successor = intCounter;
      intCounter.Successor = comparator;
      comparator.Successor = resultsCollector2;
      resultsCollector2.Successor = analyzer2;
      analyzer2.Successor = conditionalBranch;
      conditionalBranch.FalseBranch = selector;
      conditionalBranch.TrueBranch = null;
      conditionalBranch.Successor = null;
      #endregion
    }
  }
}
