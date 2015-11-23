using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Core {
  public class Shape : ShapeBase {
    // ------------------------------------------------------------------
    /// <summary>
    /// The collection of shapes.
    /// </summary>
    // ------------------------------------------------------------------
    CollectionBase<IDiagramEntity> myEntities;

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the friendly name of this shape.
    /// </summary>
    // ------------------------------------------------------------------
    public override string EntityName {
      get {
        return "Shape";
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or set the owned shapes.
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<IDiagramEntity> Entities {
      get {
        return myEntities;
      }
      set {
        myEntities = value;
      }
    }

    public virtual GraphicsPath GraphicsPath {
      get {
        GraphicsPath path = new GraphicsPath();
        path.AddRectangle(mRectangle);
        return path;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Default constructor.
    /// </summary>
    // ------------------------------------------------------------------
    public Shape()
      : base() {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor that receives the model this shape belongs to.
    /// </summary>
    /// <param name="model">IModel</param>
    // ------------------------------------------------------------------
    public Shape(IModel model)
      : base(model) {
    }

    protected override void Initialize() {
      base.Initialize();
      myEntities = new CollectionBase<IDiagramEntity>();
      AttachEventsToShapeCollection();
    }

    protected virtual void AttachEventsToShapeCollection() {
      myEntities.OnClear += new EventHandler(OnEntitiesClear);
      myEntities.OnItemAdded +=
          new EventHandler<CollectionEventArgs<IDiagramEntity>>(
          OnEntityAdded);
      myEntities.OnItemRemoved +=
          new EventHandler<CollectionEventArgs<IDiagramEntity>>(
          OnEntityRemoved);
    }

    void OnEntityRemoved(object sender, CollectionEventArgs<IDiagramEntity> e) {
      CalculateRectangle();
    }

    void OnEntityAdded(object sender, CollectionEventArgs<IDiagramEntity> e) {
      if (myEntities.Count == 1) {
        mRectangle = e.Item.Rectangle;
      } else {
        mRectangle = Rectangle.Union(
            (Rectangle)mRectangle,
            e.Item.Rectangle);
      }
    }

    void OnEntitiesClear(object sender, EventArgs e) {
      mRectangle = Rectangle.Empty;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Calculates the bounding rectangle of this shape from all children.
    /// </summary>
    // ------------------------------------------------------------------
    public void CalculateRectangle() {
      if (myEntities == null || myEntities.Count == 0)
        return;
      Rectangle rec = myEntities[0].Rectangle;
      foreach (IDiagramEntity entity in Entities) {
        //cascade the calculation if necessary
        if (entity is IGroup) (entity as IGroup).CalculateRectangle();

        rec = Rectangle.Union(rec, entity.Rectangle);
      }
      this.mRectangle = rec;
      this.mRectangle.Inflate(20, 20);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Tests whether the group is hit by the mouse
    /// </summary>
    /// <param name="p">Point</param>
    /// <returns>bool</returns>
    // ------------------------------------------------------------------
    public override bool Hit(Point p) {
      foreach (IDiagramEntity entity in myEntities) {
        if (entity.Hit(p)) {
          return true;
        }
      }
      return false;
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
      foreach (IDiagramEntity entity in myEntities) {
        entity.MoveBy(p);
      }
      mRectangle.X += p.X;
      mRectangle.Y += p.Y;

      //refresh things
      this.Invalidate(recBefore);//position before the move
      this.Invalidate();//current position

    }

    public override void Paint(Graphics g) {
      base.Paint(g);

      GraphicsPath path = GraphicsPath;
      if (PaintStyle != null) {
        g.FillPath(mPaintStyle.GetBrush(mRectangle), path);
      }

      if (Hovered) {
        g.DrawPath(ArtPalette.HighlightPen, path);
      } else if (PenStyle != null) {
        g.DrawPath(mPenStyle.DrawingPen(), path);
      }

      foreach (IDiagramEntity entity in myEntities) {
        entity.Paint(g);
      }
    }

    public override bool MouseDown(System.Windows.Forms.MouseEventArgs e) {
      bool result = base.MouseDown(e);

      foreach (IDiagramEntity entity in myEntities) {
        if (entity.Hit(e.Location)) {
          this.Model.Selection.SelectedItems.Add(entity);
          if (entity.MouseDown(e)) {
            result = true;
          }
        }
      }

      return result;
    }
  }
}
