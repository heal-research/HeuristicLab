using System.Drawing;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// The base class for zooming tools.  Zoom tools allow the user to 
  /// incrementally adjust (increase or decrease) the View's magnification.
  /// </summary>
  // ----------------------------------------------------------------------
  public class ZoomToolBase : AbstractTool {
    // ------------------------------------------------------------------
    /// <summary>
    /// Used to multiply the current magnification by.
    /// </summary>
    // ------------------------------------------------------------------
    protected float myZoomFactor = 0.9F;

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the multiplier used to adjust the View's
    /// Magnification by.  The default is 0.9.
    /// </summary>
    // ------------------------------------------------------------------
    public float ZoomFactor {
      get {
        return myZoomFactor;
      }
      set {
        myZoomFactor = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="toolName">string: The  name of the tool.</param>
    // ------------------------------------------------------------------
    public ZoomToolBase(string toolName)
      : base(toolName) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Activates this tool - the View's current magnification is
    /// mulitiplied by 'ZoomFactor'.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void OnActivateTool() {
      base.OnActivateTool();

      SizeF size = Controller.View.Magnification;
      SizeF autoScrollMinSize =
          Controller.ParentControl.AutoScrollMinSize;
      Point origin = Controller.View.Origin;
      Point parentAutoScrollPosition =
          Controller.ParentControl.AutoScrollPosition;

      Controller.View.Magnification = new SizeF(
          size.Width * myZoomFactor,
          size.Height * myZoomFactor);

      // Remember to also adjust the diagram's scroll bars.
      size = new SizeF(
          autoScrollMinSize.Width * myZoomFactor,
          autoScrollMinSize.Height * myZoomFactor);
      Controller.ParentControl.AutoScrollMinSize = Size.Round(size);

      // Should we set the Origin to the location of the selected items
      // if there are any?  This will allow the user to zoom in on
      // a selection.
      if (this.Controller.Model.Selection.SelectedItems.Count > 0) {
        Bundle bundle = new Bundle(this.Controller.Model.Selection.SelectedItems);
        Point bundleLocation = bundle.Rectangle.Location;

        // Don't move the origin *exactly* to the bundle's location.
        // Offset it a little so the bundle is butted-up with the
        // upper-right hand corner of the screen.
        bundleLocation.Offset(-20, -20);
        origin.Offset(Point.Round(Controller.View.WorldToView(
            bundleLocation)));

        Controller.View.Origin = origin;
      }

      Controller.ParentControl.AutoScrollPosition =
          Controller.View.Origin;

      DeactivateTool();
    }
  }
}
