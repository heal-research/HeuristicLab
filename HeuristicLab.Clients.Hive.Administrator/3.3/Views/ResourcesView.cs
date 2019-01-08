#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [View("Resources View")]
  [Content(typeof(IItemList<Resource>), false)]
  public partial class ResourcesView : ItemView, IDisposable {
    private const int slaveImageIndex = 0;
    private const int slaveGroupImageIndex = 1;
    public const string UNGROUPED_GROUP_NAME = "UNGROUPED";
    public const string UNGROUPED_GROUP_DESCRIPTION = "Contains slaves that are not assigned to any group.";
    private const string SELECTED_TAG = ""; // " [selected]";
    private const string NOT_STORED_TAG = "*"; // " [not stored]";
    private const string CHANGES_NOT_STORED_TAG = "*"; // " [changes not stored]";

    private readonly Color changedColor = Color.FromArgb(255, 87, 191, 193); // #57bfc1
    private readonly Color selectedBackColor = Color.DodgerBlue;
    private readonly Color selectedForeColor = Color.White;
    private readonly Color calculatingColor = Color.FromArgb(255, 58, 114, 35); // #3a7223
    private readonly Color offlineColor = Color.FromArgb(255, 187, 36, 36); // #bb2424
    private readonly Color grayTextColor = SystemColors.GrayText;


    private TreeNode ungroupedGroupNode;

    private Resource selectedResource = null;
    public Resource SelectedResource {
      get { return selectedResource; }
      set { if (selectedResource != value) ChangeSelectedResource(value); }
    }

    private readonly object locker = new object();
    private bool refreshingInternal = false;
    private bool refreshingExternal = false;

    public new IItemList<Resource> Content {
      get { return (IItemList<Resource>)base.Content; }
      set { base.Content = value; }
    }

    public ResourcesView() {
      InitializeComponent();

      treeView.ImageList.Images.Add(VSImageLibrary.MonitorLarge);
      treeView.ImageList.Images.Add(VSImageLibrary.NetworkCenterLarge);

      HiveAdminClient.Instance.Refreshing += HiveAdminClient_Instance_Refreshing;
      HiveAdminClient.Instance.Refreshed += HiveAdminClient_Instance_Refreshed;
    }

    #region Overrides
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
        treeView.Nodes.Clear();
        viewHost.Content = null;
        scheduleView.Content = null;
      } else {
        BuildResourceTree(Content);
      }
      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();

      bool locked = Content == null || Locked || ReadOnly;
      bool addLocked = locked
                    || !IsAdmin()
                    || (selectedResource is Slave && selectedResource.ParentResourceId != null)
                    || (selectedResource != null && selectedResource.Id == Guid.Empty);

      HashSet<Guid> descendantResources = null;
      bool selectedRDeleteLocked = selectedResource == null
                              || (selectedResource.Id != Guid.Empty && (!HiveAdminClient.Instance.ResourceDescendants.TryGetValue(selectedResource.Id, out descendantResources) || descendantResources.Any()));

      var nodes = GetCheckedNodes(treeView.Nodes).ToList();
      var checkedResources = nodes.Select(x => x.Tag).OfType<Resource>().ToList();
      bool checkedRDeleteLocked = false;
      for (int i = 0; !checkedRDeleteLocked && i < checkedResources.Count; i++) {
        if (checkedResources[i].Id != Guid.Empty &&
            (!HiveAdminClient.Instance.ResourceDescendants.TryGetValue(checkedResources[i].Id, out descendantResources) ||
             descendantResources.Any()))
          checkedRDeleteLocked = true;
      }

      bool deleteLocked = locked
                          || !IsAdmin()
                          || !Content.Any()
                          || checkedResources.Any() && checkedRDeleteLocked
                          || !checkedResources.Any() && selectedRDeleteLocked;

      bool saveLocked = locked
                       || !IsAdmin()
                       || !Content.Any()
                       || selectedResource == null;

      btnAddGroup.Enabled = !addLocked;
      btnRemoveGroup.Enabled = !deleteLocked;
      btnSave.Enabled = !saveLocked;
      viewHost.Locked = locked || !IsAdmin();
      scheduleView.Locked = locked || !IsAdmin();
    }
    #endregion

    #region Event Handlers
    private void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<Resource>> e) {
      if (InvokeRequired) Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<Resource>>>)Content_ItemsAdded, sender, e);
      else {
        OnContentChanged();
      }
    }

    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Resource>> e) {
      if (InvokeRequired) Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<Resource>>>)Content_ItemsRemoved, sender, e);
      else {
        OnContentChanged();
      }
    }

    private void SlaveViewContent_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (InvokeRequired) Invoke((Action<object, PropertyChangedEventArgs>)SlaveViewContent_PropertyChanged, sender, e);
      else {
        OnContentChanged();
        if (e.PropertyName == "HbInterval") {
          UpdateChildHbIntervall((Resource)viewHost.Content);
        }
      }
    }

    private void HiveAdminClient_Instance_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)HiveAdminClient_Instance_Refreshing, sender, e);
      else {
        lock (locker) {
          if (refreshingExternal) return;
          if (!refreshingInternal) refreshingExternal = true;
        }

        Progress.Show(this, "Refreshing ...", ProgressMode.Indeterminate);
        SetEnabledStateOfControls();
      }
    }

    private void HiveAdminClient_Instance_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)HiveAdminClient_Instance_Refreshed, sender, e);
      else {
        if (refreshingExternal) refreshingExternal = false;
        Content = HiveAdminClient.Instance.Resources;

        Progress.Hide(this);
        SetEnabledStateOfControls();
      }
    }

    private async void ResourcesView_Load(object sender, EventArgs e) {
      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(() => UpdateResources());
    }

    private void ResourcesView_Disposed(object sender, EventArgs e) {
      HiveAdminClient.Instance.Refreshed -= HiveAdminClient_Instance_Refreshed;
      HiveAdminClient.Instance.Refreshing -= HiveAdminClient_Instance_Refreshing;
    }

    private async void btnRefresh_Click(object sender, EventArgs e) {
      lock (locker) {
        if (!btnRefresh.Enabled) return;
        btnRefresh.Enabled = false;
      }

      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => UpdateResources(),
        finallyCallback: () => btnRefresh.Enabled = true);
    }

    private void btnAddGroup_Click(object sender, EventArgs e) {
      var parentResourceId = selectedResource is SlaveGroup ? selectedResource.Id : (Guid?)null;

      var group = new SlaveGroup {
        Name = "New Group",
        OwnerUserId = UserInformation.Instance.User.Id,
        ParentResourceId = parentResourceId
      };

      SelectedResource = group;
      Content.Add(group);
    }

    private async void btnRemoveGroup_Click(object sender, EventArgs e) {
      var nodes = GetCheckedNodes(treeView.Nodes).ToList();
      var checkedResources = nodes.Select(x => x.Tag).OfType<Resource>().ToList();
      if (selectedResource == null && !checkedResources.Any()) return;

      lock (locker) {
        if (!btnRemoveGroup.Enabled) return;
        btnRemoveGroup.Enabled = false;
      }

      if (checkedResources.Count > 0) {
        var result = MessageBox.Show(
          "Do you really want to delete all " + checkedResources.Count + " checked resources?",
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question);
        if (result == DialogResult.Yes) {
          await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
            action: () => {
              RemoveResource(checkedResources);
            });
        }
      } else {
        var res = checkedResources.Any() ? checkedResources.First() : selectedResource;
        var result = MessageBox.Show(
          "Do you really want to delete the selected resource " + res.Name + "?",
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question);
        if (result == DialogResult.Yes) {
          await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
            action: () => {
              RemoveResource(res);
            });
        }
      }

      OnContentChanged();
      SetEnabledStateOfControls();
    }

    private async void btnSave_Click(object sender, EventArgs e) {
      lock (locker) {
        if (!btnSave.Enabled) return;
        btnSave.Enabled = false;
      }

      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => {
          var resourcesToSave = Content.Where(x => x.Id == Guid.Empty || x.Modified);
          foreach (var resource in resourcesToSave)
            resource.Store();
          UpdateResources();
        });

      OnContentChanged();
      SetEnabledStateOfControls();
    }

    private void treeSlaveGroup_MouseDown(object sender, MouseEventArgs e) {
      var node = treeView.GetNodeAt(e.Location);
      if (node == null || node == ungroupedGroupNode) return;
      var r = (Resource)node.Tag;
      if (!HiveAdminClient.Instance.DisabledParentResources.Contains(r)) ChangeSelectedResourceNode(node);
    }

    private void treeSlaveGroup_BeforeSelect(object sender, TreeViewCancelEventArgs e) {
      e.Cancel = true;
    }

    private void treeSlaveGroup_BeforeCheck(object sender, TreeViewCancelEventArgs e) {
      if (!IsAdmin() || e.Node == ungroupedGroupNode) {
        e.Cancel = true;
      } else {
        var r = (Resource)e.Node.Tag;
        if (HiveAdminClient.Instance.DisabledParentResources.Contains(r)) {
          e.Cancel = true;
        }
      }
    }

    private void treeSlaveGroup_AfterCheck(object sender, TreeViewEventArgs e) {
      SetEnabledStateOfControls();
    }

    private void treeSlaveGroup_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect == DragDropEffects.None) return;

      var targetNode = treeView.GetNodeAt(treeView.PointToClient(new Point(e.X, e.Y)));
      var targetResource = (targetNode != null) ? (Resource)targetNode.Tag : null;
      var resources = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IEnumerable<Resource>;

      foreach (var r in resources) {
        r.ParentResourceId = targetResource != null ? targetResource.Id : (Guid?)null;
      }

      // TODO
      //HiveAdminClient.Instance.UpdateResourceGenealogy(Content);
      OnContentChanged();
    }

    private void treeSlaveGroup_ItemDrag(object sender, ItemDragEventArgs e) {
      if (!IsAdmin()) return;

      var nodes = GetCheckedNodes(treeView.Nodes).ToList();
      TreeNode sourceNode = (TreeNode)e.Item;
      if (!sourceNode.Checked) nodes.Add(sourceNode);
      nodes.Remove(ungroupedGroupNode);
      ungroupedGroupNode.Checked = false;
      var resources = nodes.Select(x => x.Tag).OfType<Resource>().ToList();

      if (resources.Count > 0) {
        DataObject data = new DataObject();
        data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, resources);
        var action = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
        if (action.HasFlag(DragDropEffects.Move)) {
          foreach (var node in nodes) node.Remove();
          StyleTreeNode(ungroupedGroupNode, (Resource)ungroupedGroupNode.Tag, resources);
        }
      }
    }

    private IEnumerable<TreeNode> GetCheckedNodes(TreeNodeCollection nodes) {
      if (nodes != null) {
        foreach (var node in nodes.OfType<TreeNode>()) {
          if (node.Checked && node != ungroupedGroupNode) yield return node;
          foreach (var child in GetCheckedNodes(node.Nodes))
            yield return child;
        }
      }
    }

    private void treeSlaveGroup_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.Move;
      var resources = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IEnumerable<Resource>;
      var targetNode = treeView.GetNodeAt(treeView.PointToClient(new Point(e.X, e.Y)));
      var targetResource = (targetNode != null ? targetNode.Tag : null) as Resource;

      if (!IsAdmin()
        || resources == null
        || !resources.Any()
        || resources.Any(x => !HiveAdminClient.Instance.CheckParentChange(x, targetResource))
        || (targetNode != null && (targetNode == ungroupedGroupNode || targetNode.Parent == ungroupedGroupNode))) {
        e.Effect = DragDropEffects.None;
      }
    }

    private void TabSlaveGroup_TabIndexChanged(object sender, EventArgs e) {
      throw new NotImplementedException();
    }

    private async void TabSlaveGroup_Selected(object sender, System.Windows.Forms.TabControlEventArgs e) {
      if (e.TabPage == tabSchedule) {
        await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
          action: () => UpdateSchedule(),
          finallyCallback: () => scheduleView.Content = HiveAdminClient.Instance.Downtimes);
      }
      SetEnabledStateOfControls();
    }
    #endregion

    #region Helpers
    private void BuildResourceTree(IEnumerable<Resource> resources) {
      treeView.Nodes.Clear();
      if (!resources.Any()) return;

      var disabledParentResources = HiveAdminClient.Instance.DisabledParentResources;
      var mainResources = new HashSet<Resource>(resources.OfType<SlaveGroup>()
        .Where(x => x.ParentResourceId == null));
      //var parentedMainResources = new HashSet<Resource>(resources.OfType<SlaveGroup>()
      //  .Where(x => x.ParentResourceId.HasValue && !resources.Select(y => y.Id).Contains(x.ParentResourceId.Value)));
      //mainResources.UnionWith(parentedMainResources);
      var mainDisabledParentResources = new HashSet<Resource>(disabledParentResources.Where(x => x.ParentResourceId == null || x.ParentResourceId == Guid.Empty));
      mainResources.UnionWith(mainDisabledParentResources);
      var subResources = new HashSet<Resource>(resources.Union(disabledParentResources).Except(mainResources).OrderByDescending(x => x.Name));

      var stack = new Stack<Resource>(mainResources.OrderByDescending(x => x.Name));
      if (selectedResource != null) SelectedResource = resources.Where(x => x.Id == selectedResource.Id).FirstOrDefault();
      bool nodeSelected = false;

      TreeNode currentNode = null;
      Resource currentResource = null;

      while (stack.Any()) {
        var newResource = stack.Pop();
        var newNode = new TreeNode(newResource.Name) { Tag = newResource };
        StyleTreeNode(newNode, newResource, resources);

        if (selectedResource == null && !disabledParentResources.Contains(newResource)) {
          SelectedResource = newResource;
        }
        if (!nodeSelected && selectedResource != null && newResource.Id == selectedResource.Id) {
          newNode.BackColor = selectedBackColor;
          newNode.ForeColor = selectedForeColor;
          newNode.Text += SELECTED_TAG;
          nodeSelected = true;
        }

        if (disabledParentResources.Contains(newResource)) {
          newNode.Checked = false;
          newNode.ForeColor = grayTextColor;
        }

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

        if (newResource is SlaveGroup) {
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

      // collapse slave-only nodes
      foreach (TreeNode n in treeView.Nodes) {
        CollapseSlaveOnlyNodes(n);
      }

      ungroupedGroupNode = new TreeNode(UNGROUPED_GROUP_NAME) {
        ForeColor = SystemColors.GrayText,
        ImageIndex = slaveGroupImageIndex,
        Tag = new SlaveGroup() {
          Name = UNGROUPED_GROUP_NAME,
          Description = UNGROUPED_GROUP_DESCRIPTION
        }
      };

      foreach (var slave in subResources.OfType<Slave>().OrderBy(x => x.Name)) {
        var slaveNode = new TreeNode(slave.Name) { Tag = slave };
        StyleTreeNode(slaveNode, slave, resources);
        ungroupedGroupNode.Nodes.Add(slaveNode);
        if (selectedResource == null) {
          SelectedResource = slave;
        }

        if (slave.Id == selectedResource.Id && !nodeSelected) {
          slaveNode.BackColor = selectedBackColor;
          slaveNode.ForeColor = selectedForeColor;
          slaveNode.Text += SELECTED_TAG;
          nodeSelected = true;
        }
      }

      if (ungroupedGroupNode.Nodes.Count > 0) {
        ungroupedGroupNode.Text += " [" + ungroupedGroupNode.Nodes.Count.ToString() + "]";
        ungroupedGroupNode.Expand();
        treeView.Nodes.Add(ungroupedGroupNode);
      }
    }

    private void CollapseSlaveOnlyNodes(TreeNode tn) {
      Resource r = (Resource)tn.Tag;
      var descendants = GetResourceDescendants();
      if (descendants.ContainsKey(r.Id)) {
        if (descendants[r.Id].OfType<SlaveGroup>().Any()) {
          tn.Expand();
          foreach (TreeNode n in tn.Nodes) CollapseSlaveOnlyNodes(n);
        } else {
          tn.Collapse();
        }
      }
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

    private void UpdateChildHbIntervall(Resource resource) {
      foreach (Resource r in Content.Where(x => x.ParentResourceId == resource.Id)) {
        r.HbInterval = resource.HbInterval;
        if (r is SlaveGroup) {
          UpdateChildHbIntervall(r);
        }
      }
    }

    private void UpdateResources() {
      lock (locker) {
        if (refreshingInternal || refreshingExternal) return;
        refreshingInternal = true;
      }

      try {
        HiveAdminClient.Instance.Refresh();
      } catch (AnonymousUserException) {
        ShowHiveInformationDialog();
      } finally {
        refreshingInternal = false;
      }
    }

    private void RemoveResource(Resource resource) {
      if (resource == null) return;

      try {
        if (resource.Id != Guid.Empty) {
          SelectedResource = HiveAdminClient.Instance.GetAvailableResourceAncestors(resource.Id).LastOrDefault();

          // deal with all new, but not yet saved resources
          var newResources = Content.Where(x => x.ParentResourceId == resource.Id).ToList();
          if (newResources.Any(x => x.Id != Guid.Empty)) return;
          foreach (var nr in newResources) Content.Remove(nr);

          HiveAdminClient.Delete(resource);
          UpdateResources();
        } else {
          SelectedResource = Content.FirstOrDefault(x => x.Id == resource.ParentResourceId);
          Content.Remove(resource);
        }
      } catch (AnonymousUserException) {
        ShowHiveInformationDialog();
      }
    }

    private void RemoveResource(IEnumerable<Resource> resources) {
      if (resources == null || !resources.Any()) return;

      var ids = resources.Select(x => x.Id).ToList();
      try {
        bool update = false;
        foreach (var r in resources) {
          if (r.Id != Guid.Empty) {
            if (r.Id == SelectedResource.Id)
              SelectedResource = HiveAdminClient.Instance.GetAvailableResourceAncestors(r.Id).LastOrDefault();

            // deal with all new, but not yet saved resources
            var newResources = Content.Where(x => x.ParentResourceId == r.Id).ToList();
            if (newResources.Any(x => x.Id != Guid.Empty)) return;
            foreach (var nr in newResources) Content.Remove(nr);

            HiveAdminClient.Delete(r);
            update = true;
          } else {
            if (r.Id == SelectedResource.Id)
              SelectedResource = Content.FirstOrDefault(x => x.Id == r.ParentResourceId);
            Content.Remove(r);
          }
        }
        if (update) UpdateResources();
      } catch (AnonymousUserException) {
        ShowHiveInformationDialog();
      }
    }

    private void UpdateSchedule() {
      try {
        HiveAdminClient.Instance.RefreshCalendar();
      } catch (AnonymousUserException) {
        ShowHiveInformationDialog();
      }
    }

    private bool IsAdmin() {
      return HiveRoles.CheckAdminUserPermissions();
    }

    private void StyleTreeNode(TreeNode n, Resource r, IEnumerable<Resource> resources) {
      n.Text = r.Name;
      n.BackColor = Color.Transparent;
      n.ForeColor = Color.Black;

      if (HiveAdminClient.Instance.DisabledParentResources.Select(x => x.Id).Contains(r.Id)) {
        n.ForeColor = grayTextColor;
      } else if (r.Id == Guid.Empty && n != ungroupedGroupNode /*!r.Name.StartsWith(UNGROUPED_GROUP_NAME)*/) {
        // not stored (i.e. new)
        n.Text += NOT_STORED_TAG;
      } else if (r.Modified && n != ungroupedGroupNode /*!r.Name.StartsWith(UNGROUPED_GROUP_NAME)*/) {
        // changed
        n.Text += CHANGES_NOT_STORED_TAG;
      }

      // slave count
      int childSlavesCount = 0;
      if (r.Id != Guid.Empty && r is SlaveGroup) {
        var descendants = GetResourceDescendants();
        if (descendants.ContainsKey(r.Id)) {
          childSlavesCount = resources
            .OfType<Slave>()
            .Where(x => descendants[r.Id].Select(y => y.Id)
              .Contains(x.Id))
            .Count();
        }
      } else if (n == ungroupedGroupNode /*|| r.Name.StartsWith(UNGROUPED_GROUP_NAME)*/) {
        childSlavesCount = resources
          .OfType<Slave>()
          .Where(x => x.ParentResourceId == null
            || (x.ParentResourceId.HasValue && x.ParentResourceId.Value == Guid.Empty))
          .Count();
      }
      if (childSlavesCount > 0)
        n.Text += " [" + childSlavesCount.ToString() + "]";

      // slave image index, state, utilization
      if (r is Slave) {
        n.ImageIndex = slaveImageIndex;
        var s = r as Slave;
        if (s.SlaveState == SlaveState.Calculating) {
          n.ForeColor = calculatingColor;
          n.Text += " [" + s.CpuUtilization.ToString("N2") + "%]";
        } else if (s.SlaveState == SlaveState.Offline) {
          n.ForeColor = offlineColor;
          if (s.LastHeartbeat.HasValue)
            n.Text += " [" + (s.LastHeartbeat != null ? s.LastHeartbeat.Value.ToString("g") : null) + "]";
        }
      } else {
        n.ImageIndex = slaveGroupImageIndex;
      }

      // ungrouped
      if (n == ungroupedGroupNode /*r.Name.StartsWith(UNGROUPED_GROUP_NAME)*/) {
        n.ForeColor = SystemColors.GrayText;
      }
    }

    private void ResetTreeNodes(TreeNodeCollection nodes, IEnumerable<Resource> resources) {
      foreach (TreeNode n in nodes) {
        StyleTreeNode(n, (Resource)n.Tag, resources);
        if (n.Nodes.Count > 0) {
          ResetTreeNodes(n.Nodes, resources);
        }
      }
    }

    private async void ChangeSelectedResource(Resource resource) {
      selectedResource = resource;
      viewHost.Content = selectedResource;

      HiveAdminClient.Instance.DowntimeForResourceId = selectedResource != null ? selectedResource.Id : Guid.Empty;
      if (tabSlaveGroup.SelectedTab == tabSchedule) {
        await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
          action: () => UpdateSchedule(),
          finallyCallback: () => scheduleView.Content = HiveAdminClient.Instance.Downtimes);
      }

      SetEnabledStateOfControls();
    }

    private void ChangeSelectedResourceNode(TreeNode resourceNode) {
      if (resourceNode == null) return;
      SelectedResource = (Resource)resourceNode.Tag;
      ResetTreeNodes(treeView.Nodes, Content);
      resourceNode.BackColor = selectedBackColor;
      resourceNode.ForeColor = selectedForeColor;
      resourceNode.Text += SELECTED_TAG;
    }

    private void ShowHiveInformationDialog() {
      if (InvokeRequired) Invoke((Action)ShowHiveInformationDialog);
      else {
        using (HiveInformationDialog dialog = new HiveInformationDialog()) {
          dialog.ShowDialog(this);
        }
      }
    }

    private void ResetView() {
      if (InvokeRequired) Invoke((Action)ResetView);
      else {
        treeView.Nodes.Clear();

        if (viewHost.Content != null && viewHost.Content is SlaveGroup) {
          ((SlaveGroup)viewHost.Content).PropertyChanged -= SlaveViewContent_PropertyChanged;
        }

        viewHost.Content = null;
        if (scheduleView.Content != null) {
          scheduleView.Content.Clear();
        }

        HiveAdminClient.Instance.ResetDowntime();
      }
    }


    private Dictionary<Guid, HashSet<Resource>> GetResourceDescendants() {
      var resourceDescendants = new Dictionary<Guid, HashSet<Resource>>();
      //var resources = Content.Where(x => x.Id != Guid.Empty).Union(HiveAdminClient.Instance.DisabledParentResources).ToList();      
      var resources = Content.Union(HiveAdminClient.Instance.DisabledParentResources).ToList();

      foreach (var r in resources) {
        if (!resourceDescendants.ContainsKey(r.Id))
          resourceDescendants.Add(r.Id, new HashSet<Resource>());
      }
      foreach (var r in resources) {
        var parentResourceId = r.ParentResourceId;
        while (parentResourceId != null) {
          var parent = resources.SingleOrDefault(x => x.Id == parentResourceId);
          if (parent != null) {
            resourceDescendants[parent.Id].Add(r);
            parentResourceId = parent.ParentResourceId;
          } else {
            parentResourceId = null;
          }
        }
      }
      return resourceDescendants;
    }

    #endregion
  }
}
