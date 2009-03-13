using System.Drawing;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public class YAxis : WorldShape {
    public const int PixelsPerInterval = 75;

    private ILabelProvider labelProvider = new ContinuousLabelProvider("e4");

    public ILabelProvider LabelProvider {
      get { return labelProvider; }
      set { labelProvider = value; }
    }

    public override void Draw(Graphics graphics) {
      ClearShapes();

      foreach (double y in AxisTicks.GetTicks(PixelsPerInterval, Parent.Viewport.Height,
                                              ClippingArea.Height,
                                              ClippingArea.Y1)) {
        TextShape label = new TextShape(ClippingArea.X2 - 3, y,
                                        labelProvider.GetLabel(y));
        label.AnchorPositionX = AnchorPositionX.Right;
        label.AnchorPositionY = AnchorPositionY.Middle;
        AddShape(label);
      }

      base.Draw(graphics);
    }
  }
}