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

using System.Diagnostics.Contracts;

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

    /// <summary>
    /// Concatenates matrices vertically.
    /// A      B
    /// 1 2    9 8
    /// 3 4    7 6
    /// 
    /// VertCat(A, B)
    /// 1 2 
    /// 3 4
    /// 9 8
    /// 7 6
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>A new matrix with the number of rows = a.GetLength(0) + b.GetLength(0)</returns>
    public static T[,] VertCat<T>(this T[,] a, T[,] b) {
      Contract.Assert(a.GetLength(1) == b.GetLength(1));
      var aLen = a.GetLength(0);
      var bLen = b.GetLength(0);
      var result = new T[aLen + bLen, a.GetLength(1)];
      for (int i = 0; i < aLen; i++)
        for (int j = 0; j < a.GetLength(1); j++)
          result[i, j] = a[i, j];
      for (int i = 0; i < bLen; i++)
        for (int j = 0; j < b.GetLength(1); j++)
          result[i + aLen, j] = b[i, j];

      return result;
    }

    /// <summary>
    /// Concatenates matrices horizontally.
    /// A      B
    /// 1 2    9 8
    /// 3 4    7 6
    /// 
    /// HorzCat(A, B)
    /// 1 2 9 8
    /// 3 4 7 6
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>A new matrix with the number of columns = a.GetLength(1) + b.GetLength(1)</returns>
    public static T[,] HorzCat<T>(this T[,] a, T[,] b) {
      Contract.Assert(a.GetLength(0) == b.GetLength(0));
      var aLen = a.GetLength(1);
      var bLen = b.GetLength(1);
      var result = new T[a.GetLength(0), aLen + bLen];
      for (int i = 0; i < a.GetLength(0); i++)
        for (int j = 0; j < aLen; j++)
          result[i, j] = a[i, j];
      for (int i = 0; i < a.GetLength(0); i++)
        for (int j = 0; j < bLen; j++)
          result[i, j + aLen] = b[i, j];
      return result;
    }
  }
}
