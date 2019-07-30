#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Access.Views {
  [View("RefreshableLightweightUserInformation View")]
  [Content(typeof(LightweightUser), false)]
  public partial class RefreshableLightweightUserInformationView : AsynchronousContentView {
    public new LightweightUser Content {
      get { return (LightweightUser)base.Content; }
      set { base.Content = value; }
    }

    public RefreshableLightweightUserInformationView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      lightweightUserInformationView.Content = Content;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      this.Locked = true;
      refreshButton.Enabled = Content != null;
    }

    protected override void DeregisterContentEvents() {
      Refreshing -= new EventHandler(Content_Refreshing);
      Refreshed -= new EventHandler(Content_Refreshed);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Refreshing += new EventHandler(Content_Refreshing);
      Refreshed += new EventHandler(Content_Refreshed);
    }

    protected void Content_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshing), sender, e);
      } else {
        Cursor = Cursors.AppStarting;
        refreshButton.Enabled = false;

        lightweightUserInformationView.Enabled = false;
      }
    }

    protected void Content_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshed), sender, e);
      } else {
        Cursor = Cursors.Default;
        refreshButton.Enabled = true;

        lightweightUserInformationView.Enabled = true;
        if (!UserInformation.Instance.UserExists) {
          MessageBox.Show("Couldn't fetch user information from the server." + Environment.NewLine +
            "Please verify that you have an existing user and that your user name and password is correct. ", "HeuristicLab Access Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
          lightweightUserInformationView.Content = null;
        } else {
          lightweightUserInformationView.Content = Content;
        }
      }
    }

    void lightweightUserInformationView_Changed(object sender, EventArgs e) {
      // nothing to do
    }

    private void RefreshUserData() {
      UserInformation.Instance.Refresh();
      lightweightUserInformationView.UserInformationChanged += new EventHandler(lightweightUserInformationView_Changed);
    }

    private void RefreshData() {
      ExecuteActionAsync(RefreshUserData, PluginInfrastructure.ErrorHandling.ShowErrorDialog);
    }

    public void ManualRefresh() {
      RefreshData();
    }

    protected void refreshButton_Click(object sender, System.EventArgs e) {
      RefreshData();
    }

    public void ExecuteActionAsync(Action action, Action<Exception> exceptionCallback) {
      var call = new Func<Exception>(delegate () {
        try {
          OnRefreshing();
          action();
        } catch (Exception ex) {
          return ex;
        } finally {
          OnRefreshed();
        }
        return null;
      });
      call.BeginInvoke(delegate (IAsyncResult result) {
        Exception ex = call.EndInvoke(result);
        if (ex != null) exceptionCallback(ex);
      }, null);
    }

    #region Events
    public event EventHandler Refreshing;
    private void OnRefreshing() {
      EventHandler handler = Refreshing;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Refreshed;
    private void OnRefreshed() {
      EventHandler handler = Refreshed;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
