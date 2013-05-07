#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Access.Views {
  public sealed partial class LightweightUserGroupSelectionView : UserControl {
    public LightweightUserGroupSelectionView() {
      InitializeComponent();
    }

    public ItemList<UserGroupBase> GetSelectedItems() {
      ItemList<UserGroupBase> result = new ItemList<UserGroupBase>();

      foreach (var item in itemsListView.SelectedItems) {
        UserGroupBase ugb = ((ListViewItem)item).Tag as UserGroupBase;
        if (ugb != null) {
          result.Add(ugb);
        }
      }
      return result;
    }

    private void LightweightUserGroupSelectionView_Load(object sender, EventArgs e) {
      if (!this.DesignMode && AccessClient.Instance.UsersAndGroups == null) {
        AccessClient.Instance.Refreshing += new EventHandler(Instance_Refreshing);
        AccessClient.Instance.Refreshed += new EventHandler(Instance_Refreshed);
        AccessClient.Instance.RefreshAsync(new Action<Exception>((Exception ex) => ErrorHandling.ShowErrorDialog(this, "Refresh failed.", ex)));
        AccessClient.Instance.Refreshing -= new EventHandler(Instance_Refreshing);
        AccessClient.Instance.Refreshed -= new EventHandler(Instance_Refreshed);
      }

      if (AccessClient.Instance.UsersAndGroups != null) {
        AccessClient.Instance.UsersAndGroups.ForEach(x => AddListViewItem(CreateListViewItem(x)));

        foreach (ColumnHeader c in this.itemsListView.Columns) {
          c.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }

    void Instance_Refreshed(object sender, EventArgs e) {
      if (AccessClient.Instance.UsersAndGroups != null)
        this.itemsListView.Enabled = true;
    }

    void Instance_Refreshing(object sender, EventArgs e) {
      this.itemsListView.Enabled = false;
    }

    private ListViewItem CreateListViewItem(UserGroupBase item) {
      ListViewItem listViewItem = new ListViewItem();

      listViewItem.Text = item.ToString();
      itemsListView.SmallImageList.Images.Add(item.ItemImage);
      listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      listViewItem.Tag = item;
      if (item is UserGroup) {
        listViewItem.SubItems.Insert(0, new ListViewItem.ListViewSubItem(listViewItem, item.ToString()));
        listViewItem.SubItems.Insert(1, new ListViewItem.ListViewSubItem(listViewItem, string.Empty));
        listViewItem.Group = itemsListView.Groups[0];
      } else if (item is LightweightUser) {
        var lu = item as LightweightUser;
        listViewItem.SubItems.Insert(0, new ListViewItem.ListViewSubItem(listViewItem, lu.UserName));
        listViewItem.SubItems.Insert(1, new ListViewItem.ListViewSubItem(listViewItem, lu.FullName));
        listViewItem.Group = itemsListView.Groups[1];
      }

      return listViewItem;
    }

    private void AddListViewItem(ListViewItem listViewItem) {
      itemsListView.Items.Add(listViewItem);
    }
  }
}
