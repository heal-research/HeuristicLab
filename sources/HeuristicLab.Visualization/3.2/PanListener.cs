using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  /// <summary>
  /// Supports the state panning.
  /// </summary>
  public class PanListener : IMouseEventListener {
    private Point startPoint;

    /// <summary>
    /// This event will be fired every MouseMove call.
    /// </summary>
    public event MoveHandler Pan;

    public PanListener(Point startPoint) {
      this.startPoint = startPoint;
    }

    #region IMouseEventListener Members

    public void MouseMove(object sender, MouseEventArgs e) {
      if (Pan != null) {
        Pan(startPoint, e.Location);
      }

      startPoint = e.Location;  
    }

    public void MouseUp(object sender, MouseEventArgs e) {}

    #endregion
  }
}