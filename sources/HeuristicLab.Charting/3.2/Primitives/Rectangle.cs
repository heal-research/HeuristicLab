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
  public class Rectangle : RectangularPrimitiveBase {
    public Rectangle(IChart chart, PointD lowerLeft, PointD upperRight)
      : base(chart, lowerLeft, upperRight) {
    }
    public Rectangle(IChart chart, double x1, double y1, double x2, double y2)
      : this(chart, new PointD(x1, y1), new PointD(x2, y2)) {
    }
    public Rectangle(IChart chart, PointD lowerLeft, PointD upperRight, Pen pen, Brush brush)
      : base(chart, lowerLeft, upperRight, pen, brush) {
    }
    public Rectangle(IChart chart, double x1, double y1, double x2, double y2, Pen pen, Brush brush)
      : this(chart, new PointD(x1, y1), new PointD(x2, y2), pen, brush) {
    }

    public override bool ContainsPoint(PointD point) {
      if (base.ContainsPoint(point)) return true;
      return (point.X >= LowerLeft.X) && (point.X <= UpperRight.X) &&
             (point.Y >= LowerLeft.Y) && (point.Y <= UpperRight.Y);
    }

    public override void Draw(Graphics graphics) {
      Point p = Chart.TransformWorldToPixel(new PointD(LowerLeft.X, LowerLeft.Y + Size.Height));
      Size s = Chart.TransformWorldToPixel(Size);
      if (Brush != null)
        graphics.FillRectangle(Brush, p.X, p.Y, s.Width, s.Height);
      graphics.DrawRectangle(Pen, p.X, p.Y, s.Width, s.Height);
      base.Draw(graphics);
    }
  }
}
