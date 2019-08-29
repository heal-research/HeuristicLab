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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Clients.Access;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("ProjectView")]
  [Content(typeof(Project), IsDefaultView = false)]
  public partial class ProjectPermissionsView : ItemView {
    private const int userImageIndex = 0;
    private const int userGroupImageIndex = 1;

    private readonly HashSet<UserGroupBase> assignedPermissions = new HashSet<UserGroupBase>();
    private readonly HashSet<UserGroupBase> inheritedPermissions = new HashSet<UserGroupBase>();
    private readonly HashSet<UserGroupBase> newAssignedPermissions = new HashSet<UserGroupBase>();
    private readonly HashSet<UserGroupBase> newInheritedPermissions = new HashSet<UserGroupBase>();
    private readonly Dictionary<Guid, HashSet<UserGroupBase>> userGroupAncestors = new Dictionary<Guid, HashSet<UserGroupBase>>();
    private readonly Dictionary<Guid, HashSet<UserGroupBase>> userGroupDescendants = new Dictionary<Guid, HashSet<UserGroupBase>>();

    private IEnumerable<UserGroupBase> addedPermissions;
    private IEnumerable<UserGroupBase> removedPermissions;
    private IEnumerable<UserGroupBase> addedIncludes;
    private IEnumerable<UserGroupBase> removedIncludes;

    private readonly Color addedAssignmentColor = Color.FromArgb(255, 87, 191, 193); // #57bfc1
    private readonly Color removedAssignmentColor = Color.FromArgb(255, 236, 159, 72); // #ec9f48
    private readonly Color addedIncludeColor = Color.FromArgb(25, 169, 221, 221); // #a9dddd
    private readonly Color removedIncludeColor = Color.FromArgb(25, 249, 210, 145); // #f9d291
    private readonly Color projectOwnerColor = Color.DarkRed;

    public new Project Content {
      get { return (Project)base.Content; }
      set { base.Content = value; }
    }

    public ProjectPermissionsView() {
      InitializeComponent();

      treeView.ImageList.Images.Add(VSImageLibrary.User);
      treeView.ImageList.Images.Add(VSImageLibrary.UserAccounts);
    }

    #region Overrides
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        assignedPermissions.Clear();
        inheritedPermissions.Clear();
        newAssignedPermissions.Clear();
        newInheritedPermissions.Clear();
        treeView.Nodes.Clear();
        detailsViewHost.Content = null;
      } else {
        UpdatePermissionList();        
      }
      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool enabled = Content != null && !Locked && !ReadOnly;

      inheritButton.Enabled = enabled;
      saveButton.Enabled = enabled;
      treeView.Enabled = enabled;

      if (detailsViewHost != null) {
        detailsViewHost.Locked = true;
      }

    }
    #endregion

    #region Event Handlers
    private void ProjectPermissionsView_Load(object sender, EventArgs e) {

    }

    private void refreshButton_Click(object sender, EventArgs e) {
      UpdatePermissionList();
    }

    private async void inheritButton_Click(object sender, EventArgs e) {
      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => SetGrantedProjectPermissions(Content.Id, newAssignedPermissions.Select(x => x.Id), false, true, false));
      UpdatePermissionList();
    }

    private async void saveButton_Click(object sender, EventArgs e) {
      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => SetGrantedProjectPermissions(Content.Id, newAssignedPermissions.Select(x => x.Id), false, false, false));
      UpdatePermissionList();
    }

    private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
      var selectedPermission = (UserGroupBase)e.Node.Tag;
      detailsViewHost.Content = selectedPermission;
      if (selectedPermission is LightweightUser)
        detailsViewHost.ViewType = typeof(Access.Views.RefreshableLightweightUserInformationView);
    }

    private void treeView_BeforeCheck(object sender, TreeViewCancelEventArgs e) {
      var checkedPermission = (UserGroupBase)e.Node.Tag;
      if (e.Node.Parent == null 
        || newInheritedPermissions.Contains(checkedPermission) 
        || checkedPermission.Id == Guid.Empty
        || Content.OwnerUserId == checkedPermission.Id)
          e.Cancel = true;
    }

    private void treeView_AfterCheck(object sender, TreeViewEventArgs e) {
      var checkedPermission = (UserGroupBase)e.Node.Tag;
      if (e.Node.Checked)
        newAssignedPermissions.Add(checkedPermission);
      else
        newAssignedPermissions.Remove(checkedPermission);

      UpdateNewPermissionList();
    }
    #endregion

    #region Helpers
    private void UpdatePermissionList() {
      AccessClient.Instance.Refresh();

      UpdateUserGroupGenealogy();
      UpdateAssignedPermissions();
      UpdateInheritedPermissions();
      var top = BuildPermissionsList(AccessClient.Instance.UsersAndGroups);
      detailsViewHost.Content = top;
    }

    private void UpdateNewPermissionList() {
      UpdateNewAssignedPermissions();
      UpdateNewInheritedPermissions();
      var top = BuildPermissionsList(AccessClient.Instance.UsersAndGroups);
      detailsViewHost.Content = top;
    }

    private void UpdateAssignedPermissions() {
      assignedPermissions.Clear();
      newAssignedPermissions.Clear();
      var grantedPermissions = GetGrantedPermissionsForProject(Content.Id);
      foreach (var r in grantedPermissions) {
        assignedPermissions.Add(r);
        newAssignedPermissions.Add(r);
      }
    }

    private void UpdateNewAssignedPermissions() {
      for(int i = newAssignedPermissions.Count-1; i >= 0; i--) {
        if(newAssignedPermissions.Intersect(userGroupAncestors[newAssignedPermissions.ElementAt(i).Id]).Any() 
          && newAssignedPermissions.ElementAt(i).Id != Content.OwnerUserId) {
          newAssignedPermissions.Remove(newAssignedPermissions.ElementAt(i));
        }
      }
    }

    private void UpdateInheritedPermissions() {
      inheritedPermissions.Clear();
      newInheritedPermissions.Clear();
      foreach(var item in assignedPermissions) {
        if(userGroupDescendants.ContainsKey(item.Id)) {
          foreach(var descendant in userGroupDescendants[item.Id]) {
            if(!assignedPermissions.Contains(descendant)) {
              inheritedPermissions.Add(descendant);
              newInheritedPermissions.Add(descendant);
            }
          }
        }
      }
    }

    private void UpdateNewInheritedPermissions() {
      newInheritedPermissions.Clear();
      foreach(var item in newAssignedPermissions) {
        if(userGroupDescendants.ContainsKey(item.Id)) {
          foreach(var descendant in userGroupDescendants[item.Id]) {
            if(!newAssignedPermissions.Contains(descendant))
              newInheritedPermissions.Add(descendant);
          }
        }
      }
    }

    private void UpdateUserGroupGenealogy() {
      userGroupAncestors.Clear();
      userGroupDescendants.Clear();

      var usersAndGroups = AccessClient.Instance.UsersAndGroups;
      foreach (var ug in usersAndGroups) {
        userGroupAncestors.Add(ug.Id, new HashSet<UserGroupBase>());
        userGroupDescendants.Add(ug.Id, new HashSet<UserGroupBase>());
      }

      var userGroupTree = HiveServiceLocator.Instance.CallHiveService(s => s.GetUserGroupTree());
      foreach(var branch in userGroupTree) {
        var parent = usersAndGroups.Where(x => x.Id == branch.Key).SingleOrDefault();
        if(parent != null) {
          var userGroupsToAdd = usersAndGroups.Where(x => userGroupTree[parent.Id].Contains(x.Id));
          foreach (var node in userGroupsToAdd) {
            userGroupDescendants[parent.Id].Add(node);
            userGroupAncestors[node.Id].Add(parent);
          }
        }
      }
    }

    private static IEnumerable<UserGroupBase> GetGrantedPermissionsForProject(Guid projectId) {
      if (projectId == Guid.Empty) return Enumerable.Empty<UserGroupBase>();
      var projectPermissions = HiveServiceLocator.Instance.CallHiveService(s => s.GetProjectPermissions(projectId));
      var userIds = new HashSet<Guid>(projectPermissions.Select(x => x.GrantedUserId));
      return AccessClient.Instance.UsersAndGroups.Where(x => userIds.Contains(x.Id));
    }

    private static void SetGrantedProjectPermissions(Guid projectId, IEnumerable<Guid> userIds, bool reassign, bool cascading, bool reassignCascading) {
      if (projectId == null || userIds == null) return;
      HiveServiceLocator.Instance.CallHiveService(s => {
        s.SaveProjectPermissions(projectId, userIds.ToList(), reassign, cascading, reassignCascading);
      });
    }

    private UserGroupBase BuildPermissionsList(IEnumerable<UserGroupBase> usersAndGroups) {
      addedPermissions = newAssignedPermissions.Except(assignedPermissions);
      removedPermissions = assignedPermissions.Except(newAssignedPermissions);
      addedIncludes = newInheritedPermissions.Except(inheritedPermissions);
      removedIncludes = inheritedPermissions.Except(newInheritedPermissions);

      treeView.Nodes.Clear();
      if (!usersAndGroups.Any()) return null;

      treeView.BeforeCheck -= treeView_BeforeCheck;
      treeView.AfterCheck -= treeView_AfterCheck;

      var userGroups = new HashSet<UserGroup>(usersAndGroups.OfType<UserGroup>());
      var users = new HashSet<LightweightUser>(usersAndGroups.OfType<LightweightUser>());
      UserGroupBase first = null;

      var groupsNode = new TreeNode("Groups") { ForeColor = SystemColors.GrayText };
      groupsNode.ImageIndex = groupsNode.SelectedImageIndex = userGroupImageIndex;

      foreach (var group in userGroups.OrderBy(x => x.Name)) {
        var node = new TreeNode(group.Name) { Tag = group };
        node.ImageIndex = userGroupImageIndex;
        node.SelectedImageIndex = node.ImageIndex;
        BuildNode(group, node);
        groupsNode.Nodes.Add(node);
        if (first == null) first = group;
      }

      var usersNode = new TreeNode("Users") { ForeColor = SystemColors.GrayText };
      usersNode.ImageIndex = usersNode.SelectedImageIndex = userImageIndex;

      foreach (var user in users.OrderBy(x => x.ToString())) {
        var node = new TreeNode(user.ToString()) { Tag = user };
        node.ImageIndex = userImageIndex;
        node.SelectedImageIndex = node.ImageIndex;
        BuildNode(user, node);
        usersNode.Nodes.Add(node);
        if (first == null) first = user;
      }

      treeView.Nodes.Add(groupsNode);
      treeView.Nodes.Add(usersNode);
      treeView.BeforeCheck += treeView_BeforeCheck;
      treeView.AfterCheck += treeView_AfterCheck;
      treeView.ExpandAll();

      return first;
    }

    private void BuildNode(UserGroupBase ug, TreeNode node) {

      if (newAssignedPermissions.Contains(ug)) {
        node.Checked = true;
      } else if (newInheritedPermissions.Contains(ug)) {
        node.Checked = true;
        node.ForeColor = SystemColors.GrayText;
      }

      if (inheritedPermissions.Contains(ug) && newInheritedPermissions.Contains(ug)) {
        node.Text += " [included]";
      } else if (addedIncludes.Contains(ug)) {
        node.BackColor = addedIncludeColor;
        node.ForeColor = SystemColors.GrayText;
        node.Text += " [added include]";
      } else if (removedIncludes.Contains(ug)) {
        node.BackColor = removedIncludeColor;
        node.ForeColor = SystemColors.GrayText;
        node.Text += " [removed include]";
      }

      if (addedPermissions.Contains(ug)) {
        node.BackColor = addedAssignmentColor;
        node.ForeColor = SystemColors.ControlText;
        node.Text += " [added assignment]";
      } else if (removedPermissions.Contains(ug)) {
        node.BackColor = removedAssignmentColor;
        node.ForeColor = SystemColors.ControlText;
        node.Text += " [removed assignment]";
      }

      if(Content != null && ug != null && ug.Id != Guid.Empty 
        && Content.OwnerUserId == ug.Id) {
        node.ForeColor = projectOwnerColor;
      }

    }

    #endregion
  }
}
