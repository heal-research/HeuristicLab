#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of an <see cref="OperatorGraph"/>.
  /// </summary>
  [View("Operator View (Successors)")]
  [Content(typeof(IOperator), false)]
  public sealed partial class OperatorTreeView : ItemView {
    private Dictionary<IValueParameter<IOperator>, List<TreeNode>> opParamNodeTable;
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

    private IOperator selectedOperator;
    public IOperator SelectedOperator {
      get { return selectedOperator; }
      private set {
        if (value != selectedOperator) {
          selectedOperator = value;
          OnSelectedOperatorChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraphView"/> with caption "Operator Graph".
    /// </summary>
    public OperatorTreeView() {
      InitializeComponent();
      graphTreeView.Sorted = true;
      opParamNodeTable = new Dictionary<IValueParameter<IOperator>, List<TreeNode>>();
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

    public event EventHandler SelectedOperatorChanged;
    private void OnSelectedOperatorChanged() {
      if (SelectedOperatorChanged != null)
        SelectedOperatorChanged(this, EventArgs.Empty);
    }

    #region TreeNode Management
    private TreeNode CreateTreeNode(IValueParameter<IOperator> opParam) {
      TreeNode node = new TreeNode();
      node.Text = opParam.Name + ": ";
      SetOperatorParameterTag(node, opParam);

      if (!opParamNodeTable.ContainsKey(opParam)) {
        opParamNodeTable.Add(opParam, new List<TreeNode>());
        opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
      }
      opParamNodeTable[opParam].Add(node);

      FillTreeNode(node, opParam.Value);
      return node;
    }
    private void FillTreeNode(TreeNode node, IOperator op) {
      if (op == null) {
        node.Text += "-";
        node.ToolTipText = "";
        graphTreeView.ImageList.Images.Add(HeuristicLab.Common.Resources.VS2008ImageLibrary.Nothing);
        node.ImageIndex = graphTreeView.ImageList.Images.Count - 1;
        node.SelectedImageIndex = node.ImageIndex;
        node.ForeColor = graphTreeView.ForeColor;
      } else {
        node.Text += op.Name;
        node.ToolTipText = op.ItemName + ": " + op.ItemDescription;
        graphTreeView.ImageList.Images.Add(op.ItemImage);
        node.ImageIndex = graphTreeView.ImageList.Images.Count - 1;
        node.SelectedImageIndex = node.ImageIndex;
        SetOperatorTag(node, op);

        if (!operatorNodeTable.ContainsKey(op)) {
          operatorNodeTable.Add(op, new List<TreeNode>());
          op.ItemImageChanged += new EventHandler(op_ItemImageChanged);
          op.NameChanged += new EventHandler(op_NameChanged);
          op.BreakpointChanged += new EventHandler(op_BreakpointChanged);
          parametersOperatorTable.Add(op.Parameters, op);
          op.Parameters.ItemsAdded += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsAdded);
          op.Parameters.ItemsRemoved += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsRemoved);
          op.Parameters.ItemsReplaced += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsReplaced);
          op.Parameters.CollectionReset += new CollectionItemsChangedEventHandler<IParameter>(Parameters_CollectionReset);
        }
        operatorNodeTable[op].Add(node);

        if (op.Breakpoint) node.ForeColor = Color.Red;
        else node.ForeColor = graphTreeView.ForeColor;

        foreach (IParameter param in op.Parameters) {
          if (param is IValueParameter<IOperator>)
            node.Nodes.Add(new TreeNode());
        }
        node.Collapse();
      }
    }
    private void ClearTreeNode(TreeNode node) {
      while (node.Nodes.Count > 0)
        RemoveTreeNode(node.Nodes[0]);

      if (node.ImageIndex != -1) {
        int i = node.ImageIndex;
        CorrectImageIndexes(graphTreeView.Nodes, i);
        graphTreeView.ImageList.Images.RemoveAt(i);
      }

      IOperator op = GetOperatorTag(node);
      if (op != null) {
        operatorNodeTable[op].Remove(node);
        if (operatorNodeTable[op].Count == 0) {
          op.ItemImageChanged -= new EventHandler(op_ItemImageChanged);
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

      IValueParameter<IOperator> opParam = GetOperatorParameterTag(node);
      if (opParam != null) {
        opParamNodeTable[opParam].Remove(node);
        if (opParamNodeTable[opParam].Count == 0) {
          opParam.ValueChanged -= new EventHandler(opParam_ValueChanged);
          opParamNodeTable.Remove(opParam);
        }
      }
      SetOperatorParameterTag(node, null);
      node.Remove();
    }
    private void AddParameterNodes(IOperator op, IEnumerable<IParameter> parameters) {
      foreach (IParameter param in parameters) {
        IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
        if (opParam != null) {
          foreach (TreeNode node in operatorNodeTable[op])
            node.Nodes.Add(CreateTreeNode(opParam));
        }
      }
    }
    private void RemoveParameterNodes(IEnumerable<IParameter> parameters) {
      foreach (IParameter param in parameters) {
        IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
        if (opParam != null) {
          while (opParamNodeTable.ContainsKey(opParam))
            RemoveTreeNode(opParamNodeTable[opParam][0]);
        }
      }
    }
    #endregion

    #region Parameter and Operator Events
    private void opParam_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(opParam_ValueChanged), sender, e);
      else {
        IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)sender;
        foreach (TreeNode node in opParamNodeTable[opParam].ToArray())
          ClearTreeNode(node);
        foreach (TreeNode node in opParamNodeTable[opParam]) {
          node.Text = opParam.Name + ": ";
          FillTreeNode(node, opParam.Value);
        }
      }
    }
    void op_ItemImageChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(op_ItemImageChanged), sender, e);
      else {
        IOperator op = (IOperator)sender;
        foreach (TreeNode node in operatorNodeTable[op]) {
          int i = node.ImageIndex;
          graphTreeView.ImageList.Images[i] = op.ItemImage;
          node.ImageIndex = -1;
          node.SelectedImageIndex = -1;
          node.ImageIndex = i;
          node.SelectedImageIndex = i;
        }
      }
    }
    private void op_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(op_NameChanged), sender, e);
      else {
        IOperator op = (IOperator)sender;
        foreach (TreeNode node in operatorNodeTable[op]) {
          IValueParameter<IOperator> opParam = GetOperatorParameterTag(node);
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
          IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
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
        IValueParameter<IOperator> opParam = GetOperatorParameterTag(graphTreeView.SelectedNode);
        if (opParam != null) opParam.Value = null;
      }
    }
    private void graphTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      SelectedOperator = graphTreeView.SelectedNode == null ? null : GetOperatorTag(graphTreeView.SelectedNode);
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
      IValueParameter<IOperator> opParam = GetOperatorParameterTag(node);
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
        IValueParameter<IOperator> opParam = GetOperatorParameterTag(node);
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

    private IValueParameter<IOperator> GetOperatorParameterTag(TreeNode node) {
      if (node.Tag != null)
        return ((Tuple<IValueParameter<IOperator>, IOperator>)node.Tag).Item1;
      else
        return null;
    }
    private void SetOperatorParameterTag(TreeNode node, IValueParameter<IOperator> opParam) {
      if (node.Tag == null)
        node.Tag = new Tuple<IValueParameter<IOperator>, IOperator>(opParam, null);
      else
        ((Tuple<IValueParameter<IOperator>, IOperator>)node.Tag).Item1 = opParam;
    }
    private IOperator GetOperatorTag(TreeNode node) {
      if (node.Tag != null)
        return ((Tuple<IValueParameter<IOperator>, IOperator>)node.Tag).Item2;
      else
        return null;
    }
    private void SetOperatorTag(TreeNode node, IOperator op) {
      if (node.Tag == null)
        node.Tag = new Tuple<IValueParameter<IOperator>, IOperator>(null, op);
      else
        ((Tuple<IValueParameter<IOperator>, IOperator>)node.Tag).Item2 = op;
    }

    private void CorrectImageIndexes(TreeNodeCollection nodes, int removedIndex) {
      foreach (TreeNode node in nodes) {
        if (node.ImageIndex > removedIndex) {
          node.ImageIndex--;
          node.SelectedImageIndex--;
        }
        CorrectImageIndexes(node.Nodes, removedIndex);
      }
    }
    #endregion
  }
}
