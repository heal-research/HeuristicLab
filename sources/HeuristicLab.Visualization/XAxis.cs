using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {

  public class XAxis : WorldShape {
    private readonly IDictionary<int, TextShape> labels = new Dictionary<int, TextShape>();

    public XAxis(RectangleD clippingArea, RectangleD boundingBox)
      : base(clippingArea, boundingBox) {}

    public void ClearLabels() {
      labels.Clear();
    }

    public void SetLabel(int i, string text) {
      labels[i] = new TextShape(i, 0, text);
    }

    public override void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      shapes.Clear();

      for (int i = (int)(ClippingArea.X1 - 1); i <= ClippingArea.X2 + 1; i++) {
        TextShape label;

        if (labels.ContainsKey(i)) {
          label = labels[i];
        } else {
          label = new TextShape(i, 0, i.ToString());
        }

        label.Y = ClippingArea.Height - 3;

        shapes.Add(label);
      }

      base.Draw(graphics, viewport, clippingArea);
    }
  }
}