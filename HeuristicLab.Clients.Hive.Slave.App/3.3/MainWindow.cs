#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.SlaveCore.Views;

namespace HeuristicLab.Clients.Hive.Slave.App {
  public partial class MainWindow : Form {
    public MainWindow() {
      InitializeComponent();
      slaveMainView.VisibilitySwitched += new System.EventHandler(slaveMainView_WindowStateChanged);
    }

    void slaveMainView_WindowStateChanged(object sender, EventArgs e) {
      if (this.Visible) {
        Hide();
      } else {
        Show();
        if (WindowState == FormWindowState.Minimized) {
          WindowState = FormWindowState.Normal;
        }
      }
    }

    public SlaveItem Content {
      get { return slaveMainView.Content; }
      set {
        slaveMainView.Content = value;
      }
    }

    private void MainWindow_SizeChanged(object sender, EventArgs e) {
      if (WindowState == FormWindowState.Minimized) {
        Hide();
      }
    }
  }
}
