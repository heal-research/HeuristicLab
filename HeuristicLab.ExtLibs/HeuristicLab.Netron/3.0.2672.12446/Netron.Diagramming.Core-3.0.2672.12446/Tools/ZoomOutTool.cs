
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Allows the user to incrementally adjust (increase) the magnification.
  /// </summary>
  // ----------------------------------------------------------------------
  public class ZoomOutTool : ZoomToolBase {
    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The  name of the tool.</param>
    // ------------------------------------------------------------------
    public ZoomOutTool(string toolName)
      : base(toolName) {
      // Decrease the magnification by a little to zoom out.
      base.myZoomFactor = 0.9F;
    }
  }
}
