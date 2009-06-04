using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  /// <summary>
  /// Interface for MouseEventListeners like ZoomListener, PanListener etc. to simplify state handling.
  /// </summary>
  public interface IMouseEventListener {
    /// <summary>
    /// Call this method on mouse move.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void MouseMove(object sender, MouseEventArgs e);

    /// <summary>
    /// Call this method on mouse up.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void MouseUp(object sender, MouseEventArgs e);
  }

  public delegate void MoveHandler(Point startPoint, Point endPoint);
  public delegate void RectangleHandler(Rectangle rectangle);
}