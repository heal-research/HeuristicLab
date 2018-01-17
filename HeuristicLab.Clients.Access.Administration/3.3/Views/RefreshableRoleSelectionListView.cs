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
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Access.Administration {
  public partial class RefreshableRoleSelectionListView : RefreshableView {
    public RefreshableRoleSelectionListView() {
      InitializeComponent();
    }

    public List<Role> CurrentRoles = new List<Role>();

    private User currentUser;
    public User CurrentUser {
      get { return currentUser; }
      set {
        currentUser = value;
        roleSelectionListView.Content = null;
        storeButton.Enabled = false;
      }
    }

    protected override void RefreshData() {
      if (CurrentUser != null) {
        Content.ExecuteActionAsync(new Action(delegate {
          if (Content.Roles == null) {
            Content.RefreshRoles();
          }
          CurrentRoles = AccessAdministrationClient.CallAccessService<List<Role>>(s => s.GetUserRoles(CurrentUser));
        }), PluginInfrastructure.ErrorHandling.ShowErrorDialog);
      }
    }

    protected override void Content_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshing), sender, e);
      } else {
        base.Content_Refreshing(sender, e);
        storeButton.Enabled = false;
        roleSelectionListView.Enabled = false;
      }
    }

    protected override void Content_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshed), sender, e);
      } else {
        base.Content_Refreshed(sender, e);
        if (Content.Roles != null) {
          roleSelectionListView.Enabled = true;
          storeButton.Enabled = true;

          roleSelectionListView.Content = new ItemList<Role>(Content.Roles);
          foreach (var role in CurrentRoles) {
            foreach (ListViewItem lstRole in roleSelectionListView.ItemsListView.Items) {
              if (((Role)lstRole.Tag).Equals(role)) {
                lstRole.Checked = true;
              }
            }
          }
        }
      }
    }

    private void storeButton_Click(object sender, EventArgs e) {
      List<Role> addedRoles = new List<Role>();
      List<Role> deletedRoles = new List<Role>();

      foreach (ListViewItem lstRole in roleSelectionListView.ItemsListView.Items) {
        if (!CurrentRoles.Contains(((Role)lstRole.Tag)) && lstRole.Checked) {
          addedRoles.Add((Role)lstRole.Tag);
          CurrentRoles.Add((Role)lstRole.Tag);
        }

        if (CurrentRoles.Contains(((Role)lstRole.Tag)) && !lstRole.Checked) {
          deletedRoles.Add((Role)lstRole.Tag);
          CurrentRoles.Remove((Role)lstRole.Tag);
        }
      }

      Content.ExecuteActionAsync(new Action(delegate {
        foreach (var role in addedRoles) {
          AccessAdministrationClient.CallAccessService(s => s.AddUserToRole(role, CurrentUser));
        }

        foreach (var role in deletedRoles) {
          AccessAdministrationClient.CallAccessService(s => s.RemoveUserFromRole(role, CurrentUser));
        }
      }), PluginInfrastructure.ErrorHandling.ShowErrorDialog);
    }
  }
}
