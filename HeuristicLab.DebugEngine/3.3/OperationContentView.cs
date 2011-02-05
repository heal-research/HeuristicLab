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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.DebugEngine {

  [View("Operation Content View")]
  [Content(typeof(OperationContent), IsDefaultView = true)]
  public partial class OperationContentView : AsynchronousContentView {
    public new OperationContent Content {
      get { return (OperationContent)base.Content; }
      set { base.Content = value; }
    }

    public OperationContentView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        nameTextBox.Text = "";
        scopeTreeView.Nodes.Clear();
        executionContextTreeView.Nodes.Clear();
        iconBox.Image = null;
      } else {
        nameTextBox.Text = Content.Name;
        UpdateScopeTree();
        UpdateExecutionContext();
        iconBox.Image = Content.Icon;
      }
    }

    private object GetParameterValue(IParameter param, IExecutionContext context, out string actualName) {
      param = (IParameter)param.Clone();
      param.ExecutionContext = context;
      object value = null;
      try {
        value = param.ActualValue;
      } catch (Exception x) {
        value = x.Message;
      }
      ILookupParameter lookupParam = param as ILookupParameter;
      if (lookupParam != null)
        actualName = lookupParam.ActualName;
      else
        actualName = null;
      return value;
    }

    private void UpdateScopeTree() {
      scopeTreeView.BeginUpdate();
      scopeTreeView.Nodes.Clear();
      scopeTreeView.ImageList.Images.Clear();
      scopeTreeView.ImageList.Images.Add(VSImageLibrary.Namespace);
      scopeTreeView.ImageList.Images.Add(VSImageLibrary.Field);
      if (Content.IsContext) {
        var scope = Content.ExecutionContext.Scope;
        while (scope != null && scope.Parent != null)
          scope = scope.Parent;
        if (scope != null)
          AddScope(scopeTreeView.Nodes, scope);
      }
      scopeTreeView.EndUpdate();
    }

    private void AddScope(TreeNodeCollection nodes, IScope scope) {
      TreeNode node = nodes.Add(string.Format("{0} ({1}+{2})",
        scope.Name, scope.Variables.Count, scope.SubScopes.Count));
      node.Tag = scope;
      node.ImageIndex = 0;
      node.SelectedImageIndex = 0;
      foreach (var var in scope.Variables) {
        TreeNode varNode = node.Nodes.Add(string.Format("{0} = {1}", var.Name, var.Value));
        varNode.Tag = var.Value;
        varNode.ToolTipText = string.Format("{0}{1}{1}{2}",
          Utils.TypeName(var.Value), Environment.NewLine,
          Utils.Wrap(var.Description ?? var.ItemDescription, 60));
        varNode.ImageIndex = 1;
        varNode.SelectedImageIndex = 1;
      }
      foreach (var subScope in scope.SubScopes)
        AddScope(node.Nodes, subScope);
      if (Content.IsAtomic && Content.AtomicOperation.Scope == scope) {
        node.NodeFont = new Font(DefaultFont, FontStyle.Bold);
        node.ForeColor = Color.White;
        node.BackColor = Color.Crimson;
        node.Expand();
        scopeTreeView.TopNode = node;
        node.ToolTipText = "Current scope of active operation";
      }
    }


    private void UpdateExecutionContext() {
      executionContextTreeView.BeginUpdate();
      executionContextTreeView.Nodes.Clear();
      executionContextTreeView.ImageList.Images.Clear();
      executionContextTreeView.ImageList.Images.Add(VSImageLibrary.Namespace);
      if (Content.IsContext) {
        AddExecutionContext(Content.ExecutionContext, executionContextTreeView.Nodes);
      }
      executionContextTreeView.ExpandAll();
      if (executionContextTreeView.Nodes.Count > 0)
        executionContextTreeView.TopNode = executionContextTreeView.Nodes[0];
      executionContextTreeView.EndUpdate();
    }

    private void AddExecutionContext(IExecutionContext executionContext, TreeNodeCollection nodes) {
      ExecutionContext context = executionContext as ExecutionContext;
      StringBuilder name = new StringBuilder();
      if (context != null && context.Operator != null)
        name.Append(context.Operator.Name);
      else
        name.Append("<Context>");
      name.Append("@").Append(executionContext.Scope.Name);
      TreeNode node = nodes.Add(name.ToString());
      node.Tag = executionContext;
      node.ImageIndex = 0;
      node.SelectedImageIndex = 0;
      foreach (var param in executionContext.Parameters) {
        string actualName = null;
        object value = GetParameterValue(param, executionContext, out actualName);
        if (value == null)
          value = "null";
        string label = actualName != null && actualName != param.Name ?
          string.Format("{0} ({1}) = {2}", param.Name, actualName, value) :
          string.Format("{0} = {1}", param.Name, value);
        TreeNode paramNode = node.Nodes.Add(label);
        paramNode.Tag = param;
        executionContextTreeView.ImageList.Images.Add(param.ItemImage ?? VSImageLibrary.Nothing);
        paramNode.ImageIndex = executionContextTreeView.ImageList.Images.Count - 1;
        paramNode.SelectedImageIndex = paramNode.ImageIndex;
        paramNode.ToolTipText = string.Format("{0}{1}{1}{2}",
          Utils.TypeName(param), Environment.NewLine,
          Utils.Wrap(param.Description ?? param.ItemDescription, 60));
      }
      if (executionContext.Parent != null)
        AddExecutionContext(executionContext.Parent, node.Nodes);
    }

    #region Event Handlers (child controls)

    private void executionContextTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
      if (e.Node != null) {
        IParameter param = e.Node.Tag as IParameter;
        if (param != null)
          MainFormManager.MainForm.ShowContent(param);
      }
    }

    private void scopeTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
      if (e.Node != null) {
        IItem item = e.Node.Tag as IItem;
        if (item != null)
          MainFormManager.MainForm.ShowContent(item);
      }
    }

    private TreeNode selectedScopeNode = null;
    private void executionContextTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
      scopeTreeView.BeginUpdate();
      if (selectedScopeNode != null) {
        if (Content.IsAtomic && Content.AtomicOperation.Scope == selectedScopeNode.Tag) {
          selectedScopeNode.BackColor = Color.Crimson;
          selectedScopeNode.ForeColor = Color.White;
          selectedScopeNode.NodeFont = new Font(DefaultFont, FontStyle.Bold);
        } else {
          selectedScopeNode.BackColor = Color.White;
          selectedScopeNode.ForeColor = Color.Black;
          selectedScopeNode.NodeFont = DefaultFont;
        }
      }
      if (e.Node != null) {
        IExecutionContext context = e.Node.Tag as IExecutionContext;
        if (context != null && context.Scope != null) {
          TreeNode scopeNode = FindScopeNode(context.Scope, scopeTreeView.Nodes);
          if (scopeNode != null) {
            if (Content.IsAtomic && Content.AtomicOperation.Scope == scopeNode.Tag) {
              scopeNode.BackColor = Color.DarkViolet;
              scopeNode.ForeColor = Color.White;
              scopeNode.NodeFont = new Font(DefaultFont, FontStyle.Bold);
            } else {
              scopeNode.BackColor = Color.Blue;
              scopeNode.ForeColor = Color.White;
              scopeNode.NodeFont = new Font(DefaultFont, FontStyle.Bold);
            }
            selectedScopeNode = scopeNode;
            scopeTreeView.TopNode = scopeNode;
          }
        }
      }
      scopeTreeView.EndUpdate();
    }

    private TreeNode FindScopeNode(IScope scope, TreeNodeCollection nodes) {
      foreach (TreeNode node in nodes) {
        if (node.Tag == scope) {
          return node;
        } else {
          TreeNode childNode = FindScopeNode(scope, node.Nodes);
          if (childNode != null)
            return childNode;
        }
      }
      return null;
    }

    private void nameTextBox_DoubleClick(object sender, EventArgs e) {
      if (Content != null && Content.IsAtomic && Content.AtomicOperation.Operator != null)
        MainFormManager.MainForm.ShowContent(Content.AtomicOperation.Operator);
    }

    private void ShowValue_Click(object sender, EventArgs e) {
      if (executionContextTreeView.SelectedNode == null)
        return;
      IParameter param = executionContextTreeView.SelectedNode.Tag as IParameter;
      if (param != null) {
        string actualName = null;
        IExecutionContext context = executionContextTreeView.SelectedNode.Parent.Tag as IExecutionContext ?? Content.ExecutionContext;
        MainFormManager.MainForm.ShowContent(GetParameterValue(param, context, out actualName) as IContent);
      }
    }

    private void executionContextConextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
      IParameter param = executionContextTreeView.SelectedNode.Tag as IParameter;
      if (param != null) {
        string actualName = null;
        IExecutionContext context = executionContextTreeView.SelectedNode.Parent.Tag as IExecutionContext ?? Content.ExecutionContext;
        showValueToolStripMenuItem.Enabled = GetParameterValue(param, context, out actualName) is IContent;
      } else
        e.Cancel = true;
    }

    private void executionContextTreeView_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button == System.Windows.Forms.MouseButtons.Right)
        executionContextTreeView.SelectedNode = executionContextTreeView.GetNodeAt(e.Location);
    }

    #endregion

  }
}
