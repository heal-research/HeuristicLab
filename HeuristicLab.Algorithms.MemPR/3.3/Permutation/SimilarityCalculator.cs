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
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Algorithms.MemPR.Permutation {
  public static class PermutationSimilarityCalculator {

    public static double CalculateSimilarity(Encodings.PermutationEncoding.Permutation left, Encodings.PermutationEncoding.Permutation right) {
      if (left == null || right == null)
        throw new ArgumentException("Cannot calculate similarity because one of the provided solutions or both are null.");
      if (left.PermutationType != right.PermutationType)
        throw new ArgumentException("Cannot calculate similarity because the provided solutions have different types.");
      if (left.Length != right.Length)
        throw new ArgumentException("Cannot calculate similarity because the provided solutions have different lengths.");
      if (object.ReferenceEquals(left, right)) return 1.0;

      switch (left.PermutationType) {
        case PermutationTypes.Absolute:
          return CalculateAbsolute(left, right);
        case PermutationTypes.RelativeDirected:
          return CalculateRelativeDirected(left, right);
        case PermutationTypes.RelativeUndirected:
          return CalculateRelativeUndirected(left, right);
        default:
          throw new InvalidOperationException("unknown permutation type");
      }
    }

    private static double CalculateAbsolute(Encodings.PermutationEncoding.Permutation left, Encodings.PermutationEncoding.Permutation right) {
      double similarity = 0.0;
      for (int i = 0; i < left.Length; i++)
        if (left[i] == right[i]) similarity++;

      return similarity / left.Length;
    }

    private static double CalculateRelativeDirected(Encodings.PermutationEncoding.Permutation left, Encodings.PermutationEncoding.Permutation right) {
      int[] edgesR = CalculateEdgesVector(right);
      int[] edgesL = CalculateEdgesVector(left);

      double similarity = 0.0;
      for (int i = 0; i < left.Length; i++) {
        if (edgesL[i] == edgesR[i]) similarity++;
      }

      return similarity / left.Length;
    }

    private static double CalculateRelativeUndirected(Encodings.PermutationEncoding.Permutation left, Encodings.PermutationEncoding.Permutation right) {
      int[] edgesR = CalculateEdgesVector(right);
      int[] edgesL = CalculateEdgesVector(left);

      double similarity = 0.0;
      for (int i = 0; i < left.Length; i++) {
        if ((edgesL[i] == edgesR[i]) || (edgesL[edgesR[i]] == i))
          similarity++;
      }

      return similarity / left.Length;
    }

    private static int[] CalculateEdgesVector(Encodings.PermutationEncoding.Permutation permutation) {
      // transform path representation into adjacency representation
      int[] edgesVector = new int[permutation.Length];
      for (int i = 0; i < permutation.Length - 1; i++)
        edgesVector[permutation[i]] = permutation[i + 1];
      edgesVector[permutation[permutation.Length - 1]] = permutation[0];
      return edgesVector;
    }
  }
}
