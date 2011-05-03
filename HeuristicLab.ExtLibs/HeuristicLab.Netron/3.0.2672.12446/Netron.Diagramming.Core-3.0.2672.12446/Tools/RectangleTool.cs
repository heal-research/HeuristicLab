using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Tool to draw rectangles on the canvas.
  /// </summary>
  // ----------------------------------------------------------------------
  public class RectangleTool : AbstractDrawingTool {
    #region Fields

    #endregion

    #region Properties

    #endregion

    #region Constructor
    public RectangleTool()
      : base("Rectangle Tool") {
    }
    public RectangleTool(string name)
      : base(name) {

    }
    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Activates the tool - the cursor is set to the Icons.DrawRectangle.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void OnActivateTool() {
      base.OnActivateTool();
      Cursor = CursorPalette.DrawRectangle;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Draws a ghost rectangle on the canvas if 'IsActive' and 'started'
    /// is true.
    /// </summary>
    /// <param name="e">MouseEventArgs</param>
    // ------------------------------------------------------------------
    protected override void OnMouseMove(MouseEventArgs e) {
      base.OnMouseMove(e);
      if (IsActive && started) {
        Point point = new Point(e.X, e.Y);

        Controller.View.PaintGhostRectangle(startingPoint, point);

        Controller.View.Invalidate(System.Drawing.Rectangle.Inflate(
            Controller.View.Ghost.Rectangle, 20, 20));
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// This method will be called when the user has finished drawing a 
    /// ghost rectangle or bundle and initiates the actual creation of a 
    /// bundle and the addition to the model via the appropriate command.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void GhostDrawingComplete() {
      if ((IsActive == false) ||
          (started == false)) {
        return;
      }

      try {
        SimpleRectangle shape = new SimpleRectangle(this.Controller.Model);
        shape.Width = (int)Rectangle.Width;
        shape.Height = (int)Rectangle.Height;
        AddShapeCommand cmd = new AddShapeCommand(
            this.Controller,
            shape,
            new Point((int)Rectangle.X, (int)Rectangle.Y));

        this.Controller.UndoManager.AddUndoCommand(cmd);
        cmd.Redo();
      }
      catch {
        base.Controller.DeactivateTool(this);
        Controller.View.Invalidate();
        throw;
      }

      //base.Controller.DeactivateTool(this);
    }


    #endregion
  }

}
