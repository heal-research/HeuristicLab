#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HeuristicLab.Charting {
  public class Line : LinearPrimitiveBase {
    public Line(IChart chart, PointD start, PointD end)
      : base(chart, start, end) {
    }
    public Line(IChart chart, double x1, double y1, double x2, double y2)
      : this(chart, new PointD(x1, y1), new PointD(x2, y2)) {
    }
    public Line(IChart chart, PointD start, PointD end, Pen pen)
      : base(chart, start, end, pen, null) {
    }
    public Line(IChart chart, double x1, double y1, double x2, double y2, Pen pen)
      : this(chart, new PointD(x1, y1), new PointD(x2, y2), pen) {
    }

    public override bool ContainsPoint(PointD point) {
      if (base.ContainsPoint(point)) return true;

      double penWidthX = Chart.PixelToWorldRatio.Width * (Pen.Width + 1);
      double penWidthY = Chart.PixelToWorldRatio.Height * (Pen.Width + 1);

      if ((point.X < (Math.Min(Start.X, End.X) - (penWidthX / 2))) ||
          (point.Y < (Math.Min(Start.Y, End.Y) - (penWidthY / 2))) ||
          (point.X > (Math.Max(Start.X, End.X) + (penWidthX / 2))) ||
          (point.Y > (Math.Max(Start.Y, End.Y) + (penWidthY / 2)))) {
        return false;
      }

      // calculate distance between point P(X,Y) and line
      // d(P,g) = |AP.n|/|n|
      Offset start_end = End - Start;
      Offset n = new Offset(start_end.DY, -1 * start_end.DX);
      Offset start_point = point - Start;
      double d = Math.Abs(start_point.DX * n.DX + start_point.DY * n.DY) / n.Length;

      return d <= Math.Max(penWidthX, penWidthY) / 2;
    }

    public override void Draw(Graphics graphics) {
      graphics.DrawLine(Pen, Chart.TransformWorldToPixel(Start), Chart.TransformWorldToPixel(End));
      base.Draw(graphics);
    }
  }
}
