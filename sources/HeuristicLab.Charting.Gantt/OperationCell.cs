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
using HeuristicLab.Charting.Grid;
using System.Drawing;
using HeuristicLab.Scheduling.JSSP;

namespace HeuristicLab.Charting.Gantt {
  public class OperationCell : SimpleCell {
    private Brush textBrush;

    public OperationCell(Gridchart chart) : base(chart) { 
      Value = "";
      textBrush = Brushes.Black;
    }

    public override void RenderCell(PointD lowerLeft, PointD upperRight) {
      Rectangle rect;
      Text t; 
      PointD ur = upperRight;
      PointD ll = lowerLeft;
      double height = (upperRight.Y - lowerLeft.Y) * Rowspan;
      ur.X += (upperRight.X - lowerLeft.X) * (Colspan - 1);
      ll.Y = ur.Y - height * 0.8;
      ur.Y -= height * 0.2; 
      if(this.Primitives.Count == 0) {
        rect = new Rectangle(Chart, ll, ur, Pen, Brush);
        t = new Text(Chart, ll, ur);
        t.Brush = textBrush; 
        t.String = Value.ToString();
      } else {
        rect = (Rectangle)this.Primitives[1];
        t = (Text) this.Primitives[0];
        rect.SetPosition(ll, ur);
        t.SetPosition(ll, ur);
        t.Brush = textBrush; 
        t.String = Value.ToString();
      }
      rect.Group = this;
      t.Group = this; 
      this.Clear();
      this.lowerLeft = lowerLeft;
      this.upperRight = upperRight;
      if((bool) this.Display) {
        this.Add(rect);
        this.Add(t);
        t.IntoForeground(); 
      }
    }
  }
}
