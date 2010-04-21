﻿#region License Information
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

namespace HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm {
  /// <summary>
  /// The self-adaptive segregative genetic algorithm with simulated annealing aspects (Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications. CRC Press).
  /// </summary>
  [Item("SASEGASA", "The self-adaptive segregative genetic algorithm with simulated annealing aspects (Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications. CRC Press).")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class SASEGASA : EngineAlgorithm {

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
    private ValueParameter<IntValue> NumberOfVillagesParameter {
      get { return (ValueParameter<IntValue>)Parameters["NumberOfVillages"]; }
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
    private ValueLookupParameter<DoubleValue> SuccessRatioParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    private ValueLookupParameter<DoubleValue> ComparisonFactorLowerBoundParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorLowerBound"]; }
    }
    private ValueLookupParameter<DoubleValue> ComparisonFactorUpperBoundParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorUpperBound"]; }
    }
    private OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier> ComparisonFactorModifierParameter {
      get { return (OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters["ComparisonFactorModifier"]; }
    }
    private ValueLookupParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    private ValueLookupParameter<BoolValue> OffspringSelectionBeforeMutationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["OffspringSelectionBeforeMutation"]; }
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
    public BoolValue Parallel {
      get { return ParallelParameter.Value; }
      set { ParallelParameter.Value = value; }
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
    private IDiscreteDoubleValueModifier ComparisonFactorModifier {
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
    private List<ISelector> selectors;
    private IEnumerable<ISelector> Selectors {
      get { return selectors; }
    }
    private List<IDiscreteDoubleValueModifier> comparisonFactorModifiers;
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
      get { return (SASEGASAMainLoop)VillageProcessor.Successor; }
    }
    #endregion

    [StorableConstructor]
    private SASEGASA(bool deserializing) : base(deserializing) { }
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
      Parameters.Add(new ValueParameter<BoolValue>("Parallel", "True if the villages should be run in parallel (also requires a parallel engine)", new BoolValue(false)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved.", new DoubleValue(1)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorLowerBound", "The lower bound of the comparison factor (start).", new DoubleValue(0.3)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorUpperBound", "The upper bound of the comparison factor (end).", new DoubleValue(0.7)));
      Parameters.Add(new OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>("ComparisonFactorModifier", "The operator used to modify the comparison factor.", new ItemSet<IDiscreteDoubleValueModifier>(new IDiscreteDoubleValueModifier[] { new LinearDiscreteDoubleValueModifier() }), new LinearDiscreteDoubleValueModifier()));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm.", new DoubleValue(100)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation.", new BoolValue(false)));

      RandomCreator randomCreator = new RandomCreator();
      SubScopesCreator populationCreator = new SubScopesCreator();
      UniformSubScopesProcessor ussp1 = new UniformSubScopesProcessor();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
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

      ussp1.Parallel = null;
      ussp1.ParallelParameter.ActualName = ParallelParameter.Name;
      ussp1.Operator = solutionsCreator;
      ussp1.Successor = mainLoop;

      solutionsCreator.NumberOfSolutionsParameter.ActualName = PopulationSizeParameter.Name;
      solutionsCreator.Successor = null;


      mainLoop.NumberOfVillagesParameter.ActualName = NumberOfVillagesParameter.Name;
      mainLoop.SelectorParameter.ActualName = SelectorParameter.Name;
      mainLoop.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainLoop.ElitesParameter.ActualName = ElitesParameter.Name;
      mainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      mainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainLoop.RandomParameter.ActualName = randomCreator.RandomParameter.ActualName;
      mainLoop.ResultsParameter.ActualName = "Results";
      mainLoop.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      mainLoop.ComparisonFactorLowerBoundParameter.ActualName = ComparisonFactorLowerBoundParameter.Name;
      mainLoop.ComparisonFactorModifierParameter.ActualName = ComparisonFactorModifierParameter.Name;
      mainLoop.ComparisonFactorUpperBoundParameter.ActualName = ComparisonFactorUpperBoundParameter.Name;
      mainLoop.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      mainLoop.MaximumGenerationsParameter.ActualName = MaximumGenerationsParameter.Name;
      mainLoop.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
      mainLoop.Successor = null;

      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SASEGASA clone = (SASEGASA)base.Clone(cloner);
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
      ParameterizeStochasticOperator(Problem.Visualizer);
      foreach (IOperator op in Problem.Operators) ParameterizeStochasticOperator(op);
      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      ParameterizeSelectors();
      UpdateCrossovers();
      UpdateMutators();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      if (Problem.Visualizer != null) Problem.Visualizer.VisualizationParameter.ActualNameChanged += new EventHandler(Visualizer_VisualizationParameter_ActualNameChanged);
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
    protected override void Problem_VisualizerChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperator(Problem.Visualizer);
      ParameterizeMainLoop();
      if (Problem.Visualizer != null) Problem.Visualizer.VisualizationParameter.ActualNameChanged += new EventHandler(Visualizer_VisualizationParameter_ActualNameChanged);
      base.Problem_VisualizerChanged(sender, e);
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
      NumberOfVillages.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      ParameterizeSelectors();
    }
    private void PopulationSize_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeSelectors();
    }
    private void Visualizer_VisualizationParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
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
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      InitializeSelectors();
      UpdateSelectors();
      InitializeComparisonFactorModifiers();
      UpdateComparisonFactorModifiers();
      NumberOfVillagesParameter.ValueChanged += new EventHandler(NumberOfVillagesParameter_ValueChanged);
      NumberOfVillages.ValueChanged += new EventHandler(NumberOfVillages_ValueChanged);
      PopulationSizeParameter.ValueChanged += new EventHandler(PopulationSizeParameter_ValueChanged);
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      ElitesParameter.ValueChanged += new EventHandler(ElitesParameter_ValueChanged);
      Elites.ValueChanged += new EventHandler(Elites_ValueChanged);
      MaximumGenerationsParameter.ValueChanged += new EventHandler(MaximumGenerationsParameter_ValueChanged);
      MaximumGenerations.ValueChanged += new EventHandler(MaximumGenerations_ValueChanged);
      if (Problem != null) {
        UpdateCrossovers();
        UpdateMutators();
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
        if (Problem.Visualizer != null) Problem.Visualizer.VisualizationParameter.ActualNameChanged += new EventHandler(Visualizer_VisualizationParameter_ActualNameChanged);
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
      MainLoop.VisualizerParameter.ActualName = Problem.VisualizerParameter.Name;
      MainLoop.MigrationIntervalParameter.Value = new IntValue(MaximumGenerations.Value / NumberOfVillages.Value);
      if (Problem.Visualizer != null)
        MainLoop.VisualizationParameter.ActualName = Problem.Visualizer.VisualizationParameter.ActualName;
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      if (op is IStochasticOperator)
        ((IStochasticOperator)op).RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
    }
    private void InitializeSelectors() {
      selectors = new List<ISelector>();
      selectors.AddRange(ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name));
      ParameterizeSelectors();
    }
    private void InitializeComparisonFactorModifiers() {
      comparisonFactorModifiers = new List<IDiscreteDoubleValueModifier>();
      comparisonFactorModifiers.AddRange(ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>().OrderBy(x => x.Name));
      ParameterizeComparisonFactorModifiers();
    }
    private void ParameterizeSelectors() {
      foreach (ISelector selector in Selectors) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Value = new IntValue(2 * (PopulationSize.Value - Elites.Value));
        ParameterizeStochasticOperator(selector);
      }
      if (Problem != null) {
        foreach (ISingleObjectiveSelector selector in Selectors.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        }
      }
    }
    private void ParameterizeComparisonFactorModifiers() {
      foreach (IDiscreteDoubleValueModifier modifier in comparisonFactorModifiers) {
        modifier.IndexParameter.ActualName = "Reunifications";
        modifier.StartIndexParameter.Value = new IntValue(0);
        modifier.StartValueParameter.ActualName = ComparisonFactorLowerBoundParameter.Name;
        modifier.EndIndexParameter.Value = new IntValue(NumberOfVillages.Value - 1);
        modifier.EndValueParameter.ActualName = ComparisonFactorUpperBoundParameter.Name;
        modifier.ValueParameter.ActualName = "ComparisonFactor";
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
    }
    private void UpdateComparisonFactorModifiers() {
      IDiscreteDoubleValueModifier oldModifier = ComparisonFactorModifier;

      ComparisonFactorModifierParameter.ValidValues.Clear();
      foreach (IDiscreteDoubleValueModifier modifier in comparisonFactorModifiers)
        ComparisonFactorModifierParameter.ValidValues.Add(modifier);

      if (oldModifier != null) {
        IDiscreteDoubleValueModifier mod = ComparisonFactorModifierParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldModifier.GetType());
        if (mod != null) ComparisonFactorModifierParameter.Value = mod;
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
    #endregion
  }
}
