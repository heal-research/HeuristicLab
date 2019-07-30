#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.PluginInfrastructure;

using VisualSymbolicExpressionTreeNode = HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.VisualTreeNode<HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ISymbolicExpressionTreeNode>;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  public sealed partial class SymbolicExpressionGrammarAllowedChildSymbolsControl : UserControl {
    private ObservableList<ISymbolicExpressionTreeNode> selectedSymbolicExpressionTreeNodes;

    public SymbolicExpressionGrammarAllowedChildSymbolsControl() {
      InitializeComponent();
      selectedSymbolicExpressionTreeNodes = new ObservableList<ISymbolicExpressionTreeNode>();
    }

    private ISymbolicExpressionGrammar grammar;
    public ISymbolicExpressionGrammar Grammar {
      get { return grammar; }
      set {
        if (grammar != value) {
          if (grammar != null) DeregisterGrammarEvents();
          grammar = value;
          if (grammar != null) RegisterGrammarEvents();
          OnGrammarChanged();
        }
      }
    }

    private ISymbol symbol;
    public ISymbol Symbol {
      get { return symbol; }
      set {
        if (symbol != value) {
          if (value != null && grammar == null) throw new InvalidOperationException("grammar is null");
          if (value != null && !grammar.ContainsSymbol(value)) throw new ArgumentException("grammar does not contain symbol.");
          symbol = value;
          OnSymbolChanged();
        }
      }
    }

    private void RegisterGrammarEvents() {
      grammar.Changed += new EventHandler(Grammar_Changed);
      grammar.ReadOnlyChanged += new EventHandler(Grammar_ReadOnlyChanged);
    }
    private void DeregisterGrammarEvents() {
      grammar.Changed -= new EventHandler(Grammar_Changed);
      grammar.ReadOnlyChanged -= new EventHandler(Grammar_ReadOnlyChanged);
    }

    private void Grammar_Changed(object sender, EventArgs e) {
      if (Grammar == null) return;
      if (Symbol == null) return;
      if (Symbol != null && !Grammar.ContainsSymbol(Symbol)) Symbol = null;
      else BuildAllowedChildSymbolsTree();
    }

    private void Grammar_ReadOnlyChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((MethodInvoker)BuildAllowedChildSymbolsTree);
      else BuildAllowedChildSymbolsTree();
    }

    private void OnGrammarChanged() {
      if (Grammar == null) {
        symbolicExpressionTreeChart.Tree = null;
        Symbol = null;
      }
    }
    private void OnSymbolChanged() {
      if (Symbol == null) symbolicExpressionTreeChart.Tree = null;
      else BuildAllowedChildSymbolsTree();
    }

    private void BuildAllowedChildSymbolsTree() {
      if (Symbol == null) {
        symbolicExpressionTreeChart.Tree = null;
        return;
      }

      var tree = new SymbolicExpressionTree(new SymbolicExpressionTreeNode(Symbol));
      if (Grammar.GetMaximumSubtreeCount(Symbol) > 0) {
        for (int i = 0; i < Grammar.GetMaximumSubtreeCount(Symbol); i++) {
          var node = new DummySymbol("Subtree " + i).CreateTreeNode();
          var groupSymbols = grammar.GetAllowedChildSymbols(Symbol, i).OfType<GroupSymbol>().ToList();
          foreach (var childSymbol in Grammar.GetAllowedChildSymbols(Symbol, i)) {
            if (!groupSymbols.Any(g => g != childSymbol && g.Flatten().Contains(childSymbol)))
              node.AddSubtree(new SymbolicExpressionTreeNode(childSymbol));
          }
          tree.Root.AddSubtree(node);
        }
      }
      symbolicExpressionTreeChart.Tree = tree;
      symbolicExpressionTreeChart.SuspendRepaint = true;
      foreach (var subtreeNode in tree.Root.Subtrees) {
        foreach (var allowedChildNode in subtreeNode.Subtrees) {
          var visualLine = symbolicExpressionTreeChart.GetVisualSymbolicExpressionTreeNodeConnection(subtreeNode, allowedChildNode);
          visualLine.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        }
      }

      for (int i = Grammar.GetMinimumSubtreeCount(symbol); i < Grammar.GetMaximumSubtreeCount(symbol); i++) {
        var subtreeNode = tree.Root.GetSubtree(i);
        var visualTreeNode = symbolicExpressionTreeChart.GetVisualSymbolicExpressionTreeNode(subtreeNode);
        visualTreeNode.TextColor = Color.Gray;
        visualTreeNode.LineColor = Color.LightGray;

        var visualLine = symbolicExpressionTreeChart.GetVisualSymbolicExpressionTreeNodeConnection(tree.Root, subtreeNode);
        visualLine.LineColor = Color.LightGray;

        foreach (var allowedChildNode in subtreeNode.Subtrees) {
          visualTreeNode = symbolicExpressionTreeChart.GetVisualSymbolicExpressionTreeNode(allowedChildNode);
          visualTreeNode.TextColor = Color.Gray;
          visualTreeNode.LineColor = Color.LightGray;

          visualLine = symbolicExpressionTreeChart.GetVisualSymbolicExpressionTreeNodeConnection(subtreeNode, allowedChildNode);
          visualLine.LineColor = Color.LightGray;
        }
      }
      symbolicExpressionTreeChart.SuspendRepaint = false;
      UpdateSelectedSymbolicExpressionTreeNodes();
    }

    private void UpdateSelectedSymbolicExpressionTreeNodes() {
      foreach (var node in symbolicExpressionTreeChart.Tree.IterateNodesPrefix()) {
        var visualNode = symbolicExpressionTreeChart.GetVisualSymbolicExpressionTreeNode(node);
        if (!selectedSymbolicExpressionTreeNodes.Contains(node)) visualNode.FillColor = Color.White;
        else visualNode.FillColor = Color.LightSteelBlue;
      }
      symbolicExpressionTreeChart.RepaintNodes();
    }

    private void symbolicExpressionTreeChart_SymbolicExpressionTreeNodeClicked(object sender, MouseEventArgs e) {
      if (Grammar.ReadOnly) return;
      if ((Control.ModifierKeys & Keys.Control) == 0)
        selectedSymbolicExpressionTreeNodes.Clear();

      VisualSymbolicExpressionTreeNode clickedNode = (VisualSymbolicExpressionTreeNode)sender;
      var selectedNode = clickedNode.Content;
      if (selectedNode.SubtreeCount == 0) {
        if (!selectedSymbolicExpressionTreeNodes.Contains(selectedNode))
          selectedSymbolicExpressionTreeNodes.Add(selectedNode);
        else
          selectedSymbolicExpressionTreeNodes.Remove(selectedNode);
      }

      UpdateSelectedSymbolicExpressionTreeNodes();
    }

    private void symbolicExpressionTreeChart_KeyDown(object sender, KeyEventArgs e) {
      if (Grammar.ReadOnly) return;
      if (e.KeyCode == Keys.Delete) {
        var root = symbolicExpressionTreeChart.Tree.Root;
        Grammar.StartGrammarManipulation();
        foreach (var node in selectedSymbolicExpressionTreeNodes) {
          int argIndex = root.IndexOfSubtree(node.Parent);
          Grammar.RemoveAllowedChildSymbol(root.Symbol, node.Symbol, argIndex);
        }

        selectedSymbolicExpressionTreeNodes.Clear();
        Grammar.FinishedGrammarManipulation();
      }
    }

    #region drag & drop operations
    private bool validDragOperation;
    private void symbolicExpressionTreeChart_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (Grammar.ReadOnly) return;

      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      var symbol = data as ISymbol;
      var symbols = data as IEnumerable<ISymbol>;
      if (symbol != null && !(symbol is IReadOnlySymbol) && Grammar.ContainsSymbol(symbol)) validDragOperation = true;
      else if (symbols != null && symbols.All(s => !(symbol is IReadOnlySymbol) && Grammar.ContainsSymbol(s))) validDragOperation = true;
    }

    private void symbolicExpressionTreeChart_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (validDragOperation) {
        Point coordinates = symbolicExpressionTreeChart.PointToClient(new Point(e.X, e.Y));
        var visualNode = symbolicExpressionTreeChart.FindVisualSymbolicExpressionTreeNodeAt(coordinates.X, coordinates.Y);
        if (visualNode != null) {
          var node = visualNode.Content;
          var root = symbolicExpressionTreeChart.Tree.Root;
          if (node == root || node.Parent == root) e.Effect = DragDropEffects.Copy;
        }
      }
    }

    private void symbolicExpressionTreeChart_DragDrop(object sender, DragEventArgs e) {
      Point coordinates = symbolicExpressionTreeChart.PointToClient(new Point(e.X, e.Y));
      var node = symbolicExpressionTreeChart.FindVisualSymbolicExpressionTreeNodeAt(coordinates.X, coordinates.Y);
      var root = symbolicExpressionTreeChart.Tree.Root;

      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      var symbol = data as ISymbol;
      var symbols = data as IEnumerable<ISymbol>;

      if (node.Content == root) {
        if (symbol != null)
          Grammar.AddAllowedChildSymbol(root.Symbol, symbol);
        else if (symbols != null)
          foreach (var s in symbols) Grammar.AddAllowedChildSymbol(root.Symbol, s);
      } else {
        int argumentIndex = root.IndexOfSubtree(node.Content);
        if (symbol != null)
          Grammar.AddAllowedChildSymbol(root.Symbol, symbol, argumentIndex);
        else if (symbols != null)
          foreach (var s in symbols) Grammar.AddAllowedChildSymbol(root.Symbol, s, argumentIndex);
      }
      BuildAllowedChildSymbolsTree();
    }
    #endregion

    #region draw and handle root node with buttons to manipulate the subtree count
    private RectangleF increaseMinimumSubtreeCountRectangle;
    private RectangleF decreaseMinimumSubtreeCountRectangle;
    private RectangleF increaseMaximumSubtreeCountRectangle;
    private RectangleF decreaseMaximumSubtreeCountRectangle;
    private void allowedChildSymbolsControl_Paint(object sender, PaintEventArgs e) {
      increaseMinimumSubtreeCountRectangle = RectangleF.Empty;
      decreaseMinimumSubtreeCountRectangle = RectangleF.Empty;
      increaseMaximumSubtreeCountRectangle = RectangleF.Empty;
      decreaseMaximumSubtreeCountRectangle = RectangleF.Empty;

      if (Grammar == null) return;
      if (symbolicExpressionTreeChart.Tree == null) return;

      var rootNode = symbolicExpressionTreeChart.Tree.Root;
      var visualRootNode = symbolicExpressionTreeChart.GetVisualSymbolicExpressionTreeNode(rootNode);
      var graphics = e.Graphics;

      if (rootNode.Symbol is IReadOnlySymbol) return;
      if (rootNode.Symbol.MinimumArity == rootNode.Symbol.MaximumArity) return;

      using (Pen pen = new Pen(Color.Black)) {
        using (Font font = new Font("Times New Roman", 8)) {
          var stringFormat = new StringFormat();
          stringFormat.Alignment = StringAlignment.Center;
          stringFormat.LineAlignment = StringAlignment.Center;
          int spacing = 5;
          int size = (visualRootNode.Height - spacing * 3) / 2;

          increaseMinimumSubtreeCountRectangle = new RectangleF(visualRootNode.X - spacing - size, visualRootNode.Y + spacing, size, size);
          decreaseMinimumSubtreeCountRectangle = new RectangleF(visualRootNode.X - spacing - size, visualRootNode.Y + size + 2 * spacing, size, size);
          increaseMaximumSubtreeCountRectangle = new RectangleF(visualRootNode.X + visualRootNode.Width + spacing, visualRootNode.Y + spacing, size, size);
          decreaseMaximumSubtreeCountRectangle = new RectangleF(visualRootNode.X + visualRootNode.Width + spacing, visualRootNode.Y + size + 2 * spacing, size, size);

          pen.Color = Grammar.ReadOnly || Grammar.GetMaximumSubtreeCount(rootNode.Symbol) == Grammar.GetMinimumSubtreeCount(rootNode.Symbol) ? Color.LightGray : Color.Black;
          graphics.DrawString("+", font, pen.Brush, increaseMinimumSubtreeCountRectangle, stringFormat);
          graphics.DrawRectangle(pen, Rectangle.Round(increaseMinimumSubtreeCountRectangle));
          if (pen.Color == Color.LightGray) increaseMinimumSubtreeCountRectangle = RectangleF.Empty;

          pen.Color = Grammar.ReadOnly || Grammar.GetMinimumSubtreeCount(rootNode.Symbol) == rootNode.Symbol.MinimumArity ? Color.LightGray : Color.Black;
          graphics.DrawString("-", font, pen.Brush, decreaseMinimumSubtreeCountRectangle, stringFormat);
          graphics.DrawRectangle(pen, Rectangle.Round(decreaseMinimumSubtreeCountRectangle));
          if (pen.Color == Color.LightGray) decreaseMinimumSubtreeCountRectangle = RectangleF.Empty;

          pen.Color = Grammar.ReadOnly || Grammar.GetMaximumSubtreeCount(rootNode.Symbol) == rootNode.Symbol.MaximumArity ? Color.LightGray : Color.Black;
          graphics.DrawRectangle(pen, Rectangle.Round(increaseMaximumSubtreeCountRectangle));
          graphics.DrawString("+", font, pen.Brush, increaseMaximumSubtreeCountRectangle, stringFormat);
          if (pen.Color == Color.LightGray) increaseMaximumSubtreeCountRectangle = RectangleF.Empty;

          pen.Color = Grammar.ReadOnly || Grammar.GetMaximumSubtreeCount(rootNode.Symbol) == Grammar.GetMinimumSubtreeCount(rootNode.Symbol) ? Color.LightGray : Color.Black;
          graphics.DrawRectangle(pen, Rectangle.Round(decreaseMaximumSubtreeCountRectangle));
          graphics.DrawString("-", font, pen.Brush, decreaseMaximumSubtreeCountRectangle, stringFormat);
          if (pen.Color == Color.LightGray) decreaseMaximumSubtreeCountRectangle = RectangleF.Empty;
        }
      }
    }

    private void allowedChildSymbolsControl_MouseDown(object sender, MouseEventArgs e) {
      if (Grammar == null || Grammar.ReadOnly) return;
      if (symbolicExpressionTreeChart.Tree == null) return;

      var pointF = new PointF(e.X, e.Y);
      var rootSymbol = symbolicExpressionTreeChart.Tree.Root.Symbol;
      int minimumSubtreeCount = Grammar.GetMinimumSubtreeCount(rootSymbol);
      int maximumSubtreecount = Grammar.GetMaximumSubtreeCount(rootSymbol);

      bool changed = true;
      if (increaseMinimumSubtreeCountRectangle.Contains(pointF))
        Grammar.SetSubtreeCount(rootSymbol, minimumSubtreeCount + 1, maximumSubtreecount);
      else if (decreaseMinimumSubtreeCountRectangle.Contains(pointF))
        Grammar.SetSubtreeCount(rootSymbol, minimumSubtreeCount - 1, maximumSubtreecount);
      else if (increaseMaximumSubtreeCountRectangle.Contains(pointF))
        Grammar.SetSubtreeCount(rootSymbol, minimumSubtreeCount, maximumSubtreecount + 1);
      else if (decreaseMaximumSubtreeCountRectangle.Contains(pointF))
        Grammar.SetSubtreeCount(rootSymbol, minimumSubtreeCount, maximumSubtreecount - 1);
      else
        changed = false;

      if (changed) BuildAllowedChildSymbolsTree();
    }
    #endregion

  }

  [NonDiscoverableType]
  internal class DummySymbol : Symbol {
    private const int minimumArity = 1;
    private const int maximumArity = byte.MaxValue;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    public DummySymbol(DummySymbol original, Cloner cloner) : base(original, cloner) { }
    public DummySymbol(string name) : base(name, "DummySymbol for views") { }
    public override IDeepCloneable Clone(Cloner cloner) { return new DummySymbol(this, cloner); }
  }
}
