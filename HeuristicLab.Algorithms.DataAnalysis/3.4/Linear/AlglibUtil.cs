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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public static class AlglibUtil {
    public static double[,] PrepareInputMatrix(IDataset dataset, IEnumerable<string> variables, IEnumerable<int> rows) {
      // check input variables. Only double variables are allowed.
      var invalidInputs =
        variables.Where(name => !dataset.VariableHasType<double>(name));
      if (invalidInputs.Any())
        throw new NotSupportedException("Unsupported inputs: " + string.Join(", ", invalidInputs));

      List<int> rowsList = rows.ToList();
      double[,] matrix = new double[rowsList.Count, variables.Count()];

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

    public static double[,] PrepareAndScaleInputMatrix(IDataset dataset, IEnumerable<string> variables, IEnumerable<int> rows, Scaling scaling) {
      // check input variables. Only double variables are allowed.
      var invalidInputs =
        variables.Where(name => !dataset.VariableHasType<double>(name));
      if (invalidInputs.Any())
        throw new NotSupportedException("Unsupported inputs: " + string.Join(", ", invalidInputs));

      List<string> variablesList = variables.ToList();
      List<int> rowsList = rows.ToList();

      double[,] matrix = new double[rowsList.Count, variablesList.Count];

      int col = 0;
      foreach (string column in variables) {
        var values = scaling.GetScaledValues(dataset, column, rows);
        int row = 0;
        foreach (var value in values) {
          matrix[row, col] = value;
          row++;
        }
        col++;
      }

      return matrix;
    }

    /// <summary>
    /// Prepares a binary data matrix from a number of factors and specified factor values
    /// </summary>
    /// <param name="dataset">A dataset that contains the variable values</param>
    /// <param name="factorVariables">An enumerable of categorical variables (factors). For each variable an enumerable of values must be specified.</param>
    /// <param name="rows">An enumerable of row indices for the dataset</param>
    /// <returns></returns>
    /// <remarks>Factor variables (categorical variables) are split up into multiple binary variables one for each specified value.</remarks>
    public static double[,] PrepareInputMatrix(
      IDataset dataset,
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> factorVariables,
      IEnumerable<int> rows) {
      // check input variables. Only string variables are allowed.
      var invalidInputs =
        factorVariables.Select(kvp => kvp.Key).Where(name => !dataset.VariableHasType<string>(name));
      if (invalidInputs.Any())
        throw new NotSupportedException("Unsupported inputs: " + string.Join(", ", invalidInputs));

      int numBinaryColumns = factorVariables.Sum(kvp => kvp.Value.Count());

      List<int> rowsList = rows.ToList();
      double[,] matrix = new double[rowsList.Count, numBinaryColumns];

      int col = 0;
      foreach (var kvp in factorVariables) {
        var varName = kvp.Key;
        var cats = kvp.Value;
        if (!cats.Any()) continue;
        foreach (var cat in cats) {
          var values = dataset.GetStringValues(varName, rows);
          int row = 0;
          foreach (var value in values) {
            matrix[row, col] = value == cat ? 1 : 0;
            row++;
          }
          col++;
        }
      }
      return matrix;
    }

    public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetFactorVariableValues(IDataset ds, IEnumerable<string> factorVariables, IEnumerable<int> rows) {
      return from factor in factorVariables
             let distinctValues = ds.GetStringValues(factor, rows).Distinct().ToArray()
             // 1 distinct value => skip (constant)
             // 2 distinct values => only take one of the two values
             // >=3 distinct values => create a binary value for each value
             let reducedValues = distinctValues.Length <= 2
               ? distinctValues.Take(distinctValues.Length - 1)
               : distinctValues
             select new KeyValuePair<string, IEnumerable<string>>(factor, reducedValues);
    }
  }
}
