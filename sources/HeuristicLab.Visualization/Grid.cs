using System.Drawing;

namespace HeuristicLab.Visualization {
  public class Grid : WorldShape {
    public override void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      shapes.Clear();

      foreach (double y in AxisTicks.GetTicks(YAxis.PixelsPerInterval,
                                              viewport.Height,
                                              ClippingArea.Height,
                                              ClippingArea.Y1)) {
        LineShape line = new LineShape(ClippingArea.X1, y,
                                       ClippingArea.X2, y,
                                       Color.LightBlue, 1,
                                       DrawingStyle.Dashed);
        shapes.Add(line);
      }

      foreach (double x in AxisTicks.GetTicks(XAxis.PixelsPerInterval,
                                              viewport.Width,
                                              ClippingArea.Width,
                                              ClippingArea.X1)) {
        LineShape line = new LineShape(x, ClippingArea.Y1,
                                       x, ClippingArea.Y2,
                                       Color.LightBlue, 1,
                                       DrawingStyle.Dashed);
        shapes.Add(line);
      }

      LineShape lineZeroX = new LineShape(0, ClippingArea.Y1,
                                          0, ClippingArea.Y2,
                                          Color.LightBlue, 3,
                                          DrawingStyle.Dashed);

      LineShape lineZeroY = new LineShape(ClippingArea.X1, 0,
                                          ClippingArea.X2, 0,
                                          Color.LightBlue, 3,
                                          DrawingStyle.Dashed);

      shapes.Add(lineZeroX);
      shapes.Add(lineZeroY);

      base.Draw(graphics, viewport, clippingArea);
    }
  }
}