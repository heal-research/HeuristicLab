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
using System.Collections.Generic;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Access.Administration {
  [View("UserGroup View")]
  [Content(typeof(UserGroup), true)]
  public partial class UserGroupView : ItemView {
    public new UserGroup Content {
      get { return (UserGroup)base.Content; }
      set { base.Content = value; }
    }

    public UserGroupView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      AccessClient.Instance.Refreshing += new EventHandler(Content_Refreshing);
      refreshableLightweightUserView.StorableStateChanged += new EventHandler(refreshableLightweightUserView_StorableStateChanged);
    }

    protected override void DeregisterContentEvents() {
      AccessClient.Instance.Refreshing -= new EventHandler(Content_Refreshing);
      refreshableLightweightUserView.StorableStateChanged -= new EventHandler(refreshableLightweightUserView_StorableStateChanged);
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content == null) {
        groupNameTextBox.Clear();
        idTextBox.Clear();

        refreshableLightweightUserView.Content = null;
        refreshableLightweightUserView.FetchSelectedUsers = null;
      } else {
        groupNameTextBox.Text = Content.Name;
        idTextBox.Text = Content.Id.ToString();

        refreshableLightweightUserView.Content = Content.Id != Guid.Empty ? AccessClient.Instance : null;
        refreshableLightweightUserView.FetchSelectedUsers = Content.Id != Guid.Empty ? new Func<List<Guid>>(delegate { return AccessAdministrationClient.CallAccessService(s => s.GetUserGroupIdsOfGroup(Content.Id)); }) : null;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool enabled = Content != null && !Locked;
      groupNameTextBox.ReadOnly = !enabled;
      idTextBox.ReadOnly = !enabled;
      storeButton.Enabled = enabled;
      //refreshableLightweightUserView.Enabled = enabled;
      refreshableLightweightUserView.Locked = !enabled;
      //refreshableLightweightUserView.ReadOnly = !enabled;
    }

    private void groupNameTextBox_TextChanged(object sender, EventArgs e) {
      if (Content != null && Content.Name != groupNameTextBox.Text)
        Content.Name = groupNameTextBox.Text;
    }

    private void storeButton_Click(object sender, EventArgs e) {
      Action storeAction = new Action(delegate {
        foreach (var ug in refreshableLightweightUserView.GetAddedUsers()) {
          AccessAdministrationClient.CallAccessService(s => s.AddUserGroupBaseToGroup(ug, Content));
        }
      });

      Action deleteAction = new Action(delegate {
        foreach (var ug in refreshableLightweightUserView.GetDeletedUsers()) {
          AccessAdministrationClient.CallAccessService(s => s.RemoveUserGroupBaseFromGroup(ug, Content));
        }
      });

      AccessAdministrationClient.Instance.ExecuteActionAsync(storeAction, PluginInfrastructure.ErrorHandling.ShowErrorDialog);
      AccessAdministrationClient.Instance.ExecuteActionAsync(deleteAction, PluginInfrastructure.ErrorHandling.ShowErrorDialog);
    }

    private void Content_Refreshing(object sender, EventArgs e) {
      if (!Locked) storeButton.Enabled = false;
    }

    private void refreshableLightweightUserView_StorableStateChanged(object sender, EventArgs e) {
      if (!Locked) storeButton.Enabled = true;
    }
  }
}
