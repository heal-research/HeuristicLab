using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public interface IMouseEventListener {
    void MouseMove(object sender, MouseEventArgs e);
    void MouseUp(object sender, MouseEventArgs e);
  }

  public delegate void MoveHandler(Point startPoint, Point endPoint);
  public delegate void RectangleHandler(Rectangle rectangle);
}