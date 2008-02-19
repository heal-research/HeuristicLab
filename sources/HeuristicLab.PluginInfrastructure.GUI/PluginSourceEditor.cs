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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace HeuristicLab.PluginInfrastructure.GUI {
  public partial class PluginSourceEditor : Form {
    private StringCollection settingsSources = HeuristicLab.PluginInfrastructure.GUI.Properties.Settings.Default.PluginSources;
    public PluginSourceEditor() {     
      InitializeComponent();

      string[] sources = new string[settingsSources.Count];

      settingsSources.CopyTo(sources, 0);
      sourcesTextBox.Lines = sources;
    }

    private void saveButton_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.OK;
      settingsSources.Clear();
      foreach(string line in sourcesTextBox.Lines) {
        settingsSources.Add(line);
      }

      HeuristicLab.PluginInfrastructure.GUI.Properties.Settings.Default.Save();
      Close();
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.Cancel;
      Close();
    }
  }
}
