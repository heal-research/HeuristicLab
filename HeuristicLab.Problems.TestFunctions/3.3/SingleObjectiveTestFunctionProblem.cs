#region License Information
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
      Dimension = Math.Min(Math.Max(2, TestFunction.MinimumProblemSize), TestFunction.MaximumProblemSize);
      BestKnownSolutionParameter.Value = TestFunction.GetBestKnownSolution(Dimension);


      Operators.AddRange(new IItem[] {
        new SingleObjectiveTestFunctionImprovementOperator(),
        new SingleObjectiveTestFunctionPathRelinker(),
        new SingleObjectiveTestFunctionSimilarityCalculator(),
        new EuclideanSimilarityCalculator(),
        new AdditiveMoveEvaluator() });

      Parameterize();
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
      TestFunctionParameter.ValueChanged += TestFunctionParameterOnValueChanged;
    }

    public override ISingleObjectiveEvaluationResult Evaluate(RealVector individual, IRandom random, CancellationToken cancellationToken) {
      var quality = TestFunction.Evaluate(individual);
      return new SingleObjectiveEvaluationResult(quality);
    }

    //TODO: change to new analyze interface
    public override void Analyze(ISingleObjectiveSolutionContext<RealVector>[] solutionContexts, IRandom random) {
      base.Analyze(solutionContexts, random);

      //TODO: reimplement code below using results directly

      //var best = GetBestSolution(realVectors, qualities);

      //DoubleValue bestKnownQuality = BestKnownQualityParameter.Value;
      //RealVector bestKnownSolution = null;
      //var bestKnownUpdate = bestKnownQuality == null || IsBetter(best.Item2, bestKnownQuality.Value);
      //if (bestKnownUpdate) {
      //  if (bestKnownQuality != null) bestKnownQuality.Value = best.Item2;
      //  else BestKnownQualityParameter.Value = bestKnownQuality = new DoubleValue(best.Item2);
      //  BestKnownSolutionParameter.Value = bestKnownSolution = (RealVector)best.Item1.Clone();
      //}

      //SingleObjectiveTestFunctionSolution solution = null;
      //if (results.TryGetValue("Best Solution", out var res) && res.Value != null) {
      //  solution = (SingleObjectiveTestFunctionSolution)res.Value;
      //  if (IsBetter(best.Item2, solution.BestQuality.Value)) {
      //    solution.BestRealVector = (RealVector)best.Item1.Clone();
      //    solution.BestQuality = new DoubleValue(best.Item2);
      //  }
      //} else {
      //  solution = new SingleObjectiveTestFunctionSolution((RealVector)best.Item1.Clone(),
      //                                                     new DoubleValue(best.Item2),
      //                                                     TestFunctionParameter.Value) {
      //    BestKnownRealVector = bestKnownSolution,
      //    Bounds = BoundsRefParameter.Value
      //  };
      //  results.AddOrUpdateResult("Best Solution", solution);
      //}
      //if (best.Item1.Length == 2) solution.Population = new ItemArray<RealVector>(realVectors.Select(x => (RealVector)x.Clone()));
      //if (bestKnownUpdate) solution.BestKnownRealVector = bestKnownSolution;
    }

    #region Events
    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
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
    private void Parameterize() {
      var operators = new List<IItem>();

      foreach (var op in Operators.OfType<ITestFunctionSolutionSimilarityCalculator>()) {
        operators.Add(op);
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
