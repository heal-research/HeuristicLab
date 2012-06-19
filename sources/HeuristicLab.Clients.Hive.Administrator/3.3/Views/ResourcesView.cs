#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Clients.Access;
using HeuristicLab.Clients.Hive.Views;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using TS = System.Threading.Tasks;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("Resources View")]
  [Content(typeof(IItemList<Resource>), false)]
  public partial class ResourcesView : ItemView, IDisposable {
    public new IItemList<Resource> Content {
      get { return (IItemList<Resource>)base.Content; }
      set { base.Content = value; }
    }

    public const string UngroupedGroupName = "UNGROUPED";
    private const int slaveImageIndex = 0;
    private const int slaveGroupImageIndex = 1;
    private readonly Color ownedResourceColor = Color.LightGreen;
    private TS.Task progressTask;
    private bool stopProgressTask;
    private bool currentlyAuthorized;


    public ResourcesView() {
      InitializeComponent();
      treeSlaveGroup.ImageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.MonitorLarge);
      treeSlaveGroup.ImageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.NetworkCenterLarge);

      HiveAdminClient.Instance.Refreshing += new EventHandler(Instance_Refreshing);
      HiveAdminClient.Instance.Refreshed += new EventHandler(Instance_Refreshed);

      Access.AccessClient.Instance.Refreshing += new EventHandler(Content_Refreshing);
      Access.AccessClient.Instance.Refreshed += new EventHandler(Content_Refreshed);
    }

    public new void Dispose() {
      HiveAdminClient.Instance.Refreshing -= new EventHandler(Instance_Refreshing);
      HiveAdminClient.Instance.Refreshed -= new EventHandler(Instance_Refreshed);

      Access.AccessClient.Instance.Refreshing -= new EventHandler(Content_Refreshing);
      Access.AccessClient.Instance.Refreshed -= new EventHandler(Content_Refreshed);
    }

    private void UpdateProgress() {
      while (!stopProgressTask) {
        int diff = (progressBar.Maximum - progressBar.Minimum) / 10;

        if (progressBar.InvokeRequired) {
          progressBar.Invoke(new Action(delegate() { progressBar.Value = (progressBar.Value + diff) % progressBar.Maximum; }));
        } else {
          progressBar.Value = (progressBar.Value + diff) % progressBar.Maximum;
        }

        //ok, this is not very clever...
        Thread.Sleep(500);
      }
      if (progressBar.InvokeRequired) {
        progressBar.Invoke(new Action(delegate() { progressBar.Value = progressBar.Minimum; }));
      } else {
        progressBar.Value = progressBar.Minimum;
      }
    }

    void Instance_Refreshing(object sender, EventArgs e) {
      stopProgressTask = false;
      progressTask = new TS.Task(UpdateProgress);
      progressTask.Start();
      SetEnabledStateOfControls(false);
    }

    void Instance_Refreshed(object sender, EventArgs e) {
      stopProgressTask = true;
      SetEnabledStateOfControls(true);
    }

    void Content_Refreshing(object sender, EventArgs e) {
      stopProgressTask = false;
      progressTask = new TS.Task(UpdateProgress);
      progressTask.Start();
      SetEnabledStateOfControls(false);
    }

    void Content_Refreshed(object sender, EventArgs e) {
      stopProgressTask = true;
      SetEnabledStateOfControls(true);
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<Resource>>(Content_ItemsAdded);
      Content.ItemsRemoved -= new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<Resource>>(Content_ItemsRemoved);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<Resource>>(Content_ItemsAdded);
      Content.ItemsRemoved += new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<Resource>>(Content_ItemsRemoved);
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        slaveView.Content = null;
        scheduleView.Content = null;
        permissionView.Content = null;
        permissionView.FetchSelectedUsers = null;
        treeSlaveGroup.Nodes.Clear();
      } else {
        permissionView.Content = Access.AccessClient.Instance;
        treeSlaveGroup.Nodes.Clear();

        //rebuild
        TreeNode ungrp = new TreeNode(UngroupedGroupName);
        ungrp.ImageIndex = slaveGroupImageIndex;
        ungrp.SelectedImageIndex = ungrp.ImageIndex;
        var newGroup = new SlaveGroup();
        newGroup.Name = UngroupedGroupName;
        newGroup.Id = Guid.NewGuid();
        newGroup.Description = "Contains slaves which are in no group";
        ungrp.Tag = newGroup;

        foreach (Resource g in Content) {
          if (g.GetType() == typeof(SlaveGroup)) {
            //root node
            if (g.ParentResourceId == null) {
              TreeNode tn = new TreeNode();
              tn.ImageIndex = slaveGroupImageIndex;
              tn.SelectedImageIndex = tn.ImageIndex;

              tn.Tag = g;
              tn.Text = g.Name;
              if (g.OwnerUserId == Access.UserInformation.Instance.User.Id) tn.BackColor = ownedResourceColor;

              BuildSlaveGroupTree(g, tn);
              treeSlaveGroup.Nodes.Add(tn);
            }
          } else if (g.GetType() == typeof(Slave)) {
            if (g.ParentResourceId == null) {
              var stn = new TreeNode(g.Name);
              stn.ImageIndex = slaveImageIndex;
              stn.SelectedImageIndex = stn.ImageIndex;
              stn.Tag = g;
              if (g.OwnerUserId == Access.UserInformation.Instance.User.Id) stn.BackColor = ownedResourceColor;
              ungrp.Nodes.Add(stn);
            }
          }
        }
        treeSlaveGroup.Nodes.Add(ungrp);
      }
    }

    private void BuildSlaveGroupTree(Resource g, TreeNode tn) {
      foreach (Resource r in Content.Where(s => s.ParentResourceId != null && s.ParentResourceId == g.Id)) {
        TreeNode stn = new TreeNode(r.Name);
        if (r is Slave) {
          stn.ImageIndex = slaveImageIndex;
        } else if (r is SlaveGroup) {
          stn.ImageIndex = slaveGroupImageIndex;
        }
        stn.SelectedImageIndex = stn.ImageIndex;
        stn.Tag = r;
        if (r.OwnerUserId == Access.UserInformation.Instance.User.Id) stn.BackColor = ownedResourceColor;
        tn.Nodes.Add(stn);

        BuildSlaveGroupTree(r, stn);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (Content == null) {
        btnAddGroup.Enabled = false;
        btnRemoveGroup.Enabled = false;
        btnSave.Enabled = false;
        btnPermissionsSave.Enabled = false;
        permissionView.Enabled = false;
      } else {
        btnAddGroup.Enabled = true;
        btnRemoveGroup.Enabled = true;
        btnSave.Enabled = true;
      }
    }

    public virtual void SetEnabledStateOfControls(bool state) {
      if (InvokeRequired) {
        Invoke(new Action(() => SetEnabledStateOfControls(state)));
      } else {
        if (Content == null) state = false;
        btnAddGroup.Enabled = state;
        btnRemoveGroup.Enabled = state;
        btnSave.Enabled = state;
        scheduleView.SetEnabledStateOfControls(state && IsAuthorized(slaveView.Content));
        btnPermissionsSave.Enabled = state && permissionView.FetchSelectedUsers != null && Content != null;
        permissionView.Enabled = state && permissionView.FetchSelectedUsers != null && Content != null;
      }
    }

    private bool IsAuthorized(Resource resource) {
      return resource != null
          && resource.Name != UngroupedGroupName
          && resource.Id != Guid.Empty
          && HiveServiceLocator.Instance.CallHiveService<bool>(service => { return service.AuthorizesForResourceAdministration(resource.Id); });
    }

    private void treeSlaveGroup_AfterSelect(object sender, TreeViewEventArgs e) {
      if (e.Action != TreeViewAction.Unknown) {
        Resource selectedResource = ((Resource)e.Node.Tag);
        currentlyAuthorized = IsAuthorized(selectedResource);
        if (currentlyAuthorized) {
          permissionView.FetchSelectedUsers = new Func<List<Guid>>(() => {
            return HiveServiceLocator.Instance.CallHiveService<List<ResourcePermission>>(service => {
              return service.GetResourcePermissions(selectedResource.Id);
            }).Select(x => x.GrantedUserId).ToList();
          });
          if (!tabSlaveGroup.TabPages.Contains(tabPermissions)) tabSlaveGroup.TabPages.Add(tabPermissions);
        } else {
          permissionView.FetchSelectedUsers = null;
          btnPermissionsSave.Enabled = false;
          if (selectedResource.Id == Guid.Empty) {
            if (!tabSlaveGroup.TabPages.Contains(tabPermissions)) tabSlaveGroup.TabPages.Add(tabPermissions);
          } else tabSlaveGroup.TabPages.Remove(tabPermissions);
        }

        if (slaveView.Content != null && slaveView.Content is SlaveGroup) {
          slaveView.Content.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(SlaveViewContent_PropertyChanged);
        }

        slaveView.Content = selectedResource;
        HiveAdminClient.Instance.DowntimeForResourceId = selectedResource.Id;

        if (selectedResource is SlaveGroup) {
          slaveView.Content.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SlaveViewContent_PropertyChanged);
        }

        if (tabSlaveGroup.SelectedIndex == 1) {
          UpdateScheduleAsync();
        } else if (tabSlaveGroup.SelectedIndex == 2) {
          UpdatePermissionsAsync();
        }
      }
    }

    void SlaveViewContent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      OnContentChanged();
      if (e.PropertyName == "HbInterval") {
        UpdateChildHbIntervall(slaveView.Content);
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

    private void btnAddGroup_Click(object sender, EventArgs e) {
      SlaveGroup newGroup = new SlaveGroup();
      newGroup.Name = "New Group";
      newGroup.OwnerUserId = UserInformation.Instance.User.Id;
      Content.Add(newGroup);
    }

    void Content_ItemsRemoved(object sender, Collections.CollectionItemsChangedEventArgs<Collections.IndexedItem<Resource>> e) {
      OnContentChanged();
    }

    void Content_ItemsAdded(object sender, Collections.CollectionItemsChangedEventArgs<Collections.IndexedItem<Resource>> e) {
      OnContentChanged();
    }

    private void btnRemoveGroup_Click(object sender, EventArgs e) {
      if (treeSlaveGroup.SelectedNode != null && treeSlaveGroup.SelectedNode.Tag != null) {
        Resource res = (Resource)treeSlaveGroup.SelectedNode.Tag;

        DialogResult diagRes = MessageBox.Show("Do you really want to delete " + res.Name + "?", "HeuristicLab Hive Administrator", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (diagRes == DialogResult.Yes) {
          if (res is Slave) {
            Content.Remove(res);
            HiveAdminClient.Delete(res);
          } else if (res is SlaveGroup) {
            //only delete empty groups
            if (Content.Where(s => s.ParentResourceId == res.Id).Count() < 1) {
              Content.Remove(res);
              HiveAdminClient.Delete(res);
            } else {
              MessageBox.Show("Only empty groups can be deleted.", "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          }
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e) {
      foreach (Resource res in Content) {
        if (res is SlaveGroup && res.Id == Guid.Empty) {
          SlaveGroup slaveGroup = (SlaveGroup)res;
          slaveGroup.Store();
        } else if (res.Id != Guid.Empty && res.Modified) {
          res.Store();
        }
      }
    }

    private void treeSlaveGroup_DragDrop(object sender, DragEventArgs e) {
      if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false)) {
        Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
        TreeNode destNode = ((TreeView)sender).GetNodeAt(pt);
        TreeNode newNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

        if (destNode.TreeView == newNode.TreeView) {
          if (destNode.Text == UngroupedGroupName || (destNode.Parent != null && destNode.Parent.Text == UngroupedGroupName)) {
            MessageBox.Show(string.Format("You can't drag items to the group \"{0}\".{1}This group only contains slaves which haven't yet been assigned to a real group.",
              UngroupedGroupName, Environment.NewLine), "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
          }

          SlaveGroup sgrp = null;
          TreeNode parentNode = null;
          if (destNode.Tag != null && destNode.Tag is SlaveGroup) {
            sgrp = (SlaveGroup)destNode.Tag;
            parentNode = destNode;
          } else if (destNode.Parent != null && destNode.Parent.Tag is SlaveGroup) {
            sgrp = (SlaveGroup)destNode.Parent.Tag;
            parentNode = destNode.Parent;
          }

          if (newNode.Tag is SlaveGroup && CheckParentsEqualsMovedNode(parentNode, newNode)) {
            return;
          }

          SlaveGroup parent = (SlaveGroup)parentNode.Tag;

          if (parent.OwnerUserId != null && !IsAuthorized(parent)) {
            MessageBox.Show(string.Format("You don't have the permissions to drag items to the group \"{0}\".", ((Resource)parentNode.Tag).Name),
              "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }

          if (sgrp != null && newNode.Tag != null) {
            //save parent group to get an id
            if (sgrp.Id == Guid.Empty) {
              sgrp.Store();
            }

            if (newNode.Tag is Slave) {
              Slave slave = (Slave)newNode.Tag;
              if (slave.ParentResourceId == null || (slave.ParentResourceId != null && slave.ParentResourceId != sgrp.Id)) {
                slave.ParentResourceId = sgrp.Id;
                newNode.Remove();
                parentNode.Nodes.Clear();
                BuildSlaveGroupTree(sgrp, parentNode);
              }
            } else if (newNode.Tag is SlaveGroup) {
              SlaveGroup slaveGroup = (SlaveGroup)newNode.Tag;
              if (slaveGroup.ParentResourceId == null || (slaveGroup.ParentResourceId != null && slaveGroup.ParentResourceId != sgrp.Id)) {
                slaveGroup.ParentResourceId = sgrp.Id;
                newNode.Remove();
                parentNode.Nodes.Clear();
                BuildSlaveGroupTree(sgrp, parentNode);
              }
            }
          }
        }
      }
    }

    private bool CheckParentsEqualsMovedNode(TreeNode dest, TreeNode movedNode) {
      TreeNode tmp = dest;

      while (tmp != null) {
        if (tmp == movedNode) {
          return true;
        }
        tmp = tmp.Parent;
      }
      return false;
    }

    private void treeSlaveGroup_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode sourceNode = (TreeNode)e.Item;
      if (IsAuthorized((Resource)sourceNode.Tag))
        DoDragDrop(sourceNode, DragDropEffects.All);
    }

    private void treeSlaveGroup_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.Move;
    }

    private void treeSlaveGroup_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.Move;
    }

    private void treeSlaveGroup_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
      e.Action = DragAction.Continue;
    }

    void ResetView() {
      if (this.InvokeRequired) {
        Invoke(new Action(ResetView));
      } else {
        treeSlaveGroup.Nodes.Clear();

        if (slaveView.Content != null && slaveView.Content is SlaveGroup) {
          slaveView.Content.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(SlaveViewContent_PropertyChanged);
        }
        slaveView.Content = null;
        if (scheduleView.Content != null) {
          scheduleView.Content.Clear();
        }
        HiveAdminClient.Instance.ResetDowntime();
      }
    }

    private void UpdateResources() {
      ResetView();

      try {
        HiveAdminClient.Instance.Refresh();
        Content = HiveAdminClient.Instance.Resources;
      }
      catch (MessageSecurityException) {
        ShowMessageSecurityException();
      }
      catch (AnonymousUserException) {
        ShowHiveInformationDialog();
      }
    }

    private void ShowMessageSecurityException() {
      if (this.InvokeRequired) {
        Invoke(new Action(ShowMessageSecurityException));
      } else {
        MessageBox.Show("A Message Security error has occured. This normally means that your user name or password is wrong.", "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void ShowHiveInformationDialog() {
      if (this.InvokeRequired) {
        Invoke(new Action(ShowHiveInformationDialog));
      } else {
        using (HiveInformationDialog dialog = new HiveInformationDialog()) {
          dialog.ShowDialog(this);
        }
      }
    }

    private void UpdateResourcesAsync() {
      TS.Task.Factory.StartNew(UpdateResources).ContinueWith((t) => {
        DisplayError(t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void UpdateSchedule() {
      HiveAdminClient.Instance.RefreshCalendar();
      scheduleView.Invoke(new Action(() => {
        scheduleView.Content = HiveAdminClient.Instance.Downtimes;
        SetEnabledStateOfControls(currentlyAuthorized);
      }));
    }

    private void UpdateScheduleAsync() {
      TS.Task.Factory.StartNew(UpdateSchedule).ContinueWith((t) => {
        DisplayError(t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void UpdatePermissions() {
      if (permissionView.Content != null && permissionView.FetchSelectedUsers != null)
        permissionView.Invoke(new Action(() => permissionView.ManualRefresh()));
    }

    private void UpdatePermissionsAsync() {
      TS.Task.Factory.StartNew(UpdatePermissions).ContinueWith((t) => {
        DisplayError(t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }


    private void DisplayError(Exception ex) {
      MessageBox.Show(string.Format("An error occured while updating: {0} {1}", Environment.NewLine, ex.Message), "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void tabSlaveGroup_SelectedIndexChanged(object sender, EventArgs e) {
      if (tabSlaveGroup.SelectedIndex == 1) {
        UpdateScheduleAsync();
      } else if (tabSlaveGroup.SelectedIndex == 2) {
        UpdatePermissionsAsync();
      }
    }

    private void btnRefresh_Click(object sender, EventArgs e) {
      UpdateResourcesAsync();
    }

    private void ResourcesView_Load(object sender, EventArgs e) {
      UpdateResourcesAsync();
    }

    private void btnPermissionsSave_Click(object sender, EventArgs e) {
      SetEnabledStateOfControls(false);
      HiveServiceLocator.Instance.CallHiveService(service => {
        service.GrantResourcePermissions(((Resource)treeSlaveGroup.SelectedNode.Tag).Id, permissionView.GetAddedUsers().Select(x => x.Id).ToList());
        service.RevokeResourcePermissions(((Resource)treeSlaveGroup.SelectedNode.Tag).Id, permissionView.GetDeletedUsers().Select(x => x.Id).ToList());
      });
      SetEnabledStateOfControls(true);
    }
  }
}
