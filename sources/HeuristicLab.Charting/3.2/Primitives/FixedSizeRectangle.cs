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
  public class FixedSizeRectangle : FixedSizePrimitiveBase {
    public FixedSizeRectangle(IChart chart, PointD point, Size size)
      : base(chart, point, size) {
    }
    public FixedSizeRectangle(IChart chart, double x, double y, Size size)
      : this(chart, new PointD(x, y), size) {
    }
    public FixedSizeRectangle(IChart chart, PointD point, Size size, Pen pen, Brush brush)
      : base(chart, point, size, pen, brush) {
    }
    public FixedSizeRectangle(IChart chart, double x, double y, Size size, Pen pen, Brush brush)
      : this(chart, new PointD(x, y), size, pen, brush) {
    }

    public override void Draw(Graphics graphics) {
      Point p = Chart.TransformWorldToPixel(Point);

      graphics.FillRectangle(Brush,
                             p.X - (Size.Width / 2),
                             p.Y - (Size.Height / 2),
                             Size.Width,
                             Size.Height);
      graphics.DrawRectangle(Pen,
                             p.X - (Size.Width / 2),
                             p.Y - (Size.Height / 2),
                             Size.Width,
                             Size.Height);
    }
  }
}
