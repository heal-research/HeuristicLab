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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Access.Views {
  [View("RefreshableLightweightAccessClient View")]
  [Content(typeof(AccessClient), false)]
  public partial class RefreshableLightweightAccessClientView : RefreshableView {
    public RefreshableLightweightAccessClientView() {
      InitializeComponent();
    }

    //set an action like a webservice call to retrieve the users which should be marked as checked           
    private Func<List<Guid>> fetchSelectedUsers;
    public Func<List<Guid>> FetchSelectedUsers {
      get { return fetchSelectedUsers; }
      set {
        fetchSelectedUsers = value;
        selectedUsers = null;
        lightweightUserView.Content = null;
        SetEnabledStateOfControls();
      }
    }

    private List<Guid> selectedUsers;

    protected override void OnContentChanged() {
      base.OnContentChanged();

      selectedUsers = null;
      lightweightUserView.Content = null;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      refreshButton.Enabled = FetchSelectedUsers != null && Content != null;
      lightweightUserView.Locked = Locked;
    }

    protected override void RefreshData() {
      Action completeRefreshAction = new Action(delegate {
        selectedUsers = FetchSelectedUsers();
        Content.Refresh();
      });

      Content.ExecuteActionAsync(completeRefreshAction, new Action<Exception>((Exception ex) => ErrorHandling.ShowErrorDialog(this, "Refresh failed.", ex)));
    }

    protected override void Content_Refreshing(object sender, EventArgs e) {
      base.Content_Refreshing(sender, e);
      lightweightUserView.Content = null;
    }

    protected override void Content_Refreshed(object sender, EventArgs e) {
      base.Content_Refreshed(sender, e);

      if (Content.UsersAndGroups != null) {
        lightweightUserView.Content = new ItemList<UserGroupBase>(Content.UsersAndGroups.Where(x => selectedUsers.Contains(x.Id)));
        if (lightweightUserView.Content != null) OnStorableStateChanged();
      }
    }

    public IItemList<UserGroupBase> GetDeletedUsers() {
      if (lightweightUserView.Content == null) {
        return null;
      } else {
        var deletedIds = selectedUsers.Where(x => lightweightUserView.Content.Count(y => y.Id == x) == 0);
        List<UserGroupBase> deletedUGs = Content.UsersAndGroups.Where(x => deletedIds.Contains(x.Id)).ToList();
        return new ItemList<UserGroupBase>(deletedUGs);
      }
    }

    public IItemList<UserGroupBase> GetCheckedUsers() {
      return (lightweightUserView.Content == null) ? null : lightweightUserView.Content;
    }

    public IItemList<UserGroupBase> GetAddedUsers() {
      return (lightweightUserView.Content == null) ? null : new ItemList<UserGroupBase>(lightweightUserView.Content.Where(x => !selectedUsers.Contains(x.Id)));
    }

    public event EventHandler SelectedUsersChanged;
    protected virtual void OnSelectedUsersChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnSelectedUsersChanged);
      else {
        EventHandler handler = SelectedUsersChanged;
        if (handler != null)
          handler(this, EventArgs.Empty);
      }
    }

    public event EventHandler StorableStateChanged;
    protected virtual void OnStorableStateChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnStorableStateChanged);
      else {
        EventHandler handler = StorableStateChanged;
        if (handler != null)
          handler(this, EventArgs.Empty);
      }
    }

    private void lightweightUserView_SelectedUsersChanged(object sender, EventArgs e) {
      OnSelectedUsersChanged();
    }
  }
}
