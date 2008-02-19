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
using HeuristicLab.Charting;

namespace HeuristicLab.Charting.Grid {
  public partial class TestControl : Form {
    public TestControl() {
      InitializeComponent();
      Gridchart c = new Gridchart(0, 0, 3, 3); 
      chartControl1.Chart = c;
      c.Redim(5, 5);
      CellBase cell = new SimpleCell(c);
      cell.Brush = Brushes.Crimson;
      cell.SetSpan(2, 2); 
      c[3, 3] = cell;
    }
  }
}
