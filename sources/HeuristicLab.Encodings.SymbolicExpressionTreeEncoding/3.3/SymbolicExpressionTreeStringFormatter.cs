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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  
  public class SymbolicExpressionTreeStringFormatter {
    
    public bool Indent { get; set; }
    
    public SymbolicExpressionTreeStringFormatter()
      : base() {
      Indent = true;
    }

    public string Format(SymbolicExpressionTree symbolicExpressionTree) {
      return FormatRecursively(symbolicExpressionTree.Root, 0);
    }

    private string FormatRecursively(SymbolicExpressionTreeNode node, int indentLength) {
      StringBuilder strBuilder = new StringBuilder();
      if (Indent) strBuilder.Append(' ', indentLength);
      strBuilder.Append("(");
      // internal nodes or leaf nodes?
      if (node.SubTrees.Count > 0) {
        // symbol on same line as '('
        strBuilder.AppendLine(node.ToString());
        // each subtree expression on a new line
        // and closing ')' also on new line
        foreach (var subtree in node.SubTrees) {
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
  }
}
