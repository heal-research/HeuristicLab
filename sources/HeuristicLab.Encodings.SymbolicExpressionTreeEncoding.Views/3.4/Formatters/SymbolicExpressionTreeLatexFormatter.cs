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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  [Item("LaTeX/PDF Formatter", "Formatter for symbolic expression trees for use with latex package tikz.")]
  public class SymbolicExpressionTreeLatexFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {
    private readonly static Dictionary<string, string> symbolNameMap = new Dictionary<string, string>
    {
      {"ProgramRootSymbol", "Prog"},
      {"StartSymbol","RPB"}
    };
    private readonly ReingoldTilfordLayoutEngine<ISymbolicExpressionTreeNode> layoutEngine = new ReingoldTilfordLayoutEngine<ISymbolicExpressionTreeNode>();

    public SymbolicExpressionTreeLatexFormatter()
      : base("LaTeX/PDF Formatter", "Formatter for symbolic expression trees for use with latex package tikz.") {
      layoutEngine = new ReingoldTilfordLayoutEngine<ISymbolicExpressionTreeNode> {
        HorizontalSpacing = 2,
        VerticalSpacing = 2,
        NodeWidth = 8,
        NodeHeight = 4
      };
    }

    protected SymbolicExpressionTreeLatexFormatter(SymbolicExpressionTreeLatexFormatter original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeLatexFormatter(this, cloner);
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      layoutEngine.Reset();
      var root = symbolicExpressionTree.Root;
      var actualRoot = root.SubtreeCount == 0 ? root.GetSubtree(0) : root;
      layoutEngine.Initialize(actualRoot, x => x.Subtrees);
      layoutEngine.CalculateLayout();
      var nodeCoordinates = layoutEngine.GetCoordinates();
      var sb = new StringBuilder();
      var nl = Environment.NewLine;
      double ws = 1;
      double hs = 0.7;

      sb.Append("\\documentclass[class=minimal,border=0pt]{standalone}" + nl +
                "\\usepackage{tikz}" + nl +
                "\\begin{document}" + nl +
                "\\begin{tikzpicture}" + nl +
                "\\def\\ws{1}" + nl +
                "\\def\\hs{0.7}" + nl);

      var nodeIndices = new Dictionary<ISymbolicExpressionTreeNode, int>();
      var nodes = symbolicExpressionTree.IterateNodesBreadth().ToList();
      for (int i = 0; i < nodes.Count; ++i) {
        var node = nodes[i];
        nodeIndices.Add(node, i);
        var coord = nodeCoordinates[node];
        var nodeName = symbolNameMap.ContainsKey(node.Symbol.Name) ? symbolNameMap[node.Symbol.Name] : node.ToString();
        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "\\node ({0}) at (\\ws*{1},\\hs*{2}) {{{3}}};", i, ws * coord.X, -hs * coord.Y, EscapeLatexString(nodeName)));
      }

      for (int i = 0; i < nodes.Count; ++i) {
        foreach (var s in nodes[i].Subtrees) {
          sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "\\draw ({0}) -- ({1});", i, nodeIndices[s]));
        }
      }

      sb.Append("\\end{tikzpicture}" + nl +
                "\\end{document}" + nl);
      return sb.ToString();
    }

    private static string EscapeLatexString(string s) {
      return s.Replace("\\", "\\\\").Replace("{", "\\{").Replace("}", "\\}").Replace("_", "\\_");
    }
  }
}
