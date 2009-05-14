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
      
      SVM.Node[][] nodes = new SVM.Node[rowCount][];
      List<SVM.Node> tempRow;
      double value;
      for (int row = 0; row < rowCount; row++) {
        tempRow = new List<SVM.Node>();
        for (int col = 0; col < allowedFeatures.Count; col++) {
          value = samples[(start + row) * dataset.Columns + allowedFeatures[col].Data];
          if(!double.IsNaN(value))
            tempRow.Add(new SVM.Node(allowedFeatures[col].Data, value));
        }
        nodes[row] = tempRow.ToArray();
      }

      double[] targetVector = new double[rowCount];
      for (int i = 0; i < rowCount; i++)
        targetVector[i] = samples[(start + i) * dataset.Columns + targetVariable];

      return new SVM.Problem(rowCount, targetVector, nodes, allowedFeatures.Max(x => x.Data));
    }
  }
}
