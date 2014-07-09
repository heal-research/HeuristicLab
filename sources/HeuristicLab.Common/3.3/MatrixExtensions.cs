#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Common {
  public static class MatrixExtensions {
    /// <summary>
    /// Returns a transposed matrix of the given <paramref name="matrix"/>.
    /// </summary>
    /// <typeparam name="T">The type of the matrix</typeparam>
    /// <param name="matrix">The matrix that should be transposed.</param>
    /// <returns>The transposed matrix.</returns>
    public static T[,] Transpose<T>(this T[,] matrix) {
      var result = new T[matrix.GetLength(1), matrix.GetLength(0)];
      for (int i = 0; i < matrix.GetLength(0); i++)
        for (int j = 0; j < matrix.GetLength(1); j++)
          result[j, i] = matrix[i, j];
      return result;
    }
  }
}
