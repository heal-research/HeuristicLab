using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HeuristicLab.Visualization {
  public class WorldShape : IShape {
    private RectangleD clippingArea;
    private RectangleD boundingBox;

    private readonly List<IShape> shapes = new List<IShape>();

    public WorldShape(RectangleD clippingArea, RectangleD boundingBox) {
      this.clippingArea = clippingArea;
      this.boundingBox = boundingBox;
    }

    public void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      GraphicsState gstate = graphics.Save();

      Rectangle innerViewport = Transform.ToScreen(boundingBox, viewport, clippingArea);

      graphics.SetClip(innerViewport);
      
      foreach (IShape shape in shapes) {
        shape.Draw(graphics, innerViewport, this.clippingArea);
      }
      
      graphics.Restore(gstate);
    }

    /// <summary>
    /// 
    /// </summary>
    public RectangleD BoundingBox {
      get { return boundingBox; }
    }

    /// <summary>
    /// adds a shape to the WorldShape
    /// </summary>
    /// <param name="shape">shape to add</param>
    public void AddShape(IShape shape) {
      shapes.Add(shape);
    }
  }
}
