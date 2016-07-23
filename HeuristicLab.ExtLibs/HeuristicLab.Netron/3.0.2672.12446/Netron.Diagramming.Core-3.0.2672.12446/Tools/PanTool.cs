using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// A tool that "pans" the diagram.  The 'Origin' of the diagram is
  /// adjusted when the left mouse button is held down and dragged
  /// across the canvas.
  /// </summary>
  // ----------------------------------------------------------------------
  public class PanTool :
      AbstractTool,
      IMouseListener,
      IKeyboardListener {
    // ------------------------------------------------------------------
    /// <summary>
    /// The number of times the mouse location has been updated.  This is
    /// used to determine if the cursor is updated.  The cursor is only
    /// updated every 5 times the mouse is moved and then this number is
    /// reset to 0.  The mouse is updated when this number is zero.
    /// </summary>
    // ------------------------------------------------------------------
    int mouseMoveNumber = 0;

    // ------------------------------------------------------------------
    /// <summary>
    /// Set to true when the left mouse buttone is down, set to false
    /// all other times.
    /// </summary>
    // ------------------------------------------------------------------
    bool isLeftMouseButtonPressed = false;

    // ------------------------------------------------------------------
    /// <summary>
    /// The initial location of the mouse (when the mouse was clicked).
    /// </summary>
    // ------------------------------------------------------------------
    Point initialLocation = Point.Empty;

    // ------------------------------------------------------------------
    /// <summary>
    /// The last location of the mouse.
    /// </summary>
    // ------------------------------------------------------------------
    Point previousMouseLocation = Point.Empty;

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The name of this tool.</param>
    // ------------------------------------------------------------------
    public PanTool(string toolName)
      : base(toolName) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Activates the tool and sets the cursor to a hand cursor to provide
    /// visual feedback that panning is activated.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void OnActivateTool() {
      base.OnActivateTool();
      Cursor = CursorPalette.Pan;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Compares the location specified to the previous location and
    /// sets the cursor as follows:
    ///     * current x < previous x and curreny y = previous y => pan W
    ///     * current x > previous x and curreny y = previous y => pan E
    ///     * current x = previous x and curreny y < previous y => pan N
    ///     * current x = previous x and curreny y > previous y => pan S
    ///     * current x < previous x and curreny y < previous y => pan NW
    ///     * current x > previous x and curreny y < previous y => pan NE
    ///     * current x < previous x and curreny y > previous y => pan SW
    ///     * current x > previous x and curreny y > previous y => pan SE
    /// </summary>
    /// <param name="location">Point: The current cursor location.</param>
    // ------------------------------------------------------------------
    protected void UpdateCursor(Point location) {
      if ((location.X < previousMouseLocation.X) &&
          (location.Y == previousMouseLocation.Y)) {
        Cursor = Cursors.PanWest;
      } else if ((location.X > previousMouseLocation.X) &&
              (location.Y == previousMouseLocation.Y)) {
        Cursor = Cursors.PanEast;
      } else if ((location.X == previousMouseLocation.X) &&
           (location.Y < previousMouseLocation.Y)) {
        Cursor = Cursors.PanNorth;
      } else if ((location.X == previousMouseLocation.X) &&
           (location.Y > previousMouseLocation.Y)) {
        Cursor = Cursors.PanSouth;
      } else if ((location.X < previousMouseLocation.X) &&
           (location.Y < previousMouseLocation.Y)) {
        Cursor = Cursors.PanNW;
      } else if ((location.X > previousMouseLocation.X) &&
           (location.Y < previousMouseLocation.Y)) {
        Cursor = Cursors.PanNE;
      } else if ((location.X < previousMouseLocation.X) &&
           (location.Y > previousMouseLocation.Y)) {
        Cursor = Cursors.PanSW;
      } else if ((location.X > previousMouseLocation.X) &&
           (location.Y > previousMouseLocation.Y)) {
        Cursor = Cursors.PanSE;
      }
    }

    #region IMouseListener Members

    // ------------------------------------------------------------------
    /// <summary>
    /// Starts the panning action if this tool is activated and is not
    /// suspended.
    /// </summary>
    /// <param name="e">MouseEventArgs</param>
    // ------------------------------------------------------------------
    public bool MouseDown(MouseEventArgs e) {
      if ((!IsActive) ||
          (IsSuspended == true)) {
        this.isLeftMouseButtonPressed = false;
        this.previousMouseLocation = Point.Empty;
        return false;
      }

      if (e.Button == MouseButtons.Left) {
        this.isLeftMouseButtonPressed = true;
        this.previousMouseLocation =
            Point.Round(Controller.View.WorldToView(e.Location));
        this.initialLocation = previousMouseLocation;
        return true;
      } else {
        this.isLeftMouseButtonPressed = false;
        this.previousMouseLocation = Point.Empty;
        return false;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Pans the diagram by the offset amount of the last mouse position
    /// and the new mouse position divided by 2 (to slow things down a
    /// bit; otherwise the "distance" panned is too much).
    /// </summary>
    /// <param name="e">MouseEventArgs</param>
    // ------------------------------------------------------------------
    public void MouseMove(MouseEventArgs e) {
      Point currentLocation =
          Point.Round(Controller.View.WorldToView(e.Location));
      if ((!IsSuspended) &&
          (IsActive) &&
          (this.isLeftMouseButtonPressed)) {
        // Change the cursor to indicated which direction we're 
        // panning.
        if (mouseMoveNumber == 0) {
          UpdateCursor(currentLocation);
        }
        mouseMoveNumber++;
        if (mouseMoveNumber > 5) {
          mouseMoveNumber = 0;
        }

        IDiagramControl control = Controller.ParentControl;
        Point origin = Controller.View.Origin;

        Point offset = new Point(
            (previousMouseLocation.X - currentLocation.X),
            (previousMouseLocation.Y - currentLocation.Y));

        origin.Offset(offset);

        // 0,0 is the min scrolling point.
        if (origin.X < 0) {
          origin.X = 0;
        }

        if (origin.Y < 0) {
          origin.Y = 0;
        }
        control.AutoScrollPosition = origin;
        Controller.View.Origin = origin;
        previousMouseLocation = currentLocation;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Stops the panning action (does not deactivate the tool).
    /// </summary>
    /// <param name="e">MouseEventArgs</param>
    // ------------------------------------------------------------------
    public void MouseUp(MouseEventArgs e) {
      if ((IsActive) && (!IsSuspended)) {
        this.isLeftMouseButtonPressed = false;
        this.previousMouseLocation = Point.Empty;
        Cursor = CursorPalette.Pan;
      }
    }

    #endregion

    #region IKeyboardListener Members

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IKeyboardListener - deactivates this tool when
    /// the 'escape' key is pressed.
    /// </summary>
    /// <param name="e">KeyEventArgs</param>
    // ------------------------------------------------------------------
    public void KeyDown(KeyEventArgs e) {
      if (e.KeyCode == Keys.Escape)
        DeactivateTool();
      else if (e.KeyCode == Keys.Space)
        ActivateTool();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IKeyboardListener - nothing performed here.
    /// </summary>
    /// <param name="e">KeyEventArgs</param>
    // ------------------------------------------------------------------
    public void KeyUp(KeyEventArgs e) {
      if (e.KeyCode == Keys.Space)
        DeactivateTool();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IKeyboardListener - nothing performed here.
    /// </summary>
    /// <param name="e">KeyPressEventArgs</param>
    // ------------------------------------------------------------------
    public void KeyPress(KeyPressEventArgs e) {
    }

    #endregion
  }
}
