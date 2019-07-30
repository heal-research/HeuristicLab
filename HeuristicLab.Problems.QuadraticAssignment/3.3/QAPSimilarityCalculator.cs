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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization.Operators;
using HEAL.Attic;

namespace HeuristicLab.Problems.QuadraticAssignment {
  /// <summary>
  /// An operator that performs similarity calculation between two quadratic assignment solutions.
  /// </summary>
  /// <remarks>
  /// The operator calculates the similarity based on the number of edges the two solutions have in common.
  /// </remarks>
  [Item("QAPSimilarityCalculator", "An operator that performs similarity calculation between two quadratic assignment solutions. The operator calculates the similarity based on the number of edges the two solutions have in common.")]
  [StorableType("23D76028-3E59-4E77-959A-D4A1BFB59864")]
  public sealed class QAPSimilarityCalculator : SingleObjectiveSolutionSimilarityCalculator {
    protected override bool IsCommutative { get { return true; } }

    [Storable]
    public DoubleMatrix Weights { get; set; }
    [Storable]
    public DoubleMatrix Distances { get; set; }

    [StorableConstructor]
    private QAPSimilarityCalculator(StorableConstructorFlag _) : base(_) { }
    private QAPSimilarityCalculator(QAPSimilarityCalculator original, Cloner cloner)
      : base(original, cloner) {
      Weights = cloner.Clone(original.Weights);
      Distances = cloner.Clone(original.Distances);
    }
    public QAPSimilarityCalculator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPSimilarityCalculator(this, cloner);
    }

    public static double CalculateSimilarity(Permutation left, Permutation right, DoubleMatrix weights, DoubleMatrix distances) {
      if (left == null || right == null)
        throw new ArgumentException("Cannot calculate similarity because one of the provided solutions or both are null.");
      if (left.Length != right.Length)
        throw new ArgumentException("Cannot calculate similarity because the provided solutions have different lengths.");
      if (left.Length == 0)
        throw new ArgumentException("Cannot calculate similarity because solutions are of length 0.");
      if (ReferenceEquals(left, right)) return 1.0;

      return QAPPermutationProximityCalculator.CalculatePhenotypeSimilarity(left, right, weights, distances);
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      var sol1 = leftSolution.Variables[SolutionVariableName].Value as Permutation;
      var sol2 = rightSolution.Variables[SolutionVariableName].Value as Permutation;

      return CalculateSimilarity(sol1, sol2, Weights, Distances);
    }
  }
}
