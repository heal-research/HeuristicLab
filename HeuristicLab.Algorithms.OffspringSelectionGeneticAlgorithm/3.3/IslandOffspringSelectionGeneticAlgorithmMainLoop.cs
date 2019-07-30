#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm {
  /// <summary>
  /// An island offspring selection genetic algorithm main loop operator.
  /// </summary>
  [Item("IslandOffspringSelectionGeneticAlgorithmMainLoop", "An island offspring selection genetic algorithm main loop operator.")]
  [StorableType("C7F1B472-A58A-46DA-AF59-C9D971AF9F3A")]
  public sealed class IslandOffspringSelectionGeneticAlgorithmMainLoop : AlgorithmOperator {
    #region Parameter Properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
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
    public IValueLookupParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["ReevaluateElites"]; }
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
    public ValueLookupParameter<DoubleValue> ComparisonFactorStartParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorStart"]; }
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
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    public ValueLookupParameter<IOperator> IslandAnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["IslandAnalyzer"]; }
    }
    public LookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (LookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public IValueLookupParameter<BoolValue> FillPopulationWithParentsParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["FillPopulationWithParents"]; }
    }
    #endregion

    [StorableConstructor]
    private IslandOffspringSelectionGeneticAlgorithmMainLoop(StorableConstructorFlag _) : base(_) { }
    private IslandOffspringSelectionGeneticAlgorithmMainLoop(IslandOffspringSelectionGeneticAlgorithmMainLoop original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IslandOffspringSelectionGeneticAlgorithmMainLoop(this, cloner);
    }
    public IslandOffspringSelectionGeneticAlgorithmMainLoop()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
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
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection to store the results."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Visualizer", "The operator used to visualize solutions."));
      Parameters.Add(new LookupParameter<IItem>("Visualization", "The item which represents the visualization of solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new LookupParameter<DoubleValue>("ComparisonFactor", "The comparison factor is used to determine whether the offspring should be compared to the better parent, the worse parent or a quality value linearly interpolated between them. It is in the range [0;1]."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorStart", "The initial value for the comparison factor."));
      Parameters.Add(new ValueLookupParameter<IOperator>("ComparisonFactorModifier", "The operator used to modify the comparison factor."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to the analyze the islands."));
      Parameters.Add(new ValueLookupParameter<IOperator>("IslandAnalyzer", "The operator used to analyze each island."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of times solutions have been evaluated."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("FillPopulationWithParents", "True if the population should be filled with parent individual or false if worse children should be used when the maximum selection pressure is exceeded."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor0 = new UniformSubScopesProcessor();
      VariableCreator islandVariableCreator = new VariableCreator();
      Placeholder islandAnalyzer1 = new Placeholder();
      ResultsCollector islandResultsCollector1 = new ResultsCollector();
      Assigner comparisonFactorInitializer = new Assigner();
      Placeholder analyzer1 = new Placeholder();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      ConditionalBranch islandTerminatedBySelectionPressure1 = new ConditionalBranch();
      OffspringSelectionGeneticAlgorithmMainOperator mainOperator = new OffspringSelectionGeneticAlgorithmMainOperator();
      Placeholder islandAnalyzer2 = new Placeholder();
      ResultsCollector islandResultsCollector2 = new ResultsCollector();
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
      Comparator maxEvaluatedSolutionsComparator = new Comparator();
      Placeholder comparisonFactorModifier = new Placeholder();
      Placeholder analyzer2 = new Placeholder();
      ConditionalBranch generationsTerminationCondition = new ConditionalBranch();
      ConditionalBranch terminatedIslandsCondition = new ConditionalBranch();
      ConditionalBranch evaluatedSolutionsTerminationCondition = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Migrations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0))); // Class IslandOffspringSelectionGeneticAlgorithm expects this to be called Generations
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("GenerationsSinceLastMigration", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("TerminatedIslands", new IntValue(0)));

      islandVariableCreator.CollectedValues.Add(new ValueParameter<ResultCollection>(ResultsParameter.Name, new ResultCollection()));
      islandVariableCreator.CollectedValues.Add(new ValueParameter<BoolValue>("TerminateSelectionPressure", new BoolValue(false)));
      islandVariableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("SelectionPressure", new DoubleValue(0)));

      islandAnalyzer1.Name = "Island Analyzer (placeholder)";
      islandAnalyzer1.OperatorParameter.ActualName = IslandAnalyzerParameter.Name;

      islandResultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", "Displays the rising selection pressure during a generation.", "SelectionPressure"));
      islandResultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", "Indicates how many successful children were already found during a generation (relative to the population size).", "CurrentSuccessRatio"));
      islandResultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      comparisonFactorInitializer.Name = "Initialize Comparison Factor";
      comparisonFactorInitializer.LeftSideParameter.ActualName = ComparisonFactorParameter.Name;
      comparisonFactorInitializer.RightSideParameter.ActualName = ComparisonFactorStartParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector1.CopyValue = new BoolValue(false);
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Migrations"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Comparison Factor", null, ComparisonFactorParameter.Name));
      resultsCollector1.CollectedValues.Add(new ScopeTreeLookupParameter<ResultCollection>("IslandResults", "Result set for each island", ResultsParameter.Name));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      islandTerminatedBySelectionPressure1.Name = "Island Terminated ?";
      islandTerminatedBySelectionPressure1.ConditionParameter.ActualName = "TerminateSelectionPressure";

      mainOperator.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      mainOperator.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainOperator.CurrentSuccessRatioParameter.ActualName = "CurrentSuccessRatio";
      mainOperator.ElitesParameter.ActualName = ElitesParameter.Name;
      mainOperator.ReevaluateElitesParameter.ActualName = ReevaluateElitesParameter.Name;
      mainOperator.EvaluatedSolutionsParameter.ActualName = EvaluatedSolutionsParameter.Name;
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
      mainOperator.FillPopulationWithParentsParameter.ActualName = FillPopulationWithParentsParameter.Name;

      islandAnalyzer2.Name = "Island Analyzer (placeholder)";
      islandAnalyzer2.OperatorParameter.ActualName = IslandAnalyzerParameter.Name;

      islandResultsCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", "Displays the rising selection pressure during a generation.", "SelectionPressure"));
      islandResultsCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", "Indicates how many successful children were already found during a generation (relative to the population size).", "CurrentSuccessRatio"));
      islandResultsCollector2.ResultsParameter.ActualName = "Results";

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

      maxEvaluatedSolutionsComparator.Name = "EvaluatedSolutions >= MaximumEvaluatedSolutions ?";
      maxEvaluatedSolutionsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maxEvaluatedSolutionsComparator.LeftSideParameter.ActualName = EvaluatedSolutionsParameter.Name;
      maxEvaluatedSolutionsComparator.ResultParameter.ActualName = "TerminateEvaluatedSolutions";
      maxEvaluatedSolutionsComparator.RightSideParameter.ActualName = "MaximumEvaluatedSolutions";

      comparisonFactorModifier.Name = "Update Comparison Factor (Placeholder)";
      comparisonFactorModifier.OperatorParameter.ActualName = ComparisonFactorModifierParameter.Name;

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      generationsTerminationCondition.Name = "Terminate (MaxGenerations) ?";
      generationsTerminationCondition.ConditionParameter.ActualName = "TerminateGenerations";

      terminatedIslandsCondition.Name = "Terminate (TerminatedIslands) ?";
      terminatedIslandsCondition.ConditionParameter.ActualName = "TerminateTerminatedIslands";

      evaluatedSolutionsTerminationCondition.Name = "Terminate (EvaluatedSolutions) ?";
      evaluatedSolutionsTerminationCondition.ConditionParameter.ActualName = "TerminateEvaluatedSolutions";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = uniformSubScopesProcessor0;
      uniformSubScopesProcessor0.Operator = islandVariableCreator;
      uniformSubScopesProcessor0.Successor = comparisonFactorInitializer;
      islandVariableCreator.Successor = islandAnalyzer1;
      islandAnalyzer1.Successor = islandResultsCollector1;
      islandResultsCollector1.Successor = null;
      comparisonFactorInitializer.Successor = analyzer1;
      analyzer1.Successor = resultsCollector1;
      resultsCollector1.Successor = uniformSubScopesProcessor1;
      uniformSubScopesProcessor1.Operator = islandTerminatedBySelectionPressure1;
      uniformSubScopesProcessor1.Successor = generationsCounter;
      islandTerminatedBySelectionPressure1.TrueBranch = null;
      islandTerminatedBySelectionPressure1.FalseBranch = mainOperator;
      islandTerminatedBySelectionPressure1.Successor = null;
      mainOperator.Successor = islandAnalyzer2;
      islandAnalyzer2.Successor = islandResultsCollector2;
      islandResultsCollector2.Successor = islandSelectionPressureComparator;
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
      terminatedIslandsComparator.Successor = maxEvaluatedSolutionsComparator;
      maxEvaluatedSolutionsComparator.Successor = comparisonFactorModifier;
      comparisonFactorModifier.Successor = analyzer2;
      analyzer2.Successor = generationsTerminationCondition;
      generationsTerminationCondition.TrueBranch = null;
      generationsTerminationCondition.FalseBranch = terminatedIslandsCondition;
      generationsTerminationCondition.Successor = null;
      terminatedIslandsCondition.TrueBranch = null;
      terminatedIslandsCondition.FalseBranch = evaluatedSolutionsTerminationCondition;
      terminatedIslandsCondition.Successor = null;
      evaluatedSolutionsTerminationCondition.TrueBranch = null;
      evaluatedSolutionsTerminationCondition.FalseBranch = uniformSubScopesProcessor1;
      evaluatedSolutionsTerminationCondition.Successor = null;
      #endregion
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("ReevaluateElites")) {
        Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));
      }
      if (!Parameters.ContainsKey("FillPopulationWithParents"))
        Parameters.Add(new ValueLookupParameter<BoolValue>("FillPopulationWithParents", "True if the population should be filled with parent individual or false if worse children should be used when the maximum selection pressure is exceeded."));
      #endregion
    }

    public override IOperation Apply() {
      if (CrossoverParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
