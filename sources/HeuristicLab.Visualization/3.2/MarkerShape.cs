using System.Drawing;
using System.Drawing.Drawing2D;
using HeuristicLab.Visualization.Drawing;

namespace HeuristicLab.Visualization {
  class MarkerShape : IShape {

    private IShape parent;
    private RectangleD boundingBox;
    

    private Color color;
    private int thickness;
    private int width;
    private DrawingStyle drawingStyle;

    private Pen pen;

    /// <summary>
    /// Initializes the Marker.
    /// </summary>
    /// <param name="x">x coordinate of left dataPoint</param>
    /// <param name="y">y coordinate of left dataPoint</param>
    /// <param name="width">width of the whole marker</param>
    /// <param name="color">color for the marker</param>
    public MarkerShape(double x, double y, int width, Color color) {
      this.boundingBox = new RectangleD(x , y , x, y );
      this.width = width;
      this.LSColor = color;
    }

    public RectangleD BoundingBox {
      get { return boundingBox; }
      set { boundingBox = value; }
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

    public double Y {
      get { return boundingBox.Y1; }
      set {
        boundingBox.Y1 = value ;
        boundingBox.Y2 = value ;
      }
    }


    public double X {
      get { return (boundingBox.X1+boundingBox.X2)/2; }
      set {
        boundingBox.X1 = value;
        boundingBox.X2 = value;
      }
    }

   

    /// <summary>
    /// Draws the LineShape.
    /// </summary>
    /// <param name="graphics">graphics handle to draw to</param>
    public virtual void Draw(Graphics graphics) {
      Rectangle screenRect = Transform.ToScreen(boundingBox, Parent.Viewport, Parent.ClippingArea);
      graphics.DrawEllipse(GetPen(),screenRect.Left-2, screenRect.Top-2,4,4);
    }

    private Pen GetPen() {
      if (pen == null) {
        pen = new Pen(Color.Black, 3);

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

    public DrawingStyle LSDrawingStyle {
      get { return drawingStyle; }
      set {
        drawingStyle = value;
        DisposePen();
      }
    }

  }
}
