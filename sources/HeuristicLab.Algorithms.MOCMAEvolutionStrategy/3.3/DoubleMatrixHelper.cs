#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Data;

namespace HeuristicLab.Algorithms.MOCMAEvolutionStrategy {
  internal static class DoubleMatrixHelper {
    internal static double[][] ToJaggedArray(this DoubleMatrix m) {
      if (m == null) return null;
      var i = m.Rows - 1;
      var a = new double[i][];
      for (i--; i >= 0; i--) {
        var j = m.Columns;
        a[i] = new double[j];
        for (j--; j >= 0; j--) a[i][j] = m[i, j];
      }
      return a;
    }

    internal static DoubleMatrix ToMatrix(this IEnumerable<IReadOnlyList<double>> data) {
      var d2 = data.ToArray();
      var mat = new DoubleMatrix(d2.Length, d2[0].Count);
      for (var i = 0; i < mat.Rows; i++)
        for (var j = 0; j < mat.Columns; j++)
          mat[i, j] = d2[i][j];
      return mat;
    }
  }
}
