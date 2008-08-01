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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Core {
  public partial class DatabaseOperatorLibraryView : ViewBase {
    private ChooseOperatorDialog chooseOperatorDialog;

    public IOperatorLibrary OperatorLibrary {
      get { return (IOperatorLibrary)Item; }
      set { base.Item = value; }
    }

    public DatabaseOperatorLibraryView() {
      InitializeComponent();
      operatorsTreeView.TreeViewNodeSorter = new NodeSorter();
      Caption = "Operator Library";
    }
    public DatabaseOperatorLibraryView(DatabaseOperatorLibrary operatorLibrary)
      : this() {
      OperatorLibrary = operatorLibrary;
    }

    private class NodeSorter : IComparer {
      public int Compare(object x, object y) {
        TreeNode tx = x as TreeNode;
        TreeNode ty = y as TreeNode;

        int result = string.Compare(tx.Text, ty.Text);
        if (result == 0)
          result = tx.Index.CompareTo(ty.Index);
        return result;
      }
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      operatorsTreeView.Nodes.Clear();
      addOperatorButton.Enabled = false;
      removeButton.Enabled = false;
      if (OperatorLibrary == null) {
        Caption = "Operator Library";
        operatorsGroupBox.Enabled = false;
      } else {
        Caption = OperatorLibrary.Group.Name + " (" + OperatorLibrary.GetType().Name + ")";
        operatorsGroupBox.Enabled = true;
        operatorsTreeView.Nodes.Add(CreateTreeNode(OperatorLibrary.Group));
        operatorsTreeView.Sort();
      }
    }

    private TreeNode CreateTreeNode(IOperatorGroup group) {
      TreeNode node = new TreeNode();
      node.Text = group.Name;
      node.ForeColor = Color.LightGray;
      node.Tag = group;

      foreach(IOperator op in group.Operators)
        node.Nodes.Add(CreateTreeNode(op));
      foreach(IOperatorGroup subGroup in group.SubGroups)
        node.Nodes.Add(CreateTreeNode(subGroup));
      return node;
    }
    private TreeNode CreateTreeNode(IOperator op) {
      TreeNode node = new TreeNode();
      node.Text = op.Name;
      node.ToolTipText = op.GetType().Name;
      node.Tag = op;
      return node;
    }
    private void RemoveTreeNode(TreeNode node) {
      if (node.Tag is IOperatorGroup) {
        IOperatorGroup group = node.Tag as IOperatorGroup;
        IOperatorGroup parent = node.Parent.Tag as IOperatorGroup;
        parent.RemoveSubGroup(group);
      } else if (node.Tag is IOperator) {
        IOperator op = node.Tag as IOperator;
        IOperatorGroup group = node.Parent.Tag as IOperatorGroup;
        group.RemoveOperator(op);
      }
      node.Remove();
      EnableDisableButtons();
    }

    private void EnableDisableButtons() {
      addOperatorButton.Enabled = false;
      removeButton.Enabled = false;
      operatorsTreeView.Focus();

      if (operatorsTreeView.SelectedNode != null) {
        if (operatorsTreeView.SelectedNode.Parent != null)
          removeButton.Enabled = true;
        if (operatorsTreeView.SelectedNode.Tag is IOperatorGroup) {
          addOperatorButton.Enabled = true;
        }
      }
    }

    #region TreeView Events
    private void operatorsTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      EnableDisableButtons();
    }
    private void operatorsTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
      e.CancelEdit = false;

      string name = e.Label;
      if (name != null) {
        if (e.Node.Tag is IOperatorGroup)
          ((IOperatorGroup)e.Node.Tag).Name = name;
        else if (e.Node.Tag is IOperator)
          ((IOperator)e.Node.Tag).Name = name;
      }
    }
    private void operatorsTreeView_DoubleClick(object sender, EventArgs e) {
      if (operatorsTreeView.SelectedNode != null) {
        IOperator op = operatorsTreeView.SelectedNode.Tag as IOperator;
        if (op != null) {
          IView view = op.CreateView();
          if (view != null)
            PluginManager.ControlManager.ShowControl(view);
        }
      }
    }
    #endregion

    #region Button Events
    private void addOperatorButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        TreeNode node = operatorsTreeView.SelectedNode;
        IOperatorGroup group = node.Tag as IOperatorGroup;
        group.AddOperator(chooseOperatorDialog.Operator);
        node.Nodes.Add(CreateTreeNode(chooseOperatorDialog.Operator));
        operatorsTreeView.Sort();
        EnableDisableButtons();
      }
    }
    private void removeButton_Click(object sender, EventArgs e) {
      TreeNode node = operatorsTreeView.SelectedNode;
      RemoveTreeNode(node);
    }
    #endregion

    #region Key Events
    private void operatorsTreeView_KeyDown(object sender, KeyEventArgs e) {
      TreeNode node = operatorsTreeView.SelectedNode;
      if ((e.KeyCode == Keys.Delete) && (node != null) && (node.Parent != null))
        RemoveTreeNode(operatorsTreeView.SelectedNode);
      if ((e.KeyCode == Keys.F2) && (node != null))
        node.BeginEdit();
    }
    #endregion


    #region Mouse Events
    private void operatorsTreeView_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button != MouseButtons.Right) return;
      TreeNode clickedNode = operatorsTreeView.GetNodeAt(e.X, e.Y);
      if (clickedNode != null) {
        operatorsTreeView.SelectedNode = clickedNode;
        operatorsTreeView.Refresh();
      }
    }
    #endregion

    #region Drag and Drop Events
    private void operatorsTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode node = (TreeNode)e.Item;
      IOperator op = node.Tag as IOperator;
      if (op != null) {
        DataObject data = new DataObject();
        data.SetData("IOperator", op);
        data.SetData("DragSource", operatorsTreeView);
        DoDragDrop(data, DragDropEffects.Copy);
      }
    }
    private void operatorsTreeView_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetDataPresent("IOperator")) {
        Point p = operatorsTreeView.PointToClient(new Point(e.X, e.Y));
        TreeNode node = operatorsTreeView.GetNodeAt(p);
        if ((node != null) && (node.Tag is IOperatorGroup))
          e.Effect = DragDropEffects.Copy;
      }
    }
    private void operatorsTreeView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetDataPresent("IOperator")) {
        Point p = operatorsTreeView.PointToClient(new Point(e.X, e.Y));
        TreeNode node = operatorsTreeView.GetNodeAt(p);
        if ((node != null) && (node.Tag is IOperatorGroup))
          e.Effect = DragDropEffects.Copy;
      }
    }
    private void operatorsTreeView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        if (e.Data.GetDataPresent("IOperator")) {
          IOperator op = (IOperator)e.Data.GetData("IOperator");
          Point p = operatorsTreeView.PointToClient(new Point(e.X, e.Y));
          TreeNode node = operatorsTreeView.GetNodeAt(p);
          if (node != null) {
            op = (IOperator)op.Clone();

            while (op.SubOperators.Count > 0)
              op.RemoveSubOperator(0);

            IOperatorGroup group = (IOperatorGroup)node.Tag;
            group.AddOperator(op);
            node.Nodes.Add(CreateTreeNode(op));
            operatorsTreeView.Sort();
            EnableDisableButtons();
          }
        }
      }
    }
    #endregion

    private void saveButton_Click(object sender, EventArgs e) {
      ((DatabaseOperatorLibrary)OperatorLibrary).Save();
    }

    private void refreshButton_Click(object sender, EventArgs e) {
      ((DatabaseOperatorLibrary)OperatorLibrary).Restore();
      UpdateControls();
    }
  }
}
