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

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual represenation of <see cref="IScope"/>.
  /// </summary>
  [Content(typeof(Scope), true)]
  public partial class ScopeView : ItemViewBase {
    private Dictionary<IScope, TreeNode> scopeNodeTable;
    private Dictionary<IScope, bool> scopeExpandedTable;

    /// <summary>
    /// Gets or sets the scope to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public IScope Scope {
      get { return (IScope)Item; }
      set { base.Item = value; }
    }
    private bool myAutomaticUpdating;
    /// <summary>
    /// Gets information whether the scope is automatically updating. 
    /// </summary>
    public bool AutomaticUpdating {
      get { return myAutomaticUpdating; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ScopeView"/> with caption "Scope" and 
    /// property <see cref="AutomaticUpdating"/> set to <c>false</c>.
    /// </summary>
    public ScopeView() {
      InitializeComponent();
      Caption = "Scope";
      scopeNodeTable = new Dictionary<IScope, TreeNode>();
      scopeExpandedTable = new Dictionary<IScope, bool>();
      myAutomaticUpdating = false;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="ScopeView"/> with the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ScopeView()"/>.</remarks>
    /// <param name="scope">The scope to represent visually.</param>
    public ScopeView(IScope scope)
      : this() {
      Scope = scope;
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (scopesTreeView.Nodes.Count > 0)
        RemoveTreeNode(scopesTreeView.Nodes[0]);
      scopesTreeView.SelectedNode = null;
      scopesTreeView.Nodes.Clear();
      if (Scope == null) {
        Caption = "Scope";
        scopesTreeView.Enabled = false;
      } else {
        Caption = Scope.Name + " (" + Scope.GetType().Name + ")";
        scopesTreeView.Nodes.Add(CreateTreeNode(Scope));
        scopesTreeView.Enabled = true;
      }
    }

    private TreeNode CreateTreeNode(IScope scope) {
      TreeNode node = new TreeNode();
      node.Text = scope.Name;
      node.Tag = scope;

      scopeNodeTable.Add(scope, node);
      scopeExpandedTable.Add(scope, false);
      if (myAutomaticUpdating) {
        scope.SubScopeAdded += new EventHandler<EventArgs<IScope, int>>(Scope_SubScopeAdded);
        scope.SubScopeRemoved += new EventHandler<EventArgs<IScope, int>>(Scope_SubScopeRemoved);
        scope.SubScopesReordered += new EventHandler(Scope_SubScopesReordered);
      }
      if (scope.SubScopes.Count > 0)
        node.Nodes.Add(new TreeNode());
      return node;
    }
    private void RemoveTreeNode(TreeNode node) {
      foreach (TreeNode child in node.Nodes)
        RemoveTreeNode(child);

      IScope scope = node.Tag as IScope;
      if ((scope != null) && (scopeNodeTable.ContainsKey(scope))) {
        scopeNodeTable.Remove(scope);
        scopeExpandedTable.Remove(scope);
        scope.SubScopeAdded -= new EventHandler<EventArgs<IScope, int>>(Scope_SubScopeAdded);
        scope.SubScopeRemoved -= new EventHandler<EventArgs<IScope, int>>(Scope_SubScopeRemoved);
        scope.SubScopesReordered -= new EventHandler(Scope_SubScopesReordered);
      }
    }

    #region TreeView Events
    private void scopesTreeView_DoubleClick(object sender, EventArgs e) {
      // make sure that we can't get NullPointerExceptions
      if(scopesTreeView.SelectedNode != null && scopesTreeView.SelectedNode.Tag != null) {
        IScope scope = (IScope)scopesTreeView.SelectedNode.Tag;
        MainFormManager.MainForm.ShowView(new VariablesScopeView(scope));
      }
    }
    private void scopesTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
      TreeNode node = e.Node;
      IScope scope = (IScope)node.Tag;

      node.Nodes.Clear();
      for (int i = 0; i < scope.SubScopes.Count; i++)
        node.Nodes.Add(CreateTreeNode(scope.SubScopes[i]));
      scopeExpandedTable[scope] = true;
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
      scopeExpandedTable[scope] = false;
    }
    private void scopesTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode node = (TreeNode)e.Item;
      IScope scope = node.Tag as IScope;
      if (scope != null) {
        DataObject data = new DataObject();
        data.SetData("IScope", scope);
        data.SetData("DragSource", scopesTreeView);
        DoDragDrop(data, DragDropEffects.Copy);
      }
    }
    #endregion

    #region Context Menu Events
    private void contextMenuStrip_Opening(object sender, CancelEventArgs e) {
      variablesToolStripMenuItem.Enabled = false;
      viewToolStripMenuItem.DropDownItems.Clear();
      viewToolStripMenuItem.Enabled = false;
      if (scopesTreeView.SelectedNode != null) {
        variablesToolStripMenuItem.Enabled = true;
        IScope scope = (IScope)scopesTreeView.SelectedNode.Tag;
        foreach (IVariable variable in scope.Variables) {
          if (variable.Value is IVisualizationItem) {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = variable.Name + "...";
            item.Tag = variable.Value;
            item.Click += new EventHandler(showViewToolStripMenuItem_Click);
            viewToolStripMenuItem.DropDownItems.Add(item);
          }
        }
        if (viewToolStripMenuItem.DropDownItems.Count > 0)
          viewToolStripMenuItem.Enabled = true;
      }
    }
    private void automaticUpdatingToolStripMenuItem_Click(object sender, EventArgs e) {
      ToolStripMenuItem item = (ToolStripMenuItem)sender;
      myAutomaticUpdating = item.Checked;
      if (myAutomaticUpdating)
        Refresh();
    }
    private void refreshToolStripMenuItem_Click(object sender, EventArgs e) {
      Refresh();
    }
    private void variablesToolStripMenuItem_Click(object sender, EventArgs e) {
      IScope scope = (IScope)scopesTreeView.SelectedNode.Tag;
      MainFormManager.MainForm.ShowView(new VariablesScopeView(scope));
    }
    private void showViewToolStripMenuItem_Click(object sender, EventArgs e) {
      IItem item = (IItem)((ToolStripMenuItem)sender).Tag;
      IView view = MainFormManager.CreateDefaultView(item);
      if (view != null) MainFormManager.MainForm.ShowView(view);
    }
    #endregion

    #region Scope Events
    private delegate void ScopeDelegate(IScope scope);
    private delegate void ScopeScopeIndexDelegate(IScope scope, IScope subScope, int index);
    private void Scope_SubScopeAdded(object sender, EventArgs<IScope, int> e) {
      IScope scope = (IScope)sender;
      TreeNode node = scopeNodeTable[scope];
      if (scopeExpandedTable[scope] || (scope.SubScopes.Count == 1))
        AddSubScope(scope, e.Value, e.Value2);
    }
    private void AddSubScope(IScope scope, IScope subScope, int index) {
      if (InvokeRequired) {
        Invoke(new ScopeScopeIndexDelegate(AddSubScope), scope, subScope, index);
      } else {
        TreeNode parent = scopeNodeTable[scope];
        TreeNode child;
        if (parent.IsExpanded)
          child = CreateTreeNode(subScope);
        else
          child = new TreeNode();
        parent.Nodes.Insert(index, child);
      }
    }
    private void Scope_SubScopeRemoved(object sender, EventArgs<IScope, int> e) {
      IScope scope = (IScope)sender;
      TreeNode node = scopeNodeTable[scope];
      if (scopeExpandedTable[scope] || (scope.SubScopes.Count == 0))
        RemoveSubScope(scope, e.Value, e.Value2);
    }
    private void RemoveSubScope(IScope scope, IScope subScope, int index) {
      if (InvokeRequired) {
        Invoke(new ScopeScopeIndexDelegate(RemoveSubScope), scope, subScope, index);
      } else {
        if (scopeNodeTable.ContainsKey(subScope)) {
          TreeNode node = scopeNodeTable[subScope];
          RemoveTreeNode(scopeNodeTable[subScope]);
          node.Remove();
        } else {
          TreeNode node = scopeNodeTable[scope];
          node.Nodes[0].Remove();
        }
      }
    }
    private void Scope_SubScopesReordered(object sender, EventArgs e) {
      IScope scope = (IScope)sender;
      TreeNode node = scopeNodeTable[scope];
      if (scopeExpandedTable[scope])
        ReorderSubScopes(scope);
    }
    private void ReorderSubScopes(IScope scope) {
      if (InvokeRequired) {
        Invoke(new ScopeDelegate(ReorderSubScopes), scope);
      } else {
        TreeNode node = scopeNodeTable[scope];
        node.Nodes.Clear();
        for (int i = 0; i < scope.SubScopes.Count; i++)
          node.Nodes.Add(scopeNodeTable[scope.SubScopes[i]]);
      }
    }
    #endregion

    #region Mouse Events
    private void scopesTreeView_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button != MouseButtons.Right)
        return;
      TreeNode clickedNode = scopesTreeView.GetNodeAt(e.X, e.Y);
      if (clickedNode != null) {
        scopesTreeView.SelectedNode = clickedNode;
        scopesTreeView.Refresh();
      }
    }
    #endregion
  }
}
