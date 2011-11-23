#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.Views {
  [View("ItemTree View")]
  [Content(typeof(ItemCollection<>), IsDefaultView = false)]
  public partial class ItemTreeView<T> : ItemView where T : class, IItemTree<T> {
    public new ItemCollection<T> Content {
      get { return (ItemCollection<T>)base.Content; }
      set { base.Content = value; }
    }

    public T SelectedItem {
      get {
        if (treeView.SelectedNode != null)
          return (T)treeView.SelectedNode.Tag;
        return null;
      }
    }

    public ItemTreeView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= new CollectionItemsChangedEventHandler<T>(Content_ItemsAdded);
      Content.ItemsRemoved -= new CollectionItemsChangedEventHandler<T>(Content_ItemsRemoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<T>(Content_CollectionReset);
      foreach (TreeNode node in treeView.Nodes) {
        DeregisterContentEvents((T)node.Tag, node);
      }
      base.DeregisterContentEvents();
    }

    protected virtual void DeregisterContentEvents(T item, TreeNode node) {
      item.ItemsAdded -= new CollectionItemsChangedEventHandler<IItemTree<T>>(item_ItemsAdded);
      item.ItemsRemoved -= new CollectionItemsChangedEventHandler<IItemTree<T>>(item_ItemsRemoved);
      item.CollectionReset -= new CollectionItemsChangedEventHandler<IItemTree<T>>(item_CollectionReset);
      item.ToStringChanged -= new EventHandler(item_ToStringChanged);
      item.ItemImageChanged -= new EventHandler(item_ItemImageChanged);
      foreach (TreeNode childNode in node.Nodes) {
        DeregisterContentEvents((T)childNode.Tag, childNode);
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new CollectionItemsChangedEventHandler<T>(Content_ItemsAdded);
      Content.ItemsRemoved += new CollectionItemsChangedEventHandler<T>(Content_ItemsRemoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<T>(Content_CollectionReset);
      foreach (TreeNode node in treeView.Nodes) {
        RegisterContentEvents((T)node.Tag, node);
      }
    }

    protected virtual void RegisterContentEvents(T item, TreeNode node) {
      item.ItemsAdded += new CollectionItemsChangedEventHandler<IItemTree<T>>(item_ItemsAdded);
      item.ItemsRemoved += new CollectionItemsChangedEventHandler<IItemTree<T>>(item_ItemsRemoved);
      item.CollectionReset += new CollectionItemsChangedEventHandler<IItemTree<T>>(item_CollectionReset);
      item.ToStringChanged += new EventHandler(item_ToStringChanged);
      item.ItemImageChanged += new EventHandler(item_ItemImageChanged);
      foreach (TreeNode childNode in node.Nodes) {
        RegisterContentEvents((T)childNode.Tag, childNode);
      }
    }

    #region Event Handlers (Content)
    protected virtual void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_ItemsAdded), sender, e);
      } else {
        foreach (T item in e.Items) {
          var node = AddChildNodes(item, treeView.Nodes, true);
          UpdateNodeItemImage(node);
        }
      }
    }
    protected virtual void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_ItemsRemoved), sender, e);
      } else {
        foreach (T item in e.Items) {
          RemoveChildNodes(item, treeView.Nodes);
        }
        RebuildImageList();
      }
    }
    protected virtual void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_CollectionReset), sender, e);
      } else {
        foreach (T item in e.OldItems) {
          RemoveChildNodes(item, treeView.Nodes);
        }
        RebuildImageList();
        foreach (T item in e.Items) {
          AddChildNodes(item, treeView.Nodes, true);
        }
      }
    }

    protected virtual void item_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IItemTree<T>> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IItemTree<T>>(item_ItemsAdded), sender, e);
      } else {
        TreeNode node = GetNodeByItem(sender as T);
        if (node != null) {
          foreach (T item in e.Items) {
            AddChildNodes(item, node.Nodes, true);
            node.ExpandAll();
            UpdateNodeItemImage(node);
          }
        }
      }
    }

    protected virtual void item_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IItemTree<T>> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IItemTree<T>>(item_ItemsRemoved), sender, e);
      } else {
        TreeNode node = GetNodeByItem(sender as T);
        if (node != null) {
          foreach (T item in e.Items) {
            RemoveChildNodes(item, node.Nodes);
          }
          RebuildImageList();
        }
      }
    }

    protected virtual void item_CollectionReset(object sender, CollectionItemsChangedEventArgs<IItemTree<T>> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IItemTree<T>>(item_CollectionReset), sender, e);
      } else {
        TreeNode node = GetNodeByItem(sender as T);
        if (node != null) {
          foreach (T item in e.OldItems) {
            RemoveChildNodes(item, node.Nodes);
          }
          RebuildImageList();
          foreach (T item in e.Items) {
            AddChildNodes(item, node.Nodes, true);
          }
        }
      }
    }

    protected virtual void item_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(item_ToStringChanged), sender, e);
      } else {
        var item = sender as T;
        TreeNode node = GetNodeByItem(item);
        if (node != null && node.Text != item.ToString())
          node.Text = item.ToString();
      }
    }

    protected virtual void item_ItemImageChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(item_ItemImageChanged), sender, e);
      } else {
        var item = sender as T;
        TreeNode node = GetNodeByItem(item);
        if (node != null)
          UpdateNodeItemImage(node);
      }
    }

    protected virtual void treeView_DoubleClick(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(treeView_DoubleClick), sender, e);
      } else {
        if (treeView.SelectedNode != null) {
          var item = treeView.SelectedNode.Tag as T;
          if (item != null) {
            IContentView view = MainFormManager.MainForm.ShowContent(item);
            if (view != null) {
              view.ReadOnly = ReadOnly;
              view.Locked = Locked;
            }
          }
        }
      }
    }

    protected virtual void treeView_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button == System.Windows.Forms.MouseButtons.Left) {
        // enables deselection of treeNodes
        if (treeView.SelectedNode == null) {
          return;
        }
        detailsViewHost.Content = null;
        treeView.SelectedNode = null;
        SetEnabledStateOfControls();
      }
    }
    private void treeView_MouseClick(object sender, MouseEventArgs e) {

    }
    private void treeView_Click(object sender, EventArgs e) {

    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        ClearNodes();
        detailsViewHost.Content = null;
      } else {
        ClearNodes();
        foreach (T itemTree in Content) {
          AddChildNodes(itemTree, treeView.Nodes, false);
        }
        foreach (TreeNode node in treeView.Nodes) {
          UpdateNodeItemImage(node);
        }
        detailsViewHost.Content = null;
      }
    }

    protected virtual void ClearNodes() {
      treeView.Nodes.Clear();
    }

    protected virtual TreeNode AddChildNodes(T item, TreeNodeCollection nodes, bool registerEvents) {
      TreeNode node = CreateTreeViewNode(item);
      nodes.Add(node);
      RebuildImageList(node);
      RegisterContentEvents(item, node);
      var childItems = item.GetChildItems();
      foreach (T childItem in childItems) {
        AddChildNodes(childItem, node.Nodes, registerEvents);
      }
      node.ExpandAll();
      return node;
    }

    protected virtual void RemoveChildNodes(T item, TreeNodeCollection treeNodeCollection) {
      int idx = -1;
      for (int i = 0; i < treeNodeCollection.Count; i++) {
        if (treeNodeCollection[i].Tag == item) {
          idx = i; break;
        }
      }
      if (idx > -1) {
        DeregisterContentEvents(item, treeNodeCollection[idx]);
        treeNodeCollection.RemoveAt(idx);
      }
    }

    protected virtual TreeNode GetNodeByItem(T item) {
      TreeNode found = null;
      foreach (TreeNode node in treeView.Nodes) {
        found = GetNodeByItem(node, item);
        if (found != null)
          return found;
      }
      return null;
    }

    protected virtual TreeNode GetNodeByItem(TreeNode node, T item) {
      if (node.Tag == item)
        return node;
      TreeNode found = null;
      foreach (TreeNode childNode in node.Nodes) {
        found = GetNodeByItem(childNode, item);
        if (found != null)
          return found;
      }
      return null;
    }

    protected virtual TreeNode CreateTreeViewNode(T item) {
      var node = new TreeNode(item.ToString());
      node.Tag = item;
      return node;
    }

    protected virtual void UpdateNodeItemImage(TreeNode node) {
      if (node == null) throw new ArgumentNullException();
      var item = node.Tag as T;
      int i = node.ImageIndex;
      this.imageList.Images[i] = item == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : item.ItemImage;
      node.ImageIndex = -1;
      node.ImageIndex = i;
      node.SelectedImageIndex = node.ImageIndex;
      foreach (TreeNode childNode in node.Nodes) {
        UpdateNodeItemImage(childNode);
      }
    }

    protected virtual T GetParentItem(T selectedItem) {
      var node = GetNodeByItem(selectedItem);
      if (node == null || node.Parent == null)
        return null;
      else
        return (T)node.Parent.Tag;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      addButton.Enabled = !this.ReadOnly;
      removeButton.Enabled = !this.ReadOnly && treeView.SelectedNode != null;
    }

    #region Event Handlers (child controls)
    // Put event handlers of child controls here.
    protected virtual void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
      detailsViewHost.Content = (IContent)treeView.SelectedNode.Tag;
      SetEnabledStateOfControls();
    }

    protected virtual void contextMenuStrip_Opening(object sender, CancelEventArgs e) {
      if (treeView.SelectedNode != null) {
        T selectedItem = treeView.SelectedNode.Tag as T;
        T parentItem = GetParentItem(selectedItem);
        var actions = GetTreeNodeItemActions(selectedItem);
        contextMenuStrip.Items.Clear();
        foreach (var action in actions) {
          contextMenuStrip.Items.Add(new DelegateMenuItem<T>(action.Name, action.Image, new Action<T, T>(action.Execute), selectedItem, parentItem));
        }
      }
    }

    protected virtual void addButton_Click(object sender, EventArgs e) {
      T item = CreateItem<T>();
      if (item != null)
        Content.Add(item);
    }

    protected virtual void removeButton_Click(object sender, EventArgs e) {
      if (treeView.SelectedNode != null) {

      }
    }

    protected virtual void showDetailsCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (showDetailsCheckBox.Checked) {
        splitContainer.Panel2Collapsed = false;
        detailsGroupBox.Enabled = treeView.SelectedNode != null;
        detailsViewHost.Content = treeView.SelectedNode != null ? (T)treeView.SelectedNode.Tag : null;
      } else {
        splitContainer.Panel2Collapsed = true;
        detailsViewHost.Content = null;
      }
    }
    #endregion

    protected virtual void RebuildImageList() {
      treeView.ImageList.Images.Clear();
      foreach (TreeNode node in treeView.Nodes) {
        RebuildImageList(node);
      }
    }

    protected virtual void RebuildImageList(TreeNode node) {
      var item = node.Tag as T;
      treeView.ImageList.Images.Add(item == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : item.ItemImage);
      node.ImageIndex = treeView.ImageList.Images.Count - 1;
      node.SelectedImageIndex = node.ImageIndex;
      foreach (TreeNode childNode in node.Nodes) {
        RebuildImageList(childNode);
      }
    }

    protected virtual ICollection<IItemTreeNodeAction<T>> GetTreeNodeItemActions(T selectedItem) {
      return selectedItem.Actions;
    }

    protected virtual X CreateItem<X>() where X : class {
      var typeSelectorDialog = new TypeSelectorDialog();
      typeSelectorDialog.Caption = "Select Item";
      typeSelectorDialog.TypeSelector.Caption = "Available Items";
      typeSelectorDialog.TypeSelector.Configure(typeof(X), false, true);

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          return (X)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
      return null;
    }
  }

  public class DelegateMenuItem<T> : ToolStripMenuItem {
    private Action<T, T> actionDelegate;
    private T node;
    private T parentNode;

    public DelegateMenuItem(string name, Image image, Action<T, T> actionDelegate, T node, T parentNode) {
      this.Name = name;
      this.Text = name;
      this.actionDelegate = actionDelegate;
      this.node = node;
      this.parentNode = parentNode;
      this.Image = image;
    }

    protected override void OnClick(EventArgs e) {
      base.OnClick(e);
      actionDelegate.Invoke(node, parentNode);
    }
  }
}
