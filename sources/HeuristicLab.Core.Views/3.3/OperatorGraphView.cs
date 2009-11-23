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
  /// The visual representation of an <see cref="IOperatorGraph"/>.
  /// </summary>
  [Content(typeof(OperatorGraph), true)]
  public partial class OperatorGraphView : ViewBase {
    private ChooseOperatorDialog chooseOperatorDialog;
    private Dictionary<IOperator, IList<TreeNode>> operatorNodeTable;

    /// <summary>
    /// Gets or sets the operator graph to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public IOperatorGraph OperatorGraph {
      get { return (IOperatorGraph)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraphView"/> with caption "Operator Graph".
    /// </summary>
    public OperatorGraphView() {
      InitializeComponent();
      operatorNodeTable = new Dictionary<IOperator, IList<TreeNode>>();
      operatorsListView.Columns[0].Width = Math.Max(0, operatorsListView.Width - 25);
      Caption = "Operator Graph";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraphView"/> 
    /// with the given <paramref name="operatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="OperatorGraphView()"/>.</remarks>
    /// <param name="operatorGraph">The operator graph to represent visually.</param>
    public OperatorGraphView(IOperatorGraph operatorGraph)
      : this() {
      OperatorGraph = operatorGraph;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IOperatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RemoveItemEvents() {
      OperatorGraph.OperatorAdded -= new EventHandler<EventArgs<IOperator>>(OperatorGraph_OperatorAdded);
      OperatorGraph.OperatorRemoved -= new EventHandler<EventArgs<IOperator>>(OperatorGraph_OperatorRemoved);
      OperatorGraph.InitialOperatorChanged -= new EventHandler(OperatorGraph_InitialOperatorChanged);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IOperatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      OperatorGraph.OperatorAdded += new EventHandler<EventArgs<IOperator>>(OperatorGraph_OperatorAdded);
      OperatorGraph.OperatorRemoved += new EventHandler<EventArgs<IOperator>>(OperatorGraph_OperatorRemoved);
      OperatorGraph.InitialOperatorChanged += new EventHandler(OperatorGraph_InitialOperatorChanged);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (graphTreeView.Nodes.Count > 0)
        RemoveTreeNode(graphTreeView.Nodes[0]);
      graphTreeView.SelectedNode = null;
      graphTreeView.Nodes.Clear();
      graphTreeView.Enabled = false;
      operatorsListView.Items.Clear();
      removeButton.Enabled = false;
      if (OperatorGraph == null) {
        Caption = "Operator Graph";
        operatorsListView.Enabled = false;
        addOperatorButton.Enabled = false;
      } else {
        Caption = "Operator Graph (" + OperatorGraph.GetType().Name + ")";
        foreach (IOperator op in OperatorGraph.Operators) {
          operatorsListView.Items.Add(CreateListViewItem(op));
        }
        operatorsListView.Enabled = true;
        addOperatorButton.Enabled = true;
        if (OperatorGraph.InitialOperator != null) {
          graphTreeView.Nodes.Add(CreateTreeNode(OperatorGraph.InitialOperator));
          graphTreeView.Enabled = true;
        }
      }
    }

    private ListViewItem CreateListViewItem(IOperator op) {
      ListViewItem item = new ListViewItem();
      item.Text = op.Name;
      item.Tag = op;
      item.ImageIndex = 0;
      if (op.GetType().Name == "CombinedOperator")
        item.ImageIndex = 1;
      else if (op.GetType().Name == "ProgrammableOperator")
        item.ImageIndex = 2;
      if (op == OperatorGraph.InitialOperator)
        item.Font = new Font(operatorsListView.Font, FontStyle.Bold);
      return item;
    }

    private TreeNode CreateTreeNode(IOperator op) {
      TreeNode node = new TreeNode();
      node.Text = op.Name;
      node.Tag = op;
      if (op.Breakpoint)
        node.ForeColor = Color.Red;

      if (!operatorNodeTable.ContainsKey(op)) {
        operatorNodeTable.Add(op, new List<TreeNode>());
        op.NameChanged += new EventHandler(Operator_NameChanged);
        op.BreakpointChanged += new EventHandler(Operator_BreakpointChanged);
        op.SubOperatorAdded += new EventHandler<EventArgs<IOperator, int>>(Operator_SubOperatorAdded);
        op.SubOperatorRemoved += new EventHandler<EventArgs<IOperator, int>>(Operator_SubOperatorRemoved);
      }
      operatorNodeTable[op].Add(node);

      for (int i = 0; i < op.SubOperators.Count; i++)
        node.Nodes.Add(new TreeNode());
      return node;
    }

    private void RemoveTreeNode(TreeNode node) {
      foreach (TreeNode child in node.Nodes)
        RemoveTreeNode(child);

      IOperator op = (IOperator)node.Tag;
      if (op != null) {
        operatorNodeTable[op].Remove(node);
        if (operatorNodeTable[op].Count == 0) {
          op.NameChanged -= new EventHandler(Operator_NameChanged);
          op.BreakpointChanged -= new EventHandler(Operator_BreakpointChanged);
          op.SubOperatorAdded -= new EventHandler<EventArgs<IOperator, int>>(Operator_SubOperatorAdded);
          op.SubOperatorRemoved -= new EventHandler<EventArgs<IOperator, int>>(Operator_SubOperatorRemoved);
          operatorNodeTable.Remove(op);
        }
      }
    }

    #region ListView Events
    private void operatorsListView_SelectedIndexChanged(object sender, EventArgs e) {
      removeButton.Enabled = false;
      if (operatorsListView.SelectedItems.Count > 0) {
        removeButton.Enabled = true;
      }
    }
    private void operatorsListView_DoubleClick(object sender, EventArgs e) {
      if (operatorsListView.SelectedItems.Count == 1) {
        IOperator op = (IOperator)operatorsListView.SelectedItems[0].Tag;
        IView view = (IView)MainFormManager.CreateDefaultView(op);
        if (view != null)
          PluginManager.ControlManager.ShowControl(view);
      }
    }
    #endregion

    #region TreeView Events
    private void graphTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
      TreeNode node = e.Node;
      IOperator op = (IOperator)node.Tag;
      for (int i = 0; i < node.Nodes.Count; i++) {
        if (node.Nodes[i].Tag == null) {
          node.Nodes[i].Remove();
          node.Nodes.Insert(i, CreateTreeNode(op.SubOperators[i]));
        }
      }
    }
    private void graphTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      IOperator op = (IOperator)e.Node.Tag;
      foreach (ListViewItem item in operatorsListView.Items)
        item.Selected = item.Tag == op;
    }
    #endregion

    #region Size Changed Events
    private void operatorsListView_SizeChanged(object sender, EventArgs e) {
      if (operatorsListView.Columns.Count > 0)
        operatorsListView.Columns[0].Width = Math.Max(0, operatorsListView.Width - 25);
    }
    #endregion

    #region Key Events
    private void operatorsListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if (operatorsListView.SelectedItems.Count > 0) {
          foreach (ListViewItem item in operatorsListView.SelectedItems)
            OperatorGraph.RemoveOperator(((IOperator)item.Tag).Guid);
        }
      }
      if (e.KeyCode == Keys.F2) {
        if (operatorsListView.SelectedItems.Count == 1)
          operatorsListView.SelectedItems[0].BeginEdit();
      }
    }
    private void graphTreeView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        TreeNode node = graphTreeView.SelectedNode;
        if ((node != null) && (node.Parent != null)) {
          IOperator parent = (IOperator)node.Parent.Tag;
          parent.RemoveSubOperator(node.Index);
        }
      }
    }
    #endregion

    #region Edit Events
    private void operatorsListView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
      e.CancelEdit = false;

      string name = e.Label;
      if (name != null) {
        IOperator op = (IOperator)operatorsListView.Items[e.Item].Tag;
        op.Name = name;
      }
    }
    #endregion

    #region Button Events
    private void addOperatorButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK)
        OperatorGraph.AddOperator(chooseOperatorDialog.Operator);
    }
    private void removeButton_Click(object sender, EventArgs e) {
      if (operatorsListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in operatorsListView.SelectedItems)
          OperatorGraph.RemoveOperator(((IOperator)item.Tag).Guid);
      }
    }
    #endregion

    #region Drag and Drop Events
    private void operatorsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      ListViewItem item = (ListViewItem)e.Item;
      IOperator op = (IOperator)item.Tag;
      DataObject data = new DataObject();
      data.SetData("IOperator", op);
      data.SetData("DragSource", operatorsListView);
      DoDragDrop(data, DragDropEffects.Copy);
    }
    private void operatorsListView_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetDataPresent("IOperator"))
        e.Effect = DragDropEffects.Copy;
    }
    private void operatorsListView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetDataPresent("IOperator"))
        e.Effect = DragDropEffects.Copy;
    }
    private void operatorsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        if (e.Data.GetDataPresent("IOperator")) {
          IOperator op = (IOperator)e.Data.GetData("IOperator");
          op = (IOperator)op.Clone();
          OperatorGraph.AddOperator(op);
        }
      }
    }
    private void graphTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode node = (TreeNode)e.Item;
      if (node.Parent != null) {
        IOperator op = (IOperator)node.Tag;
        IOperator parent = (IOperator)node.Parent.Tag;
        int index = node.Index;
        DataObject data = new DataObject();
        data.SetData("IOperator", op);
        data.SetData("DragSource", graphTreeView);
        data.SetData("ParentOperator", parent);
        data.SetData("Index", index);
        DoDragDrop(data, DragDropEffects.Move);
      }
    }
    private void graphTreeView_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetDataPresent("IOperator")) {
        Point p = graphTreeView.PointToClient(new Point(e.X, e.Y));
        TreeNode node = graphTreeView.GetNodeAt(p);
        if (node != null) {
          if ((e.Data.GetDataPresent("ParentOperator")) && (node.Parent != null)) {
            if ((e.Data.GetDataPresent("DragSource")) && (e.Data.GetData("DragSource") == graphTreeView))
              e.Effect = DragDropEffects.Move;
          } else {
            if ((e.Data.GetDataPresent("DragSource")) && (e.Data.GetData("DragSource") == operatorsListView))
              e.Effect = DragDropEffects.Copy;
          }
        }
      }
    }
    private void graphTreeView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetDataPresent("IOperator")) {
        Point p = graphTreeView.PointToClient(new Point(e.X, e.Y));
        TreeNode node = graphTreeView.GetNodeAt(p);
        if (node != null) {
          if ((e.Data.GetDataPresent("ParentOperator")) && (node.Parent != null)) {
            if ((e.Data.GetDataPresent("DragSource")) && (e.Data.GetData("DragSource") == graphTreeView))
              e.Effect = DragDropEffects.Move;
          } else {
            if ((e.Data.GetDataPresent("DragSource")) && (e.Data.GetData("DragSource") == operatorsListView))
              e.Effect = DragDropEffects.Copy;
          }
        }
      }
    }
    private void graphTreeView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        if (e.Data.GetDataPresent("IOperator")) {
          IOperator op = (IOperator)e.Data.GetData("IOperator");
          Point p = graphTreeView.PointToClient(new Point(e.X, e.Y));
          TreeNode node = graphTreeView.GetNodeAt(p);
          if (e.Data.GetDataPresent("ParentOperator")) {
            if (node.Parent != null) {
              TreeNode parentNode = node.Parent;
              IOperator oldParent = (IOperator)e.Data.GetData("ParentOperator");
              int oldIndex = (int)e.Data.GetData("Index");
              IOperator newParent = (IOperator)node.Parent.Tag;
              int newIndex = node.Index;
              ICollection<IConstraint> violatedConstraints;
              ICollection<IConstraint> violatedConstraints2;
              oldParent.TryRemoveSubOperator(oldIndex, out violatedConstraints);
              newParent.TryAddSubOperator(op, newIndex, out violatedConstraints2);
              if ((violatedConstraints.Count == 0) && (violatedConstraints2.Count == 0)) {
                graphTreeView.SelectedNode = parentNode.Nodes[newIndex];
              } else {
                List<IConstraint> allViolatedConstraints = new List<IConstraint>(violatedConstraints);
                allViolatedConstraints.AddRange(violatedConstraints2);
                if (Auxiliary.ShowIgnoreConstraintViolationMessageBox(allViolatedConstraints) == DialogResult.Yes) {
                  if (violatedConstraints.Count > 0)
                    oldParent.RemoveSubOperator(oldIndex);
                  if (violatedConstraints2.Count > 0)
                    newParent.AddSubOperator(op, newIndex);
                  graphTreeView.SelectedNode = parentNode.Nodes[newIndex];
                } else {
                  if (violatedConstraints.Count == 0)
                    oldParent.AddSubOperator(op, oldIndex);
                  if (violatedConstraints2.Count == 0)
                    newParent.RemoveSubOperator(newIndex);
                }
              }
            }
          } else {
            if (node != null) {
              IOperator parent = (IOperator)node.Tag;
              ICollection<IConstraint> violatedConstraints;
              if (parent.TryAddSubOperator(op, out violatedConstraints)) {
                graphTreeView.SelectedNode = node.Nodes[node.Nodes.Count - 1];
              } else if (Auxiliary.ShowIgnoreConstraintViolationMessageBox(violatedConstraints) == DialogResult.Yes) {
                parent.AddSubOperator(op);
                graphTreeView.SelectedNode = node.Nodes[node.Nodes.Count - 1];
              }
            }
          }
        }
      }
    }
    #endregion

    #region Context Menu Events
    private void operatorsContextMenuStrip_Opening(object sender, CancelEventArgs e) {
      viewToolStripMenuItem.Enabled = false;
      initialOperatorToolStripMenuItem.Enabled = false;
      initialOperatorToolStripMenuItem.Checked = false;
      if (operatorsListView.SelectedItems.Count == 1) {
        IOperator op = (IOperator)operatorsListView.SelectedItems[0].Tag;
        IView view = (IView)MainFormManager.CreateDefaultView(op);
        if (view != null) {
          viewToolStripMenuItem.Enabled = true;
          viewToolStripMenuItem.Tag = view;
        }
        initialOperatorToolStripMenuItem.Enabled = true;
        initialOperatorToolStripMenuItem.Tag = op;
        if (op == OperatorGraph.InitialOperator)
          initialOperatorToolStripMenuItem.Checked = true;
      }
    }
    private void viewToolStripMenuItem_Click(object sender, EventArgs e) {
      IView view = (IView)((ToolStripMenuItem)sender).Tag;
      PluginManager.ControlManager.ShowControl(view);
    }
    private void initialOperatorToolStripMenuItem_Click(object sender, EventArgs e) {
      if (initialOperatorToolStripMenuItem.Checked) {
        foreach (ListViewItem item in operatorsListView.Items)
          item.Font = operatorsListView.Font;
        operatorsListView.SelectedItems[0].Font = new Font(operatorsListView.Font, FontStyle.Bold);
        OperatorGraph.InitialOperator = (IOperator)initialOperatorToolStripMenuItem.Tag;
      } else {
        operatorsListView.SelectedItems[0].Font = operatorsListView.Font;
        OperatorGraph.InitialOperator = null;
      }
    }
    private void graphContextMenuStrip_Opening(object sender, CancelEventArgs e) {
      viewToolStripMenuItem1.Enabled = false;
      breakpointToolStripMenuItem.Enabled = false;
      breakpointToolStripMenuItem.Checked = false;
      if (graphTreeView.SelectedNode != null) {
        IOperator op = (IOperator)graphTreeView.SelectedNode.Tag;
        IView view = (IView)MainFormManager.CreateDefaultView(op);
        if (view != null) {
          viewToolStripMenuItem1.Enabled = true;
          viewToolStripMenuItem1.Tag = view;
        }
        breakpointToolStripMenuItem.Enabled = true;
        breakpointToolStripMenuItem.Tag = op;
        if (op.Breakpoint)
          breakpointToolStripMenuItem.Checked = true;
      }
    }
    private void breakpointToolStripMenuItem_Click(object sender, EventArgs e) {
      IOperator op = (IOperator)breakpointToolStripMenuItem.Tag;
      op.Breakpoint = breakpointToolStripMenuItem.Checked;
    }
    #endregion

    #region OperatorGraph Events
    private void OperatorGraph_OperatorAdded(object sender, EventArgs<IOperator> e) {
      operatorsListView.Items.Add(CreateListViewItem(e.Value));
    }
    private void OperatorGraph_OperatorRemoved(object sender, EventArgs<IOperator> e) {
      ListViewItem itemToDelete = null;
      foreach (ListViewItem item in operatorsListView.Items) {
        if (item.Tag == e.Value)
          itemToDelete = item;
      }
      itemToDelete.Remove();
    }
    private void OperatorGraph_InitialOperatorChanged(object sender, EventArgs e) {
      Refresh();
    }
    #endregion

    #region Operator Events
    private void Operator_NameChanged(object sender, EventArgs e) {
      IOperator op = (IOperator)sender;
      foreach (TreeNode node in operatorNodeTable[op])
        node.Text = op.Name;
    }
    private void Operator_BreakpointChanged(object sender, EventArgs e) {
      IOperator op = (IOperator)sender;
      foreach (TreeNode node in operatorNodeTable[op]) {
        if (op.Breakpoint)
          node.ForeColor = Color.Red;
        else
          node.ForeColor = graphTreeView.ForeColor;
      }
    }
    private void Operator_SubOperatorAdded(object sender, EventArgs<IOperator, int> e) {
      IOperator op = (IOperator)sender;
      if (operatorNodeTable.ContainsKey(op)) {
        TreeNode[] nodes = new TreeNode[operatorNodeTable[op].Count];
        operatorNodeTable[op].CopyTo(nodes, 0);
        foreach (TreeNode node in nodes)
          node.Nodes.Insert(e.Value2, CreateTreeNode(e.Value));
      }
    }
    private void Operator_SubOperatorRemoved(object sender, EventArgs<IOperator, int> e) {
      IOperator op = (IOperator)sender;
      if (operatorNodeTable.ContainsKey(op)) {
        TreeNode[] nodes = new TreeNode[operatorNodeTable[op].Count];
        operatorNodeTable[op].CopyTo(nodes, 0);
        foreach (TreeNode node in nodes) {
          RemoveTreeNode(node.Nodes[e.Value2]);
          node.Nodes.RemoveAt(e.Value2);
        }
      }
    }
    #endregion

    #region Mouse Events
    private void graphTreeView_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button != MouseButtons.Right) return;
      TreeNode clickedNode = graphTreeView.GetNodeAt(e.X, e.Y);
      if (clickedNode != null) {
        graphTreeView.SelectedNode = clickedNode;
        graphTreeView.Refresh();
      }
    }
    #endregion

  }
}
