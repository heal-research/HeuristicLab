using System;
using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {
  public class CompositeShape : IShape {
    private IShape parent;
    private bool showChildShapes = true;

    protected readonly List<IShape> shapes = new List<IShape>();
    protected RectangleD boundingBox = RectangleD.Empty;



    public virtual void Draw(Graphics graphics) {
      if(!showChildShapes)
        return;
      foreach (IShape shape in shapes) {
        shape.Draw(graphics);
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

    public RectangleD ClippingArea {
      get { return Parent.ClippingArea; }
    }

    public Rectangle Viewport {
      get { return Parent.Viewport; }
    }

    public IShape Parent {
      get { return parent; }
      set { parent = value; }
    }

    public bool ShowChildShapes {
      get { return showChildShapes; }
      set { showChildShapes = value; }
    }

    public void ClearShapes() {
      shapes.Clear();
      boundingBox = RectangleD.Empty;
    }

    public IShape GetShape(int index) {
      return shapes[index];
    }

    public void AddShape(IShape shape) {
      shape.Parent = this;

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