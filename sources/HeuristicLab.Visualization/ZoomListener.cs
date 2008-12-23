using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public class ZoomListener : IMouseEventListener {
    private readonly Point startPoint;

    public event DrawRectangleHandler DrawRectangle;

    public ZoomListener(Point startPoint) {
      this.startPoint = startPoint;
    }

    #region IMouseEventListener Members

    public event MouseEventHandler OnMouseMove;
    public event MouseEventHandler OnMouseUp;

    public void MouseMove(object sender, MouseEventArgs e) {
      Rectangle r = new Rectangle();
      Point actualPoint = e.Location;

      if (startPoint.X < actualPoint.X) {
        r.X = startPoint.X;
        r.Width = actualPoint.X - startPoint.X;
      } else {
        r.X = actualPoint.X;
        r.Width = startPoint.X - actualPoint.X;
      }

      if (startPoint.Y < actualPoint.Y) {
        r.Y = startPoint.Y;
        r.Height = actualPoint.Y - startPoint.Y;
      } else {
        r.Y = actualPoint.Y;
        r.Height = startPoint.Y - actualPoint.Y;
      }

      if(DrawRectangle != null) {
        DrawRectangle(r);
      }

      if (OnMouseMove != null) {
        OnMouseMove(sender, e);
      }
    }

    public void MouseUp(object sender, MouseEventArgs e) {
      if (OnMouseUp != null) {
        OnMouseUp(sender, e);
      }
    }

    #endregion
  }
}