
#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.GradientDescent {
  /// <summary>
  /// Limited-Memory BFGS optimization algorithm.
  /// </summary>
  [Item("LM-BFGS", "The limited-memory BFGS (Broyden–Fletcher–Goldfarb–Shanno) optimization algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class LbfgsAlgorithm : HeuristicOptimizationEngineAlgorithm, IStorableContent {
    public override Type ProblemType {
      get { return typeof(ISingleObjectiveHeuristicOptimizationProblem); }
    }

    public new ISingleObjectiveHeuristicOptimizationProblem Problem {
      get { return (ISingleObjectiveHeuristicOptimizationProblem)base.Problem; }
      set { base.Problem = value; }
    }

    public string Filename { get; set; }

    private const string MaxIterationsParameterName = "MaxIterations";
    private const string ApproximateGradientsParameterName = "ApproximateGradients";
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";

    #region parameter properties
    public IValueParameter<IntValue> MaxIterationsParameter {
      get { return (IValueParameter<IntValue>)Parameters[MaxIterationsParameterName]; }
    }
    public IValueParameter<IntValue> SeedParameter {
      get { return (IValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }
    public IValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }
    #endregion
    #region properties
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
    private LbfgsAnalyzer finalAnalyzer;
    [Storable]
    private Placeholder solutionCreator;
    [Storable]
    private Placeholder evaluator;

    [StorableConstructor]
    private LbfgsAlgorithm(bool deserializing) : base(deserializing) { }
    private LbfgsAlgorithm(LbfgsAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      initializer = cloner.Clone(original.initializer);
      makeStep = cloner.Clone(original.makeStep);
      updateResults = cloner.Clone(original.updateResults);
      analyzer = cloner.Clone(original.analyzer);
      finalAnalyzer = cloner.Clone(original.finalAnalyzer);
      solutionCreator = cloner.Clone(original.solutionCreator);
      evaluator = cloner.Clone(original.evaluator);
      RegisterEvents();
    }
    public LbfgsAlgorithm()
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;

      Parameters.Add(new ValueParameter<IntValue>(MaxIterationsParameterName, "The maximal number of iterations for.", new IntValue(20)));
      Parameters.Add(new ValueParameter<IntValue>(SeedParameterName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>(SetSeedRandomlyParameterName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<BoolValue>(ApproximateGradientsParameterName, "Indicates that gradients should be approximated.", new BoolValue(true)));
      Parameters[ApproximateGradientsParameterName].Hidden = true; // should not be changed

      var randomCreator = new RandomCreator();
      solutionCreator = new Placeholder();
      initializer = new LbfgsInitializer();
      makeStep = new LbfgsMakeStep();
      var branch = new ConditionalBranch();
      evaluator = new Placeholder();
      updateResults = new LbfgsUpdateResults();
      analyzer = new LbfgsAnalyzer();
      finalAnalyzer = new LbfgsAnalyzer();

      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.SeedParameter.ActualName = SeedParameterName;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameterName;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionCreator;

      solutionCreator.Name = "Solution Creator (placeholder)";
      solutionCreator.Successor = initializer;

      initializer.IterationsParameter.ActualName = MaxIterationsParameterName;
      initializer.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      initializer.Successor = makeStep;

      makeStep.StateParameter.ActualName = initializer.StateParameter.Name;
      makeStep.Successor = branch;

      branch.ConditionParameter.ActualName = makeStep.TerminationCriterionParameter.Name;
      branch.FalseBranch = evaluator;
      branch.TrueBranch = finalAnalyzer;

      evaluator.Name = "Evaluator (placeholder)";
      evaluator.Successor = updateResults;

      updateResults.StateParameter.ActualName = initializer.StateParameter.Name;
      updateResults.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      updateResults.Successor = analyzer;

      analyzer.StateParameter.ActualName = initializer.StateParameter.Name;
      analyzer.Successor = makeStep;

      finalAnalyzer.PointsTableParameter.ActualName = analyzer.PointsTableParameter.ActualName;
      finalAnalyzer.QualityGradientsTableParameter.ActualName = analyzer.QualityGradientsTableParameter.ActualName;
      finalAnalyzer.QualitiesTableParameter.ActualName = analyzer.QualitiesTableParameter.ActualName;
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
        evaluator.OperatorParameter.ActualName = Problem.EvaluatorParameter.Name;
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

    private void ParameterizeOperators() {
      var realVectorCreator = Problem.SolutionCreator as IRealVectorCreator;
      // ignore if we have a different kind of problem
      if (realVectorCreator != null) {
        var realVectorParameterName = realVectorCreator.RealVectorParameter.ActualName;
        initializer.PointParameter.ActualName = realVectorParameterName;
        makeStep.PointParameter.ActualName = realVectorParameterName;
        analyzer.PointParameter.ActualName = realVectorParameterName;
        finalAnalyzer.PointParameter.ActualName = realVectorParameterName;
      }

      var qualityParameterName = Problem.Evaluator.QualityParameter.ActualName;
      updateResults.QualityParameter.ActualName = qualityParameterName;
      analyzer.QualityParameter.ActualName = qualityParameterName;
      finalAnalyzer.QualityParameter.ActualName = qualityParameterName;
    }
  }
}
