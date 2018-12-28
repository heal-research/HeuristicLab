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

using System.Text;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HEAL.Fossil;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using System;
using System.Globalization;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {

  [Item("ExternalEvaluationSymbolicExpressionTreeStringFormatter", "A string formatter for symbolic expression trees for external evaluation.")]
  [StorableType("6621B850-D3F2-4824-B6A8-19F4969AFB44")]
  public class ExternalEvaluationSymbolicExpressionTreeStringFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {

    public bool Indent { get; set; }

    [StorableConstructor]
    protected ExternalEvaluationSymbolicExpressionTreeStringFormatter(StorableConstructorFlag _) : base(_) { }
    protected ExternalEvaluationSymbolicExpressionTreeStringFormatter(ExternalEvaluationSymbolicExpressionTreeStringFormatter original, Cloner cloner)
      : base(original, cloner) {
      Indent = original.Indent;
    }
    public ExternalEvaluationSymbolicExpressionTreeStringFormatter()
      : base() {
      Name = "External Evaluation Symbolic Expression Tree Formatter";
      Indent = true;
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      // skip root and start symbols
      return FormatRecursively(symbolicExpressionTree.Root.GetSubtree(0).GetSubtree(0), 0);
    }

    private string FormatRecursively(ISymbolicExpressionTreeNode node, int indentLength) {
      StringBuilder strBuilder = new StringBuilder();
      if (Indent) strBuilder.Append(' ', indentLength);
      if (node.Subtrees.Any()) { // internal node
        strBuilder.Append("(");
        if (node.Symbol is Addition) {
          strBuilder.AppendLine("+");
        } else if (node.Symbol is And) {
          strBuilder.AppendLine("&&");
        } else if (node.Symbol is Average) {
          strBuilder.AppendLine("avg");
        } else if (node.Symbol is Cosine) {
          strBuilder.AppendLine("cos");
        } else if (node.Symbol is Division) {
          strBuilder.AppendLine("/");
        } else if (node.Symbol is Exponential) {
          strBuilder.AppendLine("exp");
        } else if (node.Symbol is GreaterThan) {
          strBuilder.AppendLine(">");
        } else if (node.Symbol is IfThenElse) {
          strBuilder.AppendLine("if");
        } else if (node.Symbol is LessThan) {
          strBuilder.AppendLine("<");
        } else if (node.Symbol is Logarithm) {
          strBuilder.AppendLine("ln");
        } else if (node.Symbol is Multiplication) {
          strBuilder.AppendLine("*");
        } else if (node.Symbol is Not) {
          strBuilder.AppendLine("!");
        } else if (node.Symbol is Or) {
          strBuilder.AppendLine("||");
        } else if (node.Symbol is Sine) {
          strBuilder.AppendLine("sin");
        } else if (node.Symbol is Subtraction) {
          strBuilder.AppendLine("-");
        } else if (node.Symbol is Tangent) {
          strBuilder.AppendLine("tan");
        } else {
          throw new NotSupportedException("Formatting of symbol: " + node.Symbol + " not supported for external evaluation.");
        }
        // each subtree expression on a new line
        // and closing ')' also on new line
        foreach (var subtree in node.Subtrees) {
          strBuilder.AppendLine(FormatRecursively(subtree, indentLength + 2));
        }
        if (Indent) strBuilder.Append(' ', indentLength);
        strBuilder.Append(")");
      } else {
        if (node.Symbol is HeuristicLab.Problems.DataAnalysis.Symbolic.Variable) {
          var varNode = node as VariableTreeNode;
          strBuilder.AppendFormat("(* {0} {1})", varNode.VariableName, varNode.Weight.ToString("g17", CultureInfo.InvariantCulture));
        } else if (node.Symbol is Constant) {
          var constNode = node as ConstantTreeNode;
          strBuilder.Append(constNode.Value.ToString("g17", CultureInfo.InvariantCulture));
        } else {
          throw new NotSupportedException("Formatting of symbol: " + node.Symbol + " not supported for external evaluation.");
        }
      }
      return strBuilder.ToString();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExternalEvaluationSymbolicExpressionTreeStringFormatter(this, cloner);
    }
  }
}
