using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public class PanListener : IMouseEventListener {
    private Point startPoint;

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