using System.Drawing;

namespace HeuristicLab.Visualization {
  public class LegendShape : IShape {
    private readonly Color color;
    private readonly RectangleD rect;

    public LegendShape(double x1, double y1, double x2, double y2, double z, Color color) {
      rect = new RectangleD(x1, y1, x2, y2);
      this.Z = z;
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
  }
}