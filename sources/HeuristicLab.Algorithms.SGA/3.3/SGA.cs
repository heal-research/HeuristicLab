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
using HeuristicLab.Evolutionary;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.SGA {
  /// <summary>
  /// A standard genetic algorithm.
  /// </summary>
  [Item("SGA", "A standard genetic algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class SGA : EngineAlgorithm {
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
    private ValueParameter<IntData> SeedParameter {
      get { return (ValueParameter<IntData>)Parameters["Seed"]; }
    }
    private ValueParameter<BoolData> SetSeedRandomlyParameter {
      get { return (ValueParameter<BoolData>)Parameters["SetSeedRandomly"]; }
    }
    private ValueParameter<IntData> PopulationSizeParameter {
      get { return (ValueParameter<IntData>)Parameters["PopulationSize"]; }
    }
    private ConstrainedValueParameter<ISelector> SelectorParameter {
      get { return (ConstrainedValueParameter<ISelector>)Parameters["Selector"]; }
    }
    private ConstrainedValueParameter<ICrossover> CrossoverParameter {
      get { return (ConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    }
    private ValueParameter<DoubleData> MutationProbabilityParameter {
      get { return (ValueParameter<DoubleData>)Parameters["MutationProbability"]; }
    }
    private OptionalConstrainedValueParameter<IManipulator> MutatorParameter {
      get { return (OptionalConstrainedValueParameter<IManipulator>)Parameters["Mutator"]; }
    }
    private ValueParameter<IntData> ElitesParameter {
      get { return (ValueParameter<IntData>)Parameters["Elites"]; }
    }
    private ValueParameter<IntData> MaximumGenerationsParameter {
      get { return (ValueParameter<IntData>)Parameters["MaximumGenerations"]; }
    }
    #endregion

    #region Properties
    public IntData Seed {
      get { return SeedParameter.Value; }
      set { SeedParameter.Value = value; }
    }
    public BoolData SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value; }
      set { SetSeedRandomlyParameter.Value = value; }
    }
    public IntData PopulationSize {
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
    public DoubleData MutationProbability {
      get { return MutationProbabilityParameter.Value; }
      set { MutationProbabilityParameter.Value = value; }
    }
    public IManipulator Mutator {
      get { return MutatorParameter.Value; }
      set { MutatorParameter.Value = value; }
    }
    public IntData Elites {
      get { return ElitesParameter.Value; }
      set { ElitesParameter.Value = value; }
    }
    public IntData MaximumGenerations {
      get { return MaximumGenerationsParameter.Value; }
      set { MaximumGenerationsParameter.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private PopulationCreator PopulationCreator {
      get { return (PopulationCreator)RandomCreator.Successor; }
    }
    private SGAMainLoop SGAMainLoop {
      get { return (SGAMainLoop)PopulationCreator.Successor; }
    }
    private List<ISelector> selectors;
    private IEnumerable<ISelector> Selectors {
      get { return selectors; }
    }
    #endregion

    public SGA()
      : base() {
      Parameters.Add(new ValueParameter<IntData>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntData(0)));
      Parameters.Add(new ValueParameter<BoolData>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolData(true)));
      Parameters.Add(new ValueParameter<IntData>("PopulationSize", "The size of the population of solutions.", new IntData(100)));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<DoubleData>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new DoubleData(0.05)));
      Parameters.Add(new OptionalConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueParameter<IntData>("Elites", "The numer of elite solutions which are kept in each generation.", new IntData(1)));
      Parameters.Add(new ValueParameter<IntData>("MaximumGenerations", "The maximum number of generations which should be processed.", new IntData(1000)));

      RandomCreator randomCreator = new RandomCreator();
      PopulationCreator populationCreator = new PopulationCreator();
      SGAMainLoop sgaMainLoop = new SGAMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = populationCreator;

      populationCreator.PopulationSizeParameter.ActualName = PopulationSizeParameter.Name;
      populationCreator.Successor = sgaMainLoop;

      sgaMainLoop.SelectorParameter.ActualName = SelectorParameter.Name;
      sgaMainLoop.CrossoverParameter.ActualName = CrossoverParameter.Name;
      sgaMainLoop.ElitesParameter.ActualName = ElitesParameter.Name;
      sgaMainLoop.MaximumGenerationsParameter.ActualName = MaximumGenerationsParameter.Name;
      sgaMainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      sgaMainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      sgaMainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
      sgaMainLoop.ResultsParameter.ActualName = "Results";

      Initialze();
    }
    [StorableConstructor]
    private SGA(bool deserializing) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      SGA clone = (SGA)base.Clone(cloner);
      clone.Initialze();
      return clone;
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators) ParameterizeStochasticOperator(op);
      ParameterizePopulationCreator();
      ParameterizeSGAMainLoop();
      ParameterizeSelectors();
      UpdateCrossovers();
      UpdateMutators();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.OnProblemChanged();
    }
    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizePopulationCreator();
      base.Problem_SolutionCreatorChanged(sender, e);
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperator(Problem.Evaluator);
      ParameterizePopulationCreator();
      ParameterizeSGAMainLoop();
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
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      ParameterizeSelectors();
    }
    private void PopulationSize_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeSGAMainLoop();
      ParameterizeSelectors();
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
      if (Problem != null)
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
    }

    private void ParameterizePopulationCreator() {
      PopulationCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      PopulationCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
    }
    private void ParameterizeSGAMainLoop() {
      SGAMainLoop.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      SGAMainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
      SGAMainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      if (op is IStochasticOperator)
        ((IStochasticOperator)op).RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
    }
    private void InitializeSelectors() {
      selectors = new List<ISelector>();
      if (ApplicationManager.Manager != null) {
        selectors.AddRange(ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name));
        ParameterizeSelectors();
      }
    }
    private void ParameterizeSelectors() {
      foreach (ISelector selector in Selectors) {
        selector.CopySelected = new BoolData(true);
        selector.NumberOfSelectedSubScopesParameter.Value = new IntData(2 * (PopulationSizeParameter.Value.Value - ElitesParameter.Value.Value));
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
        if (oldSelector != null)
          SelectorParameter.Value = SelectorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldSelector.GetType());
      }
    }
    private void UpdateCrossovers() {
      ICrossover oldCrossover = CrossoverParameter.Value;
      CrossoverParameter.ValidValues.Clear();
      foreach (ICrossover crossover in Problem.Operators.OfType<ICrossover>().OrderBy(x => x.Name))
        CrossoverParameter.ValidValues.Add(crossover);
      if (oldCrossover != null)
        CrossoverParameter.Value = CrossoverParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldCrossover.GetType());
    }
    private void UpdateMutators() {
      IManipulator oldMutator = MutatorParameter.Value;
      MutatorParameter.ValidValues.Clear();
      foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>().OrderBy(x => x.Name))
        MutatorParameter.ValidValues.Add(mutator);
      if (oldMutator != null)
        MutatorParameter.Value = MutatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMutator.GetType());
    }
    #endregion
  }
}
