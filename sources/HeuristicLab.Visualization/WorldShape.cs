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

    protected readonly List<IShape> shapes = new List<IShape>();

    public WorldShape()
      : this(new RectangleD(0, 0, 1, 1), new RectangleD(0, 0, 1, 1)) {}

    /// <param name="clippingArea">The new clipping area of this world shape</param>
    /// <param name="boundingBox">The location and the size of this world shape in the parent's coordinate system</param>
    public WorldShape(RectangleD clippingArea, RectangleD boundingBox) {
      this.clippingArea = clippingArea;
      this.boundingBox = boundingBox;
    }

    public virtual void Draw(Graphics graphics, Rectangle parentViewport, RectangleD parentClippingArea) {
      GraphicsState gstate = graphics.Save();

      // calculate our drawing area on the screen using our location and
      // size in the parent (boundingBox), the parent's viewport and the
      // parent's clipping area
      Rectangle viewport = Transform.ToScreen(boundingBox, parentViewport, parentClippingArea);

      graphics.SetClip(viewport);

      foreach (IShape shape in shapes) {
        // draw child shapes using our own clipping area
        shape.Draw(graphics, viewport, clippingArea);
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

    public void AddShape(IShape shape) {
      shapes.Add(shape);
    }

    public bool RemoveShape(IShape shape) {
      return shapes.Remove(shape);
    }
  }
}