using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.SupportVectorMachines {
  public class SVMHelper {
    public static SVM.Problem CreateSVMProblem(Dataset dataset, ItemList<IntData> allowedFeatures, int targetVariable, int start, int end) {
      int rowCount = end - start;
      double[] samples = dataset.Samples;

      List<int> skippedFeatures = new List<int>();
      for (int i = 0; i < allowedFeatures.Count; i++) {
        if (dataset.GetRange(allowedFeatures[i].Data, start, end) == 0)
          skippedFeatures.Add(i);
      }

      double[] targetVector = new double[rowCount];
      for (int i = 0; i < rowCount; i++) {
        double value = samples[(start + i) * dataset.Columns + targetVariable];
          targetVector[i] = value;
      }
      targetVector = targetVector.Where(x=> !double.IsNaN(x)).ToArray();

      SVM.Node[][] nodes = new SVM.Node[targetVector.Length][];
      List<SVM.Node> tempRow;
      int addedRows = 0;
      for (int row = 0; row < rowCount; row++) {
        tempRow = new List<SVM.Node>();
        for (int col = 0; col < allowedFeatures.Count; col++) {
          if (!skippedFeatures.Contains(col)) {
            double value = samples[(start + row) * dataset.Columns + allowedFeatures[col].Data];
            if (!double.IsNaN(value))
              tempRow.Add(new SVM.Node(allowedFeatures[col].Data, value));
          }
        }
        if (!double.IsNaN(samples[(start + row) * dataset.Columns + targetVariable])) {
          nodes[addedRows] = tempRow.ToArray();
          addedRows++;
        }
      }

      return new SVM.Problem(targetVector.Length, targetVector, nodes, allowedFeatures.Max(x => x.Data));
    }
  }
}
