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

namespace HeuristicLab.Algorithms.GeneticAlgorithm {
  /// <summary>
  /// An island genetic algorithm main loop operator.
  /// </summary>
  [Item("IslandGeneticAlgorithmMainLoop", "An island genetic algorithm main loop operator.")]
  [StorableClass]
  public sealed class IslandGeneticAlgorithmMainLoop : AlgorithmOperator {
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
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    #endregion

    [StorableConstructor]
    private IslandGeneticAlgorithmMainLoop(bool deserializing) : base() { }
    public IslandGeneticAlgorithmMainLoop()
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
      Parameters.Add(new ValueLookupParameter<IOperator>("ImmigrationReplacer", "Replaces some of the original population with the immigrants."));
      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The size of the population of solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations that the algorithm should process."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection to store the results."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze each island."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor0 = new UniformSubScopesProcessor();
      VariableCreator islandVariableCreator = new VariableCreator();
      BestQualityMemorizer islandBestQualityMemorizer1 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator islandBestAverageWorstQualityCalculator1 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector islandDataTableValuesCollector1 = new DataTableValuesCollector();
      QualityDifferenceCalculator islandQualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      Placeholder islandAnalyzer1 = new Placeholder();
      ResultsCollector islandResultsCollector = new ResultsCollector();
      BestQualityMemorizer bestQualityMemorizer1 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer2 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator1 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector1 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator1 = new QualityDifferenceCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      Placeholder selector = new Placeholder();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      ChildrenCreator childrenCreator = new ChildrenCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      Placeholder crossover = new Placeholder();
      StochasticBranch stochasticBranch = new StochasticBranch();
      Placeholder mutator = new Placeholder();
      Placeholder evaluator = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      SubScopesProcessor subScopesProcessor2 = new SubScopesProcessor();
      BestSelector bestSelector = new BestSelector();
      RightReducer rightReducer = new RightReducer();
      MergingReducer mergingReducer = new MergingReducer();
      BestQualityMemorizer islandBestQualityMemorizer2 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator islandBestAverageWorstQualityCalculator2 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector islandDataTableValuesCollector2 = new DataTableValuesCollector();
      QualityDifferenceCalculator islandQualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      Placeholder islandAnalyzer2 = new Placeholder();
      IntCounter generationsCounter = new IntCounter();
      IntCounter generationsSinceLastMigrationCounter = new IntCounter();
      Comparator migrationComparator = new Comparator();
      ConditionalBranch migrationBranch = new ConditionalBranch();
      Assigner resetGenerationsSinceLastMigrationAssigner = new Assigner();
      IntCounter migrationsCounter = new IntCounter();
      UniformSubScopesProcessor uniformSubScopesProcessor3 = new UniformSubScopesProcessor();
      Placeholder emigrantsSelector = new Placeholder();
      Placeholder migrator = new Placeholder();
      UniformSubScopesProcessor uniformSubScopesProcessor4 = new UniformSubScopesProcessor();
      Placeholder immigrationReplacer = new Placeholder();
      Comparator generationsComparator = new Comparator();
      BestQualityMemorizer bestQualityMemorizer3 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer4 = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator2 = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector2 = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator2 = new QualityDifferenceCalculator();
      ConditionalBranch generationsTerminationCondition = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Migrations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("GenerationsSinceLastMigration", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));

      islandVariableCreator.CollectedValues.Add(new ValueParameter<ResultCollection>("IslandResults", new ResultCollection()));

      islandBestQualityMemorizer1.BestQualityParameter.ActualName = "BestQuality";
      islandBestQualityMemorizer1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      islandBestQualityMemorizer1.QualityParameter.ActualName = QualityParameter.Name;

      islandBestAverageWorstQualityCalculator1.AverageQualityParameter.ActualName = "CurrentAverageQuality";
      islandBestAverageWorstQualityCalculator1.BestQualityParameter.ActualName = "CurrentBestQuality";
      islandBestAverageWorstQualityCalculator1.MaximizationParameter.ActualName = MaximizationParameter.Name;
      islandBestAverageWorstQualityCalculator1.QualityParameter.ActualName = QualityParameter.Name;
      islandBestAverageWorstQualityCalculator1.WorstQualityParameter.ActualName = "CurrentWorstQuality";

      islandDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality", null, "CurrentBestQuality"));
      islandDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality", null, "CurrentAverageQuality"));
      islandDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality", null, "CurrentWorstQuality"));
      islandDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      islandDataTableValuesCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, "BestKnownQuality"));
      islandDataTableValuesCollector1.DataTableParameter.ActualName = "Qualities";

      islandQualityDifferenceCalculator1.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      islandQualityDifferenceCalculator1.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      islandQualityDifferenceCalculator1.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      islandQualityDifferenceCalculator1.SecondQualityParameter.ActualName = "BestQuality";

      islandAnalyzer1.Name = "Analyzer";
      islandAnalyzer1.OperatorParameter.ActualName = "Analyzer";

      islandResultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality", null, "CurrentBestQuality"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality", null, "CurrentAverageQuality"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality", null, "CurrentWorstQuality"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, "BestKnownQuality"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      islandResultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
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

      selector.Name = "Selector (placeholder)";
      selector.OperatorParameter.ActualName = SelectorParameter.Name;

      childrenCreator.ParentsPerChild = new IntValue(2);

      crossover.Name = "Crossover (placeholder)";
      crossover.OperatorParameter.ActualName = CrossoverParameter.Name;

      stochasticBranch.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      stochasticBranch.RandomParameter.ActualName = RandomParameter.Name;

      mutator.Name = "Mutator (placeholder)";
      mutator.OperatorParameter.ActualName = MutatorParameter.Name;

      evaluator.Name = "Evaluator (placeholder)";
      evaluator.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesRemover.RemoveAllSubScopes = true;

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = ElitesParameter.Name;
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;

      islandBestQualityMemorizer2.BestQualityParameter.ActualName = "BestQuality";
      islandBestQualityMemorizer2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      islandBestQualityMemorizer2.QualityParameter.ActualName = QualityParameter.Name;

      islandBestAverageWorstQualityCalculator2.AverageQualityParameter.ActualName = "CurrentAverageQuality";
      islandBestAverageWorstQualityCalculator2.BestQualityParameter.ActualName = "CurrentBestQuality";
      islandBestAverageWorstQualityCalculator2.MaximizationParameter.ActualName = MaximizationParameter.Name;
      islandBestAverageWorstQualityCalculator2.QualityParameter.ActualName = QualityParameter.Name;
      islandBestAverageWorstQualityCalculator2.WorstQualityParameter.ActualName = "CurrentWorstQuality";

      islandDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Quality", null, "CurrentBestQuality"));
      islandDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Average Quality", null, "CurrentAverageQuality"));
      islandDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Worst Quality", null, "CurrentWorstQuality"));
      islandDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      islandDataTableValuesCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, BestKnownQualityParameter.Name));
      islandDataTableValuesCollector2.DataTableParameter.ActualName = "Qualities";

      islandQualityDifferenceCalculator2.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      islandQualityDifferenceCalculator2.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      islandQualityDifferenceCalculator2.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      islandQualityDifferenceCalculator2.SecondQualityParameter.ActualName = "BestQuality";

      islandAnalyzer2.Name = "Analyzer";
      islandAnalyzer2.OperatorParameter.ActualName = "Analyzer";

      generationsCounter.Name = "Generations + 1";
      generationsCounter.Increment = new IntValue(1);
      generationsCounter.ValueParameter.ActualName = "Generations";

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

      resetGenerationsSinceLastMigrationAssigner.Name = "Reset GenerationsSinceLastMigration";
      resetGenerationsSinceLastMigrationAssigner.LeftSideParameter.ActualName = "GenerationsSinceLastMigration";
      resetGenerationsSinceLastMigrationAssigner.RightSideParameter.Value = new IntValue(0);

      migrationsCounter.Name = "Migrations + 1";
      migrationsCounter.IncrementParameter.Value = new IntValue(1);
      migrationsCounter.ValueParameter.ActualName = "Migrations";

      emigrantsSelector.Name = "Emigrants Selector (placeholder)";
      emigrantsSelector.OperatorParameter.ActualName = EmigrantsSelectorParameter.Name;

      migrator.Name = "Migrator (placeholder)";
      migrator.OperatorParameter.ActualName = MigratorParameter.Name;

      immigrationReplacer.Name = "Immigration Replacer (placeholder)";
      immigrationReplacer.OperatorParameter.ActualName = ImmigrationReplacerParameter.Name;

      generationsComparator.Name = "Generations >= MaximumGenerations ?";
      generationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      generationsComparator.LeftSideParameter.ActualName = "Generations";
      generationsComparator.ResultParameter.ActualName = "TerminateGenerations";
      generationsComparator.RightSideParameter.ActualName = MaximumGenerationsParameter.Name;

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

      generationsTerminationCondition.Name = "Terminate?";
      generationsTerminationCondition.ConditionParameter.ActualName = "TerminateGenerations";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = uniformSubScopesProcessor0;
      uniformSubScopesProcessor0.Operator = islandVariableCreator;
      uniformSubScopesProcessor0.Successor = bestQualityMemorizer1;
      islandVariableCreator.Successor = islandBestQualityMemorizer1;
      islandBestQualityMemorizer1.Successor = islandBestAverageWorstQualityCalculator1;
      islandBestAverageWorstQualityCalculator1.Successor = islandDataTableValuesCollector1;
      islandDataTableValuesCollector1.Successor = islandQualityDifferenceCalculator1;
      islandQualityDifferenceCalculator1.Successor = islandAnalyzer1;
      islandAnalyzer1.Successor = islandResultsCollector;
      bestQualityMemorizer1.Successor = bestQualityMemorizer2;
      bestQualityMemorizer2.Successor = bestAverageWorstQualityCalculator1;
      bestAverageWorstQualityCalculator1.Successor = dataTableValuesCollector1;
      dataTableValuesCollector1.Successor = qualityDifferenceCalculator1;
      qualityDifferenceCalculator1.Successor = resultsCollector;
      resultsCollector.Successor = uniformSubScopesProcessor1;
      uniformSubScopesProcessor1.Operator = selector;
      uniformSubScopesProcessor1.Successor = generationsCounter;
      selector.Successor = subScopesProcessor1;
      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(childrenCreator);
      subScopesProcessor1.Successor = subScopesProcessor2;
      childrenCreator.Successor = uniformSubScopesProcessor2;
      uniformSubScopesProcessor2.Operator = crossover;
      uniformSubScopesProcessor2.Successor = null;
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
      mergingReducer.Successor = islandBestQualityMemorizer2;
      islandBestQualityMemorizer2.Successor = islandBestAverageWorstQualityCalculator2;
      islandBestAverageWorstQualityCalculator2.Successor = islandDataTableValuesCollector2;
      islandDataTableValuesCollector2.Successor = islandQualityDifferenceCalculator2;
      islandQualityDifferenceCalculator2.Successor = islandAnalyzer2;
      islandAnalyzer2.Successor = null;
      generationsCounter.Successor = generationsSinceLastMigrationCounter;
      generationsSinceLastMigrationCounter.Successor = migrationComparator;
      migrationComparator.Successor = migrationBranch;
      migrationBranch.TrueBranch = resetGenerationsSinceLastMigrationAssigner;
      migrationBranch.FalseBranch = null;
      migrationBranch.Successor = generationsComparator;
      resetGenerationsSinceLastMigrationAssigner.Successor = migrationsCounter;
      migrationsCounter.Successor = uniformSubScopesProcessor3;
      uniformSubScopesProcessor3.Operator = emigrantsSelector;
      uniformSubScopesProcessor3.Successor = migrator;
      migrator.Successor = uniformSubScopesProcessor4;
      uniformSubScopesProcessor4.Operator = immigrationReplacer;
      uniformSubScopesProcessor4.Successor = null;
      generationsComparator.Successor = bestQualityMemorizer3;
      bestQualityMemorizer3.Successor = bestQualityMemorizer4;
      bestQualityMemorizer4.Successor = bestAverageWorstQualityCalculator2;
      bestAverageWorstQualityCalculator2.Successor = dataTableValuesCollector2;
      dataTableValuesCollector2.Successor = qualityDifferenceCalculator2;
      qualityDifferenceCalculator2.Successor = generationsTerminationCondition;
      generationsTerminationCondition.TrueBranch = null;
      generationsTerminationCondition.FalseBranch = uniformSubScopesProcessor1;
      generationsTerminationCondition.Successor = null;
      #endregion
    }
  }
}
