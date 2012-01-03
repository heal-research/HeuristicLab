#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      List<string> variablesList = variables.ToList();
      List<int> rowsList = rows.ToList();

      double[,] matrix = new double[rowsList.Count, variablesList.Count];

      int col = 0;
      foreach (string column in variables) {
        var values = dataset.GetDoubleValues(column, rows);
        int row = 0;
        foreach (var value in values) {
          matrix[row, col] = value;
          row++;
        }
        col++;
      }

      return matrix;
    }
  }
}
