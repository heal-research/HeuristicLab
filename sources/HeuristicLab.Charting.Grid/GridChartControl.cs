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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Charting.Grid {
  public partial class GridChartControl : ChartControl {
    public GridChartControl() : base() {
      InitializeComponent();
      base.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
    }

    private void pictureBox_MouseUp(object sender, MouseEventArgs e) {
      SimpleCell b = null; 
      EmptyCell empty = null;
      PointD position = Chart.TransformPixelToWorld(e.Location); 
      if(e.Button == MouseButtons.Left) {
        if(Chart.Mode == ChartMode.Select) {
          IList<IPrimitive> primitives = Chart.Group.GetAllPrimitives(position);
          foreach(IPrimitive p in primitives) {
            if(p is SimpleCell) {
              b = (SimpleCell)p;
            }
            if(p is EmptyCell) {
              empty = (EmptyCell)p;
            }
          }
          if((empty != null) && (b != null)) {
            // if cell spans across more than one cell
            // --> determine position of mouse pointer
            int[] index = b.GetSubCell(position);
            Offset diff = new Offset(index[0] * (empty.upperRight.X - empty.lowerLeft.X), -index[1] * (empty.upperRight.Y - empty.lowerLeft.Y)); 
            b.Snap(empty.lowerLeft - diff, empty.upperRight - diff);
          }
        }
      }
    }

  }
}
