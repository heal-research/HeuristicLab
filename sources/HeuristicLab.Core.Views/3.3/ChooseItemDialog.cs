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
  /// A dialog to select an item. 
  /// </summary>
  public partial class ChooseItemDialog : Form {
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

    /// <summary>
    /// Gets or sets the caption of the dialog.
    /// </summary>
    /// <remarks>Uses property <see cref="Form.Text"/> of base class <see cref="System.Windows.Forms.Form"/>.
    /// No own data storage present.</remarks>
    public string Caption {
      get { return Text; }
      set { Text = value; }
    }

    private IItem myItem;
    /// <summary>
    /// Gets the selected item.
    /// </summary>
    public IItem Item {
      get { return myItem; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ChooseItemDialog"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ChooseItemDialog(System.Type)"/> with the type of <see cref="IItem"/>
    /// as parameter.</remarks>
    public ChooseItemDialog()
      : this(typeof(IItem)) {
    }
    /// <summary>
    /// Initializes a new instance of <see cref="ChooseItemDialog"/> with 
    /// the given <paramref name="itemType"/>.
    /// </summary>
    /// <param name="itemType">The type of the items to choose from.</param>
    public ChooseItemDialog(Type itemType) {
      InitializeComponent();

      treeView.TreeViewNodeSorter = new TreeNodeSorter();

      if (itemType == null) {
        itemType = typeof(IItem);
      }

      DiscoveryService discoveryService = new DiscoveryService();
      foreach (PluginInfo plugin in discoveryService.Plugins) {
        TreeNode pluginNode = new TreeNode(plugin.Name);
        pluginNode.Tag = null;

        Type[] types = discoveryService.GetTypes(itemType, plugin);
        foreach (Type type in types) {
          if (!type.IsAbstract) {
            TreeNode itemNode = new TreeNode();
            itemNode.Text = type.Name;
            itemNode.Tag = type;
            pluginNode.Nodes.Add(itemNode);
          }
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
      Type type = (Type)treeView.SelectedNode.Tag;

      if (type.ContainsGenericParameters) {
        type = type.GetGenericTypeDefinition();
        Type[] args = new Type[type.GetGenericArguments().Length];
        ChooseTypeDialog dialog = new ChooseTypeDialog();
        for (int i = 0; i < args.Length; i++) {
          dialog.Caption = "Choose type of generic parameter " + (i + 1).ToString();
          if (dialog.ShowDialog(this) == DialogResult.OK)
            args[i] = dialog.Type;
          else
            args[i] = null;
        }
        dialog.Dispose();
        type = type.MakeGenericType(args);
      }

      myItem = (IItem)Activator.CreateInstance(type);
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void treeView_DoubleClick(object sender, EventArgs e) {
      // plugin groups have a null-tag
      if ((treeView.SelectedNode != null) && (treeView.SelectedNode.Tag != null)) {
        Type type = (Type)treeView.SelectedNode.Tag;

        if (type.ContainsGenericParameters) {
          type = type.GetGenericTypeDefinition();
          Type[] args = new Type[type.GetGenericArguments().Length];
          ChooseTypeDialog dialog = new ChooseTypeDialog();
          for (int i = 0; i < args.Length; i++) {
            dialog.Caption = "Choose type of generic parameter " + (i + 1).ToString();
            if (dialog.ShowDialog(this) == DialogResult.OK)
              args[i] = dialog.Type;
            else
              args[i] = null;
          }
          dialog.Dispose();
          type = type.MakeGenericType(args);
        }

        myItem = (IItem)Activator.CreateInstance(type);
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
    }

    private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
      okButton.Enabled = false;
      if ((treeView.SelectedNode != null) && (treeView.SelectedNode.Tag != null))
        okButton.Enabled = true;
    }

    private void ChooseItemDialog_Load(object sender, EventArgs e) {
      myItem = null;
      if (treeView.Nodes.Count == 1) {
        Type type = (Type)treeView.Nodes[0].Tag;
        if (!type.ContainsGenericParameters) {
          myItem = (IItem)Activator.CreateInstance(type);
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
    }
  }
}
