using System;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization.LabelProvider {
  public class DiscreteLabelProvider : StorableBase, ILabelProvider {
    public string GetLabel(double value) {
      int index = (int)Math.Round(value);
      double delta = Math.Abs(index - value);

      if (delta < 1e-10)
        return index.ToString();
      else
        return string.Empty;
    }
  }
}