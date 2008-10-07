using System.Drawing;

namespace HeuristicLab.Visualization {
  public class RectangleShape : IShape {
    private RectangleD rect;
    private double z;
    private Color color;

    public RectangleShape(double x1, double y1, double x2, double y2, double z, Color color) {
      this.rect = new RectangleD(x1, y1, x2, y2);
      this.z = z;
      this.color = color;
    }

    public RectangleD BoundingBox {
      get { return rect; }
    }

    public void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      using (Pen pen = new Pen(color, 1))
      using (Brush brush = new SolidBrush(color)) {
        Rectangle screenRect = Transform.ToScreen(rect, viewport, clippingArea);

        graphics.DrawRectangle(pen, screenRect);
        graphics.FillRectangle(brush, screenRect);
      }
    }

    public double Z {
      get { return z; }
      set { z = value; }
    }
  }
}
