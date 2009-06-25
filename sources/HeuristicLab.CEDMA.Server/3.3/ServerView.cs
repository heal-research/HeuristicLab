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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using HeuristicLab.PluginInfrastructure;
using System.Net;
using HeuristicLab.CEDMA.DB;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.Data.Common;
using System.Threading;
using HeuristicLab.Grid;
using HeuristicLab.Data;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Server {
  public partial class ServerView : ViewBase {
    private Server server;

    public ServerView(Server server) : base() {
      this.server = server;
      InitializeComponent();
      addressTextBox.Text = server.CedmaServiceUrl;
    }

    private void connectButton_Click(object sender, EventArgs e) {
      server.Connect(address.Text);
      UserControl executerControl = (UserControl)server.Executer.CreateView();
      executerControl.Dock = DockStyle.Fill;
      executerTabPage.Controls.Add(executerControl);
      UserControl dispatcherControl = (UserControl)server.Dispatcher.CreateView();
      dispatcherControl.Dock = DockStyle.Fill;
      dispatcherTabPage.Controls.Add(dispatcherControl);
      connectButton.Enabled = false;      
    }
  }
}
