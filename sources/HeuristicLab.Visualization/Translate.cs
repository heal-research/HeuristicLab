using System.Drawing;

namespace HeuristicLab.Visualization {
  public static class Translate {
    public static RectangleD ClippingArea(Point startPoint, Point endPoint, RectangleD clippingArea, Rectangle viewPort) {
      PointD worldStartPoint = Transform.ToWorld(startPoint, viewPort, clippingArea);
      PointD worldEndPoint = Transform.ToWorld(endPoint, viewPort, clippingArea);

      double xDiff = worldEndPoint.X - worldStartPoint.X;
      double yDiff = worldEndPoint.Y - worldStartPoint.Y;

      return RectangleD(xDiff, yDiff, clippingArea);
    }

    public static RectangleD RectangleD(double x, double y, RectangleD rectangle) {
      RectangleD rect = new RectangleD();

      rect.X1 = rectangle.X1 - x;
      rect.X2 = rectangle.X2 - x;
      rect.Y1 = rectangle.Y1 - y;
      rect.Y2 = rectangle.Y2 - y;

      return rect;
    }
  }
}