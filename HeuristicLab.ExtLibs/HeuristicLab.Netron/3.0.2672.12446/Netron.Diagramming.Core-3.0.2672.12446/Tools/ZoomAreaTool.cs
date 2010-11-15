using System;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Allows the user to zoom in to a user-drawn rectangle area.
  /// </summary>
  // ----------------------------------------------------------------------
  public class ZoomAreaTool : AbstractTool, IMouseListener {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// The location of the mouse when the motion starts.
    /// </summary>
    // ------------------------------------------------------------------
    protected Point initialPoint;

    // ------------------------------------------------------------------
    /// <summary>
    /// Says whether the startingPoint was set, otherwise the ghost will 
    /// appear even before an initial point was set!
    /// </summary>
    // ------------------------------------------------------------------
    protected bool started;

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The  name of the tool.</param>
    // ------------------------------------------------------------------
    public ZoomAreaTool(string toolName)
      : base(toolName) {
    }

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the tool is activated.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void OnActivateTool() {
      base.OnActivateTool();
      Controller.View.CurrentCursor = CursorPalette.Cross;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the mouse down event
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public bool MouseDown(MouseEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(
            "The argument object is 'null'");
      }

      if (e.Button == MouseButtons.Left && IsActive && !IsSuspended) {
        initialPoint = e.Location;
        started = true;
        return true; // This tells the tool-loop to stop looking 
        // for another handler, which keeps the CPU low.

      }
      return false;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the mouse move event
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public void MouseMove(MouseEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException(
            "The argument object is 'null'");
      }
      if (IsActive && !IsSuspended && started) {
        IView view = this.Controller.View;
        Point point = e.Location;

        Controller.View.PaintGhostRectangle(initialPoint, point);
        Rectangle area = System.Drawing.Rectangle.Inflate(
            Controller.View.Ghost.Rectangle, 20, 20);

        Controller.View.Invalidate(area);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the mouse up event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public void MouseUp(MouseEventArgs e) {
      if (IsActive) {
        IView view = Controller.View;
        if (view.Ghost != null) {
          Rectangle zoomArea = view.Ghost.Rectangle;
          view.ZoomArea(zoomArea);
          started = false;
        }
        Controller.View.ResetGhost();
      }
    }

    #endregion
  }
}
