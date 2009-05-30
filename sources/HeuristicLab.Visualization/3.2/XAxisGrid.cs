using System.Drawing;
using HeuristicLab.Visualization.Drawing;

namespace HeuristicLab.Visualization {
  public class XAxisGrid : WorldShape {
    private Color color = Color.LightBlue;

    public override void Draw(Graphics graphics) {
      ClearShapes();

      foreach (double x in AxisTicks.GetTicks(XAxis.PixelsPerInterval,
                                              Parent.Viewport.Width,
                                              ClippingArea.Width,
                                              ClippingArea.X1)) {
        LineShape line = new LineShape(x, ClippingArea.Y1,
                                       x, ClippingArea.Y2,
                                       color, 1,
                                       DrawingStyle.Dashed);
        AddShape(line);
      }

      base.Draw(graphics);
    }

    public Color Color {
      get { return color; }
      set { color = value; }
    }
  }
}