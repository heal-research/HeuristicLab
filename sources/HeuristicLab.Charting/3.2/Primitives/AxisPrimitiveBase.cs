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
  public abstract class AxisPrimitiveBase : PrimitiveBase {
    private AxisType myAxisType;
    public AxisType AxisType {
      get { return myAxisType; }
    }
    private PointD myPoint;
    public virtual PointD Point {
      get { return myPoint; }
    }
    private bool myShowGrid;
    public bool ShowGrid {
      get { return myShowGrid; }
      set {
        if (value != myShowGrid) {
          myShowGrid = value;
          OnUpdate();
        }
      }
    }
    private string myHorizontalLabel;
    public string HorizontalLabel {
      get { return myHorizontalLabel; }
      set {
        if (value != myHorizontalLabel) {
          myHorizontalLabel = value;
          OnUpdate();
        }
      }
    }
    private string myVerticalLabel;
    public string VerticalLabel {
      get { return myVerticalLabel; }
      set {
        if (value != myVerticalLabel) {
          myVerticalLabel = value;
          OnUpdate();
        }
      }
    }

    protected AxisPrimitiveBase(IChart chart, PointD point, AxisType axisType)
      : base(chart) {
      Brush = Brushes.Black;
      myPoint = point;
      myAxisType = axisType;
      myShowGrid = true;
    }
    protected AxisPrimitiveBase(IChart chart, double x, double y, AxisType axisType)
      : this(chart, new PointD(x, y), axisType) {
    }
    protected AxisPrimitiveBase(IChart chart, PointD point, AxisType axisType, Pen pen, Brush brush)
      : base(chart, pen, brush) {
      myPoint = point;
      myAxisType = axisType;
      myShowGrid = true;
    }
    protected AxisPrimitiveBase(IChart chart, double x, double y, AxisType axisType, Pen pen, Brush brush)
      : this(chart, new PointD(x, y), axisType, pen, brush) {
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
      SizeD size = Chart.TransformPixelToWorld(new Size(5, 5));
      bool result = false;
      if ((AxisType & AxisType.Horizontal) == AxisType.Horizontal)
        result = result || (point.Y >= Point.Y - (size.Height / 2)) && (point.Y <= Point.Y + (size.Height / 2));
      if ((AxisType & AxisType.Vertical) == AxisType.Vertical)
        result = result || (point.X >= Point.X - (size.Height / 2)) && (point.X <= Point.X + (size.Height / 2));
      return result;
    }
  }
}
