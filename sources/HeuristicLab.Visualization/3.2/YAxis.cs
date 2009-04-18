using System;
using System.Drawing;
using HeuristicLab.Visualization.LabelProvider;
using HeuristicLab.Visualization.Test;

namespace HeuristicLab.Visualization {
  public class YAxis : WorldShape {
    public const int PixelsPerInterval = 75;

    private ILabelProvider labelProvider = new ContinuousLabelProvider("0.####");
    private AxisPosition position = AxisPosition.Left;
    private bool showLabel = true;
    private string label = "";

    public ILabelProvider LabelProvider {
      get { return labelProvider; }
      set { labelProvider = value; }
    }

    public AxisPosition Position {
      get { return position; }
      set { position = value; }
    }

    public bool ShowLabel {
      get { return showLabel; }
      set { showLabel = value; }
    }

    public string Label {
      get { return label; }
      set { label = value; }
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
        
        TextShape tickLabel = new TextShape(x, y, labelProvider.GetLabel(y));
        tickLabel.AnchorPositionX = anchorPositionX;
        tickLabel.AnchorPositionY = AnchorPositionY.Middle;
        AddShape(tickLabel);
      }

      if (showLabel) {
        double x;
        AnchorPositionY anchorPositionY;

        switch (position) {
          case AxisPosition.Left:
            x = ClippingArea.X1 + 3;
            anchorPositionY = AnchorPositionY.Top;
            break;
          case AxisPosition.Right:
            x = ClippingArea.X2 - 3;
            anchorPositionY = AnchorPositionY.Bottom;
            break;
          default:
            throw new NotImplementedException();
        }

        TextShape label = new TextShape(x,
                                        ClippingArea.Y1 + ClippingArea.Height/2,
                                        this.label);
        label.AnchorPositionX = AnchorPositionX.Middle;
        label.AnchorPositionY = anchorPositionY;
        label.Rotation = -90;
        AddShape(label);
      }

      base.Draw(graphics);
    }
  }
}