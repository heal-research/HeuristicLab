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
using HeuristicLab.Core;

namespace HeuristicLab.DistributedEngine {
  public partial class DistributedEngineEditor : EngineBaseEditor {
    public DistributedEngine DistributedEngine {
      get { return (DistributedEngine)Engine; }
      set { base.Engine = value; }
    }

    public DistributedEngineEditor() {
      InitializeComponent();
    }
    public DistributedEngineEditor(DistributedEngine distributedEngine)
      : this() {
      DistributedEngine = distributedEngine;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (DistributedEngine == null) {
        addressTextBox.Text = "";
        addressTextBox.Enabled = false;
      } else {
        addressTextBox.Text = DistributedEngine.ServerAddress;
        addressTextBox.Enabled = true;
      }
    }

    private void addressTextBox_TextChanged(object sender, EventArgs e) {
      DistributedEngine.ServerAddress = addressTextBox.Text;
    }
  }
}
