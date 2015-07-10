using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  /// <summary>
  /// This tool informs the IController to display a ContextMenu when
  /// the right mouse button is clicked once.
  /// </summary>
  public class ContextTool : AbstractTool, IMouseListener {

    #region Fields

    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="T:HoverTool"/> class.
    /// </summary>
    /// <param name="name">The name of the tool.</param>
    // ------------------------------------------------------------------
    public ContextTool(string name)
      : base(name) {
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the tool is activated.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void OnActivateTool() {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the mouse down event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    /// <returns>Returns 'true' if the event was handled, otherwise 
    /// 'false'.</returns>
    // ------------------------------------------------------------------
    public bool MouseDown(MouseEventArgs e) {
      if (!IsSuspended && this.Enabled) {
        if (e.Button == MouseButtons.Right && e.Clicks == 1) {
          // Just the base menu for now.
          ToolStripItem[] additionalItems = null;

          this.Controller.RaiseOnShowContextMenu(
              new EntityMenuEventArgs(null, e, ref additionalItems));
          return true;
        }
      }
      return false;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the mouse move event - nothing is performed here.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public void MouseMove(MouseEventArgs e) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the mouse up event - nothing is performed here.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public void MouseUp(MouseEventArgs e) {
    }

    #endregion
  }

}
