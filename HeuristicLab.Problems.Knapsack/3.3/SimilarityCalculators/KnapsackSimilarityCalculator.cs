#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization.Operators;

namespace HeuristicLab.Problems.Knapsack {
  /// <summary>
  /// An operator that performs similarity calculation between two knapsack solutions.
  /// </summary>
  /// <remarks>
  /// The operator calculates the similarity based on the number of elements the two solutions have in common.
  /// </remarks>
  [Item("KnapsackSimilarityCalculator", "An operator that performs similarity calculation between two knapsack solutions. The operator calculates the similarity based on the number of elements the two solutions have in common.")]
  public sealed class KnapsackSimilarityCalculator : SingleObjectiveSolutionSimilarityCalculator {
    private KnapsackSimilarityCalculator(bool deserializing) : base(deserializing) { }
    private KnapsackSimilarityCalculator(KnapsackSimilarityCalculator original, Cloner cloner) : base(original, cloner) { }
    public KnapsackSimilarityCalculator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackSimilarityCalculator(this, cloner);
    }

    public static double CalculateSimilarity(BinaryVector left, BinaryVector right) {
      if (left == null || right == null)
        throw new ArgumentException("Cannot calculate similarity because one or both of the provided scopes is null.");
      if (left.Length != right.Length)
        throw new ArgumentException("Cannot calculate similarity because the provided solutions have different lengths.");
      if (left == right) return 1.0;

      double similarity = 0.0;
      for (int i = 0; i < left.Length; i++)
        if (left[i] == right[i]) similarity++;
      return similarity / left.Length;
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      var sol1 = leftSolution.Variables[SolutionVariableName].Value as BinaryVector;
      var sol2 = rightSolution.Variables[SolutionVariableName].Value as BinaryVector;

      return CalculateSimilarity(sol1, sol2);
    }
  }
}
