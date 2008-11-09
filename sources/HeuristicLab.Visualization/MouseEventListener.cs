using System.Drawing;

namespace HeuristicLab.Visualization {
  public class MouseEventListener {
    public event MouseEventListenerHandler OnMouseMove;
    public event MouseEventListenerHandler OnMouseUp;

    private Point startPoint;

    public void MouseMove(Point actualPoint) {
      if (OnMouseMove != null) {
        OnMouseMove(startPoint, actualPoint);
      }
    }

    public void MouseUp(Point actualPoint) {
      if (OnMouseUp != null) {
        OnMouseUp(startPoint, actualPoint);
      }
    }

    public Point StartPoint {
      get { return startPoint; }
      set { startPoint = value; }
    }
  }

  public delegate void MouseEventListenerHandler(Point startPoint, Point actualPoint);
}