﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.TestFunctions {
  [Item("Test Function (single-objective)", "Test function with real valued inputs and a single objective.")]
  [StorableType("F0AB7236-2C9B-49DC-9D4F-A3558FD9E992")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 90)]
  public sealed class SingleObjectiveTestFunctionProblem : RealVectorProblem,
    IProblemInstanceConsumer<SOTFData> {

    #region Parameter Properties
    public OptionalValueParameter<RealVector> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<RealVector>)Parameters["BestKnownSolution"]; }
    }
    public IValueParameter<ISingleObjectiveTestFunction> TestFunctionParameter {
      get { return (IValueParameter<ISingleObjectiveTestFunction>)Parameters["TestFunction"]; }
    }
    #endregion

    #region Properties
    public ISingleObjectiveTestFunction TestFunction {
      get { return TestFunctionParameter.Value; }
      set { TestFunctionParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private SingleObjectiveTestFunctionProblem(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveTestFunctionProblem(SingleObjectiveTestFunctionProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public SingleObjectiveTestFunctionProblem()
      : base(new RealVectorEncoding("Point")) {
      Parameters.Add(new OptionalValueParameter<RealVector>("BestKnownSolution", "The best known solution for this test function instance."));
      Parameters.Add(new ValueParameter<ISingleObjectiveTestFunction>("TestFunction", "The function that is to be optimized.", new Ackley()));
      Maximization = TestFunction.Maximization;

      BestKnownQuality = TestFunction.BestKnownQuality;
      Bounds = (DoubleMatrix)TestFunction.Bounds.Clone();
      Dimension = TestFunction.MinimumProblemSize;
      BestKnownSolutionParameter.Value = TestFunction.GetBestKnownSolution(Dimension);
      InitializeOperators();
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveTestFunctionProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
      TestFunctionParameter.ValueChanged += TestFunctionParameterOnValueChanged;
    }

    public override ISingleObjectiveEvaluationResult Evaluate(RealVector individual, IRandom random, CancellationToken cancellationToken) {
      var quality = TestFunction.Evaluate(individual);
      return new SingleObjectiveEvaluationResult(quality);
    }

    public override void Analyze(RealVector[] realVectors, double[] qualities, ResultCollection results, IRandom random) {
      var best = GetBestSolution(realVectors, qualities);

      DoubleValue bestKnownQuality = BestKnownQualityParameter.Value;
      RealVector bestKnownSolution = null;
      var bestKnownUpdate = bestKnownQuality == null || IsBetter(best.Item2, bestKnownQuality.Value);
      if (bestKnownUpdate) {
        if (bestKnownQuality != null) bestKnownQuality.Value = best.Item2;
        else BestKnownQualityParameter.Value = bestKnownQuality = new DoubleValue(best.Item2);
        BestKnownSolutionParameter.Value = bestKnownSolution = (RealVector)best.Item1.Clone();
      }

      SingleObjectiveTestFunctionSolution solution = null;
      if (results.TryGetValue("Best Solution", out var res)) {
        solution = (SingleObjectiveTestFunctionSolution)res.Value;
        if (IsBetter(best.Item2, solution.BestQuality.Value)) {
          solution.BestRealVector = (RealVector)best.Item1.Clone();
          solution.BestQuality = new DoubleValue(best.Item2);
        }
      } else {
        solution = new SingleObjectiveTestFunctionSolution((RealVector)best.Item1.Clone(),
                                                           new DoubleValue(best.Item2),
                                                           TestFunctionParameter.Value) {
          BestKnownRealVector = bestKnownSolution,
          Bounds = BoundsRefParameter.Value
        };
        results.Add(new Result("Best Solution", solution));
      }
      if (best.Item1.Length == 2) solution.Population = new ItemArray<RealVector>(realVectors.Select(x => (RealVector)x.Clone()));
      if (bestKnownUpdate) solution.BestKnownRealVector = bestKnownSolution;
    }

    #region Events
    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Parameterize();
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
      Parameterize();
    }
    protected override void DimensionOnChanged() {
      base.DimensionOnChanged();
      if (Dimension < TestFunction.MinimumProblemSize || Dimension > TestFunction.MaximumProblemSize)
        Dimension = Math.Min(TestFunction.MaximumProblemSize, Math.Max(TestFunction.MinimumProblemSize, Dimension));
    }
    protected override void BoundsOnChanged() {
      base.BoundsOnChanged();
      Parameterize();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      Parameterize();
    }
    private void TestFunctionParameterOnValueChanged(object sender, EventArgs eventArgs) {
      var problemSizeChange = Dimension < TestFunction.MinimumProblemSize
                              || Dimension > TestFunction.MaximumProblemSize;
      if (problemSizeChange) {
        Dimension = Math.Max(TestFunction.MinimumProblemSize, Math.Min(Dimension, TestFunction.MaximumProblemSize));
      }
      BestKnownQuality = TestFunction.BestKnownQuality;
      Bounds = (DoubleMatrix)TestFunction.Bounds.Clone();
      var bestSolution = TestFunction.GetBestKnownSolution(Dimension);
      BestKnownSolutionParameter.Value = bestSolution;
      Maximization = TestFunction.Maximization;

      OnReset();
    }
    #endregion

    #region Helpers
    private void InitializeOperators() {
      Operators.Add(new SingleObjectiveTestFunctionImprovementOperator());
      Operators.Add(new SingleObjectiveTestFunctionPathRelinker());
      Operators.Add(new SingleObjectiveTestFunctionSimilarityCalculator());
      Operators.Add(new EuclideanSimilarityCalculator());
      Operators.Add(new AdditiveMoveEvaluator());

      Parameterize();
    }

    private void Parameterize() {
      var operators = new List<IItem>();

      //TODO correct wiring code, because most of the parameters are wired in the encoding
      foreach (var op in Operators.OfType<PopulationSimilarityAnalyzer>()) {
        var calcs = Operators.OfType<ISolutionSimilarityCalculator>().ToArray();
        op.SimilarityCalculatorParameter.ValidValues.Clear();
        foreach (var c in calcs) {
          // TODO: unified encoding parameters
          c.SolutionVariableName = ((IRealVectorSolutionOperator)Encoding.SolutionCreator).RealVectorParameter.ActualName;
          c.QualityVariableName = Evaluator.QualityParameter.ActualName;
          op.SimilarityCalculatorParameter.ValidValues.Add(c);
        }
      }
      foreach (var op in Operators.OfType<ISingleObjectiveTestFunctionAdditiveMoveEvaluator>()) {
        operators.Add(op);
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.QualityParameter.Hidden = true;
        foreach (var movOp in Encoding.Operators.OfType<IRealVectorAdditiveMoveQualityOperator>())
          movOp.MoveQualityParameter.ActualName = op.MoveQualityParameter.ActualName;
      }
      foreach (var op in Operators.OfType<IRealVectorParticleCreator>()) {
        // TODO: unified encoding parameters
        op.RealVectorParameter.ActualName = ((IRealVectorSolutionOperator)Encoding.SolutionCreator).RealVectorParameter.ActualName;
        op.RealVectorParameter.Hidden = true;
        op.BoundsParameter.ActualName = BoundsRefParameter.Name;
        op.BoundsParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IRealVectorParticleUpdater>()) {
        // TODO: unified encoding parameters
        op.RealVectorParameter.ActualName = ((IRealVectorSolutionOperator)Encoding.SolutionCreator).RealVectorParameter.ActualName;
        op.RealVectorParameter.Hidden = true;
        op.BoundsParameter.ActualName = BoundsRefParameter.Name;
        op.BoundsParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IRealVectorSwarmUpdater>()) {
        op.MaximizationParameter.ActualName = MaximizationParameter.Name;
        op.MaximizationParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<ISingleObjectiveImprovementOperator>()) {
        operators.Add(op);
        op.SolutionParameter.ActualName = Encoding.Name;
        op.SolutionParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<ITestFunctionSolutionSimilarityCalculator>()) {
        operators.Add(op);
        op.SolutionVariableName = Encoding.Name;
        op.QualityVariableName = Evaluator.QualityParameter.ActualName;
        op.Bounds = Bounds;
      }

      if (operators.Count > 0) Encoding.ConfigureOperators(operators);
    }
    #endregion

    public void Load(SOTFData data) {
      Name = data.Name;
      Description = data.Description;
      TestFunction = data.TestFunction;
    }
  }
}
