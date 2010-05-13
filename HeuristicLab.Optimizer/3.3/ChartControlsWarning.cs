#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Optimizer {
  public partial class ChartControlsWarning : Form {
    public ChartControlsWarning() {
      InitializeComponent();
    }

    private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      try {
        System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/details.aspx?familyid=130F7986-BF49-4FE5-9CA8-910AE6EA442C&displaylang=en");
        linkLabel.LinkVisited = true;
      }
      catch (Exception) { }
    }
  }
}
