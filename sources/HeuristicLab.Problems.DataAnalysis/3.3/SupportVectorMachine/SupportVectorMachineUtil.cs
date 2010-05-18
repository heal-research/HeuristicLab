#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis.SupportVectorMachine {
  public class SupportVectorMachineUtil {
    /// <summary>
    /// Transforms <paramref name="problemData"/> into a data structure as needed by libSVM.
    /// </summary>
    /// <param name="problemData">The problem data to transform</param>
    /// <param name="start">The index of the first row of <paramref name="problemData"/> to copy to the output.</param>
    /// <param name="end">The last of the first row of <paramref name="problemData"/> to copy to the output.</param>
    /// <returns>A problem data type that can be used to train a support vector machine.</returns>
    public static SVM.Problem CreateSvmProblem(DataAnalysisProblemData problemData, int start, int end) {
      int rowCount = end - start;
      var targetVector = problemData.Dataset.GetVariableValues(problemData.TargetVariable.Value, start, end);

      SVM.Node[][] nodes = new SVM.Node[targetVector.Length][];
      List<SVM.Node> tempRow;
      int maxNodeIndex = 0;
      for (int row = 0; row < rowCount; row++) {
        tempRow = new List<SVM.Node>();
        foreach (var inputVariable in problemData.InputVariables) {
          int col = problemData.Dataset.GetVariableIndex(inputVariable.Value);
          double value = problemData.Dataset[start + row, col];
          if (!double.IsNaN(value)) {
            int nodeIndex = col + 1; // make sure the smallest nodeIndex = 1
            tempRow.Add(new SVM.Node(nodeIndex, value));
            if (nodeIndex > maxNodeIndex) maxNodeIndex = nodeIndex;
          }
        }
        nodes[row] = tempRow.OrderBy(x => x.Index).ToArray(); // make sure the values are sorted by node index
      }

      return new SVM.Problem(targetVector.Length, targetVector, nodes, maxNodeIndex);
    }
  }
}
