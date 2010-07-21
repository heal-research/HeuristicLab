using System;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Interface of a controller
  /// </summary>
  // ----------------------------------------------------------------------
  public interface IController : IUndoSupport {
    #region Events

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when a page's ambience has changed.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<AmbienceEventArgs> OnAmbienceChanged;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the undo/redo history has changed
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<HistoryChangeEventArgs> OnHistoryChange;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when a tool is activated
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<ToolEventArgs> OnToolActivate;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when a tool is deactivated
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<ToolEventArgs> OnToolDeactivate;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the something got selected and the properties of it 
    /// can/should be shown.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<SelectionEventArgs> OnShowSelectionProperties;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when an entity is added.
    /// <remarks>This event usually is bubbled from one of the 
    /// layers</remarks>
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityEventArgs> OnEntityAdded;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when an entity is removed.
    /// <remarks>This event usually is bubbled from one of the 
    /// layers</remarks>
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityEventArgs> OnEntityRemoved;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the controller receives a mouse-down notification of 
    /// the surface. This event is raised before the event is broadcasted 
    /// down to the tools.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<MouseEventArgs> OnMouseDown;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the diagram control is aksed to show the context menu
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityMenuEventArgs> OnShowContextMenu;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the model
    /// </summary>
    // ------------------------------------------------------------------
    IModel Model { get; set; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the view.
    /// </summary>
    /// <value>The view.</value>
    // ------------------------------------------------------------------
    IView View { get; set; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the currently active tool.  This can be 'null'!!!
    /// </summary>
    // ------------------------------------------------------------------
    ITool ActiveTool {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the tools.
    /// </summary>
    /// <value>The tools.</value>
    // ------------------------------------------------------------------
    CollectionBase<ITool> Tools { get; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the undo manager.
    /// </summary>
    /// <value>The undo manager.</value>
    // ------------------------------------------------------------------
    UndoManager UndoManager { get; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the parent control.
    /// </summary>
    /// <value>The parent control.</value>
    // ------------------------------------------------------------------
    IDiagramControl ParentControl { get; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this 
    /// <see cref="T:IController"/> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    // ------------------------------------------------------------------
    bool Enabled {
      get;
      set;
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Activates the text editor for the given text provider.
    /// </summary>
    /// <param name="textProvider">ITextProvider</param>
    /// <returns>bool: True if sucessful, false if not.</returns>
    // ------------------------------------------------------------------
    bool ActivateTextEditor(ITextProvider textProvider);

    // ------------------------------------------------------------------
    /// <summary>
    /// Activates the tool.
    /// </summary>
    /// <param name="toolName">Name of the tool.</param>
    // ------------------------------------------------------------------
    void ActivateTool(string toolName);

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds the tool.
    /// </summary>
    /// <param name="tool">The tool.</param>
    // ------------------------------------------------------------------
    void AddTool(ITool tool);

    // ------------------------------------------------------------------
    /// <summary>
    /// Deactivates the tool.
    /// </summary>
    /// <param name="tool">The tool.</param>
    /// <returns>bool: True if successful.</returns>
    // ------------------------------------------------------------------
    bool DeactivateTool(ITool tool);

    // ------------------------------------------------------------------
    /// <summary>
    /// Deactivates all tools.
    /// </summary>
    /// <returns>bool: True if successful.</returns>
    // ------------------------------------------------------------------
    bool DeactivateAllTools();

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnShowSelectionProperties event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.SelectionEventArgs"/> 
    /// instance containing the event data.</param>
    // ------------------------------------------------------------------
    void RaiseOnShowSelectionProperties(SelectionEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Suspends all tools
    /// </summary>
    // ------------------------------------------------------------------
    void SuspendAllTools();

    // ------------------------------------------------------------------
    /// <summary>
    /// Unsuspends all tools.
    /// </summary>
    // ------------------------------------------------------------------
    void UnsuspendAllTools();

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnShowContextMenu event
    /// </summary>
    /// <param name="e">EntityMenuEventArgs</param>
    // ------------------------------------------------------------------
    void RaiseOnShowContextMenu(EntityMenuEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Changes the paint style of the selected entities.
    /// </summary>
    /// <param name="paintStyle">IPaintStyle</param>
    // ------------------------------------------------------------------
    void ChangeStyle(IPaintStyle paintStyle);

    // ------------------------------------------------------------------
    /// <summary>
    /// Changes the pen style of the selected entities.
    /// </summary>
    /// <param name="penStyle">IPenStyle</param>
    // ------------------------------------------------------------------
    void ChangeStyle(IPenStyle penStyle);

    // ------------------------------------------------------------------
    /// <summary>
    /// Runs the specified activity.  If no activity exists with the name
    /// specified, then an exception is thrown.
    /// </summary>
    /// <param name="name">string: The name of the activity to run.</param>
    // ------------------------------------------------------------------
    void RunActivity(string name);

    // ------------------------------------------------------------------
    /// <summary>
    /// Selects all entities on the current page.  The selection is 
    /// cleared first.
    /// </summary>
    // ------------------------------------------------------------------
    void SelectAll();

    // ------------------------------------------------------------------
    /// <summary>
    /// Navigates to the next page.  Nothing is performed if the last page
    /// is currently selected and 'wrap' is false.  If 'wrap' is true,
    /// then the first page is selected.
    /// </summary>
    /// <param name="wrap">bool: Specifies if the collection is wrapped
    /// when the end is reached.</param>
    // ------------------------------------------------------------------
    void GoForward(bool wrap);

    // ------------------------------------------------------------------
    /// <summary>
    /// Navigates to the previous page.  Nothing is performed if the first 
    /// page is currently selected and 'wrap' is false.  If 'wrap' is
    /// true, then the last page is selected if the current page is the
    /// first page.
    /// </summary> 
    /// <param name="wrap">bool: Specifies if the collection is wrapped
    /// when the start is reached.</param>
    // ------------------------------------------------------------------
    void GoBack(bool wrap);

    #endregion
  }
}
