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
using System.Linq;
using System.ServiceModel.Security;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("Hive Job Manager")]
  [Content(typeof(HiveClient), true)]
  public partial class HiveJobManagerView : AsynchronousContentView {

    public new HiveClient Content {
      get { return (HiveClient)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public HiveJobManagerView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Refreshing += new EventHandler(Content_Refreshing);
      Content.Refreshed += new EventHandler(Content_Refreshed);
    }

    protected override void DeregisterContentEvents() {
      Content.Refreshing -= new EventHandler(Content_Refreshing);
      Content.Refreshed -= new EventHandler(Content_Refreshed);
      base.DeregisterContentEvents();
    }

    protected async override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        hiveExperimentListView.Content = null;
      } else {
        hiveExperimentListView.Content = Content.Jobs;
        try {
          await System.Threading.Tasks.Task.Run(() => Content.Refresh());
        } catch (Exception ex) {
          HandleServiceException(ex);
        }
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      refreshButton.Enabled = Content != null;
      hiveExperimentListView.Enabled = Content != null;
    }

    private void Content_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshing), sender, e);
      } else {
        Cursor = Cursors.AppStarting;
        refreshButton.Enabled = false;
        hiveExperimentListView.Enabled = false;
      }
    }
    private void Content_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshed), sender, e);
      } else {
        hiveExperimentListView.Content = Content.Jobs;
        refreshButton.Enabled = true;
        hiveExperimentListView.Enabled = true;
        Cursor = Cursors.Default;
      }
    }

    private async void refreshButton_Click(object sender, EventArgs e) {
      try {
        await System.Threading.Tasks.Task.Run(() => Content.Refresh());
      } catch (Exception ex) {
        HandleServiceException(ex);
      }
    }

    private void HandleServiceException(Exception ex) {
      if (this.InvokeRequired) {
        Invoke(new Action<Exception>(HandleServiceException), ex);
      } else {
        if (ex is MessageSecurityException) {
          string message =
            "A Message Security error has occured. This normally means that your user name or password is wrong. Detailed information: " + Environment.NewLine;
          message += HiveServiceLocator.Instance.GetEndpointInformation();
          ErrorHandling.ShowErrorDialog(this, message, ex);
        } else if (ex is AnonymousUserException) {
          using (HiveInformationDialog dialog = new HiveInformationDialog()) {
            dialog.ShowDialog(this);
          }
        } else {
          ErrorHandling.ShowErrorDialog(this,
            "Refresh failed. Detailed information: " + Environment.NewLine + HiveServiceLocator.Instance.GetEndpointInformation(), ex);
        }
      }
    }

    protected override void OnClosing(FormClosingEventArgs e) {
      if (Content != null && Content.Jobs != null && Content.Jobs.Any(x => x.IsProgressing)) {
        DialogResult result = MessageBox.Show("There are still unfinished down/uploads. Are you sure you want to close the window?", "HeuristicLab Hive Job Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.No) {
          e.Cancel = true;
        }
      } else {
        base.OnClosing(e);
      }
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      if (Content != null && Content.Jobs != null) {
        Content.ClearHiveClient();
        Content = null;
      }
      base.OnClosed(e);
    }
  }
}
