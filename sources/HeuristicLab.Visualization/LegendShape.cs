using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {
  public class LegendItem {
    public LegendItem(string label, Color color, int thickness) {
      Label = label;
      Color = color;
      Thickness = thickness;
    }

    public string Label { get; set; }
    public Color Color { get; set; }
    public int Thickness { get; set; }
  }


  public class LegendShape : CompositeShape {
    private RectangleD boundingBox;
    private Color color;

    private readonly IList<LegendItem> legendItems = new List<LegendItem>();

    public LegendShape(double x1, double y1, double x2, double y2, double z, Color color) {
      boundingBox = new RectangleD(x1, y1, x2, y2);
      Z = z;
      this.color = color;
      CreateLegend();
    }

    public double Z { get; set; }

    public RectangleD BoundingBox {
      get { return boundingBox; }
      set { boundingBox = value; }
    }

    public void CreateLegend() {
      double y = boundingBox.Y2;
      foreach (LegendItem item in legendItems) {
        AddShape(new LineShape(10, y - 10, 30, y - 10, 0, item.Color, item.Thickness, DrawingStyle.Solid));
        AddShape(new TextShape(35, y, item.Label));
        y -= 15;
      }
    }

    public void AddLegendItem(LegendItem item) {
      legendItems.Add(item);
    }

    public void RemoveLegendItem(LegendItem item) {
      legendItems.Remove(item);
    }

    public void ClearLegendItems() {
      legendItems.Clear();
    }
  }
}