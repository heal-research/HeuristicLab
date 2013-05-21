#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  internal delegate void
  ModifyTree(ISymbolicExpressionTree tree, ISymbolicExpressionTreeNode node, ISymbolicExpressionTreeNode oldChild, ISymbolicExpressionTreeNode newChild, bool removeSubtree = true);

  internal sealed partial class InteractiveSymbolicExpressionTreeChart : SymbolicExpressionTreeChart {
    private ISymbolicExpressionTreeNode tempNode; // node in clipboard (to be cut/copy/pasted etc)
    private VisualSymbolicExpressionTreeNode currSelected; // currently selected node
    private enum EditOp { NoOp, CopySubtree, CutSubtree }
    private EditOp lastOp = EditOp.NoOp;

    // delegate to notify the parent container (the view) about the tree edit operations that it needs to perform
    public ModifyTree ModifyTree { get; set; }

    public InteractiveSymbolicExpressionTreeChart() {
      InitializeComponent();
      currSelected = null;
      tempNode = null;
    }

    private void contextMenuStrip_Opened(object sender, EventArgs e) {
      var menuStrip = (ContextMenuStrip)sender;
      var point = menuStrip.SourceControl.PointToClient(Cursor.Position);
      var ea = new MouseEventArgs(MouseButtons.Left, 1, point.X, point.Y, 0);
      var visualNode = FindVisualSymbolicExpressionTreeNodeAt(ea.X, ea.Y);
      if (visualNode != null) {
        OnSymbolicExpressionTreeNodeClicked(visualNode, ea);
      } else {
        currSelected = null;
      }

      if (currSelected == null) {
        insertNodeToolStripMenuItem.Visible = false;
        changeNodeToolStripMenuItem.Visible = false;
        copyToolStripMenuItem.Visible = false;
        cutToolStripMenuItem.Visible = false;
        removeToolStripMenuItem.Visible = false;
        pasteToolStripMenuItem.Visible = false;
      } else {
        var node = currSelected.SymbolicExpressionTreeNode;
        insertNodeToolStripMenuItem.Visible = true;
        changeNodeToolStripMenuItem.Visible = true;
        changeNodeToolStripMenuItem.Enabled = (node is SymbolicExpressionTreeTerminalNode);
        insertNodeToolStripMenuItem.Enabled = !changeNodeToolStripMenuItem.Enabled;
        copyToolStripMenuItem.Visible = true;
        cutToolStripMenuItem.Visible = true;
        removeToolStripMenuItem.Visible = true;

        pasteToolStripMenuItem.Visible = true;
        pasteToolStripMenuItem.Enabled = tempNode != null && insertNodeToolStripMenuItem.Enabled
                                                          && !(lastOp == EditOp.CutSubtree
                                                               && tempNode.IterateNodesBreadth().Contains(currSelected.SymbolicExpressionTreeNode));
      }
    }

    protected override void OnSymbolicExpressionTreeNodeClicked(object sender, MouseEventArgs e) {
      currSelected = (VisualSymbolicExpressionTreeNode)sender; ;
      if (currSelected != null) {
        currSelected.LineColor = Color.FromArgb(130, currSelected.LineColor);
        RepaintNode(currSelected);
      }
      base.OnSymbolicExpressionTreeNodeClicked(sender, e);
    }

    protected override void SymbolicExpressionTreeChart_MouseClick(object sender, MouseEventArgs e) {
      if (currSelected != null) {
        currSelected.LineColor = Color.FromArgb(255, currSelected.LineColor);
        RepaintNode(currSelected);
      }
      base.SymbolicExpressionTreeChart_MouseClick(sender, e);
    }

    protected override void OnSymbolicExpressionTreeNodeDoubleClicked(object sender, MouseEventArgs e) {
      currSelected = null;
      base.OnSymbolicExpressionTreeNodeDoubleClicked(sender, e);
    }

    private void insertNodeToolStripMenuItem_Click(object sender, EventArgs e) {
      if (currSelected == null || currSelected.SymbolicExpressionTreeNode is SymbolicExpressionTreeTerminalNode) return;
      var parent = currSelected.SymbolicExpressionTreeNode;

      using (var dialog = new InsertNodeDialog()) {
        dialog.SetAllowedSymbols(parent.Grammar.AllowedSymbols.Where(s => s.Enabled && s.InitialFrequency > 0.0 && !(s is ProgramRootSymbol || s is StartSymbol || s is Defun)));
        dialog.ShowDialog(this);
        if (dialog.DialogResult != DialogResult.OK) return;

        var symbol = dialog.SelectedSymbol();
        var node = symbol.CreateTreeNode();
        if (node is ConstantTreeNode) {
          var constant = node as ConstantTreeNode;
          constant.Value = double.Parse(dialog.constantValueTextBox.Text);
        } else if (node is VariableTreeNode) {
          var variable = node as VariableTreeNode;
          variable.Weight = double.Parse(dialog.variableWeightTextBox.Text);
          variable.VariableName = dialog.variableNamesCombo.Text;
        } else if (node.Symbol.MinimumArity <= parent.SubtreeCount && node.Symbol.MaximumArity >= parent.SubtreeCount) {
          for (int i = parent.SubtreeCount - 1; i >= 0; --i) {
            var child = parent.GetSubtree(i);
            parent.RemoveSubtree(i);
            node.AddSubtree(child);
          }
        }
        // the if condition will always be true for the final else clause above
        if (parent.Symbol.MaximumArity > parent.SubtreeCount) {
          ModifyTree(Tree, parent, null, node);
        }
      }
      currSelected = null;
    }
    private void changeNodeToolStripMenuItem_Click(object sender, EventArgs e) {
      if (currSelected == null) return;

      var node = (ISymbolicExpressionTreeNode)currSelected.SymbolicExpressionTreeNode.Clone();
      var originalNode = currSelected.SymbolicExpressionTreeNode;

      ISymbolicExpressionTreeNode newNode = null;
      var result = DialogResult.Cancel;
      if (node is ConstantTreeNode) {
        using (var dialog = new ConstantNodeEditDialog(node)) {
          dialog.ShowDialog(this);
          newNode = dialog.NewNode;
          result = dialog.DialogResult;
        }
      } else if (node is VariableTreeNode) {
        using (var dialog = new VariableNodeEditDialog(node)) {
          dialog.ShowDialog(this);
          newNode = dialog.NewNode;
          result = dialog.DialogResult;
        }
      }
      if (result != DialogResult.OK) return;
      ModifyTree(Tree, originalNode.Parent, originalNode, newNode); // this will replace the original node with the new node
      currSelected = null;
    }
    private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.CutSubtree;
      copySubtree();
    }
    private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.CopySubtree;
      copySubtree();
    }
    private void copySubtree() {
      if (tempNode != null) {
        foreach (var subtree in tempNode.IterateNodesPostfix()) {
          var visualNode = GetVisualSymbolicExpressionTreeNode(subtree);
          visualNode.LineColor = Color.FromArgb(255, visualNode.LineColor); // reset the alpha value to 255
          visualNode.TextColor = Color.FromArgb(255, visualNode.TextColor);
          if (subtree.Parent != null) {
            var visualLine = GetVisualSymbolicExpressionTreeNodeConnection(subtree.Parent, subtree);
            visualLine.LineColor = Color.FromArgb(255, visualLine.LineColor);
          }
        }
      }
      tempNode = currSelected.SymbolicExpressionTreeNode;
      foreach (var node in tempNode.IterateNodesPostfix()) {
        var visualNode = GetVisualSymbolicExpressionTreeNode(node);
        visualNode.LineColor = Color.FromArgb(100, visualNode.LineColor);
        visualNode.TextColor = Color.FromArgb(100, visualNode.TextColor);
        foreach (var subtree in node.Subtrees) {
          var visualLine = GetVisualSymbolicExpressionTreeNodeConnection(node, subtree);
          visualLine.LineColor = Color.FromArgb(100, visualLine.LineColor);
        }
      }
      currSelected = null;
      Repaint();
    }
    private void removeNodeToolStripMenuItem_Click(object sender, EventArgs e) {
      var node = currSelected.SymbolicExpressionTreeNode;
      if (node == tempNode) tempNode = null;
      ModifyTree(Tree, node.Parent, node, null, removeSubtree: false);
      currSelected = null; // because the currently selected node was just deleted
    }
    private void removeSubtreeToolStripMenuItem_Click(object sender, EventArgs e) {
      var node = currSelected.SymbolicExpressionTreeNode;
      if (node.IterateNodesPostfix().Contains(tempNode)) tempNode = null;
      ModifyTree(Tree, node.Parent, node, null, removeSubtree: true);
      currSelected = null; // because the currently selected node was just deleted
      contextMenuStrip.Close(); // avoid display of submenus since the action has already been performed
    }
    private void pasteToolStripMenuItem_Clicked(object sender, EventArgs e) {
      if (!(lastOp == EditOp.CopySubtree || lastOp == EditOp.CutSubtree)) return;
      // check if the copied/cut node (stored in the tempNode) can be inserted as a child of the current selected node
      var node = currSelected.SymbolicExpressionTreeNode;
      if (node is ConstantTreeNode || node is VariableTreeNode) return;
      // check if the currently selected node can accept the copied node as a child 
      // no need to check the grammar, an arity check will do just fine here
      if (node.Symbol.MaximumArity <= node.SubtreeCount) return;
      switch (lastOp) {
        case EditOp.CutSubtree: {
            if (tempNode.IterateNodesBreadth().Contains(node))
              throw new ArgumentException();// cannot cut/paste a node into itself
            ModifyTree(Tree, tempNode.Parent, tempNode, null); //remove node from its original parent 	 	 
            ModifyTree(Tree, node, null, tempNode); //insert it as a child to the new parent 
            break;
          }
        case EditOp.CopySubtree: {
            var clone = (SymbolicExpressionTreeNode)tempNode.Clone();
            ModifyTree(Tree, node, null, clone);
            break;
          }
      }
      currSelected = null; // because the tree will have changed
      tempNode = null; // clear the clipboard after one paste
    }
  }
}
