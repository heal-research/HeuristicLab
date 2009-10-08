using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Common;

namespace HeuristicLab.SupportVectorMachines {
  public class SVMHelper {
    public static SVM.Problem CreateSVMProblem(Dataset dataset, int targetVariableIndex, IEnumerable<string> inputVariables, int start, int end, int minTimeOffset, int maxTimeOffset) {
      int rowCount = end - start;

      var targetVector = (from row in Enumerable.Range(start, rowCount)
                          let val = dataset.GetValue(row, targetVariableIndex)
                          where !double.IsNaN(val)
                          select val).ToArray();


      SVM.Node[][] nodes = new SVM.Node[targetVector.Length][];
      List<SVM.Node> tempRow;
      int addedRows = 0;
      int maxColumns = 0;
      for (int row = 0; row < rowCount; row++) {
        tempRow = new List<SVM.Node>();
        int nodeIndex = 0;
        foreach (var inputVariable in inputVariables) {
          ++nodeIndex;
          int col = dataset.GetVariableIndex(inputVariable);
          if (IsUsefulColumn(dataset, col, start, end)) {
            for (int timeOffset = minTimeOffset; timeOffset <= maxTimeOffset; timeOffset++) {
              int actualColumn = nodeIndex * (maxTimeOffset - minTimeOffset + 1) + (timeOffset - minTimeOffset);
              if (start + row + timeOffset >= 0 && start + row + timeOffset < dataset.Rows) {
                double value = dataset.GetValue(start + row + timeOffset, col);
                if (!double.IsNaN(value)) {
                  tempRow.Add(new SVM.Node(actualColumn, value));
                  if (actualColumn > maxColumns) maxColumns = actualColumn;
                }
              }
            }
          }
        }
        if (!double.IsNaN(dataset.GetValue(start + row, targetVariableIndex))) {
          nodes[addedRows] = tempRow.ToArray();
          addedRows++;
        }
      }

      return new SVM.Problem(targetVector.Length, targetVector, nodes, maxColumns);
    }

    // checks if the column has at least two different non-NaN and non-Infinity values
    private static bool IsUsefulColumn(Dataset dataset, int col, int start, int end) {
      double min = double.PositiveInfinity;
      double max = double.NegativeInfinity;
      for (int i = start; i < end; i++) {
        double x = dataset.GetValue(i, col);
        if (!double.IsNaN(x) && !double.IsInfinity(x)) {
          min = Math.Min(min, x);
          max = Math.Max(max, x);
        }
        if (min != max) return true;
      }
      return false;
    }
  }
}
