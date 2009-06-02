using System.Drawing;
using HeuristicLab.Visualization.Drawing;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public class XAxis : WorldShape {
    public const int PixelsPerInterval = 100;

    private ILabelProvider labelProvider = new ContinuousLabelProvider("0.####");

    private Color color = Color.Blue;
    private Font font = new Font("Arial", 8);
    private bool showLabel = true;
    private string label = "";

    public ILabelProvider LabelProvider {
      get { return labelProvider; }
      set { labelProvider = value; }
    }

    public override void Draw(Graphics graphics) {
      ClearShapes();

      foreach (double x in AxisTicks.GetTicks(PixelsPerInterval, Parent.Viewport.Width,
                                              ClippingArea.Width,
                                              ClippingArea.X1)) {
        TextShape tickLabel = new TextShape(x, ClippingArea.Height - 3,
                                            labelProvider.GetLabel(x), Font, Color);
        tickLabel.AnchorPositionX = AnchorPositionX.Middle;
        tickLabel.AnchorPositionY = AnchorPositionY.Top;
        AddShape(tickLabel);
      }

      if (showLabel) {
        TextShape label = new TextShape(ClippingArea.X1 + ClippingArea.Width/2,
                                        ClippingArea.Y1 + 3,
                                        this.label);
        label.AnchorPositionX = AnchorPositionX.Middle;
        label.AnchorPositionY = AnchorPositionY.Bottom;

        AddShape(label);
      }

      base.Draw(graphics);
    }

    public Color Color {
      get { return color; }
      set { color = value; }
    }

    public Font Font {
      get { return font; }
      set { font = value; }
    }

    public bool ShowLabel {
      get { return showLabel; }
      set { showLabel = value; }
    }

    public string Label {
      get { return label; }
      set { label = value; }
    }
  }
}