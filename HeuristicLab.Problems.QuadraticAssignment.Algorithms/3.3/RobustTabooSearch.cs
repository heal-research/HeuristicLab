#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Random;

namespace HeuristicLab.Problems.QuadraticAssignment.Algorithms {
  [Item("Robust Taboo Search", "The algorithm is described in Taillard, E. 1991. Robust Taboo Search for the Quadratic Assignment Problem. Parallel Computing 17, pp. 443-455.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class RobustTabooSearch : HeuristicOptimizationEngineAlgorithm, IStorableContent {
    public string Filename { get; set; }

    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(QuadraticAssignmentProblem); }
    }
    public new QuadraticAssignmentProblem Problem {
      get { return (QuadraticAssignmentProblem)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion

    #region Parameter Properties
    public IValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (IValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    public FixedValueParameter<IntValue> SeedParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["Seed"]; }
    }
    public FixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    public FixedValueParameter<IntValue> MaximumIterationsParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    public FixedValueParameter<IntValue> MinimumTabuTenureParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["MinimumTabuTenure"]; }
    }
    public FixedValueParameter<IntValue> MaximumTabuTenureParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["MaximumTabuTenure"]; }
    }
    public FixedValueParameter<BoolValue> UseAlternativeAspirationParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters["UseAlternativeAspiration"]; }
    }
    public FixedValueParameter<IntValue> AlternativeAspirationTenureParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["AlternativeAspirationTenure"]; }
    }
    public FixedValueParameter<BoolValue> UseNewTabuTenureAdaptionSchemeParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters["UseNewTabuTenureAdaptionScheme"]; }
    }
    public FixedValueParameter<BoolValue> TerminateOnOptimalSolutionParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters["TerminateOnOptimalSolution"]; }
    }
    #endregion

    #region Properties
    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }
    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }
    public int MaximumIterations {
      get { return MaximumIterationsParameter.Value.Value; }
      set { MaximumIterationsParameter.Value.Value = value; }
    }
    public int MinimumTabuTenure {
      get { return MinimumTabuTenureParameter.Value.Value; }
      set { MinimumTabuTenureParameter.Value.Value = value; }
    }
    public int MaximumTabuTenure {
      get { return MaximumTabuTenureParameter.Value.Value; }
      set { MaximumTabuTenureParameter.Value.Value = value; }
    }
    public bool UseAlternativeAspiration {
      get { return UseAlternativeAspirationParameter.Value.Value; }
      set { UseAlternativeAspirationParameter.Value.Value = value; }
    }
    public int AlternativeAspirationTenure {
      get { return AlternativeAspirationTenureParameter.Value.Value; }
      set { AlternativeAspirationTenureParameter.Value.Value = value; }
    }
    public bool UseNewTabuTenureAdaptionScheme {
      get { return UseNewTabuTenureAdaptionSchemeParameter.Value.Value; }
      set { UseNewTabuTenureAdaptionSchemeParameter.Value.Value = value; }
    }
    public bool TerminateOnOptimalSolution {
      get { return TerminateOnOptimalSolutionParameter.Value.Value; }
      set { TerminateOnOptimalSolutionParameter.Value.Value = value; }
    }
    #endregion

    [Storable]
    private SolutionsCreator solutionsCreator;
    [Storable]
    private RobustTabooSeachOperator mainOperator;
    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;

    [StorableConstructor]
    private RobustTabooSearch(bool deserializing) : base(deserializing) { }
    private RobustTabooSearch(RobustTabooSearch original, Cloner cloner)
      : base(original, cloner) {
      solutionsCreator = cloner.Clone(original.solutionsCreator);
      mainOperator = cloner.Clone(original.mainOperator);
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      RegisterEventHandlers();
    }
    public RobustTabooSearch() {
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The analyzers that are applied after each iteration.", new MultiAnalyzer()));
      Parameters.Add(new FixedValueParameter<IntValue>("Seed", "The seed value of the random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>("SetSeedRandomly", "True whether the seed should be set randomly for each run, false if it should be fixed.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumIterations", "The number of iterations that the algorithm should run.", new IntValue(10000)));
      Parameters.Add(new FixedValueParameter<IntValue>("MinimumTabuTenure", "The minimum tabu tenure.", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumTabuTenure", "The maximum tabu tenure.", new IntValue(20)));
      Parameters.Add(new FixedValueParameter<BoolValue>("UseAlternativeAspiration", "True if the alternative aspiration condition should be used that takes moves that have not been made for some time above others.", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>("AlternativeAspirationTenure", "The time t that a move will be remembered for the alternative aspiration condition.", new IntValue(int.MaxValue)));
      Parameters.Add(new FixedValueParameter<BoolValue>("TerminateOnOptimalSolution", "True when the algorithm should stop if it reached a quality equal or smaller to the BestKnownQuality.", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<BoolValue>("UseNewTabuTenureAdaptionScheme", @"In an updated version of his implementation, Eric Taillard introduced a different way to change the tabu tenure.
Instead of setting it uniformly between min and max, it will be set between 0 and max according to a right-skewed distribution.
Set this option to false if you want to optimize using the earlier 1991 version, and set to true if you want to optimize using the newer version.
Please note that the MinimumTabuTenure parameter has no effect in the new version.", new BoolValue(true)));

      TerminateOnOptimalSolutionParameter.Hidden = true;

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      AnalyzerParameter.Value.Operators.Add(qualityAnalyzer);

      RandomCreator randomCreator = new RandomCreator();
      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.Value = null;
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;

      VariableCreator variableCreator = new VariableCreator();
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));

      ResultsCollector resultsCollector = new ResultsCollector();
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Iterations", "The actual iteration."));

      solutionsCreator = new SolutionsCreator();
      solutionsCreator.NumberOfSolutions = new IntValue(1);

      Placeholder analyzer = new Placeholder();
      analyzer.Name = "(Analyzer)";
      analyzer.OperatorParameter.ActualName = AnalyzerParameter.Name;

      UniformSubScopesProcessor ussp = new UniformSubScopesProcessor();

      mainOperator = new RobustTabooSeachOperator();
      mainOperator.AlternativeAspirationTenureParameter.ActualName = AlternativeAspirationTenureParameter.Name;
      mainOperator.BestQualityParameter.ActualName = "BestSoFarQuality";
      mainOperator.IterationsParameter.ActualName = "Iterations";
      mainOperator.LastMoveParameter.ActualName = "LastMove";
      mainOperator.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
      mainOperator.MaximumTabuTenureParameter.ActualName = MaximumTabuTenureParameter.Name;
      mainOperator.MinimumTabuTenureParameter.ActualName = MinimumTabuTenureParameter.Name;
      mainOperator.MoveQualityMatrixParameter.ActualName = "MoveQualityMatrix";
      mainOperator.RandomParameter.ActualName = "Random";
      mainOperator.ResultsParameter.ActualName = "Results";
      mainOperator.ShortTermMemoryParameter.ActualName = "ShortTermMemory";
      mainOperator.UseAlternativeAspirationParameter.ActualName = UseAlternativeAspirationParameter.Name;

      ConditionalBranch qualityStopBranch = new ConditionalBranch();
      qualityStopBranch.Name = "Terminate on optimal quality?";
      qualityStopBranch.ConditionParameter.ActualName = "TerminateOnOptimalSolution";

      Comparator qualityComparator = new Comparator();
      qualityComparator.Comparison = new Comparison(ComparisonType.Greater);
      qualityComparator.LeftSideParameter.ActualName = "BestQuality";
      qualityComparator.RightSideParameter.ActualName = "BestKnownQuality";
      qualityComparator.ResultParameter.ActualName = "ContinueByQuality";

      ConditionalBranch continueByQualityBranch = new ConditionalBranch();
      continueByQualityBranch.ConditionParameter.ActualName = "ContinueByQuality";

      IntCounter iterationsCounter = new IntCounter();
      iterationsCounter.ValueParameter.ActualName = "Iterations";
      iterationsCounter.Increment = new IntValue(1);

      Comparator comparator = new Comparator();
      comparator.Name = "Iterations < MaximumIterations ?";
      comparator.LeftSideParameter.ActualName = "Iterations";
      comparator.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      comparator.Comparison = new Comparison(ComparisonType.Less);
      comparator.ResultParameter.ActualName = "ContinueByIteration";

      ConditionalBranch continueByIterationBranch = new ConditionalBranch();
      continueByIterationBranch.ConditionParameter.ActualName = "ContinueByIteration";

      OperatorGraph.InitialOperator = randomCreator;
      randomCreator.Successor = variableCreator;
      variableCreator.Successor = resultsCollector;
      resultsCollector.Successor = solutionsCreator;
      solutionsCreator.Successor = analyzer;
      analyzer.Successor = ussp;
      ussp.Operator = mainOperator;
      ussp.Successor = qualityStopBranch;
      qualityStopBranch.FalseBranch = iterationsCounter;
      qualityStopBranch.TrueBranch = qualityComparator;
      qualityStopBranch.Successor = null;
      qualityComparator.Successor = continueByQualityBranch;
      continueByQualityBranch.TrueBranch = iterationsCounter;
      continueByQualityBranch.FalseBranch = null;
      continueByQualityBranch.Successor = null;
      iterationsCounter.Successor = comparator;
      comparator.Successor = continueByIterationBranch;
      continueByIterationBranch.TrueBranch = analyzer;
      continueByIterationBranch.FalseBranch = null;
      continueByIterationBranch.Successor = null;

      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RobustTabooSearch(this, cloner);
    }

    #region Event Handlers
    protected override void OnProblemChanged() {
      base.OnProblemChanged();
      UpdateProblemSpecificParameters();
      ParameterizeOperators();
      UpdateAnalyzers();
    }

    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      base.Problem_EvaluatorChanged(sender, e);
      ParameterizeOperators();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
    }

    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      base.Problem_OperatorsChanged(sender, e);
      UpdateAnalyzers();
    }

    protected override void Problem_Reset(object sender, EventArgs e) {
      base.Problem_Reset(sender, e);
      UpdateProblemSpecificParameters();
      ParameterizeOperators();
      UpdateAnalyzers();
    }

    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeOperators();
    }

    private void UseAlternativeAspirationParameter_ValueChanged(object sender, EventArgs e) {
      UpdateAlternativeAspirationTenure();
    }

    private void AlternativeAspirationTenureParameter_ValueChanged(object sender, EventArgs e) {
      if (AlternativeAspirationTenure < MaximumIterations && !UseAlternativeAspiration) {
        SetSilentlyUseAlternativeAspirationParameter(true);
      } else if (AlternativeAspirationTenure >= MaximumIterations && UseAlternativeAspiration) {
        SetSilentlyUseAlternativeAspirationParameter(false);
      }
    }

    private void MaximumIterationsParameter_ValueChanged(object sender, EventArgs e) {
      if (MaximumIterations < AlternativeAspirationTenure && UseAlternativeAspiration) {
        SetSilentlyUseAlternativeAspirationParameter(false);
      } else if (MaximumIterations >= AlternativeAspirationTenure && !UseAlternativeAspiration) {
        SetSilentlyUseAlternativeAspirationParameter(true);
      }
    }

    private void UseNewTabuTenureAdaptionSchemeParameter_ValueChanged(object sender, EventArgs e) {
      UpdateProblemSpecificParameters();
    }
    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (Parameters["Analyzer"] is FixedValueParameter<MultiAnalyzer>) {
        MultiAnalyzer analyzer = AnalyzerParameter.Value;
        Parameters.Remove("Analyzer");
        Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The analyzers that are applied after each iteration.", analyzer));
      }
      #endregion
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      UseAlternativeAspirationParameter.Value.ValueChanged += new EventHandler(UseAlternativeAspirationParameter_ValueChanged);
      AlternativeAspirationTenureParameter.Value.ValueChanged += new EventHandler(AlternativeAspirationTenureParameter_ValueChanged);
      MaximumIterationsParameter.Value.ValueChanged += new EventHandler(MaximumIterationsParameter_ValueChanged);
      UseNewTabuTenureAdaptionSchemeParameter.Value.ValueChanged += new EventHandler(UseNewTabuTenureAdaptionSchemeParameter_ValueChanged);
    }

    protected override void RegisterProblemEvents() {
      base.RegisterProblemEvents();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
    }

    protected override void DeregisterProblemEvents() {
      Problem.Evaluator.QualityParameter.ActualNameChanged -= new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.DeregisterProblemEvents();
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    public override void Start() {
      if (ExecutionState == ExecutionState.Prepared) {
        int dim = Problem.Weights.Rows;
        IntMatrix shortTermMemory = new IntMatrix(dim, dim);
        for (int i = 0; i < dim; i++)
          for (int j = 0; j < dim; j++) {
            shortTermMemory[i, j] = -(dim * (i + 1) + j + 1);
          }

        GlobalScope.Variables.Add(new Variable("ShortTermMemory", shortTermMemory));
        GlobalScope.Variables.Add(new Variable("MoveQualityMatrix", new DoubleMatrix(dim, dim)));
      }
      base.Start();
    }

    private void UpdateProblemSpecificParameters() {
      UpdateTabuTenure();
      UpdateAlternativeAspirationTenure();
    }

    private void UpdateTabuTenure() {
      if (UseNewTabuTenureAdaptionScheme) {
        MinimumTabuTenure = 0;
        MaximumTabuTenure = 8 * Problem.Weights.Rows;
      } else {
        MinimumTabuTenure = (int)(0.9 * Problem.Weights.Rows);
        MaximumTabuTenure = (int)(1.1 * Problem.Weights.Rows);
      }
    }

    private void UpdateAlternativeAspirationTenure() {
      if (UseAlternativeAspiration) {
        int n = Problem.Weights.Rows;
        // Taillard has given two formulas for calculating default values: n^2 / 2 and later n^2 * 5
        // However these do not really model the values he used in his original publication though
        // The following formula is a linear regression model on the problem size and parameters
        // given in Table 3 in Taillard1991 and was lower-bounded artificially by 100
        AlternativeAspirationTenure = Math.Max(203 * n - 2274, 100);
      } else {
        AlternativeAspirationTenure = int.MaxValue;
      }
    }

    private void UpdateAnalyzers() {
      AnalyzerParameter.Value.Operators.Clear();
      if (Problem != null) {
        foreach (IAnalyzer analyzer in ((IProblem)Problem).Operators.OfType<IAnalyzer>()) {
          AnalyzerParameter.Value.Operators.Add(analyzer, analyzer.EnabledByDefault);
          if (!(analyzer is BestQAPSolutionAnalyzer))
            AnalyzerParameter.Value.Operators.SetItemCheckedState(analyzer, false);
        }
      }
      AnalyzerParameter.Value.Operators.Add(qualityAnalyzer, qualityAnalyzer.EnabledByDefault);
    }

    private void ParameterizeOperators() {
      if (Problem != null) {
        solutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
        solutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;

        qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.Name;

        mainOperator.DistancesParameter.ActualName = Problem.DistancesParameter.Name;
        mainOperator.PermutationParameter.ActualName = Problem.SolutionCreator.PermutationParameter.ActualName;
        mainOperator.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        mainOperator.WeightsParameter.ActualName = Problem.WeightsParameter.Name;
      }
    }

    private void SetSilentlyUseAlternativeAspirationParameter(bool value) {
      UseAlternativeAspirationParameter.Value.ValueChanged -= new EventHandler(UseAlternativeAspirationParameter_ValueChanged);
      UseAlternativeAspiration = value;
      UseAlternativeAspirationParameter.Value.ValueChanged += new EventHandler(UseAlternativeAspirationParameter_ValueChanged);
    }
  }
}
