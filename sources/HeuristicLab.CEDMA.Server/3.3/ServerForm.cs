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
  public partial class ServerForm : Form {
    private Server server;
    private Store store;
    private IDispatcher dispatcher;
    private IExecuter executer;

    private static readonly string rdfFile = AppDomain.CurrentDomain.BaseDirectory + "rdf_store.db3";
    private static readonly string rdfConnectionString = "sqlite:rdf:Data Source=\"" + rdfFile + "\"";

    public ServerForm() {
      InitializeComponent();
      store = new Store(rdfConnectionString);
      server = new Server(store);
      server.Start();
      addressTextBox.Text = server.CedmaServiceUrl;
    }

    private void refreshTimer_Tick(object sender, EventArgs e) {
      listBox.DataSource = executer.GetJobs();
    }

    private void connectButton_Click(object sender, EventArgs e) {
      dispatcher = new SimpleDispatcher(store);
      if (address.Text.Contains("ExecutionEngine")) {
        executer = new HiveExecuter(dispatcher, store, address.Text);
      } else { 
        // default is grid backend
        executer = new GridExecuter(dispatcher, store, address.Text);
      }
      executer.Start();
      maxActiveJobsUpDown.Enabled = true;
      maxActiveJobsUpDown.Value = executer.MaxActiveJobs;
      connectButton.Enabled = false;
      refreshTimer.Start();
    }

    private void maxActiveJobsUpDown_ValueChanged(object sender, EventArgs e) {
      executer.MaxActiveJobs = Convert.ToInt32(maxActiveJobsUpDown.Value);
    }
  }
}
