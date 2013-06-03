#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm {
  /// <summary>
  /// An offspring selection island genetic algorithm.
  /// </summary>
  [Item("Island Offspring Selection Genetic Algorithm", "An island offspring selection genetic algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class IslandOffspringSelectionGeneticAlgorithm : HeuristicOptimizationEngineAlgorithm, IStorableContent {
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
    private IFixedValueParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["ReevaluateElites"]; }
    }
    private ValueLookupParameter<DoubleValue> SuccessRatioParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    private ValueLookupParameter<DoubleValue> ComparisonFactorLowerBoundParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorLowerBound"]; }
    }
    private ValueLookupParameter<DoubleValue> ComparisonFactorUpperBoundParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorUpperBound"]; }
    }
    public IConstrainedValueParameter<IDiscreteDoubleValueModifier> ComparisonFactorModifierParameter {
      get { return (IConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters["ComparisonFactorModifier"]; }
    }
    private ValueLookupParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    private ValueLookupParameter<BoolValue> OffspringSelectionBeforeMutationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["OffspringSelectionBeforeMutation"]; }
    }
    private ValueLookupParameter<IntValue> SelectedParentsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["SelectedParents"]; }
    }
    private ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    private ValueParameter<MultiAnalyzer> IslandAnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["IslandAnalyzer"]; }
    }
    private ValueParameter<IntValue> MaximumEvaluatedSolutionsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumEvaluatedSolutions"]; }
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
    public bool ReevaluteElites {
      get { return ReevaluateElitesParameter.Value.Value; }
      set { ReevaluateElitesParameter.Value.Value = value; }
    }
    private DoubleValue SuccessRatio {
      get { return SuccessRatioParameter.Value; }
      set { SuccessRatioParameter.Value = value; }
    }
    private DoubleValue ComparisonFactorLowerBound {
      get { return ComparisonFactorLowerBoundParameter.Value; }
      set { ComparisonFactorLowerBoundParameter.Value = value; }
    }
    private DoubleValue ComparisonFactorUpperBound {
      get { return ComparisonFactorUpperBoundParameter.Value; }
      set { ComparisonFactorUpperBoundParameter.Value = value; }
    }
    public IDiscreteDoubleValueModifier ComparisonFactorModifier {
      get { return ComparisonFactorModifierParameter.Value; }
      set { ComparisonFactorModifierParameter.Value = value; }
    }
    private DoubleValue MaximumSelectionPressure {
      get { return MaximumSelectionPressureParameter.Value; }
      set { MaximumSelectionPressureParameter.Value = value; }
    }
    private BoolValue OffspringSelectionBeforeMutation {
      get { return OffspringSelectionBeforeMutationParameter.Value; }
      set { OffspringSelectionBeforeMutationParameter.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    public MultiAnalyzer IslandAnalyzer {
      get { return IslandAnalyzerParameter.Value; }
      set { IslandAnalyzerParameter.Value = value; }
    }
    public IntValue MaximumEvaluatedSolutions {
      get { return MaximumEvaluatedSolutionsParameter.Value; }
      set { MaximumEvaluatedSolutionsParameter.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private UniformSubScopesProcessor IslandProcessor {
      get { return ((RandomCreator.Successor as SubScopesCreator).Successor as UniformSubScopesProcessor); }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)IslandProcessor.Operator; }
    }
    private IslandOffspringSelectionGeneticAlgorithmMainLoop MainLoop {
      get { return FindMainLoop(IslandProcessor.Successor); }
    }
    [Storable]
    private BestAverageWorstQualityAnalyzer islandQualityAnalyzer;
    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;
    [Storable]
    private ValueAnalyzer islandSelectionPressureAnalyzer;
    [Storable]
    private ValueAnalyzer selectionPressureAnalyzer;
    [Storable]
    private SuccessfulOffspringAnalyzer successfulOffspringAnalyzer;
    #endregion

    [StorableConstructor]
    private IslandOffspringSelectionGeneticAlgorithm(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      #region Backwards Compatibility
      if (successfulOffspringAnalyzer == null)
        successfulOffspringAnalyzer = new SuccessfulOffspringAnalyzer();
      if (!Parameters.ContainsKey("ReevaluateElites")) {
        Parameters.Add(new FixedValueParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)", (BoolValue)new BoolValue(false).AsReadOnly()) { Hidden = true });
      }
      #endregion

      Initialize();
    }
    private IslandOffspringSelectionGeneticAlgorithm(IslandOffspringSelectionGeneticAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      islandQualityAnalyzer = cloner.Clone(original.islandQualityAnalyzer);
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      islandSelectionPressureAnalyzer = cloner.Clone(original.islandSelectionPressureAnalyzer);
      selectionPressureAnalyzer = cloner.Clone(original.selectionPressureAnalyzer);
      successfulOffspringAnalyzer = cloner.Clone(original.successfulOffspringAnalyzer);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IslandOffspringSelectionGeneticAlgorithm(this, cloner);
    }
    public IslandOffspringSelectionGeneticAlgorithm()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("NumberOfIslands", "The number of islands.", new IntValue(5)));
      Parameters.Add(new ValueParameter<IntValue>("MigrationInterval", "The number of generations that should pass between migration phases.", new IntValue(20)));
      Parameters.Add(new ValueParameter<PercentValue>("MigrationRate", "The proportion of individuals that should migrate between the islands.", new PercentValue(0.15)));
      Parameters.Add(new ConstrainedValueParameter<IMigrator>("Migrator", "The migration strategy."));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("EmigrantsSelector", "Selects the individuals that will be migrated."));
      Parameters.Add(new ConstrainedValueParameter<IReplacer>("ImmigrationReplacer", "Replaces part of the original population with the immigrants."));
      Parameters.Add(new ValueParameter<IntValue>("PopulationSize", "The size of the population of solutions of each island.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MaximumGenerations", "The maximum number of generations that should be processed.", new IntValue(100)));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new PercentValue(0.05)));
      Parameters.Add(new OptionalConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation.", new IntValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)", new BoolValue(false)) { Hidden = true });
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved.", new DoubleValue(1)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorLowerBound", "The lower bound of the comparison factor (start).", new DoubleValue(0)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorUpperBound", "The upper bound of the comparison factor (end).", new DoubleValue(1)));
      Parameters.Add(new OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>("ComparisonFactorModifier", "The operator used to modify the comparison factor.", new ItemSet<IDiscreteDoubleValueModifier>(new IDiscreteDoubleValueModifier[] { new LinearDiscreteDoubleValueModifier() }), new LinearDiscreteDoubleValueModifier()));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm.", new DoubleValue(100)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation.", new BoolValue(false)));
      Parameters.Add(new ValueLookupParameter<IntValue>("SelectedParents", "How much parents should be selected each time the offspring selection step is performed until the population is filled. This parameter should be about the same or twice the size of PopulationSize for smaller problems, and less for large problems.", new IntValue(200)));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze the islands.", new MultiAnalyzer()));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("IslandAnalyzer", "The operator used to analyze each island.", new MultiAnalyzer()));
      Parameters.Add(new ValueParameter<IntValue>("MaximumEvaluatedSolutions", "The maximum number of evaluated solutions (approximately).", new IntValue(int.MaxValue)));

      RandomCreator randomCreator = new RandomCreator();
      SubScopesCreator populationCreator = new SubScopesCreator();
      UniformSubScopesProcessor ussp1 = new UniformSubScopesProcessor();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      VariableCreator variableCreator = new VariableCreator();
      UniformSubScopesProcessor ussp2 = new UniformSubScopesProcessor();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      ResultsCollector resultsCollector = new ResultsCollector();
      IslandOffspringSelectionGeneticAlgorithmMainLoop mainLoop = new IslandOffspringSelectionGeneticAlgorithmMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = populationCreator;

      populationCreator.NumberOfSubScopesParameter.ActualName = NumberOfIslandsParameter.Name;
      populationCreator.Successor = ussp1;

      ussp1.Operator = solutionsCreator;
      ussp1.Successor = variableCreator;

      solutionsCreator.NumberOfSolutionsParameter.ActualName = PopulationSizeParameter.Name;
      solutionsCreator.Successor = null;

      variableCreator.Name = "Initialize EvaluatedSolutions";
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue()));
      variableCreator.Successor = ussp2;

      ussp2.Operator = subScopesCounter;
      ussp2.Successor = resultsCollector;

      subScopesCounter.Name = "Increment EvaluatedSolutions";
      subScopesCounter.ValueParameter.ActualName = "EvaluatedSolutions";

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", "", "EvaluatedSolutions"));
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
      mainLoop.ReevaluateElitesParameter.ActualName = ReevaluateElitesParameter.Name;
      mainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      mainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainLoop.RandomParameter.ActualName = randomCreator.RandomParameter.ActualName;
      mainLoop.ResultsParameter.ActualName = "Results";
      mainLoop.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      mainLoop.ComparisonFactorParameter.ActualName = "ComparisonFactor";
      mainLoop.ComparisonFactorStartParameter.ActualName = ComparisonFactorLowerBoundParameter.Name;
      mainLoop.ComparisonFactorModifierParameter.ActualName = ComparisonFactorModifierParameter.Name;
      mainLoop.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      mainLoop.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
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

      foreach (IDiscreteDoubleValueModifier modifier in ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>().OrderBy(x => x.Name))
        ComparisonFactorModifierParameter.ValidValues.Add(modifier);
      IDiscreteDoubleValueModifier linearModifier = ComparisonFactorModifierParameter.ValidValues.FirstOrDefault(x => x.GetType().Name.Equals("LinearDiscreteDoubleValueModifier"));
      if (linearModifier != null) ComparisonFactorModifierParameter.Value = linearModifier;
      ParameterizeComparisonFactorModifiers();

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      islandQualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      selectionPressureAnalyzer = new ValueAnalyzer();
      islandSelectionPressureAnalyzer = new ValueAnalyzer();
      successfulOffspringAnalyzer = new SuccessfulOffspringAnalyzer();
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
      ParameterizeStochasticOperator(Problem.Evaluator);
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
      ParameterizeStochasticOperator(Problem.Evaluator);
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
    private void MaximumMigrationsParameter_ValueChanged(object sender, EventArgs e) {
      MaximumGenerations.ValueChanged += new EventHandler(MaximumMigrations_ValueChanged);
      ParameterizeComparisonFactorModifiers();
    }
    private void MaximumMigrations_ValueChanged(object sender, EventArgs e) {
      ParameterizeComparisonFactorModifiers();
    }
    private void MigrationIntervalParameter_ValueChanged(object sender, EventArgs e) {
      MigrationInterval.ValueChanged += new EventHandler(MigrationInterval_ValueChanged);
      ParameterizeComparisonFactorModifiers();
    }
    private void MigrationInterval_ValueChanged(object sender, EventArgs e) {
      ParameterizeComparisonFactorModifiers();
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
      MigrationIntervalParameter.ValueChanged += new EventHandler(MigrationIntervalParameter_ValueChanged);
      MigrationInterval.ValueChanged += new EventHandler(MigrationInterval_ValueChanged);
      MaximumGenerationsParameter.ValueChanged += new EventHandler(MaximumMigrationsParameter_ValueChanged);
      MaximumGenerations.ValueChanged += new EventHandler(MaximumMigrations_ValueChanged);
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
      if (op is IStochasticOperator)
        ((IStochasticOperator)op).RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
    }
    private void ParameterizeSelectors() {
      foreach (ISelector selector in SelectorParameter.ValidValues) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Value = null;
        selector.NumberOfSelectedSubScopesParameter.ActualName = SelectedParentsParameter.Name;
        ParameterizeStochasticOperator(selector);
      }
      foreach (ISelector selector in EmigrantsSelectorParameter.ValidValues) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Value = new IntValue((int)Math.Ceiling(PopulationSize.Value * MigrationRate.Value));
        ParameterizeStochasticOperator(selector);
      }
      foreach (IReplacer selector in ImmigrationReplacerParameter.ValidValues) {
        ParameterizeStochasticOperator(selector);
      }
      if (Problem != null) {
        foreach (ISingleObjectiveSelector selector in SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        }
        foreach (ISingleObjectiveSelector selector in EmigrantsSelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        }
        foreach (ISingleObjectiveReplacer replacer in ImmigrationReplacerParameter.ValidValues.OfType<ISingleObjectiveReplacer>()) {
          replacer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          replacer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        }
      }
    }
    private void ParameterizeAnalyzers() {
      islandQualityAnalyzer.ResultsParameter.ActualName = "Results";
      islandQualityAnalyzer.QualityParameter.Depth = 1;
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      qualityAnalyzer.QualityParameter.Depth = 2;

      islandSelectionPressureAnalyzer.ResultsParameter.ActualName = "Results";
      islandSelectionPressureAnalyzer.Name = "SelectionPressure Analyzer";
      islandSelectionPressureAnalyzer.ValueParameter.Depth = 0;
      islandSelectionPressureAnalyzer.ValueParameter.ActualName = "SelectionPressure";
      islandSelectionPressureAnalyzer.ValuesParameter.ActualName = "Selection Pressure History";

      selectionPressureAnalyzer.ResultsParameter.ActualName = "Results";
      selectionPressureAnalyzer.Name = "SelectionPressure Analyzer";
      selectionPressureAnalyzer.ValueParameter.Depth = 1;
      selectionPressureAnalyzer.ValueParameter.ActualName = "SelectionPressure";
      selectionPressureAnalyzer.ValuesParameter.ActualName = "Selection Pressure History";

      successfulOffspringAnalyzer.ResultsParameter.ActualName = "Results";
      successfulOffspringAnalyzer.GenerationsParameter.ActualName = "Generations";
      successfulOffspringAnalyzer.SuccessfulOffspringFlagParameter.Value.Value = "SuccessfulOffspring";
      successfulOffspringAnalyzer.DepthParameter.Value = new IntValue(2);

      if (Problem != null) {
        islandQualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        islandQualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        islandQualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;

        qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
      }
    }
    private void ParameterizeComparisonFactorModifiers() {
      foreach (IDiscreteDoubleValueModifier modifier in ComparisonFactorModifierParameter.ValidValues) {
        modifier.IndexParameter.ActualName = "Generations";
        modifier.EndIndexParameter.Value = new IntValue(MigrationInterval.Value * MaximumGenerations.Value);
        modifier.EndValueParameter.ActualName = ComparisonFactorUpperBoundParameter.Name;
        modifier.StartIndexParameter.Value = new IntValue(0);
        modifier.StartValueParameter.ActualName = ComparisonFactorLowerBoundParameter.Name;
        modifier.ValueParameter.ActualName = "ComparisonFactor";
      }
    }
    private void ParameterizeIterationBasedOperators() {
      if (Problem != null) {
        foreach (IIterationBasedOperator op in Problem.Operators.OfType<IIterationBasedOperator>()) {
          op.IterationsParameter.ActualName = "Generations";
          op.MaximumIterationsParameter.ActualName = MaximumGenerationsParameter.Name;
        }
      }
    }
    private void UpdateCrossovers() {
      ICrossover oldCrossover = CrossoverParameter.Value;
      ICrossover defaultCrossover = Problem.Operators.OfType<ICrossover>().FirstOrDefault();
      CrossoverParameter.ValidValues.Clear();
      foreach (ICrossover crossover in Problem.Operators.OfType<ICrossover>().OrderBy(x => x.Name))
        CrossoverParameter.ValidValues.Add(crossover);
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
      foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>().OrderBy(x => x.Name))
        MutatorParameter.ValidValues.Add(mutator);
      if (oldMutator != null) {
        IManipulator mutator = MutatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMutator.GetType());
        if (mutator != null) MutatorParameter.Value = mutator;
      }
    }
    private void UpdateAnalyzers() {
      IslandAnalyzer.Operators.Clear();
      Analyzer.Operators.Clear();
      IslandAnalyzer.Operators.Add(islandQualityAnalyzer, islandQualityAnalyzer.EnabledByDefault);
      IslandAnalyzer.Operators.Add(islandSelectionPressureAnalyzer, islandSelectionPressureAnalyzer.EnabledByDefault);
      if (Problem != null) {
        foreach (IAnalyzer analyzer in Problem.Operators.OfType<IAnalyzer>()) {
          foreach (IScopeTreeLookupParameter param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 2;
          Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
        }
      }
      Analyzer.Operators.Add(qualityAnalyzer, qualityAnalyzer.EnabledByDefault);
      Analyzer.Operators.Add(selectionPressureAnalyzer, selectionPressureAnalyzer.EnabledByDefault);
      Analyzer.Operators.Add(successfulOffspringAnalyzer, successfulOffspringAnalyzer.EnabledByDefault);
    }
    private IslandOffspringSelectionGeneticAlgorithmMainLoop FindMainLoop(IOperator start) {
      IOperator mainLoop = start;
      while (mainLoop != null && !(mainLoop is IslandOffspringSelectionGeneticAlgorithmMainLoop))
        mainLoop = ((SingleSuccessorOperator)mainLoop).Successor;
      if (mainLoop == null) return null;
      else return (IslandOffspringSelectionGeneticAlgorithmMainLoop)mainLoop;
    }
    #endregion
  }
}
