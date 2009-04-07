using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public class ZoomListener : IMouseEventListener {
    private readonly Point startPoint;

    public event RectangleHandler DrawRectangle;
    public event RectangleHandler SetClippingArea;

    public ZoomListener(Point startPoint) {
      this.startPoint = startPoint;
    }

    #region IMouseEventListener Members

    public void MouseMove(object sender, MouseEventArgs e) {
      if(DrawRectangle != null) {
        DrawRectangle(CalcRectangle(e.Location));
      }
    }

    public void MouseUp(object sender, MouseEventArgs e) {
     if(SetClippingArea != null) {
       SetClippingArea(CalcRectangle(e.Location));
     }
    }

    #endregion

    private Rectangle CalcRectangle(Point actualPoint) {
      Rectangle rectangle = new Rectangle();

      if (startPoint.X < actualPoint.X) {
        rectangle.X = startPoint.X;
        rectangle.Width = actualPoint.X - startPoint.X;
      } else {
        rectangle.X = actualPoint.X;
        rectangle.Width = startPoint.X - actualPoint.X;
      }

      if (startPoint.Y < actualPoint.Y) {
        rectangle.Y = startPoint.Y;
        rectangle.Height = actualPoint.Y - startPoint.Y;
      } else {
        rectangle.Y = actualPoint.Y;
        rectangle.Height = startPoint.Y - actualPoint.Y;
      }

      return rectangle;
    }

    public static RectangleD ZoomClippingArea(RectangleD clippingArea, double zoomFactor) {
      double x1, x2, y1, y2, width, height;

      width = clippingArea.Width * zoomFactor;
      height = clippingArea.Height * zoomFactor;

      x1 = clippingArea.X1 - (width - clippingArea.Width) / 2;
      y1 = clippingArea.Y1 - (height - clippingArea.Height) / 2;
      x2 = width + x1;
      y2 = height + y1;

      return new RectangleD(x1, y1, x2, y2);
    }
  }
}