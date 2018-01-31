using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  public abstract class AlignmentToolBase : AbstractTool {
    protected IDiagramEntity firstEntity;
    protected int xLocationOfFirstEntity;
    protected int yLocationOfFirstEntity;
    protected int topEdgeOfFirstEntity;
    protected int bottomEdgeOfFirstEntity;
    protected Point centerOfFirstEntity;
    protected int rightEdgeOfFirstEntity;

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The name of this tool</param>
    // ------------------------------------------------------------------
    public AlignmentToolBase(string toolName)
      : base(toolName) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Activates the tool.  First a check is performed to ensure there
    /// are at least two IDiagramEntities selected.  If so, then the
    /// x, y, top edge, bottom edge, and center of the first entity
    /// is stored in local, protected variables for all other alignment
    /// tools to use.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void OnActivateTool() {
      base.OnActivateTool();

      // Make sure enough items were selected.
      if (this.Controller.Model.Selection.SelectedItems == null) {
        MessageBox.Show(
            "Nothing is selected, you need to select at " +
            "least two items to align.",
            "Nothing selected.",
            MessageBoxButtons.OK,
            MessageBoxIcon.Hand);

        return;
      }

      if (this.Controller.Model.Selection.SelectedItems.Count <= 1) {
        MessageBox.Show(
            "You need to select at least two items to align.",
            "Nothing selected.",
            MessageBoxButtons.OK,
            MessageBoxIcon.Hand);

        return;
      }

      // Since there are enough items, peform the alignment.  But
      // first get all aspects about the location of the first
      // entity.
      this.firstEntity = this.Controller.Model.Selection.SelectedItems[0];
      this.xLocationOfFirstEntity = firstEntity.Rectangle.X;
      this.yLocationOfFirstEntity = firstEntity.Rectangle.Y;
      this.topEdgeOfFirstEntity = firstEntity.Rectangle.Top;
      this.bottomEdgeOfFirstEntity = firstEntity.Rectangle.Bottom;
      this.rightEdgeOfFirstEntity = firstEntity.Rectangle.Right;
      this.centerOfFirstEntity = firstEntity.Center;

      this.Align(this.Controller.Model.Selection.SelectedItems.ToArray());

      // Reset the Tracker.
      this.Controller.View.ShowTracker();
      DeactivateTool();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Abstract method to be implemented by all alignment tools.
    /// </summary>
    /// <param name="entities">IDiagramEntity[]: All selected
    /// entities.</param>
    // ------------------------------------------------------------------
    public abstract void Align(IDiagramEntity[] entities);
  }
}
