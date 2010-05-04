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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm {
  /// <summary>
  /// An island offspring selection genetic algorithm main loop operator.
  /// </summary>
  [Item("IslandOffspringSelectionGeneticAlgorithmMainLoop", "An island offspring selection genetic algorithm main loop operator.")]
  [StorableClass]
  public sealed class IslandOffspringSelectionGeneticAlgorithmMainLoop : AlgorithmOperator {
    #region Parameter Properties
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
    public ValueLookupParameter<IntValue> NumberOfIslandsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["NumberOfIslands"]; }
    }
    public ValueLookupParameter<IntValue> MigrationIntervalParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MigrationInterval"]; }
    }
    public ValueLookupParameter<PercentValue> MigrationRateParameter {
      get { return (ValueLookupParameter<PercentValue>)Parameters["MigrationRate"]; }
    }
    public ValueLookupParameter<IOperator> MigratorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Migrator"]; }
    }
    public ValueLookupParameter<IOperator> EmigrantsSelectorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["EmigrantsSelector"]; }
    }
    public ValueLookupParameter<IOperator> ImmigrationReplacerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["ImmigrationReplacer"]; }
    }
    public ValueLookupParameter<IntValue> PopulationSizeParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    public ValueLookupParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumGenerations"]; }
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
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
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
    public LookupParameter<DoubleValue> ComparisonFactorParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
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
    private IslandOffspringSelectionGeneticAlgorithmMainLoop(bool deserializing) : base() { }
    public IslandOffspringSelectionGeneticAlgorithmMainLoop()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfIslands", "The number of islands."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MigrationInterval", "The number of generations that should pass between migration phases."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MigrationRate", "The proportion of individuals that should migrate between the islands."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Migrator", "The migration strategy."));
      Parameters.Add(new ValueLookupParameter<IOperator>("EmigrantsSelector", "Selects the individuals that will be migrated."));
      Parameters.Add(new ValueLookupParameter<IOperator>("ImmigrationReplacer", "Replaces part of the original population with the immigrants."));
      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The size of the population of solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations that should be processed."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection to store the results."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Visualizer", "The operator used to visualize solutions."));
      Parameters.Add(new LookupParameter<IItem>("Visualization", "The item which represents the visualization of solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new LookupParameter<DoubleValue>("ComparisonFactor", "The comparison factor is used to determine whether the offspring should be compared to the better parent, the worse parent or a quality value linearly interpolated between them. It is in the range [0;1]."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorLowerBound", "The lower bound of the comparison factor (start)."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorUpperBound", "The upper bound of the comparison factor (end)."));
      Parameters.Add(new ValueLookupParameter<IOperator>("ComparisonFactorModifier", "The operator used to modify the comparison factor."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor0 = new UniformSubScopesProcessor();
      VariableCreator islandVariableCreator = new VariableCreator();
      BestQualityMemorizer islandBestQualityMemorizer1 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator islandBestAverageWorstQualityCalculator1 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector islandDataTableValuesCollector1 = new DataTableValuesCollector();
      DataTableValuesCollector islandDataTableValuesCollector2 = new DataTableValuesCollector();
      QualityDifferenceCalculator islandQualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      Placeholder islandVisualizer1 = new Placeholder();
      ResultsCollector islandResultsCollector = new ResultsCollector();
      BestQualityMemorizer bestQualityMemorizer1 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer2 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator1 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector1 = new DataTableValuesCollector();
      DataTableValuesCollector dataTableValuesCollector2 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();
      Placeholder comparisonFactorModifier = new Placeholder();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      ConditionalBranch islandTerminatedBySelectionPressure1 = new ConditionalBranch();
      OffspringSelectionGeneticAlgorithmMainOperator mainOperator = new OffspringSelectionGeneticAlgorithmMainOperator();
      BestQualityMemorizer islandBestQualityMemorizer2 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator islandBestAverageWorstQualityCalculator2 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector islandDataTableValuesCollector3 = new DataTableValuesCollector();
      DataTableValuesCollector islandDataTableValuesCollector4 = new DataTableValuesCollector();
      QualityDifferenceCalculator islandQualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      Placeholder islandVisualizer2 = new Placeholder();
      IntCounter islandEvaluatedSolutionsCounter = new IntCounter();
      Comparator islandSelectionPressureComparator = new Comparator();
      ConditionalBranch islandTerminatedBySelectionPressure2 = new ConditionalBranch();
      IntCounter terminatedIslandsCounter = new IntCounter();
      IntCounter generationsCounter = new IntCounter();
      IntCounter generationsSinceLastMigrationCounter = new IntCounter();
      Comparator migrationComparator = new Comparator();
      ConditionalBranch migrationBranch = new ConditionalBranch();
      Assigner resetTerminatedIslandsAssigner = new Assigner();
      Assigner resetGenerationsSinceLastMigrationAssigner = new Assigner();
      IntCounter migrationsCounter = new IntCounter();
      UniformSubScopesProcessor uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      Assigner reviveIslandAssigner = new Assigner();
      Placeholder emigrantsSelector = new Placeholder();
      Placeholder migrator = new Placeholder();
      UniformSubScopesProcessor uniformSubScopesProcessor3 = new UniformSubScopesProcessor();
      Placeholder immigrationReplacer = new Placeholder();
      Comparator generationsComparator = new Comparator();
      Comparator terminatedIslandsComparator = new Comparator();
      BestQualityMemorizer bestQualityMemorizer3 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer4 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator2 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector3 = new DataTableValuesCollector();
      DataTableValuesCollector dataTableValuesCollector4 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      ConditionalBranch generationsTerminationCondition = new ConditionalBranch();
      ConditionalBranch terminatedIslandsCondition = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Migrations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("GenerationsSinceLastMigration", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("TerminatedIslands", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));

      islandVariableCreator.CollectedValues.Add(new ValueParameter<ResultCollection>("IslandResults", new ResultCollection()));
      islandVariableCreator.CollectedValues.Add(new ValueParameter<IntValue>("IslandEvaluatedSolutions", new IntValue(0)));
      islandVariableCreator.CollectedValues.Add(new ValueParameter<BoolValue>("TerminateSelectionPressure", new BoolValue(false)));
      islandVariableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("SelectionPressure", new DoubleValue(0)));

      islandBestQualityMemorizer1.BestQualityParameter.ActualName = "BestQuality";
      islandBestQualityMemorizer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      islandBestQualityMemorizer1.QualityParameter.ActualName = QualityParameter.Name;

      islandBestAverageWorstQualityCalculator1.AverageQualityParameter.ActualName = "CurrentAverageQuality";
      islandBestAverageWorstQualityCalculator1.BestQualityParameter.ActualName = "CurrentBestQuality";
      islandBestAverageWorstQualityCalculator1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      islandBestAverageWorstQualityCalculator1.QualityParameter.ActualName = QualityParameter.Name;
      islandBestAverageWorstQualityCalculator1.WorstQualityParameter.ActualName = "CurrentWorstQuality";
      
      islandDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      islandDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      islandDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      islandDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      islandDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      islandDataTableValuesCollector1.DataTableParameter.ActualName = "BestQualities";

      islandDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Selection Pressure", null, "SelectionPressure"));
      islandDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Maximum Selection Pressure", null, MaximumSelectionPressureParameter.Name));
      islandDataTableValuesCollector2.DataTableParameter.ActualName = "SelectionPressures";

      islandQualityDifferenceCalculator1.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      islandQualityDifferenceCalculator1.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      islandQualityDifferenceCalculator1.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      islandQualityDifferenceCalculator1.SecondQualityParameter.ActualName = "BestQuality";

      islandResultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality", null, "CurrentBestQuality"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality", null, "CurrentAverageQuality"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality", null, "CurrentWorstQuality"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "IslandEvaluatedSolutions"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Curent Comparison Factor", null, "ComparisonFactor"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", null, "SelectionPressure"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", null, "CurrentSuccessRatio"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<IItem>("Solution Visualization", null, VisualizationParameter.Name));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("SelectionPressures"));
      islandResultsCollector.ResultsParameter.ActualName = "IslandResults";

      bestQualityMemorizer1.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer1.QualityParameter.ActualName = "BestQuality";

      bestQualityMemorizer2.BestQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestQualityMemorizer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer2.QualityParameter.ActualName = "BestQuality";

      bestAverageWorstQualityCalculator1.AverageQualityParameter.ActualName = "CurrentAverageBestQuality";
      bestAverageWorstQualityCalculator1.BestQualityParameter.ActualName = "CurrentBestBestQuality";
      bestAverageWorstQualityCalculator1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestAverageWorstQualityCalculator1.QualityParameter.ActualName = "CurrentBestQuality";
      bestAverageWorstQualityCalculator1.WorstQualityParameter.ActualName = "CurrentWorstBestQuality";

      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector1.DataTableParameter.ActualName = "BestQualities";

      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Maximum Selection Pressure", null, MaximumSelectionPressureParameter.Name));
      dataTableValuesCollector2.CollectedValues.Add(new SubScopesLookupParameter<DoubleValue>("Selection Pressure Island", null, "SelectionPressure"));
      dataTableValuesCollector2.DataTableParameter.ActualName = "SelectionPressures";

      qualityDifferenceCalculator1.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator1.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.SecondQualityParameter.ActualName = "BestQuality";

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Migrations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("EvaluatedSolutions"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("BestQualities"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("SelectionPressures"));
      resultsCollector.CollectedValues.Add(new SubScopesLookupParameter<ResultCollection>("IslandResults", "Result set for each island"));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;

      comparisonFactorModifier.Name = "ComparisonFactorModifier (Placeholder)";
      comparisonFactorModifier.OperatorParameter.ActualName = ComparisonFactorModifierParameter.Name;

      islandTerminatedBySelectionPressure1.Name = "Island Terminated ?";
      islandTerminatedBySelectionPressure1.ConditionParameter.ActualName = "TerminateSelectionPressure";

      mainOperator.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      mainOperator.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainOperator.CurrentSuccessRatioParameter.ActualName = "CurrentSuccessRatio";
      mainOperator.ElitesParameter.ActualName = ElitesParameter.Name;
      mainOperator.EvaluatedSolutionsParameter.ActualName = "IslandEvaluatedSolutions";
      mainOperator.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
      mainOperator.MaximizationParameter.ActualName = MaximizationParameter.Name;
      mainOperator.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      mainOperator.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainOperator.MutatorParameter.ActualName = MutatorParameter.Name;
      mainOperator.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
      mainOperator.QualityParameter.ActualName = QualityParameter.Name;
      mainOperator.RandomParameter.ActualName = RandomParameter.Name;
      mainOperator.SelectionPressureParameter.ActualName = "SelectionPressure";
      mainOperator.SelectorParameter.ActualName = SelectorParameter.Name;
      mainOperator.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;

      islandBestQualityMemorizer2.BestQualityParameter.ActualName = "BestQuality";
      islandBestQualityMemorizer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      islandBestQualityMemorizer2.QualityParameter.ActualName = QualityParameter.Name;

      islandBestAverageWorstQualityCalculator2.AverageQualityParameter.ActualName = "CurrentAverageQuality";
      islandBestAverageWorstQualityCalculator2.BestQualityParameter.ActualName = "CurrentBestQuality";
      islandBestAverageWorstQualityCalculator2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      islandBestAverageWorstQualityCalculator2.QualityParameter.ActualName = QualityParameter.Name;
      islandBestAverageWorstQualityCalculator2.WorstQualityParameter.ActualName = "CurrentWorstQuality";

      islandDataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      islandDataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      islandDataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      islandDataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      islandDataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      islandDataTableValuesCollector3.DataTableParameter.ActualName = "BestQualities";

      islandDataTableValuesCollector4.CollectedValues.Add(new LookupParameter<DoubleValue>("Selection Pressure", null, "SelectionPressure"));
      islandDataTableValuesCollector4.CollectedValues.Add(new LookupParameter<DoubleValue>("Maximum Selection Pressure", null, MaximumSelectionPressureParameter.Name));
      islandDataTableValuesCollector4.DataTableParameter.ActualName = "SelectionPressures";

      islandQualityDifferenceCalculator2.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      islandQualityDifferenceCalculator2.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      islandQualityDifferenceCalculator2.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      islandQualityDifferenceCalculator2.SecondQualityParameter.ActualName = "BestQuality";

      islandEvaluatedSolutionsCounter.Name = "Update EvaluatedSolutions";
      islandEvaluatedSolutionsCounter.ValueParameter.ActualName = "EvaluatedSolutions";
      islandEvaluatedSolutionsCounter.Increment = null;
      islandEvaluatedSolutionsCounter.IncrementParameter.ActualName = "IslandEvaluatedSolutions";

      islandSelectionPressureComparator.Name = "SelectionPressure >= MaximumSelectionPressure ?";
      islandSelectionPressureComparator.LeftSideParameter.ActualName = "SelectionPressure";
      islandSelectionPressureComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      islandSelectionPressureComparator.RightSideParameter.ActualName = MaximumSelectionPressureParameter.Name;
      islandSelectionPressureComparator.ResultParameter.ActualName = "TerminateSelectionPressure";

      islandTerminatedBySelectionPressure2.Name = "Island Terminated ?";
      islandTerminatedBySelectionPressure2.ConditionParameter.ActualName = "TerminateSelectionPressure";

      terminatedIslandsCounter.Name = "TerminatedIslands + 1";
      terminatedIslandsCounter.ValueParameter.ActualName = "TerminatedIslands";
      terminatedIslandsCounter.Increment = new IntValue(1);

      generationsCounter.Name = "Generations + 1";
      generationsCounter.ValueParameter.ActualName = "Generations";
      generationsCounter.Increment = new IntValue(1);

      generationsSinceLastMigrationCounter.Name = "GenerationsSinceLastMigration + 1";
      generationsSinceLastMigrationCounter.ValueParameter.ActualName = "GenerationsSinceLastMigration";
      generationsSinceLastMigrationCounter.Increment = new IntValue(1);

      migrationComparator.Name = "GenerationsSinceLastMigration = MigrationInterval ?";
      migrationComparator.LeftSideParameter.ActualName = "GenerationsSinceLastMigration";
      migrationComparator.Comparison = new Comparison(ComparisonType.Equal);
      migrationComparator.RightSideParameter.ActualName = MigrationIntervalParameter.Name;
      migrationComparator.ResultParameter.ActualName = "Migrate";

      migrationBranch.Name = "Migrate?";
      migrationBranch.ConditionParameter.ActualName = "Migrate";

      resetTerminatedIslandsAssigner.Name = "Reset TerminatedIslands";
      resetTerminatedIslandsAssigner.LeftSideParameter.ActualName = "TerminatedIslands";
      resetTerminatedIslandsAssigner.RightSideParameter.Value = new IntValue(0);

      resetGenerationsSinceLastMigrationAssigner.Name = "Reset GenerationsSinceLastMigration";
      resetGenerationsSinceLastMigrationAssigner.LeftSideParameter.ActualName = "GenerationsSinceLastMigration";
      resetGenerationsSinceLastMigrationAssigner.RightSideParameter.Value = new IntValue(0);

      migrationsCounter.Name = "Migrations + 1";
      migrationsCounter.IncrementParameter.Value = new IntValue(1);
      migrationsCounter.ValueParameter.ActualName = "Migrations";

      reviveIslandAssigner.Name = "Revive Island";
      reviveIslandAssigner.LeftSideParameter.ActualName = "TerminateSelectionPressure";
      reviveIslandAssigner.RightSideParameter.Value = new BoolValue(false);

      emigrantsSelector.Name = "Emigrants Selector (placeholder)";
      emigrantsSelector.OperatorParameter.ActualName = EmigrantsSelectorParameter.Name;

      migrator.Name = "Migrator (placeholder)";
      migrator.OperatorParameter.ActualName = MigratorParameter.Name;

      immigrationReplacer.Name = "Immigration Replacer (placeholder)";
      immigrationReplacer.OperatorParameter.ActualName = ImmigrationReplacerParameter.Name;

      generationsComparator.Name = "Generations >= MaximumGenerations ?";
      generationsComparator.LeftSideParameter.ActualName = "Generations";
      generationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      generationsComparator.RightSideParameter.ActualName = MaximumGenerationsParameter.Name;
      generationsComparator.ResultParameter.ActualName = "TerminateGenerations";

      terminatedIslandsComparator.Name = "All Islands terminated ?";
      terminatedIslandsComparator.LeftSideParameter.ActualName = "TerminatedIslands";
      terminatedIslandsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      terminatedIslandsComparator.RightSideParameter.ActualName = NumberOfIslandsParameter.Name;
      terminatedIslandsComparator.ResultParameter.ActualName = "TerminateTerminatedIslands";
      
      bestQualityMemorizer3.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer3.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer3.QualityParameter.ActualName = "BestQuality";

      bestQualityMemorizer4.BestQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestQualityMemorizer4.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer4.QualityParameter.ActualName = "BestQuality";

      bestAverageWorstQualityCalculator2.AverageQualityParameter.ActualName = "CurrentAverageBestQuality";
      bestAverageWorstQualityCalculator2.BestQualityParameter.ActualName = "CurrentBestBestQuality";
      bestAverageWorstQualityCalculator2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestAverageWorstQualityCalculator2.QualityParameter.ActualName = "CurrentBestQuality";
      bestAverageWorstQualityCalculator2.WorstQualityParameter.ActualName = "CurrentWorstBestQuality";

      dataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      dataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      dataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      dataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      dataTableValuesCollector3.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector3.DataTableParameter.ActualName = "BestQualities";

      dataTableValuesCollector4.CollectedValues.Add(new LookupParameter<DoubleValue>("Maximum Selection Pressure", null, MaximumSelectionPressureParameter.Name));
      dataTableValuesCollector4.CollectedValues.Add(new SubScopesLookupParameter<DoubleValue>("Selection Pressure Island", null, "SelectionPressure"));
      dataTableValuesCollector4.DataTableParameter.ActualName = "SelectionPressures";

      qualityDifferenceCalculator2.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator2.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.SecondQualityParameter.ActualName = "BestQuality";

      generationsTerminationCondition.Name = "Terminate (MaxGenerations) ?";
      generationsTerminationCondition.ConditionParameter.ActualName = "TerminateGenerations";

      terminatedIslandsCondition.Name = "Terminate (TerminatedIslands) ?";
      terminatedIslandsCondition.ConditionParameter.ActualName = "TerminateTerminatedIslands";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = uniformSubScopesProcessor0;
      uniformSubScopesProcessor0.Operator = islandVariableCreator;
      uniformSubScopesProcessor0.Successor = bestQualityMemorizer1;
      islandVariableCreator.Successor = islandBestQualityMemorizer1;
      islandBestQualityMemorizer1.Successor = islandBestAverageWorstQualityCalculator1;
      islandBestAverageWorstQualityCalculator1.Successor = islandDataTableValuesCollector1;
      islandDataTableValuesCollector1.Successor = islandDataTableValuesCollector2;
      islandDataTableValuesCollector2.Successor = islandQualityDifferenceCalculator1;
      islandQualityDifferenceCalculator1.Successor = islandVisualizer1;
      islandVisualizer1.Successor = islandResultsCollector;
      islandResultsCollector.Successor = null;
      bestQualityMemorizer1.Successor = bestQualityMemorizer2;
      bestQualityMemorizer2.Successor = bestAverageWorstQualityCalculator1;
      bestAverageWorstQualityCalculator1.Successor = dataTableValuesCollector1;
      dataTableValuesCollector1.Successor = dataTableValuesCollector2;
      dataTableValuesCollector2.Successor = qualityDifferenceCalculator1;
      qualityDifferenceCalculator1.Successor = resultsCollector;
      resultsCollector.Successor = comparisonFactorModifier;
      comparisonFactorModifier.Successor = uniformSubScopesProcessor1;
      uniformSubScopesProcessor1.Operator = islandTerminatedBySelectionPressure1;
      uniformSubScopesProcessor1.Successor = generationsCounter;
      islandTerminatedBySelectionPressure1.TrueBranch = null;
      islandTerminatedBySelectionPressure1.FalseBranch = mainOperator;
      islandTerminatedBySelectionPressure1.Successor = null;
      mainOperator.Successor = islandBestQualityMemorizer2;
      islandBestQualityMemorizer2.Successor = islandBestAverageWorstQualityCalculator2;
      islandBestAverageWorstQualityCalculator2.Successor = islandDataTableValuesCollector3;
      islandDataTableValuesCollector3.Successor = islandDataTableValuesCollector4;
      islandDataTableValuesCollector4.Successor = islandQualityDifferenceCalculator2;
      islandQualityDifferenceCalculator2.Successor = islandVisualizer2;
      islandVisualizer2.Successor = islandEvaluatedSolutionsCounter;
      islandEvaluatedSolutionsCounter.Successor = islandSelectionPressureComparator;
      islandSelectionPressureComparator.Successor = islandTerminatedBySelectionPressure2;
      islandTerminatedBySelectionPressure2.TrueBranch = terminatedIslandsCounter;
      islandTerminatedBySelectionPressure2.FalseBranch = null;
      islandTerminatedBySelectionPressure2.Successor = null;
      generationsCounter.Successor = generationsSinceLastMigrationCounter;
      generationsSinceLastMigrationCounter.Successor = migrationComparator;
      migrationComparator.Successor = migrationBranch;
      migrationBranch.TrueBranch = resetTerminatedIslandsAssigner;
      migrationBranch.FalseBranch = null;
      migrationBranch.Successor = generationsComparator;
      resetTerminatedIslandsAssigner.Successor = resetGenerationsSinceLastMigrationAssigner;
      resetGenerationsSinceLastMigrationAssigner.Successor = migrationsCounter;
      migrationsCounter.Successor = uniformSubScopesProcessor2;
      uniformSubScopesProcessor2.Operator = reviveIslandAssigner;
      uniformSubScopesProcessor2.Successor = migrator;
      reviveIslandAssigner.Successor = emigrantsSelector;
      emigrantsSelector.Successor = null;
      migrator.Successor = uniformSubScopesProcessor3;
      uniformSubScopesProcessor3.Operator = immigrationReplacer;
      uniformSubScopesProcessor3.Successor = null;
      immigrationReplacer.Successor = null;
      generationsComparator.Successor = terminatedIslandsComparator;
      terminatedIslandsComparator.Successor = bestQualityMemorizer3;
      bestQualityMemorizer3.Successor = bestQualityMemorizer4;
      bestQualityMemorizer4.Successor = bestAverageWorstQualityCalculator2;
      bestAverageWorstQualityCalculator2.Successor = dataTableValuesCollector3;
      dataTableValuesCollector3.Successor = dataTableValuesCollector4;
      dataTableValuesCollector4.Successor = qualityDifferenceCalculator2;
      qualityDifferenceCalculator2.Successor = generationsTerminationCondition;
      generationsTerminationCondition.TrueBranch = null;
      generationsTerminationCondition.FalseBranch = terminatedIslandsCondition;
      generationsTerminationCondition.Successor = null;
      terminatedIslandsCondition.TrueBranch = null;
      terminatedIslandsCondition.FalseBranch = comparisonFactorModifier;
      terminatedIslandsCondition.Successor = null;
      #endregion
    }
  }
}
