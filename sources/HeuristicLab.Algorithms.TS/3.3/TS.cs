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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.TS {
  [Item("TS", "A tabu search algorithm.")]
  public sealed class TS : EngineAlgorithm {
    /*#region Parameter Properties
    private ValueParameter<IntData> SeedParameter {
      get { return (ValueParameter<IntData>)Parameters["Seed"]; }
    }
    private ValueParameter<BoolData> SetSeedRandomlyParameter {
      get { return (ValueParameter<BoolData>)Parameters["SetSeedRandomly"]; }
    }
    private ConstrainedValueParameter<IMoveGenerator> MoveGeneratorParameter {
      get { return (ConstrainedValueParameter<IMoveGenerator>)Parameters["MoveGenerator"]; }
    }
    private ValueParameter<IntData> MaximumIterationsParameter {
      get { return (ValueParameter<IntData>)Parameters["MaximumIterations"]; }
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
    public IMoveGenerator MoveGenerator {
      get { return MoveGeneratorParameter.Value; }
      set { MoveGeneratorParameter.Value = value; }
    }
    public IntData MaximumIterations {
      get { return MaximumIterationsParameter.Value; }
      set { MaximumIterationsParameter.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)RandomCreator.Successor; }
    }
    private TSMainLoop TSMainLoop {
      get { return (TSMainLoop)SolutionsCreator.Successor; }
    }
    #endregion

    public TS()
      : base() {
      Parameters.Add(new ValueParameter<IntData>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntData(0)));
      Parameters.Add(new ValueParameter<BoolData>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolData(true)));
      Parameters.Add(new ConstrainedValueParameter<IMoveGenerator>("MoveGenerator", "The operator used to generate moves to the neighborhood of the current solution."));
      Parameters.Add(new ValueParameter<IntData>("MaximumIterations", "The maximum number of generations which should be processed.", new IntData(1000)));

      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      TSMainLoop tsMainLoop = new TSMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutions = new IntData(1);
      solutionsCreator.Successor = tsMainLoop;

      tsMainLoop.MoveGeneratorParameter.ActualName = MoveGeneratorParameter.Name;
      tsMainLoop.MoveMakerParameter.ActualName = MoveGenerator..Name;
      tsMainLoop.ElitesParameter.ActualName = ElitesParameter.Name;
      tsMainLoop.MaximumGenerationsParameter.ActualName = MaximumGenerationsParameter.Name;
      tsMainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      tsMainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      tsMainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
      tsMainLoop.ResultsParameter.ActualName = "Results";

      Initialze();
    }
    [StorableConstructor]
    private TS(bool deserializing) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      TS clone = (TS)base.Clone(cloner);
      clone.Initialze();
      return clone;
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators) ParameterizeStochasticOperator(op);
      ParameterizeSolutionsCreator();
      ParameterizeSGAMainLoop();
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

    private void ParameterizeSolutionsCreator() {
      SolutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      SolutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
    }
    private void ParameterizeSGAMainLoop() {
      TSMainLoop.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      TSMainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
      TSMainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
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
     */
  }
}
