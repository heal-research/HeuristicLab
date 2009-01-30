using System;
using System.Collections.Generic;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization.LabelProvider {
  public class StringLabelProvider : ILabelProvider {
    private readonly Dictionary<int, string> labels = new Dictionary<int, string>();

    public void ClearLabels() {
      labels.Clear();
    }

    public void SetLabel(int index, string label) {
      labels[index] = label;
    }

    public string GetLabel(double value) {
      int index = (int)Math.Round(value);
      double delta = Math.Abs(index - value);

      string label;

      if (delta < 1e-10 && labels.TryGetValue(index, out label))
        return label;
      else
        return string.Empty;
    }
  }
}