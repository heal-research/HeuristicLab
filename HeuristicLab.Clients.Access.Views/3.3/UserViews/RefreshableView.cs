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
  [View("Refreshable View")]
  [Content(typeof(AccessClient), true)]
  public partial class RefreshableView : AsynchronousContentView {
    public new AccessClient Content {
      get { return (AccessClient)base.Content; }
      set { base.Content = value; }
    }
    public RefreshableView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.Refreshing -= new EventHandler(Content_Refreshing);
      Content.Refreshed -= new EventHandler(Content_Refreshed);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Refreshing += new EventHandler(Content_Refreshing);
      Content.Refreshed += new EventHandler(Content_Refreshed);
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      refreshButton.Enabled = Content != null;
    }

    protected virtual void Content_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshing), sender, e);
      } else {
        Cursor = Cursors.AppStarting;
        refreshButton.Enabled = false;
      }
    }

    protected virtual void Content_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshed), sender, e);
      } else {
        Cursor = Cursors.Default;
        refreshButton.Enabled = true;
      }
    }

    protected virtual void RefreshData() { }

    public void ManualRefresh() {
      RefreshData();
    }

    protected void refreshButton_Click(object sender, System.EventArgs e) {
      RefreshData();
    }
  }
}
