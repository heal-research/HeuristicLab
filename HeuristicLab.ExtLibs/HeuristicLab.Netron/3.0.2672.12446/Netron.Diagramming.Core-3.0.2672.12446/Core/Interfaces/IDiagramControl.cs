using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Interface of the surface or diagram control.
  /// </summary>
  // ----------------------------------------------------------------------
  public interface IDiagramControl {
    #region Events
    /// <summary>
    /// Occurs when a drag-and-drop operation is completed.
    /// </summary>
    event DragEventHandler DragDrop;
    /// <summary>
    /// Occurs when an object is dragged into the control's bounds.
    /// </summary>
    event DragEventHandler DragEnter;
    /// <summary>
    /// Occurs when an object is dragged out of the control's bounds.
    /// </summary>
    event EventHandler DragLeave;
    /// <summary>
    /// Occurs when an object is dragged over the control's bounds.
    /// </summary>
    event DragEventHandler DragOver;

    event GiveFeedbackEventHandler GiveFeedback;
    /// <summary>
    /// Occurs when the size of the canvas has changed
    /// </summary>
    /// <remarks>
    /// This event is usually defined already in the Control or ScrollableControl class from which
    /// the canvas inherits.
    /// </remarks>
    event EventHandler SizeChanged;
    /// <summary>
    /// Occurs when the mouse is pressed on the canvas
    /// </summary>
    /// <remarks>
    /// This event is usually defined already in the Control or ScrollableControl class from which
    /// the canvas inherits.
    /// </remarks>
    event MouseEventHandler MouseDown;
    /// <summary>
    /// Occurs when the mouse is released above the canvas
    /// </summary>
    /// <remarks>
    /// This event is usually defined already in the Control or ScrollableControl class from which
    /// the canvas inherits.
    /// </remarks>
    event MouseEventHandler MouseUp;
    /// <summary>
    /// Occurs when the mouse is moved over the canvas
    /// </summary>
    /// <remarks>
    /// This event is usually defined already in the Control or ScrollableControl class from which
    /// the canvas inherits.
    /// </remarks>
    event MouseEventHandler MouseMove;
    /// <summary>
    /// Occurs when the mouse pointer rests on the control.
    /// </summary>
    event EventHandler MouseHover;

    event MouseEventHandler MouseWheel;
    /// <summary>
    /// Occurs when a key is down
    /// </summary>
    event KeyEventHandler KeyDown;
    /// <summary>
    /// Occurs when a key is released
    /// </summary>
    event KeyEventHandler KeyUp;
    /// <summary>
    /// Occurs when a key is pressed
    /// </summary>
    event KeyPressEventHandler KeyPress;
    /// <summary>
    /// Occurs when a new diagram is started.
    /// </summary>
    event EventHandler OnNewDiagram;
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the view.
    /// </summary>
    /// <value>The view.</value>
    IView View {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the controller.
    /// </summary>
    /// <value>The controller.</value>
    IController Controller {
      get;
      set;

    }

    /// <summary>
    /// Gets or sets the Document
    /// </summary>
    Document Document {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if all shape's connectors should be shown.
    /// </summary>
    // ------------------------------------------------------------------
    bool ShowConnectors {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the scroll position.
    /// </summary>
    // ------------------------------------------------------------------
    Point AutoScrollPosition {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the client rectangle.
    /// </summary>
    /// <value>The client rectangle.</value>
    // ------------------------------------------------------------------
    Rectangle ClientRectangle {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the size of the diagram.
    /// </summary>
    /// <value>Size: The height and width.</value>
    // ------------------------------------------------------------------
    Size Size {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the minimum size of the autoscroll.
    /// </summary>
    /// <value>Size: The height and width.</value>
    // ------------------------------------------------------------------
    Size AutoScrollMinSize {
      get;
      set;
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Invalidates the specified rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    // ------------------------------------------------------------------
    void Invalidate(Rectangle rectangle);

    // ------------------------------------------------------------------
    /// <summary>
    /// Invalidates this instance.
    /// </summary>
    // ------------------------------------------------------------------
    void Invalidate();

    // ------------------------------------------------------------------
    /// <summary>
    /// Focuses this instance.
    /// </summary>
    /// <returns></returns>
    // ------------------------------------------------------------------
    bool Focus();

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays a PageSetupDialog so the user can specify how each
    /// page is printed.
    /// </summary>
    // ------------------------------------------------------------------
    void PageSetup();

    // ------------------------------------------------------------------
    /// <summary>
    /// Prints all pages of the diagram.
    /// </summary>
    // ------------------------------------------------------------------
    void Print();

    // ------------------------------------------------------------------
    /// <summary>
    /// Print previews all pages of the diagram.
    /// </summary>
    // ------------------------------------------------------------------
    void PrintPreview();

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays an OpenFileDialog for the user to specify the diagram
    /// to open.
    /// </summary>
    // ------------------------------------------------------------------
    void Open();

    // ------------------------------------------------------------------
    /// <summary>
    /// Opens the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    // ------------------------------------------------------------------
    void Open(string path);

    // ------------------------------------------------------------------
    /// <summary>
    /// If the current filename (FileName property) is empty, the a
    /// SaveFileDialog is displayed for the user to specify what to save 
    /// the diagram as.  Otherwise, the current filename is used to save 
    /// the diagram.
    /// </summary>
    // ------------------------------------------------------------------
    void Save();

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays a SaveFileDialog regardless if there's an existing
    /// filename so the user can save the diagram to a new location.
    /// </summary>
    // ------------------------------------------------------------------
    void SaveAs();

    // ------------------------------------------------------------------
    /// <summary>
    /// Saves the diagram to the path specified.
    /// </summary>
    /// <param name="path">The path.</param>
    // ------------------------------------------------------------------
    void SaveAs(string path);

    // ------------------------------------------------------------------
    /// <summary>
    /// Creates a new document.
    /// </summary>
    // ------------------------------------------------------------------
    void NewDocument();

    // ------------------------------------------------------------------
    /// <summary>
    /// Computes the location of the specified screen point into client
    /// coordinates.
    /// </summary>
    /// <param name="p">Point</param>
    /// <returns>Point</returns>
    // ------------------------------------------------------------------
    Point PointToClient(Point p);

    #endregion

  }
}
