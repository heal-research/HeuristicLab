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

namespace HeuristicLab.Problems.DataAnalysis.SupportVectorMachine {
  public class SupportVectorMachineUtil {
    /// <summary>
    /// Transforms <paramref name="problemData"/> into a data structure as needed by libSVM.
    /// </summary>
    /// <param name="problemData">The problem data to transform</param>
    /// <param name="rowIndices">The rows of the dataset that should be contained in the resulting SVM-problem</param>
    /// <returns>A problem data type that can be used to train a support vector machine.</returns>
    public static SVM.Problem CreateSvmProblem(DataAnalysisProblemData problemData, IEnumerable<int> rowIndices) {
      double[] targetVector =
        problemData.Dataset.GetEnumeratedVariableValues(problemData.TargetVariable.Value, rowIndices)
        .ToArray();

      SVM.Node[][] nodes = new SVM.Node[targetVector.Length][];
      List<SVM.Node> tempRow;
      int maxNodeIndex = 0;
      int svmProblemRowIndex = 0;
      foreach (int row in rowIndices) {
        tempRow = new List<SVM.Node>();
        foreach (var inputVariable in problemData.InputVariables.CheckedItems) {
          int col = problemData.Dataset.GetVariableIndex(inputVariable.Value.Value);
          double value = problemData.Dataset[row, col];
          if (!double.IsNaN(value)) {
            int nodeIndex = col + 1; // make sure the smallest nodeIndex is 1 (libSVM convention)
            tempRow.Add(new SVM.Node(nodeIndex, value));
            if (nodeIndex > maxNodeIndex) maxNodeIndex = nodeIndex;
          }
        }
        nodes[svmProblemRowIndex++] = tempRow.OrderBy(x => x.Index).ToArray(); // make sure the values are sorted by node index
      }

      return new SVM.Problem(targetVector.Length, targetVector, nodes, maxNodeIndex);
    }
  }
}
