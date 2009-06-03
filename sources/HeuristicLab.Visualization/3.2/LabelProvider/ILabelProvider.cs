using HeuristicLab.Core;

namespace HeuristicLab.Visualization.LabelProvider {
  public interface ILabelProvider : IStorable {
    string GetLabel(double value);
  }
}