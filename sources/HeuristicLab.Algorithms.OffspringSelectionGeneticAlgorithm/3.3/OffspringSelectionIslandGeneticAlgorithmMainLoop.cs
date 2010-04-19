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
  /// An offspring selection island genetic algorithm main loop operator.
  /// </summary>
  [Item("OffspringSelectionIslandGeneticAlgorithmMainLoop", "An offspring selection island genetic algorithm main loop operator.")]
  [StorableClass]
  public sealed class OffspringSelectionIslandGeneticAlgorithmMainLoop : AlgorithmOperator {
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
    public ValueLookupParameter<IOperator> ImmigrationSelectorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["ImmigrationSelector"]; }
    }
    public ValueLookupParameter<IntValue> PopulationSizeParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    public ValueLookupParameter<IntValue> MaximumMigrationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumMigrations"]; }
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
    private OffspringSelectionIslandGeneticAlgorithmMainLoop(bool deserializing) : base() { }
    public OffspringSelectionIslandGeneticAlgorithmMainLoop()
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
      Parameters.Add(new ValueLookupParameter<IOperator>("ImmigrationSelector", "Selects the population from the unification of the original population and the immigrants."));
      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The size of the population of solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumMigrations", "The maximum number of migrations after which the operator should terminate."));
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
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorLowerBound", "The lower bound of the comparison factor (start)."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorUpperBound", "The upper bound of the comparison factor (end)."));
      Parameters.Add(new ValueLookupParameter<IOperator>("ComparisonFactorModifier", "The operator used to modify the comparison factor."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      UniformSubScopesProcessor ussp0 = new UniformSubScopesProcessor();
      VariableCreator islandVariableCreator = new VariableCreator();
      Assigner islandVariableAssigner = new Assigner();
      BestQualityMemorizer bestQualityMemorizer1 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator1 = new BestAverageWorstQualityCalculator();
      BestQualityMemorizer bestQualityMemorizer2 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator2 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector1 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();
      UniformSubScopesProcessor ussp1 = new UniformSubScopesProcessor();
      OffspringSelectionGeneticAlgorithmMainLoop mainLoop = new OffspringSelectionGeneticAlgorithmMainLoop();
      Placeholder emigrantsSelector = new Placeholder();
      Placeholder migrator = new Placeholder();
      UniformSubScopesProcessor ussp2 = new UniformSubScopesProcessor();
      IntCounter maximumGenerationsCounter = new IntCounter();
      MergingReducer mergingReducer = new MergingReducer();
      Placeholder immigrationSelector = new Placeholder();
      RightReducer rightReducer = new RightReducer();
      IntCounter migrationsCounter = new IntCounter();
      Comparator maxMigrationsComparator = new Comparator();
      BestQualityMemorizer bestQualityMemorizer3 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator3 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector2 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      ConditionalBranch migrationTerminationCondition = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Migrations", new IntValue(0)));
      
      islandVariableCreator.CollectedValues.Add(new ValueParameter<ResultCollection>("IslandResults", new ResultCollection()));
      islandVariableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));
      islandVariableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));

      islandVariableAssigner.LeftSideParameter.ActualName = "ComparisonFactor";
      islandVariableAssigner.RightSideParameter.ActualName = ComparisonFactorLowerBoundParameter.Name;

      bestQualityMemorizer1.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer1.QualityParameter.ActualName = QualityParameter.Name;

      bestAverageWorstQualityCalculator1.AverageQualityParameter.ActualName = "CurrentAverageQuality";
      bestAverageWorstQualityCalculator1.BestQualityParameter.ActualName = "CurrentBestQuality";
      bestAverageWorstQualityCalculator1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestAverageWorstQualityCalculator1.QualityParameter.ActualName = QualityParameter.Name;
      bestAverageWorstQualityCalculator1.WorstQualityParameter.ActualName = "CurrentWorstQuality";

      bestQualityMemorizer2.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer2.QualityParameter.ActualName = "BestQuality";

      bestAverageWorstQualityCalculator2.AverageQualityParameter.ActualName = "CurrentAverageBestQuality";
      bestAverageWorstQualityCalculator2.BestQualityParameter.ActualName = "CurrentBestBestQuality";
      bestAverageWorstQualityCalculator2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestAverageWorstQualityCalculator2.QualityParameter.ActualName = "CurrentBestQuality";
      bestAverageWorstQualityCalculator2.WorstQualityParameter.ActualName = "CurrentWorstBestQuality";

      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      dataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector1.DataTableParameter.ActualName = "BestQualities";

      qualityDifferenceCalculator1.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator1.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator1.SecondQualityParameter.ActualName = "BestQuality";

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Migrations"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("BestQualities"));
      resultsCollector.CollectedValues.Add(new SubScopesLookupParameter<ResultCollection>("IslandResults", "Result set for each island"));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;

      mainLoop.VariableInitializer = null; // we do the variable initialization here
      mainLoop.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
      mainLoop.MaximizationParameter.ActualName = MaximizationParameter.Name;
      mainLoop.QualityParameter.ActualName = QualityParameter.Name;
      mainLoop.SelectorParameter.ActualName = SelectorParameter.Name;
      mainLoop.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainLoop.ElitesParameter.ActualName = ElitesParameter.Name;
      mainLoop.MaximumGenerationsParameter.ActualName = "MaximumGenerations";
      mainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      mainLoop.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
      mainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainLoop.RandomParameter.ActualName = RandomParameter.Name;
      mainLoop.ResultsParameter.ActualName = "IslandResults";
      mainLoop.VisualizerParameter.ActualName = VisualizerParameter.Name;
      mainLoop.VisualizationParameter.ActualName = VisualizationParameter.Name;
      mainLoop.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      mainLoop.ComparisonFactorLowerBoundParameter.ActualName = ComparisonFactorLowerBoundParameter.Name;
      mainLoop.ComparisonFactorModifierParameter.ActualName = ComparisonFactorModifierParameter.Name;
      mainLoop.ComparisonFactorUpperBoundParameter.ActualName = ComparisonFactorUpperBoundParameter.Name;
      mainLoop.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      mainLoop.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;

      maximumGenerationsCounter.ValueParameter.ActualName = "MaximumGenerations";
      maximumGenerationsCounter.Increment = null;
      maximumGenerationsCounter.IncrementParameter.ActualName = "MigrationInterval";

      emigrantsSelector.Name = "Emigrants Selector (placeholder)";
      emigrantsSelector.OperatorParameter.ActualName = EmigrantsSelectorParameter.Name;

      migrator.Name = "Migrator (placeholder)";
      migrator.OperatorParameter.ActualName = MigratorParameter.Name;

      immigrationSelector.Name = "Immigration Selector (placeholder)";
      immigrationSelector.OperatorParameter.ActualName = ImmigrationSelectorParameter.Name;

      migrationsCounter.Name = "Migrations + 1";
      migrationsCounter.IncrementParameter.Value = new IntValue(1);
      migrationsCounter.ValueParameter.ActualName = "Migrations";

      maxMigrationsComparator.Name = "Migrations >= MaximumMigration ?";
      maxMigrationsComparator.LeftSideParameter.ActualName = "Migrations";
      maxMigrationsComparator.RightSideParameter.ActualName = MaximumMigrationsParameter.Name;
      maxMigrationsComparator.Comparison.Value = ComparisonType.GreaterOrEqual;
      maxMigrationsComparator.ResultParameter.ActualName = "MigrationTerminate";

      bestQualityMemorizer3.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer3.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer3.QualityParameter.ActualName = "BestQuality";

      bestAverageWorstQualityCalculator3.AverageQualityParameter.ActualName = "CurrentAverageBestQuality";
      bestAverageWorstQualityCalculator3.BestQualityParameter.ActualName = "CurrentBestBestQuality";
      bestAverageWorstQualityCalculator3.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestAverageWorstQualityCalculator3.QualityParameter.ActualName = "CurrentBestQuality";
      bestAverageWorstQualityCalculator3.WorstQualityParameter.ActualName = "CurrentWorstBestQuality";

      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best BestQuality", null, "CurrentBestBestQuality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average BestQuality", null, "CurrentAverageBestQuality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst BestQuality", null, "CurrentWorstBestQuality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      dataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector2.DataTableParameter.ActualName = "BestQualities";

      qualityDifferenceCalculator2.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator2.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator2.SecondQualityParameter.ActualName = "BestQuality";

      migrationTerminationCondition.ConditionParameter.ActualName = "MigrationTerminate";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = ussp0;
      ussp0.Operator = islandVariableCreator;
      ussp0.Successor = bestQualityMemorizer2;
      islandVariableCreator.Successor = islandVariableAssigner;
      islandVariableAssigner.Successor = bestQualityMemorizer1;
      bestQualityMemorizer1.Successor = bestAverageWorstQualityCalculator1;
      bestQualityMemorizer2.Successor = bestAverageWorstQualityCalculator2;
      bestAverageWorstQualityCalculator2.Successor = dataTableValuesCollector1;
      dataTableValuesCollector1.Successor = qualityDifferenceCalculator1;
      qualityDifferenceCalculator1.Successor = resultsCollector;
      resultsCollector.Successor = ussp1;
      ussp1.Operator = mainLoop;
      ussp1.Successor = migrator;
      mainLoop.Successor = maximumGenerationsCounter;
      maximumGenerationsCounter.Successor = emigrantsSelector;
      emigrantsSelector.Successor = null;
      migrator.Successor = ussp2;
      ussp2.Operator = mergingReducer;
      ussp2.Successor = migrationsCounter;
      mergingReducer.Successor = immigrationSelector;
      immigrationSelector.Successor = rightReducer;
      rightReducer.Successor = null;
      migrationsCounter.Successor = maxMigrationsComparator;
      maxMigrationsComparator.Successor = bestQualityMemorizer3;
      bestQualityMemorizer3.Successor = bestAverageWorstQualityCalculator3;
      bestAverageWorstQualityCalculator3.Successor = dataTableValuesCollector2;
      dataTableValuesCollector2.Successor = qualityDifferenceCalculator2;
      qualityDifferenceCalculator2.Successor = migrationTerminationCondition;
      migrationTerminationCondition.FalseBranch = ussp1;
      migrationTerminationCondition.TrueBranch = null;
      migrationTerminationCondition.Successor = null;
      #endregion
    }
  }
}
