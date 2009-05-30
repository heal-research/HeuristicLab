using System.Drawing;

namespace HeuristicLab.Visualization.Legend {
  public class LegendItem {
    // default width
    // every legend under 100 gets the default width
    public static readonly int WIDTH = 100;

    private readonly IDataRow row;

    /// <summary>
    /// a legend item object with its properties
    /// </summary>
    public LegendItem(IDataRow row) {
      this.row = row;
    }

    public string Label {
      get { return row.RowSettings.Label; }
    }

    public Color Color {
      get { return row.RowSettings.Color; }
    }

    public int Thickness {
      get { return row.RowSettings.Thickness; }
    }
  }
}