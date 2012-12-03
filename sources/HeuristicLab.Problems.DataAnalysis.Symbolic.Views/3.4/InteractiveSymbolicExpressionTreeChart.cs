#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  public sealed partial class InteractiveSymbolicExpressionTreeChart : SymbolicExpressionTreeChart {
    private ISymbolicExpressionTreeNode tempNode; // node in clipboard (to be cut/copy/pasted etc)
    private VisualSymbolicExpressionTreeNode currSelected; // currently selected node
    private enum EditOp { NoOp, CopyNode, CopySubtree, CutNode, CutSubtree, RemoveNode, RemoveSubtree }
    private enum TreeState { Valid, Invalid }
    private EditOp lastOp = EditOp.NoOp;
    private TreeState treeState = TreeState.Valid; // tree edit operations must leave the tree in a valid state

    private Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> originalNodes; // map a new node to the original node it replaced

    public InteractiveSymbolicExpressionTreeChart() {
      InitializeComponent();
      currSelected = null;
      tempNode = null;

      originalNodes = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>();
    }

    public bool TreeValid { get { return TreeState.Valid == treeState; } }
    // expose an additional event for signaling to the parent view when the tree structure was modified
    // the emitting of the signal is conditional on the tree being valid, otherwise only a Repaint is issued
    public event EventHandler SymbolicExpressionTreeChanged;
    private void OnSymbolicExpressionTreeChanged(object sender, EventArgs e) {
      if (IsValid(Tree)) {
        treeStatusValue.Text = "Valid";
        treeStatusValue.ForeColor = Color.Green;
        treeState = TreeState.Valid;
        var changed = SymbolicExpressionTreeChanged;
        if (changed != null)
          changed(sender, e);
      } else {
        treeStatusValue.Text = "Invalid";
        treeStatusValue.ForeColor = Color.Red;
        treeState = TreeState.Invalid;
        Tree = Tree; // reinitialize the dictionaries and repaint
      }
      foreach (var node in originalNodes.Keys) {
        var visualNode = GetVisualSymbolicExpressionTreeNode(node);
        if (visualNode == null) continue;
        visualNode.LineColor = Color.DodgerBlue;
        RepaintNode(visualNode);
      }
    }

    private void contextMenuStrip_Opened(object sender, EventArgs e) {
      var menuStrip = (ContextMenuStrip)sender;
      var point = menuStrip.SourceControl.PointToClient(Cursor.Position);
      var ea = new MouseEventArgs(MouseButtons.Left, 1, point.X, point.Y, 0);
      InteractiveSymbolicExpressionTreeChart_MouseClick(null, ea);

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
        pasteToolStripMenuItem.Enabled = tempNode != null && insertNodeToolStripMenuItem.Enabled;
      }
    }

    protected override void OnSymbolicExpressionTreeNodeClicked(object sender, MouseEventArgs e) {
      var visualTreeNode = (VisualSymbolicExpressionTreeNode)sender;
      var lastSelected = currSelected;
      if (lastSelected != null) {
        lastSelected.LineColor = originalNodes.ContainsKey(lastSelected.SymbolicExpressionTreeNode) ? Color.DodgerBlue : Color.Black;
        RepaintNode(lastSelected);
      }

      currSelected = visualTreeNode;
      if (currSelected != null) {
        currSelected.LineColor = Color.LightGreen;
        RepaintNode(currSelected);
      }
    }

    protected override void OnSymbolicExpressionTreeNodeDoubleClicked(object sender, MouseEventArgs e) {
      var visualTreeNode = (VisualSymbolicExpressionTreeNode)sender;
      if (originalNodes.ContainsKey(visualTreeNode.SymbolicExpressionTreeNode)) {
        var originalNode = originalNodes[visualTreeNode.SymbolicExpressionTreeNode];

        var parent = visualTreeNode.SymbolicExpressionTreeNode.Parent;
        var i = parent.IndexOfSubtree(visualTreeNode.SymbolicExpressionTreeNode);
        parent.RemoveSubtree(i);
        parent.InsertSubtree(i, originalNode);

        originalNodes.Remove(visualTreeNode.SymbolicExpressionTreeNode);
        visualTreeNode.SymbolicExpressionTreeNode = originalNode;
        OnSymbolicExpressionTreeChanged(sender, EventArgs.Empty);
      } else {
        currSelected = null; // because the tree node will be folded/unfolded 
        base.OnSymbolicExpressionTreeNodeDoubleClicked(sender, e);
        // at this point the tree got redrawn, so we mark the edited nodes
        foreach (var node in originalNodes.Keys) {
          var visualNode = GetVisualSymbolicExpressionTreeNode(node);
          if (visualNode == null) continue;
          visualNode.LineColor = Color.DodgerBlue;
          RepaintNode(visualNode);
        }
      }
    }

    private void insertNodeToolStripMenuItem_Click(object sender, EventArgs e) {
      if (currSelected == null || currSelected.SymbolicExpressionTreeNode is SymbolicExpressionTreeTerminalNode) return;
      var parent = currSelected.SymbolicExpressionTreeNode;

      using (var dialog = new InsertNodeDialog()) {
        dialog.SetAllowedSymbols(parent.Grammar.AllowedSymbols.Where(s => s.Enabled && s.InitialFrequency > 0.0 && !(s is ProgramRootSymbol || s is StartSymbol || s is Defun)));
        dialog.ShowDialog(this);

        if (dialog.DialogResult == DialogResult.OK) {
          var symbol = dialog.SelectedSymbol();
          var node = symbol.CreateTreeNode();
          if (node is ConstantTreeNode) {
            var constant = node as ConstantTreeNode;
            constant.Value = double.Parse(dialog.constantValueTextBox.Text);
          } else if (node is VariableTreeNode) {
            var variable = node as VariableTreeNode;
            variable.Weight = double.Parse(dialog.variableWeightTextBox.Text);
            variable.VariableName = dialog.variableNamesCombo.Text;
          } else {
            if (node.Symbol.MinimumArity <= parent.SubtreeCount && node.Symbol.MaximumArity >= parent.SubtreeCount) {
              for (int i = parent.SubtreeCount - 1; i >= 0; --i) {
                var child = parent.GetSubtree(i);
                parent.RemoveSubtree(i);
                node.AddSubtree(child);
              }
            }
          }
          if (parent.Symbol.MaximumArity > parent.SubtreeCount) {
            parent.AddSubtree(node);
            Tree = Tree;
          }
          OnSymbolicExpressionTreeChanged(sender, e);
          currSelected = null;
        }
      }
    }

    private void changeNodeToolStripMenuItem_Click(object sender, EventArgs e) {
      if (currSelected == null) return;

      ISymbolicExpressionTreeNode node;
      if (originalNodes.ContainsKey(currSelected.SymbolicExpressionTreeNode)) {
        node = currSelected.SymbolicExpressionTreeNode;
      } else {
        node = (ISymbolicExpressionTreeNode)currSelected.SymbolicExpressionTreeNode.Clone();
      }
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
      if (originalNode != newNode) {
        var parent = originalNode.Parent;
        int i = parent.IndexOfSubtree(originalNode);
        parent.RemoveSubtree(i);
        parent.InsertSubtree(i, newNode);
        originalNodes[newNode] = originalNode;
        currSelected.SymbolicExpressionTreeNode = newNode;
      }
      OnSymbolicExpressionTreeChanged(sender, EventArgs.Empty);
    }

    private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.CutSubtree;
      tempNode = currSelected.SymbolicExpressionTreeNode;
      foreach (var node in tempNode.IterateNodesPostfix()) {
        var visualNode = GetVisualSymbolicExpressionTreeNode(node);
        visualNode.LineColor = Color.LightGray;
        visualNode.TextColor = Color.LightGray;
        if (node.SubtreeCount > 0) {
          foreach (var subtree in node.Subtrees) {
            var visualLine = GetVisualSymbolicExpressionTreeNodeConnection(node, subtree);
            visualLine.LineColor = Color.LightGray;
          }
        }
      }
      Repaint();
    }

    private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.CopySubtree;
      tempNode = currSelected.SymbolicExpressionTreeNode;
      foreach (var node in tempNode.IterateNodesPostfix()) {
        var visualNode = GetVisualSymbolicExpressionTreeNode(node);
        visualNode.LineColor = Color.LightGray;
        visualNode.TextColor = Color.LightGray;
        if (node.SubtreeCount <= 0) continue;
        foreach (var subtree in node.Subtrees) {
          var visualLine = GetVisualSymbolicExpressionTreeNodeConnection(node, subtree);
          visualLine.LineColor = Color.LightGray;
        }
      }
      Repaint();
    }

    private void removeNodeToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.RemoveNode;
      var node = currSelected.SymbolicExpressionTreeNode;
      var parent = node.Parent;
      if (parent == null || parent.Symbol is StartSymbol || parent.Symbol is ProgramRootSymbol)
        return; // the operation would result in the deletion of the entire tree
      if (parent.Symbol.MaximumArity >= node.SubtreeCount + parent.SubtreeCount - 1) { // -1 because tempNode will be removed
        parent.RemoveSubtree(parent.IndexOfSubtree(node));
        for (int i = node.SubtreeCount - 1; i >= 0; --i) {
          var child = node.GetSubtree(i);
          node.RemoveSubtree(i);
          parent.AddSubtree(child);
        }
      }
      OnSymbolicExpressionTreeChanged(sender, e);
      currSelected = null; // because the currently selected node was just deleted
    }

    private void removeSubtreeToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.RemoveSubtree;
      var node = currSelected.SymbolicExpressionTreeNode;
      var parent = node.Parent;
      if (parent == null || parent.Symbol is StartSymbol || parent.Symbol is ProgramRootSymbol)
        return; // the operation makes no sense as it would result in the deletion of the entire tree
      parent.RemoveSubtree(parent.IndexOfSubtree(node));
      OnSymbolicExpressionTreeChanged(sender, e);
      currSelected = null; // because the currently selected node was just deleted
    }

    private void pasteToolStripMenuItem_Clicked(object sender, EventArgs e) {
      if (!(lastOp == EditOp.CopyNode || lastOp == EditOp.CopySubtree || lastOp == EditOp.CutNode || lastOp == EditOp.CutSubtree))
        return;
      // check if the copied/cut node (stored in the tempNode) can be inserted as a child of the current selected node
      var node = currSelected.SymbolicExpressionTreeNode;
      if (node is ConstantTreeNode || node is VariableTreeNode) return;
      // check if the currently selected node can accept the copied node as a child 
      // no need to check the grammar, an arity check will do just fine here
      if (node.Symbol.MaximumArity > node.SubtreeCount) {
        switch (lastOp) {
          case (EditOp.CutNode): {
              // when cutting a node from the tree, it's children become children of it's parent
              var parent = tempNode.Parent;
              // arity checks to see if parent can accept node's children (we assume the grammar is already ok with that)
              // (otherise, the 'cut' part of the operation will just not do anything)
              if (parent.Symbol.MaximumArity >= tempNode.SubtreeCount + parent.SubtreeCount - 1) { // -1 because tempNode will be removed
                parent.RemoveSubtree(parent.IndexOfSubtree(tempNode));
                for (int i = tempNode.SubtreeCount - 1; i >= 0; --i) {
                  var child = tempNode.GetSubtree(i);
                  tempNode.RemoveSubtree(i);
                  parent.AddSubtree(child);
                }
                lastOp = EditOp.CopyNode;
              }
              break;
            }
          case (EditOp.CutSubtree): {
              // cut subtree
              var parent = tempNode.Parent;
              parent.RemoveSubtree(parent.IndexOfSubtree(tempNode));
              lastOp = EditOp.CopySubtree; // do this so the next paste will actually perform a copy   
              break;
            }
          case (EditOp.CopyNode): {
              // copy node
              var clone = (SymbolicExpressionTreeNode)tempNode.Clone();
              clone.Parent = tempNode.Parent;
              tempNode = clone;
              for (int i = tempNode.SubtreeCount - 1; i >= 0; --i) tempNode.RemoveSubtree(i);
              break;
            }
          case (EditOp.CopySubtree): {
              // copy subtree
              var clone = (SymbolicExpressionTreeNode)tempNode.Clone();
              clone.Parent = tempNode.Parent;
              tempNode = clone;
              break;
            }
        }
        node.AddSubtree(tempNode);
        Tree = Tree; // hack in order to trigger the reinitialization of the dictionaries after new nodes appeared in the tree
        OnSymbolicExpressionTreeChanged(sender, e);
        currSelected = null; // because the tree changed and was completely redrawn
      }
    }

    private bool IsValid(ISymbolicExpressionTree tree) {
      if (tree.IterateNodesPostfix().Any(node => node.SubtreeCount < node.Symbol.MinimumArity || node.SubtreeCount > node.Symbol.MaximumArity)) {
        treeState = TreeState.Invalid;
        return false;
      }
      treeState = TreeState.Valid;
      return true;
    }

    private void InteractiveSymbolicExpressionTreeChart_MouseClick(object sender, MouseEventArgs e) {
      var visualTreeNode = FindVisualSymbolicExpressionTreeNodeAt(e.X, e.Y);
      if (currSelected != null) {
        currSelected.LineColor = originalNodes.ContainsKey(currSelected.SymbolicExpressionTreeNode) ? Color.DodgerBlue : Color.Black;
        RepaintNode(currSelected);
      }
      currSelected = visualTreeNode;
      if (currSelected != null) {
        currSelected.LineColor = Color.LightGreen;
        RepaintNode(currSelected);
      }
    }
  }
}
