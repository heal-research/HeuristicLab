using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using HeuristicLab.Visualization.Drawing;

namespace HeuristicLab.Visualization {
  public enum AnchorPositionX {
    Left, Middle, Right
  }

  public enum AnchorPositionY {
    Bottom, Middle, Top
  }

  public class TextShape : IShape {
    private IShape parent;
    private Font font;
    private Brush brush;
    private Color color;
    private string text;
    private double x;
    private double y;
    private float rotation = 0;

    private AnchorPositionX anchorPositionX = AnchorPositionX.Left;
    private AnchorPositionY anchorPositionY = AnchorPositionY.Top;

    public TextShape(string text) : this(0, 0, text, 14) {}

    public TextShape(double x, double y, string text) : this(x, y, text, 8) {}

    public TextShape(double x, double y, string text, int fontSize) {
      this.x = x;
      this.y = y;
      this.text = text;
      font = new Font("Arial", fontSize);

      Color = Color.Blue;
    }

    public TextShape(double x, double y, string text, Font font, Color color) {
      this.x = x;
      this.y = y;
      this.text = text;
      Font = font;
      Color = color;
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

    public float Rotation {
      get { return rotation; }
      set { rotation = value; }
    }

    public Color Color {
      get { return color; }
      set {
        color = value;
        brush = new SolidBrush(color);
      }
    }

    public Font Font {
      get { return font; }
      set { font = value; }
    }

    public AnchorPositionX AnchorPositionX {
      get { return anchorPositionX; }
      set { anchorPositionX = value; }
    }

    public AnchorPositionY AnchorPositionY {
      get { return anchorPositionY; }
      set { anchorPositionY = value; }
    }

    #region IShape Members

    public void Draw(Graphics graphics) {
      int screenX = Transform.ToScreenX(x, Parent.Viewport, Parent.ClippingArea);
      int screenY = Transform.ToScreenY(y, Parent.Viewport, Parent.ClippingArea);
      int offsetX, offsetY;

      SizeF size = graphics.MeasureString(text, font);

      switch (AnchorPositionX) {
        case AnchorPositionX.Left:
          offsetX = 0;
          break;
        case AnchorPositionX.Middle:
          offsetX = -(int)(size.Width/2);
          break;
        case AnchorPositionX.Right:
          offsetX = -(int)size.Width;
          break;
        default:
          throw new NotSupportedException("Unknown anchor position: " + AnchorPositionX);
      }

      switch (AnchorPositionY) {
        case AnchorPositionY.Top:
          offsetY = 0;
          break;
        case AnchorPositionY.Middle:
          offsetY = -(int)(size.Height/2);
          break;
        case AnchorPositionY.Bottom:
          offsetY = -(int)size.Height;
          break;
        default:
          throw new NotSupportedException("Unknown anchor position: " + AnchorPositionX);
      }

      GraphicsState gstate = graphics.Save();
      graphics.TranslateTransform(screenX, screenY, MatrixOrder.Append);
      graphics.RotateTransform(rotation, MatrixOrder.Prepend);
      graphics.DrawString(text, font, brush, offsetX, offsetY);
      graphics.Restore(gstate);
    }

    public RectangleD BoundingBox {
      get { return RectangleD.Empty; }
    }

    public RectangleD ClippingArea {
      get { return Parent.ClippingArea; }
    }

    public Rectangle Viewport {
      get { return Parent.Viewport; }
    }

    public IShape Parent {
      get { return parent; }
      set { parent = value; }
    }

    #endregion
  }
}