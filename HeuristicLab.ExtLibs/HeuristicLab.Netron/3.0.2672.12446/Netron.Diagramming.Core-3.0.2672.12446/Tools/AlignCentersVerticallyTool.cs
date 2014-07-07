using System.Drawing;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Aligns the center of the selected entities vertically.
  /// </summary>
  // ----------------------------------------------------------------------
  public class AlignCentersVerticallyTool : AlignmentToolBase {
    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The name of this tool</param>
    // ------------------------------------------------------------------
    public AlignCentersVerticallyTool(string toolName)
      : base(toolName) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Aligns the center of all selected entities vertically.  The 
    /// 'Y' component of the center location of the first entity in the 
    /// selection is used for all other entities.
    /// </summary>
    // ------------------------------------------------------------------
    public override void Align(IDiagramEntity[] entities) {
      // The amount to offset the entities by.
      Point offset;

      for (int i = 1; i < entities.Length; i++) {
        IDiagramEntity entity = entities[i];

        // Keep the entities same x location but offset it's
        // y location.
        offset = new Point(
            0,
            this.centerOfFirstEntity.Y - entity.Center.Y);

        // Move the entity by this amount.
        entity.MoveBy(offset);
      }
    }
  }
}
