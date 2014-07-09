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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Problems.DataAnalysis;
using LibSVM;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public class SupportVectorMachineUtil {
    /// <summary>
    /// Transforms <paramref name="problemData"/> into a data structure as needed by libSVM.
    /// </summary>
    /// <param name="problemData">The problem data to transform</param>
    /// <param name="rowIndices">The rows of the dataset that should be contained in the resulting SVM-problem</param>
    /// <returns>A problem data type that can be used to train a support vector machine.</returns>
    public static svm_problem CreateSvmProblem(Dataset dataset, string targetVariable, IEnumerable<string> inputVariables, IEnumerable<int> rowIndices) {
      double[] targetVector =
        dataset.GetDoubleValues(targetVariable, rowIndices).ToArray();

      svm_node[][] nodes = new svm_node[targetVector.Length][];
      List<svm_node> tempRow;
      int maxNodeIndex = 0;
      int svmProblemRowIndex = 0;
      List<string> inputVariablesList = inputVariables.ToList();
      foreach (int row in rowIndices) {
        tempRow = new List<svm_node>();
        int colIndex = 1; // make sure the smallest node index for SVM = 1
        foreach (var inputVariable in inputVariablesList) {
          double value = dataset.GetDoubleValue(inputVariable, row);
          // SVM also works with missing values
          // => don't add NaN values in the dataset to the sparse SVM matrix representation
          if (!double.IsNaN(value)) {
            tempRow.Add(new svm_node() { index = colIndex, value = value }); // nodes must be sorted in ascending ordered by column index
            if (colIndex > maxNodeIndex) maxNodeIndex = colIndex;
          }
          colIndex++;
        }
        nodes[svmProblemRowIndex++] = tempRow.ToArray();
      }

      return new svm_problem() { l = targetVector.Length, y = targetVector, x = nodes };
    }
  }
}
