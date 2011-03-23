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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public static class AlglibUtil {
    public static double[,] PrepareInputMatrix(Dataset dataset, IEnumerable<string> variables, IEnumerable<int> rows) {
      List<int> allowedRows = CalculateAllowedRows(dataset, variables, rows).ToList();

      double[,] matrix = new double[allowedRows.Count, variables.Count()];
      for (int row = 0; row < allowedRows.Count; row++) {
        int col = 0;
        foreach (string column in variables) {
          matrix[row, col] = dataset[column, row];
          col++;
        }
      }
      return matrix;
    }

    private static IEnumerable<int> CalculateAllowedRows(Dataset dataset, IEnumerable<string> variables, IEnumerable<int> rows) {
      // return only rows that contain no infinity or NaN values
      return from row in rows
             where (from variable in variables
                    let x = dataset[variable, row]
                    where double.IsInfinity(x) || double.IsNaN(x)
                    select 1)
                    .Any() == false
             select row;
    }
  }
}
