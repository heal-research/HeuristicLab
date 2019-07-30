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
using System.Globalization;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Formats mathematical expressions in infix form. E.g. x1 * (3.0 * x2 + x3)
  /// </summary>
  [StorableType("6FE2C83D-A594-4ABF-B101-5AEAEA6D3E3D")]
  [Item("Infix Symbolic Expression Tree Formatter", "A string formatter that converts symbolic expression trees to infix expressions.")]

  public sealed class InfixExpressionFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {


    [StorableConstructor]
    private InfixExpressionFormatter(StorableConstructorFlag _) : base(_) { }
    private InfixExpressionFormatter(InfixExpressionFormatter original, Cloner cloner) : base(original, cloner) { }
    public InfixExpressionFormatter()
      : base() {
      Name = ItemName;
      Description = ItemDescription;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new InfixExpressionFormatter(this, cloner);
    }

    /// <summary>
    /// Produces an infix expression for a given expression tree.
    /// </summary>
    /// <param name="symbolicExpressionTree">The tree representation of the expression.</param>
    /// <param name="numberFormat">Number format that should be used for numeric parameters (e.g. NumberFormatInfo.InvariantInfo (default)).</param>
    /// <param name="formatString">The format string for numeric parameters (e.g. \"G4\" to limit to 4 digits, default is \"G\")</param>
    /// <returns>Infix expression</returns>
    public string Format(ISymbolicExpressionTree symbolicExpressionTree, NumberFormatInfo numberFormat, string formatString="G") {
      // skip root and start symbols
      StringBuilder strBuilder = new StringBuilder();
      FormatRecursively(symbolicExpressionTree.Root.GetSubtree(0).GetSubtree(0), strBuilder, numberFormat, formatString);
      return strBuilder.ToString();
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      return Format(symbolicExpressionTree, NumberFormatInfo.InvariantInfo);
    }

    private static void FormatRecursively(ISymbolicExpressionTreeNode node, StringBuilder strBuilder, NumberFormatInfo numberFormat, string formatString) {
      if (node.SubtreeCount > 1) {
        var token = GetToken(node.Symbol);
        // operators
        if (token == "+" || token == "-" || token == "OR" || token == "XOR" ||
            token == "*" || token == "/" || token == "AND" ||
            token == "^") {
          strBuilder.Append("(");
          FormatRecursively(node.Subtrees.First(), strBuilder, numberFormat, formatString);

          foreach (var subtree in node.Subtrees.Skip(1)) {
            strBuilder.Append(" ").Append(token).Append(" ");
            FormatRecursively(subtree, strBuilder, numberFormat, formatString);
          }
          strBuilder.Append(")");
        } else {
          // function with multiple arguments
          strBuilder.Append(token).Append("(");
          FormatRecursively(node.Subtrees.First(), strBuilder, numberFormat, formatString);
          foreach (var subtree in node.Subtrees.Skip(1)) {
            strBuilder.Append(", ");
            FormatRecursively(subtree, strBuilder, numberFormat, formatString);
          }
          strBuilder.Append(")");
        }
      } else if (node.SubtreeCount == 1) {
        var token = GetToken(node.Symbol);
        if (token == "-" || token == "NOT") {
          strBuilder.Append("(").Append(token).Append("(");
          FormatRecursively(node.GetSubtree(0), strBuilder, numberFormat, formatString);
          strBuilder.Append("))");
        } else if (token == "/") {
          strBuilder.Append("1/");
          FormatRecursively(node.GetSubtree(0), strBuilder, numberFormat, formatString);
        } else if (token == "+" || token == "*") {
          FormatRecursively(node.GetSubtree(0), strBuilder, numberFormat, formatString);
        } else {
          // function with only one argument
          strBuilder.Append(token).Append("(");
          FormatRecursively(node.GetSubtree(0), strBuilder, numberFormat, formatString);
          strBuilder.Append(")");
        }
      } else {
        // no subtrees
        if (node.Symbol is LaggedVariable) {
          var varNode = node as LaggedVariableTreeNode;
          if (!varNode.Weight.IsAlmost(1.0)) {
            strBuilder.Append("(");
            strBuilder.Append(varNode.Weight.ToString(formatString, numberFormat));
            strBuilder.Append("*");
          }
          strBuilder.Append("LAG(");
          if (varNode.VariableName.Contains("'")) {
            strBuilder.AppendFormat("\"{0}\"", varNode.VariableName);
          } else {
            strBuilder.AppendFormat("'{0}'", varNode.VariableName);
          }
          strBuilder.Append(", ")
            .AppendFormat(numberFormat, "{0}", varNode.Lag)
            .Append(")");
        } else if (node.Symbol is Variable) {
          var varNode = node as VariableTreeNode;
          if (!varNode.Weight.IsAlmost(1.0)) {
            strBuilder.Append("(");
            strBuilder.Append(varNode.Weight.ToString(formatString, numberFormat));
            strBuilder.Append("*");
          }
          if (varNode.VariableName.Contains("'")) {
            strBuilder.AppendFormat("\"{0}\"", varNode.VariableName);
          } else {
            strBuilder.AppendFormat("'{0}'", varNode.VariableName);
          }
          if (!varNode.Weight.IsAlmost(1.0)) {
            strBuilder.Append(")");
          }
        } else if (node.Symbol is FactorVariable) {
          var factorNode = node as FactorVariableTreeNode;
          if (factorNode.VariableName.Contains("'")) {
            strBuilder.AppendFormat("\"{0}\"", factorNode.VariableName);
          } else {
            strBuilder.AppendFormat("'{0}'", factorNode.VariableName);
          }
          strBuilder.AppendFormat("[{0}]",
            string.Join(", ", factorNode.Weights.Select(w => w.ToString(formatString, numberFormat))));
        } else if (node.Symbol is BinaryFactorVariable) {
          var factorNode = node as BinaryFactorVariableTreeNode;
          if (!factorNode.Weight.IsAlmost(1.0)) {
            strBuilder.Append("(");
            strBuilder.Append(factorNode.Weight.ToString(formatString, numberFormat));
            strBuilder.Append("*");
          }
          if (factorNode.VariableName.Contains("'")) {
            strBuilder.AppendFormat("\"{0}\"", factorNode.VariableName);
          } else {
            strBuilder.AppendFormat("'{0}'", factorNode.VariableName);
          }
          strBuilder.Append(" = ");
          if (factorNode.VariableValue.Contains("'")) {
            strBuilder.AppendFormat("\"{0}\"", factorNode.VariableValue);
          } else {
            strBuilder.AppendFormat("'{0}'", factorNode.VariableValue);
          }

          if (!factorNode.Weight.IsAlmost(1.0)) {
            strBuilder.Append(")");
          }

        } else if (node.Symbol is Constant) {
          var constNode = node as ConstantTreeNode;
          if (constNode.Value >= 0.0)
            strBuilder.Append(constNode.Value.ToString(formatString, numberFormat));
          else
            strBuilder.Append("(").Append(constNode.Value.ToString(formatString, numberFormat)).Append(")"); // (-1
        }
      }
    }

    private static string GetToken(ISymbol symbol) {
      var tok = InfixExpressionParser.knownSymbols.GetBySecond(symbol).FirstOrDefault();
      if (tok == null)
        throw new ArgumentException(string.Format("Unknown symbol {0} found.", symbol.Name));
      return tok;
    }
  }
}
