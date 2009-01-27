using System.Globalization;

namespace HeuristicLab.Visualization {
  public class DefaultLabelProvider : ILabelProvider {
    private readonly string format;

    public DefaultLabelProvider() {}

    public DefaultLabelProvider(string format) {
      this.format = format;
    }

    public string GetLabel(double value) {
      return value.ToString(format, CultureInfo.InvariantCulture);
    }
  }
}