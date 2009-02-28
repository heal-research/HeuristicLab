using System.Drawing;

namespace HeuristicLab.Visualization {
  /// <summary>
  /// GDI (or screen) coordinate system
  ///   The GDI coordinate system is specified in pixels. X goes from left to right
  ///   and Y goes from top to bottom.
  /// 
  /// World coordinate system
  ///   A world coordinate system can be freely specified. X goes from left to
  ///   right and Y goes from bottom to top.
  /// 
  /// The transformation between world- and screen-coordinate systems is done using
  /// a view port and a clipping area.
  /// </summary>
  public static class Transform {
    /// <summary>
    /// Transforms a rectangle in screen coordinates to world coordinates
    /// </summary>
    /// <param name="rect">The rectangle in screen coordinates that should be transformed</param>
    /// <param name="viewport">The target view port</param>
    /// <param name="clippingArea">The target clipping area</param>
    /// <returns>The rectangle rect transformed to world coordinates</returns>
    public static RectangleD ToWorld(Rectangle rect, Rectangle viewport, RectangleD clippingArea) {
      double x1 = ToWorldX(rect.Left, viewport, clippingArea);
      double y1 = ToWorldY(rect.Bottom, viewport, clippingArea);
      double x2 = ToWorldX(rect.Right, viewport, clippingArea);
      double y2 = ToWorldY(rect.Top, viewport, clippingArea);
      return new RectangleD(x1, y1, x2, y2);
    }

    /// <summary>
    /// Transforms a 2d point in screen coordinates to world coordinates
    /// </summary>
    /// <param name="point">The point in screen coordinates that should be transformed</param>
    /// <param name="viewport">The target view port</param>
    /// <param name="clippingArea">The target clipping area</param>
    /// <returns>The point transformed to world coordinates</returns>
    public static PointD ToWorld(Point point, Rectangle viewport, RectangleD clippingArea) {
      double x = ToWorldX(point.X, viewport, clippingArea);
      double y = ToWorldY(point.Y, viewport, clippingArea);
      return new PointD(x, y);
    }

    /// <summary>
    /// Transforms a point on the X-axis in screen coordinates to world coordinates
    /// </summary>
    /// <param name="x">The point on the X-axis in screen coordinates that should be transformed</param>
    /// <param name="viewport">The target view port</param>
    /// <param name="clippingArea">The target clipping area</param>
    /// <returns>The point transformed to world coordinates</returns>
    public static double ToWorldX(int x, Rectangle viewport, RectangleD clippingArea) {
      return clippingArea.X1 + clippingArea.Width/viewport.Width*(x - viewport.Left);
    }

    /// <summary>
    /// Transforms a point on the Y-axis in screen coordinates to world coordinates
    /// </summary>
    /// <param name="y">The point on the Y-axis in screen coordinates that should be transformed</param>
    /// <param name="viewport">The target view port</param>
    /// <param name="clippingArea">The target clipping area</param>
    /// <returns>The point transformed to world coordinates</returns>
    public static double ToWorldY(int y, Rectangle viewport, RectangleD clippingArea) {
      return clippingArea.Y1 - clippingArea.Height/viewport.Height*(y - viewport.Bottom);
    }

    /// <summary>
    /// Transforms a rectangle in world coordinates to screen coordinates
    /// </summary>
    /// <param name="rect">The rectangle in world coordinates that should be transformed</param>
    /// <param name="viewport">The target view port</param>
    /// <param name="clippingArea">The target clipping area</param>
    /// <returns>The rectangle rect transformed to screen coordinates</returns>
    public static Rectangle ToScreen(RectangleD rect, Rectangle viewport, RectangleD clippingArea) {
      int left = ToScreenX(rect.X1, viewport, clippingArea);
      int bottom = ToScreenY(rect.Y1, viewport, clippingArea);
      int right = ToScreenX(rect.X2, viewport, clippingArea);
      int top = ToScreenY(rect.Y2, viewport, clippingArea);
      return new Rectangle(left, top, right - left , bottom - top );
    }

    /// <summary>
    /// Transforms a 2d point in world coordinates to screen coordinates
    /// </summary>
    /// <param name="point">The point in world coordinates that should be transformed</param>
    /// <param name="viewport">The target view port</param>
    /// <param name="clippingArea">The target clipping area</param>
    /// <returns>The point transformed to screen coordinates</returns>
    public static Point ToScreen(PointD point, Rectangle viewport, RectangleD clippingArea) {
      int x = ToScreenX(point.X, viewport, clippingArea);
      int y = ToScreenY(point.Y, viewport, clippingArea);
      return new Point(x, y);
    }

    /// <summary>
    /// Transforms a point on the X-axis in world coordinates to screen coordinates
    /// </summary>
    /// <param name="x">The point on the X-axis in world coordinates that should be transformed</param>
    /// <param name="viewport">The target view port</param>
    /// <param name="clippingArea">The target clipping area</param>
    /// <returns>The point transformed to screen coordinates</returns>
    public static int ToScreenX(double x, Rectangle viewport, RectangleD clippingArea) {
      return (int)(viewport.Left + viewport.Width/clippingArea.Width*(x - clippingArea.X1));
    }

    /// <summary>
    /// Transforms a point on the Y-axis in world coordinates to screen coordinates
    /// </summary>
    /// <param name="y">The point on the Y-axis in world coordinates that should be transformed</param>
    /// <param name="viewport">The target view port</param>
    /// <param name="clippingArea">The target clipping area</param>
    /// <returns>The point transformed to screen coordinates</returns>
    public static int ToScreenY(double y, Rectangle viewport, RectangleD clippingArea) {
      return (int)(viewport.Bottom - viewport.Height/clippingArea.Height*(y - clippingArea.Y1));
    }
  }
}