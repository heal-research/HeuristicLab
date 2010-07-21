using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// Abstract base class for the various diagram control representations 
  /// (WebForm and WinForm). 
  /// </summary>
  [ToolboxItem(false)]
  public class DiagramControlBase :
      ScrollableControl,
      ISupportInitialize,
      IDiagramControl {
    #region Constants
    protected const string constGeneral = "General";
    #endregion

    #region Events
    /// <summary>
    /// Occurs when the something got selected and the properties of it can/should be shown.
    /// </summary>
    [Category(constGeneral),
    Description("Occurs when the something got selected and the " +
        "properties of it can/should be shown."),
    Browsable(true)]
    public event EventHandler<SelectionEventArgs> OnShowSelectionProperties;
    /// <summary>
    /// Occurs when the Properties have changed
    /// </summary>
    [Category(constGeneral),
    Description("Occurs when the properties have changed."),
    Browsable(true)]
    public event EventHandler<PropertiesEventArgs> OnShowDocumentProperties;
    /// <summary>
    /// Occurs when the properties of the canvas are exposed.
    /// </summary>
    public event EventHandler<SelectionEventArgs> OnShowCanvasProperties;
    /// <summary>
    /// Occurs when an entity is added.
    /// <remarks>This event usually is bubbled from one of the layers</remarks>
    /// </summary>
    [Category(constGeneral), Description("Occurs when an entity is added."), Browsable(true)]
    public event EventHandler<EntityEventArgs> OnEntityAdded;
    /// <summary>
    /// Occurs when an entity is removed.
    /// <remarks>This event usually is bubbled from one of the layers</remarks>
    /// </summary>
    [Category(constGeneral), Description("Occurs when an entity is removed."), Browsable(true)]
    public event EventHandler<EntityEventArgs> OnEntityRemoved;
    /// <summary>
    /// Occurs on opening a file
    /// </summary>
    [Category(constGeneral), Description("Occurs when the control will open a file."), Browsable(true)]
    public event EventHandler<FileEventArgs> OnOpeningDiagram;
    /// <summary>
    /// Occurs when a file was opened
    /// </summary>
    [Category(constGeneral), Description(" Occurs when a file was opened."), Browsable(true)]
    public event EventHandler<FileEventArgs> OnDiagramOpened;
    /// <summary>
    /// Occurs when the history has changed in the undo/redo mechanism
    /// </summary>
    public event EventHandler<HistoryChangeEventArgs> OnHistoryChange;
    /// <summary>
    /// Occurs before saving a diagram
    /// </summary>
    [Category(constGeneral), Description("Occurs before saving a diagram."), Browsable(true)]
    public event EventHandler<FileEventArgs> OnSavingDiagram;
    /// <summary>
    /// Occurs after saving a diagram
    /// </summary>
    [Category(constGeneral), Description("Occurs after saving a diagram."), Browsable(true)]
    public event EventHandler<FileEventArgs> OnDiagramSaved;
    /// <summary>
    /// Occurs when the control resets the document internally, usually through the Ctrl+N hotkey.
    /// </summary>
    [Category(constGeneral), Description("Occurs when the control resets the document internally, usually through the Ctrl+N hotkey."), Browsable(true)]
    public event EventHandler OnNewDiagram;

    #region Event raisers

    /// <summary>
    /// Raises the OnShowCanvasProperties event.
    /// </summary>
    protected void RaiseOnShowCanvasProperties(SelectionEventArgs e) {
      EventHandler<SelectionEventArgs> handler = OnShowCanvasProperties;
      if (handler != null) {
        handler(this, e);
      }
    }
    /// <summary>
    /// Raises the OnNewDiagram event.
    /// </summary>
    protected void RaiseOnNewDiagram() {
      EventHandler handler = OnNewDiagram;
      if (handler != null) {
        handler(this, EventArgs.Empty);
      }
    }
    /// <summary>
    /// Raises the OnHistory change.
    /// </summary>
    protected void RaiseOnHistoryChange(HistoryChangeEventArgs e) {
      EventHandler<HistoryChangeEventArgs> handler = OnHistoryChange;
      if (handler != null) {
        handler(this, e);
      }
    }
    /// <summary>
    /// Raises the <see cref="OnShowDocumentProperties"/> event
    /// </summary>
    /// <param name="e">Properties event argument</param>
    protected virtual void RaiseOnShowDocumentProperties(PropertiesEventArgs e) {
      EventHandler<PropertiesEventArgs> handler = OnShowDocumentProperties;
      if (handler != null) {
        handler(this, e);
      }
    }
    /// <summary>
    /// Raises the OnSavingDiagram event
    /// </summary>
    /// <param name="filePath"></param>
    public void RaiseOnSavingDiagram(string filePath) {
      if (OnSavingDiagram != null)
        OnSavingDiagram(this, new FileEventArgs(new System.IO.FileInfo(filePath)));
    }
    /// <summary>
    /// Raises the OnShowSelectionProperties event.
    /// </summary>
    /// <param name="e">The <see cref="T:Netron.Diagramming.Core.SelectionEventArgs"/> instance containing the event data.</param>
    protected virtual void RaiseOnShowSelectionProperties(SelectionEventArgs e) {
      EventHandler<SelectionEventArgs> handler = OnShowSelectionProperties;
      if (handler != null) {
        handler(this, e);
      }
    }
    /// <summary>
    /// Raises the <see cref="OnEntityAdded"/> event.
    /// </summary>
    /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
    protected virtual void RaiseOnEntityAdded(EntityEventArgs e) {
      EventHandler<EntityEventArgs> handler = OnEntityAdded;
      if (handler != null) {
        handler(this, e);
      }
    }
    /// <summary>
    /// Raises the <see cref="OnEntityRemoved"/>.
    /// </summary>
    /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
    protected virtual void RaiseOnEntityRemoved(EntityEventArgs e) {
      EventHandler<EntityEventArgs> handler = OnEntityRemoved;
      if (handler != null) {
        handler(this, e);
      }
    }
    /// <summary>
    /// Raises the OnDiagramSaved event
    /// </summary>
    /// <param name="filePath"></param>
    public void RaiseOnDiagramSaved(string filePath) {
      if (OnDiagramSaved != null)
        OnDiagramSaved(this, new FileEventArgs(new System.IO.FileInfo(filePath)));
    }
    /// <summary>
    /// Raises the OnOpeningDiagram event
    /// </summary>
    public void RaiseOnOpeningDiagram(string filePath) {
      if (OnOpeningDiagram != null)
        OnOpeningDiagram(this, new FileEventArgs(new System.IO.FileInfo(filePath)));
    }
    /// <summary>
    /// Raises the OnDiagramOpened event
    /// </summary>
    public void RaiseOnDiagramOpened(string filePath) {
      if (OnDiagramOpened != null)
        OnDiagramOpened(this, new FileEventArgs(new System.IO.FileInfo(filePath)));
    }
    #endregion


    #endregion

    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// The page settings for printing the diagram.
    /// </summary>
    // ------------------------------------------------------------------
    protected PageSettings mPageSettings;

    // ------------------------------------------------------------------
    /// <summary>
    /// The filename to use when saving the diagram.
    /// </summary>
    // ------------------------------------------------------------------
    protected string mFileName = "";

    // ------------------------------------------------------------------
    /// <summary>
    /// Collection of files that were opened.  The collection can be
    /// saved to disk by calling 'SaveRecentFiles(string path)'.
    /// </summary>
    // ------------------------------------------------------------------
    protected ArrayList myRecentFiles = new ArrayList();

    // ------------------------------------------------------------------
    /// <summary>
    /// the view
    /// </summary>
    // ------------------------------------------------------------------
    protected IView mView;

    // ------------------------------------------------------------------
    /// <summary>
    /// the controller
    /// </summary>
    // ------------------------------------------------------------------
    protected IController mController;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Document field
    /// </summary>
    // ------------------------------------------------------------------
    protected Document mDocument;
    protected bool mEnableAddConnection = true;


    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the page settings for the entire document.
    /// </summary>
    // ------------------------------------------------------------------
    public PageSettings PageSettings {
      get {
        return mPageSettings;
      }
      set {
        mPageSettings = value;

        // PageSettings has the page size in hundreths of an inch and
        // the view requires mils (thousandths of an inch).
        mView.PageSize = new Size(
            mPageSettings.PaperSize.Width * 10,
            mPageSettings.PaperSize.Height * 10);
        mView.Landscape = mPageSettings.Landscape;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if all shape's connectors are shown.
    /// </summary>
    // ------------------------------------------------------------------
    public bool ShowConnectors {
      get {
        return this.mView.ShowConnectors;
      }
      set {
        this.mView.ShowConnectors = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the filename to save the diagram as.
    /// </summary>
    // ------------------------------------------------------------------
    public string FileName {
      get {
        return mFileName;
      }
      set {
        mFileName = value;
      }
    }

    /// <summary>
    /// Pans the diagram.
    /// </summary>
    /// <value>The pan.</value>
    [Browsable(false)]
    public Point Origin {
      get { return this.Controller.View.Origin; }
      set { this.Controller.View.Origin = value; }
    }
    /// <summary>
    /// Gets or sets the magnification/zoom.
    /// </summary>
    /// <value>The magnification.</value>
    [Browsable(false)]
    public SizeF Magnification {
      get { return this.Controller.View.Magnification; }
      set { this.Controller.View.Magnification = value; }
    }

    /// <summary>
    /// Gets the selected items.
    /// </summary>
    /// <value>The selected items.</value>
    [Browsable(false)]
    public CollectionBase<IDiagramEntity> SelectedItems {
      get { return this.Controller.Model.Selection.SelectedItems; }
    }

    /// <summary>
    /// Gets or sets whether to show the rulers.
    /// </summary>
    /// <value><c>true</c> to show the rulers; otherwise, <c>false</c>.</value>
    [Browsable(true), Description("Gets or sets whether to show the rulers.")]
    public bool ShowRulers {
      get { return View.ShowRulers; }
      set { View.ShowRulers = value; }
    }

    /// <summary>
    /// Gets or sets whether a connection can be added.
    /// </summary>
    /// <value><c>true</c> if [enable add connection]; otherwise, <c>false</c>.</value>
    [Browsable(true), Description("Gets or sets whether the user can add new connections to the diagram."), Category("Interactions")]
    public bool EnableAddConnection {
      get { return mEnableAddConnection; }
      set { mEnableAddConnection = value; }
    }

    #endregion

    #region Constructor
    /// <summary>
    /// Default constructor
    /// </summary>
    protected DiagramControlBase() {
      // Update the display setting.
      Graphics g = CreateGraphics();
      Display.DpiX = g.DpiX;
      Display.DpiY = g.DpiY;

      //create the provider for all shapy diagram elements
      ShapeProvider shapeProvider = new ShapeProvider();
      TypeDescriptor.AddProvider(
          shapeProvider,
          typeof(SimpleShapeBase));

      TypeDescriptor.AddProvider(
          shapeProvider,
          typeof(ComplexShapeBase));

      //the provider for connections
      ConnectionProvider connectionProvider =
          new ConnectionProvider();

      TypeDescriptor.AddProvider(
          connectionProvider,
          typeof(ConnectionBase));

      //scrolling stuff
      this.AutoScroll = true;
      this.HScroll = true;
      this.VScroll = true;

      mPageSettings = new PageSettings();
      mPageSettings.Landscape = true;
      mPageSettings.PaperSize = new PaperSize("Letter", 850, 1100);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the background image displayed in the control.
    /// </summary>
    /// <value></value>
    /// <returns>An <see cref="T:System.Drawing.Image"></see> that represents the image to display in the background of the control.</returns>
    /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public override Image BackgroundImage {
      get {
        return base.BackgroundImage;
      }
      set {
        base.BackgroundImage = value;
        //TODO: change the backgroundtype
      }
    }
    //protected override void OnGiveFeedback(GiveFeedbackEventArgs gfbevent)
    //{
    //    base.OnGiveFeedback(gfbevent);
    //    gfbevent.UseDefaultCursors = false;
    //    Cursor.Current = CursorPallet.DropShape;
    //}
    /// <summary>
    /// 
    /// </summary>
    [Browsable(true), Description("The background color of the canvas if the type is set to 'flat'"), Category("Appearance")]
    public new Color BackColor {
      get {

        return base.BackColor;
      }
      set {
        //communicate the change down to the model
        //shouldn't this be done via the controller?
        mDocument.Model.CurrentPage.Ambience.BackgroundColor = value;
        ArtPalette.DefaultPageBackgroundColor = value;
        base.BackColor = value;
        this.Invalidate();
      }
    }
    /// <summary>
    /// Gets or sets the type of the background.
    /// </summary>
    /// <value>The type of the background.</value>
    [Browsable(true), Description("The background type"), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public CanvasBackgroundTypes BackgroundType {
      get { return mDocument.Model.CurrentPage.Ambience.BackgroundType; }
      set { mDocument.Model.CurrentPage.Ambience.BackgroundType = value; }
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
        mView = value;
      }
    }

    /// <summary>
    /// Gets or sets the controller.
    /// </summary>
    /// <value>The controller.</value>
    public IController Controller {
      get {
        return mController;
      }
      set { AttachToController(value); }

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Document.  You must call 
    /// 'AttachToDocument(Document)' to apply a new document.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Document Document {
      get { return mDocument; }
      set { mDocument = value; }
    }


    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Saves all recently opened files to the path specified.  The files
    /// are saved to a simple text file, one filename per line.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="maxFiles">int: The max number of filenams to
    /// save.</param>
    // ------------------------------------------------------------------
    public virtual void SaveRecentlyOpenedFiles(
        string path,
        int maxFiles) {
      if (myRecentFiles.Count < 1) {
        return;
      }

      StreamWriter sw = File.CreateText(path);

      // Keep track of all files saved so we don't save duplicates.
      ArrayList savedFiles = new ArrayList();
      string fileName;

      // Start at the last (i.e. most recent file opened) and save
      // until either 0 or maxFiles is reached.
      for (int i = myRecentFiles.Count - 1; i >= 0; i--) {
        if (maxFiles == 0) {
          break;
        }

        fileName = myRecentFiles[i].ToString();
        if (File.Exists(fileName)) {
          if (savedFiles.Contains(fileName) == false) {
            sw.WriteLine(fileName);
            savedFiles.Add(fileName);
            maxFiles--;
          }
        }
      }
      sw.Flush();
      sw.Close();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Reads the list of recently opened filenames from the
    /// path specified.  If the path doesn't exist or if there aren't
    /// any files to read, then 'null' is returned.
    /// </summary>
    /// <param name="path">string[]</param>
    // ------------------------------------------------------------------
    public virtual string[] ReadRecentlyOpenedFiles(string path) {
      if (File.Exists(path) == false) {
        return null;
      }

      StreamReader sr = new StreamReader(path);

      string text = sr.ReadToEnd();
      string[] filenames = text.Split("\n".ToCharArray());
      this.myRecentFiles.Clear();
      string trimmedFileName;
      foreach (string filename in filenames) {
        trimmedFileName = filename.Trim("\r".ToCharArray());
        if (File.Exists(trimmedFileName)) {
          this.myRecentFiles.Add(trimmedFileName);
        }
      }

      if (this.myRecentFiles.Count > 0) {
        return (string[])myRecentFiles.ToArray(typeof(string));
      } else {
        return null;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Sets the layout root.
    /// </summary>
    /// <param name="shape">The shape.</param>
    // ------------------------------------------------------------------
    public virtual void SetLayoutRoot(IShape shape) {
      this.Controller.Model.LayoutRoot = shape;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Runs the activity.
    /// </summary>
    /// <param name="activityName">Name of the activity.</param>
    // ------------------------------------------------------------------
    public virtual void RunActivity(string activityName) {
      this.Controller.RunActivity(activityName);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnEntityRemoved event of the Controller control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    protected virtual void HandleOnEntityRemoved(
        object sender,
        EntityEventArgs e) {
      RaiseOnEntityRemoved(e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnEntityAdded event of the Controller control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    protected virtual void HandleOnEntityAdded(
        object sender,
        EntityEventArgs e) {
      RaiseOnEntityAdded(e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnBackColorChange event of the View control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.ColorEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    protected void View_OnBackColorChange(object sender, ColorEventArgs e) {
      base.BackColor = e.Color;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Bubbles the OnHistoryChange event from the Controller to the surface
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.HistoryChangeEventArgs"/> 
    /// instance containing the event data.</param>
    // ------------------------------------------------------------------
    void mController_OnHistoryChange(
        object sender,
        HistoryChangeEventArgs e) {
      RaiseOnHistoryChange(e);
    }

    void Controller_OnShowSelectionProperties(object sender, SelectionEventArgs e) {
      RaiseOnShowSelectionProperties(e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Attaches to the controller specified and registers for all
    /// required events to update this control from the controller.
    /// </summary>
    /// <param name="controller">IController</param>
    // ------------------------------------------------------------------
    protected virtual void AttachToController(IController controller) {

      if (controller == null)
        throw new ArgumentNullException();
      mController = controller;
      mController.OnHistoryChange +=
          new EventHandler<HistoryChangeEventArgs>(
          mController_OnHistoryChange);

      mController.OnShowSelectionProperties +=
          new EventHandler<SelectionEventArgs>(
          Controller_OnShowSelectionProperties);

      mController.OnEntityAdded +=
          new EventHandler<EntityEventArgs>(
          HandleOnEntityAdded);

      mController.OnEntityRemoved +=
          new EventHandler<EntityEventArgs>(
          HandleOnEntityRemoved);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Resets the document and underlying model.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual void NewDocument() {
      // Lets see, we really only want to ask the user if they want to
      // save their changes if they don't have changes to save.
      // We can check the un/redo mananger to see if there are changes.
      if ((this.mController.UndoManager.CanRedo()) ||
          (this.mController.UndoManager.CanUndo())) {
        DialogResult result = MessageBox.Show(
            "Save changes before clearing the current diagram?",
             "Confirm Clear",
             MessageBoxButtons.YesNoCancel,
             MessageBoxIcon.Question);

        if (result == DialogResult.Yes) {
          this.Save();
        } else if (result == DialogResult.Cancel) {
          return;
        }
      }
      Document = new Document();
      this.mController.UndoManager.ClearUndoRedo();
      AttachToDocument(Document);
      this.mFileName = "";
      RaiseOnNewDiagram();
      //this.Controller.Model.Clear();            
    }

    #region Printing

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays a PageSetupDialog so the user can specify how each
    /// page is printed.
    /// </summary>
    // ------------------------------------------------------------------
    public void PageSetup() {
      PageSetupDialog dialog = new PageSetupDialog();
      dialog.PageSettings = this.PageSettings;
      if (dialog.ShowDialog() == DialogResult.OK) {
        this.PageSettings = dialog.PageSettings;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Prints all pages of the diagram.
    /// </summary>
    // ------------------------------------------------------------------
    public void Print() {
      DiagramPrinter myPrinter = new DiagramPrinter(
          this.PageSettings,
          this,
          false);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Print previews all pages of the diagram.
    /// </summary>
    // ------------------------------------------------------------------
    public void PrintPreview() {
      DiagramPrinter myPrinter = new DiagramPrinter(
          this.PageSettings,
          this,
          true);
    }

    #endregion

    #region IO
    // ------------------------------------------------------------------
    /// <summary>
    /// If the current filename (FileName property) is empty, then a
    /// SaveFileDialog is displayed for the user to specify what to save 
    /// the diagram as.  Otherwise, the current filename is used to save 
    /// the diagram.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual void Save() {
      try {
        if (this.mFileName == "") {
          SaveFileDialog dialog = new SaveFileDialog();
          if (dialog.ShowDialog() == DialogResult.OK) {
            this.SaveAs(dialog.FileName);
          }
        } else {
          this.SaveAs(this.mFileName);
        }
      }
      catch (Exception ex) {
        MessageBox.Show("An error occured while saving the " +
            "diagram.  See below for the message about the " +
            "error.\n\n" +
            "Message: " + ex.Message,
            "Save Diagram Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays a SaveFileDialog regardless if there's an existing
    /// filename so the user can save the diagram to a new location.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual void SaveAs() {
      SaveFileDialog dialog = new SaveFileDialog();

      if (dialog.ShowDialog() == DialogResult.OK) {
        SaveAs(dialog.FileName);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Saves the diagram to the given location.
    /// </summary>
    /// <param name="path">The path.</param>
    // ------------------------------------------------------------------
    public virtual void SaveAs(string path) {
      if (!Directory.Exists(Path.GetDirectoryName(path))) {
        throw new DirectoryNotFoundException(
            "Create the directory before saving the diagram to it.");
      }
      RaiseOnSavingDiagram(path);
      if (BinarySerializer.SaveAs(path, this) == true) {
        // Store the filename if the save was successful.
        this.mFileName = path;

        // Clear the undo/redo history.
        this.mController.UndoManager.ClearUndoRedo();
        RaiseOnDiagramSaved(path);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays an OpenFileDialog for the user to specify the diagram
    /// to open.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual void Open() {
      OpenFileDialog dialog = new OpenFileDialog();
      if (dialog.ShowDialog() == DialogResult.OK) {
        this.Open(dialog.FileName);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Opens the diagram from the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    // ------------------------------------------------------------------
    public virtual void Open(string path) {
      if (!Directory.Exists(Path.GetDirectoryName(path))) {
        throw new DirectoryNotFoundException(
            "Create the directory before saving the diagram to it.");
      }
      this.NewDocument();//makes it possible for the host to ask for 
      // saving first.

      RaiseOnOpeningDiagram(path);//do it before opening the new diagram.
      if (BinarySerializer.Open(path, this) == true) {
        // Store the filename if successful.
        this.mFileName = path;

        // Add the filename to the list of previously opened files.
        this.myRecentFiles.Add(path);

        // Raise that a new diagram was opened.
        RaiseOnDiagramOpened(path);
      }
      //this.mFileName = mFileName;
    }
    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Attachement to the document.
    /// </summary>
    /// <param name="document">The document.</param>
    // ------------------------------------------------------------------
    public virtual void AttachToDocument(Document document) {

      if (document == null)
        throw new ArgumentNullException();

      this.mDocument = document;

      #region re-attach to the new model
      View.Model = Document.Model;
      Controller.Model = Document.Model;
      #endregion
      #region Update the ambience
      document.Model.SetCurrentPage(0);
      #endregion

      this.Controller.Model.Selection.Clear();
      this.Invalidate();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Activates a registered tool with the given name
    /// </summary>
    /// <param name="toolName">the name of a tool</param>
    // ------------------------------------------------------------------
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public void ActivateTool(string toolName) {
      if (toolName == null || toolName.Trim().Length == 0) {
        throw new ArgumentNullException("The tool name cannot " +
            "be 'null' or empty.");
      }

      if (this.Controller == null) {
        throw new InconsistencyException(
            "The Controller of the surface is 'null', this is a " +
            "strong inconsistency in the MVC model.");
      }
      if (toolName.Trim().Length > 0)
        this.Controller.ActivateTool(toolName);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Starts the tool with the given name.
    /// </summary>
    /// <param name="toolName">the name of a registered tool</param>
    // ------------------------------------------------------------------
    public void LaunchTool(string toolName) {
      if (this.Controller == null) return;
      this.Controller.ActivateTool(toolName);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Overrides the base method to call the view which will paint the 
    /// diagram on the given graphics surface.
    /// </summary>
    /// <param name="e">PaintEventArgs</param>
    // ------------------------------------------------------------------
    protected override void OnPaint(PaintEventArgs e) {
      base.OnPaint(e);
      mView.Paint(e.Graphics);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the background of the control.
    /// </summary>
    /// <param name="pevent">A 
    /// <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that 
    /// contains information about the control to paint.</param>
    // ------------------------------------------------------------------
    protected override void OnPaintBackground(PaintEventArgs pevent) {
      base.OnPaintBackground(pevent);

      mView.PaintBackground(pevent.Graphics);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Undoes the last action that was recorded in the UndoManager.
    /// </summary>
    // ------------------------------------------------------------------
    public void Undo() {
      this.Controller.Undo();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Redoes the last action that was recorded in the UndoManager.
    /// </summary>
    // ------------------------------------------------------------------
    public void Redo() {
      this.Controller.Redo();
    }
    #endregion

    #region Explicit ISupportInitialize implementation
    void ISupportInitialize.BeginInit() {
      //here you can check the conformity of properties
      BeginInit();
    }

    /// <summary>
    /// Signals the object that initialization is complete.
    /// </summary>
    void ISupportInitialize.EndInit() {
      EndInit();
    }

    /// <summary>
    /// Signals the object that initialization is starting.
    /// </summary>
    protected virtual void BeginInit() {

    }
    /// <summary>
    /// Signals the object that initialization is complete.
    /// </summary>
    protected virtual void EndInit() {
      //necessary if the background type is gradient since the brush requires the size of the client rectangle
      mView.SetBackgroundType(BackgroundType);

      //the diagram control provider gives problems in design mode. If enable at design mode the
      //diagramming control becomes a container component rather than a form control because the essential
      //properties are disabled through the ControlProvider (the Location or ID for example).
      //Also, do not try to put this in the constructor, you need the ISupportInitialize to get a meaningful DesignMode value.
      if (!DesignMode) {
        ControlProvider controlProvider = new ControlProvider();
        TypeDescriptor.AddProvider(controlProvider, typeof(DiagramControlBase));
      }
    }
    #endregion
  }
}
