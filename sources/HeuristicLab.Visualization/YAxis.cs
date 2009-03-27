using System;
using System.Drawing;
using HeuristicLab.Visualization.LabelProvider;
using HeuristicLab.Visualization.Test;

namespace HeuristicLab.Visualization {
  public class YAxis : WorldShape {
    public const int PixelsPerInterval = 75;

    private ILabelProvider labelProvider = new ContinuousLabelProvider("e4");
    private AxisPosition position = AxisPosition.Left;

    public ILabelProvider LabelProvider {
      get { return labelProvider; }
      set { labelProvider = value; }
    }

    public AxisPosition Position {
      get { return position; }
      set { position = value; }
    }

    public override void Draw(Graphics graphics) {
      ClearShapes();

      foreach (double y in AxisTicks.GetTicks(PixelsPerInterval, Parent.Viewport.Height,
                                              ClippingArea.Height,
                                              ClippingArea.Y1)) {
        double x;
        AnchorPositionX anchorPositionX;

        switch (position) {
          case AxisPosition.Left:
            x = ClippingArea.X2 - 3;
            anchorPositionX = AnchorPositionX.Right;
            break;
          case AxisPosition.Right:
            x = ClippingArea.X1 + 3;
            anchorPositionX = AnchorPositionX.Left;
            break;
          default:
            throw new NotImplementedException();
        }
        
        TextShape label = new TextShape(x, y, labelProvider.GetLabel(y));
        label.AnchorPositionX = anchorPositionX;
        label.AnchorPositionY = AnchorPositionY.Middle;
        AddShape(label);
      }

      base.Draw(graphics);
    }
  }
}