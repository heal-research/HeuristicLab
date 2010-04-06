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
using HeuristicLab.Optimization;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.SimulatedAnnealing {
  [Item("Simulated Annealing", "A simulated annealing algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class SimulatedAnnealing : EngineAlgorithm {
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
    private ConstrainedValueParameter<IMultiMoveGenerator> MoveGeneratorParameter {
      get { return (ConstrainedValueParameter<IMultiMoveGenerator>)Parameters["MoveGenerator"]; }
    }
    private ConstrainedValueParameter<IMoveMaker> MoveMakerParameter {
      get { return (ConstrainedValueParameter<IMoveMaker>)Parameters["MoveMaker"]; }
    }
    private ConstrainedValueParameter<ISingleObjectiveMoveEvaluator> MoveEvaluatorParameter {
      get { return (ConstrainedValueParameter<ISingleObjectiveMoveEvaluator>)Parameters["MoveEvaluator"]; }
    }
    private ConstrainedValueParameter<IDiscreteDoubleValueModifier> AnnealingOperatorParameter {
      get { return (ConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters["AnnealingOperator"]; }
    }
    private ValueParameter<IntValue> MaximumIterationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    private ValueParameter<IntValue> InnerIterationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["InnerIterations"]; }
    }
    private ValueParameter<DoubleValue> StartTemperatureParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["StartTemperature"]; }
    }
    private ValueParameter<DoubleValue> EndTemperatureParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["EndTemperature"]; }
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
    public IMultiMoveGenerator MoveGenerator {
      get { return MoveGeneratorParameter.Value; }
      set { MoveGeneratorParameter.Value = value; }
    }
    public IMoveMaker MoveMaker {
      get { return MoveMakerParameter.Value; }
      set { MoveMakerParameter.Value = value; }
    }
    public ISingleObjectiveMoveEvaluator MoveEvaluator {
      get { return MoveEvaluatorParameter.Value; }
      set { MoveEvaluatorParameter.Value = value; }
    }
    public IntValue MaximumIterations {
      get { return MaximumIterationsParameter.Value; }
      set { MaximumIterationsParameter.Value = value; }
    }
    public IntValue InnerIterations {
      get { return InnerIterationsParameter.Value; }
      set { InnerIterationsParameter.Value = value; }
    }
    public DoubleValue StartTemperature {
      get { return StartTemperatureParameter.Value; }
      set { StartTemperatureParameter.Value = value; }
    }
    public DoubleValue EndTemperature {
      get { return EndTemperatureParameter.Value; }
      set { EndTemperatureParameter.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)RandomCreator.Successor; }
    }
    private SimulatedAnnealingMainLoop MainLoop {
      get { return (SimulatedAnnealingMainLoop)SolutionsCreator.Successor; }
    }

    private List<IDiscreteDoubleValueModifier> annealingOperators;
    private IEnumerable<IDiscreteDoubleValueModifier> AnnealingOperators {
      get { return annealingOperators; }
    }
    #endregion

    [StorableConstructor]
    private SimulatedAnnealing(bool deserializing) : base() { }
    public SimulatedAnnealing()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ConstrainedValueParameter<IMultiMoveGenerator>("MoveGenerator", "The operator used to generate moves to the neighborhood of the current solution."));
      Parameters.Add(new ConstrainedValueParameter<ISingleObjectiveMoveEvaluator>("MoveEvaluator", "The operator used to evaluate a move."));
      Parameters.Add(new ConstrainedValueParameter<IMoveMaker>("MoveMaker", "The operator used to perform a move."));
      Parameters.Add(new ConstrainedValueParameter<IDiscreteDoubleValueModifier>("AnnealingOperator", "The operator used to modify the temperature."));
      Parameters.Add(new ValueParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("InnerIterations", "The amount of inner iterations (number of moves before temperature is adjusted again).", new IntValue(10)));
      Parameters.Add(new ValueParameter<DoubleValue>("StartTemperature", "The initial temperature.", new DoubleValue(100)));
      Parameters.Add(new ValueParameter<DoubleValue>("EndTemperature", "The final temperature which should be reached when iterations reaches maximum iterations.", new DoubleValue(1e-6)));

      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      SimulatedAnnealingMainLoop mainLoop = new SimulatedAnnealingMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutions = new IntValue(1);
      solutionsCreator.Successor = mainLoop;

      mainLoop.MoveGeneratorParameter.ActualName = MoveGeneratorParameter.Name;
      mainLoop.MoveEvaluatorParameter.ActualName = MoveEvaluatorParameter.Name;
      mainLoop.MoveMakerParameter.ActualName = MoveMakerParameter.Name;
      mainLoop.AnnealingOperatorParameter.ActualName = AnnealingOperatorParameter.Name;
      mainLoop.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
      mainLoop.StartTemperatureParameter.ActualName = StartTemperatureParameter.Name;
      mainLoop.EndTemperatureParameter.ActualName = EndTemperatureParameter.Name;
      mainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
      mainLoop.ResultsParameter.ActualName = "Results";

      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SimulatedAnnealing clone = (SimulatedAnnealing)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    public override void Prepare() {
      if (Problem != null && MoveGenerator != null && MoveMaker != null && MoveEvaluator != null)
        base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators) ParameterizeStochasticOperator(op);
      foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
        op.MoveQualityParameter.ActualNameChanged += new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
      }
      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      UpdateMoveGenerator();
      UpdateMoveParameters();
      ParameterizeMoveEvaluators();
      ParameterizeMoveMakers();
      ParameterizeMoveGenerators();
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
      ParameterizeMoveEvaluators();
      ParameterizeMoveMakers();
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
      // This may seem pointless, but some operators already have the eventhandler registered, others don't
      // FIXME: Is there another way to solve this problem?
      foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
        op.MoveQualityParameter.ActualNameChanged -= new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
        op.MoveQualityParameter.ActualNameChanged += new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
      }
      UpdateMoveGenerator();
      UpdateMoveParameters();
      ParameterizeMainLoop();
      ParameterizeMoveEvaluators();
      ParameterizeMoveMakers();
      ParameterizeMoveGenerators();
      base.Problem_OperatorsChanged(sender, e);
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeMoveEvaluators();
      ParameterizeMoveMakers();
    }
    private void MoveGeneratorParameter_ValueChanged(object sender, EventArgs e) {
      UpdateMoveParameters();
    }
    private void MoveEvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeMoveEvaluators();
      ParameterizeMoveMakers();
    }
    private void MoveEvaluator_MoveQualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeMoveEvaluators();
      ParameterizeMoveMakers();
    }
    private void Visualizer_VisualizationParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (Problem != null) {
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
        foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
          op.MoveQualityParameter.ActualNameChanged += new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
        }
      }
      MoveGeneratorParameter.ValueChanged += new EventHandler(MoveGeneratorParameter_ValueChanged);
      MoveEvaluatorParameter.ValueChanged += new EventHandler(MoveEvaluatorParameter_ValueChanged);
      InitializeAnnealingOperators();
    }
    private void InitializeAnnealingOperators() {
      annealingOperators = new List<IDiscreteDoubleValueModifier>();
      if (ApplicationManager.Manager != null) {
        annealingOperators.AddRange(ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>().OrderBy(x => x.Name));
        ParameterizeAnnealingOperators();
      }
      AnnealingOperatorParameter.ValidValues.Clear();
      foreach (IDiscreteDoubleValueModifier op in annealingOperators)
        AnnealingOperatorParameter.ValidValues.Add(op);
    }
    private void UpdateMoveGenerator() {
      IMultiMoveGenerator oldMoveGenerator = MoveGenerator;
      MoveGeneratorParameter.ValidValues.Clear();
      if (Problem != null) {
        foreach (IMultiMoveGenerator generator in Problem.Operators.OfType<IMultiMoveGenerator>().OrderBy(x => x.Name))
          MoveGeneratorParameter.ValidValues.Add(generator);
      }
      if (oldMoveGenerator != null) {
        IMultiMoveGenerator newMoveGenerator = MoveGeneratorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveGenerator.GetType());
        if (newMoveGenerator != null) MoveGenerator = newMoveGenerator;
      }
      if (MoveGenerator == null) {
        ClearMoveParameters();
      }
    }
    private void UpdateMoveParameters() {
      IMoveMaker oldMoveMaker = MoveMaker;
      ISingleObjectiveMoveEvaluator oldMoveEvaluator = MoveEvaluator;
      ClearMoveParameters();
      if (MoveGenerator != null) {
        List<Type> moveTypes = MoveGenerator.GetType().GetInterfaces().Where(x => typeof(IMoveOperator).IsAssignableFrom(x)).ToList();
        foreach (Type type in moveTypes.ToList()) {
          if (moveTypes.Any(t => t != type && type.IsAssignableFrom(t)))
            moveTypes.Remove(type);
        }
        foreach (Type type in moveTypes) {
          var operators = Problem.Operators.Where(x => type.IsAssignableFrom(x.GetType())).OrderBy(x => x.Name);
          foreach (IMoveMaker moveMaker in operators.OfType<IMoveMaker>())
            MoveMakerParameter.ValidValues.Add(moveMaker);
          foreach (ISingleObjectiveMoveEvaluator moveEvaluator in operators.OfType<ISingleObjectiveMoveEvaluator>())
            MoveEvaluatorParameter.ValidValues.Add(moveEvaluator);
        }
        if (oldMoveMaker != null) {
          IMoveMaker mm = MoveMakerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveMaker.GetType());
          if (mm != null) MoveMaker = mm;
        }
        if (oldMoveEvaluator != null) {
          ISingleObjectiveMoveEvaluator me = MoveEvaluatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveEvaluator.GetType());
          if (me != null) MoveEvaluator = me;
        }
      }
    }
    private void ClearMoveParameters() {
      MoveMakerParameter.ValidValues.Clear();
      MoveEvaluatorParameter.ValidValues.Clear();
    }
    private void ParameterizeSolutionsCreator() {
      SolutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      SolutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
    }
    private void ParameterizeMainLoop() {
      MainLoop.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
      MainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
      MainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      if (MoveEvaluator != null)
        MainLoop.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
      MainLoop.VisualizerParameter.ActualName = Problem.VisualizerParameter.Name;
      if (Problem.Visualizer != null)
        MainLoop.VisualizationParameter.ActualName = Problem.Visualizer.VisualizationParameter.ActualName;
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      if (op is IStochasticOperator)
        ((IStochasticOperator)op).RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
    }
    private void ParameterizeMoveEvaluators() {
      foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
        op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      }
    }
    private void ParameterizeMoveMakers() {
      foreach (IMoveMaker op in Problem.Operators.OfType<IMoveMaker>()) {
        op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        if (MoveEvaluator != null)
          op.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
      }
    }
    private void ParameterizeAnnealingOperators() {
      foreach (IDiscreteDoubleValueModifier op in annealingOperators) {
        op.IndexParameter.ActualName = "Iterations";
        op.StartIndexParameter.Value = new IntValue(0);
        op.EndIndexParameter.ActualName = MaximumIterationsParameter.Name;
        op.ValueParameter.ActualName = "Temperature";
        op.StartValueParameter.ActualName = StartTemperatureParameter.Name;
        op.EndValueParameter.ActualName = EndTemperatureParameter.Name;
        ParameterizeStochasticOperator(op);
      }
    }
    private void ParameterizeMoveGenerators() {
      foreach (IMultiMoveGenerator op in Problem.Operators.OfType<IMultiMoveGenerator>()) {
        op.SampleSizeParameter.ActualName = InnerIterationsParameter.Name;
      }
    }
    #endregion
  }
}
