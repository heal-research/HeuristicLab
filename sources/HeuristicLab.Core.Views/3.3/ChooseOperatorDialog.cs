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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// A dialog to select an operator out of a library.
  /// </summary>
  public partial class ChooseOperatorDialog : Form {
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

    private IOperator myOperator;
    /// <summary>
    /// Gets the selected operator.
    /// </summary>
    public IOperator Operator {
      get { return myOperator; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ChooseOperatorDialog"/>.
    /// </summary>
    public ChooseOperatorDialog() {
      InitializeComponent();
      operatorLibrary = null;
      myOperator = null;
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

    #region Form Events
    private void ChooseOperatorDialog_Shown(object sender, EventArgs e) {
      operatorLibraryOperatorsTreeView.SelectedNode = null;
      operatorLibraryOperatorsDescriptionTextBox.Text = "";
      myOperator = null;
      builtinOperatorsTreeView.SelectedNode = null;
      builtinOperatorsDescriptionTextBox.Text = "";
      okButton.Enabled = false;
    }
    #endregion

    #region Selection Events
    private void operatorsTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      okButton.Enabled = false;
      myOperator = null;
      operatorLibraryOperatorsDescriptionTextBox.Text = "";
      if ((operatorLibraryOperatorsTreeView.SelectedNode != null) && (operatorLibraryOperatorsTreeView.SelectedNode.Tag is IOperator)) {
        myOperator = (IOperator)operatorLibraryOperatorsTreeView.SelectedNode.Tag;
        myOperator = (IOperator)myOperator.Clone();
        operatorLibraryOperatorsDescriptionTextBox.Text = myOperator.Description;
        okButton.Enabled = true;
      }
    }
    private void builtinOperatorsTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      okButton.Enabled = false;
      myOperator = null;
      builtinOperatorsDescriptionTextBox.Text = "";
      if ((builtinOperatorsTreeView.SelectedNode != null) && (builtinOperatorsTreeView.SelectedNode.Tag is Type)) {
        okButton.Enabled = true;
        myOperator = (IOperator)Activator.CreateInstance((Type)builtinOperatorsTreeView.SelectedNode.Tag);
        builtinOperatorsDescriptionTextBox.Text = myOperator.Description;
      }
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
    private void okButton_Click(object sender, EventArgs e) {
      if (Operator != null) {
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
    }
    private void treeView_DoubleClick(object sender, EventArgs e) {
      if (Operator != null) {
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
    }
    #endregion
  }
}
