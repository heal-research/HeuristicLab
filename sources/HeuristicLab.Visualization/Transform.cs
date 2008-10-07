using System.Drawing;

namespace HeuristicLab.Visualization {
  public static class Transform {
    // Screen to world transformations
    public static RectangleD ToWorld(Rectangle rect, Rectangle viewport, RectangleD clippingArea) {
      double x1 = ToWorldX(rect.Left, viewport, clippingArea);
      double y1 = ToWorldY(rect.Bottom, viewport, clippingArea);
      double x2 = ToWorldX(rect.Right, viewport, clippingArea);
      double y2 = ToWorldY(rect.Top, viewport, clippingArea);
      return new RectangleD(x1, y1, x2, y2);
    }

    public static PointD ToWorld(Point point, Rectangle viewport, RectangleD clippingArea) {
      double x = ToWorldX(point.X, viewport, clippingArea);
      double y = ToWorldY(point.Y, viewport, clippingArea);
      return new PointD(x, y);
    }

    public static double ToWorldX(int x, Rectangle viewport, RectangleD clippingArea) {
      return clippingArea.X1 + clippingArea.Width/viewport.Width*(x - viewport.Left);
    }

    public static double ToWorldY(int y, Rectangle viewport, RectangleD clippingArea) {
      return clippingArea.Y1 - clippingArea.Height/viewport.Height*(y - viewport.Bottom);
    }

    // World to screen transformations
    public static Rectangle ToScreen(RectangleD rect, Rectangle viewport, RectangleD clippingArea) {
      int left = ToScreenX(rect.X1, viewport, clippingArea);
      int bottom = ToScreenY(rect.Y1, viewport, clippingArea);
      int right = ToScreenX(rect.X2, viewport, clippingArea);
      int top = ToScreenY(rect.Y2, viewport, clippingArea);
      return new Rectangle(left, top, right - left + 1, bottom - top + 1);
    }

    public static Point ToScreen(PointD point, Rectangle viewport, RectangleD clippingArea) {
      int x = ToScreenX(point.X, viewport, clippingArea);
      int y = ToScreenY(point.Y, viewport, clippingArea);
      return new Point(x, y);
    }

    public static int ToScreenX(double x, Rectangle viewport, RectangleD clippingArea) {
      return (int)(viewport.Left + viewport.Width/clippingArea.Width*(x - clippingArea.X1));
    }

    public static int ToScreenY(double y, Rectangle viewport, RectangleD clippingArea) {
      return (int)(viewport.Bottom - viewport.Height/clippingArea.Height*(y - clippingArea.Y1));
    }
  }
}