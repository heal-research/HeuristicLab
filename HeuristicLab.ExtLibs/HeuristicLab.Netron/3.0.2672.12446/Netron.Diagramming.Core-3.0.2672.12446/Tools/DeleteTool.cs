
namespace Netron.Diagramming.Core {
  public class DeleteTool : AbstractTool {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName"></param>
    public DeleteTool(string toolName)
      : base(toolName) {
    }

    protected override void OnActivateTool() {
      base.OnActivateTool();

      DeleteCommand cmd;

      if (this.Controller.Model.Selection.SelectedItems.Count > 0) {
        // If any one entity in the selction can't be deleted,
        // remove it from the selection.
        for (int i = 0; i < this.Controller.Model.Selection.SelectedItems.Count; i++) {
          IDiagramEntity entity = this.Controller.Model.Selection.SelectedItems[i];
          if (entity.AllowDelete == false) {
            this.Controller.Model.Selection.SelectedItems.Remove(entity);
            i--;
          }
        }
        cmd = new DeleteCommand(
                this.Controller,
               this.Controller.Model.Selection.SelectedItems.Copy());
        this.Controller.UndoManager.AddUndoCommand(cmd);

        // Alert each entity that they're about to be deleted.
        foreach (IDiagramEntity entity in this.Controller.Model.Selection.SelectedItems) {
          entity.OnBeforeDelete(cmd);
        }

        cmd.Redo();

        // Alert each entity that they have been deleted.
        foreach (IDiagramEntity entity in this.Controller.Model.Selection.SelectedItems) {
          entity.OnAfterDelete(cmd);
        }
      }

      DeactivateTool();
    }
  }
}
