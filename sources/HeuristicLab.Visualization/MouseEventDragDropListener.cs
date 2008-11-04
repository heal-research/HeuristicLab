using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  internal class MouseEventDragDropListener : IMouseEventListener{
    #region Interface members

    public event MouseEventHandler OnMouseDown;
    public event MouseEventHandler OnMouseUp;
    public event MouseEventHandler OnMouseMove;

    #endregion

    public event MouseEventHandler OnDrag;
    public event MouseEventHandler OnDrop;
  }
}