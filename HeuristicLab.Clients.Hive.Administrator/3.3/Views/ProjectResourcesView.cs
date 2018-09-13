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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Clients.Access;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using System.Collections;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("ProjectView")]
  [Content(typeof(Project), IsDefaultView = false)]
  public partial class ProjectResourcesView : ItemView {
    private const int slaveImageIndex = 0;
    private const int slaveGroupImageIndex = 1;
    public const string UNGROUPED_GROUP_NAME = "UNGROUPED";
    public const string UNGROUPED_GROUP_DESCRIPTION = "Contains slaves that are not assigned to any group.";
    public const string IMMUTABLE_TAG = " [assigned, immutable]";
    public const string INCLUDED_TAG = " [included]";
    public const string ADDED_ASSIGNMENT_TAG = " [added assignment]";
    public const string REMOVED_ASSIGNMENT_TAG = " [removed assignment]";
    public const string ADDED_INCLUDE_TAG = " [added include]";
    public const string REMOVED_INCLUDE_TAG = " [removed include]";

    private readonly HashSet<Resource> assignedResources = new HashSet<Resource>();
    private readonly HashSet<Resource> newAssignedResources = new HashSet<Resource>();
    private readonly HashSet<Resource> includedResources = new HashSet<Resource>();
    private readonly HashSet<Resource> newIncludedResources = new HashSet<Resource>();

    private readonly Color addedAssignmentColor = Color.FromArgb(255, 87, 191, 193); // #57bfc1
    private readonly Color removedAssignmentColor = Color.FromArgb(255, 236, 159, 72); // #ec9f48
    private readonly Color addedIncludeColor = Color.FromArgb(25, 169, 221, 221); // #a9dddd
    private readonly Color removedIncludeColor = Color.FromArgb(25, 249, 210, 145); // #f9d291
    private readonly Color selectedBackColor = Color.DodgerBlue;
    private readonly Color selectedForeColor = Color.White;
    private readonly Color grayTextColor = SystemColors.GrayText;

    private HashSet<Resource> projectExclusiveResources = new HashSet<Resource>();
    private TreeNode ungroupedGroupNode;

    public new Project Content {
      get { return (Project)base.Content; }
      set { base.Content = value; }
    }

    public ProjectResourcesView() {
      InitializeComponent();

      treeView.ImageList.Images.Add(VSImageLibrary.MonitorLarge);
      treeView.ImageList.Images.Add(VSImageLibrary.NetworkCenterLarge);
    }

    #region Overrides
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        assignedResources.Clear();
        newAssignedResources.Clear();
        includedResources.Clear();
        treeView.Nodes.Clear();
        detailsViewHost.Content = null;
      } else {
        UpdateAssignedResources();
        UpdateIncludedResources();
        var top = BuildResourceTree();
        detailsViewHost.Content = top;
        detailsViewHost.Locked = true;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool enabled = Content != null && !Locked && !ReadOnly;

      inheritButton.Enabled = enabled;
      saveButton.Enabled = enabled;
      treeView.Enabled = enabled;
    }
    #endregion

    #region Event Handlers
    private void ProjectResourcesView_Load(object sender, EventArgs e) {

    }

    private void refreshButton_Click(object sender, EventArgs e) {
      HiveAdminClient.Instance.Refresh();
      UpdateAssignedResources();
      UpdateIncludedResources();
      var top = BuildResourceTree();
      detailsViewHost.Content = top;
    }

    private async void inheritButton_Click(object sender, EventArgs e) {
      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => {
          SetAssignedProjectResources(Content.Id, newAssignedResources.Select(x => x.Id), false, true, false);
        });
      HiveAdminClient.Instance.Refresh();
      UpdateResourceTree();
    }

    private async void saveButton_Click(object sender, EventArgs e) {
      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => {
          SetAssignedProjectResources(Content.Id, newAssignedResources.Select(x => x.Id), false, false, false);
        });
      HiveAdminClient.Instance.Refresh();
      UpdateResourceTree();
    }

    private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e) {
      if (e.Node == null || e.Node == ungroupedGroupNode)
        e.Cancel = true;

      var selectedResource = (Resource)e.Node.Tag;
      if(HiveAdminClient.Instance.DisabledParentResources.Contains(selectedResource))
        e.Cancel = true;
    }

    private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
      var selectedResource = (Resource)e.Node.Tag;
      detailsViewHost.Content = selectedResource;
    }

    private void treeView_BeforeCheck(object sender, TreeViewCancelEventArgs e) {
      if (e.Node == null || e.Node == ungroupedGroupNode) {
        e.Cancel = true;
        return;
      }
        
      var checkedResource = (Resource)e.Node.Tag;
      if (checkedResource == null
        || checkedResource.Id == Guid.Empty
        || HiveAdminClient.Instance.DisabledParentResources.Contains(checkedResource) 
        || newIncludedResources.Contains(checkedResource)) {
        e.Cancel = true;
      } else if (!IsAdmin()) {
          if (!HiveAdminClient.Instance.CheckOwnershipOfParentProject(Content, UserInformation.Instance.User.Id) 
            || !HiveAdminClient.Instance.GetAvailableProjectAncestors(Content.Id).Any() 
            || projectExclusiveResources.Contains(checkedResource)) {
            e.Cancel = true;
          }
      }
    }

    private void treeView_AfterCheck(object sender, TreeViewEventArgs e) {
      var checkedResource = (Resource)e.Node.Tag;
      if (e.Node.Checked) {
        newAssignedResources.Add(checkedResource);
      } else {
        newAssignedResources.Remove(checkedResource);
      }

      UpdateNewResourceTree();
    }
    #endregion

    #region Helpers

    private void UpdateResourceTree() {
      UpdateAssignedResources();
      UpdateIncludedResources();

      var top = BuildResourceTree();
      detailsViewHost.Content = top;
    }

    private void UpdateNewResourceTree() {
      UpdateNewAssignedResources();
      UpdateNewIncludedResources();
      var top = BuildResourceTree();
      detailsViewHost.Content = top;
    }

    private IEnumerable<Resource> GetAvailableResourcesForProjectAdministration(Guid projectId) {
      projectExclusiveResources.Clear();
      if (projectId == Guid.Empty) return Enumerable.Empty<Resource>();
      var resources = HiveAdminClient.Instance.Resources.Where(x => x.Id != Guid.Empty);
      if (!resources.Any()) return resources;
      if (IsAdmin()) return resources;

      // get project specific assigned resources
      var projectResources = resources.Where(x => 
        HiveAdminClient.Instance.ProjectResourceAssignments
          .Where(a => a.ProjectId == projectId)
          .Select(a => a.ResourceId)
        .Contains(x.Id));

      // look up for assignments of ancestor projects
      var projectIds = new HashSet<Guid>();
      HiveAdminClient.Instance.GetAvailableProjectAncestors(projectId).ToList().ForEach(x => projectIds.Add(x.Id));

      var ancestorProjectResources = resources.Where(x => 
        HiveAdminClient.Instance.ProjectResourceAssignments
          .Where(a => projectIds.Contains(a.ProjectId))
          .Select(a => a.ResourceId)
        .Contains(x.Id));

      // look down for included resources of ancestor projects
      HashSet<Resource> availableResources = new HashSet<Resource>(ancestorProjectResources);
      foreach (var r in ancestorProjectResources) {
        foreach(var d in HiveAdminClient.Instance.GetAvailableResourceDescendants(r.Id)) {
          availableResources.Add(d);
        }
      }

      // determine resources, which are exclusively assigned to the currently selected project
      projectResources
        .Except(availableResources)
        .ToList()
        .ForEach(x => projectExclusiveResources.Add(x));

      // look down for included resources of currently selected project
      if (projectExclusiveResources.Any()) {
        foreach (var r in projectExclusiveResources.ToArray()) {
          foreach (var d in HiveAdminClient.Instance.GetAvailableResourceDescendants(r.Id)) {
            projectExclusiveResources.Add(d);
          }  
        }
      }

      return availableResources.Union(projectExclusiveResources);
    }

    private IEnumerable<Resource> GetAssignedResourcesForProject(Guid projectId) {
      if (projectId == Guid.Empty) return Enumerable.Empty<Resource>();
      return HiveAdminClient.Instance.Resources.Where(x =>
        HiveAdminClient.Instance.ProjectResourceAssignments
          .Where(a => a.ProjectId == projectId)
          .Select(a => a.ResourceId)
        .Contains(x.Id));
    }

    private void SetAssignedProjectResources(Guid projectId, IEnumerable<Guid> resourceIds, bool reassign, bool cascading, bool reassignCascading) {
      if (projectId == null || resourceIds == null) return;
      HiveServiceLocator.Instance.CallHiveService(s => {
       s.SaveProjectResourceAssignments(projectId, resourceIds.ToList(), reassign, cascading, reassignCascading);
      });
    }

    private void UpdateNewAssignedResources() {
      for(int i = newAssignedResources.Count -1; i >= 0; i--) {
        if(newAssignedResources.Intersect(HiveAdminClient.Instance.GetAvailableResourceAncestors(newAssignedResources.ElementAt(i).Id)).Any()) {
          newAssignedResources.Remove(newAssignedResources.ElementAt(i));
        }
      }
    }

    private void UpdateAssignedResources() {
      assignedResources.Clear();
      newAssignedResources.Clear();
      foreach (var r in GetAssignedResourcesForProject(Content.Id)) {
        assignedResources.Add(r);
        newAssignedResources.Add(r);
      }
    }

    private void UpdateIncludedResources() {
      includedResources.Clear();
      newIncludedResources.Clear();
      foreach (var a in assignedResources) {
        foreach (var r in HiveAdminClient.Instance.GetAvailableResourceDescendants(a.Id)) {
          includedResources.Add(r);
          newIncludedResources.Add(r);
        }
      }
    }

    private void UpdateNewIncludedResources() {
      newIncludedResources.Clear();
      foreach (var a in newAssignedResources) {
        foreach (var r in HiveAdminClient.Instance.GetAvailableResourceDescendants(a.Id)) {
          newIncludedResources.Add(r);
        }
      }
    }

    private Resource BuildResourceTree() {
      treeView.Nodes.Clear();

      treeView.BeforeCheck -= treeView_BeforeCheck;
      treeView.AfterCheck -= treeView_AfterCheck;

      var resources = GetAvailableResourcesForProjectAdministration(Content.Id);

      var disabledParentResources = HiveAdminClient.Instance.DisabledParentResources;
      var mainResources = new HashSet<Resource>(resources.OfType<SlaveGroup>().Where(x => x.ParentResourceId == null));
      //var parentedMainResources = new HashSet<Resource>(resources.OfType<SlaveGroup>()
      //  .Where(x => x.ParentResourceId.HasValue && !resources.Select(y => y.Id).Contains(x.ParentResourceId.Value)));
      //mainResources.UnionWith(parentedMainResources);
      var mainDisabledParentResources = new HashSet<Resource>(disabledParentResources.Where(x => x.ParentResourceId == null || x.ParentResourceId == Guid.Empty));
      mainResources.UnionWith(mainDisabledParentResources);
      var subResources = new HashSet<Resource>(resources.Union(disabledParentResources).Except(mainResources).OrderByDescending(x => x.Name));

      var stack = new Stack<Resource>(mainResources.OrderByDescending(x => x.Name));
      
      Resource top = null;
      //bool nodeSelected = false;
      if (detailsViewHost != null && detailsViewHost.Content != null && detailsViewHost.Content is Resource) {
        var resourceId = ((Resource)detailsViewHost.Content).Id;
        top = resources.Where(x => x.Id == resourceId).FirstOrDefault();
      }
      

      TreeNode currentNode = null;
      Resource currentResource = null;

      var addedAssignments = newAssignedResources.Except(assignedResources);
      var removedAssignments = assignedResources.Except(newAssignedResources);
      var addedIncludes = newIncludedResources.Except(includedResources);
      var removedIncludes = includedResources.Except(newIncludedResources);

      while (stack.Any()) {
        var newResource = stack.Pop();
        var newNode = new TreeNode(newResource.Name) { Tag = newResource };

        if(top == null && !disabledParentResources.Contains(newResource)) {
          top = newResource;
        }

        //if(!nodeSelected && top != null && newResource.Id == top.Id) {
        //  newNode.BackColor = selectedBackColor;
        //  newNode.ForeColor = selectedForeColor;
        //  nodeSelected = true;
        //}

        // search for parent node of newNode and save in currentNode
        // necessary since newNodes (stack top items) might be siblings
        // or grand..grandparents of previous node (currentNode)
        while (currentNode != null && newResource.ParentResourceId != currentResource.Id) {
          currentNode = currentNode.Parent;
          currentResource = currentNode == null ? null : (Resource)currentNode.Tag;
        }

        if (currentNode == null) {
          treeView.Nodes.Add(newNode);
        } else {
          currentNode.Nodes.Add(newNode);
        }

        if (disabledParentResources.Contains(newResource)) {
          newNode.Checked = false;
          newNode.ForeColor = grayTextColor;
        } else if (newAssignedResources.Contains(newResource)) {
          newNode.Checked = true;
          if(!HiveRoles.CheckAdminUserPermissions()) {
            if (!HiveAdminClient.Instance.CheckOwnershipOfParentProject(Content, UserInformation.Instance.User.Id) 
              || !HiveAdminClient.Instance.GetAvailableProjectAncestors(Content.Id).Any() 
              || projectExclusiveResources.Contains(newResource)) {
              newNode.ForeColor = SystemColors.GrayText;
              newNode.Text += IMMUTABLE_TAG;
            }
          }

        } else if (newIncludedResources.Contains(newResource)) {
          newNode.Checked = true;
          newNode.ForeColor = SystemColors.GrayText;
        }

          if (includedResources.Contains(newResource) && newIncludedResources.Contains(newResource)) {
          newNode.Text += INCLUDED_TAG;
        } else if (addedIncludes.Contains(newResource)) {
          newNode.BackColor = addedIncludeColor;
          newNode.ForeColor = SystemColors.GrayText;
          newNode.Text += ADDED_INCLUDE_TAG;
        } else if (removedIncludes.Contains(newResource)) {
          newNode.BackColor = removedIncludeColor;
          newNode.Text += REMOVED_INCLUDE_TAG;
        }

        if (addedAssignments.Contains(newResource)) {
          newNode.BackColor = addedAssignmentColor;
          newNode.ForeColor = SystemColors.ControlText;
          newNode.Text += ADDED_ASSIGNMENT_TAG;
        } else if (removedAssignments.Contains(newResource)) {
          newNode.BackColor = removedAssignmentColor;
          newNode.ForeColor = SystemColors.ControlText;
          newNode.Text += REMOVED_ASSIGNMENT_TAG;
        }

        if (newResource is Slave) {
          newNode.ImageIndex = slaveImageIndex;
        } else {
          newNode.ImageIndex = slaveGroupImageIndex;

          var childResources = subResources.Where(x => x.ParentResourceId == newResource.Id);
          if (childResources.Any()) {
            foreach (var resource in childResources.OrderByDescending(x => x.Name)) {
              subResources.Remove(resource);
              stack.Push(resource);
            }
            currentNode = newNode;
            currentResource = newResource;
          }
        }
        newNode.SelectedImageIndex = newNode.ImageIndex;
      }

      var ungroupedSlaves = subResources.OfType<Slave>().OrderBy(x => x.Name);
      if(ungroupedSlaves.Any()) {
        ungroupedGroupNode = new TreeNode(UNGROUPED_GROUP_NAME) {
          ForeColor = SystemColors.GrayText,
          Tag = new SlaveGroup() {
            Name = UNGROUPED_GROUP_NAME,
            Description = UNGROUPED_GROUP_DESCRIPTION
          }
        };

        foreach (var slave in ungroupedSlaves) {
          var slaveNode = new TreeNode(slave.Name) { Tag = slave };
          ungroupedGroupNode.Nodes.Add(slaveNode);
        }
        treeView.Nodes.Add(ungroupedGroupNode);
      } else if (ungroupedGroupNode != null) {
        ungroupedGroupNode.Nodes.Clear();
      }

      treeView.BeforeCheck += treeView_BeforeCheck;
      treeView.AfterCheck += treeView_AfterCheck;
      
      ExpandResourceNodesOfInterest(treeView.Nodes);

      return top;
    }

    private void ExpandResourceNodesOfInterest(TreeNodeCollection nodes) {
      foreach (TreeNode n in nodes) {
        Resource r = (Resource)n.Tag;
        if (n.Nodes.Count > 0) {
          if (HiveAdminClient.Instance.GetAvailableResourceDescendants(r.Id).OfType<SlaveGroup>().Any()) {
            n.Expand();
            ExpandResourceNodesOfInterest(n.Nodes);
          } else {
            n.Collapse();
          }
        } else {
          n.Collapse();
        }
      }
    }

    private bool IsAdmin() {
      return HiveRoles.CheckAdminUserPermissions();
    }

    #endregion
  }
}
