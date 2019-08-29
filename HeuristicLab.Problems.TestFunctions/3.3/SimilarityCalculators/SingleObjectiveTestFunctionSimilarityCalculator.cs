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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization.Operators;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// An operator that performs similarity calculation between two test functions solutions.
  /// </summary>
  /// <remarks>
  /// The operator calculates the similarity based on the euclidean distance of the two solutions in n-dimensional space.
  /// </remarks>
  [Item("SingleObjectiveTestFunctionSimilarityCalculator", "An operator that performs similarity calculation between two test functions solutions. The operator calculates the similarity based on the euclidean distance of the two solutions in n-dimensional space.")]
  [StorableType("E6CD365A-5554-42D5-8A5B-6CE3B916A375")]
  public sealed class SingleObjectiveTestFunctionSimilarityCalculator : SingleObjectiveSolutionSimilarityCalculator, ITestFunctionSolutionSimilarityCalculator {
    protected override bool IsCommutative { get { return true; } }

    [Storable]
    public DoubleMatrix Bounds { get; set; }

    [StorableConstructor]
    private SingleObjectiveTestFunctionSimilarityCalculator(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveTestFunctionSimilarityCalculator(SingleObjectiveTestFunctionSimilarityCalculator original, Cloner cloner)
      : base(original, cloner) {
      this.Bounds = cloner.Clone(original.Bounds);
    }
    public SingleObjectiveTestFunctionSimilarityCalculator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveTestFunctionSimilarityCalculator(this, cloner);
    }

    public static double CalculateSimilarity(RealVector left, RealVector right, DoubleMatrix bounds) {
      if (left == null || right == null)
        throw new ArgumentException("Cannot calculate similarity because one of the provided solutions or both are null.");
      if (bounds == null)
        throw new ArgumentException("Cannot calculate similarity because no bounds were provided.");
      if (left.Length != right.Length)
        throw new ArgumentException("Cannot calculate similarity because the provided solutions have different lengths.");
      if (left == right) return 1.0;

      double maxSum = 0.0;
      for (int i = 0; i < left.Length; i++)
        maxSum += Math.Pow(bounds[0, 0] - bounds[0, 1], 2);
      double maxDistance = Math.Sqrt(maxSum) / left.Length;

      double sum = 0.0;
      for (int i = 0; i < left.Length; i++)
        sum += Math.Pow(left[i] - right[i], 2);
      double distance = Math.Sqrt(sum) / left.Length;

      return 1.0 - distance / maxDistance;
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      var sol1 = leftSolution.Variables[SolutionVariableName].Value as RealVector;
      var sol2 = rightSolution.Variables[SolutionVariableName].Value as RealVector;

      return CalculateSimilarity(sol1, sol2, Bounds);
    }
  }
}
