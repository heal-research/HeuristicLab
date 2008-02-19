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
  public class FixedSizeCircle : FixedSizePrimitiveBase {
    public FixedSizeCircle(IChart chart, PointD point, int diameter)
      : base(chart, point, new Size(diameter, diameter)) {
    }
    public FixedSizeCircle(IChart chart, double x, double y, int diameter)
      : this(chart, new PointD(x, y), diameter) {
    }
    public FixedSizeCircle(IChart chart, PointD point, int diameter, Pen pen, Brush brush)
      : base(chart, point, new Size(diameter, diameter), pen, brush) {
    }
    public FixedSizeCircle(IChart chart, double x, double y, int diameter, Pen pen, Brush brush)
      : this(chart, new PointD(x, y), diameter, pen, brush) {
    }

    public override void Draw(Graphics graphics) {
      Point p = Chart.TransformWorldToPixel(Point);

      graphics.FillEllipse(Brush,
                           p.X - (Size.Width / 2),
                           p.Y - (Size.Height / 2),
                           Size.Width,
                           Size.Height);
      graphics.DrawEllipse(Pen,
                           p.X - (Size.Width / 2),
                           p.Y - (Size.Height / 2),
                           Size.Width,
                           Size.Height);
    }
  }
}
