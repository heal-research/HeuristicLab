#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.Clients.Access.Views {
  public partial class ClientRegistrationDialog : Form {
    public ClientRegistrationDialog() {
      InitializeComponent();

      try {
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(ClientRegistrationDialog), "Documents.ClientRegistrationInfo.rtf"))
          richTextBox1.LoadFile(stream, RichTextBoxStreamType.RichText);
      }
      catch (Exception) { }
    }

    private void btnCollectInformation_Click(object sender, EventArgs e) {
      progressBar.Visible = true;
      progressBar.Style = ProgressBarStyle.Marquee;
      btnCollectInformation.Enabled = false;
      btnRegisterClient.Enabled = false;

      Task<Client> task = Task.Factory.StartNew<Client>(ClientInformationUtils.CollectClientInformation);
      task.ContinueWith(c => DisplayContent(c.Result));
    }

    private void DisplayContent(Client client) {
      if (!this.Disposing) {
        if (this.InvokeRequired) {
          Invoke(new Action<Client>(DisplayContent), client);
        } else {
          clientView.Content = client;
          progressBar.Visible = false;
          btnCollectInformation.Enabled = true;
          btnRegisterClient.Enabled = true;
        }
      }
    }

    private void AddClient() {
      AccessClient.CallAccessService(x => x.AddClient(clientView.Content));
    }

    private void RefreshClientInformation(Task task) {
      ClientInformation.Instance.Refresh();
    }

    private void btnRegisterClient_Click(object sender, EventArgs e) {
      progressBar.Visible = true;
      btnRegisterClient.Enabled = false;
      btnCollectInformation.Enabled = false;
      btnCancel.Enabled = false;

      Task task = Task.Factory.StartNew(AddClient);
      task.ContinueWith(RefreshClientInformation, TaskContinuationOptions.NotOnFaulted);
      task.ContinueWith(FinishRegistration, TaskContinuationOptions.NotOnFaulted);
      task.ContinueWith(HandleRegistrationError, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void FinishRegistration(Task task) {
      if (!this.Disposing) {
        if (this.InvokeRequired) {
          Invoke(new Action<Task>(FinishRegistration), task);
        } else {
          progressBar.Visible = false;
          btnCollectInformation.Enabled = true;
          btnCancel.Enabled = true;
          MessageBox.Show("Your HeuristicLab client has been registered successfully.", "HeuristicLab Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
      }
    }

    private void HandleRegistrationError(Task task) {
      if (!this.Disposing) {
        if (this.InvokeRequired) {
          Invoke(new Action<Task>(HandleRegistrationError), task);
        } else {
          progressBar.Visible = false;
          btnRegisterClient.Enabled = true;
          btnCollectInformation.Enabled = true;
          btnCancel.Enabled = true;
          PluginInfrastructure.ErrorHandling.ShowErrorDialog(task.Exception);
        }
      }
    }

    private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e) {
      System.Diagnostics.Process.Start(e.LinkText);
    }
  }
}
