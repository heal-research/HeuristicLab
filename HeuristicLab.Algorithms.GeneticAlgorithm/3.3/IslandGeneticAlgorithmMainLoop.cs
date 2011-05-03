#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
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
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
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
    #endregion

    [StorableConstructor]
    private IslandGeneticAlgorithmMainLoop(bool deserializing) : base(deserializing) { }
    private IslandGeneticAlgorithmMainLoop(IslandGeneticAlgorithmMainLoop original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IslandGeneticAlgorithmMainLoop(this, cloner);
    }
    public IslandGeneticAlgorithmMainLoop()
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
      Parameters.Add(new ValueLookupParameter<IOperator>("ImmigrationReplacer", "Replaces some of the original population with the immigrants."));
      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The size of the population of solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations that the algorithm should process."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection to store the results."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to the analyze the islands."));
      Parameters.Add(new ValueLookupParameter<IOperator>("IslandAnalyzer", "The operator used to analyze each island."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of times a solution has been evaluated."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor0 = new UniformSubScopesProcessor();
      VariableCreator islandVariableCreator = new VariableCreator();
      Placeholder islandAnalyzer1 = new Placeholder();
      Placeholder analyzer1 = new Placeholder();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      Placeholder selector = new Placeholder();
      SubScopesProcessor subScopesProcessor1 = new SubScopesProcessor();
      ChildrenCreator childrenCreator = new ChildrenCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor2 = new UniformSubScopesProcessor();
      Placeholder crossover = new Placeholder();
      StochasticBranch stochasticBranch = new StochasticBranch();
      Placeholder mutator = new Placeholder();
      SubScopesRemover subScopesRemover = new SubScopesRemover();
      UniformSubScopesProcessor uniformSubScopesProcessor3 = new UniformSubScopesProcessor();
      Placeholder evaluator = new Placeholder();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      SubScopesProcessor subScopesProcessor2 = new SubScopesProcessor();
      BestSelector bestSelector = new BestSelector();
      RightReducer rightReducer = new RightReducer();
      MergingReducer mergingReducer = new MergingReducer();
      IntCounter generationsCounter = new IntCounter();
      UniformSubScopesProcessor uniformSubScopesProcessor4 = new UniformSubScopesProcessor();
      Placeholder islandAnalyzer2 = new Placeholder();
      IntCounter generationsSinceLastMigrationCounter = new IntCounter();
      Comparator migrationComparator = new Comparator();
      ConditionalBranch migrationBranch = new ConditionalBranch();
      Assigner resetGenerationsSinceLastMigrationAssigner = new Assigner();
      IntCounter migrationsCounter = new IntCounter();
      UniformSubScopesProcessor uniformSubScopesProcessor5 = new UniformSubScopesProcessor();
      Placeholder emigrantsSelector = new Placeholder();
      Placeholder migrator = new Placeholder();
      UniformSubScopesProcessor uniformSubScopesProcessor6 = new UniformSubScopesProcessor();
      Placeholder immigrationReplacer = new Placeholder();
      Comparator generationsComparator = new Comparator();
      Placeholder analyzer2 = new Placeholder();
      ConditionalBranch generationsTerminationCondition = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Migrations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("GenerationsSinceLastMigration", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0))); // Class IslandGeneticAlgorithm expects this to be called Generations

      islandVariableCreator.CollectedValues.Add(new ValueParameter<ResultCollection>("Results", new ResultCollection()));

      islandAnalyzer1.Name = "Island Analyzer (placeholder)";
      islandAnalyzer1.OperatorParameter.ActualName = IslandAnalyzerParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Migrations"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector1.CollectedValues.Add(new ScopeTreeLookupParameter<ResultCollection>("IslandResults", "Result set for each island", "Results"));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      selector.Name = "Selector (placeholder)";
      selector.OperatorParameter.ActualName = SelectorParameter.Name;

      childrenCreator.ParentsPerChild = new IntValue(2);

      crossover.Name = "Crossover (placeholder)";
      crossover.OperatorParameter.ActualName = CrossoverParameter.Name;

      stochasticBranch.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      stochasticBranch.RandomParameter.ActualName = RandomParameter.Name;

      mutator.Name = "Mutator (placeholder)";
      mutator.OperatorParameter.ActualName = MutatorParameter.Name;

      subScopesRemover.RemoveAllSubScopes = true;

      uniformSubScopesProcessor3.Parallel.Value = true;

      evaluator.Name = "Evaluator (placeholder)";
      evaluator.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesCounter.Name = "Increment EvaluatedSolutions";
      subScopesCounter.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = ElitesParameter.Name;
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;

      islandAnalyzer2.Name = "Island Analyzer (placeholder)";
      islandAnalyzer2.OperatorParameter.ActualName = IslandAnalyzerParameter.Name;

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

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      generationsTerminationCondition.Name = "Terminate?";
      generationsTerminationCondition.ConditionParameter.ActualName = "TerminateGenerations";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = uniformSubScopesProcessor0;
      uniformSubScopesProcessor0.Operator = islandVariableCreator;
      uniformSubScopesProcessor0.Successor = analyzer1;
      islandVariableCreator.Successor = islandAnalyzer1;
      islandAnalyzer1.Successor = null;
      analyzer1.Successor = resultsCollector1;
      resultsCollector1.Successor = uniformSubScopesProcessor1;
      uniformSubScopesProcessor1.Operator = selector;
      uniformSubScopesProcessor1.Successor = generationsCounter;
      selector.Successor = subScopesProcessor1;
      subScopesProcessor1.Operators.Add(new EmptyOperator());
      subScopesProcessor1.Operators.Add(childrenCreator);
      subScopesProcessor1.Successor = subScopesProcessor2;
      childrenCreator.Successor = uniformSubScopesProcessor2;
      uniformSubScopesProcessor2.Operator = crossover;
      uniformSubScopesProcessor2.Successor = uniformSubScopesProcessor3;
      crossover.Successor = stochasticBranch;
      stochasticBranch.FirstBranch = mutator;
      stochasticBranch.SecondBranch = null;
      stochasticBranch.Successor = subScopesRemover;
      mutator.Successor = null;
      subScopesRemover.Successor = null;
      uniformSubScopesProcessor3.Operator = evaluator;
      uniformSubScopesProcessor3.Successor = subScopesCounter;
      evaluator.Successor = null;
      subScopesCounter.Successor = null;
      subScopesProcessor2.Operators.Add(bestSelector);
      subScopesProcessor2.Operators.Add(new EmptyOperator());
      subScopesProcessor2.Successor = mergingReducer;
      bestSelector.Successor = rightReducer;
      rightReducer.Successor = null;
      mergingReducer.Successor = null;
      generationsCounter.Successor = uniformSubScopesProcessor4;
      uniformSubScopesProcessor4.Operator = islandAnalyzer2;
      uniformSubScopesProcessor4.Successor = generationsSinceLastMigrationCounter;
      islandAnalyzer2.Successor = null;
      generationsSinceLastMigrationCounter.Successor = migrationComparator;
      migrationComparator.Successor = migrationBranch;
      migrationBranch.TrueBranch = resetGenerationsSinceLastMigrationAssigner;
      migrationBranch.FalseBranch = null;
      migrationBranch.Successor = generationsComparator;
      resetGenerationsSinceLastMigrationAssigner.Successor = migrationsCounter;
      migrationsCounter.Successor = uniformSubScopesProcessor5;
      uniformSubScopesProcessor5.Operator = emigrantsSelector;
      uniformSubScopesProcessor5.Successor = migrator;
      migrator.Successor = uniformSubScopesProcessor6;
      uniformSubScopesProcessor6.Operator = immigrationReplacer;
      uniformSubScopesProcessor6.Successor = null;
      generationsComparator.Successor = analyzer2;
      analyzer2.Successor = generationsTerminationCondition;
      generationsTerminationCondition.TrueBranch = null;
      generationsTerminationCondition.FalseBranch = uniformSubScopesProcessor1;
      generationsTerminationCondition.Successor = null;
      #endregion
    }

    public override IOperation Apply() {
      if (CrossoverParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
