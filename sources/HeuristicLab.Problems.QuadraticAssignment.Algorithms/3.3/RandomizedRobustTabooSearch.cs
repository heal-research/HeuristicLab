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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.QuadraticAssignment.Algorithms {
  [Item("Randomized Robust Taboo Search (QAP)", "This algorithm is based on Taillard, E. 1991. Robust Taboo Search for the Quadratic Assignment Problem. Parallel Computing 17, pp. 443-455.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class RandomizedRobustTabooSearchQAP : HeuristicOptimizationEngineAlgorithm, IStorableContent {
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
    private FixedValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (FixedValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    private FixedValueParameter<IntValue> SeedParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private FixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    private FixedValueParameter<IntValue> MaximumIterationsParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    private FixedValueParameter<IntValue> MinimumTabuTenureParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["MinimumTabuTenure"]; }
    }
    private FixedValueParameter<IntValue> MaximumTabuTenureParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["MaximumTabuTenure"]; }
    }
    private FixedValueParameter<IntValue> TabuTenureAdaptionIntervalParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["TabuTenureAdaptionInterval"]; }
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
    public int TabuTenureAdaptionInterval {
      get { return TabuTenureAdaptionIntervalParameter.Value.Value; }
      set { TabuTenureAdaptionIntervalParameter.Value.Value = value; }
    }
    #endregion

    [Storable]
    private SolutionsCreator solutionsCreator;
    [Storable]
    private QAPRandomizedRobustTabooSeachOperator mainOperator;
    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;

    [StorableConstructor]
    private RandomizedRobustTabooSearchQAP(bool deserializing) : base(deserializing) { }
    private RandomizedRobustTabooSearchQAP(RandomizedRobustTabooSearchQAP original, Cloner cloner)
      : base(original, cloner) {
      solutionsCreator = cloner.Clone(original.solutionsCreator);
      mainOperator = cloner.Clone(original.mainOperator);
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
    }
    public RandomizedRobustTabooSearchQAP() {
      Parameters.Add(new FixedValueParameter<MultiAnalyzer>("Analyzer", "The analyzers that are applied after each iteration.", new MultiAnalyzer()));
      Parameters.Add(new FixedValueParameter<IntValue>("Seed", "The seed value of the random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>("SetSeedRandomly", "True whether the seed should be set randomly for each run, false if it should be fixed.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumIterations", "The number of iterations that the algorithm should run.", new IntValue(10000)));
      Parameters.Add(new FixedValueParameter<IntValue>("MinimumTabuTenure", "The minimum tabu tenure.", new IntValue(20)));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumTabuTenure", "The maximum tabu tenure.", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<IntValue>("TabuTenureAdaptionInterval", "The amount of iterations that have to pass before the tabu tenure is adapted.", new IntValue(60)));

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      AnalyzerParameter.Value.Operators.Add(qualityAnalyzer);

      RandomCreator randomCreator = new RandomCreator();
      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.Value = null;
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;

      OperatorGraph.InitialOperator = randomCreator;

      VariableCreator variableCreator = new VariableCreator();
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("TabuTenure", new IntValue(0)));
      randomCreator.Successor = variableCreator;

      ResultsCollector resultsCollector = new ResultsCollector();
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Iterations", "The actual iteration."));
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("TabuTenure", "The actual tabu tenure."));
      variableCreator.Successor = resultsCollector;

      solutionsCreator = new SolutionsCreator();
      solutionsCreator.NumberOfSolutions = new IntValue(1);
      resultsCollector.Successor = solutionsCreator;

      Placeholder analyzer = new Placeholder();
      analyzer.Name = "(Analyzer)";
      analyzer.OperatorParameter.ActualName = AnalyzerParameter.Name;
      solutionsCreator.Successor = analyzer;

      UniformSubScopesProcessor ussp = new UniformSubScopesProcessor();
      analyzer.Successor = ussp;

      mainOperator = new QAPRandomizedRobustTabooSeachOperator();
      mainOperator.BestQualityParameter.ActualName = "BestSoFarQuality";
      mainOperator.IterationsParameter.ActualName = "Iterations";
      mainOperator.LongTermMemoryParameter.ActualName = "LongTermMemory";
      mainOperator.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
      mainOperator.MaximumTabuTenureParameter.ActualName = MaximumTabuTenureParameter.Name;
      mainOperator.MinimumTabuTenureParameter.ActualName = MinimumTabuTenureParameter.Name;
      mainOperator.RandomParameter.ActualName = "Random";
      mainOperator.ShortTermMemoryParameter.ActualName = "ShortTermMemory";
      mainOperator.TabuTenureAdaptionIntervalParameter.ActualName = TabuTenureAdaptionIntervalParameter.Name;
      mainOperator.TabuTenureParameter.ActualName = "TabuTenure";
      mainOperator.ResultsParameter.ActualName = "Results";
      ussp.Operator = mainOperator;

      IntCounter iterationsCounter = new IntCounter();
      iterationsCounter.ValueParameter.ActualName = "Iterations";
      iterationsCounter.Increment = new IntValue(1);
      ussp.Successor = iterationsCounter;

      Comparator comparator = new Comparator();
      comparator.Name = "Iterations < MaximumIterations ?";
      comparator.LeftSideParameter.ActualName = "Iterations";
      comparator.RightSideParameter.ActualName = MaximumIterationsParameter.Name;
      comparator.Comparison = new Comparison(ComparisonType.Less);
      comparator.ResultParameter.ActualName = "ContinueByIteration";
      iterationsCounter.Successor = comparator;

      ConditionalBranch branch = new ConditionalBranch();
      branch.ConditionParameter.ActualName = "ContinueByIteration";
      branch.TrueBranch = analyzer;
      comparator.Successor = branch;

      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomizedRobustTabooSearchQAP(this, cloner);
    }

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

    private void RegisterEventHandlers() {
      if (Problem != null) {
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      }
    }

    public override void Start() {
      if (ExecutionState == ExecutionState.Prepared) {
        IntMatrix shortTermMemory = new IntMatrix(Problem.Weights.Rows, Problem.Weights.Rows);
        DoubleMatrix shortTermMemory2 = new DoubleMatrix(Problem.Weights.Rows, Problem.Weights.Rows);
        IntMatrix longTermMemory = new IntMatrix(Problem.Weights.Rows, Problem.Weights.Rows);
        for (int i = 0; i < shortTermMemory.Rows; i++)
          for (int j = 0; j < shortTermMemory.Rows; j++) {
            shortTermMemory[i, j] = -1;
            shortTermMemory2[i, j] = double.MinValue;
            longTermMemory[i, j] = -1;
          }

        GlobalScope.Variables.Add(new Variable("ShortTermMemory", shortTermMemory));
        GlobalScope.Variables.Add(new Variable("ShortTermMemory2", shortTermMemory2));
        GlobalScope.Variables.Add(new Variable("LongTermMemory", longTermMemory));
      }
      base.Start();
    }

    private void UpdateProblemSpecificParameters() {
      MinimumTabuTenure = (int)(0.9 * Problem.Weights.Rows);
      MaximumTabuTenure = (int)(1.1 * Problem.Weights.Rows);
      TabuTenureAdaptionInterval = 2 * MaximumTabuTenure;
    }

    private void UpdateAnalyzers() {
      AnalyzerParameter.Value.Operators.Clear();
      if (Problem != null) {
        foreach (IAnalyzer analyzer in ((IProblem)Problem).Operators.OfType<IAnalyzer>())
          if (!(analyzer is AlleleFrequencyAnalyzer<Permutation>) && !(analyzer is PopulationDiversityAnalyzer<Permutation>))
            AnalyzerParameter.Value.Operators.Add(analyzer);
      }
      AnalyzerParameter.Value.Operators.Add(qualityAnalyzer);
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
  }
}
