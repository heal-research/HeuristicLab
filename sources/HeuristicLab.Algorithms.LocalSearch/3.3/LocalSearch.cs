#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.LocalSearch {
  [Item("Local Search (LS)", "A local search algorithm.")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms, Priority = 100)]
  [StorableClass]
  public sealed class LocalSearch : HeuristicOptimizationEngineAlgorithm, IStorableContent {
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
    public IConstrainedValueParameter<IMoveGenerator> MoveGeneratorParameter {
      get { return (IConstrainedValueParameter<IMoveGenerator>)Parameters["MoveGenerator"]; }
    }
    public IConstrainedValueParameter<IMoveMaker> MoveMakerParameter {
      get { return (IConstrainedValueParameter<IMoveMaker>)Parameters["MoveMaker"]; }
    }
    public IConstrainedValueParameter<ISingleObjectiveMoveEvaluator> MoveEvaluatorParameter {
      get { return (IConstrainedValueParameter<ISingleObjectiveMoveEvaluator>)Parameters["MoveEvaluator"]; }
    }
    private ValueParameter<IntValue> MaximumIterationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    private ValueParameter<IntValue> SampleSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["SampleSize"]; }
    }
    private ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
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
    public IMoveGenerator MoveGenerator {
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
    public IntValue SampleSize {
      get { return SampleSizeParameter.Value; }
      set { SampleSizeParameter.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)RandomCreator.Successor; }
    }
    private LocalSearchMainLoop MainLoop {
      get { return FindMainLoop(SolutionsCreator.Successor); }
    }
    [Storable]
    private BestAverageWorstQualityAnalyzer moveQualityAnalyzer;
    #endregion

    [StorableConstructor]
    private LocalSearch(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    private LocalSearch(LocalSearch original, Cloner cloner)
      : base(original, cloner) {
      moveQualityAnalyzer = cloner.Clone(original.moveQualityAnalyzer);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LocalSearch(this, cloner);
    }
    public LocalSearch()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ConstrainedValueParameter<IMoveGenerator>("MoveGenerator", "The operator used to generate moves to the neighborhood of the current solution."));
      Parameters.Add(new ConstrainedValueParameter<IMoveMaker>("MoveMaker", "The operator used to perform a move."));
      Parameters.Add(new ConstrainedValueParameter<ISingleObjectiveMoveEvaluator>("MoveEvaluator", "The operator used to evaluate a move."));
      Parameters.Add(new ValueParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed.", new IntValue(1000)));
      Parameters.Add(new ValueParameter<IntValue>("SampleSize", "Number of moves that MultiMoveGenerators should create. This is ignored for Exhaustive- and SingleMoveGenerators.", new IntValue(100)));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze the solution and moves.", new MultiAnalyzer()));

      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      VariableCreator variableCreator = new VariableCreator();
      ResultsCollector resultsCollector = new ResultsCollector();
      LocalSearchMainLoop mainLoop = new LocalSearchMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutions = new IntValue(1);
      solutionsCreator.Successor = variableCreator;

      variableCreator.Name = "Initialize EvaluatedMoves";
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedMoves", new IntValue()));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(0)));
      variableCreator.Successor = resultsCollector;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Moves", null, "EvaluatedMoves"));
      resultsCollector.ResultsParameter.ActualName = "Results";
      resultsCollector.Successor = mainLoop;

      mainLoop.MoveGeneratorParameter.ActualName = MoveGeneratorParameter.Name;
      mainLoop.MoveMakerParameter.ActualName = MoveMakerParameter.Name;
      mainLoop.MoveEvaluatorParameter.ActualName = MoveEvaluatorParameter.Name;
      mainLoop.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
      mainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
      mainLoop.ResultsParameter.ActualName = "Results";
      mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
      mainLoop.EvaluatedMovesParameter.ActualName = "EvaluatedMoves";
      mainLoop.IterationsParameter.ActualName = "Iterations";
      mainLoop.BestLocalQualityParameter.ActualName = "BestQuality";

      moveQualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      ParameterizeAnalyzers();
      UpdateAnalyzers();

      Initialize();
    }

    public override void Prepare() {
      if (Problem != null && MoveGenerator != null && MoveMaker != null && MoveEvaluator != null)
        base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
        op.MoveQualityParameter.ActualNameChanged += new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
      }
      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      UpdateMoveGenerator();
      UpdateMoveParameters();
      UpdateAnalyzers();
      ParameterizeMoveGenerators();
      ParameterizeMoveEvaluators();
      ParameterizeMoveMakers();
      ParameterizeAnalyzers();
      ParameterizeIterationBasedOperators();
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
      ParameterizeAnalyzers();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      // This may seem pointless, but some operators already have the eventhandler registered, others don't
      // FIXME: Is there another way to solve this problem?
      foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
        op.MoveQualityParameter.ActualNameChanged -= new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
        op.MoveQualityParameter.ActualNameChanged += new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
      }
      UpdateMoveGenerator();
      UpdateMoveParameters();
      UpdateAnalyzers();
      ParameterizeMainLoop();
      ParameterizeMoveGenerators();
      ParameterizeMoveEvaluators();
      ParameterizeMoveMakers();
      ParameterizeAnalyzers();
      ParameterizeIterationBasedOperators();
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
      ParameterizeAnalyzers();
    }
    private void MoveEvaluator_MoveQualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeMoveEvaluators();
      ParameterizeMoveMakers();
      ParameterizeAnalyzers();
    }
    #endregion

    #region Helpers
    private void Initialize() {
      if (Problem != null) {
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
        foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
          op.MoveQualityParameter.ActualNameChanged += new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
        }
      }
      MoveGeneratorParameter.ValueChanged += new EventHandler(MoveGeneratorParameter_ValueChanged);
      MoveEvaluatorParameter.ValueChanged += new EventHandler(MoveEvaluatorParameter_ValueChanged);
    }
    private void UpdateMoveGenerator() {
      IMoveGenerator oldMoveGenerator = MoveGenerator;
      IMoveGenerator defaultMoveGenerator = Problem.Operators.OfType<IMoveGenerator>().FirstOrDefault();
      MoveGeneratorParameter.ValidValues.Clear();
      if (Problem != null) {
        foreach (IMoveGenerator generator in Problem.Operators.OfType<IMoveGenerator>().OrderBy(x => x.Name))
          MoveGeneratorParameter.ValidValues.Add(generator);
      }
      if (oldMoveGenerator != null) {
        IMoveGenerator newMoveGenerator = MoveGeneratorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveGenerator.GetType());
        if (newMoveGenerator != null) MoveGenerator = newMoveGenerator;
        else oldMoveGenerator = null;
      }
      if (oldMoveGenerator == null && defaultMoveGenerator != null)
        MoveGenerator = defaultMoveGenerator;
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

        var operators = Problem.Operators.Where(op => moveTypes.Any(m => m.IsInstanceOfType(op))).ToList();
        IMoveMaker defaultMoveMaker = operators.OfType<IMoveMaker>().FirstOrDefault();
        ISingleObjectiveMoveEvaluator defaultMoveEvaluator = operators.OfType<ISingleObjectiveMoveEvaluator>().FirstOrDefault();
        foreach (IMoveMaker moveMaker in operators.OfType<IMoveMaker>().OrderBy(op => op.Name))
          MoveMakerParameter.ValidValues.Add(moveMaker);
        foreach (ISingleObjectiveMoveEvaluator moveEvaluator in operators.OfType<ISingleObjectiveMoveEvaluator>().OrderBy(op => op.Name))
          MoveEvaluatorParameter.ValidValues.Add(moveEvaluator);

        if (oldMoveMaker != null) {
          IMoveMaker mm = MoveMakerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveMaker.GetType());
          if (mm != null) MoveMaker = mm;
          else oldMoveMaker = null;
        }
        if (oldMoveMaker == null && defaultMoveMaker != null)
          MoveMaker = defaultMoveMaker;

        if (oldMoveEvaluator != null) {
          ISingleObjectiveMoveEvaluator me = MoveEvaluatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveEvaluator.GetType());
          if (me != null) MoveEvaluator = me;
          else oldMoveEvaluator = null;
        }
        if (oldMoveEvaluator == null && defaultMoveEvaluator != null)
          MoveEvaluator = defaultMoveEvaluator;
      }
    }
    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (Problem != null) {
        foreach (IAnalyzer analyzer in Problem.Operators.OfType<IAnalyzer>()) {
          foreach (IScopeTreeLookupParameter param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 0;
          Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
        }
      }
      Analyzer.Operators.Add(moveQualityAnalyzer, moveQualityAnalyzer.EnabledByDefault);
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
      if (Problem != null) {
        MainLoop.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        MainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        MainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      }
      if (MoveEvaluator != null)
        MainLoop.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      if (op is IStochasticOperator) {
        IStochasticOperator stOp = (IStochasticOperator)op;
        stOp.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
        stOp.RandomParameter.Hidden = true;
      }
    }
    private void ParameterizeMoveGenerators() {
      if (Problem != null) {
        foreach (IMultiMoveGenerator generator in Problem.Operators.OfType<IMultiMoveGenerator>()) {
          generator.SampleSizeParameter.ActualName = SampleSizeParameter.Name;
          generator.SampleSizeParameter.Hidden = true;
        }
      }
    }
    private void ParameterizeMoveEvaluators() {
      foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
        op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        op.QualityParameter.Hidden = true;
      }
    }
    private void ParameterizeMoveMakers() {
      foreach (IMoveMaker op in Problem.Operators.OfType<IMoveMaker>()) {
        op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        op.QualityParameter.Hidden = true;
        if (MoveEvaluator != null) {
          op.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
          op.MoveQualityParameter.Hidden = true;
        } else {
          op.MoveQualityParameter.Hidden = false;
        }
      }
    }
    private void ParameterizeAnalyzers() {
      moveQualityAnalyzer.ResultsParameter.ActualName = "Results";
      moveQualityAnalyzer.ResultsParameter.Hidden = true;
      if (Problem != null) {
        moveQualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        moveQualityAnalyzer.MaximizationParameter.Hidden = true;
        if (MoveEvaluator != null) {
          moveQualityAnalyzer.QualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
          moveQualityAnalyzer.QualityParameter.Hidden = true;
        } else moveQualityAnalyzer.QualityParameter.Hidden = false;
        moveQualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        moveQualityAnalyzer.BestKnownQualityParameter.Hidden = true;
      } else {
        moveQualityAnalyzer.MaximizationParameter.Hidden = false;
        moveQualityAnalyzer.BestKnownQualityParameter.Hidden = false;
      }
    }
    private void ParameterizeIterationBasedOperators() {
      if (Problem != null) {
        foreach (IIterationBasedOperator op in Problem.Operators.OfType<IIterationBasedOperator>()) {
          op.IterationsParameter.ActualName = "Iterations";
          op.IterationsParameter.Hidden = true;
          op.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
          op.MaximumIterationsParameter.Hidden = true;
        }
      }
    }
    private LocalSearchMainLoop FindMainLoop(IOperator start) {
      IOperator mainLoop = start;
      while (mainLoop != null && !(mainLoop is LocalSearchMainLoop))
        mainLoop = ((SingleSuccessorOperator)mainLoop).Successor;
      if (mainLoop == null) return null;
      else return (LocalSearchMainLoop)mainLoop;
    }
    #endregion
  }
}
