using System.Drawing;

namespace HeuristicLab.Visualization {
  public class RectangleShape : IShape {
    private IShape parent;
    private RectangleD rectangle;

    private Color color;
    private int opacity = 255;

    private Pen pen;
    private Brush brush;

    public RectangleShape(double x1, double y1, double x2, double y2, Color color) {
      rectangle = new RectangleD(x1, y1, x2, y2);
      this.color = color;
    }

    public RectangleD BoundingBox {
      get { return rectangle; }
    }

    public RectangleD ClippingArea {
      get { return Parent.ClippingArea; }
    }

    public Rectangle Viewport {
      get { return Parent.Viewport; }
    }

    public IShape Parent {
      get { return parent; }
      set { parent = value; }
    }

    public RectangleD Rectangle {
      get { return rectangle; }
      set { rectangle = value; }
    }

    public void Draw(Graphics graphics) {
      Rectangle screenRect = Transform.ToScreen(rectangle, Parent.Viewport, Parent.ClippingArea);

      graphics.DrawRectangle(GetPen(), screenRect);
      graphics.FillRectangle(GetBrush(), screenRect);
    }

    private Pen GetPen() {
      if (pen == null)
        pen = new Pen(color, 1);
      return pen;
    }

    private Brush GetBrush() {
      if (brush == null)
        brush = new SolidBrush(Color.FromArgb(opacity, color));
      return brush;
    }

    public int Opacity {
      get { return opacity; }
      set {
        opacity = value;
        DisposeDrawingTools();
      }
    }

    public Color Color {
      get { return color; }
      set {
        color = value;
        DisposeDrawingTools();
      }
    }

    private void DisposeDrawingTools() {
      if (pen != null) {
        pen.Dispose();
        pen = null;
      }

      if (brush != null) {
        brush.Dispose();
        brush = null;
      }
    }
  }
}