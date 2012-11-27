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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  public sealed partial class InteractiveSymbolicExpressionTreeChart : SymbolicExpressionTreeChart {
    private ISymbolicExpressionTreeNode tempNode;
    private VisualSymbolicExpressionTreeNode lastSelected; // previously selected node
    private VisualSymbolicExpressionTreeNode currSelected; // currently selected node
    private enum EditOp { NoOp, CopyNode, CopySubtree, CutNode, CutSubtree, DeleteNode, DeleteSubtree }
    private enum TreeState { Valid, Invalid }
    private EditOp lastOp = EditOp.NoOp;
    private TreeState treeState = TreeState.Valid; // tree edit operations must leave the tree in a valid state

    public InteractiveSymbolicExpressionTreeChart() {
      InitializeComponent();
      // add extra actions in the context menu strips


      lastSelected = null;
      currSelected = null;
      tempNode = null;
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
        Repaint();
      }
    }

    private void contextMenuStrip_Opened(object sender, EventArgs e) {
      var menu = sender as ContextMenuStrip;
      if (menu == null) return;
      if (currSelected == null) {
        insertNodeToolStripMenuItem.Visible = false;
        changeValueToolStripMenuItem.Visible = false;
        copyToolStripMenuItem.Visible = false;
        cutToolStripMenuItem.Visible = false;
        deleteToolStripMenuItem.Visible = false;
        pasteToolStripMenuItem.Visible = false;
      } else {
        var node = currSelected.SymbolicExpressionTreeNode;
        changeValueToolStripMenuItem.Visible = (node is SymbolicExpressionTreeTerminalNode);
        insertNodeToolStripMenuItem.Visible = !changeValueToolStripMenuItem.Visible;
        copyToolStripMenuItem.Visible = true;
        cutToolStripMenuItem.Visible = true;
        deleteToolStripMenuItem.Visible = true;
        pasteToolStripMenuItem.Visible = tempNode != null && insertNodeToolStripMenuItem.Visible;
      }
    }

    protected override void OnSymbolicExpressionTreeNodeClicked(object sender, MouseEventArgs e) {
      var visualTreeNode = FindVisualSymbolicExpressionTreeNodeAt(e.X, e.Y);
      if (visualTreeNode == null || visualTreeNode == currSelected) return;
      lastSelected = currSelected;
      if (lastSelected != null)
        lastSelected.LineColor = Color.Black;
      currSelected = visualTreeNode;
      currSelected.LineColor = Color.LightGreen;
      Repaint();
      base.OnSymbolicExpressionTreeNodeClicked(sender, e);
    }

    private void insertNodeToolStripMenuItem_Click(object sender, EventArgs e) {
      if (currSelected == null || currSelected.SymbolicExpressionTreeNode is SymbolicExpressionTreeTerminalNode) return;
      var node = currSelected.SymbolicExpressionTreeNode;

      using (var dialog = new InsertNodeDialog()) {
        dialog.SetAllowedSymbols(node.Grammar.AllowedSymbols.Where(s => s.Enabled && s.InitialFrequency > 0.0 && !(s is ProgramRootSymbol || s is StartSymbol || s is Defun)));
        dialog.DialogValidated += InsertNodeDialog_Validated;
        dialog.ShowDialog(this);
      }
    }

    private void changeValueToolStripMenuItem_Click(object sender, EventArgs e) {
      if (currSelected == null) return;
      var node = currSelected.SymbolicExpressionTreeNode;
      using (var dialog = new ValueChangeDialog()) {
        dialog.SetContent(node);
        dialog.DialogValidated += ChangeValueDialog_Validated;
        dialog.ShowDialog(this);
      }
    }

    public event EventHandler SymbolicExpressionTreeNodeChanged;
    private void OnSymbolicExpressionTreeNodeChanged(object sender, EventArgs e) {
      var changed = SymbolicExpressionTreeNodeChanged;
      if (changed != null)
        SymbolicExpressionTreeNodeChanged(sender, e);
    }

    private void ChangeValueDialog_Validated(object sender, EventArgs e) {
      OnSymbolicExpressionTreeNodeChanged(sender, e);
    }

    private void InsertNodeDialog_Validated(object sender, EventArgs e) {
      var dialog = (InsertNodeDialog)sender;
      var symbol = dialog.SelectedSymbol();
      var node = symbol.CreateTreeNode();
      var parent = currSelected.SymbolicExpressionTreeNode;
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
    }

    private void cutNodeToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.CutNode;
      tempNode = currSelected.SymbolicExpressionTreeNode;
      var visualNode = GetVisualSymbolicExpressionTreeNode(tempNode);
      visualNode.LineColor = Color.LightGray;
      visualNode.TextColor = Color.LightGray;
      Repaint();
    }

    private void cutSubtreeToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.CutSubtree;
      tempNode = currSelected.SymbolicExpressionTreeNode; // should never be null
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

    private void copyNodeToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.CopyNode;
      tempNode = currSelected.SymbolicExpressionTreeNode;
      currSelected.LineColor = Color.LightGray;
      currSelected.TextColor = Color.LightGray;
      Repaint();
    }

    private void copySubtreeToolStripMenuItem_Click(object sender, EventArgs e) {
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

    private void deleteNodeToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.DeleteNode;
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
      currSelected = null; // because the currently selected node was just deleted
      OnSymbolicExpressionTreeChanged(sender, e);
    }

    private void deleteSubtreeToolStripMenuItem_Click(object sender, EventArgs e) {
      lastOp = EditOp.DeleteSubtree;
      var node = currSelected.SymbolicExpressionTreeNode;
      var parent = node.Parent;
      if (parent == null || parent.Symbol is StartSymbol || parent.Symbol is ProgramRootSymbol)
        return; // the operation makes no sense as it would result in the deletion of the entire tree
      parent.RemoveSubtree(parent.IndexOfSubtree(node));
      currSelected = null; // because the currently selected node was just deleted
      OnSymbolicExpressionTreeChanged(sender, e);
    }

    private void pasteToolStripMenuItem_Clicked(object sender, EventArgs e) {
      if (!(lastOp == EditOp.CopyNode || lastOp == EditOp.CopySubtree || lastOp == EditOp.CutNode || lastOp == EditOp.CutSubtree))
        return;
      // check if the copied/cut node (stored in the tempNode) can be inserted as a child of the current selected node
      var node = currSelected.SymbolicExpressionTreeNode;
      if (node is ConstantTreeNode || node is VariableTreeNode) return; // nothing to do
      // check if the currently selected node can accept the copied node as a child 
      // no need to check the grammar, an arity check will do just fine here
      if (node.Symbol.MaximumArity > node.SubtreeCount) {
        switch (lastOp) {
          case (EditOp.CutNode): {
              // when cutting a node from the tree, it's children become children of it's parent
              var parent = tempNode.Parent;
              // arity checks to see if parent can accept node's children (we assume the grammar is already ok with that)
              // (otherise, the 'cut' part of the operation will just not do anything)
              if (parent.Symbol.MaximumArity >= tempNode.SubtreeCount + parent.SubtreeCount - 1) {
                // -1 because tempNode will be removed
                parent.RemoveSubtree(parent.IndexOfSubtree(tempNode));
                for (int i = tempNode.SubtreeCount - 1; i >= 0; --i) {
                  var child = tempNode.GetSubtree(i);
                  tempNode.RemoveSubtree(i);
                  parent.AddSubtree(child);
                }
                lastOp = EditOp.CopyNode;
                currSelected = null;
              }
              break;
            }
          case (EditOp.CutSubtree): {
              // cut subtree
              var parent = tempNode.Parent;
              parent.RemoveSubtree(parent.IndexOfSubtree(tempNode));
              lastOp = EditOp.CopySubtree; // do this so the next paste will actually perform a copy   
              currSelected = null;
              break;
            }
          case (EditOp.CopyNode): {
              // copy node
              var clone = (SymbolicExpressionTreeNode)tempNode.Clone(); // should never be null
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
        Tree = Tree; // hack in order to trigger the reinitialization of the dictionaries after new nodes appeared in the graph
        OnSymbolicExpressionTreeChanged(sender, e);
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
  }
}
