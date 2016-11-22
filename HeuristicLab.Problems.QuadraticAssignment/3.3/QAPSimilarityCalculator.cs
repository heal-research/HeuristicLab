#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization.Operators;

namespace HeuristicLab.Problems.QuadraticAssignment {
  /// <summary>
  /// An operator that performs similarity calculation between two quadratic assignment solutions.
  /// </summary>
  /// <remarks>
  /// The operator calculates the similarity based on the number of edges the two solutions have in common.
  /// </remarks>
  [Item("QAPSimilarityCalculator", "An operator that performs similarity calculation between two quadratic assignment solutions. The operator calculates the similarity based on the number of edges the two solutions have in common.")]
  public sealed class QAPSimilarityCalculator : SingleObjectiveSolutionSimilarityCalculator {
    protected override bool IsCommutative { get { return true; } }

    private QAPSimilarityCalculator(bool deserializing) : base(deserializing) { }
    private QAPSimilarityCalculator(QAPSimilarityCalculator original, Cloner cloner) : base(original, cloner) { }
    public QAPSimilarityCalculator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPSimilarityCalculator(this, cloner);
    }

    public static double CalculateSimilarity(Permutation left, Permutation right) {
      if (left == null || right == null)
        throw new ArgumentException("Cannot calculate similarity because one of the provided solutions or both are null.");
      if (left.Length != right.Length)
        throw new ArgumentException("Cannot calculate similarity because the provided solutions have different lengths.");
      if (object.ReferenceEquals(left, right)) return 1.0;

      return QAPPermutationProximityCalculator.CalculateGenotypeSimilarity(left, right);
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      var sol1 = leftSolution.Variables[SolutionVariableName].Value as Permutation;
      var sol2 = rightSolution.Variables[SolutionVariableName].Value as Permutation;

      return CalculateSimilarity(sol1, sol2);
    }
  }
}
