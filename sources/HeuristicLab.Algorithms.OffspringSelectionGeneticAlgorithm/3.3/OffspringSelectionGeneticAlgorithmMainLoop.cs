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

namespace HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm {
  /// <summary>
  /// An operator which represents the main loop of an offspring selection genetic algorithm.
  /// </summary>
  [Item("OffspringSelectionGeneticAlgorithmMainLoop", "An operator which represents the main loop of an offspring selection genetic algorithm.")]
  [StorableClass]
  public sealed class OffspringSelectionGeneticAlgorithmMainLoop : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ValueLookupParameter<IntValue> PopulationSizeParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["PopulationSize"]; }
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
    public ValueLookupParameter<IOperator> VisualizerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Visualizer"]; }
    }
    public LookupParameter<IItem> VisualizationParameter {
      get { return (LookupParameter<IItem>)Parameters["Visualization"]; }
    }
    public ValueLookupParameter<DoubleValue> SuccessRatioParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    public ValueLookupParameter<DoubleValue> ComparisonFactorLowerBoundParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorLowerBound"]; }
    }
    public ValueLookupParameter<DoubleValue> ComparisonFactorUpperBoundParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorUpperBound"]; }
    }
    public ValueLookupParameter<IOperator> ComparisonFactorModifierParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["ComparisonFactorModifier"]; }
    }
    public ValueLookupParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    public ValueLookupParameter<BoolValue> OffspringSelectionBeforeMutationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["OffspringSelectionBeforeMutation"]; }
    }
    #endregion

    [StorableConstructor]
    private OffspringSelectionGeneticAlgorithmMainLoop(bool deserializing) : base() { }
    public OffspringSelectionGeneticAlgorithmMainLoop()
      : base() {
      Initialize();
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The size of the population."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Visualizer", "The operator used to visualize solutions."));
      Parameters.Add(new LookupParameter<IItem>("Visualization", "The item which represents the visualization of solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorLowerBound", "The lower bound of the comparison factor (start)."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorUpperBound", "The upper bound of the comparison factor (end)."));
      Parameters.Add(new ValueLookupParameter<IOperator>("ComparisonFactorModifier", "The operator used to modify the comparison factor."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      Assigner variableAssigner = new Assigner();
      BestQualityMemorizer bestQualityMemorizer1 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer2 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator1 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector1 = new DataTableValuesCollector();
      DataTableValuesCollector selPressDataTableValuesCollector1 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      Placeholder visualizer1 = new Placeholder();
      ResultsCollector resultsCollector = new ResultsCollector();
      Placeholder selector = new Placeholder();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      ChildrenCreator childrenCreator = new ChildrenCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor = new UniformSubScopesProcessor();
      Placeholder crossover = new Placeholder();
      ConditionalBranch osBeforeMutationBranch = new ConditionalBranch();
      Placeholder evaluator1 = new Placeholder();
      WeightedParentsQualityComparator qualityComparer1 = new WeightedParentsQualityComparator();
      StochasticBranch mutationBranch1 = new StochasticBranch();
      Placeholder mutator1 = new Placeholder();
      Placeholder evaluator2 = new Placeholder();
      StochasticBranch mutationBranch2 = new StochasticBranch();
      Placeholder mutator2 = new Placeholder();
      Placeholder evaluator3 = new Placeholder();
      WeightedParentsQualityComparator qualityComparer2 = new WeightedParentsQualityComparator();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      ConditionalSelector conditionalSelector = new ConditionalSelector();
      OffspringSelector offspringSelector = new OffspringSelector();
      SubScopesProcessor subScopesProcessor2 = new SubScopesProcessor();
      BestSelector bestSelector = new BestSelector();
      RightReducer rightReducer = new RightReducer();
      MergingReducer mergingReducer = new MergingReducer();
      IntCounter intCounter = new IntCounter();
      Placeholder comparisonFactorModifier = new Placeholder();
      Comparator comparator1 = new Comparator();
      Comparator comparator2 = new Comparator();
      BestQualityMemorizer bestQualityMemorizer3 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer4 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator2 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector2 = new DataTableValuesCollector();
      DataTableValuesCollector selPressDataTableValuesCollector2 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      Placeholder visualizer2 = new Placeholder();
      ConditionalBranch conditionalBranch1 = new ConditionalBranch();
      ConditionalBranch conditionalBranch2 = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("SelectionPressure", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("CurrentSuccessRatio", new DoubleValue(0)));

      variableAssigner.LeftSideParameter.ActualName = "ComparisonFactor";
      variableAssigner.RightSideParameter.ActualName = ComparisonFactorLowerBoundParameter.Name;

      bestQualityMemorizer1.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer1.QualityParameter.ActualName = QualityParameter.Name;

      bestQualityMemorizer2.BestQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestQualityMemorizer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer2.QualityParameter.ActualName = QualityParameter.Name;

      bestAverageWorstQualityCalculator1.AverageQualityParameter.ActualName = "CurrentAverageQuality";
      bestAverageWorstQualityCalculator1.BestQualityParameter.ActualName = "CurrentBestQuality";
      bestAverageWorstQualityCalculator1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestAverageWorstQualityCalculator1.QualityParameter.ActualName = QualityParameter.Name;
      bestAverageWorstQualityCalculator1.WorstQualityParameter.ActualName = "CurrentWorstQuality";

      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality", null, "CurrentBestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality", null, "CurrentAverageQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality", null, "CurrentWorstQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector1.DataTableParameter.ActualName = "Qualities";

      selPressDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Selection Pressure", null, "SelectionPressure"));
      selPressDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Maximum Selection Pressure", null, MaximumSelectionPressureParameter.Name));
      selPressDataTableValuesCollector1.DataTableParameter.ActualName = "SelectionPressures";

      qualityDifferenceCalculator1.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator1.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.SecondQualityParameter.ActualName = "BestQuality";

      visualizer1.Name = "Visualizer (placeholder)";
      visualizer1.OperatorParameter.ActualName = VisualizerParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality", null, "CurrentBestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality", null, "CurrentAverageQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality", null, "CurrentWorstQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", null, "SelectionPressure"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", null, "CurrentSuccessRatio"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IItem>("Solution Visualization", null, VisualizationParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("SelectionPressures"));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;

      selector.Name = "Selector (placeholder)";
      selector.OperatorParameter.ActualName = SelectorParameter.Name;

      childrenCreator.ParentsPerChild = new IntValue(2);

      crossover.Name = "Crossover (placeholder)";
      crossover.OperatorParameter.ActualName = CrossoverParameter.Name;

      osBeforeMutationBranch.Name = "Apply OS before mutation?";
      osBeforeMutationBranch.ConditionParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;

      evaluator1.Name = "Evaluator (placeholder)";
      evaluator1.OperatorParameter.ActualName = EvaluatorParameter.Name;

      qualityComparer1.ComparisonFactorParameter.ActualName = "ComparisonFactor";
      qualityComparer1.LeftSideParameter.ActualName = QualityParameter.Name;
      qualityComparer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      qualityComparer1.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparer1.ResultParameter.ActualName = "SuccessfulOffspring";

      mutationBranch1.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mutationBranch1.RandomParameter.ActualName = RandomParameter.Name;

      mutator1.Name = "Mutator (placeholder)";
      mutator1.OperatorParameter.ActualName = MutatorParameter.Name;

      evaluator2.Name = "Evaluator (placeholder)";
      evaluator2.OperatorParameter.ActualName = EvaluatorParameter.Name;

      mutationBranch2.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mutationBranch2.RandomParameter.ActualName = RandomParameter.Name;

      mutator2.Name = "Mutator (placeholder)";
      mutator2.OperatorParameter.ActualName = MutatorParameter.Name;

      evaluator3.Name = "Evaluator (placeholder)";
      evaluator3.OperatorParameter.ActualName = EvaluatorParameter.Name;

      qualityComparer2.ComparisonFactorParameter.ActualName = "ComparisonFactor";
      qualityComparer2.LeftSideParameter.ActualName = QualityParameter.Name;
      qualityComparer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      qualityComparer2.RightSideParameter.ActualName = QualityParameter.Name;
      qualityComparer2.ResultParameter.ActualName = "SuccessfulOffspring";

      subScopesRemover.RemoveAllSubScopes = true;

      conditionalSelector.CopySelected = new BoolValue(false);
      conditionalSelector.ConditionParameter.ActualName = "SuccessfulOffspring";

      offspringSelector.CurrentSuccessRatioParameter.ActualName = "CurrentSuccessRatio";
      offspringSelector.LuckyLosersParameter.ActualName = "OSLuckyLosers";
      offspringSelector.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      offspringSelector.PopulationSizeParameter.ActualName = PopulationSizeParameter.Name;
      offspringSelector.SelectionPressureParameter.ActualName = "SelectionPressure";
      offspringSelector.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      offspringSelector.WinnersParameter.ActualName = "OSWinners";

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = ElitesParameter.Name;
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;

      intCounter.Increment = new IntValue(1);
      intCounter.ValueParameter.ActualName = "Generations";

      comparisonFactorModifier.Name = "Modify ComparisonFactor (placeholder)";
      comparisonFactorModifier.OperatorParameter.ActualName = ComparisonFactorModifierParameter.Name;

      comparator1.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      comparator1.LeftSideParameter.ActualName = "Generations";
      comparator1.ResultParameter.ActualName = "TerminateMaximumGenerations";
      comparator1.RightSideParameter.ActualName = MaximumGenerationsParameter.Name;

      comparator2.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      comparator2.LeftSideParameter.ActualName = "SelectionPressure";
      comparator2.ResultParameter.ActualName = "TerminateSelectionPressure";
      comparator2.RightSideParameter.ActualName = MaximumSelectionPressureParameter.Name;

      bestQualityMemorizer3.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer3.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer3.QualityParameter.ActualName = QualityParameter.Name;

      bestQualityMemorizer4.BestQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestQualityMemorizer4.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer4.QualityParameter.ActualName = QualityParameter.Name;

      bestAverageWorstQualityCalculator2.AverageQualityParameter.ActualName = "CurrentAverageQuality";
      bestAverageWorstQualityCalculator2.BestQualityParameter.ActualName = "CurrentBestQuality";
      bestAverageWorstQualityCalculator2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestAverageWorstQualityCalculator2.QualityParameter.ActualName = QualityParameter.Name;
      bestAverageWorstQualityCalculator2.WorstQualityParameter.ActualName = "CurrentWorstQuality";

      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality", null, "CurrentBestQuality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality", null, "CurrentAverageQuality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality", null, "CurrentWorstQuality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector2.DataTableParameter.ActualName = "Qualities";

      selPressDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Selection Pressure", null, "SelectionPressure"));
      selPressDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Maximum Selection Pressure", null, MaximumSelectionPressureParameter.Name));
      selPressDataTableValuesCollector2.DataTableParameter.ActualName = "SelectionPressures";

      qualityDifferenceCalculator2.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator2.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.SecondQualityParameter.ActualName = "BestQuality";

      visualizer2.Name = "Visualizer (placeholder)";
      visualizer2.OperatorParameter.ActualName = VisualizerParameter.Name;

      conditionalBranch1.Name = "MaximumSelectionPressure reached?";
      conditionalBranch1.ConditionParameter.ActualName = "TerminateSelectionPressure";

      conditionalBranch2.Name = "MaximumGenerations reached?";
      conditionalBranch2.ConditionParameter.ActualName = "TerminateMaximumGenerations";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = variableAssigner;
      variableAssigner.Successor = bestQualityMemorizer1;
      bestQualityMemorizer1.Successor = bestQualityMemorizer2;
      bestQualityMemorizer2.Successor = bestAverageWorstQualityCalculator1;
      bestAverageWorstQualityCalculator1.Successor = dataTableValuesCollector1;
      dataTableValuesCollector1.Successor = selPressDataTableValuesCollector1;
      selPressDataTableValuesCollector1.Successor = qualityDifferenceCalculator1;
      qualityDifferenceCalculator1.Successor = visualizer1;
      visualizer1.Successor = resultsCollector;
      resultsCollector.Successor = selector;
      selector.Successor = subScopesProcessor1;
      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(childrenCreator);
      subScopesProcessor1.Successor = offspringSelector;
      childrenCreator.Successor = uniformSubScopesProcessor;
      uniformSubScopesProcessor.Operator = crossover;
      uniformSubScopesProcessor.Successor = conditionalSelector;
      crossover.Successor = osBeforeMutationBranch;
      osBeforeMutationBranch.TrueBranch = evaluator1;
      osBeforeMutationBranch.FalseBranch = mutationBranch2;
      osBeforeMutationBranch.Successor = subScopesRemover;
      evaluator1.Successor = qualityComparer1;
      qualityComparer1.Successor = mutationBranch1;
      mutationBranch1.FirstBranch = mutator1;
      mutationBranch1.SecondBranch = null;
      mutationBranch1.Successor = null;
      mutator1.Successor = evaluator2;
      evaluator2.Successor = null;
      mutationBranch2.FirstBranch = mutator2;
      mutationBranch2.SecondBranch = null;
      mutationBranch2.Successor = evaluator3;
      mutator2.Successor = null;
      evaluator3.Successor = qualityComparer2;
      subScopesRemover.Successor = null;
      offspringSelector.OffspringCreator = selector;
      offspringSelector.Successor = subScopesProcessor2;
      subScopesProcessor2.Operators.Add(bestSelector);
      subScopesProcessor2.Operators.Add(new EmptyOperator());
      subScopesProcessor2.Successor = mergingReducer;
      bestSelector.Successor = rightReducer;
      rightReducer.Successor = null;
      mergingReducer.Successor = intCounter;
      intCounter.Successor = comparisonFactorModifier;
      comparisonFactorModifier.Successor = comparator1;
      comparator1.Successor = comparator2;
      comparator2.Successor = bestQualityMemorizer3;
      bestQualityMemorizer3.Successor = bestQualityMemorizer4;
      bestQualityMemorizer4.Successor = bestAverageWorstQualityCalculator2;
      bestAverageWorstQualityCalculator2.Successor = dataTableValuesCollector2;
      dataTableValuesCollector2.Successor = selPressDataTableValuesCollector2;
      selPressDataTableValuesCollector2.Successor = qualityDifferenceCalculator2;
      qualityDifferenceCalculator2.Successor = visualizer2;
      visualizer2.Successor = conditionalBranch1;
      conditionalBranch1.FalseBranch = conditionalBranch2;
      conditionalBranch1.TrueBranch = null;
      conditionalBranch1.Successor = null;
      conditionalBranch2.FalseBranch = selector;
      conditionalBranch2.TrueBranch = null;
      conditionalBranch2.Successor = null;
      #endregion
    }
  }
}
