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

namespace HeuristicLab.Problems.DataAnalysis {
  public static class DatasetExtensions {
    public static double[,] ToArray(this IDataset dataset, IEnumerable<string> variables, IEnumerable<int> rows) {
      return ToArray(dataset,
        variables,
        transformations: variables.Select(_ => (ITransformation<double>)null),  // no transform
        rows: rows);
    }
    public static double[,] ToArray(this IDataset dataset, IEnumerable<string> variables, IEnumerable<ITransformation<double>> transformations, IEnumerable<int> rows) {
      string[] variablesArr = variables.ToArray();
      int[] rowsArr = rows.ToArray();
      ITransformation<double>[] transformArr = transformations.ToArray();
      if (transformArr.Length != variablesArr.Length)
        throw new ArgumentException("Number of variables and number of transformations must match.");

      double[,] matrix = new double[rowsArr.Length, variablesArr.Length];

      for (int i = 0; i < variablesArr.Length; i++) {
        var origValues = dataset.GetDoubleValues(variablesArr[i], rowsArr);
        var values = transformArr[i] != null ? transformArr[i].Apply(origValues) : origValues;
        int row = 0;
        foreach (var value in values) {
          matrix[row, i] = value;
          row++;
        }
      }

      return matrix;
    }
  }
}
