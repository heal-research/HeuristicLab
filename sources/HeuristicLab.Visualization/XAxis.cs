using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {
  // TODO move to own file
  public class TextShape : IShape {
    private double x;
    private double y;
    private string text;

    private Font font;
    private Color color;
    private Brush brush;

    public TextShape(double x, double y, string text) : this(x, y, text, 8) {}

    public TextShape(double x, double y, string text, int fontSize) {
      this.x = x;
      this.y = y;
      this.text = text;
      this.font = new Font("Arial", fontSize);

      this.Color = Color.Blue;
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

    public double X {
      get { return x; }
      set { x = value; }
    }

    public double Y {
      get { return y; }
      set { y = value; }
    }

    public Color Color {
      get { return color; }
      set {
        this.color = value;
        this.brush = new SolidBrush(color);
      }
    }
  }

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