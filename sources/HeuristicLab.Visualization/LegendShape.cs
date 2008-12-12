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
      int startY = 10;
      foreach (LegendItem i in legendItems) {
        using (Pen pen = new Pen(i.Color, 5)) {
          graphics.DrawLine(pen, 10, startY+10, 30, startY+10);
        }
        using (Brush brush = new SolidBrush(Color.Black)) {
          graphics.DrawString(i.Label, new Font("Arial", 12), brush, 35, startY);
        }
        startY += 15;
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