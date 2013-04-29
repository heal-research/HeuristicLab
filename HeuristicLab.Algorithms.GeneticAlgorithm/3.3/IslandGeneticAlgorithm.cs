#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.GeneticAlgorithm {
  /// <summary>
  /// An island genetic algorithm.
  /// </summary>
  [Item("Island Genetic Algorithm", "An island genetic algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class IslandGeneticAlgorithm : HeuristicOptimizationEngineAlgorithm, IStorableContent {
    public string Filename { get; set; }

    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(ISingleObjectiveHeuristicOptimizationProblem); }
    }
    public new ISingleObjectiveHeuristicOptimizationProblem Problem {
      get { return (ISingleObjectiveHeuristicOptimizationProblem)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion

    #region Parameter Properties
    private ValueParameter<IntValue> SeedParameter {
      get { return (ValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private ValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (ValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    private ValueParameter<IntValue> NumberOfIslandsParameter {
      get { return (ValueParameter<IntValue>)Parameters["NumberOfIslands"]; }
    }
    private ValueParameter<IntValue> MigrationIntervalParameter {
      get { return (ValueParameter<IntValue>)Parameters["MigrationInterval"]; }
    }
    private ValueParameter<PercentValue> MigrationRateParameter {
      get { return (ValueParameter<PercentValue>)Parameters["MigrationRate"]; }
    }
    public IConstrainedValueParameter<IMigrator> MigratorParameter {
      get { return (IConstrainedValueParameter<IMigrator>)Parameters["Migrator"]; }
    }
    public IConstrainedValueParameter<ISelector> EmigrantsSelectorParameter {
      get { return (IConstrainedValueParameter<ISelector>)Parameters["EmigrantsSelector"]; }
    }
    public IConstrainedValueParameter<IReplacer> ImmigrationReplacerParameter {
      get { return (IConstrainedValueParameter<IReplacer>)Parameters["ImmigrationReplacer"]; }
    }
    private ValueParameter<IntValue> PopulationSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    private ValueParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }
    public IConstrainedValueParameter<ISelector> SelectorParameter {
      get { return (IConstrainedValueParameter<ISelector>)Parameters["Selector"]; }
    }
    public IConstrainedValueParameter<ICrossover> CrossoverParameter {
      get { return (IConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    }
    private ValueParameter<PercentValue> MutationProbabilityParameter {
      get { return (ValueParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    public IConstrainedValueParameter<IManipulator> MutatorParameter {
      get { return (IConstrainedValueParameter<IManipulator>)Parameters["Mutator"]; }
    }
    private ValueParameter<IntValue> ElitesParameter {
      get { return (ValueParameter<IntValue>)Parameters["Elites"]; }
    }
    private ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    private ValueParameter<MultiAnalyzer> IslandAnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["IslandAnalyzer"]; }
    }
    #endregion

    #region Properties
    public IntValue Seed {
      get { return SeedParameter.Value; }
      set { SeedParameter.Value = value; }
    }
    public BoolValue SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value; }
      set { SetSeedRandomlyParameter.Value = value; }
    }
    public IntValue NumberOfIslands {
      get { return NumberOfIslandsParameter.Value; }
      set { NumberOfIslandsParameter.Value = value; }
    }
    public IntValue MigrationInterval {
      get { return MigrationIntervalParameter.Value; }
      set { MigrationIntervalParameter.Value = value; }
    }
    public PercentValue MigrationRate {
      get { return MigrationRateParameter.Value; }
      set { MigrationRateParameter.Value = value; }
    }
    public IMigrator Migrator {
      get { return MigratorParameter.Value; }
      set { MigratorParameter.Value = value; }
    }
    public ISelector EmigrantsSelector {
      get { return EmigrantsSelectorParameter.Value; }
      set { EmigrantsSelectorParameter.Value = value; }
    }
    public IReplacer ImmigrationReplacer {
      get { return ImmigrationReplacerParameter.Value; }
      set { ImmigrationReplacerParameter.Value = value; }
    }
    public IntValue PopulationSize {
      get { return PopulationSizeParameter.Value; }
      set { PopulationSizeParameter.Value = value; }
    }
    public IntValue MaximumGenerations {
      get { return MaximumGenerationsParameter.Value; }
      set { MaximumGenerationsParameter.Value = value; }
    }
    public ISelector Selector {
      get { return SelectorParameter.Value; }
      set { SelectorParameter.Value = value; }
    }
    public ICrossover Crossover {
      get { return CrossoverParameter.Value; }
      set { CrossoverParameter.Value = value; }
    }
    public PercentValue MutationProbability {
      get { return MutationProbabilityParameter.Value; }
      set { MutationProbabilityParameter.Value = value; }
    }
    public IManipulator Mutator {
      get { return MutatorParameter.Value; }
      set { MutatorParameter.Value = value; }
    }
    public IntValue Elites {
      get { return ElitesParameter.Value; }
      set { ElitesParameter.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    public MultiAnalyzer IslandAnalyzer {
      get { return IslandAnalyzerParameter.Value; }
      set { IslandAnalyzerParameter.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private UniformSubScopesProcessor IslandProcessor {
      get { return OperatorGraph.Iterate().OfType<UniformSubScopesProcessor>().First(x => x.Operator is SolutionsCreator); }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)IslandProcessor.Operator; }
    }
    private IslandGeneticAlgorithmMainLoop MainLoop {
      get { return FindMainLoop(IslandProcessor.Successor); }
    }
    [Storable]
    private BestAverageWorstQualityAnalyzer islandQualityAnalyzer;
    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;
    #endregion

    [StorableConstructor]
    private IslandGeneticAlgorithm(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    private IslandGeneticAlgorithm(IslandGeneticAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      islandQualityAnalyzer = cloner.Clone(original.islandQualityAnalyzer);
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IslandGeneticAlgorithm(this, cloner);
    }

    public IslandGeneticAlgorithm()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("NumberOfIslands", "The number of islands.", new IntValue(5)));
      Parameters.Add(new ValueParameter<IntValue>("MigrationInterval", "The number of generations that should pass between migration phases.", new IntValue(20)));
      Parameters.Add(new ValueParameter<PercentValue>("MigrationRate", "The proportion of individuals that should migrate between the islands.", new PercentValue(0.15)));
      Parameters.Add(new ConstrainedValueParameter<IMigrator>("Migrator", "The migration strategy."));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("EmigrantsSelector", "Selects the individuals that will be migrated."));
      Parameters.Add(new ConstrainedValueParameter<IReplacer>("ImmigrationReplacer", "Selects the population from the unification of the original population and the immigrants."));
      Parameters.Add(new ValueParameter<IntValue>("PopulationSize", "The size of the population of solutions.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MaximumGenerations", "The maximum number of generations that should be processed.", new IntValue(1000)));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new PercentValue(0.05)));
      Parameters.Add(new OptionalConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation.", new IntValue(1)));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze the islands.", new MultiAnalyzer()));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("IslandAnalyzer", "The operator used to analyze each island.", new MultiAnalyzer()));

      RandomCreator randomCreator = new RandomCreator();
      UniformSubScopesProcessor ussp0 = new UniformSubScopesProcessor();
      LocalRandomCreator localRandomCreator = new LocalRandomCreator();
      RandomCreator globalRandomResetter = new RandomCreator();
      SubScopesCreator populationCreator = new SubScopesCreator();
      UniformSubScopesProcessor ussp1 = new UniformSubScopesProcessor();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      VariableCreator variableCreator = new VariableCreator();
      UniformSubScopesProcessor ussp2 = new UniformSubScopesProcessor();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      ResultsCollector resultsCollector = new ResultsCollector();
      IslandGeneticAlgorithmMainLoop mainLoop = new IslandGeneticAlgorithmMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "GlobalRandom";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = populationCreator;

      populationCreator.NumberOfSubScopesParameter.ActualName = NumberOfIslandsParameter.Name;
      populationCreator.Successor = ussp0;

      ussp0.Operator = localRandomCreator;
      ussp0.Successor = globalRandomResetter;

      // BackwardsCompatibility3.3
      // the global random is resetted to ensure the same algorithm results
      #region Backwards compatible code, remove global random resetter with 3.4 and rewire the operator graph
      globalRandomResetter.RandomParameter.ActualName = "GlobalRandom";
      globalRandomResetter.SeedParameter.ActualName = SeedParameter.Name;
      globalRandomResetter.SeedParameter.Value = null;
      globalRandomResetter.SetSeedRandomlyParameter.Value = new BoolValue(false);
      globalRandomResetter.Successor = ussp1;
      #endregion

      ussp1.Operator = solutionsCreator;
      ussp1.Successor = variableCreator;

      solutionsCreator.NumberOfSolutionsParameter.ActualName = PopulationSizeParameter.Name;
      //don't create solutions in parallel because the hive engine would distribute these tasks
      solutionsCreator.ParallelParameter.Value = new BoolValue(false);
      solutionsCreator.Successor = null;

      variableCreator.Name = "Initialize EvaluatedSolutions";
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue()));
      variableCreator.Successor = ussp2;

      ussp2.Operator = subScopesCounter;
      ussp2.Successor = resultsCollector;

      subScopesCounter.Name = "Count EvaluatedSolutions";
      subScopesCounter.ValueParameter.ActualName = "EvaluatedSolutions";
      subScopesCounter.Successor = null;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "EvaluatedSolutions"));
      resultsCollector.ResultsParameter.ActualName = "Results";
      resultsCollector.Successor = mainLoop;

      mainLoop.EmigrantsSelectorParameter.ActualName = EmigrantsSelectorParameter.Name;
      mainLoop.ImmigrationReplacerParameter.ActualName = ImmigrationReplacerParameter.Name;
      mainLoop.MaximumGenerationsParameter.ActualName = MaximumGenerationsParameter.Name;
      mainLoop.MigrationIntervalParameter.ActualName = MigrationIntervalParameter.Name;
      mainLoop.MigrationRateParameter.ActualName = MigrationRateParameter.Name;
      mainLoop.MigratorParameter.ActualName = MigratorParameter.Name;
      mainLoop.NumberOfIslandsParameter.ActualName = NumberOfIslandsParameter.Name;
      mainLoop.SelectorParameter.ActualName = SelectorParameter.Name;
      mainLoop.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainLoop.ElitesParameter.ActualName = ElitesParameter.Name;
      mainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      mainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainLoop.RandomParameter.ActualName = randomCreator.RandomParameter.ActualName;
      mainLoop.ResultsParameter.ActualName = "Results";
      mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
      mainLoop.IslandAnalyzerParameter.ActualName = IslandAnalyzerParameter.Name;
      mainLoop.EvaluatedSolutionsParameter.ActualName = "EvaluatedSolutions";
      mainLoop.Successor = null;

      foreach (ISelector selector in ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name))
        SelectorParameter.ValidValues.Add(selector);
      ISelector proportionalSelector = SelectorParameter.ValidValues.FirstOrDefault(x => x.GetType().Name.Equals("ProportionalSelector"));
      if (proportionalSelector != null) SelectorParameter.Value = proportionalSelector;

      foreach (ISelector selector in ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name))
        EmigrantsSelectorParameter.ValidValues.Add(selector);

      foreach (IReplacer replacer in ApplicationManager.Manager.GetInstances<IReplacer>().OrderBy(x => x.Name))
        ImmigrationReplacerParameter.ValidValues.Add(replacer);

      ParameterizeSelectors();

      foreach (IMigrator migrator in ApplicationManager.Manager.GetInstances<IMigrator>().OrderBy(x => x.Name))
        MigratorParameter.ValidValues.Add(migrator);

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      islandQualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      ParameterizeAnalyzers();
      UpdateAnalyzers();

      Initialize();
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperatorForIsland(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      ParameterizeSelectors();
      ParameterizeAnalyzers();
      ParameterizeIterationBasedOperators();
      UpdateCrossovers();
      UpdateMutators();
      UpdateAnalyzers();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.OnProblemChanged();
    }

    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeSolutionsCreator();
      base.Problem_SolutionCreatorChanged(sender, e);
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperatorForIsland(Problem.Evaluator);
      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      ParameterizeSelectors();
      ParameterizeAnalyzers();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeIterationBasedOperators();
      UpdateCrossovers();
      UpdateMutators();
      UpdateAnalyzers();
      base.Problem_OperatorsChanged(sender, e);
    }
    private void ElitesParameter_ValueChanged(object sender, EventArgs e) {
      Elites.ValueChanged += new EventHandler(Elites_ValueChanged);
      ParameterizeSelectors();
    }
    private void Elites_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }
    private void PopulationSizeParameter_ValueChanged(object sender, EventArgs e) {
      NumberOfIslands.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      ParameterizeSelectors();
    }
    private void PopulationSize_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeSelectors();
      ParameterizeAnalyzers();
    }
    private void MigrationRateParameter_ValueChanged(object sender, EventArgs e) {
      MigrationRate.ValueChanged += new EventHandler(MigrationRate_ValueChanged);
      ParameterizeSelectors();
    }
    private void MigrationRate_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }
    #endregion

    #region Helpers
    private void Initialize() {
      PopulationSizeParameter.ValueChanged += new EventHandler(PopulationSizeParameter_ValueChanged);
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      MigrationRateParameter.ValueChanged += new EventHandler(MigrationRateParameter_ValueChanged);
      MigrationRate.ValueChanged += new EventHandler(MigrationRate_ValueChanged);
      ElitesParameter.ValueChanged += new EventHandler(ElitesParameter_ValueChanged);
      Elites.ValueChanged += new EventHandler(Elites_ValueChanged);
      if (Problem != null) {
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      }
    }
    private void ParameterizeSolutionsCreator() {
      SolutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      SolutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
    }
    private void ParameterizeMainLoop() {
      MainLoop.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
      MainLoop.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      MainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
      MainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      IStochasticOperator stochasticOp = op as IStochasticOperator;
      if (stochasticOp != null) {
        stochasticOp.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
        stochasticOp.RandomParameter.Hidden = true;
      }
    }
    private void ParameterizeStochasticOperatorForIsland(IOperator op) {
      IStochasticOperator stochasticOp = op as IStochasticOperator;
      if (stochasticOp != null) {
        stochasticOp.RandomParameter.ActualName = "LocalRandom";
        stochasticOp.RandomParameter.Hidden = true;
      }
    }
    private void ParameterizeSelectors() {
      foreach (ISelector selector in SelectorParameter.ValidValues) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Value = new IntValue(2 * (PopulationSize.Value - Elites.Value));
        selector.NumberOfSelectedSubScopesParameter.Hidden = true;
        ParameterizeStochasticOperatorForIsland(selector);
      }
      foreach (ISelector selector in EmigrantsSelectorParameter.ValidValues) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Value = new IntValue((int)Math.Ceiling(PopulationSize.Value * MigrationRate.Value));
        selector.NumberOfSelectedSubScopesParameter.Hidden = true;
        ParameterizeStochasticOperator(selector);
      }
      foreach (IReplacer replacer in ImmigrationReplacerParameter.ValidValues) {
        ParameterizeStochasticOperator(replacer);
      }
      if (Problem != null) {
        foreach (ISingleObjectiveSelector selector in SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.MaximizationParameter.Hidden = true;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
          selector.QualityParameter.Hidden = true;
        }
        foreach (ISingleObjectiveSelector selector in EmigrantsSelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.MaximizationParameter.Hidden = true;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
          selector.QualityParameter.Hidden = true;
        }
        foreach (ISingleObjectiveReplacer selector in ImmigrationReplacerParameter.ValidValues.OfType<ISingleObjectiveReplacer>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.MaximizationParameter.Hidden = true;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
          selector.QualityParameter.Hidden = true;
        }
      }
    }
    private void ParameterizeAnalyzers() {
      islandQualityAnalyzer.ResultsParameter.ActualName = "Results";
      islandQualityAnalyzer.ResultsParameter.Hidden = true;
      islandQualityAnalyzer.QualityParameter.Depth = 1;
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      qualityAnalyzer.ResultsParameter.Hidden = true;
      qualityAnalyzer.QualityParameter.Depth = 2;

      if (Problem != null) {
        islandQualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        islandQualityAnalyzer.MaximizationParameter.Hidden = true;
        islandQualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        islandQualityAnalyzer.QualityParameter.Hidden = true;
        islandQualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        islandQualityAnalyzer.BestKnownQualityParameter.Hidden = true;
        qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        qualityAnalyzer.MaximizationParameter.Hidden = true;
        qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        qualityAnalyzer.QualityParameter.Hidden = true;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        qualityAnalyzer.BestKnownQualityParameter.Hidden = true;
      }
    }
    private void ParameterizeIterationBasedOperators() {
      if (Problem != null) {
        foreach (IIterationBasedOperator op in Problem.Operators.OfType<IIterationBasedOperator>()) {
          op.IterationsParameter.ActualName = "Generations";
          op.IterationsParameter.Hidden = true;
          op.MaximumIterationsParameter.ActualName = "MaximumGenerations";
          op.MaximumIterationsParameter.Hidden = true;
        }
      }
    }
    private void UpdateCrossovers() {
      ICrossover oldCrossover = CrossoverParameter.Value;
      ICrossover defaultCrossover = Problem.Operators.OfType<ICrossover>().FirstOrDefault();
      CrossoverParameter.ValidValues.Clear();
      foreach (ICrossover crossover in Problem.Operators.OfType<ICrossover>().OrderBy(x => x.Name)) {
        ParameterizeStochasticOperatorForIsland(crossover);
        CrossoverParameter.ValidValues.Add(crossover);
      }
      if (oldCrossover != null) {
        ICrossover crossover = CrossoverParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldCrossover.GetType());
        if (crossover != null) CrossoverParameter.Value = crossover;
        else oldCrossover = null;
      }
      if (oldCrossover == null && defaultCrossover != null)
        CrossoverParameter.Value = defaultCrossover;
    }
    private void UpdateMutators() {
      IManipulator oldMutator = MutatorParameter.Value;
      MutatorParameter.ValidValues.Clear();
      foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>().OrderBy(x => x.Name)) {
        ParameterizeStochasticOperatorForIsland(mutator);
        MutatorParameter.ValidValues.Add(mutator);
      }
      if (oldMutator != null) {
        IManipulator mutator = MutatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMutator.GetType());
        if (mutator != null) MutatorParameter.Value = mutator;
      }
    }
    private void UpdateAnalyzers() {
      IslandAnalyzer.Operators.Clear();
      Analyzer.Operators.Clear();
      IslandAnalyzer.Operators.Add(islandQualityAnalyzer, islandQualityAnalyzer.EnabledByDefault);
      if (Problem != null) {
        foreach (IAnalyzer analyzer in Problem.Operators.OfType<IAnalyzer>()) {
          foreach (IScopeTreeLookupParameter param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 2;
          Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
        }
      }
      Analyzer.Operators.Add(qualityAnalyzer, qualityAnalyzer.EnabledByDefault);
    }
    private IslandGeneticAlgorithmMainLoop FindMainLoop(IOperator start) {
      IOperator mainLoop = start;
      while (mainLoop != null && !(mainLoop is IslandGeneticAlgorithmMainLoop))
        mainLoop = ((SingleSuccessorOperator)mainLoop).Successor;
      if (mainLoop == null) return null;
      else return (IslandGeneticAlgorithmMainLoop)mainLoop;
    }
    #endregion
  }
}
