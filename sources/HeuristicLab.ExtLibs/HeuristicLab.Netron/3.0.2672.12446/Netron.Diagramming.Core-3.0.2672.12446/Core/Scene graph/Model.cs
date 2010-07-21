using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Implementation of the <see cref="IModel"/> interface; the 'database' 
  /// of the control.
  /// </summary>
  // ----------------------------------------------------------------------
  public partial class Model :
      IModel,
      IDisposable {
    #region Events

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the current page has changed.
    /// </summary>
    // ------------------------------------------------------------------
    public event CurrentPageChangedEventHandler OnCurrentPageChanged;

    /// <summary>
    /// Occurs when an entity is removed from the model
    /// </summary>
    public event EventHandler<EntityEventArgs> OnEntityRemoved;
    /// <summary>
    /// Occurs when an entity is added to the model.
    /// </summary>
    public event EventHandler<EntityEventArgs> OnEntityAdded;
    /// <summary>
    /// Occurs when an element of the diagram requests a refresh of a region/rectangle of the canvas
    /// </summary>
    public event EventHandler<RectangleEventArgs> OnInvalidateRectangle;
    /// <summary>
    /// Occurs when an element of the diagram requests a refresh
    /// </summary>
    public event EventHandler OnInvalidate;
    /// <summary>
    /// Occurs when the ConnectionCollection has changed
    /// </summary>
    public event EventHandler<ConnectionCollectionEventArgs> OnConnectionCollectionChanged;
    /// <summary>
    /// Occurs when the cursor is changed and the surface is supposed to set the cursor accordingly.
    /// </summary>
    public event EventHandler<CursorEventArgs> OnCursorChange;
    /// <summary>
    /// Raises the <see cref="OnConnectionCollectionChanged"/> event
    /// </summary>
    /// <param name="e">ConnectionCollection event argument</param>
    private void RaiseOnConnectionCollectionChanged(ConnectionCollectionEventArgs e) {
      EventHandler<ConnectionCollectionEventArgs> handler = OnConnectionCollectionChanged;
      if (handler != null) {
        handler(this, e);
      }
    }

    public void RaiseOnCursorChange(Cursor cursor) {
      EventHandler<CursorEventArgs> handler = OnCursorChange;
      if (handler != null)
        handler(this, new CursorEventArgs(cursor));
    }

    /// <summary>
    /// Raises the on invalidate.
    /// </summary>
    public void RaiseOnInvalidate() {
      if (OnInvalidate != null)
        OnInvalidate(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raises the OnInvalidateRectangle event.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    public void RaiseOnInvalidateRectangle(Rectangle rectangle) {
      EventHandler<RectangleEventArgs> handler = OnInvalidateRectangle;
      if (handler != null) {
        handler(this, new RectangleEventArgs(rectangle));
      }
    }
    /// <summary>
    /// Occurs when the bounding region (aka client-rectangle) of the canvas has changed
    /// </summary>
    public event EventHandler<RectangleEventArgs> OnRectangleChanged;
    /// <summary>
    /// Raises the <see cref="OnRectangleChanged"/> event.
    /// </summary>
    /// <param name="e"></param>
    private void RaiseOnRectangleChanged(RectangleEventArgs e) {
      EventHandler<RectangleEventArgs> handler = OnRectangleChanged;
      if (handler != null) {
        handler(this, e);
      }
    }
    /// <summary>
    /// Occurs when the diagram info (aka user metadata) has changed
    /// </summary>
    public event EventHandler<DiagramInformationEventArgs> OnDiagramInformationChanged;
    /// <summary>
    /// Raises the <see cref="OnDiagramInformationChanged"/> event.
    /// </summary>
    /// <param name="e"></param>
    private void RaiseOnDiagramInformationChanged(DiagramInformationEventArgs e) {
      EventHandler<DiagramInformationEventArgs> handler = OnDiagramInformationChanged;
      if (handler != null) {
        handler(this, e);
      }
    }

    /// <summary>
    /// Occurs when the Ambience has changed
    /// </summary>
    public event EventHandler<AmbienceEventArgs> OnAmbienceChanged;
    /// <summary>
    /// Raises the <see cref="OnAmbienceChanged"/> event
    /// </summary>
    /// <param name="e"></param>
    private void RaiseOnAmbienceChanged(AmbienceEventArgs e) {
      EventHandler<AmbienceEventArgs> handler = OnAmbienceChanged;
      if (handler != null) {
        handler(this, e);
      }
    }

    /// <summary>
    /// Raises the <see cref="OnEntityAdded"/> event
    /// </summary>
    /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
    private void RaiseOnEntityAdded(EntityEventArgs e) {
      EventHandler<EntityEventArgs> handler = OnEntityAdded;
      if (handler != null) {
        handler(this, e);
      }
    }

    /// <summary>
    /// Raises the <see cref="OnEntityRemoved"/> event.
    /// </summary>
    /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
    private void RaiseOnEntityRemoved(EntityEventArgs e) {
      EventHandler<EntityEventArgs> handler = OnEntityRemoved;
      if (handler != null) {
        handler(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the <see cref="OnCurrentPageChanged"/> event.
    /// </summary>
    /// <param name="e">PageEventArgs</param>
    // ------------------------------------------------------------------
    protected virtual void RaiseOnCurrentPageChanged(PageEventArgs e) {
      if (this.OnCurrentPageChanged != null) {
        OnCurrentPageChanged(this, e);
      }
    }

    #endregion

    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// Model.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double modelVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if all shape's connectors are shown.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mShowConnectors = true;

    private GraphicsUnit measurementUnits = GraphicsUnit.Pixel;
    /// <summary>
    /// the LayoutRoot field
    /// </summary>
    private IShape mLayoutRoot;
    /// <summary>
    /// the DefaultPage field
    /// </summary>
    [NonSerialized]
    private IPage mDefaultPage;
    /// <summary>
    /// the page collection
    /// </summary>
    [NonSerialized]
    private CollectionBase<IPage> mPages;
    ///<summary>
    /// the shapes of the diagram
    /// </summary>
    //[NonSerialized]
    //private CollectionBase<IShape> mShapes;
    /// <summary>
    /// the bounding rectangle
    /// </summary>
    [NonSerialized]
    private Rectangle mRectangle;
    /// <summary>
    /// the metadata of the diagram
    /// </summary>
    [NonSerialized]
    private DocumentInformation mInformation;

    /// <summary>
    /// the collection of to-be-painted diagram entities
    /// </summary>
    //[NonSerialized]
    //private CollectionBase<IDiagramEntity> Paintables;
    /// <summary>
    /// the CurrentPage field
    /// </summary>
    [NonSerialized]
    private IPage mCurrentPage;

    private float mMeasurementScale = 1.0F;
    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual double Version {
      get {
        return modelVersion;
      }
    }

    private Selection selection;
    public Selection Selection {
      get { return this.selection; }
      set { this.selection = value; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if all shape's connectors are shown.
    /// </summary>
    // ------------------------------------------------------------------
    public bool ShowConnectors {
      get {
        return this.mShowConnectors;
      }
      set {
        this.mShowConnectors = value;
        foreach (IPage page in this.mPages) {
          foreach (IShape shape in page.Shapes) {
            shape.ShowConnectors = this.mShowConnectors;
          }
        }
      }
    }

    [Browsable(true)]
    [Description("Scaling value for logical units.")]
    public float MeasurementScale {
      get {
        return mMeasurementScale;
      }

      set {
        mMeasurementScale = value;
      }
    }

    [BrowsableAttribute(true)]
    [Description("Logical unit of measurement")]
    public GraphicsUnit MeasurementUnits {
      get {
        return measurementUnits;
      }

      set {
        measurementUnits = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the LayoutRoot
    /// </summary>
    // ------------------------------------------------------------------
    public IShape LayoutRoot {
      get { return mLayoutRoot; }
      set {
        mLayoutRoot = value;

      }
    }

    // ------------------------------------------------------------------        
    /// <summary>
    /// Gets the current page.  Use 'SetCurrentPage(IPage page)' or
    /// 'SetCurrentPage(int index)' to set the current page.
    /// </summary>
    // ------------------------------------------------------------------
    public IPage CurrentPage {
      get { return mCurrentPage; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the paintables.
    /// </summary>
    /// <value>The paintables.</value>
    // ------------------------------------------------------------------
    public CollectionBase<IDiagramEntity> Paintables {
      get {
        return mCurrentPage.Entities;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the pages of the diagram control.  Use method 'AddPage' to
    /// add a page so the page gets attached to this Model.
    /// </summary>
    /// <value>The pages.</value>
    // ------------------------------------------------------------------
    public CollectionBase<IPage> Pages {
      get { return mPages; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the default page
    /// </summary>
    // ------------------------------------------------------------------
    public IPage DefaultPage {
      get { return mDefaultPage; }
      set { mDefaultPage = value; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the shapes of the current page.
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<IShape> Shapes {
      get {
        return CurrentPage.Shapes;
        //return mShapes;
      }
      //internal set
      //{
      //    mShapes = value;
      //}
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the information of the diagram
    /// </summary>
    // ------------------------------------------------------------------
    internal DocumentInformation Information {
      get {
        return mInformation;
      }
      set {
        mInformation = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the bounding rectangle of the diagram (client rectangle)
    /// </summary>
    // ------------------------------------------------------------------
    public Rectangle Rectangle {
      get {
        return mRectangle;
      }
      set {
        mRectangle = value;
        RaiseOnRectangleChanged(new RectangleEventArgs(value));
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the horizontal coordinate of the diagram
    /// </summary>
    // ------------------------------------------------------------------
    public float X {
      get {
        return mRectangle.X;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the verticle coordinate of the diagram
    /// </summary>
    // ------------------------------------------------------------------
    public float Y {
      get {
        return mRectangle.Y;
      }

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the width of the diagram
    /// </summary>
    // ------------------------------------------------------------------
    public float Width {
      get {
        return mRectangle.Width;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the height of the diagram
    /// </summary>
    // ------------------------------------------------------------------
    public float Height {
      get {
        return mRectangle.Height;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the collection of connections
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<IConnection> Connections {
      get {
        return CurrentPage.Connections;
      }

    }
    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    /// <summary>
    /// Default constructor
    /// </summary>
    // ------------------------------------------------------------------
    public Model() {
      //here I'll have to work on the scene graph
      //this.mShapes = new CollectionBase<IShape>();
      //the default page

      //the page collection
      mPages = new CollectionBase<IPage>();
      Page p = new Page("Default Page", this);
      p.Ambience.PageColor = ArtPalette.DefaultPageColor;
      mPages.Add(p);

      Init();
    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes this object
    /// <remarks>See also the <see cref="OnDeserialized"/> event for 
    /// post-deserialization actions to which this method is related.
    /// </remarks>
    /// </summary>
    // ------------------------------------------------------------------
    private void Init() {
      if (mPages == null) {
        throw new InconsistencyException(
            "The page collection is 'null'.");
      }
      if (mPages.Count == 0) {
        throw new InconsistencyException(
            "The page collection should contain at least one page.");
      }

      foreach (IPage page in mPages)
        AttachToPage(page);
      mDefaultPage = mPages[0];
      // Initially the current page is the zero-th page in the 
      // collection.
      SetCurrentPage(0);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds a page.  This should be used when adding pages rather than
    /// though the Pages property so the page gets attached to the Model.
    /// </summary>
    /// <param name="page">IPage: The page to add.</param>
    /// <returns>IPage</returns>
    // ------------------------------------------------------------------
    public IPage AddPage(IPage page) {
      mPages.Add(page);
      AttachToPage(page);
      return page;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds a page.  This should be used when adding pages rather than
    /// though the Pages property so the page gets attached to the Model.
    /// The page name is set to "Page" plus the new number of pages.
    /// For example, if there are currently two pages, then "Page3" is 
    /// set as the new page name.
    /// </summary>
    /// <returns>IPage</returns>
    // ------------------------------------------------------------------
    public IPage AddPage() {
      string pageName = this.GetDefaultNewPageName();
      Page page = new Page(pageName, this);
      return AddPage(page);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Deletes the page specified if it is not the default page.
    /// </summary>
    /// <param name="page">IPage: The page to remove.</param>
    /// <param name="allowWarnings">bool: Specifies if the user should
    /// be given the option to cancel the action if the current page
    /// has entities.  Also, when set to true, if the current page is
    /// the default page, then a message box is shown informing the
    /// user that the default page cannot be deleted.</param>
    /// <returns>bool: If the delete was successful.  True is returned
    /// if the current page was removed.</returns>
    // ------------------------------------------------------------------
    public bool RemovePage(IPage page, bool allowWarnings) {
      if (page == this.DefaultPage) {
        if (allowWarnings) {
          MessageBox.Show(
          "The default page cannot be deleted.",
          "Delete Page Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        }
        return false;
      }

      int pageIndex = 0;
      int newCurrentPageIndex = 0;

      // Ask the user if they really want to remove the page if
      // it has entities if 'allowWarnings' is true.
      if ((allowWarnings) && (page.Entities.Count > 0)) {
        if (MessageBox.Show(
            "Are you sure you want to delete this page?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button1) == DialogResult.No) {
          return false;
        }
      }

      // Now we can continue with deleting the page.
      // If the page specified is the current page, then we need to set
      // the current page to another page.  Let's use the previous one 
      // in the list.  
      if (page == CurrentPage) {
        pageIndex = Pages.IndexOf(page);

        if (pageIndex > 0) {
          newCurrentPageIndex = pageIndex - 1;
        }
        SetCurrentPage(newCurrentPageIndex);
      }

      Pages.Remove(page);
      return true;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns a new page name that's unique from all the others.
    /// </summary>
    /// <returns>string: Returns "Page" plus the number of pages IF A
    /// NEW PAGE WERE ADDED.  For example, if there are currently two
    /// pages, then "Page3" is returned.</returns>
    // ------------------------------------------------------------------
    public string GetDefaultNewPageName() {
      string name = "Page" + (Pages.Count + 1).ToString();
      return name;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Sets the current page.
    /// </summary>
    /// <param name="page">The page.</param>
    // ------------------------------------------------------------------
    public void SetCurrentPage(IPage page) {
      mCurrentPage = page;
      RaiseOnAmbienceChanged(new AmbienceEventArgs(page.Ambience));
      RaiseOnCurrentPageChanged(new PageEventArgs(page));

      //change the paintables as well            
      //Paintables = new CollectionBase<IDiagramEntity>();

      #region Reload of the z-order, usually only necessary after deserialization

      CollectionBase<IDiagramEntity> collected = new CollectionBase<IDiagramEntity>();
      //pick up the non-group entities
      foreach (IDiagramEntity entity in Paintables)
        if (!typeof(IGroup).IsInstanceOfType(entity))
          collected.Add(entity);

      if (collected.Count > 0) {
        Algorithms.SortInPlace<IDiagramEntity>(collected, new SceneIndexComparer<IDiagramEntity>());
        //Paintables.AddRange(collected);
      }
      #endregion

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Sets the current page.
    /// </summary>
    /// <param name="pageIndex">Index of the page.</param>
    // ------------------------------------------------------------------
    public void SetCurrentPage(int pageIndex) {
      if (mPages == null ||
          mPages.Count == 0 ||
          pageIndex >= mPages.Count ||
          pageIndex < 0) {
        throw new IndexOutOfRangeException(
            "The page index is outside the page range.");
      }
      SetCurrentPage(mPages[pageIndex]);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Attaches this model to the page specified.  The Model property for
    /// the page is set and the IPage events needed by this are registered
    /// for here (OnEntityAdded, OnEntityRemoved, OnClear, and 
    /// OnAmbienceChanged).
    /// </summary>
    /// <param name="page">IPage: The page to attach this model to.</param>
    // ------------------------------------------------------------------
    private void AttachToPage(IPage page) {
      page.OnEntityAdded += new EventHandler<EntityEventArgs>(mDefaultPage_OnEntityAdded);
      page.OnEntityRemoved += new EventHandler<EntityEventArgs>(mDefaultPage_OnEntityRemoved);
      page.OnClear += new EventHandler(mDefaultPage_OnClear);
      page.OnAmbienceChanged += new EventHandler<AmbienceEventArgs>(mDefaultPage_OnAmbienceChanged);
      page.Model = this;
    }

    void mDefaultPage_OnAmbienceChanged(object sender, AmbienceEventArgs e) {
      RaiseOnAmbienceChanged(e);
    }

    #region Paintables transfers on Page changes

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnClear event of the DefaultPage.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    void mDefaultPage_OnClear(object sender, EventArgs e) {
      Paintables.Clear();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnEntityRemoved event of the DefaultPage.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    void mDefaultPage_OnEntityRemoved(object sender, EntityEventArgs e) {
      if (Paintables.Contains(e.Entity)) {
        //shift the entities above the one to be removed
        int index = e.Entity.SceneIndex;
        foreach (IDiagramEntity entity in Paintables) {
          if (entity.SceneIndex > index)
            entity.SceneIndex--;
        }
        Paintables.Remove(e.Entity);
      }
      //if the selection contains the shape we have to remove it from the selection
      if (Selection.SelectedItems.Contains(e.Entity)) {
        Selection.SelectedItems.Remove(e.Entity);
      }
      RaiseOnEntityRemoved(e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnEntityAdded event of the Page and adds the new 
    /// entity to the Paintables.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    void mDefaultPage_OnEntityAdded(object sender, EntityEventArgs e) {
      //don't add it if it's already there or if it's a group (unless you want to deploy something special to emphasize a group shape).
      if (!Paintables.Contains(e.Entity)) {
        if ((e.Entity is IGroup) && !(e.Entity as IGroup).EmphasizeGroup) {
          return;
        }
        //set the new entity on top of the stack
        e.Entity.SceneIndex = Paintables.Count;
        Paintables.Add(e.Entity);
      }
      #region Addition callback
      IAdditionCallback callback = e.Entity.GetService(typeof(IAdditionCallback)) as IAdditionCallback;
      if (callback != null)
        callback.OnAddition();
      #endregion
      RaiseOnEntityAdded(e);
    }
    #endregion

    #region Methods

    #region Ordering methods
    /// <summary>
    /// Re-sets the scene-index of the paintables
    /// </summary>
    private void ReAssignSceneIndex(CollectionBase<IDiagramEntity> entities) {
      for (int i = 0; i < entities.Count; i++) {
        entities[i].SceneIndex = i;
      }
    }
    /// <summary>
    /// Sends to entity to the bottom of the z-order stack.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public void SendToBack(IDiagramEntity entity) {
      ILayer layer = CurrentPage.GetLayer(entity);
      if (layer != null) {
        layer.Entities.Remove(entity);
        layer.Entities.Insert(0, entity);
        ReAssignSceneIndex(layer.Entities);
        Rectangle rec = entity.Rectangle;
        rec.Inflate(20, 20);
        this.RaiseOnInvalidateRectangle(Rectangle);
      }
      //if(Paintables.Contains(entity))
      //{
      //    Paintables.Remove(entity);
      //    Paintables.Insert(0, entity);
      //    ReAssignSceneIndex();
      //    Rectangle rec = entity.Rectangle;
      //    rec.Inflate(20, 20);
      //    this.RaiseOnInvalidateRectangle(Rectangle);
      //}
    }

    /// <summary>
    /// Sends the entity down the z-order stack with the specified amount.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="zShift">The z shift.</param>
    public void SendBackwards(IDiagramEntity entity, int zShift) {
      ILayer layer = CurrentPage.GetLayer(entity);
      if (layer != null) {
        int newpos = layer.Entities.IndexOf(entity) - zShift;
        //if this is the first in the row you cannot move it lower
        if (newpos >= 0) {
          layer.Entities.Remove(entity);
          layer.Entities.Insert(newpos, entity);
          ReAssignSceneIndex(layer.Entities);
          Rectangle rec = entity.Rectangle;
          rec.Inflate(20, 20);
          this.RaiseOnInvalidateRectangle(Rectangle);
        }
      }

      //if (Paintables.Contains(entity))
      //{
      //    int newpos = Paintables.IndexOf(entity) - zShift;
      //    //if this is the first in the row you cannot move it lower
      //    if (newpos >= 0)
      //    {
      //        Paintables.Remove(entity);
      //        Paintables.Insert(newpos, entity);
      //        ReAssignSceneIndex(Paintables);
      //        Rectangle rec = entity.Rectangle;
      //        rec.Inflate(20, 20);
      //        this.RaiseOnInvalidateRectangle(Rectangle);
      //    }
      //}
    }
    /// <summary>
    /// Sends the entity one level down the z-order stack.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public void SendBackwards(IDiagramEntity entity) {
      SendBackwards(entity, 1);
    }

    /// <summary>
    /// Sends the entity to the top of the z-order stack.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public void SendForwards(IDiagramEntity entity) {
      SendForwards(entity, 1);
    }
    /// <summary>
    /// Sends the entity up the z-order stack with the specified amount.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="zShift">The z shift.</param>
    public void SendForwards(IDiagramEntity entity, int zShift) {
      ILayer layer = CurrentPage.GetLayer(entity);
      if (layer != null) {
        int newpos = layer.Entities.IndexOf(entity) + zShift;
        //if this is the last in the row you cannot move it higher
        if (newpos < layer.Entities.Count) {
          layer.Entities.Remove(entity);
          layer.Entities.Insert(newpos, entity); //does it works when this is an addition at the top?
          ReAssignSceneIndex(layer.Entities);
          Rectangle rec = entity.Rectangle;
          rec.Inflate(20, 20);
          this.RaiseOnInvalidateRectangle(Rectangle);
        }
      }

      //if (Paintables.Contains(entity) && zShift>=1)
      //{
      //    int newpos = Paintables.IndexOf(entity) + zShift;
      //    //if this is the last in the row you cannot move it higher
      //    if (newpos < Paintables.Count)
      //    {
      //        Paintables.Remove(entity);
      //        Paintables.Insert(newpos, entity); //does it works when this is an addition at the top?
      //        ReAssignSceneIndex(Paintables);
      //        Rectangle rec = entity.Rectangle;
      //        rec.Inflate(20, 20);
      //        this.RaiseOnInvalidateRectangle(Rectangle);
      //    }
      //}
    }

    /// <summary>
    /// Sends the entity to the front of the z-order stack.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public void SendToFront(IDiagramEntity entity) {
      ILayer layer = CurrentPage.GetLayer(entity);
      if (layer != null) {
        layer.Entities.Remove(entity);
        layer.Entities.Add(entity);
        ReAssignSceneIndex(layer.Entities);
        Rectangle rec = entity.Rectangle;
        rec.Inflate(20, 20);
        this.RaiseOnInvalidateRectangle(Rectangle);
      }

      //if(Paintables.Contains(entity))
      //{
      //    Paintables.Remove(entity);
      //    Paintables.Add(entity);
      //    ReAssignSceneIndex(Paintables);
      //    Rectangle rec = entity.Rectangle;
      //    rec.Inflate(20, 20);
      //    this.RaiseOnInvalidateRectangle(Rectangle);
      //}
    }
    #endregion

    #region Diagram manipulation actions

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns the number of shapes in the current page that are of
    /// the type specified.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>int</returns>
    // ------------------------------------------------------------------
    public int NumberOfShapes(Type type) {
      int count = 0;
      foreach (IShape shape in Shapes) {
        if (shape.GetType() == type) {
          count++;
        }
      }
      return count;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the shape at the specified location.  If no shape could be
    /// found then 'null' is returned.
    /// </summary>
    /// <param name="surfacePoint">Point: The location in world 
    /// coordinates.</param>
    /// <returns>IShape</returns>
    // ------------------------------------------------------------------
    public IShape GetShapeAt(Point surfacePoint) {
      foreach (IShape shape in this.CurrentPage.Shapes) {
        if (shape.Hit(surfacePoint)) {
          return shape;
        }
      }
      return null;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds an entity to the diagram.  The magnification level for the
    /// entity is set to the current magnification level of the current 
    /// page.
    /// </summary>
    /// <param name="entity">IDiagramEntity: The entity to add.</param>
    /// <returns>IDiagramEntity: The added entity.</returns>
    // ------------------------------------------------------------------
    public IDiagramEntity AddEntity(IDiagramEntity entity) {
      SetModel(entity);
      //By default the new entity is added to the default layer in the 
      // current page.
      CurrentPage.DefaultLayer.Entities.Add(entity);
      entity.Attached(CurrentPage.DefaultLayer);
      entity.Magnification = CurrentPage.Magnification;
      return entity;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds a shape to the diagram
    /// </summary>
    /// <param name="shape">IShape: The shape to add.</param>
    /// <returns>IShape: The added shape.</returns>
    // ------------------------------------------------------------------
    public IShape AddShape(IShape shape) {

      SetModel(shape);
      // By default the new shape is added to the default layer in the 
      // current page.
      CurrentPage.DefaultLayer.Entities.Add(shape);
      shape.Attached(CurrentPage.DefaultLayer);
      shape.ShowConnectors = this.mShowConnectors;
      return shape;

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds a connection to the diagram
    /// </summary>
    /// <param name="connection">a connection</param>
    // ------------------------------------------------------------------
    public IConnection AddConnection(IConnection connection) {
      SetModel(connection);
      CurrentPage.DefaultLayer.Entities.Add(connection);
      connection.Attached(CurrentPage.DefaultLayer);
      return connection;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds a connection between two shape connectors.        
    /// </summary>
    /// <param name="from">From connector.</param>
    /// <param name="to">To connector.</param>
    // ------------------------------------------------------------------
    public IConnection AddConnection(IConnector from, IConnector to) {
      Connection con = new Connection(from.Point, to.Point);
      this.AddConnection(con);
      return con;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Sets the model (recursively) on the given entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    // ------------------------------------------------------------------
    public void SetModel(IDiagramEntity entity) {
      if (entity is IConnector) {
        (entity as IConnector).Model = this;
      } else if (entity is IConnection) {
        IConnection con = entity as IConnection;
        con.Model = this;
        Debug.Assert(con.From != null, "The 'From' connector is not set.");
        con.From.Model = this;
        Debug.Assert(con.From != null, "The 'To' connector is not set.");
        con.To.Model = this;
      } else if (entity is IShape) {
        IShape shape = entity as IShape;
        shape.Model = this;
        foreach (IConnector co in shape.Connectors) {
          co.Model = this;
        }
      } else if (entity is IGroup) {
        IGroup group = entity as IGroup;
        group.Model = this;
        foreach (IDiagramEntity child in group.Entities) {
          SetModel(child);
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Removes the shape from the diagram.
    /// </summary>
    /// <param name="shape">The shape.</param>
    // ------------------------------------------------------------------
    public void RemoveShape(IShape shape) {
      //remove it from the layer(s)
      foreach (IPage page in mPages) {
        foreach (ILayer layer in page.Layers) {
          if (layer.Entities.Contains(shape)) {
            layer.Entities.Remove(shape);
            shape.Detached(CurrentPage.DefaultLayer);
          }
        }
      }

      // The old way, when there weren't multiple pages and layers.
      //if (CurrentPage.DefaultLayer.Entities.Contains(shape))
      //{
      //    CurrentPage.DefaultLayer.Entities.Remove(shape);
      //    shape.Detached(CurrentPage.DefaultLayer);
      //}
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Removes all entities that are currently selected.
    /// </summary>
    /// <param name="entity">The entity.</param>
    // ------------------------------------------------------------------
    public void RemoveSelectedItems() {
      if (this.Selection.SelectedItems.Count < 1) {
        return;
      }

      int numberOfItems = this.Selection.SelectedItems.Count;
      for (int i = 0; i < numberOfItems - 1; i++) {
        IDiagramEntity entity = Selection.SelectedItems[0];
        foreach (IPage page in mPages) {
          foreach (ILayer layer in page.Layers) {
            if (layer.Entities.Contains(entity)) {
              layer.Entities.Remove(entity);
              entity.Detached(DefaultPage.DefaultLayer);
            }
          }
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Removes the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    // ------------------------------------------------------------------
    public void Remove(IDiagramEntity entity) {
      if (CurrentPage.DefaultLayer.Entities.Contains(entity)) {
        CurrentPage.DefaultLayer.Entities.Remove(entity);
        entity.Detached(CurrentPage.DefaultLayer);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Adds a collection of entities to the diagram
    /// </summary>
    /// <param name="collection">The collection.</param>
    // ------------------------------------------------------------------
    public void AddEntities(CollectionBase<IDiagramEntity> collection) {
      foreach (IDiagramEntity entity in collection) {
        SetModel(entity);
        CurrentPage.DefaultLayer.Entities.Add(entity);
        entity.Attached(CurrentPage.DefaultLayer);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Unwraps an entity
    /// <list type="bullet">
    /// <term>Uid</term><description>Generates a new <see cref="IDiagramEntity.Uid"/> for the entity. </description>
    /// <tem>Model</tem><description>Assigns the Model property to the entity.</description>
    /// 
    /// </list>
    /// </summary>
    // ------------------------------------------------------------------
    public void Unwrap(IDiagramEntity entity) {
      //set a new unique identifier for this copied object
      entity.NewUid(true);
      //this assignment will be recursive if needed
      SetModel(entity);
      CurrentPage.DefaultLayer.Entities.Add(entity);
      entity.Attached(CurrentPage.DefaultLayer);

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Unwraps the specified collection.
    /// </summary>
    /// <param name="collection">The collection.</param>
    // ------------------------------------------------------------------
    public void Unwrap(CollectionBase<IDiagramEntity> collection) {
      if (collection == null)
        return;
      foreach (IDiagramEntity entity in collection) {
        Unwrap(entity);
      }
      //reconnect the connectors, just like the deserialization of a filed diagram
      Dictionary<Guid, Anchor>.Enumerator enumer = Anchors.GetEnumerator();
      System.Collections.Generic.KeyValuePair<Guid, Anchor> pair;
      Anchor anchor;
      while (enumer.MoveNext()) {
        pair = enumer.Current;
        anchor = pair.Value;
        if (anchor.Parent != Guid.Empty) //there's a parent connector
                {
          if (Anchors.ContainsKey(anchor.Parent)) {
            Anchors.GetAnchor(anchor.Parent).Instance.AttachConnector(anchor.Instance);
          }
        }
      }
      //clean up the anchoring matrix
      Anchors.Clear();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Clears the current page.
    /// </summary>
    // ------------------------------------------------------------------
    public void Clear() {
      //clear the scene-graph
      this.CurrentPage.DefaultLayer.Entities.Clear();
    }
    #endregion

    #endregion

    #region Standard IDispose implementation

    // ------------------------------------------------------------------
    /// <summary>
    /// Disposes the view.
    /// </summary>
    // ------------------------------------------------------------------
    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Disposes the view.
    /// </summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    // ------------------------------------------------------------------
    private void Dispose(bool disposing) {
      if (disposing) {

      }

    }

    #endregion
  }
}
