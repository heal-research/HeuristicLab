﻿#region License Information
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

using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  [View("Graphical Representation")]
  [Content(typeof(ISymbolicDataAnalysisModel), true)]
  public partial class GraphicalSymbolicDataAnalysisModelView : AsynchronousContentView {
    public GraphicalSymbolicDataAnalysisModelView()
      : base() {
      InitializeComponent();
    }

    public new ISymbolicDataAnalysisModel Content {
      get { return (ISymbolicDataAnalysisModel)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      symbolicExpressionTreeChart.Tree = null;
      if (Content != null) {
        symbolicExpressionTreeChart.Tree = Content.SymbolicExpressionTree;
        RepaintNodes();
      }
    }

    protected void RepaintNodes() {
      var tree = symbolicExpressionTreeChart.Tree;
      if (tree != null) {
        foreach (var n in tree.IterateNodesPrefix()) {
          if (n.Symbol is SubFunctionSymbol) {
            var visualNode = symbolicExpressionTreeChart.GetVisualSymbolicExpressionTreeNode(n);
            visualNode.FillColor = Color.LightCyan;
            visualNode.LineColor = Color.SlateGray;
          }
        }
        symbolicExpressionTreeChart.RepaintNodes();
      }
    }
  }
}
