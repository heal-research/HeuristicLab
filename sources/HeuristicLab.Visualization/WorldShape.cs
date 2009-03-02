using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HeuristicLab.Visualization {
  /// <summary>
  /// World shapes are composite shapes that have their own coordinate system
  /// which is independent from their parent's coordinate system.
  /// </summary>
  public class WorldShape : IShape {
    private RectangleD clippingArea; // own clipping area
    private RectangleD boundingBox;
    private IShape parent;

    private readonly List<IShape> shapes = new List<IShape>();

    public WorldShape() {
      this.clippingArea = new RectangleD(0, 0, 1, 1);
      this.boundingBox = new RectangleD(0, 0, 1, 1);
    }

    public virtual void Draw(Graphics graphics) {
      GraphicsState gstate = graphics.Save();

      graphics.SetClip(Viewport);

      foreach (IShape shape in shapes) {
        // draw child shapes using our own clipping area
        shape.Draw(graphics);
      }

      graphics.Restore(gstate);
    }

    public RectangleD BoundingBox {
      get { return boundingBox; }
      set { boundingBox = value; }
    }

    /// <summary>
    /// The world shape's own clipping area.
    /// This overrides the clipping area of the parent shape.
    /// </summary>
    public RectangleD ClippingArea {
      get { return clippingArea; }
      set { clippingArea = value; }
    }

    public Rectangle Viewport {
      get {
        // calculate our drawing area on the screen using our location and
        // size in the parent (boundingBox), the parent's viewport and the
        // parent's clipping area
        Rectangle viewport = Transform.ToScreen(boundingBox, Parent.Viewport, Parent.ClippingArea);
        return viewport;
      }
    }

    public IShape Parent {
      get { return parent; }
      set { parent = value; }
    }

    public void ClearShapes() {
      shapes.Clear();
    }

    public void AddShape(IShape shape) {
      shape.Parent = this;
      shapes.Add(shape);
    }

    public bool RemoveShape(IShape shape) {
      return shapes.Remove(shape);
    }
  }
}