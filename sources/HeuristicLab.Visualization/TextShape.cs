using System.Drawing;

namespace HeuristicLab.Visualization {
  public class TextShape : IShape {
    private Font font;
    private Brush brush;
    private Color color;
    private string text;
    private double x;
    private double y;

    public TextShape(double x, double y, string text) : this(x, y, text, 8) {}

    public TextShape(double x, double y, string text, int fontSize) {
      this.x = x;
      this.y = y;
      this.text = text;
      font = new Font("Arial", fontSize);

      Color = Color.Blue;
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
        color = value;
        brush = new SolidBrush(color);
      }
    }

    #region IShape Members

    public void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      int screenX = Transform.ToScreenX(x, viewport, clippingArea);
      int screenY = Transform.ToScreenY(y, viewport, clippingArea);

      graphics.DrawString(text, font, brush, screenX, screenY);
    }

    public RectangleD BoundingBox {
      get { return RectangleD.Empty; }
    }

    #endregion
  }
}