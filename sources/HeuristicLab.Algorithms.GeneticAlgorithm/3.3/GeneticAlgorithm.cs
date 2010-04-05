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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.GeneticAlgorithm {
  /// <summary>
  /// A genetic algorithm.
  /// </summary>
  [Item("Genetic Algorithm", "A genetic algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class GeneticAlgorithm : EngineAlgorithm {
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
    private ValueParameter<IntValue> PopulationSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["PopulationSize"]; }
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
    private ValueParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumGenerations"]; }
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
    public IntValue PopulationSize {
      get { return PopulationSizeParameter.Value; }
      set { PopulationSizeParameter.Value = value; }
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
    public IntValue MaximumGenerations {
      get { return MaximumGenerationsParameter.Value; }
      set { MaximumGenerationsParameter.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)RandomCreator.Successor; }
    }
    private GeneticAlgorithmMainLoop GeneticAlgorithmMainLoop {
      get { return (GeneticAlgorithmMainLoop)SolutionsCreator.Successor; }
    }
    private List<ISelector> selectors;
    private IEnumerable<ISelector> Selectors {
      get { return selectors; }
    }
    #endregion

    public GeneticAlgorithm()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("PopulationSize", "The size of the population of solutions.", new IntValue(100)));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new PercentValue(0.05)));
      Parameters.Add(new OptionalConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation.", new IntValue(1)));
      Parameters.Add(new ValueParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed.", new IntValue(1000)));

      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      GeneticAlgorithmMainLoop geneticAlgorithmMainLoop = new GeneticAlgorithmMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutionsParameter.ActualName = PopulationSizeParameter.Name;
      solutionsCreator.Successor = geneticAlgorithmMainLoop;

      geneticAlgorithmMainLoop.SelectorParameter.ActualName = SelectorParameter.Name;
      geneticAlgorithmMainLoop.CrossoverParameter.ActualName = CrossoverParameter.Name;
      geneticAlgorithmMainLoop.ElitesParameter.ActualName = ElitesParameter.Name;
      geneticAlgorithmMainLoop.MaximumGenerationsParameter.ActualName = MaximumGenerationsParameter.Name;
      geneticAlgorithmMainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      geneticAlgorithmMainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      geneticAlgorithmMainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
      geneticAlgorithmMainLoop.ResultsParameter.ActualName = "Results";

      Initialze();
    }
    [StorableConstructor]
    private GeneticAlgorithm(bool deserializing) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      GeneticAlgorithm clone = (GeneticAlgorithm)base.Clone(cloner);
      clone.Initialze();
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
      ParameterizeGeneticAlgorithmMainLoop();
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
      ParameterizeGeneticAlgorithmMainLoop();
      ParameterizeSelectors();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_VisualizerChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperator(Problem.Visualizer);
      ParameterizeGeneticAlgorithmMainLoop();
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
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      ParameterizeSelectors();
    }
    private void PopulationSize_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeGeneticAlgorithmMainLoop();
      ParameterizeSelectors();
    }
    private void Visualizer_VisualizationParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeGeneticAlgorithmMainLoop();
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialze() {
      InitializeSelectors();
      UpdateSelectors();
      PopulationSizeParameter.ValueChanged += new EventHandler(PopulationSizeParameter_ValueChanged);
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      ElitesParameter.ValueChanged += new EventHandler(ElitesParameter_ValueChanged);
      Elites.ValueChanged += new EventHandler(Elites_ValueChanged);
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
    private void ParameterizeGeneticAlgorithmMainLoop() {
      GeneticAlgorithmMainLoop.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
      GeneticAlgorithmMainLoop.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      GeneticAlgorithmMainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
      GeneticAlgorithmMainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      GeneticAlgorithmMainLoop.VisualizerParameter.ActualName = Problem.VisualizerParameter.Name;
      if (Problem.Visualizer != null)
        GeneticAlgorithmMainLoop.VisualizationParameter.ActualName = Problem.Visualizer.VisualizationParameter.ActualName;
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      if (op is IStochasticOperator)
        ((IStochasticOperator)op).RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
    }
    private void InitializeSelectors() {
      selectors = new List<ISelector>();
      if (ApplicationManager.Manager != null) selectors.AddRange(ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name));
      ParameterizeSelectors();
    }
    private void ParameterizeSelectors() {
      foreach (ISelector selector in Selectors) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Value = new IntValue(2 * (PopulationSizeParameter.Value.Value - ElitesParameter.Value.Value));
        ParameterizeStochasticOperator(selector);
      }
      if (Problem != null) {
        foreach (ISingleObjectiveSelector selector in Selectors.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        }
      }
    }
    private void UpdateSelectors() {
      if (ApplicationManager.Manager != null) {
        ISelector oldSelector = SelectorParameter.Value;
        SelectorParameter.ValidValues.Clear();
        foreach (ISelector selector in Selectors.OrderBy(x => x.Name))
          SelectorParameter.ValidValues.Add(selector);
        if (oldSelector != null) {
          ISelector selector = SelectorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldSelector.GetType());
          if (selector != null) SelectorParameter.Value = selector;
        }
      }
    }
    private void UpdateCrossovers() {
      if (ApplicationManager.Manager != null) {
        ICrossover oldCrossover = CrossoverParameter.Value;
        CrossoverParameter.ValidValues.Clear();
        foreach (ICrossover crossover in Problem.Operators.OfType<ICrossover>().OrderBy(x => x.Name))
          CrossoverParameter.ValidValues.Add(crossover);
        if (oldCrossover != null) {
          ICrossover crossover = CrossoverParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldCrossover.GetType());
          if (crossover != null) CrossoverParameter.Value = crossover;
        }
      }
    }
    private void UpdateMutators() {
      if (ApplicationManager.Manager != null) {
        IManipulator oldMutator = MutatorParameter.Value;
        MutatorParameter.ValidValues.Clear();
        foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>().OrderBy(x => x.Name))
          MutatorParameter.ValidValues.Add(mutator);
        if (oldMutator != null) {
          IManipulator mutator = MutatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMutator.GetType());
          if (mutator != null) MutatorParameter.Value = mutator;
        }
      }
    }
    #endregion
  }
}
