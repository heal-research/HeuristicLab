using System.Drawing;

namespace HeuristicLab.Visualization {
  public interface IShape {
    void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea);
    RectangleD BoundingBox { get; }
  }
}
