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
using System.Windows.Forms;
using System.Drawing;

namespace HeuristicLab.Charting.Grid {
  public class SimpleCell : CellBase {
    
    public SimpleCell(Gridchart chart)
      : base(chart) {
      DragDropEnabled = true; 
    }

    public override void Move(Offset delta) {
      if (DragDropEnabled) {
        base.Move(delta);
        SetPosition(lowerLeft + delta, upperRight + delta);
      }
    }

    public int[] GetSubCell(PointD point) {
      Offset diff = lowerLeft - point;
      diff.DX /= Math.Abs(lowerLeft.X - upperRight.X);
      diff.DY = (upperRight.Y - point.Y) / Math.Abs(lowerLeft.Y - upperRight.Y); 
      int[] result = new int[]{(int) Math.Floor(Math.Abs(diff.DX)), (int) Math.Floor(Math.Abs(diff.DY))}; 
      return result; 
    }

    public virtual void SetPosition(PointD lowerLeft, PointD upperRight) {
      if((lowerLeft.X > upperRight.X) || (lowerLeft.Y > upperRight.Y))
        throw new ArgumentException("Lower left point is greater than upper right point");
      RenderCell(lowerLeft, upperRight); 
      OnUpdate();
    }

    public override void MouseDrag(PointD point, Offset offset, MouseButtons button) {
      if(button == MouseButtons.Left) {
        Move(offset);
      }
    }

    public void Snap(PointD ll, PointD ur) {
      this.SetPosition(ll, ur);
    }
  }
}
