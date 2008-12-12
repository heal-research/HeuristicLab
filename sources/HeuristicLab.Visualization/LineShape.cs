using System.Drawing;
using System.Drawing.Drawing2D;

namespace HeuristicLab.Visualization {
  public class LineShape : IShape {
    private RectangleD boundingBox;
    private double z;
    private Color color;
    private int thickness;
    private DashStyle dashStyle;

    /// <summary>
    /// Initializes the LineShape.
    /// </summary>
    /// <param name="x1">x coordinate of left lineEndPoind</param>
    /// <param name="y1">y coordinate of left lineEndPoind</param>
    /// <param name="x2">x coordinate of right lineEndPoind</param>
    /// <param name="y2">y coordinate of right lineEndPoind</param>
    /// <param name="color">color for the LineShape</param>
    public LineShape(double x1, double y1, double x2, double y2, double z, Color color, int thickness, DrawingStyle style) {
      this.boundingBox = new RectangleD(x1, y1, x2, y2);
      this.z = z;
      this.color = color;
      this.thickness = thickness;
      if (style==DrawingStyle.Dashed) {
        this.dashStyle = DashStyle.Dash;
      }
      else {
        this.dashStyle = DashStyle.Solid;        //default
      }
    }

    public RectangleD BoundingBox {
      get { return boundingBox; }
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
    /// <param name="viewport">rectangle in value-coordinates to display</param>
    /// <param name="clippingArea">rectangle in screen-coordinates to draw</param>
    public void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      using (Pen pen = new Pen(color, thickness)){
        pen.DashStyle = this.dashStyle;
        Rectangle screenRect = Transform.ToScreen(boundingBox, viewport, clippingArea);
        graphics.DrawLine(pen,screenRect.Left, screenRect.Bottom, screenRect.Right, screenRect.Top);
      }
    }

    public double Z {
      get { return z; }
      set { z = value; }
    }
  }
}
