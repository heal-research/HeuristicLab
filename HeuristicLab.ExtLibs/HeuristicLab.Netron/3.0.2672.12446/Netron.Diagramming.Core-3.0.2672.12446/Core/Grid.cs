using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Core {
  public class Grid {
    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    // ------------------------------------------------------------------
    public Grid() {
    }

    public static void Paint(
        Graphics g,
        RectangleF area,
        float horizontalSpacing,
        float verticalSpacing,
        float penWidth) {
      //ControlPaint.DrawGrid(
      //    g,
      //    area,
      //    new Size(20, 20),
      //    Color.Wheat);

      //g.SmoothingMode = SmoothingMode.HighQuality;
      //g.CompositingQuality = CompositingQuality.HighQuality;
      //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
      g.SetClip(area);
      float sideLength = Math.Min(area.Width, area.Height);
      float horizSpacing = sideLength / horizontalSpacing;
      float vertSpacing = sideLength / verticalSpacing;

      Pen majorPen = new Pen(Color.DarkGray);
      majorPen.Width = penWidth;
      majorPen.DashStyle = DashStyle.Dot;

      float top = area.Top;  // The top y coord.
      float bottom = area.Bottom;  // The bottom y coord.
      float left = area.Left;  // The left y coord.
      float right = area.Right;  // The right y coord.
      PointF p1;  // The starting point for the grid line.
      PointF p2;  // The end point for the grid line.

      // Draw the horizontal lines, starting at the top.
      for (float i = top; i <= bottom; i += horizontalSpacing) {
        p1 = new PointF(left, i);
        p2 = new PointF(right, i);
        g.DrawLine(majorPen, p1, p2);
      }

      // Draw the major vertical lines, starting at the left edge.
      for (float i = left; i <= right; i += verticalSpacing) {
        p1 = new PointF(i, top);
        p2 = new PointF(i, bottom);
        g.DrawLine(majorPen, p1, p2);
      }
      g.ResetClip();
    }
  }
}
