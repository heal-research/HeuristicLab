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
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.GeneticAlgorithm {
  /// <summary>
  /// An island genetic algorithm main loop operator.
  /// </summary>
  [Item("IslandGeneticAlgorithmMainLoop", "An island genetic algorithm main loop operator.")]
  [StorableType("E015F135-AA23-4425-A84A-DEF880185D0A")]
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
    public IValueLookupParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["ReevaluateElites"]; }
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
    public LookupParameter<IntValue> IslandGenerations {
      get { return (LookupParameter<IntValue>)Parameters["IslandGenerations"]; }
    }
    public LookupParameter<IntValue> IslandEvaluatedSolutions {
      get { return (LookupParameter<IntValue>)Parameters["IslandEvaluatedSolutions"]; }
    }
    public ValueLookupParameter<BoolValue> Migrate {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Migrate"]; }
    }
    #endregion

    [StorableConstructor]
    private IslandGeneticAlgorithmMainLoop(StorableConstructorFlag _) : base(_) { }
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
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection to store the results."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to the analyze the islands."));
      Parameters.Add(new ValueLookupParameter<IOperator>("IslandAnalyzer", "The operator used to analyze each island."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of times a solution has been evaluated."));
      Parameters.Add(new LookupParameter<IntValue>("IslandGenerations", "The number of generations calculated on one island."));
      Parameters.Add(new LookupParameter<IntValue>("IslandEvaluatedSolutions", "The number of times a solution has been evaluated on one island."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Migrate", "Migrate the island?"));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor0 = new UniformSubScopesProcessor();
      VariableCreator islandVariableCreator = new VariableCreator();
      Placeholder islandAnalyzer1 = new Placeholder();
      LocalRandomCreator localRandomCreator = new LocalRandomCreator();
      Placeholder analyzer1 = new Placeholder();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      UniformSubScopesProcessor uniformSubScopesProcessor1 = new UniformSubScopesProcessor();
      Assigner generationsAssigner = new Assigner();
      Assigner evaluatedSolutionsAssigner = new Assigner();
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
      IntCounter islandGenerationsCounter = new IntCounter();
      Comparator checkIslandGenerationsReachedMaximum = new Comparator();
      ConditionalBranch checkContinueEvolution = new ConditionalBranch();
      DataReducer generationsReducer = new DataReducer();
      DataReducer evaluatedSolutionsReducer = new DataReducer();
      Placeholder islandAnalyzer2 = new Placeholder();
      UniformSubScopesProcessor uniformSubScopesProcessor5 = new UniformSubScopesProcessor();
      Placeholder emigrantsSelector = new Placeholder();
      IntCounter migrationsCounter = new IntCounter();
      Placeholder migrator = new Placeholder();
      UniformSubScopesProcessor uniformSubScopesProcessor6 = new UniformSubScopesProcessor();
      Placeholder immigrationReplacer = new Placeholder();
      Comparator generationsComparator = new Comparator();
      Placeholder analyzer2 = new Placeholder();
      ConditionalBranch generationsTerminationCondition = new ConditionalBranch();
      ConditionalBranch reevaluateElitesBranch = new ConditionalBranch();


      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Migrations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("GenerationsSinceLastMigration", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0))); // Class IslandGeneticAlgorithm expects this to be called Generations

      islandVariableCreator.CollectedValues.Add(new ValueParameter<ResultCollection>("Results", new ResultCollection()));
      islandVariableCreator.CollectedValues.Add(new ValueParameter<IntValue>("IslandGenerations", new IntValue(0)));
      islandVariableCreator.CollectedValues.Add(new ValueParameter<IntValue>("IslandEvaluatedSolutions", new IntValue(0)));

      islandAnalyzer1.Name = "Island Analyzer (placeholder)";
      islandAnalyzer1.OperatorParameter.ActualName = IslandAnalyzerParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Migrations"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector1.CollectedValues.Add(new ScopeTreeLookupParameter<ResultCollection>("IslandResults", "Result set for each island", "Results"));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      uniformSubScopesProcessor1.Parallel.Value = true;

      generationsAssigner.Name = "Initialize Island Generations";
      generationsAssigner.LeftSideParameter.ActualName = IslandGenerations.Name;
      generationsAssigner.RightSideParameter.Value = new IntValue(0);

      evaluatedSolutionsAssigner.Name = "Initialize Island evaluated solutions";
      evaluatedSolutionsAssigner.LeftSideParameter.ActualName = IslandEvaluatedSolutions.Name;
      evaluatedSolutionsAssigner.RightSideParameter.Value = new IntValue(0);

      selector.Name = "Selector (placeholder)";
      selector.OperatorParameter.ActualName = SelectorParameter.Name;

      childrenCreator.ParentsPerChild = new IntValue(2);

      crossover.Name = "Crossover (placeholder)";
      crossover.OperatorParameter.ActualName = CrossoverParameter.Name;

      stochasticBranch.ProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      //set it to the random number generator of the island
      stochasticBranch.RandomParameter.ActualName = "LocalRandom";

      mutator.Name = "Mutator (placeholder)";
      mutator.OperatorParameter.ActualName = MutatorParameter.Name;

      subScopesRemover.RemoveAllSubScopes = true;

      evaluator.Name = "Evaluator (placeholder)";
      evaluator.OperatorParameter.ActualName = EvaluatorParameter.Name;

      subScopesCounter.Name = "Increment EvaluatedSolutions";
      subScopesCounter.ValueParameter.ActualName = IslandEvaluatedSolutions.Name;

      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = ElitesParameter.Name;
      bestSelector.QualityParameter.ActualName = QualityParameter.Name;

      islandGenerationsCounter.Name = "Increment island generatrions";
      islandGenerationsCounter.ValueParameter.ActualName = IslandGenerations.Name;
      islandGenerationsCounter.Increment = new IntValue(1);

      checkIslandGenerationsReachedMaximum.LeftSideParameter.ActualName = IslandGenerations.Name;
      checkIslandGenerationsReachedMaximum.RightSideParameter.ActualName = MigrationIntervalParameter.Name;
      checkIslandGenerationsReachedMaximum.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      checkIslandGenerationsReachedMaximum.ResultParameter.ActualName = Migrate.Name;

      checkContinueEvolution.Name = "Migrate?";
      checkContinueEvolution.ConditionParameter.ActualName = Migrate.Name;
      checkContinueEvolution.FalseBranch = selector;

      islandAnalyzer2.Name = "Island Analyzer (placeholder)";
      islandAnalyzer2.OperatorParameter.ActualName = IslandAnalyzerParameter.Name;

      generationsReducer.Name = "Increment Generations";
      generationsReducer.ParameterToReduce.ActualName = islandGenerationsCounter.ValueParameter.ActualName;
      generationsReducer.TargetParameter.ActualName = "Generations";
      generationsReducer.ReductionOperation.Value = new ReductionOperation(ReductionOperations.Min);
      generationsReducer.TargetOperation.Value = new ReductionOperation(ReductionOperations.Sum);

      evaluatedSolutionsReducer.Name = "Increment Evaluated Solutions";
      evaluatedSolutionsReducer.ParameterToReduce.ActualName = IslandEvaluatedSolutions.Name;
      evaluatedSolutionsReducer.TargetParameter.ActualName = EvaluatedSolutionsParameter.Name;
      evaluatedSolutionsReducer.ReductionOperation.Value = new ReductionOperation(ReductionOperations.Sum);
      evaluatedSolutionsReducer.TargetOperation.Value = new ReductionOperation(ReductionOperations.Sum);

      emigrantsSelector.Name = "Emigrants Selector (placeholder)";
      emigrantsSelector.OperatorParameter.ActualName = EmigrantsSelectorParameter.Name;

      migrationsCounter.Name = "Increment number of Migrations";
      migrationsCounter.ValueParameter.ActualName = "Migrations";
      migrationsCounter.Increment = new IntValue(1);

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

      reevaluateElitesBranch.ConditionParameter.ActualName = "ReevaluateElites";
      reevaluateElitesBranch.Name = "Reevaluate elites ?";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = uniformSubScopesProcessor0;
      uniformSubScopesProcessor0.Operator = islandVariableCreator;
      uniformSubScopesProcessor0.Successor = analyzer1;
      islandVariableCreator.Successor = islandAnalyzer1;
      // BackwardsCompatibility3.3
      //the local randoms are created by the island GA itself and are only here to ensure same algorithm results
      #region Backwards compatible code, remove local random creator with 3.4 and rewire the operator graph
      islandAnalyzer1.Successor = localRandomCreator;
      localRandomCreator.Successor = null;
      #endregion
      analyzer1.Successor = resultsCollector1;
      resultsCollector1.Successor = uniformSubScopesProcessor1;
      uniformSubScopesProcessor1.Operator = generationsAssigner;
      uniformSubScopesProcessor1.Successor = generationsReducer;
      generationsReducer.Successor = evaluatedSolutionsReducer;
      evaluatedSolutionsReducer.Successor = migrationsCounter;
      migrationsCounter.Successor = uniformSubScopesProcessor5;
      generationsAssigner.Successor = evaluatedSolutionsAssigner;
      evaluatedSolutionsAssigner.Successor = selector;
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
      mergingReducer.Successor = islandAnalyzer2;
      bestSelector.Successor = rightReducer;
      rightReducer.Successor = reevaluateElitesBranch;
      reevaluateElitesBranch.TrueBranch = uniformSubScopesProcessor3;
      reevaluateElitesBranch.FalseBranch = null;
      reevaluateElitesBranch.Successor = null;
      islandAnalyzer2.Successor = islandGenerationsCounter;
      islandGenerationsCounter.Successor = checkIslandGenerationsReachedMaximum;
      checkIslandGenerationsReachedMaximum.Successor = checkContinueEvolution;
      uniformSubScopesProcessor5.Operator = emigrantsSelector;
      emigrantsSelector.Successor = null;
      uniformSubScopesProcessor5.Successor = migrator;
      migrator.Successor = uniformSubScopesProcessor6;
      uniformSubScopesProcessor6.Operator = immigrationReplacer;
      uniformSubScopesProcessor6.Successor = generationsComparator;
      generationsComparator.Successor = analyzer2;
      analyzer2.Successor = generationsTerminationCondition;
      generationsTerminationCondition.TrueBranch = null;
      generationsTerminationCondition.FalseBranch = uniformSubScopesProcessor1;
      generationsTerminationCondition.Successor = null;
      #endregion
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("ReevaluateElites")) {
        Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));
      }
      #endregion
    }

    public override IOperation Apply() {
      if (CrossoverParameter.ActualValue == null)
        return null;
      return base.Apply();
    }
  }
}
