using System.Drawing;

namespace HeuristicLab.Visualization {
  public class RectangleShape : IShape {
    private RectangleD rectangle;
    private double z;
    private Color color;
    private int opacity = 255;

    public RectangleShape(double x1, double y1, double x2, double y2, double z, Color color) {
      rectangle = new RectangleD(x1, y1, x2, y2);
      this.z = z;
      this.color = color;
    }

    public RectangleD BoundingBox {
      get { return rectangle; }
    }

    public void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      Color brushColor = Color.FromArgb(opacity, color);

      using (Pen pen = new Pen(color, 1))
      using (Brush brush = new SolidBrush(brushColor)) {
        Rectangle screenRect = Transform.ToScreen(rectangle, viewport, clippingArea);

        graphics.DrawRectangle(pen, screenRect);
        graphics.FillRectangle(brush, screenRect);
      }
    }

    public double Z {
      get { return z; }
      set { z = value; }
    }

    public int Opacity {
      get { return opacity; }
      set { opacity = value; }
    }

    public RectangleD Rectangle {
      get { return rectangle; }
      set { rectangle = value; }
    }
  }
}