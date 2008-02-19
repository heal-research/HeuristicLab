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
  public abstract class RectangularPrimitiveBase : PrimitiveBase {
    protected IGroup selectionRectangles;

    private PointD myLowerLeft;
    public virtual PointD LowerLeft {
      get { return myLowerLeft; }
    }
    private PointD myUpperRight;
    public virtual PointD UpperRight {
      get { return myUpperRight; }
    }
    public virtual SizeD Size {
      get { return new SizeD(UpperRight.X - LowerLeft.X, UpperRight.Y - LowerLeft.Y); }
    }

    protected RectangularPrimitiveBase(IChart chart, PointD lowerLeft, PointD upperRight)
      : base(chart) {
      selectionRectangles = new Group(chart);
      SetPosition(lowerLeft, upperRight);
    }
    protected RectangularPrimitiveBase(IChart chart, double x1, double y1, double x2, double y2)
      : this(chart, new PointD(x1, y1), new PointD(x2, y2)) {
    }
    protected RectangularPrimitiveBase(IChart chart, PointD lowerLeft, PointD upperRight, Pen pen, Brush brush)
      : base(chart, pen, brush) {
      selectionRectangles = new Group(chart);
      SetPosition(lowerLeft, upperRight);
    }
    protected RectangularPrimitiveBase(IChart chart, double x1, double y1, double x2, double y2, Pen pen, Brush brush)
      : this(chart, new PointD(x1, y1), new PointD(x2, y2), pen, brush) {
    }

    public virtual void SetPosition(PointD lowerLeft, PointD upperRight) {
      if ((lowerLeft.X > upperRight.X) || (lowerLeft.Y > upperRight.Y))
        throw new ArgumentException("Lower left point is greater than upper right point");

      myLowerLeft = lowerLeft;
      myUpperRight = upperRight;
      OnUpdate();
    }
    public void SetPosition(double x1, double y1, double x2, double y2) {
      SetPosition(new PointD(x1, y1), new PointD(x2, y2));
    }
    public override void Move(Offset delta) {
      SetPosition(LowerLeft + delta, UpperRight + delta);
    }

    public override bool ContainsPoint(PointD point) {
      if (Selected) {
        if (selectionRectangles.ContainsPoint(point)) return true;
      }
      return false;
    }

    public override void MouseDrag(PointD point, Offset offset, MouseButtons button) {
      if (button == MouseButtons.Left) {
        if (Selected) {
          if (selectionRectangles.ContainsPoint(point)) {
            SelectionRectangle rect = (SelectionRectangle)selectionRectangles.GetPrimitive(point);
            PointD point1 = PointD.Empty;
            PointD point2 = PointD.Empty;
            if (rect.Point == LowerLeft) {
              point1 = LowerLeft + offset;
              point2 = UpperRight;
            } else if (rect.Point == UpperRight) {
              point1 = LowerLeft;
              point2 = UpperRight + offset;
            } else if ((rect.Point.X == LowerLeft.X) && (rect.Point.Y == UpperRight.Y)) {
              point1 = new PointD(LowerLeft.X + offset.DX, LowerLeft.Y);
              point2 = new PointD(UpperRight.X, UpperRight.Y + offset.DY);
            } else if ((rect.Point.X == UpperRight.X) && (rect.Point.Y == LowerLeft.Y)) {
              point1 = new PointD(LowerLeft.X, LowerLeft.Y + offset.DY);
              point2 = new PointD(UpperRight.X + offset.DX, UpperRight.Y);
            }
            SetPosition(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y),
                        Math.Max(point1.X, point2.X), Math.Max(point1.Y, point2.Y));
          } else {
            base.MouseDrag(point, offset, button);
          }
        }
      }
    }

    public override Cursor GetCursor(PointD point) {
      if (Selected) {
        Cursor cursor = selectionRectangles.GetCursor(point);
        if (cursor != null) return cursor;
      }
      return base.GetCursor(point);
    }

    public override void PostDraw(Graphics graphics) {
      selectionRectangles.Clear();
      if (Selected) {
        Pen pen = new Pen(Color.LightGray, 3);
        pen.DashStyle = DashStyle.Dash;
        Point p = Chart.TransformWorldToPixel(new PointD(LowerLeft.X, LowerLeft.Y + Size.Height));
        Size s = Chart.TransformWorldToPixel(Size);
        graphics.DrawRectangle(pen, p.X, p.Y, s.Width, s.Height);
        selectionRectangles.Add(new SelectionRectangle(Chart, UpperRight.X, LowerLeft.Y, Cursors.SizeNWSE));
        selectionRectangles.Add(new SelectionRectangle(Chart, LowerLeft.X, LowerLeft.Y, Cursors.SizeNESW));
        selectionRectangles.Add(new SelectionRectangle(Chart, UpperRight.X, UpperRight.Y, Cursors.SizeNESW));
        selectionRectangles.Add(new SelectionRectangle(Chart, LowerLeft.X, UpperRight.Y, Cursors.SizeNWSE));

        selectionRectangles.Draw(graphics);
      }
    }
  }
}
