using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Abstract base implementation of the <see cref="IController"/> 
  /// interface.
  /// </summary>
  // ----------------------------------------------------------------------
  public abstract class ControllerBase : IUndoSupport, IController {
    #region Events

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the context menu is shown.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityMenuEventArgs> OnShowContextMenu;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when a tool is asked to be deactivated.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<ToolEventArgs> OnToolDeactivate;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when a tool is asked to be activated.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<ToolEventArgs> OnToolActivate;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the history has changed in the undo/redo mechanism.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<HistoryChangeEventArgs> OnHistoryChange;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the something got selected and the properties of it 
    /// can/should be shown.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<SelectionEventArgs> OnShowSelectionProperties;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when an entity is added.
    /// <remarks>This event usually is bubbled from one of the 
    /// layers.</remarks>
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityEventArgs> OnEntityAdded;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when an entity is removed.
    /// <remarks>This event usually is bubbled from one of the 
    /// layers.</remarks>
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityEventArgs> OnEntityRemoved;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the controller receives a mouse-down notification of 
    /// the surface. This event is raised before the
    /// event is broadcasted down to the tools.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<MouseEventArgs> OnMouseDown;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the Ambience has changed.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<AmbienceEventArgs> OnAmbienceChanged;


    #endregion

    #region Tool Names

    public const string AlignBottomEdgesToolName =
        "Align Bottom Edges Tool";

    public const string AlignCentersHorizToolName =
        "Align Centers Horizontally";

    public const string AlignCentersVertToolName =
        "Align Centers Vertically";

    public const string AlignLeftEdgesToolName =
        "Align Left Edges Tool";

    public const string AlignRightEdgesToolName =
        "Align Right Edges Tool";

    public const string AlignTopEdgesToolName =
        "Align Top Edges Tool";

    public const string ComplexRectangleToolName = "ComplexRectangle Tool";
    public const string ConnectionToolName = "Connection Tool";
    public const string ConnectorMoverToolName = "Connector Mover Tool";
    public const string ContextToolName = "Context Tool";
    public const string CopyToolName = "Copy Tool";
    public const string CutToolName = "Cut Tool";
    public const string DeleteToolName = "Delete Tool";
    public const string DragDropToolName = "DragDrop Tool";
    public const string EllipseToolName = "Ellipse Tool";
    public const string GroupToolName = "Group Tool";
    public const string HitToolName = "Hit Tool";
    public const string HoverToolName = "Hover Tool";
    public const string ImageExportToolName = "Image Export Tool";
    public const string MoveToolName = "Move Tool";
    public const string MultiLineToolName = "MultiLine Tool";
    public const string PanToolName = "Pan Tool";
    public const string PasteToolName = "Paste Tool";
    public const string PolygonToolName = "Polygon Tool";
    public const string RectangleToolName = "Rectangle Tool";
    public const string ScribbleToolName = "Scribble Tool";
    public const string SelectionToolName = "Selection Tool";
    public const string SendBackwardsToolName = "SendBackwards Tool";
    public const string SendForwardsToolName = "SendForwards Tool";
    public const string SendToBackToolName = "SendToBack Tool";
    public const string SendToFrontToolName = "SendToFront Tool";
    public const string TransformToolName = "Transform Tool";
    public const string UngroupToolName = "Ungroup Tool";
    public const string ZoomAreaToolName = "Zoom Area Tool";
    public const string ZoomInToolName = "Zoom In Tool";
    public const string ZoomOutToolName = "Zoom Out Tool";

    #endregion

    #region Fields

    private bool eventsEnabled = true;
    private bool controllerEnabled = true;

    private IModel mModel;
    private UndoManager mUndoManager;
    ITool activeTool;

    /// <summary>
    /// the View field
    /// </summary>
    private IView mView;
    protected CollectionBase<IMouseListener> mouseListeners;
    protected CollectionBase<IKeyboardListener> keyboardListeners;
    protected CollectionBase<IDragDropListener> dragdropListeners;
    private IDiagramControl parentControl;
    protected CollectionBase<ITool> registeredTools;
    protected CollectionBase<IActivity> registeredActivity;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:ControllerBase"/> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    public bool Enabled {
      get {
        return controllerEnabled;
      }
      set {
        controllerEnabled = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the currently active tool.  This can be 'null'!!!
    /// </summary>
    // ------------------------------------------------------------------
    public ITool ActiveTool {
      get {
        return this.activeTool;
      }
    }

    /// <summary>
    /// Gets or sets the parent control.
    /// </summary>
    /// <value>The parent control.</value>
    public IDiagramControl ParentControl {
      get { return parentControl; }
      protected internal set { parentControl = value; }
    }
    /// <summary>
    /// Gets the registered tools.
    /// </summary>
    /// <value>The tools.</value>
    public CollectionBase<ITool> Tools {
      get { return registeredTools; }
    }



    /// <summary>
    /// Gets the undo manager.
    /// </summary>
    /// <value>The undo manager.</value>
    public UndoManager UndoManager {
      get {
        return mUndoManager;
      }

    }

    /// <summary>
    /// Gets or sets the model
    /// </summary>
    /// <value></value>
    public IModel Model {
      get {
        return mModel;
      }
      set {
        AttachToModel(value);
      }
    }

    /// <summary>
    /// Gets or sets the view.
    /// </summary>
    /// <value>The view.</value>
    public IView View {
      get {
        return mView;
      }
      set {
        AttachToView(value);
      }
    }

    /// <summary>
    /// Attaches to the given view.
    /// </summary>
    /// <param name="view">The view.</param>
    private void AttachToView(IView view) {
      if (view == null)
        throw new ArgumentNullException();

      mView = view;
    }
    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    /// <summary>
    /// Default constructor.
    /// </summary>
    // ------------------------------------------------------------------
    protected ControllerBase(IDiagramControl surface) {
      //doesn't work if you supply a null reference
      if (surface == null) {
        throw new NullReferenceException(
            "The diagram control assigned to the controller " +
            "cannot be 'null'");
      }

      //create the undo/redo manager
      mUndoManager = new UndoManager(15);
      mUndoManager.OnHistoryChange += new EventHandler(
          mUndoManager_OnHistoryChange);

      #region Instantiation of listeners
      mouseListeners = new CollectionBase<IMouseListener>();
      keyboardListeners = new CollectionBase<IKeyboardListener>();
      dragdropListeners = new CollectionBase<IDragDropListener>();
      #endregion

      //keep a reference to the parent control
      parentControl = surface;

      AttachToSurface(parentControl);



      //Initialize the colorscheme
      ArtPalette.Init();

      #region Tools: the registration order matters!
      /*
             The order in in which the tools are added matters, at least 
             some of them.
               * The TransformTool should come before the HitTool and the 
                 MoveTool after the HitTool.
               * The order of the drawing tools does not matter.
               * It's also important to remark that the tools do not depend 
                 on the Model.
             */

      registeredTools = new CollectionBase<ITool>();

      this.AddTool(new TransformTool(TransformToolName));

      this.AddTool(new HitTool(HitToolName));

      this.AddTool(new MoveTool(MoveToolName));

      this.AddTool(new RectangleTool(RectangleToolName));

      this.AddTool(new ComplexRectangleTool(ComplexRectangleToolName));

      this.AddTool(new EllipseTool(EllipseToolName));

      this.AddTool(new SelectionTool(SelectionToolName));

      this.AddTool(new DragDropTool(DragDropToolName));

      this.AddTool(new ConnectionTool(ConnectionToolName));

      this.AddTool(new ConnectorMoverTool(ConnectorMoverToolName));

      this.AddTool(new GroupTool(GroupToolName));

      this.AddTool(new UngroupTool(UngroupToolName));

      this.AddTool(new SendToBackTool(SendToBackToolName));

      this.AddTool(new SendBackwardsTool(SendBackwardsToolName));

      this.AddTool(new SendForwardsTool(SendForwardsToolName));

      this.AddTool(new SendToFrontTool(SendToFrontToolName));

      this.AddTool(new HoverTool(HoverToolName));

      this.AddTool(new ContextTool(ContextToolName));

      this.AddTool(new CopyTool(CopyToolName));

      this.AddTool(new CutTool(CutToolName));

      this.AddTool(new PasteTool(PasteToolName));

      this.AddTool(new DeleteTool(DeleteToolName));

      this.AddTool(new ScribbleTool(ScribbleToolName));

      this.AddTool(new PolygonTool(PolygonToolName));

      this.AddTool(new MultiLineTool(MultiLineToolName));

      this.AddTool(new AlignBottomEdgesTool(AlignBottomEdgesToolName));

      this.AddTool(
          new AlignCentersHorizontallyTool(AlignCentersHorizToolName));

      this.AddTool(
          new AlignCentersVerticallyTool(AlignCentersVertToolName));

      this.AddTool(new AlignLeftEdgesTool(AlignLeftEdgesToolName));

      this.AddTool(new AlignRightEdgesTool(AlignRightEdgesToolName));

      this.AddTool(new AlignTopEdgesTool(AlignTopEdgesToolName));

      this.AddTool(new ZoomAreaTool(ZoomAreaToolName));

      this.AddTool(new ZoomInTool(ZoomInToolName));

      this.AddTool(new ZoomOutTool(ZoomOutToolName));

      this.AddTool(new PanTool(PanToolName));

      this.AddTool(new ImageExportTool(ImageExportToolName));

      #endregion

      #region Hotkeys
      HotKeys keys = new HotKeys(this);
      this.keyboardListeners.Add(keys);
      #endregion

      #region Activities
      // This is in a way a waste of memory; the layouts should not 
      // necessarily be loaded before they are actually requested. 
      // You could register only the (string) names instead.
      // But for just a few algorithms this is OK and the advantage 
      // of this registration is that one can register actions from 
      // outside the library, in the hosting form for example.
      registeredActivity = new CollectionBase<IActivity>();
      AddActivity(new RandomLayout(this));
      AddActivity(new FruchtermanReingoldLayout(this));
      AddActivity(new StandardTreeLayout(this));
      AddActivity(new RadialTreeLayout(this));
      AddActivity(new BalloonTreeLayout(this));
      AddActivity(new ForceDirectedLayout(this));
      #endregion
    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Attaches the given model to the controller.
    /// </summary>
    /// <param name="model">IModel</param>
    // ------------------------------------------------------------------
    protected virtual void AttachToModel(IModel model) {
      if (model == null)
        throw new ArgumentNullException();

      mModel = model;
      mModel.OnEntityAdded +=
          new EventHandler<EntityEventArgs>(mModel_OnEntityAdded);
      mModel.OnEntityRemoved +=
          new EventHandler<EntityEventArgs>(mModel_OnEntityRemoved);
      mModel.OnAmbienceChanged +=
          new EventHandler<AmbienceEventArgs>(mModel_OnAmbienceChanged);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Passes the OnAmbienceChanged event on.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">AmbienceEventArgs</param>
    // ------------------------------------------------------------------
    void mModel_OnAmbienceChanged(
        object sender,
        AmbienceEventArgs e) {
      RaiseOnAmbienceChanged(e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Passes the OnEntityRemoved event on.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EntityEventArgs</param>
    // ------------------------------------------------------------------
    void mModel_OnEntityRemoved(
        object sender,
        EntityEventArgs e) {
      RaiseOnEntityRemoved(e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Passes the OnEntityAdded event on.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">EntityEventArgs</param>
    // ------------------------------------------------------------------
    void mModel_OnEntityAdded(
        object sender,
        EntityEventArgs e) {
      RaiseOnEntityAdded(e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Bubbles the OnHistoryChange event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    void mUndoManager_OnHistoryChange(object sender, EventArgs e) {
      RaiseOnHistoryChange();
    }

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Activates the text editor for the given text provider.
    /// </summary>
    /// <param name="textProvider">ITextProvider</param>
    /// <returns>bool: True if sucessful, false if not.</returns>
    // ------------------------------------------------------------------
    public abstract bool ActivateTextEditor(ITextProvider textProvider);

    // ------------------------------------------------------------------
    /// <summary>
    /// Changes the paint style of the selected entities.
    /// </summary>
    /// <param name="paintStyle">IPaintStyle</param>
    // ------------------------------------------------------------------
    public void ChangeStyle(IPaintStyle paintStyle) {

      // Note that you need a copy of the selected item otherwise the 
      // undo/redo will fail once the selection has changed
      FillStyleCommand cmd = new FillStyleCommand(
          this,
          this.Model.Selection.SelectedItems.Copy(),
          paintStyle);

      this.UndoManager.AddUndoCommand(cmd);
      cmd.Redo();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Changes the pen style  of the selected entities.
    /// </summary>
    /// <param name="penStyle">The pen style.</param>
    // ------------------------------------------------------------------
    public void ChangeStyle(IPenStyle penStyle) {
      PenStyleCommand cmd = new PenStyleCommand(
          this,
          this.Model.Selection.SelectedItems.Copy(),
          penStyle);

      this.UndoManager.AddUndoCommand(cmd);

      cmd.Redo();
    }

    #region Event Raisers

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnShowContextMenu event
    /// </summary>
    /// <param name="e">EntityMenuEventArgs</param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnShowContextMenu(EntityMenuEventArgs e) {
      EventHandler<EntityMenuEventArgs> handler = OnShowContextMenu;
      if (handler != null) {
        handler(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnHistory change.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual void RaiseOnHistoryChange() {
      EventHandler<HistoryChangeEventArgs> handler = OnHistoryChange;
      if (handler != null) {
        handler(this, new HistoryChangeEventArgs(
            this.UndoManager.RedoText,
            this.UndoManager.UndoText));
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the <see cref="OnToolDeactivate"/> event.
    /// </summary>
    /// <param name="e">ConnectionCollection event argument</param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnToolDeactivate(ToolEventArgs e) {
      EventHandler<ToolEventArgs> handler = OnToolDeactivate;
      if (handler != null) {
        handler(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the <see cref="OnToolActivate"/> event
    /// </summary>
    /// <param name="e">ConnectionCollection event argument</param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnToolActivate(ToolEventArgs e) {
      EventHandler<ToolEventArgs> handler = OnToolActivate;
      if (handler != null) {
        handler(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnShowSelectionProperties event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.SelectionEventArgs"/> 
    /// instance containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnShowSelectionProperties(SelectionEventArgs e) {
      EventHandler<SelectionEventArgs> handler = OnShowSelectionProperties;
      if (handler != null) {
        handler(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the <see cref="OnMouseDown "/> event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    protected virtual void RaiseOnMouseDown(MouseEventArgs e) {
      if (OnMouseDown != null)
        OnMouseDown(this, e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the <see cref="OnEntityAdded"/> event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    protected virtual void RaiseOnEntityAdded(EntityEventArgs e) {
      EventHandler<EntityEventArgs> handler = OnEntityAdded;
      if (handler != null) {
        handler(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the <see cref="OnEntityRemoved"/> event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    protected virtual void RaiseOnEntityRemoved(EntityEventArgs e) {
      EventHandler<EntityEventArgs> handler = OnEntityRemoved;
      if (handler != null) {
        handler(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the <see cref="OnAmbienceChanged"/> event.
    /// </summary>
    /// <param name="e">AmbienceEventArgs</param>
    // ------------------------------------------------------------------
    protected virtual void RaiseOnAmbienceChanged(AmbienceEventArgs e) {
      EventHandler<AmbienceEventArgs> handler = OnAmbienceChanged;
      if (handler != null) {
        handler(this, e);
      }
    }

    #endregion

    #region Tool (de)activation methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Deactivates the given tool.
    /// </summary>
    /// <param name="tool">a registered ITool</param>
    /// <returns>bool: True if successful.</returns>
    // ------------------------------------------------------------------
    public bool DeactivateTool(ITool tool) {
      bool flag = false;
      if (tool != null && tool.Enabled && tool.IsActive) {
        //IEnumerator iEnumerator = tools.GetEnumerator();
        //Tool tool2 = null;
        //while (iEnumerator.MoveNext())
        //{
        //    tool2 = iEnumerator.Current is Tool;
        //    if (tool2 != null && tool2 != tool)
        //    {
        //        tool2.ToolDeactivating(tool);
        //    }
        //}
        flag = tool.DeactivateTool();
        if (flag && eventsEnabled) {
          RaiseOnToolDeactivate(new ToolEventArgs(tool));
        }
      }
      return flag;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Deactivates all tools.
    /// </summary>
    /// <returns>bool: True if successful.</returns>
    // ------------------------------------------------------------------
    public bool DeactivateAllTools() {
      bool successful = true;

      // If the deactivation of any tool returns false, then we will
      // return false.
      foreach (ITool tool in this.Tools) {
        successful = successful & this.DeactivateTool(tool);
      }
      this.activeTool = null;
      return successful;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Activates the tool with the given name
    /// </summary>
    /// <param name="toolName"></param>
    // ------------------------------------------------------------------
    public void ActivateTool(string toolName) {
      if (!controllerEnabled)
        return;

      //using anonymous method here
      Predicate<ITool> predicate = delegate(ITool tool) {
        if (tool.Name.ToLower() == toolName.ToLower())//not case sensitive
          return true;
        else
          return false;
      };

      // First deactivate the current tool.
      if (this.activeTool != null) {
        this.activeTool.DeactivateTool();
      }
      ITool foundTool = this.registeredTools.Find(predicate);
      ActivateTool(foundTool);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Suspends all tools
    /// </summary>
    // ------------------------------------------------------------------
    public void SuspendAllTools() {
      foreach (ITool tool in this.Tools) {
        tool.IsSuspended = true;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Unsuspends all tools.
    /// </summary>
    // ------------------------------------------------------------------
    public void UnsuspendAllTools() {
      foreach (ITool tool in this.Tools) {
        tool.IsSuspended = false;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Activates a registered tool
    /// </summary>
    /// <param name="tool">a registered ITool</param>
    /// <returns>bool: Returns if the activation was successful.  True is
    /// returned if it was, false if not.</returns>
    // ------------------------------------------------------------------
    private bool ActivateTool(ITool tool) {
      if (!controllerEnabled)
        return false;
      bool flag = false;
      if (tool != null && tool.CanActivate) {
        flag = tool.ActivateTool();
        this.activeTool = tool;
        if (flag && eventsEnabled) {
          RaiseOnToolActivate(new ToolEventArgs(tool));
        }
      }
      return flag;
    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds the given tool.
    /// </summary>
    /// <param name="tool">The tool.</param>
    // ------------------------------------------------------------------
    public void AddTool(ITool tool) {
      tool.Controller = this;
      // Add the tool to the collection even if it doesn't attach to 
      // anything (yet)
      registeredTools.Add(tool);

      IMouseListener mouseTool = null;
      if ((mouseTool = tool as IMouseListener) != null)
        mouseListeners.Add(mouseTool);

      IKeyboardListener keyboardTool = null;
      if ((keyboardTool = tool as IKeyboardListener) != null)
        keyboardListeners.Add(keyboardTool);

      IDragDropListener dragdropTool = null;
      if ((dragdropTool = tool as IDragDropListener) != null)
        dragdropListeners.Add(dragdropTool);

      // Watch when the tool is (de)activated so we can pass it on.
      tool.OnToolActivate +=
          new EventHandler<ToolEventArgs>(AddedTool_OnToolActivate);

      tool.OnToolDeactivate +=
          new EventHandler<ToolEventArgs>(AddedTool_OnToolDeactivate);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when an added tool is deactivated.  The event is passed on
    /// by calling RaiseOnToolDeactivate.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">ToolEventArgs</param>
    // ------------------------------------------------------------------
    protected void AddedTool_OnToolDeactivate(object sender, ToolEventArgs e) {
      ITool nextActiveToolInList = null;
      if (this.activeTool == e.Properties) {
        foreach (ITool tool in this.Tools) {
          if (tool.IsActive) {
            nextActiveToolInList = tool;
            break;
          }
        }
        activeTool = nextActiveToolInList;
      }
      this.RaiseOnToolDeactivate(e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when an added tool is activated.  The event is passed on
    /// by calling RaiseOnToolActivate.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">ToolEventArgs</param>
    // ------------------------------------------------------------------
    protected void AddedTool_OnToolActivate(object sender, ToolEventArgs e) {
      this.RaiseOnToolActivate(e);
    }

    #region Activity

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds the given activity to the controller.
    /// </summary>
    /// <param name="activity">The activity.</param>
    // ------------------------------------------------------------------
    public void AddActivity(IActivity activity) {

      registeredActivity.Add(activity);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Runs the given activity.
    /// </summary>
    /// <param name="activity">The activity.</param>
    // ------------------------------------------------------------------
    protected void RunActivity(IActivity activity) {
      if (activity == null) return;
      PrepareActivity(activity);

      activity.Run();

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Runs the given activity.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <param name="milliseconds">The milliseconds.</param>
    // ------------------------------------------------------------------
    protected void RunActivity(IActivity activity, int milliseconds) {
      if (activity == null) return;
      PrepareActivity(activity);
      activity.Run(milliseconds);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Prepares the activity.
    /// </summary>
    /// <param name="activity">The activity.</param>
    // ------------------------------------------------------------------
    private void PrepareActivity(IActivity activity) {
      if (activity is IAction)
        (activity as IAction).Model = this.Model;
      if (activity is ILayout) {
        (activity as ILayout).Bounds = parentControl.ClientRectangle;
        (activity as ILayout).Center = new PointF(parentControl.ClientRectangle.Width / 2, parentControl.ClientRectangle.Height / 2);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Runs the activity with the name specified.  If no activity could
    /// by found with the given name then an exception is thrown.
    /// </summary>
    /// <param name="activityName">string: The name of the 
    /// activity to run.</param>
    // ------------------------------------------------------------------
    public void RunActivity(string activityName) {
      if (!controllerEnabled)
        return;

      this.View.CurrentCursor = Cursors.WaitCursor;
      controllerEnabled = false;

      IActivity foundActivity = FindActivity(activityName);
      if (foundActivity != null) {
        RunActivity(foundActivity);
      }

      controllerEnabled = true;
      this.View.CurrentCursor = Cursors.Default;

      // After returning the canvas back to "normal", if the activity
      // wasn't found throw an exception (as specified by IController).
      if (foundActivity == null) {
        throw new Exception("Activity '" + activityName +
            "' could not be found.");
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Finds the activity with the given name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>IActivity</returns>
    // ------------------------------------------------------------------
    protected IActivity FindActivity(string name) {
      //using anonymous method here
      Predicate<IActivity> predicate = delegate(IActivity activity) {
        if (activity.Name.ToLower() == name.ToLower())//not case sensitive
          return true;
        else
          return false;
      };
      return this.registeredActivity.Find(predicate);

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Runs the given activity for the specified time span.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="milliseconds">The milliseconds.</param>
    // ------------------------------------------------------------------
    public void RunActivity(string name, int milliseconds) {
      if (!controllerEnabled)
        return;

      IActivity foundActivity = FindActivity(name);
      if (foundActivity != null)
        RunActivity(foundActivity, milliseconds);
    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Attaches this controller to the surface.
    /// </summary>
    /// <param name="surface">The surface.</param>
    // ------------------------------------------------------------------
    protected virtual void AttachToSurface(IDiagramControl surface) {
      #region Mouse events
      surface.MouseDown += new MouseEventHandler(OnSurfaceMouseDown);
      surface.MouseUp += new MouseEventHandler(OnSurfaceMouseUp);
      surface.MouseMove += new MouseEventHandler(OnSurfaceMouseMove);
      surface.MouseHover += new EventHandler(OnSurfaceMouseHover);
      surface.MouseWheel += new MouseEventHandler(surface_MouseWheel);
      #endregion

      #region Keyboard events
      surface.KeyDown += new KeyEventHandler(surface_KeyDown);
      surface.KeyUp += new KeyEventHandler(surface_KeyUp);
      surface.KeyPress += new KeyPressEventHandler(surface_KeyPress);
      #endregion

      #region Dragdrop events
      surface.DragDrop += new DragEventHandler(surface_DragDrop);
      surface.DragEnter += new DragEventHandler(surface_DragEnter);
      surface.DragLeave += new EventHandler(surface_DragLeave);
      surface.DragOver += new DragEventHandler(surface_DragOver);
      surface.GiveFeedback +=
          new GiveFeedbackEventHandler(surface_GiveFeedback);
      #endregion
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the MouseWheel event of the surface control.
    /// <remarks>In the WinForm implementation this routine is not called 
    /// because it gives some flickering effects; the hotkeys are 
    /// implemented in the overriden OnMouseWheel method instead.</remarks>
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    void surface_MouseWheel(object sender, MouseEventArgs e) {
      Point p = View.Origin;
      SizeF magnification = View.Magnification;
      int newValue = 0;

      if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
        #region Zooming

        SizeF s = magnification;

        // If zooming in, e.Delta is < 0 so a value of 1.1 is used to
        // offset the current magnification by.  One mouse wheel 
        // position on my PC corresponds to a delta of 120 (positive 
        // for zooming out, neg for zooming in).
        float alpha = e.Delta > 0 ? 1.1F : 0.9F;
        View.Magnification = new SizeF(
            s.Width * alpha,
            s.Height * alpha);

        float w = (float)parentControl.AutoScrollPosition.X /
            (float)parentControl.AutoScrollMinSize.Width;

        float h = (float)parentControl.AutoScrollPosition.Y /
            (float)parentControl.AutoScrollMinSize.Height;

        // Resize the scrollbars proportionally to keep the actual 
        // canvas constant.
        //s = new SizeF(
        //    parentControl.AutoScrollMinSize.Width * alpha,
        //    parentControl.AutoScrollMinSize.Height * alpha);

        //parentControl.AutoScrollMinSize = Size.Round(s);
        RectangleF pageBounds = Model.CurrentPage.Bounds;
        pageBounds.Inflate(s);
        SizeF deltaSize = new SizeF(
            pageBounds.Width - parentControl.ClientRectangle.Width,
            pageBounds.Height - parentControl.ClientRectangle.Height);

        if ((deltaSize.Width > 0) && (deltaSize.Height > 0)) {
          parentControl.AutoScrollMinSize = Size.Round(deltaSize);
        }

        //Point v = Origin;
        //v.Offset(
        //    Convert.ToInt32((alpha - 1) * v.X), 
        //    Convert.ToInt32((alpha - 1) * v.Y));
        //v.X = (int)Math.Round((double)(v.X - alpha));
        //v.Y = (int)Math.Round((double)(v.Y - alpha));
        //Origin = v;

        #endregion
      } else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) {
        #region Pan horizontal
        newValue = p.X - Math.Sign(e.Delta) * 20;
        if (newValue > 0)
          View.Origin = new Point(newValue, p.Y);
        else
          View.Origin = new Point(0, p.Y);

        #endregion
      } else {
        #region Default vertical scroll
        newValue = View.Origin.Y -
            Math.Sign(e.Delta) * 20;

        if (newValue > 0)
          View.Origin = new Point(
              View.Origin.X,
              newValue);
        else
          View.Origin = new Point(
              View.Origin.X,
              0);
        #endregion
      }

      this.parentControl.AutoScrollPosition = View.Origin;
      HandledMouseEventArgs eventargs = e as HandledMouseEventArgs;
      if (eventargs != null)
        eventargs.Handled = true;
      View.Invalidate();
    }

    #region DragDrop event handlers

    /// <summary>
    /// Handles the GiveFeedback event of the surface control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.GiveFeedbackEventArgs"/> instance containing the event data.</param>
    void surface_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
      if (!controllerEnabled) {
        return;
      }

      foreach (IDragDropListener listener in dragdropListeners) {
        listener.GiveFeedback(e);
      }
    }
    /// <summary>
    /// Handles the DragOver event of the surface control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
    void surface_DragOver(object sender, DragEventArgs e) {
      if (!controllerEnabled) {
        return;
      }

      foreach (IDragDropListener listener in dragdropListeners) {
        listener.OnDragOver(e);
      }
    }

    /// <summary>
    /// Handles the DragLeave event of the surface control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    void surface_DragLeave(object sender, EventArgs e) {
      if (!controllerEnabled) {
        return;
      }

      foreach (IDragDropListener listener in dragdropListeners) {
        listener.OnDragLeave(e);
      }
    }

    /// <summary>
    /// Handles the DragEnter event of the surface control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
    void surface_DragEnter(object sender, DragEventArgs e) {
      if (!controllerEnabled) {
        return;
      }

      foreach (IDragDropListener listener in dragdropListeners) {
        listener.OnDragEnter(e);
      }
    }

    /// <summary>
    /// Handles the DragDrop event of the surface control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
    void surface_DragDrop(object sender, DragEventArgs e) {
      if (!controllerEnabled) {
        return;
      }

      foreach (IDragDropListener listener in dragdropListeners) {
        listener.OnDragDrop(e);
      }
    }
    #endregion

    #region Keyboard event handlers
    void surface_KeyPress(object sender, KeyPressEventArgs e) {

      foreach (IKeyboardListener listener in keyboardListeners) {
        listener.KeyPress(e);
      }

      foreach (IDiagramEntity entity in this.Model.Selection.SelectedItems) {
        if (entity is IKeyboardListener) {
          (entity as IKeyboardListener).KeyPress(e);
        }
      }
    }

    void surface_KeyUp(object sender, KeyEventArgs e) {

      foreach (IKeyboardListener listener in keyboardListeners) {
        listener.KeyUp(e);
      }

      foreach (IDiagramEntity entity in this.Model.Selection.SelectedItems) {
        if (entity is IKeyboardListener) {
          (entity as IKeyboardListener).KeyUp(e);
        }
      }
    }

    void surface_KeyDown(object sender, KeyEventArgs e) {
      foreach (IKeyboardListener listener in keyboardListeners) {
        listener.KeyDown(e);
      }

      foreach (IDiagramEntity entity in this.Model.Selection.SelectedItems) {
        if (entity is IKeyboardListener) {
          (entity as IKeyboardListener).KeyDown(e);
        }
      }
    }

    #endregion

    #region Mouse event handlers
    /// <summary>
    /// Implements the observer pattern for the mouse hover event, 
    /// communicating the event to all listeners implementing the 
    /// necessary interface.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance 
    /// containing the event data.</param>
    protected virtual void OnSurfaceMouseHover(object sender, EventArgs e) {
      //if (eventsEnabled)
      //    RaiseOnMouseHover(e);
      //if (!controllerEnabled)
      //    return;  
    }
    /// <summary>
    /// Implements the observer pattern for the mouse down event, 
    /// communicating the event to all listeners implementing the 
    /// necessary interface.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    protected virtual void OnSurfaceMouseDown(
        object sender,
        MouseEventArgs e) {
      #region Coordinates logic
      // Get a point adjusted by the current scroll position and zoom factor            
      Point p = Point.Round(
          this.View.ViewToWorld(this.View.DeviceToView(e.Location)));

      HandledMouseEventArgs ce =
          new HandledMouseEventArgs(
          e.Button,
          e.Clicks,
          p.X,
          p.Y,
          e.Delta);
      #endregion

      if (eventsEnabled)
        RaiseOnMouseDown(ce);
      if (!controllerEnabled || ce.Handled)
        return;
      this.parentControl.Focus();

      //(parentControl as Win.DiagramControl).toolTip.Show("Yihaaa", parentControl as Win.DiagramControl, ce.Location);

      //this selection process will work independently of the tools because
      //some tools need the current selection or hit entity
      //On the other hand, when drawing a simple rectangle for example the selection
      //should be off, so there is an overhead.
      //Selection.CollectEntitiesAt(e.Location);

      //raise the event to give the host the opportunity to show the properties of the selected item(s)
      //Note that if the selection is empty the property grid will show 'nothing'.
      RaiseOnShowSelectionProperties(new SelectionEventArgs(this.Model.Selection.SelectedItems.ToArray()));

      foreach (IMouseListener listener in mouseListeners) {
        if (listener.MouseDown(ce))
          break;
      }
    }
    /// <summary>
    /// Handles the MouseMove event of the surface control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    protected virtual void OnSurfaceMouseMove(object sender, MouseEventArgs e) {
      if (!controllerEnabled)
        return;

      #region Coordinates logic
      // Get a point adjusted by the current scroll position and zoom 
      // factor.
      //Point p = new Point(e.X - parentControl.AutoScrollPosition.X, e.Y - parentControl.AutoScrollPosition.Y);
      Point p = Point.Round(this.View.ViewToWorld(
          this.View.DeviceToView(e.Location)));

      MouseEventArgs ce = new MouseEventArgs(
          e.Button,
          e.Clicks,
          p.X,
          p.Y,
          e.Delta);

      #endregion
      foreach (IMouseListener listener in mouseListeners) {
        listener.MouseMove(ce);
      }
    }

    /// <summary>
    /// Handles the MouseUp event of the surface control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    protected virtual void OnSurfaceMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
      if (!controllerEnabled)
        return;
      #region Coordinates logic
      // Get a point adjusted by the current scroll position and zoom factor
      //Point p = new Point(e.X - parentControl.AutoScrollPosition.X, e.Y - parentControl.AutoScrollPosition.Y);
      Point p = Point.Round(this.View.ViewToWorld(this.View.DeviceToView(e.Location)));
      MouseEventArgs ce = new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta);
      #endregion
      foreach (IMouseListener listener in mouseListeners) {
        listener.MouseUp(ce);
      }
    }


    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Undo of the last action
    /// </summary>
    /// <remarks>Calling this on a class level will call the Undo method 
    /// of the last ICommand in the stack.</remarks>
    // ------------------------------------------------------------------
    public void Undo() {
      // Reset the tracker or show the tracker after the undo operation 
      // since the undo does not take care of it
      this.View.ResetTracker();
      mUndoManager.Undo();
      this.View.ShowTracker();
      this.View.Invalidate();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Performs the actual action or redo in case the actions was undone 
    /// before.
    /// </summary>
    /// <remarks>Calling this on a class level will call the Redo method 
    /// of the last ICommand in the stack.</remarks>
    // ------------------------------------------------------------------
    public void Redo() {
      mUndoManager.Redo();
      this.View.ShowTracker();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Selects all entities on the current page.  The selection is 
    /// cleared first.
    /// </summary>
    // ------------------------------------------------------------------
    public void SelectAll() {
      this.View.ResetTracker();
      this.Model.Selection.Clear();
      this.Model.Selection.SelectedItems = this.Model.CurrentPage.Entities;
      this.View.ShowTracker();
      this.View.Invalidate();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Navigates to the next page.  Nothing is performed if the last page
    /// is currently selected and 'wrap' is false.  If 'wrap' is true,
    /// then the first page is selected.
    /// </summary>
    /// <param name="wrap">bool: Specifies if the collection is wrapped
    /// when the end is reached.</param>
    // ------------------------------------------------------------------
    public void GoForward(bool wrap) {
      // We can't go anywhere if there's only one page!
      if (Model.Pages.Count == 1) {
        return;
      }

      int index = Model.Pages.IndexOf(Model.CurrentPage);

      int newIndex = 0;  // The index of the page to select.

      if (index >= (Model.Pages.Count - 1)) {
        // The last page is currently active, so if 'wrap' is
        // false then just return.
        if (wrap == false) {
          return;
        }

        // Otherwise, if 'wrap' is true then we want the first page
        // in the collection to be active.
        newIndex = 0;
      } else {
        newIndex = index + 1;
      }

      DeactivateAllTools();
      View.HideTracker();  // Just in case there are selected items.
      Model.SetCurrentPage(Model.Pages[newIndex]);
      View.Invalidate();
    }

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
    public void GoBack(bool wrap) {
      // We can't go anywhere if there's only one page!
      if (Model.Pages.Count == 1) {
        return;
      }

      int index = Model.Pages.IndexOf(Model.CurrentPage);

      int newIndex = 0;  // The index of the page to select.

      if (index == 0) {
        // The first page is currently active, so if 'wrap' is
        // false then just return.
        if (wrap == false) {
          return;
        }

        // Otherwise, since 'wrap' is true then we want the last page
        // in the collection to be active.
        newIndex = Model.Pages.Count - 1;
      } else {
        newIndex = index - 1;
      }

      DeactivateAllTools();
      View.HideTracker();  // Just in case there are selected items.
      Model.SetCurrentPage(Model.Pages[newIndex]);
      View.Invalidate();
    }
    #endregion

  }
}
