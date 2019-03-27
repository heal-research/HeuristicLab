
#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.GradientDescent {
  /// <summary>
  /// Limited-Memory BFGS optimization algorithm.
  /// </summary>
  [Item("LM-BFGS", "The limited-memory BFGS (Broyden–Fletcher–Goldfarb–Shanno) optimization algorithm.")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms, Priority = 160)]
  [StorableType("55E85596-0FC7-41B5-9B90-9A8BF33B7C55")]
  public sealed class LbfgsAlgorithm : HeuristicOptimizationEngineAlgorithm, IStorableContent {
    public override Type ProblemType {
      get { return typeof(ISingleObjectiveHeuristicOptimizationProblem); }
    }

    public new ISingleObjectiveHeuristicOptimizationProblem Problem {
      get { return (ISingleObjectiveHeuristicOptimizationProblem)base.Problem; }
      set { base.Problem = value; }
    }

    public string Filename { get; set; }

    private const string AnalyzerParameterName = "Analyzer";
    private const string MaxIterationsParameterName = "MaxIterations";
    private const string ApproximateGradientsParameterName = "ApproximateGradients";
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    private const string GradientCheckStepSizeParameterName = "GradientCheckStepSize";

    #region parameter properties
    public IValueParameter<IMultiAnalyzer> AnalyzerParameter {
      get { return (IValueParameter<IMultiAnalyzer>)Parameters[AnalyzerParameterName]; }
    }
    public IValueParameter<IntValue> MaxIterationsParameter {
      get { return (IValueParameter<IntValue>)Parameters[MaxIterationsParameterName]; }
    }
    public IValueParameter<IntValue> SeedParameter {
      get { return (IValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }
    public IValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }
    public IValueParameter<DoubleValue> GradientStepSizeParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[GradientCheckStepSizeParameterName]; }
    }
    #endregion
    #region properties
    public IMultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    public int MaxIterations {
      set { MaxIterationsParameter.Value.Value = value; }
      get { return MaxIterationsParameter.Value.Value; }
    }
    public int Seed { get { return SeedParameter.Value.Value; } set { SeedParameter.Value.Value = value; } }
    public bool SetSeedRandomly { get { return SetSeedRandomlyParameter.Value.Value; } set { SetSeedRandomlyParameter.Value.Value = value; } }
    #endregion

    [Storable]
    private LbfgsInitializer initializer;
    [Storable]
    private LbfgsMakeStep makeStep;
    [Storable]
    private LbfgsUpdateResults updateResults;
    [Storable]
    private LbfgsAnalyzer analyzer;
    [Storable]
    private Placeholder solutionCreator;
    [Storable]
    private Placeholder evaluator;

    [StorableConstructor]
    private LbfgsAlgorithm(StorableConstructorFlag _) : base(_) { }
    private LbfgsAlgorithm(LbfgsAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      initializer = cloner.Clone(original.initializer);
      makeStep = cloner.Clone(original.makeStep);
      updateResults = cloner.Clone(original.updateResults);
      analyzer = cloner.Clone(original.analyzer);
      solutionCreator = cloner.Clone(original.solutionCreator);
      evaluator = cloner.Clone(original.evaluator);
      RegisterEvents();
    }
    public LbfgsAlgorithm()
      : base() {
      Parameters.Add(new ValueParameter<IMultiAnalyzer>(AnalyzerParameterName, "The analyzers that will be executed on the solution.", new MultiAnalyzer()));
      Parameters.Add(new ValueParameter<IntValue>(MaxIterationsParameterName, "The maximal number of iterations for.", new IntValue(20)));
      Parameters.Add(new ValueParameter<IntValue>(SeedParameterName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>(SetSeedRandomlyParameterName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<BoolValue>(ApproximateGradientsParameterName, "Indicates that gradients should be approximated.", new BoolValue(true)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>(GradientCheckStepSizeParameterName, "Step size for the gradient check (should be used for debugging the gradient calculation only)."));
      // these parameter should not be changed usually
      Parameters[ApproximateGradientsParameterName].Hidden = true;
      Parameters[GradientCheckStepSizeParameterName].Hidden = true;

      var randomCreator = new RandomCreator();
      solutionCreator = new Placeholder();
      initializer = new LbfgsInitializer();
      makeStep = new LbfgsMakeStep();
      var branch = new ConditionalBranch();
      evaluator = new Placeholder();
      updateResults = new LbfgsUpdateResults();
      var analyzerPlaceholder = new Placeholder();
      var finalAnalyzerPlaceholder = new Placeholder();

      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.SeedParameter.ActualName = SeedParameterName;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameterName;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionCreator;

      solutionCreator.Name = "(Solution Creator)";
      solutionCreator.Successor = initializer;

      initializer.IterationsParameter.ActualName = MaxIterationsParameterName;
      initializer.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      initializer.Successor = makeStep;

      makeStep.StateParameter.ActualName = initializer.StateParameter.Name;
      makeStep.Successor = branch;

      branch.ConditionParameter.ActualName = makeStep.TerminationCriterionParameter.Name;
      branch.FalseBranch = evaluator;
      branch.TrueBranch = finalAnalyzerPlaceholder;

      evaluator.Name = "(Evaluator)";
      evaluator.Successor = updateResults;

      updateResults.StateParameter.ActualName = initializer.StateParameter.Name;
      updateResults.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      updateResults.Successor = analyzerPlaceholder;

      analyzerPlaceholder.Name = "(Analyzer)";
      analyzerPlaceholder.OperatorParameter.ActualName = AnalyzerParameterName;
      analyzerPlaceholder.Successor = makeStep;

      finalAnalyzerPlaceholder.Name = "(Analyzer)";
      finalAnalyzerPlaceholder.OperatorParameter.ActualName = AnalyzerParameterName;
      finalAnalyzerPlaceholder.Successor = null;

      analyzer = new LbfgsAnalyzer();
      analyzer.StateParameter.ActualName = initializer.StateParameter.Name;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LbfgsAlgorithm(this, cloner);
    }

    #region events
    private void RegisterEvents() {
      if (Problem != null) {
        RegisterSolutionCreatorEvents();
        RegisterEvaluatorEvents();
      }
    }

    protected override void OnProblemChanged() {
      base.OnProblemChanged();
      if (Problem != null) {
        RegisterEvents();
        solutionCreator.OperatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
        solutionCreator.OperatorParameter.Hidden = true;
        evaluator.OperatorParameter.ActualName = Problem.EvaluatorParameter.Name;
        evaluator.OperatorParameter.Hidden = true;
        UpdateAnalyzers();
        ParameterizeOperators();
      }
    }

    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      base.Problem_SolutionCreatorChanged(sender, e);
      RegisterSolutionCreatorEvents();
      ParameterizeOperators();
    }

    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      base.Problem_EvaluatorChanged(sender, e);
      RegisterEvaluatorEvents();
      ParameterizeOperators();
    }

    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      base.Problem_OperatorsChanged(sender, e);
      RegisterEvents();
      solutionCreator.OperatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
      solutionCreator.OperatorParameter.Hidden = true;
      evaluator.OperatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      evaluator.OperatorParameter.Hidden = true;
      UpdateAnalyzers();
      ParameterizeOperators();
    }

    private void RegisterSolutionCreatorEvents() {
      var realVectorCreator = Problem.SolutionCreator as IRealVectorCreator;
      // ignore if we have a different kind of problem
      if (realVectorCreator != null) {
        realVectorCreator.RealVectorParameter.ActualNameChanged += (sender, args) => ParameterizeOperators();
      }
    }

    private void RegisterEvaluatorEvents() {
      Problem.Evaluator.QualityParameter.ActualNameChanged += (sender, args) => ParameterizeOperators();
    }
    #endregion

    protected override void OnStarted() {
      var realVectorCreator = Problem.SolutionCreator as IRealVectorCreator;
      // must catch the case that user loaded an unsupported problem
      if (realVectorCreator == null)
        throw new InvalidOperationException("LM-BFGS only works with problems using a real-value encoding.");
      base.OnStarted();
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (Problem != null) {
        foreach (var a in Problem.Operators.OfType<IAnalyzer>()) {
          foreach (var param in a.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 0;
          Analyzer.Operators.Add(a, a.EnabledByDefault);
        }
      }
      Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
    }

    private void ParameterizeOperators() {
      var realVectorCreator = Problem.SolutionCreator as IRealVectorCreator;
      // ignore if we have a different kind of problem
      if (realVectorCreator != null) {
        var realVectorParameterName = realVectorCreator.RealVectorParameter.ActualName;
        initializer.PointParameter.ActualName = realVectorParameterName;
        initializer.PointParameter.Hidden = true;
        makeStep.PointParameter.ActualName = realVectorParameterName;
        makeStep.PointParameter.Hidden = true;
        analyzer.PointParameter.ActualName = realVectorParameterName;
        analyzer.PointParameter.Hidden = true;
      }

      var qualityParameterName = Problem.Evaluator.QualityParameter.ActualName;
      updateResults.QualityParameter.ActualName = qualityParameterName;
      updateResults.QualityParameter.Hidden = true;
      analyzer.QualityParameter.ActualName = qualityParameterName;
      analyzer.QualityParameter.Hidden = true;
    }
  }
}
