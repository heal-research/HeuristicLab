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
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.GeneticAlgorithm {
  /// <summary>
  /// An island genetic algorithm.
  /// </summary>
  [Item("Island Genetic Algorithm", "An island genetic algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class IslandGeneticAlgorithm : EngineAlgorithm {

    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(ISingleObjectiveProblem); }
    }
    public new ISingleObjectiveProblem Problem {
      get { return (ISingleObjectiveProblem)base.Problem; }
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
    private ConstrainedValueParameter<IMigrator> MigratorParameter {
      get { return (ConstrainedValueParameter<IMigrator>)Parameters["Migrator"]; }
    }
    private ConstrainedValueParameter<ISelector> EmigrantsSelectorParameter {
      get { return (ConstrainedValueParameter<ISelector>)Parameters["EmigrantsSelector"]; }
    }
    private ConstrainedValueParameter<IReplacer> ImmigrationReplacerParameter {
      get { return (ConstrainedValueParameter<IReplacer>)Parameters["ImmigrationReplacer"]; }
    }
    private ValueParameter<IntValue> PopulationSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    private ValueParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }
    private ConstrainedValueParameter<ISelector> SelectorParameter {
      get { return (ConstrainedValueParameter<ISelector>)Parameters["Selector"]; }
    }
    private ConstrainedValueParameter<ICrossover> CrossoverParameter {
      get { return (ConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    }
    private ValueParameter<PercentValue> MutationProbabilityParameter {
      get { return (ValueParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    private OptionalConstrainedValueParameter<IManipulator> MutatorParameter {
      get { return (OptionalConstrainedValueParameter<IManipulator>)Parameters["Mutator"]; }
    }
    private ValueParameter<IntValue> ElitesParameter {
      get { return (ValueParameter<IntValue>)Parameters["Elites"]; }
    }
    private ValueParameter<BoolValue> ParallelParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Parallel"]; }
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
    public BoolValue Parallel {
      get { return ParallelParameter.Value; }
      set { ParallelParameter.Value = value; }
    }
    private List<ISelector> selectors;
    private IEnumerable<ISelector> Selectors {
      get { return selectors; }
    }
    private List<ISelector> emigrantsSelectors;
    private List<IReplacer> immigrationReplacers;
    private List<IMigrator> migrators;
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private UniformSubScopesProcessor IslandProcessor {
      get { return ((RandomCreator.Successor as SubScopesCreator).Successor as UniformSubScopesProcessor); }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)IslandProcessor.Operator; }
    }
    private IslandGeneticAlgorithmMainLoop MainLoop {
      get { return (IslandGeneticAlgorithmMainLoop)IslandProcessor.Successor; }
    }
    #endregion

    [StorableConstructor]
    private IslandGeneticAlgorithm(bool deserializing) : base(deserializing) { }
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
      Parameters.Add(new ValueParameter<BoolValue>("Parallel", "True if the islands should be run in parallel (also requires a parallel engine)", new BoolValue(false)));

      RandomCreator randomCreator = new RandomCreator();
      SubScopesCreator populationCreator = new SubScopesCreator();
      UniformSubScopesProcessor ussp1 = new UniformSubScopesProcessor();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      IslandGeneticAlgorithmMainLoop mainLoop = new IslandGeneticAlgorithmMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = populationCreator;

      populationCreator.NumberOfSubScopesParameter.ActualName = NumberOfIslandsParameter.Name;
      populationCreator.Successor = ussp1;

      ussp1.Parallel = null;
      ussp1.ParallelParameter.ActualName = ParallelParameter.Name;
      ussp1.Operator = solutionsCreator;
      ussp1.Successor = mainLoop;

      solutionsCreator.NumberOfSolutionsParameter.ActualName = PopulationSizeParameter.Name;
      solutionsCreator.Successor = null;

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

      mainLoop.Successor = null;

      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      IslandGeneticAlgorithm clone = (IslandGeneticAlgorithm)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators) ParameterizeStochasticOperator(op);
      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      ParameterizeSelectors();
      UpdateCrossovers();
      UpdateMutators();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.OnProblemChanged();
    }

    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeSolutionsCreator();
      base.Problem_SolutionCreatorChanged(sender, e);
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperator(Problem.Evaluator);
      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      ParameterizeSelectors();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      foreach (IOperator op in Problem.Operators) ParameterizeStochasticOperator(op);
      UpdateCrossovers();
      UpdateMutators();
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
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      InitializeSelectors();
      UpdateSelectors();
      InitializeMigrators();
      UpdateMigrators();
      PopulationSizeParameter.ValueChanged += new EventHandler(PopulationSizeParameter_ValueChanged);
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      MigrationRateParameter.ValueChanged += new EventHandler(MigrationRateParameter_ValueChanged);
      MigrationRate.ValueChanged += new EventHandler(MigrationRate_ValueChanged);
      ElitesParameter.ValueChanged += new EventHandler(ElitesParameter_ValueChanged);
      Elites.ValueChanged += new EventHandler(Elites_ValueChanged);
      if (Problem != null) {
        UpdateCrossovers();
        UpdateMutators();
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
      if (op is IStochasticOperator)
        ((IStochasticOperator)op).RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
    }
    private void InitializeSelectors() {
      selectors = new List<ISelector>();
      selectors.AddRange(ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name));
      emigrantsSelectors = new List<ISelector>();
      emigrantsSelectors.AddRange(ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name));
      immigrationReplacers = new List<IReplacer>();
      immigrationReplacers.AddRange(ApplicationManager.Manager.GetInstances<IReplacer>().OrderBy(x => x.Name));
      ParameterizeSelectors();
    }
    private void InitializeMigrators() {
      migrators = new List<IMigrator>();
      migrators.AddRange(ApplicationManager.Manager.GetInstances<IMigrator>().OrderBy(x => x.Name));
      UpdateMigrators();
    }
    private void ParameterizeSelectors() {
      foreach (ISelector selector in Selectors) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Value = new IntValue(2 * (PopulationSize.Value - Elites.Value));
        ParameterizeStochasticOperator(selector);
      }
      foreach (ISelector selector in emigrantsSelectors) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Value = new IntValue((int)Math.Ceiling(PopulationSize.Value * MigrationRate.Value));
        ParameterizeStochasticOperator(selector);
      }
      foreach (IReplacer replacer in immigrationReplacers) {
        ParameterizeStochasticOperator(replacer);
      }
      if (Problem != null) {
        foreach (ISingleObjectiveSelector selector in Selectors.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        }
        foreach (ISingleObjectiveSelector selector in emigrantsSelectors.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        }
        foreach (ISingleObjectiveReplacer selector in immigrationReplacers.OfType<ISingleObjectiveReplacer>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        }
      }
    }
    private void UpdateSelectors() {
      ISelector oldSelector = SelectorParameter.Value;
      SelectorParameter.ValidValues.Clear();
      foreach (ISelector selector in Selectors.OrderBy(x => x.Name))
        SelectorParameter.ValidValues.Add(selector);

      ISelector proportionalSelector = SelectorParameter.ValidValues.FirstOrDefault(x => x.GetType().Name.Equals("ProportionalSelector"));
      if (proportionalSelector != null) SelectorParameter.Value = proportionalSelector;

      if (oldSelector != null) {
        ISelector selector = SelectorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldSelector.GetType());
        if (selector != null) SelectorParameter.Value = selector;
      }

      oldSelector = EmigrantsSelector;
      EmigrantsSelectorParameter.ValidValues.Clear();
      foreach (ISelector selector in emigrantsSelectors)
        EmigrantsSelectorParameter.ValidValues.Add(selector);
      if (oldSelector != null) {
        ISelector selector = EmigrantsSelectorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldSelector.GetType());
        if (selector != null) EmigrantsSelectorParameter.Value = selector;
      }

      IReplacer oldReplacer = ImmigrationReplacerParameter.Value;
      ImmigrationReplacerParameter.ValidValues.Clear();
      foreach (IReplacer replacer in immigrationReplacers)
        ImmigrationReplacerParameter.ValidValues.Add(replacer);
      if (oldReplacer != null) {
        IReplacer replacer = ImmigrationReplacerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldSelector.GetType());
        if (replacer != null) ImmigrationReplacerParameter.Value = replacer;
      }
    }
    private void UpdateMigrators() {
      IMigrator oldMigrator = Migrator;
      MigratorParameter.ValidValues.Clear();
      foreach (IMigrator migrator in migrators)
        MigratorParameter.ValidValues.Add(migrator);
      if (oldMigrator != null) {
        IMigrator migrator = MigratorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMigrator.GetType());
        if (migrator != null) MigratorParameter.Value = migrator;
      } else if (MigratorParameter.ValidValues.Count > 0) MigratorParameter.Value = MigratorParameter.ValidValues.First();
    }
    private void UpdateCrossovers() {
      ICrossover oldCrossover = CrossoverParameter.Value;
      CrossoverParameter.ValidValues.Clear();
      foreach (ICrossover crossover in Problem.Operators.OfType<ICrossover>().OrderBy(x => x.Name))
        CrossoverParameter.ValidValues.Add(crossover);
      if (oldCrossover != null) {
        ICrossover crossover = CrossoverParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldCrossover.GetType());
        if (crossover != null) CrossoverParameter.Value = crossover;
      }
    }
    private void UpdateMutators() {
      IManipulator oldMutator = MutatorParameter.Value;
      MutatorParameter.ValidValues.Clear();
      foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>().OrderBy(x => x.Name))
        MutatorParameter.ValidValues.Add(mutator);
      if (oldMutator != null) {
        IManipulator mutator = MutatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMutator.GetType());
        if (mutator != null) MutatorParameter.Value = mutator;
      }
    }
    #endregion
  }
}
