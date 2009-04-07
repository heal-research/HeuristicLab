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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Data;

namespace HeuristicLab.GP {
  public partial class FunctionTreeView : ViewBase {
    private IFunctionTree functionTree;

    private IFunctionTree selectedBranch;
    private IVariable selectedVariable;
    private IFunctionTreeNameGenerator nameGenerator;
    private IFunctionTreeNameGenerator[] allNameGenerators;
    private MenuItem representationsMenu;

    public FunctionTreeView() {
      nameGenerator = new DefaultFunctionTreeNameGenerator();
      InitializeComponent();

      DiscoveryService discoveryService = new DiscoveryService();
      allNameGenerators = discoveryService.GetInstances<IFunctionTreeNameGenerator>();
      representationsMenu = new MenuItem();
      representationsMenu.Text = "Tree representation";
      representationsMenu.Name = "Tree representation";
      foreach (IFunctionTreeNameGenerator generator in allNameGenerators) {
        MenuItem mi = new MenuItem(generator.Name, MakeNameGeneratorDelegate(generator), Shortcut.None);
        if (generator is DefaultFunctionTreeNameGenerator) mi.Checked = true;
        else mi.Checked = false;
        mi.Tag = generator;
        representationsMenu.MenuItems.Add(mi);
      }
    }

    public FunctionTreeView(IFunctionTree functionTree)
      : this() {
      this.functionTree = functionTree;
      Refresh();
    }

    protected override void UpdateControls() {
      funTreeView.Nodes.Clear();
      TreeNode rootNode = new TreeNode();
      rootNode.Name = functionTree.Function.Name;
      rootNode.Text = nameGenerator.GetName(functionTree);
      rootNode.Tag = functionTree;
      treeNodeContextMenu.MenuItems.Clear();
      treeNodeContextMenu.MenuItems.Add(representationsMenu);
      DiscoveryService discoveryService = new DiscoveryService();
      IFunctionTreeExporter[] exporters = discoveryService.GetInstances<IFunctionTreeExporter>();
      foreach (IFunctionTreeExporter exporter in exporters) {
        string result;
        // register a menu item for the exporter
        MenuItem item = new MenuItem("Copy to clip-board (" + exporter.Name + ")",
          MakeExporterDelegate(exporter), Shortcut.None);
        // try to export the whole tree 
        if (exporter.TryExport(functionTree, out result)) {
          // if it worked enable the context-menu item
          item.Enabled = true;
        } else {
          item.Enabled = false;
        }
        treeNodeContextMenu.MenuItems.Add(item);
      }
      rootNode.ContextMenu = treeNodeContextMenu;
      funTreeView.Nodes.Add(rootNode);

      foreach (IFunctionTree subTree in functionTree.SubTrees) {
        CreateTree(rootNode, subTree);
      }
      funTreeView.ExpandAll();
    }

    private EventHandler MakeNameGeneratorDelegate(IFunctionTreeNameGenerator generator) {
      return delegate(object source, EventArgs args) {
        IFunctionTreeNameGenerator g = (IFunctionTreeNameGenerator)((MenuItem)source).Tag;
        this.nameGenerator = g;
        foreach (MenuItem otherMenuItem in representationsMenu.MenuItems) otherMenuItem.Checked = false;
        ((MenuItem)source).Checked = true;
        UpdateControls();
      };
    }

    private EventHandler MakeExporterDelegate(IFunctionTreeExporter exporter) {
      return delegate(object source, EventArgs args) {
        TreeNode node = funTreeView.SelectedNode;
        if (node == null || node.Tag == null) return;
        Clipboard.SetText(exporter.Export((IFunctionTree)node.Tag));
      };
    }

    private void CreateTree(TreeNode rootNode, IFunctionTree functionTree) {
      TreeNode node = new TreeNode();
      node.Name = functionTree.Function.Name;
      node.Text = nameGenerator.GetName(functionTree);
      node.Tag = functionTree;
      node.ContextMenu = treeNodeContextMenu;
      rootNode.Nodes.Add(node);
      foreach (IFunctionTree subTree in functionTree.SubTrees) {
        CreateTree(node, subTree);
      }
    }

    private void functionTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      variablesListBox.Items.Clear();
      variablesSplitContainer.Panel2.Controls.Clear();
      templateTextBox.Clear();
      editButton.Enabled = false;
      if (funTreeView.SelectedNode != null && funTreeView.SelectedNode.Tag != null) {
        IFunctionTree selectedBranch = (IFunctionTree)funTreeView.SelectedNode.Tag;
        UpdateVariablesList(selectedBranch);
        templateTextBox.Text = nameGenerator.GetName(selectedBranch);
        this.selectedBranch = selectedBranch;
        editButton.Enabled = true;
      }
    }

    private void UpdateVariablesList(IFunctionTree functionTree) {
      foreach (IVariable variable in functionTree.LocalVariables) {
        variablesListBox.Items.Add(variable.Name);
      }
    }

    private void variablesListBox_SelectedIndexChanged(object sender, EventArgs e) {
      // in case we had an event-handler registered for a different variable => unregister the event-handler
      if (selectedVariable != null) {
        selectedVariable.Value.Changed -= new EventHandler(selectedVariable_ValueChanged);
      }
      if (variablesListBox.SelectedItem != null) {
        string selectedVariableName = (string)variablesListBox.SelectedItem;
        selectedVariable = selectedBranch.GetLocalVariable(selectedVariableName);
        variablesSplitContainer.Panel2.Controls.Clear();
        Control editor = (Control)selectedVariable.CreateView();
        variablesSplitContainer.Panel2.Controls.Add(editor);
        editor.Dock = DockStyle.Fill;
        // register an event handler that updates the treenode when the value of the variable is changed by the user
        selectedVariable.Value.Changed += new EventHandler(selectedVariable_ValueChanged);
      } else {
        variablesSplitContainer.Panel2.Controls.Clear();
      }
    }

    void selectedVariable_ValueChanged(object sender, EventArgs e) {
      if (funTreeView.SelectedNode != null && funTreeView.SelectedNode.Tag != null) {
        TreeNode node = funTreeView.SelectedNode;
        node.Text = nameGenerator.GetName(functionTree);
      }
    }

    protected virtual void editButton_Click(object sender, EventArgs e) {
      PluginManager.ControlManager.ShowControl(selectedBranch.Function.CreateView());
    }

    private void funTreeView_MouseUp(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        // Select the clicked node
        funTreeView.SelectedNode = funTreeView.GetNodeAt(e.X, e.Y);
      }
    }
  }
}
