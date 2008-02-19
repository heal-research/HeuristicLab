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

namespace HeuristicLab.Core {
  public partial class ChooseTypeDialog : Form {
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

    public string Caption {
      get { return Text; }
      set { Text = value; }
    }

    private Type myType;
    public Type Type {
      get { return myType; }
    }

    public ChooseTypeDialog()
      : this(typeof(IItem)) {
    }

    public ChooseTypeDialog(Type baseType) {
      InitializeComponent();

      treeView.TreeViewNodeSorter = new TreeNodeSorter();

      if (baseType == null) {
        baseType = typeof(IItem);
      }

      DiscoveryService discoveryService = new DiscoveryService();
      foreach (PluginInfo plugin in discoveryService.Plugins) {
        TreeNode pluginNode = new TreeNode(plugin.Name);
        pluginNode.Tag = null;

        Type[] types = discoveryService.GetTypes(baseType, plugin);
        foreach (Type type in types) {
          TreeNode itemNode = new TreeNode();
          itemNode.Text = type.Name;
          itemNode.Tag = type;
          pluginNode.Nodes.Add(itemNode);
        }
        if (pluginNode.Nodes.Count > 0) {
          treeView.Nodes.Add(pluginNode);
        }
      }
      if (treeView.Nodes.Count == 0) {
        treeView.Enabled = false;
        treeView.Nodes.Add(new TreeNode("No item types available"));
      }
      okButton.Enabled = false;

      if (treeView.Nodes.Count == 1) {
        TreeNodeCollection items = treeView.Nodes[0].Nodes;
        treeView.Nodes.Clear();
        foreach (TreeNode i in items) {
          treeView.Nodes.Add(i);
        }
      }
    }

    private void okButton_Click(object sender, EventArgs e) {
      myType = (Type)treeView.SelectedNode.Tag;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void treeView_DoubleClick(object sender, EventArgs e) {
      // plugin groups have a null-tag
      if ((treeView.SelectedNode != null) && (treeView.SelectedNode.Tag != null)) {
        myType = (Type)treeView.SelectedNode.Tag;
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
    }

    private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
      okButton.Enabled = false;
      if ((treeView.SelectedNode != null) && (treeView.SelectedNode.Tag != null))
        okButton.Enabled = true;
    }

    private void ChooseTypeDialog_Load(object sender, EventArgs e) {
      myType = null;
      if (treeView.Nodes.Count == 1) {
        myType = (Type)treeView.Nodes[0].Tag;
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
    }
  }
}
