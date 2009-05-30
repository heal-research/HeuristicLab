using System.Drawing;

namespace HeuristicLab.Visualization.Drawing {
  /// <summary>
  /// This is the base interface that has to be implemented by all shapes.
  /// 
  /// View port
  ///   The view port is the available drawing area on the screen.
  ///   It is specified in the GDI coordinate system.
  /// 
  /// Clipping area
  ///   The clipping area is the range of values visible on the view port.
  ///   Values outside this area will not be visible (they "get clipped").
  ///   It can either be inherited from the parent, or it can be overriden
  ///   to create distorted realities (see world shapes).
  ///   It is specified in a world coordinate system.
  /// 
  /// Bounding box
  ///   The bounding box is the location and the size of a shape on its parent.
  ///   It is specified in the parent's world coordinate system.
  /// </summary>
  public interface IShape {
    /// <summary>
    /// Draws the shape
    /// </summary>
    /// <param name="graphics">The Graphics object used to draw the shape</param>
    void Draw(Graphics graphics);

    /// <summary>
    /// The shape's bounding box
    /// </summary>
    RectangleD BoundingBox { get; }

    RectangleD ClippingArea { get; }
    Rectangle Viewport { get; }

    IShape Parent { get; set; }
  }
}