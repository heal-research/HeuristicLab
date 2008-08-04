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
    private GridClient client;
    public ClientForm() {
      Trace.Listeners.Clear();
      Trace.Listeners.Add(new EventLogTraceListener("HeuristicLab.Grid"));

      InitializeComponent();

      client = new GridClient();
      UpdateControl();
    }

    private void startButton_Click(object sender, EventArgs e) {
      client.Start(addressTextBox.Text);
      UpdateControl();
    }

    private void stopButton_Click(object sender, EventArgs e) {
      client.Stop();
      UpdateControl();
    }

    private void UpdateControl() {
      if(client.Waiting) {
        startButton.Enabled = false;
        stopButton.Enabled = true;
        statusTextBox.Text = "Waiting for engine";
      } else if(client.Executing) {
        startButton.Enabled = false;
        stopButton.Enabled = true;
        statusTextBox.Text = "Executing engine";
      } else if(client.Stopped) {
        startButton.Enabled = true;
        stopButton.Enabled = false;
        statusTextBox.Text = "Stopped";
      }
      statusStrip.Text = client.StatusMessage;
    }

    private void timer_Tick(object sender, EventArgs e) {
      UpdateControl();
    }
  }
}
