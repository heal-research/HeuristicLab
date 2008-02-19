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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;

namespace HeuristicLab.AdvancedOptimizationFrontend {
  public partial class AvailableOperatorsForm : DockContent {
    #region Inner Class TreeNodeSorter
    private class TreeNodeSorter : IComparer {
      public int Compare(object x, object y) {
        TreeNode tx = x as TreeNode;
        TreeNode ty = y as TreeNode;

        int result = string.Compare(tx.Text, ty.Text);
        if (result == 0)
          result = tx.Index.CompareTo(ty.Index);
        return result;
      }
    }
    #endregion

    private IOperatorLibrary operatorLibrary;

    public AvailableOperatorsForm() {
      InitializeComponent();
      operatorLibrary = null;
      TreeNodeSorter nodeSorter = new TreeNodeSorter();
      operatorLibraryOperatorsTreeView.TreeViewNodeSorter = nodeSorter;
      builtinOperatorsTreeView.TreeViewNodeSorter = nodeSorter;

      DiscoveryService discoveryService = new DiscoveryService();
      PluginInfo[] plugins = discoveryService.Plugins;
      foreach(PluginInfo plugin in plugins) {
        TreeNode pluginItem = new TreeNode();
        pluginItem.Text = plugin.Name;
        pluginItem.Tag = plugin;

        Type[] operators = discoveryService.GetTypes(typeof(IOperator), plugin);
        foreach(Type type in operators) {
          if(!type.IsAbstract) {
            TreeNode operatorItem = new TreeNode();
            operatorItem.Text = type.Name;
            operatorItem.Tag = type;
            pluginItem.Nodes.Add(operatorItem);
          }
        }
        // add plugin node only if it contains operators
        if(pluginItem.Nodes.Count > 0) {
          builtinOperatorsTreeView.Nodes.Add(pluginItem);
        }
      }
      if (builtinOperatorsTreeView.Nodes.Count == 0) {
        builtinOperatorsTreeView.Enabled = false;
        builtinOperatorsTreeView.Nodes.Add("No operators available");
      }
      builtinOperatorsTreeView.Sort();
    }

    private void UpdateOperatorsTreeView() {
      operatorLibraryOperatorsTreeView.Nodes.Clear();
      operatorLibraryOperatorsTreeView.Nodes.Add(CreateTreeNode(operatorLibrary.Group));
      operatorLibraryOperatorsTreeView.Sort();
    }

    private TreeNode CreateTreeNode(IOperatorGroup group) {
      TreeNode node = new TreeNode();
      node.Text = group.Name;
      node.ForeColor = Color.LightGray;
      node.Tag = group;

      foreach (IOperator op in group.Operators)
        node.Nodes.Add(CreateTreeNode(op));
      foreach (IOperatorGroup subGroup in group.SubGroups)
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

    #region Selection Events
    private void operatorsTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      operatorLibraryOperatorsDescriptionTextBox.Text = "";
      if ((operatorLibraryOperatorsTreeView.SelectedNode != null) && (operatorLibraryOperatorsTreeView.SelectedNode.Tag is IOperator))
        operatorLibraryOperatorsDescriptionTextBox.Text = ((IOperator)operatorLibraryOperatorsTreeView.SelectedNode.Tag).Description;
    }
    private void builtinOperatorsTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      builtinOperatorsDescriptionTextBox.Text = "";
      if ((builtinOperatorsTreeView.SelectedNode != null) && (builtinOperatorsTreeView.SelectedNode.Tag is Type))
        builtinOperatorsDescriptionTextBox.Text = ((IOperator)Activator.CreateInstance((Type)builtinOperatorsTreeView.SelectedNode.Tag)).Description;
    }
    #endregion

    #region Click Events
    private void loadButton_Click(object sender, EventArgs e) {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        operatorLibrary = (IOperatorLibrary)PersistenceManager.Load(openFileDialog.FileName);
        operatorLibraryTextBox.Text = openFileDialog.FileName;
        toolTip.SetToolTip(operatorLibraryTextBox, openFileDialog.FileName);
        operatorLibraryOperatorsGroupBox.Enabled = true;
        UpdateOperatorsTreeView();
      }
    }
    private void closeButton_Click(object sender, EventArgs e) {
      this.Close();
    }
    #endregion

    #region Drag and Drop Events
    private void operatorLibraryOperatorsTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode node = e.Item as TreeNode;
      if ((node != null) && (node.Tag is IOperator)) {
        IOperator op = (IOperator)node.Tag;
        op = (IOperator)op.Clone();
        DataObject data = new DataObject();
        data.SetData("IOperator", op);
        data.SetData("DragSource", operatorLibraryOperatorsTreeView);
        DoDragDrop(data, DragDropEffects.Copy);
      }
    }
    private void builtinOperatorsTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode node = e.Item as TreeNode;
      if ((node != null) && (node.Tag is Type)) {
        IOperator op = (IOperator)Activator.CreateInstance((Type)node.Tag);
        DataObject data = new DataObject();
        data.SetData("IOperator", op);
        data.SetData("DragSource", builtinOperatorsTreeView);
        DoDragDrop(data, DragDropEffects.Copy);
      }
    }
    #endregion
  }
}
