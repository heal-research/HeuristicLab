using System.Drawing;
using HeuristicLab.Visualization.Drawing;

namespace HeuristicLab.Visualization {
  public class YAxisGrid : WorldShape {
    private Color color = Color.LightBlue;

    public override void Draw(Graphics graphics) {
      ClearShapes();

      foreach (double y in AxisTicks.GetTicks(YAxis.PixelsPerInterval,
                                              Parent.Viewport.Height,
                                              ClippingArea.Height,
                                              ClippingArea.Y1)) {
        LineShape line = new LineShape(ClippingArea.X1, y,
                                       ClippingArea.X2, y,
                                       color, 1,
                                       DrawingStyle.Dashed);
        AddShape(line);
      }

      LineShape lineZeroX = new LineShape(0, ClippingArea.Y1,
                                          0, ClippingArea.Y2,
                                          color, 3,
                                          DrawingStyle.Dashed);

      LineShape lineZeroY = new LineShape(ClippingArea.X1, 0,
                                          ClippingArea.X2, 0,
                                          color, 3,
                                          DrawingStyle.Dashed);

      AddShape(lineZeroX);
      AddShape(lineZeroY);

      base.Draw(graphics);
    }

    public Color Color {
      get { return color; }
      set { color = value; }
    }
  }
}