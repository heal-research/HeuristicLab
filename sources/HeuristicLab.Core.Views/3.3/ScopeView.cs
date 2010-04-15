#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual represenation of <see cref="Scope"/>.
  /// </summary>
  [View("Scope View")]
  [Content(typeof(Scope), true)]
  [Content(typeof(IScope), false)]
  public sealed partial class ScopeView : ItemView {
    private Dictionary<IScope, TreeNode> scopeNodeTable;
    private Dictionary<ScopeList, IScope> subScopesScopeTable;

    /// <summary>
    /// Gets or sets the scope to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new IScope Content {
      get { return (IScope)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ScopeView"/> with caption "Scope" and 
    /// property <see cref="AutomaticUpdating"/> set to <c>false</c>.
    /// </summary>
    public ScopeView() {
      InitializeComponent();
      Caption = "Scope";
      scopeNodeTable = new Dictionary<IScope, TreeNode>();
      subScopesScopeTable = new Dictionary<ScopeList, IScope>();

    }
    /// <summary>
    /// Initializes a new instance of <see cref="ScopeView"/> with the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ScopeView()"/>.</remarks>
    /// <param name="scope">The scope to represent visually.</param>
    public ScopeView(IScope content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (scopesTreeView.Nodes.Count > 0) {
        RemoveTreeNode(scopesTreeView.Nodes[0]);
        scopesTreeView.Nodes.Clear();
      }
      variableCollectionView.Content = null;
      Caption = "Scope";
      if (Content != null) {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        scopesTreeView.Nodes.Add(CreateTreeNode(Content));
      }
      SetEnabledStateOfControls();
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }

    private void SetEnabledStateOfControls() {
      scopesTreeView.Enabled = Content != null;
      variableCollectionView.Enabled = Content != null;
      variableCollectionView.ReadOnly = ReadOnly;
    }

    private TreeNode CreateTreeNode(IScope scope) {
      TreeNode node = new TreeNode();
      node.Text = scope.Name;
      node.Tag = scope;

      scopeNodeTable.Add(scope, node);
      scope.NameChanged += new EventHandler(Scope_NameChanged);
      scope.SubScopes.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsAdded);
      scope.SubScopes.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsRemoved);
      scope.SubScopes.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsReplaced);
      scope.SubScopes.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsMoved);
      scope.SubScopes.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_CollectionReset);
      subScopesScopeTable.Add(scope.SubScopes, scope);
      if (scope.SubScopes.Count > 0)
        node.Nodes.Add(new TreeNode());
      return node;
    }

    private void RemoveTreeNode(TreeNode node) {
      foreach (TreeNode child in node.Nodes)
        RemoveTreeNode(child);

      IScope scope = node.Tag as IScope;
      if (scope != null) {
        scopeNodeTable.Remove(scope);
        scope.NameChanged -= new EventHandler(Scope_NameChanged);
        scope.SubScopes.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsAdded);
        scope.SubScopes.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsRemoved);
        scope.SubScopes.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsReplaced);
        scope.SubScopes.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsMoved);
        scope.SubScopes.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_CollectionReset);
        subScopesScopeTable.Remove(scope.SubScopes);
      }
    }

    #region TreeView Events
    private void scopesTreeView_MouseDown(object sender, MouseEventArgs e) {
      TreeNode node = scopesTreeView.GetNodeAt(e.X, e.Y);
      if ((node != null) && (node.Tag is IScope)) {
        variableCollectionView.Content = ((IScope)node.Tag).Variables;
        variableCollectionView.Enabled = true;
      } else {
        variableCollectionView.Content = null;
        variableCollectionView.Enabled = false;
        if (node == null) scopesTreeView.SelectedNode = null;
      }
    }
    private void scopesTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
      TreeNode node = e.Node;
      IScope scope = (IScope)node.Tag;

      node.Nodes.Clear();
      for (int i = 0; i < scope.SubScopes.Count; i++)
        node.Nodes.Add(CreateTreeNode(scope.SubScopes[i]));
    }
    private void scopesTreeView_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e) {
      TreeNode node = e.Node;
      IScope scope = (IScope)node.Tag;

      if (node.Nodes.Count > 0) {
        for (int i = 0; i < node.Nodes.Count; i++)
          RemoveTreeNode(node.Nodes[i]);
        node.Nodes.Clear();
        node.Nodes.Add(new TreeNode());
      }
    }
    private void scopesTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode node = (TreeNode)e.Item;
      IScope scope = node.Tag as IScope;
      if (scope != null) {
        DataObject data = new DataObject();
        data.SetData("Type", scope.GetType());
        data.SetData("Value", scope);
        DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
      }
    }
    #endregion

    #region Scope Events
    private void Scope_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Scope_NameChanged), sender, e);
      else {
        IScope scope = (IScope)sender;
        scopeNodeTable[scope].Text = scope.Name;
      }
    }
    #endregion

    #region SubScopes Events
    private void SubScopes_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsAdded), sender, e);
      else {
        IScope parentScope = subScopesScopeTable[(ScopeList)sender];
        TreeNode parentNode = scopeNodeTable[parentScope];
        if (parentNode.IsExpanded) {
          foreach (IndexedItem<IScope> item in e.Items) {
            TreeNode node = CreateTreeNode(item.Value);
            parentNode.Nodes.Insert(item.Index, node);
          }
        } else if (parentNode.Nodes.Count == 0) {
          parentNode.Nodes.Add(new TreeNode());
        }
      }
    }
    private void SubScopes_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsRemoved), sender, e);
      else {
        IScope parentScope = subScopesScopeTable[(ScopeList)sender];
        TreeNode parentNode = scopeNodeTable[parentScope];
        if (parentNode.IsExpanded) {
          foreach (IndexedItem<IScope> item in e.Items) {
            TreeNode node = scopeNodeTable[item.Value];
            RemoveTreeNode(node);
            node.Remove();
          }
        } else if (parentScope.SubScopes.Count == 0) {
          parentNode.Nodes.Clear();
        }
      }
    }
    private void SubScopes_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsReplaced), sender, e);
      else {
        IScope parentScope = subScopesScopeTable[(ScopeList)sender];
        TreeNode parentNode = scopeNodeTable[parentScope];
        if (parentNode.IsExpanded) {
          foreach (IndexedItem<IScope> item in e.Items) {
            TreeNode node = parentNode.Nodes[item.Index];
            RemoveTreeNode(node);
            node.Remove();
            node = CreateTreeNode(item.Value);
            parentNode.Nodes.Insert(item.Index, node);
          }
        }
      }
    }
    private void SubScopes_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsMoved), sender, e);
      else {
        IScope parentScope = subScopesScopeTable[(ScopeList)sender];
        TreeNode parentNode = scopeNodeTable[parentScope];
        if (parentNode.IsExpanded) {
          parentNode.Nodes.Clear();
          foreach (IndexedItem<IScope> item in e.Items)
            parentNode.Nodes.Insert(item.Index, scopeNodeTable[item.Value]);
        }
      }
    }
    private void SubScopes_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_CollectionReset), sender, e);
      else {
        IScope parentScope = subScopesScopeTable[(ScopeList)sender];
        TreeNode parentNode = scopeNodeTable[parentScope];
        if (parentNode.IsExpanded) {
          foreach (TreeNode node in parentNode.Nodes)
            RemoveTreeNode(node);
          parentNode.Nodes.Clear();
          foreach (IndexedItem<IScope> item in e.Items) {
            TreeNode node = CreateTreeNode(item.Value);
            parentNode.Nodes.Insert(item.Index, node);
          }
        } else {
          parentNode.Nodes.Clear();
          if (parentScope.SubScopes.Count > 0)
            parentNode.Nodes.Add(new TreeNode());
        }
      }
    }
    #endregion
  }
}
