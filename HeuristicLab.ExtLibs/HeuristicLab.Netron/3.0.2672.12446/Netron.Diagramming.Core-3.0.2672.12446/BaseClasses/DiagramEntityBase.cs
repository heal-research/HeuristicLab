using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Abstract base class for every diagram entity.
  /// </summary>
  // ----------------------------------------------------------------------
  public abstract partial class DiagramEntityBase :
      IDiagramEntity,
      IMouseListener,
      IHoverListener,
      IKeyboardListener,
      IDisposable {
    #region Events

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the user click on the entity.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityEventArgs> OnClick;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when a mouse button is pressed while over the entity.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityMouseEventArgs> OnMouseDown;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when a mouse button is released while over the entity.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityMouseEventArgs> OnMouseUp;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the mouse is moved while over the entity.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityMouseEventArgs> OnMouseMove;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the mouse enters the entity.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityMouseEventArgs> OnMouseEnter;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the mouse hovers over the entity.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityMouseEventArgs> OnMouseHover;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the mouse leaves the entity.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityMouseEventArgs> OnMouseLeave;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the entity's properties have changed
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityEventArgs> OnEntityChange;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the entity is selected. This can be different than the 
    /// OnClick because the selector can select and entity without 
    /// clicking on it.
    /// </summary>
    // ------------------------------------------------------------------
    public event EventHandler<EntityEventArgs> OnEntitySelect;

    #endregion

    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// DiagramEntityBase.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double diagramEntityBaseVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// The services of this entity.
    /// </summary>
    // ------------------------------------------------------------------
    protected Dictionary<Type, IInteraction> mServices;

    // ------------------------------------------------------------------
    /// <summary>
    /// The Rectangle on which any bundle lives.
    /// </summary>
    // ------------------------------------------------------------------
    protected Rectangle mRectangle = Rectangle.Empty;

    // ------------------------------------------------------------------
    /// <summary>
    /// General prupose tag
    /// </summary>
    // ------------------------------------------------------------------
    protected object mTag;

    // ------------------------------------------------------------------
    /// <summary>
    /// tells whether the current entity is hovered by the mouse
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mHovered;

    // ------------------------------------------------------------------
    /// <summary>
    /// The current magnification of the view.
    /// </summary>
    // ------------------------------------------------------------------
    protected SizeF mMagnification = new SizeF(100F, 100F);

    // ------------------------------------------------------------------
    /// <summary>
    /// The Model to which the eneity belongs.
    /// </summary>
    // ------------------------------------------------------------------
    protected IModel mModel;

    // ------------------------------------------------------------------
    /// <summary>
    /// The layer to which this entity is attached in the Model.
    /// </summary>
    // ------------------------------------------------------------------
    protected ILayer mLayer;

    // ------------------------------------------------------------------
    /// <summary>
    /// tells whether the entity is selected
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mIsSelected;

    // ------------------------------------------------------------------
    /// <summary>
    /// the current draw style
    /// </summary>
    // ------------------------------------------------------------------
    protected IPenStyle mPenStyle;

    // ------------------------------------------------------------------
    /// <summary>
    /// the current paint style
    /// </summary>
    // ------------------------------------------------------------------
    protected IPaintStyle mPaintStyle;

    // ------------------------------------------------------------------
    /// <summary>
    /// the default pen to be used by the Paint method
    /// </summary>
    // ------------------------------------------------------------------
    protected Pen mPen;

    // ------------------------------------------------------------------
    /// <summary>
    /// the default brush to be used by the Paint method
    /// </summary>
    // ------------------------------------------------------------------
    protected Brush mBrush;

    // ------------------------------------------------------------------
    /// <summary>
    /// the name of the entity
    /// </summary>
    // ------------------------------------------------------------------
    protected string mName;

    // ------------------------------------------------------------------
    /// <summary>
    /// a weak reference to the parent
    /// </summary>
    // ------------------------------------------------------------------
    protected WeakReference mParent;

    // ------------------------------------------------------------------
    /// <summary>
    /// the scene index, i.e. the index of this entity in the scene-graph.
    /// </summary>
    // ------------------------------------------------------------------
    protected int mSceneIndex;

    // ------------------------------------------------------------------
    /// <summary>
    /// The top-group to underneath which this entity resides.
    /// </summary>
    // ------------------------------------------------------------------
    protected IGroup mGroup;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if the entity can be moved.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mAllowMove = true;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if the entity can be deleted.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mAllowDelete = true;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if the entity can be resized.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mResizable = true;

    // ------------------------------------------------------------------
    /// <summary>
    /// The minimum size this entity can be.  The default value is:
    /// width = 10, height = 10.  This seems to be the min size that
    /// keeps the connectors relative position correct during a Transform.
    /// </summary>
    // ------------------------------------------------------------------
    protected Size myMinSize = new Size(10, 10);

    // ------------------------------------------------------------------
    /// <summary>
    /// The maximum size this entity can be.  The default value is:
    /// width = 10000, height = 10000.
    /// </summary>
    // ------------------------------------------------------------------
    protected Size myMaxSize = new Size(10000, 10000);

    // ------------------------------------------------------------------
    /// <summary>
    /// The unique identifier of this entity
    /// </summary>
    // ------------------------------------------------------------------
    protected Guid mUid = Guid.NewGuid();

    // ------------------------------------------------------------------
    /// <summary>
    /// whether the entity is visible
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mVisible = true;

    // ------------------------------------------------------------------
    /// <summary>
    /// The Enabled field.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mEnabled = true;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual double Version {
      get {
        return diagramEntityBaseVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the services provided by this entity.
    /// </summary>
    /// <value>The services.</value>
    // ------------------------------------------------------------------
    public Dictionary<Type, IInteraction> Services {
      get { return mServices; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets whether this entity is Enabled.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual bool Enabled {
      get {
        return mEnabled;
      }
      set {
        mEnabled = value;
        RaiseOnChange(this, new EntityEventArgs(this));
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this entity is visible.
    /// </summary>
    /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
    // ------------------------------------------------------------------
    public virtual bool Visible {
      get {
        return mVisible;
      }
      set {
        mVisible = value;
        RaiseOnChange(this, new EntityEventArgs(this));
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the drawing style.
    /// </summary>
    /// <value>The draw style.</value>
    // ------------------------------------------------------------------
    public virtual IPenStyle PenStyle {
      get {
        return mPenStyle;
      }
      set {
        mPenStyle = value;
        RaiseOnChange(this, new EntityEventArgs(this));
        UpdatePaintingMaterial();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the paint style.
    /// </summary>
    /// <value>The paint style.</value>
    // ------------------------------------------------------------------
    public virtual IPaintStyle PaintStyle {
      get {
        return mPaintStyle;
      }
      set {
        mPaintStyle = value;
        RaiseOnChange(this, new EntityEventArgs(this));
        UpdatePaintingMaterial();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the globally unique identifier of this entity
    /// </summary>
    /// <value></value>
    // ------------------------------------------------------------------
    public virtual Guid Uid {
      get {
        return mUid;
      }

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this 
    /// <see cref="IDiagramEntity"/> can be moved.
    /// </summary>
    /// <value><c>true</c> if movable; otherwise, <c>false</c>.</value>
    // ------------------------------------------------------------------
    public virtual bool AllowMove {
      get {
        return mAllowMove;
      }
      set {
        mAllowMove = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this 
    /// <see cref="IDiagramEntity"/> can be deleted.
    /// </summary>
    /// <value><c>true</c> if deletable; otherwise, <c>false</c>.</value>
    // ------------------------------------------------------------------
    public virtual bool AllowDelete {
      get {
        return mAllowDelete;
      }
      set {
        mAllowDelete = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Resizable
    /// </summary>
    // ------------------------------------------------------------------
    public virtual bool Resizable {
      get {
        return mResizable;
      }
      set {
        mResizable = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the minimum size of the entity.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Size MinSize {
      get {
        return myMinSize;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the maximum size of the entity.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Size MaxSize {
      get {
        return myMaxSize;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the <see cref="Brush"/> to paint this entity.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Brush Brush {
      get {
        return mBrush;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the pen to draw this entity.
    /// </summary>
    /// <value>The pen.</value>
    // ------------------------------------------------------------------
    public virtual Pen Pen {
      get {
        return mPen;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the friendly name of the entity to be displayed in the UI.
    /// </summary>
    /// <value></value>
    // ------------------------------------------------------------------
    public abstract string EntityName {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the general purpose tag
    /// </summary>
    // ------------------------------------------------------------------
    public virtual object Tag {
      get {
        return mTag;
      }
      set {
        mTag = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the index of this entity in the scene-graph.
    /// </summary>
    /// <value>The index of the scene.</value>
    // ------------------------------------------------------------------
    public virtual int SceneIndex {
      get {
        return mSceneIndex;
      }
      set {
        mSceneIndex = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the unique top-group to which this entity belongs.
    /// </summary>
    /// <value></value>
    // ------------------------------------------------------------------
    public virtual IGroup Group {
      get {
        return mGroup;
      }
      set {
        mGroup = value;
        // Propagate downwards if this is a group shape, but not if 
        // the value is 'null' since the group becomes the value of 
        // the Group property.  Note that we could have used a formal 
        // depth-traversal algorithm.
        if (this is IGroup) {
          if (value == null)//occurs on an ungroup action
                    {
            foreach (IDiagramEntity entity in
                (this as IGroup).Entities) {
              entity.Group = this as IGroup;
            }
          } else //occurs when grouping
                    {
            foreach (IDiagramEntity entity in
                (this as IGroup).Entities) {
              entity.Group = value;
            }
          }
        }

      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets whether the entity is hovered by the mouse
    /// </summary>
    // ------------------------------------------------------------------
    public virtual bool Hovered {
      get {
        return mHovered;
      }
      set {
        mHovered = value;
        Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the parent of the entity
    /// </summary>
    // ------------------------------------------------------------------
    public virtual object Parent {
      get {
        if (mParent != null && mParent.IsAlive) {
          return mParent.Target;
        } else {
          return null;
        }
      }
      set {
        mParent = new WeakReference(value);
        RaiseOnChange(this, new EntityEventArgs(this));
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the name of the entity
    /// </summary>
    // ------------------------------------------------------------------
    public virtual string Name {
      get {
        return mName;
      }
      set {
        mName = value;
        RaiseOnChange(this, new EntityEventArgs(this));
      }
    }

    #region Bounds and point calculations

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the bounds of the paintable entity.
    /// </summary>
    /// <value></value>
    // ------------------------------------------------------------------
    public abstract Rectangle Rectangle {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the top left corner of this entity, which is the same as
    /// 'Rectangle.Location'.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Point TopLeftCorner {
      get {
        return this.Rectangle.Location;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the top right corner of this entity.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Point TopRightCorner {
      get {
        return new Point(
            Rectangle.Right,
            Rectangle.Top);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the bottom left corner of this entity.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Point BottomLeftCorner {
      get {
        return new Point(
            Rectangle.Left,
            Rectangle.Bottom);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the bottom right corner of this entity.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Point BottomRightCorner {
      get {
        return new Point(
            Rectangle.Right,
            Rectangle.Bottom);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the center point of the paintable entity (the center of the
    /// Rectangle).
    /// </summary>
    /// <value>Point</value>
    // ------------------------------------------------------------------
    public virtual Point Center {
      get {
        // Make sure the bounds are legal first.
        if ((this.mRectangle == null) ||
            (this.mRectangle == Rectangle.Empty)) {
          return Point.Empty;
        }

        int x =
            (this.mRectangle.Left) +
            (this.mRectangle.Width / 2);

        int y =
            (this.mRectangle.Top) +
            (this.mRectangle.Height / 2);
        return new Point(x, y);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the center of the bottom edge of the bounding rectangle.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Point BottomCenter {
      get {
        return new Point(
            BottomLeftCorner.X + (mRectangle.Width / 2),
            BottomLeftCorner.Y);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the center of the top edge of the bounding rectangle.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Point TopCenter {
      get {
        return new Point(
            TopLeftCorner.X + (mRectangle.Width / 2),
            TopLeftCorner.Y);
      }
    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets whether the entity is selected
    /// </summary>
    // ------------------------------------------------------------------
    [Browsable(false)]
    public virtual bool IsSelected {
      get {
        return mIsSelected;
      }
      set {
        mIsSelected = value;
        if (value == true) {
          this.RaiseOnSelect(this, new EntityEventArgs(this));
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the current magnification used by the view.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual SizeF Magnification {
      get {
        return mMagnification;
      }
      set {
        mMagnification = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the canvas to which the entity belongs.
    /// </summary>
    // ------------------------------------------------------------------
    [Browsable(false)]
    public virtual IModel Model {
      get {
        return mModel;
      }
      set {
        mModel = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the ILayer this entity is attached to in the IModel.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual ILayer Layer {
      get {
        return mLayer;
      }
      set {
        mLayer = value;
      }
    }

    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor with the model of the entity.
    /// </summary>
    /// <param mName="model">IModel</param>
    // ------------------------------------------------------------------
    protected DiagramEntityBase(IModel model) {
      this.mModel = model;
      Initialize();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// The empty constructor is required to make deserialization work.
    /// </summary>
    // ------------------------------------------------------------------
    protected DiagramEntityBase() {
      Initialize();
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Called after an entity is deleted.
    /// </summary>
    /// <param name="deleteCommand">DeleteCommand: The un/redoable command
    /// that's part of the undo/redo mechanism.</param>
    // ------------------------------------------------------------------
    public virtual void OnAfterDelete(DeleteCommand deleteCommand) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Called before an entity is deleted.
    /// </summary>
    /// <param name="deleteCommand">DeleteCommand: The un/redoable command
    /// that's part of the undo/redo mechanism.</param>
    // ------------------------------------------------------------------
    public virtual void OnBeforeDelete(DeleteCommand deleteCommand) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when a new DiagramEntityBase is instantiated.
    /// </summary>
    // ------------------------------------------------------------------
    protected virtual void Initialize() {
      PaintStyle = ArtPalette.GetDefaultPaintStyle();
      PenStyle = ArtPalette.GetDefaultPenStyle();

      mServices = new Dictionary<Type, IInteraction>();
      mServices[typeof(IMouseListener)] = this;
      mServices[typeof(IHoverListener)] = this;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Generates a new Uid for this entity.
    /// </summary>
    /// <param name="recursive">if the Uid has to be changed recursively 
    /// down to the sub-entities, set to true, otherwise false.</param>
    // ------------------------------------------------------------------
    public virtual void NewUid(bool recursive) {
      this.mUid = Guid.NewGuid();
      RaiseOnChange(this, new EntityEventArgs(this));
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Defines a mechanism for retrieving a service object; that is, an 
    /// object that provides custom support to other objects.
    /// </summary>
    /// <param name="serviceType">An object that specifies the type of 
    /// service object to get.</param>
    /// <returns>
    /// A service object of type serviceType.-or- null if there is no 
    /// service object of type serviceType.
    /// </returns>
    // ------------------------------------------------------------------
    public virtual object GetService(Type serviceType) {
      if (Services.ContainsKey(serviceType)) {
        return Services[serviceType];
      } else {
        return null;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the entity on the control
    /// </summary>
    /// <param mName="g">the graphics object to paint on</param>
    // ------------------------------------------------------------------
    public abstract void Paint(Graphics g);

    // ------------------------------------------------------------------
    /// <summary>
    /// Tests whether the entity is hit by the mouse
    /// </summary>
    /// <param>a Point location</param>
    /// <param name="p"></param>
    // ------------------------------------------------------------------
    public abstract bool Hit(Point p);

    // ------------------------------------------------------------------
    /// <summary>
    /// Invalidates the entity
    /// </summary>
    // ------------------------------------------------------------------
    public abstract void Invalidate();

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the entity is detached from the canvas (temporarily
    /// removed but not disposed, like in a cut operation).
    /// </summary>
    // ------------------------------------------------------------------
    public virtual void Detached(ILayer layer) {
      mLayer = null;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the entity is attached to a Layer.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual void Attached(ILayer layer) {
      mLayer = layer;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Invalidates a rectangle of the canvas
    /// </summary>
    /// <param name="rectangle"></param>
    // ------------------------------------------------------------------
    public virtual void Invalidate(Rectangle rectangle) {
      if (Model != null)
        Model.RaiseOnInvalidateRectangle(rectangle);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Updates pens and brushes
    /// </summary>
    // ------------------------------------------------------------------
    protected virtual void UpdatePaintingMaterial() {
      // First make sure we have a valid rectangle.
      if (mRectangle.Width == 0) {
        mRectangle.Width = 1;
      }

      if (mRectangle.Height == 0) {
        mRectangle.Height = 1;
      }

      if (mPenStyle != null) {
        mPen = mPenStyle.DrawingPen();
      }

      if (mPaintStyle != null) {
        mBrush = mPaintStyle.GetBrush(this.Rectangle);
      }

      Invalidate();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Moves the entity on the canvas
    /// </summary>
    /// <param mName="p">the shifting vector, not an absolute 
    /// position!</param>
    // ------------------------------------------------------------------
    public abstract void MoveBy(Point p);

    // ------------------------------------------------------------------
    /// <summary>
    /// The custom elements to be added to the menu on a per-entity basis.
    /// </summary>
    /// <returns>ToolStripItem[]</returns>
    // ------------------------------------------------------------------
    public abstract ToolStripItem[] Menu();

    #region Raisers

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the onclick event.
    /// </summary>
    /// <param name="e"></param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnClick(EntityEventArgs e) {
      if (OnClick != null)
        OnClick(this, e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseDown event.
    /// </summary>
    /// <param name="e">EntityMouseEventArgs</param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnMouseDown(EntityMouseEventArgs e) {
      if (OnMouseDown != null) {
        OnMouseDown(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseUp event.
    /// </summary>
    /// <param name="e">EntityMouseEventArgs</param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnMouseUp(EntityMouseEventArgs e) {
      if (OnMouseUp != null) {
        OnMouseUp(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseMove event.
    /// </summary>
    /// <param name="e">EntityMouseEventArgs</param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnMouseMove(EntityMouseEventArgs e) {
      if (OnMouseMove != null) {
        OnMouseMove(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseEnter event.
    /// </summary>
    /// <param name="e">EntityMouseEventArgs</param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnMouseEnter(EntityMouseEventArgs e) {
      if (OnMouseEnter != null) {
        OnMouseEnter(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseEnter event.
    /// </summary>
    /// <param name="e">EntityMouseEventArgs</param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnMouseHover(EntityMouseEventArgs e) {
      if (OnMouseHover != null) {
        OnMouseHover(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseLeave event.
    /// </summary>
    /// <param name="e">EntityMouseEventArgs</param>
    // ------------------------------------------------------------------
    public virtual void RaiseOnMouseLeave(EntityMouseEventArgs e) {
      if (OnMouseLeave != null) {
        OnMouseLeave(this, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnSelect event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    protected virtual void RaiseOnSelect(
        object sender,
        EntityEventArgs e) {
      if (OnEntitySelect != null) {
        OnEntitySelect(sender, e);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnChange event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    protected virtual void RaiseOnChange(object sender, EntityEventArgs e) {
      if (OnEntityChange != null)
        OnEntityChange(sender, e);
    }

    #endregion

    #endregion

    #region Standard IDispose implementation
    /// <summary>
    /// Disposes the entity.
    /// </summary>
    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);


    }

    /// <summary>
    /// Disposes the entity.
    /// </summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected virtual void Dispose(bool disposing) {
      if (disposing) {
        #region free managed resources


        if (mPen != null) {
          mPen.Dispose();
          mPen = null;
        }
        if (mBrush != null) {
          mBrush.Dispose();
          mBrush = null;
        }
        #endregion
      }

    }

    #endregion

    #region IMouseListener Members

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of the <see cref="IMouseListener"/>.  This is
    /// the method called when a mouse button is pressed while over this 
    /// entity.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    /// <returns>bool: Whether or not the mouse down was handled by this
    /// entity.  The default here is false.  Sub-entities should override
    /// this to provide their own functionality if needed.</returns>
    // ------------------------------------------------------------------
    public virtual bool MouseDown(MouseEventArgs e) {
      this.RaiseOnMouseDown(new EntityMouseEventArgs(this, e));

      // By default we're not handling the mouse down event here,
      // we're just passing it on.  Let the sub-entities handle it 
      // by overriding this method.
      return false;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of the <see cref="IMouseListener"/>.  This is
    /// the method called when the mouse is moved while over this 
    /// entity.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void MouseMove(MouseEventArgs e) {
      this.RaiseOnMouseMove(new EntityMouseEventArgs(this, e));
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of the <see cref="IMouseListener"/>.  This is
    /// the method called when a mouse button is released while over this 
    /// entity.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void MouseUp(MouseEventArgs e) {
      this.RaiseOnMouseUp(new EntityMouseEventArgs(this, e));
    }

    #endregion

    #region IHoverListener Members

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of the <see cref="IHoverListener"/>.  This is
    /// the method called when the mouse hovers over this entity.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void MouseHover(MouseEventArgs e) {
      this.RaiseOnMouseHover(new EntityMouseEventArgs(this, e));
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of the <see cref="IHoverListener"/>.  This is
    /// the method called when the mouse enters this entity.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void MouseEnter(MouseEventArgs e) {
      this.RaiseOnMouseEnter(new EntityMouseEventArgs(this, e));
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of the <see cref="IHoverListener"/>.  This is
    /// the method called when the mouse leaves this entity.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void MouseLeave(MouseEventArgs e) {
      this.RaiseOnMouseLeave(new EntityMouseEventArgs(this, e));
    }

    #endregion

    #region IKeyboardListener Members

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of the <see cref="IKeyboardListener"/>.  This is
    /// the method called when a key is pressed down.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.KeyEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void KeyDown(KeyEventArgs e) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of the <see cref="IKeyboardListener"/>.  This is
    /// the method called when a key is released.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.KeyEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void KeyUp(KeyEventArgs e) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of the <see cref="IKeyboardListener"/>.  This is
    /// the method called when a key is pressed.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.KeyPressEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void KeyPress(KeyPressEventArgs e) {
    }

    #endregion
  }
}
