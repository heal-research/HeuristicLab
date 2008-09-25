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
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public partial class LocalProcessDriverConfigurationView : ViewBase {
    public LocalProcessDriverConfiguration LocalProcessDriverConfiguration {
      get { return (LocalProcessDriverConfiguration)base.Item; }
      set { base.Item = value; }
    }

    public LocalProcessDriverConfigurationView() {
      InitializeComponent();
    }

    public LocalProcessDriverConfigurationView(LocalProcessDriverConfiguration localProcessDriverConfiguration)
      : this() {
      LocalProcessDriverConfiguration = localProcessDriverConfiguration;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (LocalProcessDriverConfiguration == null) {
        executablePathStringDataView.Enabled = false;
        executablePathStringDataView.StringData = null;
        argumentsStringDataView.Enabled = false;
        argumentsStringDataView.StringData = null;
      } else {
        executablePathStringDataView.StringData = LocalProcessDriverConfiguration.ExecutablePath;
        executablePathStringDataView.Enabled = true;
        argumentsStringDataView.StringData = LocalProcessDriverConfiguration.Arguments;
        argumentsStringDataView.Enabled = true;
      }
    }

    private void browseExecutableButtom_Click(object sender, EventArgs e) {
      if (LocalProcessDriverConfiguration != null && executablePathOpenFileDialog.ShowDialog() == DialogResult.OK) {
        LocalProcessDriverConfiguration.ExecutablePath = new StringData(executablePathOpenFileDialog.FileName);
      }
    }
  }
}
