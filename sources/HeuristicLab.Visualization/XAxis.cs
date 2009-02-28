using System.Drawing;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public class XAxis : WorldShape {
    public const int PixelsPerInterval = 100;
    
    private ILabelProvider labelProvider = new ContinuousLabelProvider("0.####");

    public ILabelProvider LabelProvider {
      get { return labelProvider; }
      set { labelProvider = value; }
    }

    public override void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      shapes.Clear();

      foreach (double x in AxisTicks.GetTicks(PixelsPerInterval, viewport.Width,
                                              ClippingArea.Width,
                                              ClippingArea.X1)) {
        TextShape label = new TextShape(x, ClippingArea.Height - 3,
                                        labelProvider.GetLabel(x));
        label.AnchorPositionX = AnchorPositionX.Middle;
        label.AnchorPositionY = AnchorPositionY.Top;
        shapes.Add(label);
      }

      base.Draw(graphics, viewport, clippingArea);
    }
  }
}