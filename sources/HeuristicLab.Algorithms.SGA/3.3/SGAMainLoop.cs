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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.SGA {
  /// <summary>
  /// An operator which represents the main loop of a standard genetic algorithm (SGA).
  /// </summary>
  [Item("SGAMainLoop", "An operator which represents the main loop of a standard genetic algorithm (SGA).")]
  [Creatable("Test")]
  [StorableClass]
  public sealed class SGAMainLoop : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public SubScopesLookupParameter<DoubleValue> QualityParameter {
      get { return (SubScopesLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public ValueLookupParameter<IOperator> SelectorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Selector"]; }
    }
    public ValueLookupParameter<IOperator> CrossoverParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Crossover"]; }
    }
    public ValueLookupParameter<DoubleValue> MutationProbabilityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MutationProbability"]; }
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
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private SGAMainLoop(bool deserializing) : base() { }
    public SGAMainLoop()
      : base() {
      Initialize();
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents a population of solutions on which the SGA should be applied."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      BestQualityMemorizer bestQualityMemorizer1 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator1 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector1 = new DataTableValuesCollector();
      ResultsCollector resultsCollector = new ResultsCollector();
      SubScopesSorter subScopesSorter1 = new SubScopesSorter();
      Placeholder selector = new Placeholder();
      SequentialSubScopesProcessor sequentialSubScopesProcessor1 = new SequentialSubScopesProcessor();
      ChildrenCreator childrenCreator = new ChildrenCreator();
      UniformSequentialSubScopesProcessor uniformSequentialSubScopesProcessor = new UniformSequentialSubScopesProcessor();
      Placeholder crossover = new Placeholder();
      StochasticBranch stochasticBranch = new StochasticBranch();
      Placeholder mutator = new Placeholder();
      Placeholder evaluator = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      SubScopesSorter subScopesSorter2 = new SubScopesSorter();
      SequentialSubScopesProcessor sequentialSubScopesProcessor2 = new SequentialSubScopesProcessor();
      LeftSelector leftSelector = new LeftSelector();
      RightReducer rightReducer = new RightReducer();
      MergingReducer mergingReducer = new MergingReducer();
      IntCounter intCounter = new IntCounter();
      Comparator comparator = new Comparator();
      BestQualityMemorizer bestQualityMemorizer2 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator2 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector2 = new DataTableValuesCollector();
      ConditionalBranch conditionalBranch = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));

      bestQualityMemorizer1.BestQualityParameter.ActualName = "Best Quality";
      bestQualityMemorizer1.MaximizationParameter.ActualName = "Maximization";
      bestQualityMemorizer1.QualityParameter.ActualName = "Quality";

      bestAverageWorstQualityCalculator1.AverageQualityParameter.ActualName = "Current Average Quality";
      bestAverageWorstQualityCalculator1.BestQualityParameter.ActualName = "Current Best Quality";
      bestAverageWorstQualityCalculator1.MaximizationParameter.ActualName = "Maximization";
      bestAverageWorstQualityCalculator1.QualityParameter.ActualName = "Quality";
      bestAverageWorstQualityCalculator1.WorstQualityParameter.ActualName = "Current Worst Quality";

      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, "BestKnownQuality"));
      dataTableValuesCollector1.DataTableParameter.ActualName = "Qualities";

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, "BestKnownQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
      resultsCollector.ResultsParameter.ActualName = "Results";

      subScopesSorter1.DescendingParameter.ActualName = "Maximization";
      subScopesSorter1.ValueParameter.ActualName = "Quality";

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

      subScopesSorter2.DescendingParameter.ActualName = "Maximization";
      subScopesSorter2.ValueParameter.ActualName = "Quality";

      leftSelector.CopySelected = new BoolValue(false);
      leftSelector.NumberOfSelectedSubScopesParameter.ActualName = "Elites";

      intCounter.Increment = new IntValue(1);
      intCounter.ValueParameter.ActualName = "Generations";

      comparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      comparator.LeftSideParameter.ActualName = "Generations";
      comparator.ResultParameter.ActualName = "Terminate";
      comparator.RightSideParameter.ActualName = "MaximumGenerations";

      bestQualityMemorizer2.BestQualityParameter.ActualName = "Best Quality";
      bestQualityMemorizer2.MaximizationParameter.ActualName = "Maximization";
      bestQualityMemorizer2.QualityParameter.ActualName = "Quality";

      bestAverageWorstQualityCalculator2.AverageQualityParameter.ActualName = "Current Average Quality";
      bestAverageWorstQualityCalculator2.BestQualityParameter.ActualName = "Current Best Quality";
      bestAverageWorstQualityCalculator2.MaximizationParameter.ActualName = "Maximization";
      bestAverageWorstQualityCalculator2.QualityParameter.ActualName = "Quality";
      bestAverageWorstQualityCalculator2.WorstQualityParameter.ActualName = "Current Worst Quality";

      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, "BestKnownQuality"));
      dataTableValuesCollector2.DataTableParameter.ActualName = "Qualities";

      conditionalBranch.ConditionParameter.ActualName = "Terminate";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = bestQualityMemorizer1;
      bestQualityMemorizer1.Successor = bestAverageWorstQualityCalculator1;
      bestAverageWorstQualityCalculator1.Successor = dataTableValuesCollector1;
      dataTableValuesCollector1.Successor = resultsCollector;
      resultsCollector.Successor = subScopesSorter1;
      subScopesSorter1.Successor = selector;
      selector.Successor = sequentialSubScopesProcessor1;
      sequentialSubScopesProcessor1.Operators.Add(new EmptyOperator());
      sequentialSubScopesProcessor1.Operators.Add(childrenCreator);
      sequentialSubScopesProcessor1.Successor = sequentialSubScopesProcessor2;
      childrenCreator.Successor = uniformSequentialSubScopesProcessor;
      uniformSequentialSubScopesProcessor.Operator = crossover;
      uniformSequentialSubScopesProcessor.Successor = subScopesSorter2;
      crossover.Successor = stochasticBranch;
      stochasticBranch.FirstBranch = mutator;
      stochasticBranch.SecondBranch = null;
      stochasticBranch.Successor = evaluator;
      mutator.Successor = null;
      evaluator.Successor = subScopesRemover;
      subScopesRemover.Successor = null;
      subScopesSorter2.Successor = null;
      sequentialSubScopesProcessor2.Operators.Add(leftSelector);
      sequentialSubScopesProcessor2.Operators.Add(new EmptyOperator());
      sequentialSubScopesProcessor2.Successor = mergingReducer;
      leftSelector.Successor = rightReducer;
      rightReducer.Successor = null;
      mergingReducer.Successor = intCounter;
      intCounter.Successor = comparator;
      comparator.Successor = bestQualityMemorizer2;
      bestQualityMemorizer2.Successor = bestAverageWorstQualityCalculator2;
      bestAverageWorstQualityCalculator2.Successor = dataTableValuesCollector2;
      dataTableValuesCollector2.Successor = conditionalBranch;
      conditionalBranch.FalseBranch = subScopesSorter1;
      conditionalBranch.TrueBranch = null;
      conditionalBranch.Successor = null;
      #endregion
    }
  }
}
