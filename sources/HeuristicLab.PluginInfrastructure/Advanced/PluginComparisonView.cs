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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Manager;
using System.ServiceModel;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class PluginComparisonView : Form {

    public PluginComparisonView(IPluginDescription localPlugin, IPluginDescription serverPlugin) {
      InitializeComponent();
      //Caption = "Compare plugins";

      var localPluginView = new PluginView(localPlugin);
      localPluginView.Dock = DockStyle.Fill;
      tableLayoutPanel.Controls.Add(localPluginView, 0, 0);
      if (serverPlugin != null) {
        var serverPluginView = new PluginView(serverPlugin);
        serverPluginView.Dock = DockStyle.Fill;
        tableLayoutPanel.Controls.Add(serverPluginView, 1, 0);
      } else {
        Label emptyLabel = new Label();
        emptyLabel.Text = "No matching server plugin";
        emptyLabel.TextAlign = ContentAlignment.MiddleCenter;
        emptyLabel.Dock = DockStyle.Fill;
        tableLayoutPanel.Controls.Add(emptyLabel, 1, 0);
      }
    }
  }
}
