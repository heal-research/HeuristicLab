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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Clients.Access;
using HeuristicLab.Clients.Hive.Views;
using HeuristicLab.Collections;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("Projects View")]
  [Content(typeof(IItemList<Project>), false)]
  public partial class ProjectsView : ItemView, IDisposable {
    private const int greenFlagImageIndex = 0;
    private const int redFlagImageIndex = 1;
    private const string SELECTED_TAG = ""; // " [selected]";
    private const string NOT_STORED_TAG = "*"; // " [not stored]";
    private const string CHANGES_NOT_STORED_TAG = "*"; // " [changes not stored]";

    private readonly Color selectedBackColor = Color.DodgerBlue;
    private readonly Color selectedForeColor = Color.White;
    private readonly Color grayTextColor = SystemColors.GrayText;

    private Project selectedProject = null;
    public Project SelectedProject {
      get { return selectedProject; }
      set { if (selectedProject != value) ChangeSelectedProject(value); }
    }

    private readonly object locker = new object();

    public new IItemList<Project> Content {
      get { return (IItemList<Project>)base.Content; }
      set { base.Content = value; }
    }

    public ProjectsView() {
      InitializeComponent();

      projectsTreeView.ImageList.Images.Add(VSImageLibrary.FlagGreen);
      projectsTreeView.ImageList.Images.Add(VSImageLibrary.FlagRed);

      HiveAdminClient.Instance.Refreshing += HiveAdminClient_Instance_Refreshing;
      HiveAdminClient.Instance.Refreshed += HiveAdminClient_Instance_Refreshed;
      AccessClient.Instance.Refreshing += AccessClient_Instance_Refreshing;
      AccessClient.Instance.Refreshed += AccessClient_Instance_Refreshed;
    }

    #region Overrides
    protected override void OnClosing(FormClosingEventArgs e) {
      AccessClient.Instance.Refreshed -= AccessClient_Instance_Refreshed;
      AccessClient.Instance.Refreshing -= AccessClient_Instance_Refreshing;
      HiveAdminClient.Instance.Refreshed -= HiveAdminClient_Instance_Refreshed;
      HiveAdminClient.Instance.Refreshing -= HiveAdminClient_Instance_Refreshing;
      base.OnClosing(e);
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += Content_ItemsAdded;
      Content.ItemsRemoved += Content_ItemsRemoved;
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsRemoved -= Content_ItemsRemoved;
      Content.ItemsAdded -= Content_ItemsAdded;
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        projectsTreeView.Nodes.Clear();
        projectView.Content = null;
        projectPermissionsView.Content = null;
        projectResourcesView.Content = null;
        projectJobsView.Content = null;
        selectedProject = null;
      } else {
        BuildProjectTree(Content);
      }
      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();      

      bool locked = Content == null || Locked || ReadOnly;      
      bool parentOwner = selectedProject != null && HiveAdminClient.Instance.CheckOwnershipOfParentProject(selectedProject, UserInformation.Instance.User.Id);
      bool selectedProjectDisabled = selectedProject == null 
                                     || selectedProject != null && selectedProject.Id == Guid.Empty;

      bool selectedProjectHasJobs =
        !selectedProjectDisabled && HiveAdminClient.Instance.Jobs.ContainsKey(selectedProject.Id)
                                 && HiveAdminClient.Instance.Jobs[selectedProject.Id] != null
                                 && HiveAdminClient.Instance.Jobs[selectedProject.Id].Any();

      bool addLocked = locked
                       || (selectedProject == null && !IsAdmin())
                       || (selectedProject != null && selectedProject.Id == Guid.Empty)
                       || (selectedProject != null && !IsAdmin() && !parentOwner && selectedProject.OwnerUserId != UserInformation.Instance.User.Id)
                       || (selectedProject != null && (DateTime.Now < selectedProject.StartDate || DateTime.Now > selectedProject.EndDate));

      bool deleteLocked = locked
                          || !Content.Any()
                          || selectedProject == null
                          || Content.Any(x => x.ParentProjectId == selectedProject.Id)
                          || selectedProjectHasJobs
                          || (!IsAdmin() && !parentOwner);

      bool saveLocked = locked
                        || !Content.Any()
                        || selectedProject == null
                        || (!IsAdmin() && !parentOwner && selectedProject.OwnerUserId != UserInformation.Instance.User.Id);


      addButton.Enabled = !addLocked;
      removeButton.Enabled = !deleteLocked;
      saveProjectButton.Enabled = !saveLocked;

      projectView.Enabled = !locked;
      projectPermissionsView.Enabled = !locked && !selectedProjectDisabled;
      projectResourcesView.Enabled = !locked && !selectedProjectDisabled;
      projectJobsView.Enabled = !locked && !selectedProjectDisabled;

      projectView.Locked = locked;
      projectPermissionsView.Locked = locked || selectedProjectDisabled;
      projectResourcesView.Locked = locked || selectedProjectDisabled;
      projectJobsView.Locked = locked || selectedProjectDisabled;
    }
    #endregion

    #region Event Handlers
    private void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<Project>> e) {
      if (InvokeRequired) Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<Project>>>)Content_ItemsAdded, sender, e);
      else {
        OnContentChanged();
      }
    }

    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Project>> e) {
      if (InvokeRequired) Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<Project>>>)Content_ItemsRemoved, sender, e);
      else {
        OnContentChanged();
      }
    }

    private void ProjectViewContent_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (InvokeRequired) Invoke((Action<object, PropertyChangedEventArgs>)ProjectViewContent_PropertyChanged, sender, e);
      else {
        OnContentChanged();
      }
    }

    private void HiveAdminClient_Instance_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)HiveAdminClient_Instance_Refreshing, sender, e);
      else {
        var mainForm = MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>();
        mainForm.AddOperationProgressToView(this, "Refreshing ...");
        SetEnabledStateOfControls();
      }
    }

    private void HiveAdminClient_Instance_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)HiveAdminClient_Instance_Refreshed, sender, e);
      else {
        var mainForm = MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>();
        mainForm.RemoveOperationProgressFromView(this);
        SetEnabledStateOfControls();
      }
    }

    private void AccessClient_Instance_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)AccessClient_Instance_Refreshing, sender, e);
      else {
        var mainForm = MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>();
        mainForm.AddOperationProgressToView(this, "Refreshing ...");
        SetEnabledStateOfControls();
      }
    }

    private void AccessClient_Instance_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)AccessClient_Instance_Refreshed, sender, e);
      else {
        var mainForm = MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>();
        mainForm.RemoveOperationProgressFromView(this);
        SetEnabledStateOfControls();
      }
    }

    private async void ProjectsView_Load(object sender, EventArgs e) {
      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => UpdateProjects());
    }

    private async void refreshButton_Click(object sender, EventArgs e) {
      lock (locker) {
        if (!refreshButton.Enabled) return;
        refreshButton.Enabled = false;
      }

      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => UpdateProjects(),
        finallyCallback: () => {
          refreshButton.Enabled = true;
        });
    }

    private void addButton_Click(object sender, EventArgs e) {      

      if (selectedProject == null && !IsAdmin()) {
        MessageBox.Show(
          "You are not allowed to add a root project - please select a parent project.",
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
        return;
      }
      
      if (selectedProject != null && selectedProject.Id == Guid.Empty) {
        MessageBox.Show(
          "You cannot add a project to a not yet stored project.",
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
        return;
      }

      var project = new Project {
        Name = "New Project",
        OwnerUserId = UserInformation.Instance.User.Id,        
      };
      if(selectedProject != null) {
        project.ParentProjectId = selectedProject.Id;
        project.EndDate = selectedProject.EndDate;
      }

      SelectedProject = project;
      Content.Add(project);
    }

    private async void removeButton_Click(object sender, EventArgs e) {
      if (selectedProject == null) return;

      lock (locker) {
        // for details go to ChangeSelectedProject(..)
        if (!removeButton.Enabled) return;
        removeButton.Enabled = false;
      }

      var result = MessageBox.Show(
        "Do you really want to delete " + selectedProject.Name + "?",
        "HeuristicLab Hive Administrator",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

      if (result == DialogResult.Yes) {
        await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
          action: () => {
            RemoveProject(selectedProject);  
          });
      }
      SetEnabledStateOfControls();
    }

    private async void saveProjectButton_Click(object sender, EventArgs e) {
      lock (locker) {
        if (!saveProjectButton.Enabled) return;
        saveProjectButton.Enabled = false;
      }

      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => {
          //if (selectedProject != null && selectedProject.Id == Guid.Empty)
          //  SelectedProject = HiveAdminClient.Instance.GetAvailableProjectAncestors(selectedProject.Id).LastOrDefault();
          var projectsToSave = Content.Where(x => x.Id == Guid.Empty || x.Modified);
          foreach (var project in projectsToSave)
            project.Store();
          
          UpdateProjects();
        },
        finallyCallback: () => saveProjectButton.Enabled = true);

      OnContentChanged();
    }

    private void projectsTreeView_MouseDown(object sender, MouseEventArgs e) {
      var node = projectsTreeView.GetNodeAt(e.Location);
      if (node == null) return;
      var p = (Project)node.Tag;
      if(!HiveAdminClient.Instance.DisabledParentProjects.Contains(p)) ChangeSelectedProjectNode(node);
    }

    private void projectsTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e) {
      e.Cancel = true;
    }

    private void projectsTreeView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect == DragDropEffects.None) return;

      var sourceNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
      if (sourceNode == null) return;
      var sourceProject = ((Project)sourceNode.Tag);
      if (sourceProject == null) return;

      var treeView = (TreeView)sender;
      if (sourceNode.TreeView != treeView) return;

      var targetPoint = treeView.PointToClient(new Point(e.X, e.Y));
      var targetNode = treeView.GetNodeAt(targetPoint);

      var targetProject = (targetNode != null) ? (Project)targetNode.Tag : null;

      if (!HiveAdminClient.Instance.CheckParentChange(sourceProject, targetProject)) {
        MessageBox.Show(
          "You cannot drag projects to this project.",
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
        return;
      }

      if (sourceNode.Parent == null) {
        treeView.Nodes.Remove(sourceNode);
      } else {
        sourceNode.Parent.Nodes.Remove(sourceNode);
        sourceProject.ParentProjectId = null;
      }

      if (targetNode == null) {
        treeView.Nodes.Add(sourceNode);
      } else if(targetProject.Id != Guid.Empty) {
        targetNode.Nodes.Add(sourceNode);
        sourceProject.ParentProjectId = targetProject.Id;
      }

      SelectedProject = sourceProject;
      OnContentChanged();
    }

    private void projectsTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      var sourceNode = (TreeNode)e.Item;
      if (IsAuthorized((Project)sourceNode.Tag))
        DoDragDrop(sourceNode, DragDropEffects.All);
    }

    private void projectsTreeView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.Move;

      var sourceNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
      var sourceProject = ((Project)sourceNode.Tag);

      var targetPoint = projectsTreeView.PointToClient(new Point(e.X, e.Y));
      var targetNode = projectsTreeView.GetNodeAt(targetPoint);
      var targetProject = (targetNode != null) ? (Project)targetNode.Tag : null;

      if ((!IsAdmin() && (targetNode == null || targetProject == null))
      || sourceNode == null
      || sourceProject == null
      || sourceNode == targetNode
      || !HiveAdminClient.Instance.CheckParentChange(sourceProject, targetProject)) {
        e.Effect = DragDropEffects.None;
      }
    }

    private void projectsTreeView_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
      e.Action = DragAction.Continue;
    }
    #endregion

    #region Helpers
    private void BuildProjectTree(IEnumerable<Project> projects) {
      projectsTreeView.Nodes.Clear();
      if (!projects.Any()) return;

      var disabledParentProjects = HiveAdminClient.Instance.DisabledParentProjects;
      var mainProjects = new HashSet<Project>(projects.Where(x => x.ParentProjectId == null));
      //var parentedMainProjects = new HashSet<Project>(projects
      //  .Where(x => x.ParentProjectId.HasValue
      //  && !projects.Select(y => y.Id).Contains(x.ParentProjectId.Value)));
      //mainProjects.UnionWith(parentedMainProjects);
      var mainDisabledParentProjects = new HashSet<Project>(disabledParentProjects.Where(x => x.ParentProjectId == null || x.ParentProjectId == Guid.Empty));
      mainProjects.UnionWith(mainDisabledParentProjects);
      var subProbjects = new HashSet<Project>(projects.Union(disabledParentProjects).Except(mainProjects));

      var stack = new Stack<Project>(mainProjects.OrderByDescending(x => x.Name));
      if (selectedProject != null) SelectedProject = projects.Where(x => x.Id == selectedProject.Id).FirstOrDefault();
      bool nodeSelected = false;

      TreeNode currentNode = null;
      Project currentProject = null;

      while (stack.Any()) {
        var newProject = stack.Pop();
        var newNode = new TreeNode(newProject.Name) { Tag = newProject };
        StyleTreeNode(newNode, newProject);
      
        if (selectedProject == null && !disabledParentProjects.Contains(newProject)) {
          SelectedProject = newProject;
        }
        if (!nodeSelected && selectedProject != null && selectedProject.Id == newProject.Id) {
          newNode.BackColor = selectedBackColor;
          newNode.ForeColor = selectedForeColor;
          newNode.Text += SELECTED_TAG;
          nodeSelected = true;
        }

        // search for parent node of newNode and save in currentNode
        // necessary since newNodes (stack top items) might be siblings
        // or grand..grandparents of previous node (currentNode)
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

        if (disabledParentProjects.Contains(newProject)) {
          newNode.Checked = false;
          newNode.ForeColor = grayTextColor;
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

    private void StyleTreeNode(TreeNode n, Project p) {
      n.Text = p.Name;
      n.BackColor = Color.Transparent;
      n.ForeColor = Color.Black;

      if (HiveAdminClient.Instance.DisabledParentProjects.Select(x => x.Id).Contains(p.Id)) {
        n.ForeColor = grayTextColor;
      } else if (p.Id == Guid.Empty) {
        n.Text += NOT_STORED_TAG;
      } else if (p.Modified) {
        n.Text += CHANGES_NOT_STORED_TAG;
      }
    }

    private void ResetTreeNodes(TreeNodeCollection nodes) {
      foreach (TreeNode n in nodes) {
        StyleTreeNode(n, (Project)n.Tag);
        if (n.Nodes.Count > 0) {
          ResetTreeNodes(n.Nodes);
        }
      }
    }

    private void ChangeSelectedProject(Project project) {
      projectView.Content = project;
      projectPermissionsView.Content = project;
      projectResourcesView.Content = project;
      projectJobsView.Content = project;
      selectedProject = project;
      SetEnabledStateOfControls();
    }

    private void ChangeSelectedProjectNode(TreeNode projectNode) {
      if (projectNode == null) return;
      SelectedProject = (Project)projectNode.Tag;
      ResetTreeNodes(projectsTreeView.Nodes);
      projectNode.BackColor = selectedBackColor;
      projectNode.ForeColor = selectedForeColor;
      projectNode.Text += SELECTED_TAG;
    }

    private void UpdateProjects() {
      try {
        HiveAdminClient.Instance.Refresh();
        Content = HiveAdminClient.Instance.Projects;
      } catch (AnonymousUserException) {
        ShowHiveInformationDialog();
      }
    }

    private void RemoveProject(Project project) {
      if (project == null) return;

      try {
        if (project.Id != Guid.Empty) {
          SelectedProject = HiveAdminClient.Instance.GetAvailableProjectAncestors(project.Id).LastOrDefault();
          HiveAdminClient.Delete(project);          
          UpdateProjects();
        } else {          
          SelectedProject = Content.FirstOrDefault(x => x.Id == project.ParentProjectId);
          Content.Remove(project);
        }
      } catch (AnonymousUserException) {
        ShowHiveInformationDialog();
      }
    }

    private bool IsAuthorized(Project project) {
      return project != null && UserInformation.Instance.UserExists;
    }

    private bool IsAdmin() {
      return HiveRoles.CheckAdminUserPermissions();
    }

    private void ShowHiveInformationDialog() {
      if (InvokeRequired) Invoke((Action)ShowHiveInformationDialog);
      else {
        using (HiveInformationDialog dialog = new HiveInformationDialog()) {
          dialog.ShowDialog(this);
        }
      }
    }
    #endregion
  }
}
