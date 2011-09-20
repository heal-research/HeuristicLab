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
      return PrepareInputMatrix(dataset, variables, rows, new int[] { 0 });
    }

    public static double[,] PrepareInputMatrix(Dataset dataset, IEnumerable<string> variables, IEnumerable<int> rows, IEnumerable<int> lags) {
      int maxLag = lags.Max();

      // drop last variable (target variable)
      List<string> inputVariablesList = variables
        .Reverse()
        .Skip(1)
        .Reverse()
        .ToList();
      string targetVariable = variables.Last();
      List<int> rowsList = rows.ToList();
      int nRows = rowsList.Count - maxLag;
      double[,] matrix = new double[nRows, inputVariablesList.Count * lags.Count() + 1];

      int col = 0;
      int row = 0;
      // input variables
      foreach (int lag in lags) {
        foreach (string column in inputVariablesList) {
          var values = dataset.GetDoubleValues(column, rows.Select(x => x - lag).Take(nRows));
          row = 0;
          foreach (var value in values) {
            if (row >= 0) {
              matrix[row, col] = value;
            }
            row++;
          }
          col++;
        }
      }
      // target variable
      row = 0;
      foreach (var value in dataset.GetDoubleValues(targetVariable, rows).Take(nRows)) {
        matrix[row, col] = value;
        row++;
      }
      return matrix;
    }
  }
}
