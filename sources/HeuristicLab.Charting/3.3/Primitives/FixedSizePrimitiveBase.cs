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
  public abstract class FixedSizePrimitiveBase : PrimitiveBase {
    private PointD myPoint;
    public virtual PointD Point {
      get { return myPoint; }
    }
    private Size mySize;
    public virtual Size Size {
      get { return mySize; }
    }

    protected FixedSizePrimitiveBase(IChart chart, PointD point, Size size)
      : base(chart) {
      myPoint = point;
      mySize = size;
    }
    protected FixedSizePrimitiveBase(IChart chart, double x, double y, Size size)
      : this(chart, new PointD(x, y), size) {
    }
    protected FixedSizePrimitiveBase(IChart chart, PointD point, Size size, Pen pen, Brush brush)
      : base(chart, pen, brush) {
      myPoint = point;
      mySize = size;
    }
    protected FixedSizePrimitiveBase(IChart chart, double x, double y, Size size, Pen pen, Brush brush)
      : this(chart, new PointD(x, y), size, pen, brush) {
    }

    public virtual void SetPosition(PointD point) {
      myPoint = point;
      OnUpdate();
    }
    public void SetPosition(double x, double y) {
      SetPosition(new PointD(x, y));
    }
    public override void Move(Offset delta) {
      SetPosition(Point + delta);
    }

    public override bool ContainsPoint(PointD point) {
      SizeD size = Chart.TransformPixelToWorld(Size);
      return (point.X >= Point.X - (size.Width / 2)) && (point.X <= Point.X + (size.Width / 2)) &&
             (point.Y >= Point.Y - (size.Height / 2)) && (point.Y <= Point.Y + (size.Height / 2));
    }
  }
}
