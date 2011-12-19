#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis {
  public static class MatrixExtensions<T> {
    public static T[,] Create(T[] first, T[] second) {
      if (first.Length != second.Length)
        throw new ArgumentException("Both arrays must be of same length.");
      T[,] array = new T[first.Length, 2];
      for (int i = 0; i < first.Length; i++) {
        array[i, 0] = first[i];
        array[i, 1] = second[i];
      }
      return array;
    }

    public static T[] GetColumn(T[,] matrix, int column) {
      if (column >= matrix.GetLength(1)) throw new ArgumentException("Invalid column index: " + column + ". Matrix has only " + matrix.GetLength(1) + " columns.");
      T[] result = new T[matrix.GetLength(0)];
      for (int i = 0; i < result.Length; i++) {
        result[i] = matrix[i, column];
      }
      return result;
    }
  }
}
