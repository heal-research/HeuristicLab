
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// The tool that performs the "cut" operation.
  /// </summary>
  // ----------------------------------------------------------------------
  public class CutTool : AbstractTool {
    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The name of this tool.</param>
    // ------------------------------------------------------------------
    public CutTool(string toolName)
      : base(toolName) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the tool is activated.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void OnActivateTool() {
      if (this.Controller.Model.Selection.SelectedItems.Count == 0)
        return;

      // How about for the cut, if we use two existing tools.  First,
      // activate the copy tool, then activate the delete tool.  That's
      // essentially a cut operation, right?
      Controller.ActivateTool(ControllerBase.CopyToolName);
      Controller.ActivateTool(ControllerBase.DeleteToolName);
    }
  }
}
