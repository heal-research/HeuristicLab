using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {
  // TODO move to own file
  public class TextShape : IShape {
    private readonly double x;
    private readonly double y;
    private string text;

    private Font font = new Font("Arial", 8);
    private Brush brush = new SolidBrush(Color.Blue);

    public TextShape(double x, double y, string text) {
      this.x = x;
      this.y = y;
      this.text = text;
    }

    public void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      int screenX = Transform.ToScreenX(x, viewport, clippingArea);
      int screenY = Transform.ToScreenY(y, viewport, clippingArea);

      graphics.DrawString(text, font, brush, screenX, screenY);
    }

    public RectangleD BoundingBox {
      get { return RectangleD.Empty; }
    }

    public string Text {
      get { return text; }
      set { text = value; }
    }
  }

  public class XAxis : CompositeShape {
    private readonly List<TextShape> labels = new List<TextShape>();
    private readonly LineShape axisLine = new LineShape(0, 0, 0, 0, 0, Color.Black, 1, DrawingStyle.Solid);

    public void ClearLabels() {
      shapes.Clear();
      labels.Clear();

      shapes.Add(axisLine);
    }

    public override void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      axisLine.X1 = Transform.ToWorldX(viewport.Left, viewport, clippingArea);
      axisLine.X2 = Transform.ToWorldX(viewport.Right, viewport, clippingArea);

      base.Draw(graphics, viewport, clippingArea);
    }

    public void SetLabel(int i, string text) {
      while (i >= labels.Count) {
        TextShape label = new TextShape(i, 0, i.ToString());
        labels.Add(label);
        AddShape(label);
      }
      labels[i].Text = text;
    }
  }
}