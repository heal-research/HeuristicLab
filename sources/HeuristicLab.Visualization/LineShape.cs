using System.Drawing;

namespace HeuristicLab.Visualization {
  public class LineShape : IShape {
    private RectangleD boundingRect;
    private double z;
    private Color color;

    /// <summary>
    /// Initializes the LineShape.
    /// </summary>
    /// <param name="x1">x coordinate of left lineEndPoind</param>
    /// <param name="y1">y coordinate of left lineEndPoind</param>
    /// <param name="x2">x coordinate of right lineEndPoind</param>
    /// <param name="y2">y coordinate of right lineEndPoind</param>
    /// <param name="color">color for the LineShape</param>
    public LineShape(double x1, double y1, double x2, double y2, double z, Color color) {
      this.boundingRect = new RectangleD(x1, y1, x2, y2);
      this.z = z;
      this.color = color;
    }

    public RectangleD BoundingBox {
      get { return boundingRect; }
    }

    /// <summary>
    /// Draws the LineShape.
    /// </summary>
    /// <param name="graphics">graphics handle to draw to</param>
    /// <param name="viewport">rectangle in value-coordinates to display</param>
    /// <param name="clippingArea">rectangle in screen-coordinates to draw</param>
    public void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      using (Pen pen = new Pen(color, 3)){
        Rectangle screenRect = Transform.ToScreen(boundingRect, viewport, clippingArea);
        graphics.DrawLine(pen,screenRect.Left, screenRect.Bottom, screenRect.Right, screenRect.Top);
      }
    }

    public double Z {
      get { return z; }
      set { z = value; }
    }
  }
}
