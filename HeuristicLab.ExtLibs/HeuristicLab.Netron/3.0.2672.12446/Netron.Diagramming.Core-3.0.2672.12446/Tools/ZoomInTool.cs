
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Allows the user to incrementally adjust (increase) the magnification.
  /// </summary>
  // ----------------------------------------------------------------------
  public class ZoomInTool : ZoomToolBase {
    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The  name of the tool.</param>
    // ------------------------------------------------------------------
    public ZoomInTool(string toolName)
      : base(toolName) {
      // Increase the magnification by a little to zoom in.
      base.myZoomFactor = 1.1F;
    }
  }
}
