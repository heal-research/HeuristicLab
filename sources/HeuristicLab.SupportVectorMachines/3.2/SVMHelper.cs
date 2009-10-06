using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.SupportVectorMachines {
  public class SVMHelper {

    public static SVM.Problem CreateSVMProblem(Dataset dataset, int targetVariable, int start, int end, int minTimeOffset, int maxTimeOffset) {
      return CreateSVMProblem(dataset, targetVariable, Enumerable.Range(0, dataset.Columns).ToDictionary<int, int>(x => x), start, end, minTimeOffset, maxTimeOffset);
    }

    public static SVM.Problem CreateSVMProblem(Dataset dataset, int targetVariable, Dictionary<int, int> columnMapping, int start, int end, int minTimeOffset, int maxTimeOffset) {
      int rowCount = end - start;
      List<int> skippedFeatures = new List<int>();
      for (int i = 0; i < dataset.Columns; i++) {
        if (i != targetVariable) {
          if (dataset.GetRange(i, start, end) == 0)
            skippedFeatures.Add(i);
        }
      }

      int maxColumns = 0;

      double[] targetVector = new double[rowCount];
      for (int i = 0; i < rowCount; i++) {
        double value = dataset.GetValue(start + i, targetVariable);
        targetVector[i] = value;
      }
      targetVector = targetVector.Where(x => !double.IsNaN(x)).ToArray();

      SVM.Node[][] nodes = new SVM.Node[targetVector.Length][];
      List<SVM.Node> tempRow;
      int addedRows = 0;
      int timeOffsetBase = columnMapping.Count;
      for (int row = 0; row < rowCount; row++) {
        tempRow = new List<SVM.Node>();
        for (int col = 0; col < dataset.Columns; col++) {
          if (!skippedFeatures.Contains(col) && col != targetVariable && columnMapping.ContainsKey(col)) {
            for (int timeOffset = minTimeOffset; timeOffset <= maxTimeOffset; timeOffset++) {
              int actualColumn = columnMapping[col] * (maxTimeOffset - minTimeOffset + 1) + (timeOffset - minTimeOffset);
              double value = dataset.GetValue(start + row + timeOffset, col);
              if (!double.IsNaN(value)) {
                tempRow.Add(new SVM.Node(actualColumn, value));
                if (actualColumn > maxColumns) maxColumns = actualColumn;
              }
            }
          }
        }
        if (!double.IsNaN(dataset.GetValue(start + row, targetVariable))) {
          nodes[addedRows] = tempRow.ToArray();
          addedRows++;
        }
      }

      return new SVM.Problem(targetVector.Length, targetVector, nodes, maxColumns);
    }
  }
}
