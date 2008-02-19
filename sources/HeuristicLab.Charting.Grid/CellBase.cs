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
using HeuristicLab.Charting;

namespace HeuristicLab.Charting.Grid {
  public class CellBase : Group, ICell {
    #region ICell Members
    int rowspan;
    public int Rowspan {
      get { return rowspan; }
    }

    int colspan;
    public int Colspan {
      get { return colspan; }
    }

    object val;
    public object Value {
      get { return val; }
      set { val = value; }
    }

    bool display;
    public object Display {
      get { return display; }
      set { 
        display = (bool) value;
        RenderCell(lowerLeft, upperRight);
      }
    }

    public PointD lowerLeft;
    public PointD upperRight; 

    public override Pen Pen {
      get { return base.Pen; }
      set {
        base.Pen = value;
        foreach(IPrimitive p in Primitives) {
          p.Pen = base.Pen; 
        }
        OnUpdate();
      }
    }

    public override Brush Brush {
      get { return base.Brush; }
      set {
        base.Brush = value;
        foreach(IPrimitive p in Primitives) {
          p.Brush = base.Brush;
        }
        OnUpdate();
      }
    }
    #endregion 

    private bool dragDropEnabled;
    public bool DragDropEnabled {
      get { return dragDropEnabled; }
      set { dragDropEnabled = (bool)value; }
    }

    public override void Move(Offset delta) {
      if (dragDropEnabled) {
        base.Move(delta); 
      }
    }

    public CellBase(Gridchart chart) : base(chart) {
      Chart.Group.Add(this);
      display = true; 
      rowspan = 1;
      colspan = 1;
      dragDropEnabled = true; 
    }

    public void SetSpan(int rowspan, int colspan) {
      this.rowspan = Math.Max(rowspan, 1);
      this.colspan = Math.Max(colspan, 1);
      RenderCell(lowerLeft, upperRight); 
    }

    public virtual void RenderCell(PointD lowerLeft, PointD upperRight) {
      Rectangle rect;
      PointD ur = upperRight;
      PointD ll = lowerLeft; 
      ur.X += (upperRight.X - lowerLeft.X) * (colspan - 1);
      ll.Y -= (upperRight.Y - lowerLeft.Y) * (rowspan - 1); 
      if (this.Primitives.Count == 0) {
        rect = new Rectangle(Chart, ll, ur, Pen, Brush);
      } else {
        rect = (Rectangle) this.Primitives[0];
        rect.SetPosition(ll, ur);
      }
      rect.Group = this;
      this.Clear();
      this.lowerLeft = lowerLeft;
      this.upperRight = upperRight;
      if(display) {
        this.Add(rect);
      }
    }
  }
}
