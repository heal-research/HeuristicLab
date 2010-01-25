#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Collections;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual represenation of <see cref="Scope"/>.
  /// </summary>
  [Content(typeof(Scope), true)]
  public sealed partial class ScopeView : ItemView {
    private Dictionary<Scope, TreeNode> scopeNodeTable;
    private Dictionary<ScopeList, Scope> subScopesScopeTable;

    /// <summary>
    /// Gets or sets the scope to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public Scope Scope {
      get { return (Scope)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ScopeView"/> with caption "Scope" and 
    /// property <see cref="AutomaticUpdating"/> set to <c>false</c>.
    /// </summary>
    public ScopeView() {
      InitializeComponent();
      Caption = "Scope";
      scopeNodeTable = new Dictionary<Scope, TreeNode>();
      subScopesScopeTable = new Dictionary<ScopeList, Scope>();

    }
    /// <summary>
    /// Initializes a new instance of <see cref="ScopeView"/> with the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ScopeView()"/>.</remarks>
    /// <param name="scope">The scope to represent visually.</param>
    public ScopeView(Scope scope)
      : this() {
      Scope = scope;
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (scopesTreeView.Nodes.Count > 0) {
        RemoveTreeNode(scopesTreeView.Nodes[0]);
        scopesTreeView.Nodes.Clear();
      }
      variableCollectionView.NamedItemCollection = null;
      variableCollectionView.Enabled = false;
      if (Scope == null) {
        Caption = "Scope";
        scopesTreeView.Enabled = false;
      } else {
        Caption = Scope.Name + " (" + Scope.GetType().Name + ")";
        scopesTreeView.Enabled = true;
        scopesTreeView.Nodes.Add(CreateTreeNode(Scope));
      }
    }

    private TreeNode CreateTreeNode(Scope scope) {
      TreeNode node = new TreeNode();
      node.Text = scope.Name;
      node.Tag = scope;

      scopeNodeTable.Add(scope, node);
      scope.NameChanged += new EventHandler(Scope_NameChanged);
      scope.SubScopes.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsAdded);
      scope.SubScopes.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsRemoved);
      scope.SubScopes.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsReplaced);
      scope.SubScopes.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsMoved);
      scope.SubScopes.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_CollectionReset);
      subScopesScopeTable.Add(scope.SubScopes, scope);
      if (scope.SubScopes.Count > 0)
        node.Nodes.Add(new TreeNode());
      return node;
    }

    private void RemoveTreeNode(TreeNode node) {
      foreach (TreeNode child in node.Nodes)
        RemoveTreeNode(child);

      Scope scope = node.Tag as Scope;
      if (scope != null) {
        scopeNodeTable.Remove(scope);
        scope.NameChanged -= new EventHandler(Scope_NameChanged);
        scope.SubScopes.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsAdded);
        scope.SubScopes.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsRemoved);
        scope.SubScopes.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsReplaced);
        scope.SubScopes.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsMoved);
        scope.SubScopes.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_CollectionReset);
        subScopesScopeTable.Remove(scope.SubScopes);
      }
    }

    #region TreeView Events
    private void scopesTreeView_MouseDown(object sender, MouseEventArgs e) {
      TreeNode node = scopesTreeView.GetNodeAt(e.X, e.Y);
      if ((node != null) && (node.Tag is Scope)) {
        variableCollectionView.NamedItemCollection = ((Scope)node.Tag).Variables;
        variableCollectionView.Enabled = true;
      } else {
        variableCollectionView.NamedItemCollection = null;
        variableCollectionView.Enabled = false;
        if (node == null) scopesTreeView.SelectedNode = null;
      }
    }
    private void scopesTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
      TreeNode node = e.Node;
      Scope scope = (Scope)node.Tag;

      node.Nodes.Clear();
      for (int i = 0; i < scope.SubScopes.Count; i++)
        node.Nodes.Add(CreateTreeNode(scope.SubScopes[i]));
    }
    private void scopesTreeView_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e) {
      TreeNode node = e.Node;
      Scope scope = (Scope)node.Tag;

      if (node.Nodes.Count > 0) {
        for (int i = 0; i < node.Nodes.Count; i++)
          RemoveTreeNode(node.Nodes[i]);
        node.Nodes.Clear();
        node.Nodes.Add(new TreeNode());
      }
    }
    private void scopesTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode node = (TreeNode)e.Item;
      Scope scope = node.Tag as Scope;
      if (scope != null) {
        DataObject data = new DataObject();
        data.SetData("Scope", scope);
        data.SetData("DragSource", scopesTreeView);
        DoDragDrop(data, DragDropEffects.Copy);
      }
    }
    #endregion

    #region Scope Events
    private void Scope_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Scope_NameChanged), sender, e);
      else {
        Scope scope = (Scope)sender;
        scopeNodeTable[scope].Text = scope.Name;
      }
    }
    #endregion

    #region SubScopes Events
    private void SubScopes_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<Scope>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsAdded), sender, e);
      else {
        Scope parentScope = subScopesScopeTable[(ScopeList)sender];
        TreeNode parentNode = scopeNodeTable[parentScope];
        if (parentNode.IsExpanded) {
          foreach (IndexedItem<Scope> item in e.Items) {
            TreeNode node = CreateTreeNode(item.Value);
            parentNode.Nodes.Insert(item.Index, node);
          }
        } else if (parentNode.Nodes.Count == 0) {
          parentNode.Nodes.Add(new TreeNode());
        }
      }
    }
    private void SubScopes_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Scope>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsRemoved), sender, e);
      else {
        Scope parentScope = subScopesScopeTable[(ScopeList)sender];
        TreeNode parentNode = scopeNodeTable[parentScope];
        if (parentNode.IsExpanded) {
          foreach (IndexedItem<Scope> item in e.Items) {
            TreeNode node = scopeNodeTable[item.Value];
            RemoveTreeNode(node);
            node.Remove();
          }
        } else if (parentScope.SubScopes.Count == 0) {
          parentNode.Nodes.Clear();
        }
      }
    }
    private void SubScopes_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<Scope>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsReplaced), sender, e);
      else {
        Scope parentScope = subScopesScopeTable[(ScopeList)sender];
        TreeNode parentNode = scopeNodeTable[parentScope];
        if (parentNode.IsExpanded) {
          foreach (IndexedItem<Scope> item in e.Items) {
            TreeNode node = parentNode.Nodes[item.Index];
            RemoveTreeNode(node);
            node.Remove();
            node = CreateTreeNode(item.Value);
            parentNode.Nodes.Insert(item.Index, node);
          }
        }
      }
    }
    private void SubScopes_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Scope>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_ItemsMoved), sender, e);
      else {
        Scope parentScope = subScopesScopeTable[(ScopeList)sender];
        TreeNode parentNode = scopeNodeTable[parentScope];
        if (parentNode.IsExpanded) {
          parentNode.Nodes.Clear();
          foreach (IndexedItem<Scope> item in e.Items)
            parentNode.Nodes.Insert(item.Index, scopeNodeTable[item.Value]);
        }
      }
    }
    private void SubScopes_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<Scope>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Scope>>(SubScopes_CollectionReset), sender, e);
      else {
        Scope parentScope = subScopesScopeTable[(ScopeList)sender];
        TreeNode parentNode = scopeNodeTable[parentScope];
        if (parentNode.IsExpanded) {
          foreach (TreeNode node in parentNode.Nodes)
            RemoveTreeNode(node);
          parentNode.Nodes.Clear();
          foreach (IndexedItem<Scope> item in e.Items) {
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
