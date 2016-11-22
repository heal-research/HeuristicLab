using System.Drawing;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Aligns the right edges of the selected entities.
  /// </summary>
  // ----------------------------------------------------------------------
  public class AlignRightEdgesTool : AlignmentToolBase {
    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The name of this tool</param>
    // ------------------------------------------------------------------
    public AlignRightEdgesTool(string toolName)
      : base(toolName) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Aligns the right edges of all selected entities.  The horizontal
    /// location of the first entity in the selection is used for all 
    /// other entities horizontal location.
    /// </summary>
    // ------------------------------------------------------------------
    public override void Align(IDiagramEntity[] entities) {
      // We want to align the left edges, so we need to set the
      // horizontal location of each shape to one value.  We're
      // going use the first entity in the selection to determine
      // this setting.
      Point offset;

      for (int i = 1; i < entities.Length; i++) {
        IDiagramEntity entity = entities[i];

        // Keep the entities same y location but offset it's
        // x location.
        offset = new Point(
            this.rightEdgeOfFirstEntity - entity.Rectangle.Right,
            0);

        // Move the entity by this amount.
        entity.MoveBy(offset);
      }
    }
  }
}
