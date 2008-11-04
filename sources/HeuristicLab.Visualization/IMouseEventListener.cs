using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  internal interface IMouseEventListener {
    event MouseEventHandler OnMouseDown;
    event MouseEventHandler OnMouseUp;
    event MouseEventHandler OnMouseMove;
  }
}