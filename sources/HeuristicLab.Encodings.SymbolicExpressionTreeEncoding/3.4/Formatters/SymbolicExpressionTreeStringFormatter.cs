#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {

  [Item("Default String Formatter", "The default string formatter for symbolic expression trees.")]
  [StorableClass]
  public class SymbolicExpressionTreeStringFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {

    public bool Indent { get; set; }

    [StorableConstructor]
    protected SymbolicExpressionTreeStringFormatter(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeStringFormatter(SymbolicExpressionTreeStringFormatter original, Cloner cloner)
      : base(original, cloner) {
      Indent = original.Indent;
    }
    public SymbolicExpressionTreeStringFormatter()
      : base() {
      Name = "Default String Formatter";
      Indent = true;
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      return FormatRecursively(symbolicExpressionTree.Root, 0);
    }

    private string FormatRecursively(ISymbolicExpressionTreeNode node, int indentLength) {
      StringBuilder strBuilder = new StringBuilder();
      if (Indent) strBuilder.Append(' ', indentLength);
      strBuilder.Append("(");
      // internal nodes or leaf nodes?
      if (node.Subtrees.Count() > 0) {
        // symbol on same line as '('
        strBuilder.AppendLine(node.ToString());
        // each subtree expression on a new line
        // and closing ')' also on new line
        foreach (var subtree in node.Subtrees) {
          strBuilder.AppendLine(FormatRecursively(subtree, indentLength + 2));
        }
        if (Indent) strBuilder.Append(' ', indentLength);
        strBuilder.Append(")");
      } else {
        // symbol in the same line with as '(' and ')'
        strBuilder.Append(node.ToString());
        strBuilder.Append(")");
      }
      return strBuilder.ToString();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeStringFormatter(this, cloner);
    }
  }
}
