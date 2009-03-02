using System.Drawing;
using System.Drawing.Drawing2D;

namespace HeuristicLab.Visualization {
  public class LineShape : IShape {
    private IShape parent;
    private RectangleD boundingBox;

    private Color color;
    private int thickness;
    private DrawingStyle drawingStyle;

    private Pen pen;

    /// <summary>
    /// Initializes the LineShape.
    /// </summary>
    /// <param name="x1">x coordinate of left lineEndPoind</param>
    /// <param name="y1">y coordinate of left lineEndPoind</param>
    /// <param name="x2">x coordinate of right lineEndPoind</param>
    /// <param name="y2">y coordinate of right lineEndPoind</param>
    /// <param name="color">color for the LineShape</param>
    /// <param name="thickness">tickness of the line in pixels</param>
    /// <param name="drawingStyle">drawing style of the line (solid, dashed, dotted,...)</param>
    public LineShape(double x1, double y1, double x2, double y2, Color color, int thickness, DrawingStyle drawingStyle) {
      this.boundingBox = new RectangleD(x1, y1, x2, y2);
      this.LSColor = color;
      this.LSThickness = thickness;
      this.LSDrawingStyle = drawingStyle;
    }

    public RectangleD BoundingBox {
      get { return boundingBox; }
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

    public double Y1 {
      get { return boundingBox.Y1; }
      set { boundingBox.Y1 = value; }
    }

    public double Y2 {
      get { return boundingBox.Y2; }
      set { boundingBox.Y2 = value; }
    }

    public double X1 {
      get { return boundingBox.X1; }
      set { boundingBox.X1 = value; }
    }

    public double X2 {
      get { return boundingBox.X2; }
      set { boundingBox.X2 = value; }
    }

    /// <summary>
    /// Draws the LineShape.
    /// </summary>
    /// <param name="graphics">graphics handle to draw to</param>
    public void Draw(Graphics graphics) {
      Rectangle screenRect = Transform.ToScreen(boundingBox, Parent.Viewport, Parent.ClippingArea);

      graphics.DrawLine(GetPen(), screenRect.Left, screenRect.Bottom, screenRect.Right, screenRect.Top);
    }

    private Pen GetPen() {
      if (pen == null) {
        pen = new Pen(LSColor, LSThickness);

        switch (LSDrawingStyle) {
          case DrawingStyle.Dashed:
            pen.DashStyle = DashStyle.Dash;
            break;
          default:
            pen.DashStyle = DashStyle.Solid;
            break;
        }
      }

      return pen;
    }

    private void DisposePen() {
      if (pen != null) {
        pen.Dispose();
        pen = null;
      }
    }

    public Color LSColor {
      get { return color; }
      set {
        color = value;
        DisposePen();
      }
    }

    public int LSThickness {
      get { return thickness; }
      set {
        thickness = value;
        DisposePen();
      }
    }

    public DrawingStyle LSDrawingStyle {
      get { return drawingStyle; }
      set {
        drawingStyle = value;
        DisposePen();
      }
    }
  }
}