using System.Globalization;

namespace HeuristicLab.Visualization.LabelProvider {
  public class ContinuousLabelProvider : ILabelProvider {
    private readonly string format;

    public ContinuousLabelProvider() {}

    public ContinuousLabelProvider(string format) {
      this.format = format;
    }

    public string GetLabel(double value) {
      return value.ToString(format, CultureInfo.InvariantCulture);
    }
  }
}