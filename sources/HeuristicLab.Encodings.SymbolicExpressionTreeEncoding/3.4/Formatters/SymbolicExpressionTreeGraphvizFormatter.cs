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

using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Common;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Formatters {
  [Item("GraphViz String Formatter", "Formatter for symbolic expression trees for visualization with GraphViz.")]
  public sealed class SymbolicExpressionTreeGraphvizFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {
    public bool Indent { get; set; }

    private readonly static Dictionary<string, string> symbolNameMap = new Dictionary<string, string>() {
      // match Koza style
      {"ProgramRootSymbol", "Prog"},
      {"StartSymbol", "RPB"}, 
    };

    public SymbolicExpressionTreeGraphvizFormatter()
      : base("GraphViz String Formatter", "Formatter for symbolic expression trees for visualization with GraphViz.") {
      Indent = true;
    }
    private SymbolicExpressionTreeGraphvizFormatter(SymbolicExpressionTreeGraphvizFormatter original, Cloner cloner)
      : base(original, cloner) {
      this.Indent = original.Indent;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeGraphvizFormatter(this, cloner);
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      int nodeCounter = 1;
      StringBuilder strBuilder = new StringBuilder();
      strBuilder.AppendLine("graph {");
      strBuilder.AppendLine(FormatRecursively(symbolicExpressionTree.Root, 0, ref nodeCounter));
      strBuilder.AppendLine("}");
      return strBuilder.ToString();
    }

    private string FormatRecursively(ISymbolicExpressionTreeNode node, int indentLength, ref int nodeId) {
      // save id of current node
      int currentNodeId = nodeId;
      // increment id for next node
      nodeId++;

      StringBuilder strBuilder = new StringBuilder();
      if (Indent) strBuilder.Append(' ', indentLength);

      // get label for node and map if necessary
      string nodeLabel = node.ToString();
      if (symbolNameMap.ContainsKey(nodeLabel)) {
        nodeLabel = symbolNameMap[nodeLabel];
      }

      strBuilder.Append("node" + currentNodeId + "[label=\"" + nodeLabel + "\"");
      // leaf nodes should have box shape
      if (node.SubtreeCount == 0) {
        strBuilder.AppendLine(", shape=\"box\"];");
      } else {
        strBuilder.AppendLine("];");
      }

      // internal nodes or leaf nodes?
      foreach (ISymbolicExpressionTreeNode subTree in node.Subtrees) {
        // add an edge 
        if (Indent) strBuilder.Append(' ', indentLength);
        strBuilder.AppendLine("node" + currentNodeId + " -- node" + nodeId + ";");
        // format the whole subtree
        strBuilder.Append(FormatRecursively(subTree, indentLength + 2, ref nodeId));
      }

      return strBuilder.ToString();
    }
  }
}
