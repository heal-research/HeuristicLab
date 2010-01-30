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
using HeuristicLab.Collections;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of an <see cref="OperatorGraph"/>.
  /// </summary>
  [Content(typeof(IOperator), false)]
  public sealed partial class OperatorTreeView : ItemView {
    private Dictionary<IOperatorParameter, List<TreeNode>> operatorParameterNodeTable;
    private Dictionary<IOperator, List<TreeNode>> operatorNodeTable;
    private Dictionary<IObservableKeyedCollection<string, IParameter>, IOperator> parametersOperatorTable;

    /// <summary>
    /// Gets or sets the operator graph to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new IOperator Content {
      get { return (IOperator)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraphView"/> with caption "Operator Graph".
    /// </summary>
    public OperatorTreeView() {
      InitializeComponent();
      graphTreeView.Sorted = true;
      operatorParameterNodeTable = new Dictionary<IOperatorParameter, List<TreeNode>>();
      operatorNodeTable = new Dictionary<IOperator, List<TreeNode>>();
      parametersOperatorTable = new Dictionary<IObservableKeyedCollection<string, IParameter>, IOperator>();
      Caption = "Operator";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraphView"/> 
    /// with the given <paramref name="operatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="OperatorGraphView()"/>.</remarks>
    /// <param name="operatorGraph">The operator graph to represent visually.</param>
    public OperatorTreeView(IOperator content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (graphTreeView.Nodes.Count > 0)
        RemoveTreeNode(graphTreeView.Nodes[0]);
      graphTreeView.Enabled = false;
      Caption = "Operator";
      if (Content != null) {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        TreeNode root = new TreeNode();
        FillTreeNode(root, Content);
        graphTreeView.Nodes.Add(root);
        graphTreeView.Enabled = true;
      }
    }

    #region TreeNode Management
    private TreeNode CreateTreeNode(IOperatorParameter operatorParameter) {
      TreeNode node = new TreeNode();
      node.Text = operatorParameter.Name + ": ";
      SetOperatorParameterTag(node, operatorParameter);

      if (!operatorParameterNodeTable.ContainsKey(operatorParameter)) {
        operatorParameterNodeTable.Add(operatorParameter, new List<TreeNode>());
        operatorParameter.ValueChanged += new EventHandler(operatorParameter_ValueChanged);
      }
      operatorParameterNodeTable[operatorParameter].Add(node);

      IOperator op = operatorParameter.Value;
      if (op == null)
        node.Text += "-";
      else
        FillTreeNode(node, op);

      return node;
    }
    private void FillTreeNode(TreeNode node, IOperator op) {
      if (!graphTreeView.ImageList.Images.ContainsKey(op.GetType().FullName))
        graphTreeView.ImageList.Images.Add(op.GetType().FullName, op.ItemImage);

      node.Text += op.Name;
      node.ToolTipText = op.ItemName + ": " + op.ItemDescription;
      node.ImageIndex = graphTreeView.ImageList.Images.IndexOfKey(op.GetType().FullName);
      node.SelectedImageIndex = node.ImageIndex;
      SetOperatorTag(node, op);

      if (!operatorNodeTable.ContainsKey(op)) {
        operatorNodeTable.Add(op, new List<TreeNode>());
        op.NameChanged += new EventHandler(op_NameChanged);
        op.BreakpointChanged += new EventHandler(op_BreakpointChanged);
        parametersOperatorTable.Add(op.Parameters, op);
        op.Parameters.ItemsAdded += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsAdded);
        op.Parameters.ItemsRemoved += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsRemoved);
        op.Parameters.ItemsReplaced += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsReplaced);
        op.Parameters.CollectionReset += new CollectionItemsChangedEventHandler<IParameter>(Parameters_CollectionReset);
      }
      operatorNodeTable[op].Add(node);

      if (op.Breakpoint)
        node.ForeColor = Color.Red;

      foreach (IParameter param in op.Parameters) {
        if (param is IOperatorParameter)
          node.Nodes.Add(new TreeNode());
      }
      node.Collapse();
    }
    private void ClearTreeNode(TreeNode node) {
      while (node.Nodes.Count > 0)
        RemoveTreeNode(node.Nodes[0]);

      IOperator op = GetOperatorTag(node);
      if (op != null) {
        operatorNodeTable[op].Remove(node);
        if (operatorNodeTable[op].Count == 0) {
          op.NameChanged -= new EventHandler(op_NameChanged);
          op.BreakpointChanged -= new EventHandler(op_BreakpointChanged);
          operatorNodeTable.Remove(op);
          op.Parameters.ItemsAdded -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsAdded);
          op.Parameters.ItemsRemoved -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsRemoved);
          op.Parameters.ItemsReplaced -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsReplaced);
          op.Parameters.CollectionReset -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_CollectionReset);
          parametersOperatorTable.Remove(op.Parameters);
        }
      }
      SetOperatorTag(node, null);
    }
    private void RemoveTreeNode(TreeNode node) {
      ClearTreeNode(node);

      IOperatorParameter opParam = GetOperatorParameterTag(node);
      if (opParam != null) {
        operatorParameterNodeTable[opParam].Remove(node);
        if (operatorParameterNodeTable[opParam].Count == 0) {
          opParam.ValueChanged -= new EventHandler(operatorParameter_ValueChanged);
          operatorParameterNodeTable.Remove(opParam);
        }
      }
      SetOperatorParameterTag(node, null);
      node.Remove();
    }
    private void AddParameterNodes(IOperator op, IEnumerable<IParameter> parameters) {
      foreach (IParameter param in parameters) {
        IOperatorParameter opParam = param as IOperatorParameter;
        if (opParam != null) {
          foreach (TreeNode node in operatorNodeTable[op])
            node.Nodes.Add(CreateTreeNode(opParam));
        }
      }
    }
    private void RemoveParameterNodes(IEnumerable<IParameter> parameters) {
      foreach (IParameter param in parameters) {
        IOperatorParameter opParam = param as IOperatorParameter;
        if (opParam != null) {
          while (operatorParameterNodeTable.ContainsKey(opParam))
            RemoveTreeNode(operatorParameterNodeTable[opParam][0]);
        }
      }
    }
    #endregion

    #region Parameter and Operator Events
    private void operatorParameter_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(operatorParameter_ValueChanged), sender, e);
      else {
        IOperatorParameter opParam = (IOperatorParameter)sender;
        foreach (TreeNode node in operatorParameterNodeTable[opParam].ToArray())
          ClearTreeNode(node);
        foreach (TreeNode node in operatorParameterNodeTable[opParam]) {
          node.Text = opParam.Name + ": ";
          if (opParam.Value == null)
            node.Text += "-";
          else
            FillTreeNode(node, opParam.Value);
        }
      }
    }
    private void op_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(op_NameChanged), sender, e);
      else {
        IOperator op = (IOperator)sender;
        foreach (TreeNode node in operatorNodeTable[op]) {
          IOperatorParameter opParam = GetOperatorParameterTag(node);
          if (opParam == null)
            node.Text = op.Name + " (" + op.ItemName + ")";
          else
            node.Text = opParam.Name + ": " + op.Name;
        }
      }
    }
    private void op_BreakpointChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(op_BreakpointChanged), sender, e);
      else {
        IOperator op = (IOperator)sender;
        foreach (TreeNode node in operatorNodeTable[op]) {
          if (op.Breakpoint)
            node.ForeColor = Color.Red;
          else
            node.ForeColor = graphTreeView.ForeColor;
        }
      }
    }

    private void Parameters_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsAdded), sender, e);
      else {
        IObservableKeyedCollection<string, IParameter> coll = (IObservableKeyedCollection<string, IParameter>)sender;
        IOperator op = parametersOperatorTable[coll];
        AddParameterNodes(op, e.Items);
      }
    }
    private void Parameters_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsRemoved), sender, e);
      else
        RemoveParameterNodes(e.Items);
    }
    private void Parameters_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsReplaced), sender, e);
      else {
        RemoveParameterNodes(e.Items);
        IObservableKeyedCollection<string, IParameter> coll = (IObservableKeyedCollection<string, IParameter>)sender;
        IOperator op = parametersOperatorTable[coll];
        AddParameterNodes(op, e.Items);
      }
    }
    private void Parameters_CollectionReset(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IParameter>(Parameters_CollectionReset), sender, e);
      else {
        RemoveParameterNodes(e.Items);
        IObservableKeyedCollection<string, IParameter> coll = (IObservableKeyedCollection<string, IParameter>)sender;
        IOperator op = parametersOperatorTable[coll];
        AddParameterNodes(op, e.Items);
      }
    }
    #endregion

    #region TreeView Events
    private void graphTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
      TreeNode node = e.Node;
      if ((node.Nodes.Count > 0) && (node.Nodes[0].Tag == null)) {
        node.Nodes.Clear();
        IOperator op = GetOperatorTag(node);
        foreach (IParameter param in op.Parameters) {
          IOperatorParameter opParam = param as IOperatorParameter;
          if (opParam != null) node.Nodes.Add(CreateTreeNode(opParam));
        }
      }
    }
    private void graphTreeView_MouseDown(object sender, MouseEventArgs e) {
      TreeNode node = graphTreeView.GetNodeAt(e.X, e.Y);
      graphTreeView.SelectedNode = node;
      graphTreeView.Refresh();
    }
    private void graphTreeView_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Delete) && (graphTreeView.SelectedNode != null)) {
        IOperatorParameter opParam = GetOperatorParameterTag(graphTreeView.SelectedNode);
        if (opParam != null) opParam.Value = null;
      }
    }
    private void graphContextMenuStrip_Opening(object sender, CancelEventArgs e) {
      viewToolStripMenuItem.Enabled = false;
      breakpointToolStripMenuItem.Enabled = false;
      breakpointToolStripMenuItem.Checked = false;
      if (graphTreeView.SelectedNode != null) {
        IOperator op = GetOperatorTag(graphTreeView.SelectedNode);
        if (op != null) {
          IView view = MainFormManager.CreateDefaultView(op);
          if (view != null) {
            viewToolStripMenuItem.Enabled = true;
            viewToolStripMenuItem.Tag = view;
          }
          breakpointToolStripMenuItem.Enabled = true;
          breakpointToolStripMenuItem.Tag = op;
          if (op.Breakpoint)
            breakpointToolStripMenuItem.Checked = true;
        }
      }
    }
    private void graphTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode node = (TreeNode)e.Item;
      IOperatorParameter opParam = GetOperatorParameterTag(node);
      IOperator op = GetOperatorTag(node);
      DataObject data = new DataObject();
      data.SetData("Type", op.GetType());
      data.SetData("Value", op);
      if (opParam == null) {
        DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
      } else {
        DragDropEffects action = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
        if ((action & DragDropEffects.Move) == DragDropEffects.Move)
          opParam.Value = null;
      }
    }
    private void graphTreeView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if ((type != null) && (typeof(IOperator).IsAssignableFrom(type))) {
        TreeNode node = graphTreeView.GetNodeAt(graphTreeView.PointToClient(new Point(e.X, e.Y)));
        if ((node != null) && !node.IsExpanded) node.Expand();
        if ((node != null) && (GetOperatorParameterTag(node) != null)) {
          if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Copy;  // CTRL key
          else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
          else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
          else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
          else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
        }
      }
    }
    private void graphTreeView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IOperator op = e.Data.GetData("Value") as IOperator;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) op = (IOperator)op.Clone();
        TreeNode node = graphTreeView.GetNodeAt(graphTreeView.PointToClient(new Point(e.X, e.Y)));
        IOperatorParameter opParam = GetOperatorParameterTag(node);
        opParam.Value = op;
      }
    }
    #endregion

    #region Context Menu Events
    private void viewToolStripMenuItem_Click(object sender, EventArgs e) {
      IView view = ((ToolStripMenuItem)sender).Tag as IView;
      if (view != null) view.Show();
    }
    private void breakpointToolStripMenuItem_Click(object sender, EventArgs e) {
      IOperator op = (IOperator)breakpointToolStripMenuItem.Tag;
      op.Breakpoint = breakpointToolStripMenuItem.Checked;
    }
    #endregion

    #region Helpers
    private class Tuple<T1, T2> {
      public T1 Item1 { get; set; }
      public T2 Item2 { get; set; }

      public Tuple() {
        Item1 = default(T1);
        Item2 = default(T2);
      }
      public Tuple(T1 item1, T2 item2) {
        Item1 = item1;
        Item2 = item2;
      }
    }

    private IOperatorParameter GetOperatorParameterTag(TreeNode node) {
      if (node.Tag != null)
        return ((Tuple<IOperatorParameter, IOperator>)node.Tag).Item1;
      else
        return null;
    }
    private void SetOperatorParameterTag(TreeNode node, IOperatorParameter operatorParameter) {
      if (node.Tag == null)
        node.Tag = new Tuple<IOperatorParameter, IOperator>(operatorParameter, null);
      else
        ((Tuple<IOperatorParameter, IOperator>)node.Tag).Item1 = operatorParameter;
    }
    private IOperator GetOperatorTag(TreeNode node) {
      if (node.Tag != null)
        return ((Tuple<IOperatorParameter, IOperator>)node.Tag).Item2;
      else
        return null;
    }
    private void SetOperatorTag(TreeNode node, IOperator op) {
      if (node.Tag == null)
        node.Tag = new Tuple<IOperatorParameter, IOperator>(null, op);
      else
        ((Tuple<IOperatorParameter, IOperator>)node.Tag).Item2 = op;
    }
    #endregion
  }
}
