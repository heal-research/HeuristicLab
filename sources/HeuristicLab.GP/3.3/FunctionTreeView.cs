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
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP {
  public partial class FunctionTreeView : ViewBase {
    private IFunctionTree functionTree;

    public FunctionTreeView() {
      InitializeComponent();
    }

    public FunctionTreeView(IFunctionTree functionTree)
      : this() {
      this.functionTree = functionTree;
      Refresh();
    }

    protected override void UpdateControls() {
      funTreeView.Nodes.Clear();
      TreeNode rootNode = new TreeNode();
      rootNode.Name = functionTree.ToString();
      rootNode.Text = functionTree.ToString();
      rootNode.Tag = functionTree;
      treeNodeContextMenu.MenuItems.Clear();
      DiscoveryService discoveryService = new DiscoveryService();
      IFunctionTreeSerializer[] exporters = discoveryService.GetInstances<IFunctionTreeSerializer>();
      foreach (IFunctionTreeSerializer exporter in exporters) {
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

    private EventHandler MakeExporterDelegate(IFunctionTreeSerializer exporter) {
      return delegate(object source, EventArgs args) {
        TreeNode node = funTreeView.SelectedNode;
        if (node == null || node.Tag == null) return;
        Clipboard.SetText(exporter.Export((IFunctionTree)node.Tag));
      };
    }

    private void CreateTree(TreeNode rootNode, IFunctionTree functionTree) {
      TreeNode node = new TreeNode();
      node.Name = functionTree.ToString();
      node.Text = functionTree.ToString();
      node.Tag = functionTree;
      node.ContextMenu = treeNodeContextMenu;
      rootNode.Nodes.Add(node);
      foreach (IFunctionTree subTree in functionTree.SubTrees) {
        CreateTree(node, subTree);
      }
    }

    private void funTreeView_MouseUp(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        // Select the clicked node
        funTreeView.SelectedNode = funTreeView.GetNodeAt(e.X, e.Y);
      }
    }
  }
}
