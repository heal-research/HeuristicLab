#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  [View("Hive Project Selector View")]
  [Content(typeof(IItemList<Project>), true)]
  public partial class HiveProjectSelector : ItemView, IDisposable {
    private const int greenFlagImageIndex = 0;
    private const int redFlagImageIndex = 1;
    private const int slaveImageIndex = 0;
    private const int slaveGroupImageIndex = 1;
    public const string additionalSlavesGroupName = "Additional Slaves";
    public const string additionalSlavesGroupDescription = "Contains additional slaves which are either ungrouped or the parenting slave group is not assigned to the selected project.";

    private const string CURRENT_SELECTION_TAG = " [current assignment]";
    private const string NEW_SELECTION_TAG = " [new assignment]";
    private const string CHANGED_SELECTION_TAG = " [changed assignment]";
    private const string ADDED_SELECTION_TAG = " [added assignment]";
    private const string REMOVED_SELECTION_TAG = " [removed assignment]";
    private const string SELECTED_TAG = " [assigned]";
    private const string INCLUDED_TAG = " [included]";
    private const string ADDED_INCLUDE_TAG = " [added include]";
    private const string REMOVED_INCLUDE_TAG = " [removed include]";

    private TreeNode additionalNode;

    private readonly HashSet<TreeNode> mainTreeNodes = new HashSet<TreeNode>();
    private readonly HashSet<TreeNode> filteredTreeNodes = new HashSet<TreeNode>();

    private readonly HashSet<Resource> availableResources = new HashSet<Resource>();
    private readonly HashSet<Resource> assignedResources = new HashSet<Resource>();
    private readonly HashSet<Resource> includedResources = new HashSet<Resource>();
    private readonly HashSet<Resource> newAssignedResources = new HashSet<Resource>();
    private readonly HashSet<Resource> newIncludedResources = new HashSet<Resource>();

    private readonly Color addedAssignmentColor = Color.FromArgb(255, 87, 191, 193); // #57bfc1
    private readonly Color removedAssignmentColor = Color.FromArgb(255, 236, 159, 72); // #ec9f48
    private readonly Color addedIncludeColor = Color.FromArgb(25, 169, 221, 221); // #a9dddd
    private readonly Color removedIncludeColor = Color.FromArgb(25, 249, 210, 145); // #f9d291
    private readonly Color selectedBackColor = Color.DodgerBlue;
    private readonly Color selectedForeColor = Color.White;
    private readonly Color controlTextColor = SystemColors.ControlText;
    private readonly Color grayTextColor = SystemColors.GrayText;

    private string currentSearchString;

    private void resetHiveResourceSelector() {
      lastSelectedProject = null;
      selectedProject = null;
      projectId = null;
    }

    private Guid jobId;
    public Guid JobId {
      get { return jobId; }
      set {
        if (jobId == value) return;
        jobId = value;
        resetHiveResourceSelector();
      }
    }

    private Guid? projectId;
    public Guid? ProjectId {
      get { return projectId; }
      set {
        if (projectId == value) return;
        projectId = value;
      }
    }

    private Guid? selectedProjectId;
    public Guid? SelectedProjectId {
      get { return selectedProjectId; }
      set {
        if (selectedProjectId == value) return;
        selectedProjectId = value;
      }
    }

    private IEnumerable<Guid> selectedResourceIds;
    public IEnumerable<Guid> SelectedResourceIds {
      get { return selectedResourceIds; }
      set {
        if (selectedResourceIds == value) return;
        selectedResourceIds = value;
      }
    }

    public bool ChangedProjectSelection {
      get {
        if ((lastSelectedProject == null && selectedProject != null) 
          || (lastSelectedProject != null && selectedProject == null)
          || (lastSelectedProject != null && selectedProject != null && lastSelectedProject.Id != selectedProject.Id))
          return true;
        else return false;
      }
    }

    public bool ChangedResources {
      get { return !assignedResources.SetEquals(newAssignedResources); }
    }

    private Project lastSelectedProject;
    private Project selectedProject;
    public Project SelectedProject {
      get { return selectedProject; }
      set {
        if (selectedProject == value) return;

        if ((JobId == Guid.Empty || JobId == null) 
          && (value == null || SelectedProject == null || value.Id != SelectedProject.Id)) selectedResourceIds = null;
        lastSelectedProject = selectedProject;
        selectedProject = value;

        UpdateResourceTree();                
        ExtractStatistics();
        OnSelectedProjectChanged();
      }
    }

    public int AssignedCores {
      get {
        HashSet<Slave> newAssignedSlaves = new HashSet<Slave>(newAssignedResources.OfType<Slave>());
        foreach(var slaveGroup in newAssignedResources.OfType<SlaveGroup>()) {
          foreach(var slave in HiveClient.Instance.GetAvailableResourceDescendants(slaveGroup.Id).OfType<Slave>()) {
            newAssignedSlaves.Add(slave);
          }
        }
        return newAssignedSlaves.Sum(x => x.Cores.GetValueOrDefault());
      }
    }

    public IEnumerable<Resource> AssignedResources {
      get { return newAssignedResources; }
      set {
        if (newAssignedResources == value) return;
        newAssignedResources.Clear();
        foreach(var resource in value) {
          newAssignedResources.Add(resource);
        }
      }
    }


    public new IItemList<Project> Content {
      get { return (IItemList<Project>)base.Content; }
      set { base.Content = value; }
    }

    public HiveProjectSelector() {
      InitializeComponent();

      projectsImageList.Images.Add(VSImageLibrary.FlagGreen);
      projectsImageList.Images.Add(VSImageLibrary.FlagRed);
      resourcesImageList.Images.Add(VSImageLibrary.MonitorLarge);
      resourcesImageList.Images.Add(VSImageLibrary.NetworkCenterLarge);
    }

    #region Overrides
    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content != null) {               
        if (SelectedProjectId.HasValue && SelectedProjectId.Value != Guid.Empty) {
          SelectedProject = GetSelectedProjectById(SelectedProjectId.Value);
        } else {
          SelectedProject = null;
        }
        //ExtractStatistics();
        UpdateProjectTree();

      } else {
        lastSelectedProject = null;
        selectedProject = null;
        selectedProjectId = null;
        projectsTreeView.Nodes.Clear();
        resourcesTreeView.Nodes.Clear();
      }
    }

    #endregion

    #region Event Handlers
    private void HiveProjectSelector_Load(object sender, EventArgs e) {
      projectsTreeView.Nodes.Clear();
      resourcesTreeView.Nodes.Clear();
    }

    private void searchTextBox_TextChanged(object sender, EventArgs e) {
      currentSearchString = searchTextBox.Text.ToLower();
      //UpdateFilteredTree();
      UpdateProjectTree();
    }

    private void searchTextBox_MouseDown(object sender, MouseEventArgs e) {
      resourcesTreeView.SelectedNode = null;
      ExtractStatistics();
    }

    private void projectsTreeView_MouseDown(object sender, MouseEventArgs e) {
      resourcesTreeView.SelectedNode = null;
      ExtractStatistics();
    }

    private void projectsTreeView_MouseDoubleClick(object sender, MouseEventArgs e) {
      OnProjectsTreeViewDoubleClicked();
    }

    private void projectsTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e) {
      var node = (Project)e.Node.Tag;
      if (HiveClient.Instance.DisabledParentProjects.Contains(node)) {
        e.Cancel = true;
      }
    }

    private void projectsTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      var node = (Project)e.Node.Tag;

      if (node == null) {
        projectsTreeView.SelectedNode = null;
      } else if (HiveClient.Instance.DisabledParentProjects.Contains(node)) {
        return;
      } else {
        ResetTreeNodes(projectsTreeView.Nodes);
        e.Node.BackColor = selectedBackColor;
        e.Node.ForeColor = selectedForeColor;

        if (node.Id == projectId) {
          e.Node.Text += CURRENT_SELECTION_TAG;
        } else if (projectId == null || projectId == Guid.Empty) {
          e.Node.Text += NEW_SELECTION_TAG;
        } else {
          e.Node.Text += CHANGED_SELECTION_TAG;
        }


      }
      SelectedProject = node;
    }

    private void resourcesTreeView_MouseDown(object sender, MouseEventArgs e) {
      var node = resourcesTreeView.GetNodeAt(new Point(e.X, e.Y));

      if (node == null && e.Button == MouseButtons.Left) {
        resourcesTreeView.SelectedNode = null;
        ExtractStatistics();
      }
    }

    private void resourcesTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e) {
      if(e.Node == null) {
        e.Cancel = true;
        resourcesTreeView.SelectedNode = null;
        ExtractStatistics();
      } else if (e.Node == additionalNode) {
        e.Cancel = true;
      } else {
        var r = (Resource)e.Node.Tag;
        if(r != null && !HiveClient.Instance.DisabledParentResources.Contains(r)) {
          ExtractStatistics(r);
        } else {
          e.Cancel = true;
        }
      }
    }

    private void resourcesTreeView_BeforeCheck(object sender, TreeViewCancelEventArgs e) {
      if(e.Node == null || e.Node == additionalNode) {
        e.Cancel = true;
      } else {
        var checkedResource = (Resource)e.Node.Tag;
        if (checkedResource == null
          || checkedResource.Id == Guid.Empty
          || HiveClient.Instance.DisabledParentResources.Contains(checkedResource)
          || newIncludedResources.Contains(checkedResource)) {
          e.Cancel = true;
        }
      }
    }

    private void resourcesTreeView_AfterCheck(object sender, TreeViewEventArgs e) {
      var checkedResource = (Resource)e.Node.Tag;
      if (e.Node.Checked) {
        newAssignedResources.Add(checkedResource);
      } else {
        newAssignedResources.Remove(checkedResource);
      }

      UpdateResourceTreeAfterCheck();
      if(resourcesTreeView.SelectedNode == null)
        ExtractStatistics();
      OnAssignedResourcesChanged();
    }

    private void resourcesTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
      return;
    }
    #endregion

    #region Helpers

    private Project GetSelectedProjectById(Guid projectId) {
      return Content.Where(x => x.Id == projectId).SingleOrDefault();
    }

    private void UpdateProjectTree() {

      if (string.IsNullOrEmpty(currentSearchString)) {
        BuildProjectTree(Content);
      } else {
        HashSet<Project> filteredProjects = new HashSet<Project>();
        foreach(var project in Content) {
          if(project.Name.ToLower().Contains(currentSearchString.ToLower())) {
            filteredProjects.Add(project);
            filteredProjects.UnionWith(Content.Where(p => HiveClient.Instance.GetAvailableProjectAncestors(project.Id).Select(x => x.Id).Contains(p.Id)));
          }
        }
        BuildProjectTree(filteredProjects);
      }
    }

    private void BuildProjectTree(IEnumerable<Project> projects) {
      projectsTreeView.Nodes.Clear();
      if (!projects.Any()) return;

      var disabledParentProjects = HiveClient.Instance.DisabledParentProjects;
      // select all top level projects (withouth parent, or without any ancestor within current project collection)
      var mainProjects = new HashSet<Project>(projects.Where(x => x.ParentProjectId == null));
      //var parentedMainProjects = new HashSet<Project>(projects
      //  .Where(x => x.ParentProjectId.HasValue
      //  && !projects.Select(y => y.Id).Contains(x.ParentProjectId.Value)
      //  && !projects.SelectMany(y => HiveClient.Instance.ProjectAncestors[y.Id]).Contains(x.ParentProjectId.Value)));
      //mainProjects.UnionWith(parentedMainProjects);
      var mainDisabledParentProjects = new HashSet<Project>(disabledParentProjects.Where(x => x.ParentProjectId == null || x.ParentProjectId == Guid.Empty));
      mainProjects.UnionWith(mainDisabledParentProjects);
      var subProbjects = new HashSet<Project>(projects.Union(disabledParentProjects).Except(mainProjects));
      //foreach (var p in subProbjects) {
      //  p.ParentProjectId = HiveClient.Instance.ProjectAncestors[p.Id].Where(x => projects.Select(y => y.Id).Contains(x)).FirstOrDefault();
      //}

      var stack = new Stack<Project>(mainProjects.OrderByDescending(x => x.Name));

      TreeNode currentNode = null;
      Project currentProject = null;

      while(stack.Any()) {
        var newProject = stack.Pop();
        var newNode = new TreeNode(newProject.Name) { Tag = newProject };

        while (currentNode != null && newProject.ParentProjectId != currentProject.Id) {
          currentNode = currentNode.Parent;
          currentProject = currentNode == null ? null : (Project)currentNode.Tag;
        }

        if (currentNode == null) {
          projectsTreeView.Nodes.Add(newNode);
          newNode.ImageIndex = greenFlagImageIndex;
        } else {
          currentNode.Nodes.Add(newNode);
          newNode.ImageIndex = redFlagImageIndex;
        }

        newNode.SelectedImageIndex = newNode.ImageIndex;

        if(disabledParentProjects.Contains(newProject)) {
          newNode.Checked = false;
          newNode.ForeColor = grayTextColor;
        }
        else if (SelectedProject != null && SelectedProject.Id.Equals(newProject.Id)) {
          newNode.BackColor = selectedBackColor;
          newNode.ForeColor = selectedForeColor;
          if(SelectedProject.Id == projectId) {
            newNode.Text += CURRENT_SELECTION_TAG;
          } else if (projectId == null || projectId == Guid.Empty) {
            newNode.Text += NEW_SELECTION_TAG;
          } else {
            newNode.Text += CHANGED_SELECTION_TAG;
          }
        }

        if (!string.IsNullOrEmpty(currentSearchString) && newProject.Name.ToLower().Contains(currentSearchString.ToLower())) {
          newNode.BackColor = Color.LightBlue;
        }

        var childProjects = subProbjects.Where(x => x.ParentProjectId == newProject.Id);
        if (childProjects.Any()) {
          foreach (var project in childProjects.OrderByDescending(x => x.Name)) {
            subProbjects.Remove(project);
            stack.Push(project);
          }
          currentNode = newNode;
          currentProject = newProject;
        }
      }

      projectsTreeView.ExpandAll();
    }

    private static IEnumerable<Resource> GetAssignedResourcesForProject(Guid projectId) {
      var assignedProjectResources = HiveServiceLocator.Instance.CallHiveService(s => s.GetAssignedResourcesForProject(projectId));
      return HiveClient.Instance.Resources.Where(x => assignedProjectResources.Select(y => y.ResourceId).Contains(x.Id));
    }

    private static IEnumerable<Resource> GetAssignedResourcesForJob(Guid jobId) {
      var assignedJobResources = HiveServiceLocator.Instance.CallHiveService(s => s.GetAssignedResourcesForJob(jobId));
      return HiveClient.Instance.Resources.Where(x => assignedJobResources.Select(y => y.ResourceId).Contains(x.Id));
    }

    private void UpdateResourceTree() {
      UpdateAvailableResources();
      UpdateAssignedResources();
      UpdateIncludedResources();
      BuildResourceTree(availableResources);
    }

    private void UpdateResourceTreeAfterCheck() {
      resourcesTreeView.BeforeCheck -= resourcesTreeView_BeforeCheck;
      resourcesTreeView.AfterCheck -= resourcesTreeView_AfterCheck;
      UpdateNewAssignedResources();
      UpdateNewIncludedResources();
      SetTreeNodes(resourcesTreeView.Nodes);
      resourcesTreeView.BeforeCheck += resourcesTreeView_BeforeCheck;
      resourcesTreeView.AfterCheck += resourcesTreeView_AfterCheck;
    }

    private void UpdateAvailableResources() {
      availableResources.Clear();
      if (selectedProject != null) {
        var assignedProjectResources = GetAssignedResourcesForProject(selectedProject.Id);
        foreach (var resource in assignedProjectResources) {
          availableResources.Add(resource);
          foreach(var descendant in HiveClient.Instance.Resources.Where(x => HiveClient.Instance.ResourceDescendants[resource.Id].Contains(x.Id))) {
            availableResources.Add(descendant);
          }
        }
      }
      //ExtractStatistics();
      //OnAssignedResourcesChanged();
    }

    private void UpdateAssignedResources() {
      assignedResources.Clear();
      newAssignedResources.Clear();

      if (JobId == Guid.Empty || JobId == null) { // new, unchanged jobs get all avaialable resources
        // update new assigned resources
        if(selectedResourceIds == null) {
          foreach (var resource in availableResources
            .Where(x => !x.ParentResourceId.HasValue
            || !availableResources.Select(y => y.Id).Contains(x.ParentResourceId.Value))) {
            newAssignedResources.Add(resource);
          }
        } else {
          foreach(var resource in availableResources.Where(x => selectedResourceIds.Contains(x.Id))) {
            newAssignedResources.Add(resource);
          }
        }
      } else if(selectedProject.Id == projectId) { // existent, unchanged jobs get all assigned resources
        // update assigned resources
        var assignedJobResources = GetAssignedResourcesForJob(JobId);
        foreach (var resource in assignedJobResources) {
          assignedResources.Add(resource);
          if (selectedResourceIds == null) {
            newAssignedResources.Add(resource);
          }
        }

        if(selectedResourceIds != null) {
          foreach (var resource in availableResources.Where(x => selectedResourceIds.Contains(x.Id))) {
            newAssignedResources.Add(resource);
          }
        }
      }

      //ExtractStatistics();
      OnAssignedResourcesChanged();
    }

    private void UpdateNewAssignedResources() {
      for(int i = newAssignedResources.Count-1; i>=0; i--) {
        if(newAssignedResources.Intersect(HiveClient.Instance.GetAvailableResourceAncestors(newAssignedResources.ElementAt(i).Id)).Any()) {
          newAssignedResources.Remove(newAssignedResources.ElementAt(i));
        }
      }
    }

    private void UpdateIncludedResources() {
      includedResources.Clear();
      newIncludedResources.Clear();

      if (JobId != Guid.Empty) {
        foreach (var item in assignedResources) {
          foreach (var descendant in HiveClient.Instance.GetAvailableResourceDescendants(item.Id)) {
            includedResources.Add(descendant);
          }
        }
      }

      foreach (var item in newAssignedResources) {
        foreach (var descendant in HiveClient.Instance.GetAvailableResourceDescendants(item.Id)) {
          newIncludedResources.Add(descendant);
        }
      }
    }

    private void UpdateNewIncludedResources() {
      newIncludedResources.Clear();
      foreach (var item in newAssignedResources) {
        foreach (var descendant in HiveClient.Instance.GetAvailableResourceDescendants(item.Id)) {
          newIncludedResources.Add(descendant);
        }
      }
    }

    private void BuildResourceTree(IEnumerable<Resource> resources) {
      resourcesTreeView.Nodes.Clear();
      if (!resources.Any()) return;

      resourcesTreeView.BeforeCheck -= resourcesTreeView_BeforeCheck;
      resourcesTreeView.AfterCheck -= resourcesTreeView_AfterCheck;

      //var disabledParentResources = HiveClient.Instance.DisabledParentResources;
      var disabledParentResources = HiveClient.Instance.GetDisabledResourceAncestors(resources);
      var mainResources = new HashSet<Resource>(resources.OfType<SlaveGroup>().Where(x => x.ParentResourceId == null));
      //var parentedMainResources = new HashSet<Resource>(resources.OfType<SlaveGroup>()
      //  .Where(x => x.ParentResourceId.HasValue && !resources.Select(y => y.Id).Contains(x.ParentResourceId.Value)));
      //mainResources.UnionWith(parentedMainResources);
      //var subResources = new HashSet<Resource>(resources.Except(mainResources));
      var mainDisabledParentResources = new HashSet<Resource>(disabledParentResources.Where(x => x.ParentResourceId == null || x.ParentResourceId == Guid.Empty));
      mainResources.UnionWith(mainDisabledParentResources);
      var subResources = new HashSet<Resource>(resources.Union(disabledParentResources).Except(mainResources).OrderByDescending(x => x.Name));

      var addedAssignments = newAssignedResources.Except(assignedResources);
      var removedAssignments = assignedResources.Except(newAssignedResources);
      var addedIncludes = newIncludedResources.Except(includedResources);
      var removedIncludes = includedResources.Except(newIncludedResources);

      TreeNode currentNode = null;
      Resource currentResource = null;

      var stack = new Stack<Resource>(mainResources.OrderByDescending(x => x.Name));

      while (stack.Any()) {
        var newResource = stack.Pop();
        var newNode = new TreeNode(newResource.Name) { Tag = newResource };

        // search for parent node of newNode and save in currentNode
        // necessary since newNodes (stack top items) might be siblings
        // or grand..grandparents of previous node (currentNode)
        while (currentNode != null && newResource.ParentResourceId != currentResource.Id) {
          currentNode = currentNode.Parent;
          currentResource = currentNode == null ? null : (Resource)currentNode.Tag;
        }

        if (currentNode == null) {
          resourcesTreeView.Nodes.Add(newNode);
        } else {
          currentNode.Nodes.Add(newNode);
        }

        if (disabledParentResources.Contains(newResource)) {
          newNode.Checked = false;
          newNode.ForeColor = grayTextColor;
        } else if (newAssignedResources.Select(x => x.Id).Contains(newResource.Id)) {
          newNode.Checked = true;
          if(!addedAssignments.Select(x => x.Id).Contains(newResource.Id) && !removedAssignments.Select(x => x.Id).Contains(newResource.Id)) {
            newNode.Text += SELECTED_TAG;
          }
        } else if (newIncludedResources.Select(x => x.Id).Contains(newResource.Id)) {
          newNode.Checked = true;
          newNode.ForeColor = grayTextColor;
        }

        if (includedResources.Select(x => x.Id).Contains(newResource.Id) && newIncludedResources.Select(x => x.Id).Contains(newResource.Id)) {
          newNode.Text += INCLUDED_TAG;
        } else if (addedIncludes.Select(x => x.Id).Contains(newResource.Id)) {
          newNode.BackColor = addedIncludeColor;
          newNode.ForeColor = grayTextColor;
          newNode.Text += ADDED_INCLUDE_TAG;
        } else if (removedIncludes.Select(x => x.Id).Contains(newResource.Id)) {
          newNode.BackColor = removedIncludeColor;
          newNode.Text += REMOVED_INCLUDE_TAG;
        }

        if (addedAssignments.Select(x => x.Id).Contains(newResource.Id)) {
          newNode.BackColor = addedAssignmentColor;
          newNode.ForeColor = controlTextColor;
          newNode.Text += ADDED_SELECTION_TAG;
        } else if (removedAssignments.Select(x => x.Id).Contains(newResource.Id)) {
          newNode.BackColor = removedAssignmentColor;
          newNode.ForeColor = controlTextColor;
          newNode.Text += REMOVED_SELECTION_TAG;
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

      var singleSlaves = subResources.OfType<Slave>();
      if (singleSlaves.Any()) {

        additionalNode = new TreeNode(additionalSlavesGroupName) {
          ForeColor = SystemColors.GrayText,
          ImageIndex = slaveGroupImageIndex,
          Tag = new SlaveGroup() {
            Name = additionalSlavesGroupName,
            Description = additionalSlavesGroupDescription
          }
        };

        foreach (var slave in singleSlaves.OrderBy(x => x.Name)) {
          var slaveNode = new TreeNode(slave.Name) { Tag = slave };

          if (newAssignedResources.Select(x => x.Id).Contains(slave.Id)) {
            slaveNode.Checked = true;
            if (!addedAssignments.Select(x => x.Id).Contains(slave.Id) &&
                !removedAssignments.Select(x => x.Id).Contains(slave.Id)) {
              slaveNode.Text += SELECTED_TAG;
            }
          }

          if (addedAssignments.Select(x => x.Id).Contains(slave.Id)) {
            slaveNode.BackColor = addedAssignmentColor;
            slaveNode.ForeColor = controlTextColor;
            slaveNode.Text += ADDED_SELECTION_TAG;
          } else if (removedAssignments.Select(x => x.Id).Contains(slave.Id)) {
            slaveNode.BackColor = removedAssignmentColor;
            slaveNode.ForeColor = controlTextColor;
            slaveNode.Text += REMOVED_SELECTION_TAG;
          }

          additionalNode.Nodes.Add(slaveNode);
        }

        resourcesTreeView.Nodes.Add(additionalNode);
      }

      ExpandResourceNodesOfInterest(resourcesTreeView.Nodes);

      resourcesTreeView.BeforeCheck += resourcesTreeView_BeforeCheck;
      resourcesTreeView.AfterCheck += resourcesTreeView_AfterCheck;
    }

    private void ExpandResourceNodesOfInterest(TreeNodeCollection nodes) {
      foreach(TreeNode n in nodes) {
        Resource r = (Resource)n.Tag;
        if(n.Nodes.Count > 0) {
          if(HiveClient.Instance.GetAvailableResourceDescendants(r.Id).OfType<SlaveGroup>().Any()            
            || HiveClient.Instance.GetAvailableResourceDescendants(r.Id).OfType<Slave>().Intersect(assignedResources.Union(newAssignedResources)).Any()
            || (n == additionalNode && additionalNode.Nodes.Count > 0 && additionalNode.Nodes.Cast<TreeNode>().Any(x => x.Checked))) {
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

    private void CollapseSlaveOnlyNodes(TreeNode tn) {
      if (!(tn.Tag is Resource)) return;

      Resource r = (Resource)tn.Tag;
      if (HiveClient.Instance.GetAvailableResourceDescendants(r.Id).OfType<SlaveGroup>().Any()) {
        tn.Expand();
        foreach (TreeNode n in tn.Nodes) CollapseSlaveOnlyNodes(n);
      } else {
        tn.Collapse();
      }
    }

    private void ExtractStatistics(Resource resource = null) { 
      HashSet<Slave> newAssignedSlaves = new HashSet<Slave>(newAssignedResources.OfType<Slave>());
      foreach (var slaveGroup in newAssignedResources.OfType<SlaveGroup>()) {
        foreach (var slave in HiveClient.Instance.GetAvailableResourceDescendants(slaveGroup.Id).OfType<Slave>()) {
          newAssignedSlaves.Add(slave);
        }
      }

      HashSet<Slave> selectedSlaves = null;

      if (resource != null) {
        var slaveGroup = resource as SlaveGroup;
        if (slaveGroup != null) {
          selectedSlaves = new HashSet<Slave>(HiveClient.Instance.GetAvailableResourceDescendants(slaveGroup.Id).OfType<Slave>());
        } else {
          selectedSlaves = new HashSet<Slave>(new[] { resource as Slave });
        }
      } else {
        selectedSlaves = newAssignedSlaves;
      }

      int sumCores = selectedSlaves.Sum(x => x.Cores.GetValueOrDefault());
      int sumFreeCores = selectedSlaves.Sum(x => x.FreeCores.GetValueOrDefault());
      double sumMemory = selectedSlaves.Sum(x => x.Memory.GetValueOrDefault()) / 1024.0;
      double sumFreeMemory = selectedSlaves.Sum(x => x.FreeMemory.GetValueOrDefault()) / 1024.0;

      coresSummaryLabel.Text = string.Format("{0} Total ({1} Free / {2} Used)", sumCores, sumFreeCores, sumCores - sumFreeCores);
      memorySummaryLabel.Text = string.Format("{0:0.00} GB Total ({1:0.00} GB Free / {2:0.00} GB Used)", sumMemory, sumFreeMemory, sumMemory - sumFreeMemory);
    }

    private void StyleTreeNode(TreeNode n, string name) {
      n.Text = name;
      n.BackColor = Color.Transparent;
      n.ForeColor = Color.Black;

      if(n.Tag is Project) {
        var p = (Project)n.Tag;
        if(HiveClient.Instance.DisabledParentProjects.Select(x => x.Id).Contains(p.Id)) {
          n.Checked = false;
          n.ForeColor = grayTextColor;
        }
      } else if(n.Tag is Resource) {
        var r = (Resource)n.Tag;
        if(HiveClient.Instance.DisabledParentResources.Select(x => x.Id).Contains(r.Id) || n == additionalNode) {
          n.Checked = false;
          n.ForeColor = grayTextColor;
        }
      }
    }

    private void ResetTreeNodes(TreeNodeCollection nodes) {
      foreach (TreeNode n in nodes) {
        string name = "";
        if (n.Tag is Project) name = ((Project)n.Tag).Name;
        else if (n.Tag is Resource) name = ((Resource)n.Tag).Name;
        StyleTreeNode(n, name);
        if (n.Nodes.Count > 0) {
          ResetTreeNodes(n.Nodes);
        }
      }
    }

    private void SetTreeNodes(TreeNodeCollection nodes) {
      var addedAssignments = newAssignedResources.Except(assignedResources);
      var removedAssignments = assignedResources.Except(newAssignedResources);
      var addedIncludes = newIncludedResources.Except(includedResources);
      var removedIncludes = includedResources.Except(newIncludedResources);

      foreach (TreeNode n in nodes) {

        if(n.Tag is Resource) {
          // reset
          var resource = (Resource)n.Tag;
          n.Text = resource.Name;
          n.BackColor = Color.Transparent;
          n.ForeColor = Color.Black;
          n.Checked = false;

          if (HiveClient.Instance.DisabledParentResources.Select(x => x.Id).Contains(resource.Id) || n == additionalNode) {
            n.ForeColor = grayTextColor;
          }

          // add additional info
          if (newAssignedResources.Select(x => x.Id).Contains(resource.Id)) {
            n.Checked = true;
            if (!addedAssignments.Select(x => x.Id).Contains(resource.Id) && !removedAssignments.Select(x => x.Id).Contains(resource.Id)) {
              n.Text += SELECTED_TAG;
            }
          } else if (newIncludedResources.Select(x => x.Id).Contains(resource.Id)) {
            n.Checked = true;
            n.ForeColor = grayTextColor;
          }

          if (includedResources.Select(x => x.Id).Contains(resource.Id) && newIncludedResources.Select(x => x.Id).Contains(resource.Id)) {
            n.Text += INCLUDED_TAG;
          } else if (addedIncludes.Select(x => x.Id).Contains(resource.Id)) {
            n.BackColor = addedIncludeColor;
            n.ForeColor = grayTextColor;
            n.Text += ADDED_INCLUDE_TAG;
          } else if (removedIncludes.Select(x => x.Id).Contains(resource.Id)) {
            n.BackColor = removedIncludeColor;
            n.Text += REMOVED_INCLUDE_TAG;
          }

          if (addedAssignments.Select(x => x.Id).Contains(resource.Id)) {
            n.BackColor = addedAssignmentColor;
            n.ForeColor = controlTextColor;
            n.Text += ADDED_SELECTION_TAG;
          } else if (removedAssignments.Select(x => x.Id).Contains(resource.Id)) {
            n.BackColor = removedAssignmentColor;
            n.ForeColor = controlTextColor;
            n.Text += REMOVED_SELECTION_TAG;
          }
        }

        if(n.Nodes.Count > 0) {
          SetTreeNodes(n.Nodes);
        }
      }
    }

    #endregion

    #region Events
    public event EventHandler SelectedProjectChanged;
    private void OnSelectedProjectChanged() {
      var handler = SelectedProjectChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler AssignedResourcesChanged;
    private void OnAssignedResourcesChanged() {
      var handler = AssignedResourcesChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ProjectsTreeViewDoubleClicked;
    private void OnProjectsTreeViewDoubleClicked() {
      var handler = ProjectsTreeViewDoubleClicked;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}