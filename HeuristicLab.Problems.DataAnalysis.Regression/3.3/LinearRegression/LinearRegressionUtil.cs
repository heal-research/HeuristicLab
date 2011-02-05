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

namespace HeuristicLab.Problems.DataAnalysis.Regression.LinearRegression {
  public static class LinearRegressionUtil {
    public static double[,] PrepareInputMatrix(Dataset dataset, string targetVariable, IEnumerable<string> allowedInputVariables, int start, int end) {
      List<int> allowedRows = CalculateAllowedRows(dataset, targetVariable, allowedInputVariables, start, end);

      double[,] matrix = new double[allowedRows.Count, allowedInputVariables.Count() + 1];
      for (int row = 0; row < allowedRows.Count; row++) {
        int col = 0;
        foreach (string column in allowedInputVariables) {
          matrix[row, col] = dataset[column, row];
          col++;
        }
        matrix[row, allowedInputVariables.Count()] = dataset[targetVariable, row];
      }
      return matrix;
    }

    private static List<int> CalculateAllowedRows(Dataset dataset, string targetVariable, IEnumerable<string> allowedInputVariables, int start, int end) {
      List<int> allowedRows = new List<int>();
      bool add = false;

      for (int row = start; row < end; row++) {
        add = true;
        foreach (string column in allowedInputVariables) {
          double value = dataset[column, row];
          if (double.IsInfinity(value) ||
            double.IsNaN(value))
            add = false;
        }
        if (double.IsNaN(dataset[targetVariable, row]))
          add = false;
        if (add)
          allowedRows.Add(row);
        add = true;
      }
      return allowedRows;
    }
  }
}
