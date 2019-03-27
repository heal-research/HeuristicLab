#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  [View("Symbolic Expression Grammar Editor")]
  [Content(typeof(ISymbolicExpressionGrammar), true)]
  public partial class SymbolicExpressionGrammarEditorView : NamedItemView {
    public SymbolicExpressionGrammarEditorView() {
      InitializeComponent();
    }

    public override bool ReadOnly {
      get {
        if ((Content != null) && Content.ReadOnly) return true;
        return base.ReadOnly;
      }
      set {
        if ((Content != null) && Content.ReadOnly) base.ReadOnly = true;
        else base.ReadOnly = value;
      }
    }

    public new ISymbolicExpressionGrammar Content {
      get { return (ISymbolicExpressionGrammar)base.Content; }
      set { base.Content = value; }
    }

    private Color treeViewBackColor = Color.Empty;
    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (Content == null || Content.ReadOnly || ReadOnly || Locked) {
        addButton.Enabled = false;
        removeButton.Enabled = false;
        copyButton.Enabled = false;
        treeViewBackColor = symbolsTreeView.BackColor;
        symbolsTreeView.BackColor = Color.FromArgb(255, 240, 240, 240);
      } else {
        addButton.Enabled = true;
        if (symbolsTreeView.SelectedNode != null && !(symbolsTreeView.SelectedNode.Tag is IReadOnlySymbol)) {
          removeButton.Enabled = true;
          copyButton.Enabled = true;
        }
        treeViewBackColor = Color.Empty;
        symbolsTreeView.BackColor = treeViewBackColor;
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        symbolsTreeView.Nodes.Clear();
        UpdateSymbolsTreeView();

        symbolsTreeView.CollapseAll();
        foreach (var node in IterateTreeNodes())
          if (node.Checked) node.Expand();

        //mkommend: scrolls to the top node
        symbolsTreeView.Nodes[0].EnsureVisible();

        allowedChildSymbolsControl.Grammar = Content;
        allowedChildSymbolsControl.Symbol = null;
        symbolDetailsViewHost.Content = null;
      } else {
        symbolsTreeView.Nodes.Clear();
        allowedChildSymbolsControl.Grammar = null;
        symbolDetailsViewHost.Content = null;
      }
    }

    #region events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ReadOnlyChanged += new System.EventHandler(Content_ReadOnlyChanged);
      Content.Changed += new System.EventHandler(Content_Changed);
    }
    protected override void DeregisterContentEvents() {
      Content.ReadOnlyChanged -= new System.EventHandler(Content_ReadOnlyChanged);
      Content.Changed -= new System.EventHandler(Content_Changed);
      base.DeregisterContentEvents();
    }

    private void Content_ReadOnlyChanged(object sender, EventArgs e) {
      ReadOnly = Content.ReadOnly;
    }

    private void Content_Changed(object sender, EventArgs e) {
      ISymbol symbol = null;
      if (symbolsTreeView.SelectedNode != null)
        symbol = (ISymbol)symbolsTreeView.SelectedNode.Tag;

      allowedChildSymbolsControl.Grammar = Content;

      UpdateSymbolsTreeView();
      if (symbol != null && Content.ContainsSymbol(symbol)) {
        symbolsTreeView.SelectedNode = IterateTreeNodes().Where(n => n.Tag == symbol).ToList().FirstOrDefault();
        UpdateSymbolDetailsViews();
      }
    }
    #endregion

    private void UpdateSymbolsTreeView() {
      var symbols = Content.Symbols.ToList();
      foreach (var treeNode in IterateTreeNodes().ToList()) {
        var symbol = treeNode.Tag as ISymbol;
        if (!symbols.Contains(symbol))
          treeNode.Remove();
      }

      var groupSymbols = symbols.OfType<GroupSymbol>().ToList();
      var topLevelSymbols = Content.Symbols.Where(s => !groupSymbols.Any(g => g.Symbols.Contains(s)));
      UpdateChildTreeNodes(symbolsTreeView.Nodes, topLevelSymbols);
      RebuildImageList();
    }

    private void UpdateChildTreeNodes(TreeNodeCollection collection, IEnumerable<ISymbol> symbols) {
      foreach (ISymbol symbol in symbols) {
        if (symbol is ProgramRootSymbol) continue;
        if (symbol is Defun) continue;

        TreeNode node = collection.Cast<TreeNode>().Where(n => n.Tag == symbol).FirstOrDefault();
        if (node == null) {
          node = new TreeNode();
          node.Tag = symbol;
          collection.Add(node);
        }
        node.Checked = symbol.Enabled;
        node.Text = symbol.Name;

        var groupSymbol = symbol as GroupSymbol;
        if (groupSymbol != null) UpdateChildTreeNodes(node.Nodes, groupSymbol.Symbols);
      }
    }

    private void symbolsTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if (e.Action != TreeViewAction.Unknown) UpdateSymbolDetailsViews();
      SetEnabledStateOfControls();
    }

    private void symbolsTreeView_BeforeCheck(object sender, TreeViewCancelEventArgs e) {
      if (e.Action == TreeViewAction.Unknown) return;
      e.Cancel = Content == null || Content.ReadOnly || ReadOnly || Locked;
    }

    private void symbolsTreeView_AfterCheck(object sender, TreeViewEventArgs e) {
      if (e.Action == TreeViewAction.Unknown) return;
      Content.StartGrammarManipulation();
      allowedChildSymbolsControl.Symbol = null;
      var symbol = (ISymbol)e.Node.Tag;
      symbol.Enabled = e.Node.Checked;
      foreach (var node in IterateTreeNodes())
        node.Checked = ((ISymbol)node.Tag).Enabled;

      Content.FinishedGrammarManipulation();
    }

    #region drag & drop operations
    private void symbolsTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (!Locked) {
        var treeNode = e.Item as TreeNode;
        var data = new DataObject();
        data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, treeNode.Tag);
        validDragOperation = true;
        DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move);
      }
    }


    private bool validDragOperation;
    private void symbolsTreeView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (Content == null || Content.ReadOnly || ReadOnly || Locked) return;

      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      var symbol = data as ISymbol;
      if (symbol != null && !(symbol is IReadOnlySymbol)) validDragOperation = true;
    }
    private void symbolsTreeView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (validDragOperation) {
        GroupSymbol groupSymbol = null;
        Point mouse = symbolsTreeView.PointToClient(new Point(e.X, e.Y));
        TreeNode node = symbolsTreeView.GetNodeAt(mouse);
        if (node == null) return;
        groupSymbol = node.Tag as GroupSymbol;
        if (groupSymbol == null) return;
        var symbol = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        if (symbol == groupSymbol) return;

        if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
      }
    }
    private void symbolsTreeView_DragDrop(object sender, DragEventArgs e) {
      var symbol = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as ISymbol;

      GroupSymbol groupSymbol = null;
      Point mouse = symbolsTreeView.PointToClient(new Point(e.X, e.Y));
      TreeNode node = symbolsTreeView.GetNodeAt(mouse);
      if (node != null) groupSymbol = node.Tag as GroupSymbol;
      if (node != null && groupSymbol == null) groupSymbol = node.Parent.Tag as GroupSymbol;

      Content.StartGrammarManipulation();
      Cloner cloner = new Cloner();
      var clonedSymbol = cloner.Clone(symbol);
      ChangeDuplicateSymbolNames(clonedSymbol);

      if (groupSymbol != null) groupSymbol.SymbolsCollection.Add(clonedSymbol);
      else Content.AddSymbol(clonedSymbol);

      UpdateGrammerConstraintsForClonedSymbol(symbol, cloner);
      Content.FinishedGrammarManipulation();
    }
    #endregion

    private void symbolsTreeView_MouseDown(object sender, MouseEventArgs e) {
      // enables deselection of treeNodes
      Point coordinates = new Point(e.X, e.Y);
      TreeNode node = symbolsTreeView.GetNodeAt(coordinates);
      if (e.Button == MouseButtons.Left && node == null) {
        symbolsTreeView.SelectedNode = null;
        symbolDetailsViewHost.Content = null;
        SetEnabledStateOfControls();
      }
    }

    private void symbolsTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
      var symbol = e.Node.Tag as ISymbol;
      if (symbol == null) return;
      if (e.Button != MouseButtons.Left) return;
      if (e.X < e.Node.Bounds.Left - symbolsTreeView.ImageList.Images[e.Node.ImageIndex].Width || e.X > e.Node.Bounds.Right) return;
      MainFormManager.MainForm.ShowContent(symbol);
      e.Node.Toggle();
    }

    private void symbolsTreeView_KeyDown(object sender, KeyEventArgs e) {
      if (Content == null || Content.ReadOnly || ReadOnly || Locked) return;
      if (symbolsTreeView.SelectedNode == null) return;
      if (e.KeyCode != Keys.Delete) return;

      var symbol = (ISymbol)symbolsTreeView.SelectedNode.Tag;
      if (!(symbol is IReadOnlySymbol))
        Content.RemoveSymbol(symbol);

      SetEnabledStateOfControls();
      UpdateSymbolDetailsViews();
      RebuildImageList();
    }

    #region button events
    private TypeSelectorDialog typeSelectorDialog;
    private void addButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Symbol";
        typeSelectorDialog.TypeSelector.Caption = "Available Symbols";
        typeSelectorDialog.TypeSelector.Configure(typeof(ISymbol), false, false, (t) => { return !typeof(IReadOnlySymbol).IsAssignableFrom(t); });
      }
      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          ISymbol symbol = (ISymbol)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
          ChangeDuplicateSymbolNames(symbol);
          GroupSymbol groupSymbol = null;

          TreeNode selectedNode = symbolsTreeView.SelectedNode;
          if (selectedNode != null) {
            groupSymbol = selectedNode.Tag as GroupSymbol;
            if (groupSymbol == null && selectedNode.Parent != null) groupSymbol = selectedNode.Parent.Tag as GroupSymbol;
          }
          if (groupSymbol != null) groupSymbol.SymbolsCollection.Add(symbol);
          else Content.AddSymbol(symbol);
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    private void copyButton_Click(object sender, EventArgs e) {
      var symbol = symbolsTreeView.SelectedNode.Tag as ISymbol;
      if (symbol != null && !(symbol is IReadOnlySymbol)) {

        Content.StartGrammarManipulation();
        Cloner cloner = new Cloner();
        var clonedSymbol = cloner.Clone(symbol);
        ChangeDuplicateSymbolNames(clonedSymbol);

        GroupSymbol groupSymbol = null;
        if (symbolsTreeView.SelectedNode.Parent != null) groupSymbol = symbolsTreeView.SelectedNode.Parent.Tag as GroupSymbol;

        if (groupSymbol != null) groupSymbol.SymbolsCollection.Add(clonedSymbol);
        else Content.AddSymbol(clonedSymbol);

        UpdateGrammerConstraintsForClonedSymbol(symbol, cloner);
        Content.FinishedGrammarManipulation();
      }
    }

    private void removeButton_Click(object sender, EventArgs e) {
      var symbol = symbolsTreeView.SelectedNode.Tag as ISymbol;
      if (symbol != null && !(symbol is IReadOnlySymbol)) {
        Content.RemoveSymbol(symbol);
      }
    }

    private void showDetailsCheckBox_CheckedChanged(object sender, EventArgs e) {
      splitContainer1.Panel2Collapsed = !showDetailsCheckBox.Checked;
    }

    private void showSampleTreeButton_Click(object sender, EventArgs e) {
      SymbolicExpressionGrammarSampleExpressionTreeView view = new SymbolicExpressionGrammarSampleExpressionTreeView();
      view.Content = Content;
      view.Show();
    }

    #endregion

    #region helpers
    private void UpdateGrammerConstraintsForClonedSymbol(ISymbol symbol, Cloner cloner) {
      foreach (var s in symbol.Flatten().Where(x => !(x is GroupSymbol))) {
        if (!cloner.ClonedObjectRegistered(s)) throw new InvalidOperationException();
        var clone = cloner.Clone(s);
        Content.SetSubtreeCount(clone, Content.GetMinimumSubtreeCount(s), Content.GetMaximumSubtreeCount(s));
        foreach (var childSymbol in Content.GetAllowedChildSymbols(s)) {
          var newChildSymbol = childSymbol;
          if (cloner.ClonedObjectRegistered(childSymbol)) newChildSymbol = cloner.Clone(childSymbol);
          Content.AddAllowedChildSymbol(clone, newChildSymbol);
        }
        for (int i = 0; i < Content.GetMaximumSubtreeCount(s); i++) {
          foreach (var childSymbol in Content.GetAllowedChildSymbols(s, i)) {
            var newChildSymbol = childSymbol;
            if (cloner.ClonedObjectRegistered(childSymbol)) newChildSymbol = cloner.Clone(childSymbol);
            Content.AddAllowedChildSymbol(clone, newChildSymbol, i);
          }
        }
      }
    }

    private void ChangeDuplicateSymbolNames(ISymbol symbol) {
      foreach (var s in symbol.Flatten()) {
        var originalSymbolName = s.Name;
        int i = 1;
        while (Content.ContainsSymbol(s)) {
          s.Name = originalSymbolName + i;
          i++;
        }
      }
    }

    private void UpdateSymbolDetailsViews() {
      if (symbolsTreeView.SelectedNode != null) {
        symbolDetailsViewHost.Content = (ISymbol)symbolsTreeView.SelectedNode.Tag;
        allowedChildSymbolsControl.Symbol = (ISymbol)symbolsTreeView.SelectedNode.Tag;
      } else {
        symbolDetailsViewHost.Content = null;
        allowedChildSymbolsControl.Symbol = null;
      }
    }

    private IEnumerable<TreeNode> IterateTreeNodes(TreeNode node = null) {
      TreeNodeCollection nodes;
      if (node == null)
        nodes = symbolsTreeView.Nodes;
      else {
        nodes = node.Nodes;
        yield return node;
      }

      foreach (var childNode in nodes.OfType<TreeNode>())
        foreach (var n in IterateTreeNodes(childNode))
          yield return n;
    }

    protected virtual void RebuildImageList() {
      symbolsTreeView.ImageList.Images.Clear();
      foreach (TreeNode treeNode in IterateTreeNodes()) {
        var symbol = (ISymbol)treeNode.Tag;
        symbolsTreeView.ImageList.Images.Add(symbol == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : symbol.ItemImage);
        treeNode.ImageIndex = symbolsTreeView.ImageList.Images.Count - 1;
      }
    }

    //necessary code to handle dock correctly regarding the expanded nodes
    bool[] expandendedState;
    protected override void OnHandleCreated(EventArgs e) {
      base.OnHandleCreated(e);
      if (expandendedState == null) return;
      var nodes = IterateTreeNodes().ToList();
      for (int i = 0; i < nodes.Count; i++)
        if (expandendedState[i]) nodes[i].Expand();
    }
    protected override void OnHandleDestroyed(EventArgs e) {
      base.OnHandleDestroyed(e);
      var nodes = IterateTreeNodes().ToList();
      expandendedState = new bool[nodes.Count];
      for (int i = 0; i < nodes.Count; i++)
        expandendedState[i] = nodes[i].IsExpanded;
    }
    #endregion
  }

  //this class is necessary to prevent double clicks which do not fire the checkbox checked event
  //workaround taken from http://connect.microsoft.com/VisualStudio/feedback/details/374516/treeview-control-does-not-fire-events-reliably-when-double-clicking-on-checkbox
  internal class CheckBoxTreeView : TreeView {
    protected override void WndProc(ref Message m) {
      // Suppress WM_LBUTTONDBLCLK
      if (m.Msg == 0x203 && IsOnCheckBox(m)) { m.Result = IntPtr.Zero; } else base.WndProc(ref m);
    }

    private int GetXLParam(IntPtr lParam) {
      return lParam.ToInt32() & 0xffff;
    }

    private int GetYLParam(IntPtr lParam) {
      return lParam.ToInt32() >> 16;
    }

    private bool IsOnCheckBox(Message m) {
      int x = GetXLParam(m.LParam);
      int y = GetYLParam(m.LParam);
      TreeNode node = this.GetNodeAt(x, y);
      return ((x <= node.Bounds.Left - 20) && (x >= node.Bounds.Left - 32));
    }
  }
}
