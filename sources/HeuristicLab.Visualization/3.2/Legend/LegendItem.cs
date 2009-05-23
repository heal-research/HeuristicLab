using System.Drawing;

namespace HeuristicLab.Visualization.Legend {
  public class LegendItem {

    // default width
    // every legend under 100 gets the default width
    public static readonly int WIDTH = 100;

    /// <summary>
    /// a legend item object with its properties
    /// </summary>
    /// <param name="label">the label of the legend</param>
    /// <param name="color">the color of the legend line</param>
    /// <param name="thickness">the thickness of the legend line</param>
    public LegendItem(string label, Color color, int thickness) {
      Label = label;
      Color = color;
      Thickness = thickness;
    }

    public string Label { get; set; }
    public Color Color { get; set; }
    public int Thickness { get; set; }
  }
}