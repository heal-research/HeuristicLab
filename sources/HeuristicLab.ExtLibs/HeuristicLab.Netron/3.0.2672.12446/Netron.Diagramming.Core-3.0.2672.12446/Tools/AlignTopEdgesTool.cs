using System.Drawing;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Aligns the top edges of the selected entities.
  /// </summary>
  // ----------------------------------------------------------------------
  public class AlignTopEdgesTool : AlignmentToolBase {
    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The name of this tool</param>
    // ------------------------------------------------------------------
    public AlignTopEdgesTool(string toolName)
      : base(toolName) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Aligns the top edges of all selected entities.  The vertical
    /// location of the first entity in the selection is used for all 
    /// other entities vertical location.
    /// </summary>
    // ------------------------------------------------------------------
    public override void Align(IDiagramEntity[] entities) {
      // We want to align the top edges, so we need to set the
      // vertical location of each shape to one value.  We're
      // going use the first entity in the selection to determine
      // this setting.
      Point offset;

      for (int i = 1; i < entities.Length; i++) {
        IDiagramEntity entity = entities[i];

        // Keep the entities same y location but offset it's
        // x location.
        offset = new Point(
            0,
            this.topEdgeOfFirstEntity - entity.Rectangle.Top);

        // Move the entity by this amount.
        entity.MoveBy(offset);
      }
    }
  }
}
