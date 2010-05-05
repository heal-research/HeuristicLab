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
using HeuristicLab.Optimization;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;
using HeuristicLab.Analysis;

namespace HeuristicLab.Algorithms.TabuSearch {
  [Item("Tabu Search", "A tabu search algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class TabuSearch : EngineAlgorithm {
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
    private ConstrainedValueParameter<IMoveGenerator> MoveGeneratorParameter {
      get { return (ConstrainedValueParameter<IMoveGenerator>)Parameters["MoveGenerator"]; }
    }
    private ConstrainedValueParameter<IMoveMaker> MoveMakerParameter {
      get { return (ConstrainedValueParameter<IMoveMaker>)Parameters["MoveMaker"]; }
    }
    private ConstrainedValueParameter<ISingleObjectiveMoveEvaluator> MoveEvaluatorParameter {
      get { return (ConstrainedValueParameter<ISingleObjectiveMoveEvaluator>)Parameters["MoveEvaluator"]; }
    }
    private ConstrainedValueParameter<ITabuChecker> TabuCheckerParameter {
      get { return (ConstrainedValueParameter<ITabuChecker>)Parameters["TabuChecker"]; }
    }
    private ConstrainedValueParameter<ITabuMaker> TabuMakerParameter {
      get { return (ConstrainedValueParameter<ITabuMaker>)Parameters["TabuMaker"]; }
    }
    private ValueParameter<IntValue> TabuTenureParameter {
      get { return (ValueParameter<IntValue>)Parameters["TabuTenure"]; }
    }
    private ValueParameter<IntValue> MaximumIterationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    private ValueParameter<IntValue> SampleSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["SampleSize"]; }
    }
    private ValueParameter<MultiAnalyzer<IPopulationAnalyzer>> MoveAnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer<IPopulationAnalyzer>>)Parameters["MoveAnalyzer"]; }
    }
    private ValueParameter<MultiAnalyzer<ISolutionAnalyzer>> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer<ISolutionAnalyzer>>)Parameters["Analyzer"]; }
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
    public ITabuChecker TabuChecker {
      get { return TabuCheckerParameter.Value; }
      set { TabuCheckerParameter.Value = value; }
    }
    public ITabuMaker TabuMaker {
      get { return TabuMakerParameter.Value; }
      set { TabuMakerParameter.Value = value; }
    }
    public IntValue TabuTenure {
      get { return TabuTenureParameter.Value; }
      set { TabuTenureParameter.Value = value; }
    }
    public IntValue MaximumIterations {
      get { return MaximumIterationsParameter.Value; }
      set { MaximumIterationsParameter.Value = value; }
    }
    public MultiAnalyzer<IPopulationAnalyzer> MoveAnalyzer {
      get { return MoveAnalyzerParameter.Value; }
      set { MoveAnalyzerParameter.Value = value; }
    }
    public MultiAnalyzer<ISolutionAnalyzer> Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)RandomCreator.Successor; }
    }
    private TabuSearchMainLoop MainLoop {
      get { return (TabuSearchMainLoop)SolutionsCreator.Successor; }
    }
    private PopulationBestAverageWorstQualityAnalyzer moveQualityAnalyzer;
    private TabuNeighborhoodAnalyzer tabuNeighborhoodAnalyzer;
    #endregion

    public TabuSearch()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ConstrainedValueParameter<IMoveGenerator>("MoveGenerator", "The operator used to generate moves to the neighborhood of the current solution."));
      Parameters.Add(new ConstrainedValueParameter<IMoveMaker>("MoveMaker", "The operator used to perform a move."));
      Parameters.Add(new ConstrainedValueParameter<ISingleObjectiveMoveEvaluator>("MoveEvaluator", "The operator used to evaluate a move."));
      Parameters.Add(new ConstrainedValueParameter<ITabuChecker>("TabuChecker", "The operator to check whether a move is tabu or not."));
      Parameters.Add(new ConstrainedValueParameter<ITabuMaker>("TabuMaker", "The operator used to insert attributes of a move into the tabu list."));
      Parameters.Add(new ValueParameter<IntValue>("TabuTenure", "The length of the tabu list.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed.", new IntValue(1000)));
      Parameters.Add(new ValueParameter<IntValue>("SampleSize", "The neighborhood size for stochastic sampling move generators", new IntValue(100)));
      Parameters.Add(new ValueParameter<MultiAnalyzer<IPopulationAnalyzer>>("MoveAnalyzer", "The operator used to analyze the moves.", new MultiAnalyzer<IPopulationAnalyzer>()));
      Parameters.Add(new ValueParameter<MultiAnalyzer<ISolutionAnalyzer>>("Analyzer", "The operator used to analyze the solution.", new MultiAnalyzer<ISolutionAnalyzer>()));
      
      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      TabuSearchMainLoop tsMainLoop = new TabuSearchMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutions = new IntValue(1);
      solutionsCreator.Successor = tsMainLoop;

      tsMainLoop.MoveGeneratorParameter.ActualName = MoveGeneratorParameter.Name;
      tsMainLoop.MoveMakerParameter.ActualName = MoveMakerParameter.Name;
      tsMainLoop.MoveEvaluatorParameter.ActualName = MoveEvaluatorParameter.Name;
      tsMainLoop.TabuCheckerParameter.ActualName = TabuCheckerParameter.Name;
      tsMainLoop.TabuMakerParameter.ActualName = TabuMakerParameter.Name;
      tsMainLoop.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
      tsMainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
      tsMainLoop.ResultsParameter.ActualName = "Results";
      tsMainLoop.MoveAnalyzerParameter.ActualName = MoveAnalyzerParameter.Name;
      tsMainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;

      Initialize();
    }
    [StorableConstructor]
    private TabuSearch(bool deserializing) : base(deserializing) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      TabuSearch clone = (TabuSearch)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    public override void Prepare() {
      if (Problem != null && MoveGenerator != null && MoveMaker != null && MoveEvaluator != null &&
          TabuChecker != null && TabuMaker != null)
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
      foreach (ITabuChecker op in Problem.Operators.OfType<ITabuChecker>()) {
        op.MoveTabuParameter.ActualNameChanged += new EventHandler(TabuChecker_MoveTabuParameter_ActualNameChanged);
      }
      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      UpdateMoveGenerator();
      UpdateMoveParameters();
      UpdateAnalyzers();
      ParameterizeMoveGenerators();
      ParameterizeMoveEvaluator();
      ParameterizeMoveMaker();
      ParameterizeTabuMaker();
      ParameterizeTabuChecker();
      ParameterizeAnalyzers();
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
      ParameterizeMoveEvaluator();
      ParameterizeMoveMaker();
      ParameterizeTabuMaker();
      ParameterizeTabuChecker();
      ParameterizeAnalyzers();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      foreach (IOperator op in Problem.Operators) ParameterizeStochasticOperator(op);
      // This may seem pointless, but some operators already have the eventhandler registered, others don't
      // FIXME: Is there another way to solve this problem?
      foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
        op.MoveQualityParameter.ActualNameChanged -= new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
        op.MoveQualityParameter.ActualNameChanged += new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
      }
      foreach (ITabuChecker op in Problem.Operators.OfType<ITabuChecker>()) {
        op.MoveTabuParameter.ActualNameChanged -= new EventHandler(TabuChecker_MoveTabuParameter_ActualNameChanged);
        op.MoveTabuParameter.ActualNameChanged += new EventHandler(TabuChecker_MoveTabuParameter_ActualNameChanged);
      }
      UpdateMoveGenerator();
      UpdateMoveParameters();
      UpdateAnalyzers();
      ParameterizeMainLoop();
      ParameterizeMoveGenerators();
      ParameterizeMoveEvaluator();
      ParameterizeMoveMaker();
      ParameterizeTabuMaker();
      ParameterizeTabuChecker();
      ParameterizeAnalyzers();
      base.Problem_OperatorsChanged(sender, e);
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeMoveEvaluator();
      ParameterizeMoveMaker();
      ParameterizeTabuMaker();
      ParameterizeTabuChecker();
    }
    private void MoveGeneratorParameter_ValueChanged(object sender, EventArgs e) {
      UpdateMoveParameters();
    }
    private void MoveEvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeMoveEvaluator();
      ParameterizeMoveMaker();
      ParameterizeTabuMaker();
      ParameterizeTabuChecker();
      ParameterizeAnalyzers();
    }
    private void MoveEvaluator_MoveQualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeMoveEvaluator();
      ParameterizeMoveMaker();
      ParameterizeTabuMaker();
      ParameterizeTabuChecker();
      ParameterizeAnalyzers();
    }
    private void TabuCheckerParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeAnalyzers();
    }
    private void TabuChecker_MoveTabuParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeAnalyzers();
    }
    private void SampleSizeParameter_NameChanged(object sender, EventArgs e) {
      ParameterizeMoveGenerators();
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      InitializeAnalyzers();
      UpdateAnalyzers();
      if (Problem != null) {
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
        foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
          op.MoveQualityParameter.ActualNameChanged += new EventHandler(MoveEvaluator_MoveQualityParameter_ActualNameChanged);
        }
      }
      MoveGeneratorParameter.ValueChanged += new EventHandler(MoveGeneratorParameter_ValueChanged);
      MoveEvaluatorParameter.ValueChanged += new EventHandler(MoveEvaluatorParameter_ValueChanged);
      TabuCheckerParameter.ValueChanged += new EventHandler(TabuCheckerParameter_ValueChanged);
      SampleSizeParameter.NameChanged += new EventHandler(SampleSizeParameter_NameChanged);
    }
    private void InitializeAnalyzers() {
      moveQualityAnalyzer = new PopulationBestAverageWorstQualityAnalyzer();
      tabuNeighborhoodAnalyzer = new TabuNeighborhoodAnalyzer();
      ParameterizeAnalyzers();
    }
    private void UpdateMoveGenerator() {
      IMoveGenerator oldMoveGenerator = MoveGenerator;
      MoveGeneratorParameter.ValidValues.Clear();
      if (Problem != null) {
        foreach (IMoveGenerator generator in Problem.Operators.OfType<IMoveGenerator>().OrderBy(x => x.Name)) {
          MoveGeneratorParameter.ValidValues.Add(generator);
        }
      }
      if (oldMoveGenerator != null && MoveGeneratorParameter.ValidValues.Any(x => x.GetType() == oldMoveGenerator.GetType()))
        MoveGenerator = MoveGeneratorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveGenerator.GetType());
      if (MoveGenerator == null) {
        ClearMoveParameters();
      }
    }
    private void UpdateMoveParameters() {
      IMoveMaker oldMoveMaker = MoveMaker;
      ISingleObjectiveMoveEvaluator oldMoveEvaluator = MoveEvaluator;
      ITabuChecker oldTabuMoveEvaluator = TabuChecker;
      ITabuMaker oldTabuMoveMaker = TabuMaker;
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
          foreach (ITabuChecker tabuMoveEvaluator in operators.OfType<ITabuChecker>())
            TabuCheckerParameter.ValidValues.Add(tabuMoveEvaluator);
          foreach (ITabuMaker tabuMoveMaker in operators.OfType<ITabuMaker>())
            TabuMakerParameter.ValidValues.Add(tabuMoveMaker);
        }
        if (oldMoveMaker != null) {
          IMoveMaker mm = MoveMakerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveMaker.GetType());
          if (mm != null) MoveMaker = mm;
        }
        if (oldMoveEvaluator != null) {
          ISingleObjectiveMoveEvaluator me = MoveEvaluatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMoveEvaluator.GetType());
          if (me != null) MoveEvaluator = me;
        }
        if (oldTabuMoveMaker != null) {
          ITabuMaker tmm = TabuMakerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldTabuMoveMaker.GetType());
          if (tmm != null) TabuMaker = tmm;
        }
        if (oldTabuMoveEvaluator != null) {
          ITabuChecker tme = TabuCheckerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldTabuMoveEvaluator.GetType());
          if (tme != null) TabuChecker = tme;
        }
      }
    }
    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      MoveAnalyzer.Operators.Clear();
      MoveAnalyzer.Operators.Add(moveQualityAnalyzer);
      MoveAnalyzer.Operators.Add(tabuNeighborhoodAnalyzer);
      if (Problem != null) {
        foreach (ISolutionAnalyzer analyzer in Problem.Operators.OfType<ISolutionAnalyzer>().OrderBy(x => x.Name))
          Analyzer.Operators.Add(analyzer);
      }
    }
    private void ClearMoveParameters() {
      MoveMakerParameter.ValidValues.Clear();
      MoveEvaluatorParameter.ValidValues.Clear();
      TabuCheckerParameter.ValidValues.Clear();
      TabuMakerParameter.ValidValues.Clear();
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
      if (TabuChecker != null)
        MainLoop.MoveTabuParameter.ActualName = TabuChecker.MoveTabuParameter.ActualName;
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      if (op is IStochasticOperator)
        ((IStochasticOperator)op).RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
    }
    private void ParameterizeMoveGenerators() {
      if (Problem != null) {
        foreach (IMultiMoveGenerator generator in Problem.Operators.OfType<IMultiMoveGenerator>())
          generator.SampleSizeParameter.ActualName = SampleSizeParameter.Name;
      }
    }
    private void ParameterizeMoveEvaluator() {
      foreach (ISingleObjectiveMoveEvaluator op in Problem.Operators.OfType<ISingleObjectiveMoveEvaluator>()) {
        op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      }
    }
    private void ParameterizeMoveMaker() {
      foreach (IMoveMaker op in Problem.Operators.OfType<IMoveMaker>()) {
        op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        if (MoveEvaluator != null)
          op.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
      }
    }
    private void ParameterizeTabuMaker() {
      foreach (ITabuMaker op in Problem.Operators.OfType<ITabuMaker>()) {
        op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        if (MoveEvaluator != null)
          op.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
      }
    }
    private void ParameterizeTabuChecker() {
      foreach (ITabuChecker op in Problem.Operators.OfType<ITabuChecker>()) {
        if (MoveEvaluator != null)
          op.MoveQualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
        if (TabuChecker != null)
          op.MoveTabuParameter.ActualName = TabuChecker.MoveTabuParameter.ActualName;
      }
    }
    private void ParameterizeAnalyzers() {
      moveQualityAnalyzer.ResultsParameter.ActualName = "Results";
      tabuNeighborhoodAnalyzer.ResultsParameter.ActualName = "Results";
      tabuNeighborhoodAnalyzer.PercentTabuParameter.ActualName = "PercentTabu";
      if (Problem != null) {
        moveQualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        if (MoveEvaluator != null)
          moveQualityAnalyzer.QualityParameter.ActualName = MoveEvaluator.MoveQualityParameter.ActualName;
        moveQualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        if (TabuChecker != null)
          tabuNeighborhoodAnalyzer.IsTabuParameter.ActualName = TabuChecker.MoveTabuParameter.ActualName;
      }
    }
    #endregion
  }
}
