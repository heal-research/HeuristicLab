using System;
using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {
  public class CompositeShape : IShape {
    protected readonly List<IShape> shapes = new List<IShape>();
    protected RectangleD boundingBox = RectangleD.Empty;

    public virtual void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      foreach (IShape shape in shapes) {
        shape.Draw(graphics, viewport, clippingArea);
      }
    }

    public RectangleD BoundingBox {
      get {
        if (shapes.Count == 0) {
          throw new InvalidOperationException("No shapes, no bounding box.");
        }

        return boundingBox;
      }
    }

    public void ClearShapes() {
      shapes.Clear();
      boundingBox = RectangleD.Empty;
    }

    public void AddShape(IShape shape) {
      if (shapes.Count == 0) {
        boundingBox = shape.BoundingBox;
      } else {
        boundingBox = new RectangleD(Math.Min(boundingBox.X1, shape.BoundingBox.X1),
                                     Math.Min(boundingBox.Y1, shape.BoundingBox.Y1),
                                     Math.Max(boundingBox.X2, shape.BoundingBox.X2),
                                     Math.Max(boundingBox.Y2, shape.BoundingBox.Y2));
      }
      shapes.Add(shape);
    }
  }
}