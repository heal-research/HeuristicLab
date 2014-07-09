#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public partial class RefreshableLightweightUserInformationView : RefreshableView {
    public RefreshableLightweightUserInformationView() {
      InitializeComponent();
    }

    private void RefreshUserData() {
      UserInformation.Instance.Refresh();
      lightweightUserInformationView.UserInformationChanged += new EventHandler(lightweightUserInformationView_Changed);
    }

    void lightweightUserInformationView_Changed(object sender, EventArgs e) {
      if (!storeButton.Enabled) storeButton.Enabled = true;
    }

    protected override void RefreshData() {
      Content.ExecuteActionAsync(RefreshUserData, PluginInfrastructure.ErrorHandling.ShowErrorDialog);
    }

    protected override void Content_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshing), sender, e);
      } else {
        base.Content_Refreshing(sender, e);
        lightweightUserInformationView.Enabled = false;
        storeButton.Enabled = false;
      }
    }

    protected override void Content_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshed), sender, e);
      } else {
        base.Content_Refreshed(sender, e);
        lightweightUserInformationView.Enabled = true;
        if (!UserInformation.Instance.UserExists) {
          MessageBox.Show("Couldn't fetch user information from the server." + Environment.NewLine +
            "Please verify that you have an existing user and that your user name and password is correct. ", "HeuristicLab Access Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
          lightweightUserInformationView.Content = null;
        } else {
          lightweightUserInformationView.Content = UserInformation.Instance.User;
        }
      }
    }

    private void storeButton_Click(object sender, EventArgs e) {
      AccessClient.Instance.ExecuteActionAsync(new Action(delegate {
        AccessClient.CallAccessService(x => x.UpdateLightweightUser(UserInformation.Instance.User));
      }), HeuristicLab.PluginInfrastructure.ErrorHandling.ShowErrorDialog);
    }
  }
}
