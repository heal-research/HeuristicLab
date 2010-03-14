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
using HeuristicLab.Evolutionary;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.SGA {
  /// <summary>
  /// An operator which represents a Standard Genetic Algorithm.
  /// </summary>
  [Item("SGAOperator", "An operator which represents a Standard Genetic Algorithm.")]
  [Creatable("Test")]
  [StorableClass]
  public class SGAOperator : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolData> MaximizationParameter {
      get { return (ValueLookupParameter<BoolData>)Parameters["Maximization"]; }
    }
    public SubScopesLookupParameter<DoubleData> QualityParameter {
      get { return (SubScopesLookupParameter<DoubleData>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<IOperator> SelectorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Selector"]; }
    }
    public ValueLookupParameter<IOperator> CrossoverParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Crossover"]; }
    }
    public ValueLookupParameter<DoubleData> MutationProbabilityParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["MutationProbability"]; }
    }
    public ValueLookupParameter<IOperator> MutatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Mutator"]; }
    }
    public ValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    public ValueLookupParameter<IntData> ElitesParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["Elites"]; }
    }
    public ValueLookupParameter<IntData> MaximumGenerationsParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["MaximumGenerations"]; }
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

    public SGAOperator()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolData>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new SubScopesLookupParameter<DoubleData>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleData>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueLookupParameter<IntData>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<IntData>("MaximumGenerations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents a population of solutions on which the SGA should be applied."));
      #endregion

      #region Create operator graph
      VariableCreator variableCreator = new VariableCreator();
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
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector = new DataTableValuesCollector();
      ConditionalBranch conditionalBranch = new ConditionalBranch();

      OperatorGraph.InitialOperator = variableCreator;

      variableCreator.CollectedValues.Add(new ValueParameter<IntData>("Generations", new IntData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleData>("Best Quality", new DoubleData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleData>("Average Quality", new DoubleData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleData>("Worst Quality", new DoubleData(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DataTable>("Qualities", new DataTable("Qualities")));
      variableCreator.Successor = resultsCollector;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntData>("Generations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Best Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Average Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Worst Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
      resultsCollector.ResultsParameter.ActualName = "Results";
      resultsCollector.Successor = subScopesSorter1;

      subScopesSorter1.DescendingParameter.ActualName = "Maximization";
      subScopesSorter1.ValueParameter.ActualName = "Quality";
      subScopesSorter1.Successor = selector;

      selector.Name = "Selector";
      selector.OperatorParameter.ActualName = "Selector";
      selector.Successor = sequentialSubScopesProcessor1;

      sequentialSubScopesProcessor1.Operators.Add(new EmptyOperator());
      sequentialSubScopesProcessor1.Operators.Add(childrenCreator);
      sequentialSubScopesProcessor1.Successor = sequentialSubScopesProcessor2;

      childrenCreator.ParentsPerChild = new IntData(2);
      childrenCreator.Successor = uniformSequentialSubScopesProcessor;

      uniformSequentialSubScopesProcessor.Operator = crossover;
      uniformSequentialSubScopesProcessor.Successor = subScopesSorter2;

      crossover.Name = "Crossover";
      crossover.OperatorParameter.ActualName = "Crossover";
      crossover.Successor = stochasticBranch;

      stochasticBranch.FirstBranch = mutator;
      stochasticBranch.ProbabilityParameter.ActualName = "MutationProbability";
      stochasticBranch.RandomParameter.ActualName = "Random";
      stochasticBranch.SecondBranch = null;
      stochasticBranch.Successor = evaluator;

      mutator.Name = "Mutator";
      mutator.OperatorParameter.ActualName = "Mutator";
      mutator.Successor = null;

      evaluator.Name = "Evaluator";
      evaluator.OperatorParameter.ActualName = "Evaluator";
      evaluator.Successor = subScopesRemover;

      subScopesRemover.RemoveAllSubScopes = true;
      subScopesRemover.Successor = null;

      subScopesSorter2.DescendingParameter.ActualName = "Maximization";
      subScopesSorter2.ValueParameter.ActualName = "Quality";
      subScopesSorter2.Successor = null;

      sequentialSubScopesProcessor2.Operators.Add(leftSelector);
      sequentialSubScopesProcessor2.Operators.Add(new EmptyOperator());
      sequentialSubScopesProcessor2.Successor = mergingReducer;

      leftSelector.CopySelected = new BoolData(false);
      leftSelector.NumberOfSelectedSubScopesParameter.ActualName = "Elites";
      leftSelector.Successor = rightReducer;

      rightReducer.Successor = null;

      mergingReducer.Successor = intCounter;

      intCounter.Increment = new IntData(1);
      intCounter.ValueParameter.ActualName = "Generations";
      intCounter.Successor = comparator;

      comparator.Comparison = new ComparisonData(Comparison.GreaterOrEqual);
      comparator.LeftSideParameter.ActualName = "Generations";
      comparator.ResultParameter.ActualName = "Terminate";
      comparator.RightSideParameter.ActualName = "MaximumGenerations";
      comparator.Successor = bestAverageWorstQualityCalculator;

      bestAverageWorstQualityCalculator.AverageQualityParameter.ActualName = "Average Quality";
      bestAverageWorstQualityCalculator.BestQualityParameter.ActualName = "Best Quality";
      bestAverageWorstQualityCalculator.MaximizationParameter.ActualName = "Maximization";
      bestAverageWorstQualityCalculator.QualityParameter.ActualName = "Quality";
      bestAverageWorstQualityCalculator.WorstQualityParameter.ActualName = "Worst Quality";
      bestAverageWorstQualityCalculator.Successor = dataTableValuesCollector;

      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Best Quality"));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Average Quality"));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleData>("Worst Quality"));
      dataTableValuesCollector.DataTableParameter.ActualName = "Qualities";
      dataTableValuesCollector.Successor = conditionalBranch;

      conditionalBranch.ConditionParameter.ActualName = "Terminate";
      conditionalBranch.FalseBranch = subScopesSorter1;
      conditionalBranch.TrueBranch = null;
      conditionalBranch.Successor = null;
      #endregion
    }
  }
}
