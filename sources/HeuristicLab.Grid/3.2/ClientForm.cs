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
using HeuristicLab.Core;
using System.Xml;
using System.Threading;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace HeuristicLab.Grid {


  public partial class ClientForm : Form {

    private List<ClientController> clientControllers;

    public ClientForm() {
      InitializeComponent();
      clientControllers = new List<ClientController>();
      nClientsControl.Value = Environment.ProcessorCount;
      clientControllerBindingSource.DataSource = clientControllers;
      UpdateControl();
    }

    private void startButton_Click(object sender, EventArgs e) {
      foreach (var client in clientControllers) {
        client.Client.Start(addressTextBox.Text);
      }
      UpdateControl();
    }

    private void stopButton_Click(object sender, EventArgs e) {
      foreach (var client in clientControllers) {
        client.Client.Stop();
      }
      UpdateControl();
    }

    private void UpdateControl() {
      foreach (var client in clientControllers) {
        if (client.Client.Waiting) {
          startButton.Enabled = false;
          stopButton.Enabled = true;
          client.Status = "Waiting for engine";
        } else if (client.Client.Executing) {
          startButton.Enabled = false;
          stopButton.Enabled = true;
          client.Status = "Executing engine";
        } else if (client.Client.Stopped) {
          startButton.Enabled = true;
          stopButton.Enabled = false;
          client.Status = "Stopped";
        }
        client.Message = client.Client.StatusMessage;
      }
      clientGrid.Invalidate();
    }

    private void timer_Tick(object sender, EventArgs e) {
      UpdateControl();
    }

    private void nClientsControl_ValueChanged(object sender, EventArgs e) {
      if (nClientsControl.Value < 0)
        nClientsControl.Value = 0;
      while (clientControllers.Count < nClientsControl.Value)
        clientControllers.Add(new ClientController() { Client = new GridClient() });
      while (clientControllers.Count > nClientsControl.Value) {
        try {
          clientControllers[clientControllers.Count - 1].Client.Stop();
          clientControllers.RemoveAt(clientControllers.Count - 1);
        } catch { }
      }
      clientGrid.Invalidate();
    }
  }
}
