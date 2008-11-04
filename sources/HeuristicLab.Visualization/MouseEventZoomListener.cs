using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  internal class MouseEventZoomListener : IMouseEventListener {
    #region Interface members

    public event MouseEventHandler OnMouseDown;
    public event MouseEventHandler OnMouseUp;
    public event MouseEventHandler OnMouseMove;

    #endregion

    public MouseEventHandler OnZoomAreaChanged;
    public MouseEventHandler OnZoomFinished;
  }
}