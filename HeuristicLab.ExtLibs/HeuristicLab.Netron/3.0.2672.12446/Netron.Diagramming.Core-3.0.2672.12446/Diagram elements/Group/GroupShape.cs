using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// This entity represents a group of entities and corresponds to the 
  /// '(un)grouping' feature.
  /// </summary>
  // ----------------------------------------------------------------------
  partial class GroupShape : DiagramEntityBase, IGroup {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// GroupShape.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double groupShapeVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// The spacing between the bounds (union) of the entities and the
    /// outter edge of the group.
    /// </summary>
    // ------------------------------------------------------------------
    protected int myMargin = 10;

    // ------------------------------------------------------------------
    /// <summary>
    /// The Entities field.
    /// </summary>
    // ------------------------------------------------------------------
    private CollectionBase<IDiagramEntity> mEntities;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if ungrouping is allowed.
    /// </summary>
    // ------------------------------------------------------------------
    bool mAllowUnGroup = false;

    // ------------------------------------------------------------------
    /// <summary>
    /// in essence, whether the group shape should be painted.
    /// </summary>
    // ------------------------------------------------------------------
    private bool mEmphasizeGroup = true;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return groupShapeVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the current magnification level used by the view.
    /// Overriden to update each entity in this group.
    /// </summary>
    // ------------------------------------------------------------------
    public override SizeF Magnification {
      get {
        return base.Magnification;
      }
      set {
        base.Magnification = value;
        foreach (IDiagramEntity entiy in Entities) {
          entiy.Magnification = value;
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// The spacing between the bounds (union) of the entities and the
    /// outter edge of the group.
    /// </summary>
    // ------------------------------------------------------------------
    public int Margin {
      get {
        return myMargin;
      }
      set {
        myMargin = value;
        Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets whether the group as a shape should be painted on 
    /// the canvas.
    /// </summary>
    /// <value><c>true</c> To paint the group shape; otherwise, 
    /// <c>false</c>.</value>
    // ------------------------------------------------------------------
    public bool EmphasizeGroup {
      get { return mEmphasizeGroup; }
      set { mEmphasizeGroup = value; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if un-grouping of the entities is allowed.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual bool CanUnGroup {
      get {
        return mAllowUnGroup;
      }
      set {
        mAllowUnGroup = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the friendly name of the entity to be displayed in the UI.
    /// </summary>
    /// <value></value>
    // ------------------------------------------------------------------
    public override string EntityName {
      get {
        return "Group shape";
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the rectangle.
    /// </summary>
    /// <value>The rectangle.</value>
    // ------------------------------------------------------------------
    public override Rectangle Rectangle {
      get {
        return mRectangle;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the entities directly underneath. To get the whole 
    /// branch of entities in case of nested groups, <see cref="Leafs"/>.
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<IDiagramEntity> Entities {
      get {
        return mEntities;
      }
      set {
        mEntities = value;
        AttachEventsToEnityCollection(mEntities);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the whole branch of entities if this group has sub-groups. 
    /// The result will not contain the group shapes.
    /// </summary>
    /// <value>The branch.</value>
    // ------------------------------------------------------------------
    public CollectionBase<IDiagramEntity> Leafs {
      get {
        CollectionBase<IDiagramEntity> flatList =
            new CollectionBase<IDiagramEntity>();

        foreach (IDiagramEntity entity in mEntities) {
          if (entity is IGroup)
            Utils.TraverseCollect(entity as IGroup, ref flatList);
          else
            flatList.Add(entity);
        }
        return flatList;
      }
    }
    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    /// <summary>
    /// Default constructor.
    /// </summary>
    // ------------------------------------------------------------------
    public GroupShape()
      : base() {
      this.PaintStyle = null;
      PenStyle = null;
    }

    // ------------------------------------------------------------------
    ///<summary>
    ///Constructor that receives the parent Model.
    ///</summary>
    ///<param name="model">Model</param>
    // ------------------------------------------------------------------
    public GroupShape(IModel model)
      : base(model) {
      this.PaintStyle = null;
      PenStyle = null;
    }
    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes the group shape.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void Initialize() {
      base.Initialize();
      this.mEntities = new CollectionBase<IDiagramEntity>();
      AttachEventsToEnityCollection(mEntities);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Hooks-up the required events to monitor the collection of entities
    /// specified.
    /// </summary>
    /// <param name="entities">CollectionBase<IDiagramEntity></param>
    // ------------------------------------------------------------------
    protected virtual void AttachEventsToEnityCollection(
        CollectionBase<IDiagramEntity> entities) {
      this.mEntities.OnItemAdded +=
          new EventHandler<CollectionEventArgs<IDiagramEntity>>(
          OnEntityAdded);

      this.mEntities.OnClear += new EventHandler(OnClearEntities);

      this.mEntities.OnItemRemoved +=
          new EventHandler<CollectionEventArgs<IDiagramEntity>>(
          OnEntityRemoved);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// The custom menu to be added to the base menu of this entity.
    /// </summary>
    /// <returns>ToolStripItem[]</returns>
    // ------------------------------------------------------------------
    public override ToolStripItem[] Menu() {
      return null;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnItemRemoved of the Entities
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">CollectionEventArgs<IDiagramEntity></param>
    // ------------------------------------------------------------------
    protected virtual void OnEntityRemoved(
        object sender,
        CollectionEventArgs<IDiagramEntity> e) {
      CalculateRectangle();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnClear event of the Entities.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> Instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    protected virtual void OnClearEntities(object sender, EventArgs e) {
      mRectangle = Rectangle.Empty;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnItemAdded event of the Entities
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    // ------------------------------------------------------------------
    protected virtual void OnEntityAdded(
        object sender,
        CollectionEventArgs<IDiagramEntity> e) {
      // I'm changing the behavior of the Group.  Rather than the
      // entities still being attached to the Model and this not,
      // I'm switching that - the Group is added to the Model and
      // the entities are removed because they're contained within
      // this.
      if (Model.Paintables.Contains(e.Item)) {
        Model.Paintables.Remove(e.Item);
      }
      e.Item.Group = this;
      CalculateRectangle();
      Invalidate();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Calculates the bounding rectangle of this group.
    /// </summary>
    // ------------------------------------------------------------------
    public void CalculateRectangle() {
      if (mEntities == null || mEntities.Count == 0)
        return;
      Rectangle rec = mEntities[0].Rectangle;
      foreach (IDiagramEntity entity in Entities) {
        //cascade the calculation if necessary
        if (entity is IGroup) (entity as IGroup).CalculateRectangle();

        rec = Rectangle.Union(rec, entity.Rectangle);
      }

      rec.Inflate(myMargin, myMargin);
      this.mRectangle = rec;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the entity on the control
    /// <remarks>This method should not be called since the painting 
    /// occurs via the <see cref="Model.Paintables"/>.
    /// Use the <see cref="CollapsibleGroupShape"/> if you want a visible 
    /// group.
    /// </remarks>
    /// </summary>
    /// <param name="g">Graphics</param>
    // ------------------------------------------------------------------
    public override void Paint(Graphics g) {
      // Only draw the background and border of the group if we're
      // supposed to.
      if (EmphasizeGroup) {
        if (mPaintStyle != null) {
          g.FillRectangle(mPaintStyle.GetBrush(mRectangle), mRectangle);
        }

        if ((Hovered) && (ArtPalette.EnableShadows)) {
          g.DrawRectangle(ArtPalette.HighlightPen, mRectangle);
        } else if (mPenStyle != null) {
          g.DrawRectangle(mPenStyle.DrawingPen(), mRectangle);
        }
      }

      // Now draw each entity (child) in this group.
      foreach (IDiagramEntity entity in mEntities) {
        entity.Paint(g);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Tests whether the group is hit by the mouse
    /// </summary>
    /// <param name="p">Point</param>
    /// <returns>bool</returns>
    // ------------------------------------------------------------------
    public override bool Hit(Point p) {
      if (EmphasizeGroup) {
        Rectangle r = new Rectangle(p, new Size(5, 5));
        return Rectangle.Contains(r);
      } else {
        foreach (IDiagramEntity entity in mEntities) {
          if (entity.Hit(p)) {
            return true;
          }
        }

        // If we made it this far then no entity was hit so
        // return false.
        return false;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Invalidates the entity
    /// </summary>
    // ------------------------------------------------------------------
    public override void Invalidate() {

      if (mRectangle == null)
        return;

      Rectangle rec = mRectangle;
      rec.Inflate(20, 20);
      Model.RaiseOnInvalidateRectangle(rec);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Moves the entity on the canvas
    /// </summary>
    /// <param name="p">Point</param>
    // ------------------------------------------------------------------
    public override void MoveBy(Point p) {

      Rectangle recBefore = mRectangle;
      recBefore.Inflate(20, 20);

      // No need to invalidate since it'll be done by the individual 
      // move actions.
      foreach (IDiagramEntity entity in mEntities) {
        entity.MoveBy(p);
      }
      mRectangle.X += p.X;
      mRectangle.Y += p.Y;

      //refresh things
      this.Invalidate(recBefore);//position before the move
      this.Invalidate();//current position

    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Passes the mouse down event to all entities in the collection.
    /// </summary>
    /// <param name="e">MouseEventArgs</param>
    /// <returns>bool: Returns 'true' if the mouse down event was
    /// handled here, 'false' if not.</returns>
    // ------------------------------------------------------------------
    public override bool MouseDown(MouseEventArgs e) {
      base.MouseDown(e);
      bool result = false;

      // Only pass the mouse down event on if we're already selected.
      if (IsSelected == false) {
        return false;
      }

      foreach (IDiagramEntity entity in mEntities) {
        if (entity.Hit(e.Location)) {
          if (entity.MouseDown(e)) {
            result = true;
          }
        }
      }
      return result;
    }
  }
}
