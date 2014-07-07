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
  /// The self-adaptive segregative genetic algorithm with simulated annealing aspects (Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications. CRC Press).
  /// </summary>
  [Item("SASEGASA", "The self-adaptive segregative genetic algorithm with simulated annealing aspects (Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications. CRC Press).")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class SASEGASA : HeuristicOptimizationEngineAlgorithm, IStorableContent {
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
    private ValueParameter<IntValue> NumberOfVillagesParameter {
      get { return (ValueParameter<IntValue>)Parameters["NumberOfVillages"]; }
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
    private ValueLookupParameter<DoubleValue> FinalMaximumSelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["FinalMaximumSelectionPressure"]; }
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
    private ValueParameter<MultiAnalyzer> VillageAnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["VillageAnalyzer"]; }
    }
    private ValueParameter<IntValue> MaximumEvaluatedSolutionsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumEvaluatedSolutions"]; }
    }
    private IFixedValueParameter<BoolValue> FillPopulationWithParentsParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["FillPopulationWithParents"]; }
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
    public IntValue NumberOfVillages {
      get { return NumberOfVillagesParameter.Value; }
      set { NumberOfVillagesParameter.Value = value; }
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
    public DoubleValue SuccessRatio {
      get { return SuccessRatioParameter.Value; }
      set { SuccessRatioParameter.Value = value; }
    }
    public DoubleValue ComparisonFactorLowerBound {
      get { return ComparisonFactorLowerBoundParameter.Value; }
      set { ComparisonFactorLowerBoundParameter.Value = value; }
    }
    public DoubleValue ComparisonFactorUpperBound {
      get { return ComparisonFactorUpperBoundParameter.Value; }
      set { ComparisonFactorUpperBoundParameter.Value = value; }
    }
    public IDiscreteDoubleValueModifier ComparisonFactorModifier {
      get { return ComparisonFactorModifierParameter.Value; }
      set { ComparisonFactorModifierParameter.Value = value; }
    }
    public DoubleValue MaximumSelectionPressure {
      get { return MaximumSelectionPressureParameter.Value; }
      set { MaximumSelectionPressureParameter.Value = value; }
    }
    public DoubleValue FinalMaximumSelectionPressure {
      get { return FinalMaximumSelectionPressureParameter.Value; }
      set { FinalMaximumSelectionPressureParameter.Value = value; }
    }
    public BoolValue OffspringSelectionBeforeMutation {
      get { return OffspringSelectionBeforeMutationParameter.Value; }
      set { OffspringSelectionBeforeMutationParameter.Value = value; }
    }
    public IntValue SelectedParents {
      get { return SelectedParentsParameter.Value; }
      set { SelectedParentsParameter.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    public MultiAnalyzer VillageAnalyzer {
      get { return VillageAnalyzerParameter.Value; }
      set { VillageAnalyzerParameter.Value = value; }
    }
    public IntValue MaximumEvaluatedSolutions {
      get { return MaximumEvaluatedSolutionsParameter.Value; }
      set { MaximumEvaluatedSolutionsParameter.Value = value; }
    }
    public bool FillPopulationWithParents {
      get { return FillPopulationWithParentsParameter.Value.Value; }
      set { FillPopulationWithParentsParameter.Value.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private UniformSubScopesProcessor VillageProcessor {
      get { return ((RandomCreator.Successor as SubScopesCreator).Successor as UniformSubScopesProcessor); }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)VillageProcessor.Operator; }
    }
    private SASEGASAMainLoop MainLoop {
      get { return FindMainLoop(VillageProcessor.Successor); }
    }
    [Storable]
    private BestAverageWorstQualityAnalyzer villageQualityAnalyzer;
    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;
    [Storable]
    private ValueAnalyzer villageSelectionPressureAnalyzer;
    [Storable]
    private ValueAnalyzer selectionPressureAnalyzer;
    [Storable]
    private SuccessfulOffspringAnalyzer successfulOffspringAnalyzer;
    #endregion

    [StorableConstructor]
    private SASEGASA(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (successfulOffspringAnalyzer == null)
        successfulOffspringAnalyzer = new SuccessfulOffspringAnalyzer();
      if (!Parameters.ContainsKey("ReevaluateElites")) {
        Parameters.Add(new FixedValueParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)", (BoolValue)new BoolValue(false).AsReadOnly()) { Hidden = true });
      }
      if (!Parameters.ContainsKey("FillPopulationWithParents"))
        Parameters.Add(new FixedValueParameter<BoolValue>("FillPopulationWithParents", "True if the population should be filled with parent individual or false if worse children should be used when the maximum selection pressure is exceeded.", new BoolValue(false)) { Hidden = true });
      #endregion

      Initialize();
    }
    private SASEGASA(SASEGASA original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      villageQualityAnalyzer = cloner.Clone(original.villageQualityAnalyzer);
      selectionPressureAnalyzer = cloner.Clone(original.selectionPressureAnalyzer);
      villageSelectionPressureAnalyzer = cloner.Clone(original.villageSelectionPressureAnalyzer);
      successfulOffspringAnalyzer = cloner.Clone(original.successfulOffspringAnalyzer);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SASEGASA(this, cloner);
    }
    public SASEGASA()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("NumberOfVillages", "The initial number of villages.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("PopulationSize", "The size of the population of solutions.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MaximumGenerations", "The maximum number of generations that should be processed.", new IntValue(1000)));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new PercentValue(0.05)));
      Parameters.Add(new OptionalConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation.", new IntValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)", new BoolValue(false)) { Hidden = true });
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved.", new DoubleValue(1)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorLowerBound", "The lower bound of the comparison factor (start).", new DoubleValue(0.3)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorUpperBound", "The upper bound of the comparison factor (end).", new DoubleValue(0.7)));
      Parameters.Add(new OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>("ComparisonFactorModifier", "The operator used to modify the comparison factor.", new ItemSet<IDiscreteDoubleValueModifier>(new IDiscreteDoubleValueModifier[] { new LinearDiscreteDoubleValueModifier() }), new LinearDiscreteDoubleValueModifier()));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm.", new DoubleValue(100)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("FinalMaximumSelectionPressure", "The maximum selection pressure used when there is only one village left.", new DoubleValue(100)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation.", new BoolValue(false)));
      Parameters.Add(new ValueLookupParameter<IntValue>("SelectedParents", "How much parents should be selected each time the offspring selection step is performed until the population is filled. This parameter should be about the same or twice the size of PopulationSize for smaller problems, and less for large problems.", new IntValue(200)));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze the villages.", new MultiAnalyzer()));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("VillageAnalyzer", "The operator used to analyze each village.", new MultiAnalyzer()));
      Parameters.Add(new ValueParameter<IntValue>("MaximumEvaluatedSolutions", "The maximum number of evaluated solutions (approximately).", new IntValue(int.MaxValue)));
      Parameters.Add(new FixedValueParameter<BoolValue>("FillPopulationWithParents", "True if the population should be filled with parent individual or false if worse children should be used when the maximum selection pressure is exceeded.", new BoolValue(true)) { Hidden = true });

      RandomCreator randomCreator = new RandomCreator();
      SubScopesCreator populationCreator = new SubScopesCreator();
      UniformSubScopesProcessor ussp1 = new UniformSubScopesProcessor();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      VariableCreator variableCreator = new VariableCreator();
      UniformSubScopesProcessor ussp2 = new UniformSubScopesProcessor();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      ResultsCollector resultsCollector = new ResultsCollector();
      SASEGASAMainLoop mainLoop = new SASEGASAMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = populationCreator;

      populationCreator.NumberOfSubScopesParameter.ActualName = NumberOfVillagesParameter.Name;
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

      mainLoop.NumberOfVillagesParameter.ActualName = NumberOfVillagesParameter.Name;
      mainLoop.SelectorParameter.ActualName = SelectorParameter.Name;
      mainLoop.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainLoop.ElitesParameter.ActualName = ElitesParameter.Name;
      mainLoop.ReevaluateElitesParameter.ActualName = ReevaluateElitesParameter.Name;
      mainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      mainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainLoop.RandomParameter.ActualName = randomCreator.RandomParameter.ActualName;
      mainLoop.ResultsParameter.ActualName = "Results";
      mainLoop.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      mainLoop.ComparisonFactorStartParameter.ActualName = ComparisonFactorLowerBoundParameter.Name;
      mainLoop.ComparisonFactorModifierParameter.ActualName = ComparisonFactorModifierParameter.Name;
      mainLoop.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      mainLoop.FinalMaximumSelectionPressureParameter.ActualName = FinalMaximumSelectionPressureParameter.Name;
      mainLoop.MaximumGenerationsParameter.ActualName = MaximumGenerationsParameter.Name;
      mainLoop.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
      mainLoop.EvaluatedSolutionsParameter.ActualName = "EvaluatedSolutions";
      mainLoop.FillPopulationWithParentsParameter.ActualName = FillPopulationWithParentsParameter.Name;
      mainLoop.Successor = null;

      foreach (ISelector selector in ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name))
        SelectorParameter.ValidValues.Add(selector);
      ISelector proportionalSelector = SelectorParameter.ValidValues.FirstOrDefault(x => x.GetType().Name.Equals("ProportionalSelector"));
      if (proportionalSelector != null) SelectorParameter.Value = proportionalSelector;

      ParameterizeSelectors();

      foreach (IDiscreteDoubleValueModifier modifier in ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>().OrderBy(x => x.Name))
        ComparisonFactorModifierParameter.ValidValues.Add(modifier);
      IDiscreteDoubleValueModifier linearModifier = ComparisonFactorModifierParameter.ValidValues.FirstOrDefault(x => x.GetType().Name.Equals("LinearDiscreteDoubleValueModifier"));
      if (linearModifier != null) ComparisonFactorModifierParameter.Value = linearModifier;
      ParameterizeComparisonFactorModifiers();

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      villageQualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      selectionPressureAnalyzer = new ValueAnalyzer();
      villageSelectionPressureAnalyzer = new ValueAnalyzer();
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
      NumberOfVillages.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
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
    private void MaximumGenerationsParameter_ValueChanged(object sender, EventArgs e) {
      MaximumGenerations.ValueChanged += new EventHandler(MaximumGenerations_ValueChanged);
      MaximumGenerations_ValueChanged(sender, e);
    }
    private void MaximumGenerations_ValueChanged(object sender, EventArgs e) {
      if (MaximumGenerations.Value < NumberOfVillages.Value) NumberOfVillages.Value = MaximumGenerations.Value;
      ParameterizeMainLoop();
    }
    private void NumberOfVillagesParameter_ValueChanged(object sender, EventArgs e) {
      NumberOfVillages.ValueChanged += new EventHandler(NumberOfVillages_ValueChanged);
      NumberOfVillages_ValueChanged(sender, e);
    }
    private void NumberOfVillages_ValueChanged(object sender, EventArgs e) {
      if (NumberOfVillages.Value > MaximumGenerations.Value) MaximumGenerations.Value = NumberOfVillages.Value;
      ParameterizeComparisonFactorModifiers();
      ParameterizeMainLoop();
    }
    #endregion

    #region Helpers
    private void Initialize() {
      NumberOfVillagesParameter.ValueChanged += new EventHandler(NumberOfVillagesParameter_ValueChanged);
      NumberOfVillages.ValueChanged += new EventHandler(NumberOfVillages_ValueChanged);
      PopulationSizeParameter.ValueChanged += new EventHandler(PopulationSizeParameter_ValueChanged);
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      ElitesParameter.ValueChanged += new EventHandler(ElitesParameter_ValueChanged);
      Elites.ValueChanged += new EventHandler(Elites_ValueChanged);
      MaximumGenerationsParameter.ValueChanged += new EventHandler(MaximumGenerationsParameter_ValueChanged);
      MaximumGenerations.ValueChanged += new EventHandler(MaximumGenerations_ValueChanged);
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
      MainLoop.MigrationIntervalParameter.Value = new IntValue(MaximumGenerations.Value / NumberOfVillages.Value);
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
      if (Problem != null) {
        foreach (ISingleObjectiveSelector selector in SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        }
      }
    }
    private void ParameterizeAnalyzers() {
      villageQualityAnalyzer.ResultsParameter.ActualName = "Results";
      villageQualityAnalyzer.QualityParameter.Depth = 1;
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      qualityAnalyzer.QualityParameter.Depth = 2;

      villageSelectionPressureAnalyzer.ResultsParameter.ActualName = "Results";
      villageSelectionPressureAnalyzer.Name = "SelectionPressure Analyzer";
      villageSelectionPressureAnalyzer.ValueParameter.Depth = 0;
      villageSelectionPressureAnalyzer.ValueParameter.ActualName = "SelectionPressure";
      villageSelectionPressureAnalyzer.ValuesParameter.ActualName = "Selection Pressure History";

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
        villageQualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        villageQualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        villageQualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;

        qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
      }
    }
    private void ParameterizeComparisonFactorModifiers() {
      foreach (IDiscreteDoubleValueModifier modifier in ComparisonFactorModifierParameter.ValidValues) {
        modifier.IndexParameter.ActualName = "Reunifications";
        modifier.StartIndexParameter.Value = new IntValue(0);
        modifier.StartValueParameter.ActualName = ComparisonFactorLowerBoundParameter.Name;
        modifier.EndIndexParameter.Value = new IntValue(NumberOfVillages.Value - 1);
        modifier.EndValueParameter.ActualName = ComparisonFactorUpperBoundParameter.Name;
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
    private void UpdateAnalyzers() {
      VillageAnalyzer.Operators.Clear();
      Analyzer.Operators.Clear();
      VillageAnalyzer.Operators.Add(villageQualityAnalyzer, villageQualityAnalyzer.EnabledByDefault);
      VillageAnalyzer.Operators.Add(villageSelectionPressureAnalyzer, villageSelectionPressureAnalyzer.EnabledByDefault);
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
    private SASEGASAMainLoop FindMainLoop(IOperator start) {
      IOperator mainLoop = start;
      while (mainLoop != null && !(mainLoop is SASEGASAMainLoop))
        mainLoop = ((SingleSuccessorOperator)mainLoop).Successor;
      if (mainLoop == null) return null;
      else return (SASEGASAMainLoop)mainLoop;
    }
    #endregion
  }
}
