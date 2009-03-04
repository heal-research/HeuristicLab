using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public class PanListener : IMouseEventListener {
    private Point startPoint;

    public event MoveHandler Pan;
    public event MoveHandler PanEnd;

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

    public void MouseUp(object sender, MouseEventArgs e) {
      if(PanEnd != null) {
        PanEnd(startPoint, e.Location);
      }
    }

    #endregion
  }
}