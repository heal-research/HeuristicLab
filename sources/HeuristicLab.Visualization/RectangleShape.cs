using System.Drawing;

namespace HeuristicLab.Visualization {
  public class RectangleShape : IShape {
    private RectangleD rectangle;
    private Color color;
    private int opacity = 255;

    public RectangleShape(double x1, double y1, double x2, double y2, Color color) {
      rectangle = new RectangleD(x1, y1, x2, y2);
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

    public int Opacity {
      get { return opacity; }
      set { opacity = value; }
    }

    public RectangleD Rectangle {
      get { return rectangle; }
      set { rectangle = value; }
    }

    public Color Color {
      get { return color; }
      set { color = value; }
    }
  }
}