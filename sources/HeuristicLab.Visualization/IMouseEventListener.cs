using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public interface IMouseEventListener {
    void MouseMove(object sender, MouseEventArgs e);
    void MouseUp(object sender, MouseEventArgs e);

    event MouseEventHandler OnMouseMove;
    event MouseEventHandler OnMouseUp;
  }

  public delegate void SetNewClippingAreaHandler(RectangleD newClippingArea);
  public delegate void DrawRectangleHandler(Rectangle rectangle);
}