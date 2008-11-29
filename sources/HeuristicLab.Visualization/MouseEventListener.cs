using System.Drawing;

namespace HeuristicLab.Visualization {
  /// <summary>
  /// Helper class for different OnMouseMove and OnMouseUp implementations.
  /// </summary>
  public class MouseEventListener {
    /// <summary>
    /// Fired when the MouseMove method was called.
    /// </summary>
    public event MouseEventListenerHandler OnMouseMove;

    /// <summary>
    /// Fired when the MouseUp method was called.
    /// </summary>
    public event MouseEventListenerHandler OnMouseUp;

    private Point startPoint;

    /// <summary>
    /// Call this method to fire the OnMouseMove event.
    /// </summary>
    /// <param name="actualPoint"></param>
    public void MouseMove(Point actualPoint) {
      if (OnMouseMove != null) {
        OnMouseMove(startPoint, actualPoint);
      }
    }

    /// <summary>
    /// Call this method to fire the OnMouseUp event.
    /// </summary>
    /// <param name="actualPoint">Actual point of the mouse</param>
    public void MouseUp(Point actualPoint) {
      if (OnMouseUp != null) {
        OnMouseUp(startPoint, actualPoint);
      }
    }

    /// <summary>
    /// Gets or sets the starting point of the mouse.
    /// </summary>
    public Point StartPoint {
      get { return startPoint; }
      set { startPoint = value; }
    }
  }

  /// <summary>
  /// Handler for the MouseEventListener events.
  /// </summary>
  /// <param name="startPoint">Starting point of the mouse.</param>
  /// <param name="actualPoint">Actual point of the mouse.</param>
  public delegate void MouseEventListenerHandler(Point startPoint, Point actualPoint);
}