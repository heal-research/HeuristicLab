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

namespace HeuristicLab.Charting {
  public class Ellipse : RectangularPrimitiveBase {
    public Ellipse(IChart chart, PointD lowerLeft, PointD upperRight)
      : base(chart, lowerLeft, upperRight) {
    }
    public Ellipse(IChart chart, double x1, double y1, double x2, double y2)
      : this(chart, new PointD(x1, y1), new PointD(x2, y2)) {
    }
    public Ellipse(IChart chart, PointD lowerLeft, PointD upperRight, Pen pen, Brush brush)
      : base(chart, lowerLeft, upperRight, pen, brush) {
    }
    public Ellipse(IChart chart, double x1, double y1, double x2, double y2, Pen pen, Brush brush)
      : this(chart, new PointD(x1, y1), new PointD(x2, y2), pen, brush) {
    }

    public override bool ContainsPoint(PointD point) {
      if (base.ContainsPoint(point)) return true;

      PointD midpoint;
      PointD focus1;
      PointD focus2;
      Offset focus1_point;
      Offset focus2_point;

      double a = Size.Width / 2;   // half length of horizontal axis
      double b = Size.Height / 2;  // half length of vertical axis
      midpoint = LowerLeft + new Offset(a, b);

      if (a >= b) {
        double e = Math.Sqrt(a * a - b * b);  // linear eccentricity
        focus1 = new PointD(midpoint.X - e, midpoint.Y);
        focus2 = new PointD(midpoint.X + e, midpoint.Y);
      } else {
        double e = Math.Sqrt(b * b - a * a);  // linear eccentricity
        focus1 = new PointD(midpoint.X, midpoint.Y - e);
        focus2 = new PointD(midpoint.X, midpoint.Y + e);
      }
      focus1_point = focus1 - point;
      focus2_point = focus2 - point;

      return focus1_point.Length + focus2_point.Length <= 2 * Math.Max(a, b);
    }

    public override void Draw(Graphics graphics) {
      Point p = Chart.TransformWorldToPixel(new PointD(LowerLeft.X, LowerLeft.Y + Size.Height));
      Size s = Chart.TransformWorldToPixel(Size);
      if (Brush != null)
        graphics.FillEllipse(Brush, p.X, p.Y, s.Width, s.Height);
      graphics.DrawEllipse(Pen, p.X, p.Y, s.Width, s.Height);
      base.Draw(graphics);
    }
  }
}
