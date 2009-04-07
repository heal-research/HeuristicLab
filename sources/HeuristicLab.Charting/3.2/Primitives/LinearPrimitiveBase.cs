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
  public abstract class LinearPrimitiveBase : PrimitiveBase {
    protected IGroup selectionRectangles;

    private PointD myStart;
    public virtual PointD Start {
      get { return myStart; }
    }
    private PointD myEnd;
    public virtual PointD End {
      get { return myEnd; }
    }
    public virtual SizeD Size {
      get { return new SizeD(End.X - Start.X, End.Y - Start.Y); }
    }
    public virtual double Length {
      get { return (Start - End).Length; }
    }

    protected LinearPrimitiveBase(IChart chart, PointD start, PointD end)
      : base(chart) {
      selectionRectangles = new Group(chart);
      myStart = start;
      myEnd = end;
    }
    protected LinearPrimitiveBase(IChart chart, double x1, double y1, double x2, double y2)
      : this(chart, new PointD(x1, y1), new PointD(x2, y2)) {
    }
    protected LinearPrimitiveBase(IChart chart, PointD start, PointD end, Pen pen, Brush brush)
      : base(chart, pen, brush) {
      selectionRectangles = new Group(chart);
      myStart = start;
      myEnd = end;
    }
    protected LinearPrimitiveBase(IChart chart, double x1, double y1, double x2, double y2, Pen pen, Brush brush)
      : this(chart, new PointD(x1, y1), new PointD(x2, y2), pen, brush) {
    }

    public virtual void SetPosition(PointD start, PointD end) {
      myStart = start;
      myEnd = end;
      OnUpdate();
    }
    public void SetPosition(double x1, double y1, double x2, double y2) {
      SetPosition(new PointD(x1, y1), new PointD(x2, y2));
    }
    public override void Move(Offset delta) {
      SetPosition(Start + delta, End + delta);
    }

    public override bool ContainsPoint(PointD point) {
      if (Selected) {
        if (selectionRectangles.GetPrimitive(point) != null) return true;
      }
      return false;
    }

    public override void MouseDrag(PointD point, Offset offset, MouseButtons button) {
      if (button == MouseButtons.Left) {
        if (Selected) {
          if (selectionRectangles.ContainsPoint(point)) {
            SelectionRectangle rect = (SelectionRectangle)selectionRectangles.GetPrimitive(point);
            if (rect.Point == Start) {
              SetPosition(Start + offset, End);
            } else if (rect.Point == End) {
              SetPosition(Start, End + offset);
            }
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
        graphics.DrawLine(pen,
                          Chart.TransformWorldToPixel(Start),
                          Chart.TransformWorldToPixel(End));
        selectionRectangles.Add(new SelectionRectangle(Chart, Start, Cursors.SizeAll));
        selectionRectangles.Add(new SelectionRectangle(Chart, End, Cursors.SizeAll));

        selectionRectangles.Draw(graphics);
      }
    }
  }
}
