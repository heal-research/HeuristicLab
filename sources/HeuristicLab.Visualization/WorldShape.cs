using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HeuristicLab.Visualization {
  public class WorldShape : IShape {
    private RectangleD clippingArea;
    private RectangleD boundingBox;

    protected readonly List<IShape> shapes = new List<IShape>();

    public WorldShape()
      : this(new RectangleD(0, 0, 1, 1), new RectangleD(0, 0, 1, 1)) {}

    public WorldShape(RectangleD clippingArea, RectangleD boundingBox) {
      this.clippingArea = clippingArea;
      this.boundingBox = boundingBox;
    }

    public virtual void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      GraphicsState gstate = graphics.Save();

      Rectangle innerViewport = Transform.ToScreen(boundingBox, viewport, clippingArea);

      graphics.SetClip(innerViewport);

      foreach (IShape shape in shapes) {
        shape.Draw(graphics, innerViewport, this.clippingArea);
      }

      graphics.Restore(gstate);
    }

    public RectangleD BoundingBox {
      get { return boundingBox; }
      set { boundingBox = value; }
    }

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