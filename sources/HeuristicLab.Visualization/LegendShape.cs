using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {
  public class LegendItem {
    public LegendItem(string label, Color color) {
      Label = label;
      Color = color;
    }

    public string Label { get; set; }
    public Color Color { get; set; }
  }


  public class LegendShape : IShape {
    private readonly Color color;

    private readonly IList<LegendItem> legendItems = new List<LegendItem>();
    private readonly RectangleD rect;

    public LegendShape(double x1, double y1, double x2, double y2, double z, Color color) {
      rect = new RectangleD(x1, y1, x2, y2);
      Z = z;
      this.color = color;
    }

    public double Z { get; set; }

    #region IShape Members

    public RectangleD BoundingBox {
      get { return rect; }
    }

    public void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      using (var pen = new Pen(color, 1))
      using (Brush brush = new SolidBrush(color)) {
        Rectangle screenRect = Transform.ToScreen(rect, viewport, clippingArea);

        graphics.DrawRectangle(pen, screenRect);
        graphics.FillRectangle(brush, screenRect);
      }
    }

    #endregion

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