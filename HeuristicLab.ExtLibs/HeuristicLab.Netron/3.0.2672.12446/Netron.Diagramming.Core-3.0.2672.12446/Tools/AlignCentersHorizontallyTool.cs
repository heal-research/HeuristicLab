using System.Drawing;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Aligns the center of the selected entities horizontally.
  /// </summary>
  // ----------------------------------------------------------------------
  public class AlignCentersHorizontallyTool : AlignmentToolBase {
    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The name of this tool</param>
    // ------------------------------------------------------------------
    public AlignCentersHorizontallyTool(string toolName)
      : base(toolName) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Aligns the center of all selected entities horizontally.  The 
    /// 'X' component of the center location of the first entity in the 
    /// selection is used for all other entities.
    /// </summary>
    // ------------------------------------------------------------------
    public override void Align(IDiagramEntity[] entities) {
      // The amount to offset the entities by.
      Point offset;

      for (int i = 1; i < entities.Length; i++) {
        IDiagramEntity entity = entities[i];

        // Keep the entities same y location but offset it's
        // x location.
        offset = new Point(
            this.centerOfFirstEntity.X - entity.Center.X,
            0);

        // Move the entity by this amount.
        entity.MoveBy(offset);
      }
    }
  }
}
