#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Access.Views {
  public partial class UserInformationDialog : Form {
    public UserInformationDialog() {
      InitializeComponent();
    }

    protected override void OnLoad(System.EventArgs e) {
      base.OnLoad(e);
      if (!DesignMode) {
        refreshableLightweightUserInformationView.Content = AccessClient.Instance;
        AccessClient.Instance.Refreshing += new System.EventHandler(Instance_Refreshing);
        AccessClient.Instance.Refreshed += new System.EventHandler(Instance_Refreshed);
      }
    }

    void Instance_Refreshed(object sender, EventArgs e) {
      if (this.InvokeRequired)
        this.Invoke(new Action<object, EventArgs>(Instance_Refreshed), sender, e);
      else
        closeButton.Enabled = true;
    }

    void Instance_Refreshing(object sender, EventArgs e) {
      if (this.InvokeRequired)
        this.Invoke(new Action<object, EventArgs>(Instance_Refreshing), sender, e);
      else
        closeButton.Enabled = false;
    }

    private void closeButton_Click(object sender, System.EventArgs e) {
      Close();
    }

    private void UserInformationDialog_FormClosing(object sender, FormClosingEventArgs e) {
      AccessClient.Instance.Refreshing -= new System.EventHandler(Instance_Refreshing);
      AccessClient.Instance.Refreshed -= new System.EventHandler(Instance_Refreshed);
    }
  }
}
