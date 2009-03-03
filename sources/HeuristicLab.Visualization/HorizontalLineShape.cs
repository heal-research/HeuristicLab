using System.Drawing;

namespace HeuristicLab.Visualization {
  public class HorizontalLineShape : LineShape {
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
    public HorizontalLineShape(double x1, double y1, double x2, double y2, Color color, int thickness,
                               DrawingStyle drawingStyle) : base(x1, y1, x2, y2, color, thickness, drawingStyle) {}

    public override void Draw(Graphics graphics) {
      X1 = Parent.ClippingArea.X1;
      X2 = Parent.ClippingArea.X2;
      Y2 = Y1;
      base.Draw(graphics);
    }
  }
}
