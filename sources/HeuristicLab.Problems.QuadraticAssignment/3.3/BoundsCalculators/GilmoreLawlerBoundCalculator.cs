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
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.LinearAssignment;

namespace HeuristicLab.Problems.QuadraticAssignment {
  public static class GilmoreLawlerBoundCalculator {
    public static double CalculateLowerBound(DoubleMatrix weights, DoubleMatrix distances) {
      Permutation tmp;
      return CalculateLowerBound(weights, distances, out tmp);
    }

    public static double CalculateLowerBound(DoubleMatrix weights, DoubleMatrix distances, out Permutation solution) {
      int N = weights.Rows;
      DoubleMatrix sortedWeights = SortEachRowExceptDiagonal(weights), sortedDistances = SortEachRowExceptDiagonal(distances);

      DoubleMatrix costs = new DoubleMatrix(N, N);
      for (int i = 0; i < N; i++) {
        for (int j = 0; j < N; j++) {
          for (int k = 0; k < N - 1; k++)
            costs[i, j] += sortedWeights[i, k] * sortedDistances[j, N - 2 - k];
          costs[i, j] += sortedWeights[i, N - 1] * sortedDistances[j, N - 1];
        }
      }

      double result;
      solution = new Permutation(PermutationTypes.Absolute, LinearAssignmentProblemSolver.Solve(costs, out result));
      return result;
    }

    private static DoubleMatrix SortEachRowExceptDiagonal(DoubleMatrix matrix) {
      var result = new DoubleMatrix(matrix.Rows, matrix.Columns);

      double[] row = new double[matrix.Rows - 1];
      for (int i = 0; i < matrix.Rows; i++) {
        int counter = 0;
        for (int j = 0; j < matrix.Columns; j++)
          if (i != j) row[counter++] = matrix[i, j];
        Array.Sort(row);
        for (int k = 0; k < matrix.Columns - 1; k++)
          result[i, k] = row[k];
        result[i, matrix.Columns - 1] = matrix[i, i];
      }
      return result;
    }
  }
}
