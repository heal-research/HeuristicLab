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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.Clients.Access.Views {
  public partial class ClientInformationDialog : Form {
    public ClientInformationDialog() {
      InitializeComponent();
    }

    private void okButton_Click(object sender, System.EventArgs e) {
      Close();
    }

    private void refreshButton_Click(object sender, System.EventArgs e) {
      RefreshInformation();
    }

    private void registerClientButton_Click(object sender, EventArgs e) {
      using (ClientRegistrationDialog regDialog = new ClientRegistrationDialog()) {
        regDialog.ShowDialog(this);
        RefreshInformation();
      }
    }

    private void RefreshInformation() {
      clientView.StartProgressView();
      Task.Factory.StartNew(new Action(delegate {
        ClientInformation.Instance.Refresh();
        clientView.Content = ClientInformation.Instance.ClientInfo;
        clientView.FinishProgressView();
        EnableRegistration();
      }));
    }

    private void EnableRegistration() {
      if (InvokeRequired) {
        Invoke(new Action(EnableRegistration));
      } else {
        registerClientButton.Visible = !ClientInformation.Instance.ClientExists;
        infoLabel.Visible = !ClientInformation.Instance.ClientExists;
      }
    }
  }
}
