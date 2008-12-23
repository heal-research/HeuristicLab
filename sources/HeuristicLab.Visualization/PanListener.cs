using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public class PanListener : IMouseEventListener {
    private readonly Rectangle viewPort;
    private readonly RectangleD clippingArea;
    private readonly Point startPoint;

    public PanListener(Rectangle viewPort, RectangleD clippingArea, Point startPoint) {
      this.viewPort = viewPort;
      this.clippingArea = clippingArea;
      this.startPoint = startPoint;
    }

    #region IMouseEventListener Members

    public event SetNewClippingAreaHandler SetNewClippingArea;
    public event MouseEventHandler OnMouseMove;
    public event MouseEventHandler OnMouseUp;

    public void MouseMove(object sender, MouseEventArgs e) {
      PointD worldStartPoint = Transform.ToWorld(startPoint, viewPort, clippingArea);
      PointD worldActualPoint = Transform.ToWorld(e.Location, viewPort, clippingArea);

      double xDiff = worldActualPoint.X - worldStartPoint.X;
      double yDiff = worldActualPoint.Y - worldStartPoint.Y;

      RectangleD newClippingArea = new RectangleD();
      newClippingArea.X1 = clippingArea.X1 - xDiff;
      newClippingArea.X2 = clippingArea.X2 - xDiff;
      newClippingArea.Y1 = clippingArea.Y1 - yDiff;
      newClippingArea.Y2 = clippingArea.Y2 - yDiff;

      if (SetNewClippingArea != null) {
        SetNewClippingArea(newClippingArea);
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