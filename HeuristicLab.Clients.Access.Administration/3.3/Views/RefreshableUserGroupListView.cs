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
namespace HeuristicLab.Clients.Access.Administration {
  public partial class RefreshableUserGroupListView : RefreshableView {
    public RefreshableUserGroupListView() {
      InitializeComponent();
    }

    protected override void RefreshData() {
      Content.RefreshUserGroupsAsync(PluginInfrastructure.ErrorHandling.ShowErrorDialog);
    }

    protected override void Content_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshing), sender, e);
      } else {
        base.Content_Refreshing(sender, e);
        userGroupListView.Enabled = false;
        storeButton.Enabled = false;
        if (Content.Groups != null) {
          Content.Groups.ItemsRemoved -= new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<UserGroup>>(Groups_ItemsRemoved);
        }
      }
    }

    protected override void Content_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshed), sender, e);
      } else {
        base.Content_Refreshed(sender, e);
        userGroupListView.Enabled = true;
        userGroupListView.Content = Content.Groups;
        if (Content.Groups != null) {
          storeButton.Enabled = true;
          Content.Groups.ItemsRemoved += new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<UserGroup>>(Groups_ItemsRemoved);
        }
      }
    }

    void Groups_ItemsRemoved(object sender, Collections.CollectionItemsChangedEventArgs<Collections.IndexedItem<UserGroup>> e) {
      foreach (var u in e.Items) {
        Content.DeleteUserGroupAsync(u.Value, PluginInfrastructure.ErrorHandling.ShowErrorDialog);
      }
    }

    protected override void DeregisterContentEvents() {
      if (Content.Groups != null) {
        Content.Groups.ItemsRemoved -= new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<UserGroup>>(Groups_ItemsRemoved);
      }
      base.DeregisterContentEvents();
    }

    private void storeButton_Click(object sender, EventArgs e) {
      Content.StoreUserGroupsAsync(PluginInfrastructure.ErrorHandling.ShowErrorDialog);
    }
  }
}
