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
using System.Globalization;
using System.Linq;
using System.Text;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class BaseInfixExpressionFormatter {
    public static void FormatRecursively(ISymbolicExpressionTreeNode node, StringBuilder strBuilder,
                                          NumberFormatInfo numberFormat, string formatString, List<KeyValuePair<string, double>> constants = null) {
      if (node.SubtreeCount > 1) {
        var token = GetToken(node.Symbol);
        // operators
        if (token == "+" || token == "-" || token == "OR" || token == "XOR" ||
            token == "*" || token == "/" || token == "AND" ||
            token == "^") {
          strBuilder.Append("(");
          FormatRecursively(node.Subtrees.First(), strBuilder, numberFormat, formatString, constants);

          foreach (var subtree in node.Subtrees.Skip(1)) {
            strBuilder.Append(" ").Append(token).Append(" ");
            FormatRecursively(subtree, strBuilder, numberFormat, formatString, constants);
          }

          strBuilder.Append(")");
        } else {
          // function with multiple arguments
          strBuilder.Append(token).Append("(");
          FormatRecursively(node.Subtrees.First(), strBuilder, numberFormat, formatString, constants);
          foreach (var subtree in node.Subtrees.Skip(1)) {
            strBuilder.Append(", ");
            FormatRecursively(subtree, strBuilder, numberFormat, formatString, constants);
          }

          strBuilder.Append(")");
        }
      } else if (node.SubtreeCount == 1) {
        var token = GetToken(node.Symbol);
        if (token == "-" || token == "NOT") {
          strBuilder.Append("(").Append(token).Append("(");
          FormatRecursively(node.GetSubtree(0), strBuilder, numberFormat, formatString, constants);
          strBuilder.Append("))");
        } else if (token == "/") {
          strBuilder.Append("1/");
          FormatRecursively(node.GetSubtree(0), strBuilder, numberFormat, formatString, constants);
        } else if (token == "+" || token == "*") {
          FormatRecursively(node.GetSubtree(0), strBuilder, numberFormat, formatString, constants);
        } else {
          // function with only one argument
          strBuilder.Append(token).Append("(");
          FormatRecursively(node.GetSubtree(0), strBuilder, numberFormat, formatString, constants);
          strBuilder.Append(")");
        }
      } else {
        // no subtrees
        if (node.Symbol is LaggedVariable) {
          var varNode = node as LaggedVariableTreeNode;
          if (!varNode.Weight.IsAlmost(1.0)) {
            strBuilder.Append("(");
            if (constants != null) {
              strBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}", varNode.Weight);
            } else {
              strBuilder.Append(varNode.Weight.ToString(formatString, numberFormat));
            }
            strBuilder.Append("*");
          }

          strBuilder.Append("LAG(");
          if (varNode.VariableName.Contains("'"))
            strBuilder.AppendFormat("\"{0}\"", varNode.VariableName);
          else
            strBuilder.AppendFormat("'{0}'", varNode.VariableName);

          strBuilder.Append(", ")
                    .AppendFormat(numberFormat, "{0}", varNode.Lag)
                    .Append(")");
        } else if (node.Symbol is Variable) {
          var varNode = node as VariableTreeNode;
          if (!varNode.Weight.IsAlmost(1.0)) {
            strBuilder.Append("(");
            if (constants != null) {
              string constantKey = $"c_{constants.Count}";
              strBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}", constantKey);
              constants.Add(new KeyValuePair<string, double>(constantKey, varNode.Weight));
            } else {
              strBuilder.Append(varNode.Weight.ToString(formatString, numberFormat));
            }

            strBuilder.Append("*");
          }

          if (varNode.VariableName.Contains("'"))
            strBuilder.AppendFormat("\"{0}\"", varNode.VariableName);
          else
            strBuilder.AppendFormat("'{0}'", varNode.VariableName);

          if (!varNode.Weight.IsAlmost(1.0)) strBuilder.Append(")");
        } else if (node.Symbol is FactorVariable) {
          var factorNode = node as FactorVariableTreeNode;
          if (factorNode.VariableName.Contains("'"))
            strBuilder.AppendFormat("\"{0}\"", factorNode.VariableName);
          else
            strBuilder.AppendFormat("'{0}'", factorNode.VariableName);

          strBuilder.AppendFormat("[{0}]",
            string.Join(", ", factorNode.Weights.Select(w => w.ToString(formatString, numberFormat))));
        } else if (node.Symbol is BinaryFactorVariable) {
          var factorNode = node as BinaryFactorVariableTreeNode;
          if (!factorNode.Weight.IsAlmost(1.0)) {
            strBuilder.Append("(");
            if (constants != null) {
              strBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}", factorNode.Weight);
            } else {
              strBuilder.Append(factorNode.Weight.ToString(formatString, numberFormat));
            }

            strBuilder.Append("*");
          }

          if (factorNode.VariableName.Contains("'"))
            strBuilder.AppendFormat("\"{0}\"", factorNode.VariableName);
          else
            strBuilder.AppendFormat("'{0}'", factorNode.VariableName);

          strBuilder.Append(" = ");
          if (factorNode.VariableValue.Contains("'"))
            strBuilder.AppendFormat("\"{0}\"", factorNode.VariableValue);
          else
            strBuilder.AppendFormat("'{0}'", factorNode.VariableValue);

          if (!factorNode.Weight.IsAlmost(1.0)) strBuilder.Append(")");
        } else if (node.Symbol is Constant) {
          var constNode = node as ConstantTreeNode;
          if (constants != null) {
            string constantKey = $"c_{constants.Count}";

            strBuilder.AppendFormat(CultureInfo.InvariantCulture, constantKey);
            constants.Add(new KeyValuePair<string, double>(constantKey, constNode.Value));
          } else {
            if (constNode.Value >= 0.0)
              strBuilder.Append(constNode.Value.ToString(formatString, numberFormat));
            else
              strBuilder.Append("(").Append(constNode.Value.ToString(formatString, numberFormat))
                        .Append(")"); // (-1
          }
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

  /// <summary>
  /// Formats mathematical expressions in infix form. E.g. x1 * (3.0 * x2 + x3)
  /// </summary>
  [StorableType("6FE2C83D-A594-4ABF-B101-5AEAEA6D3E3D")]
  [Item("Infix Symbolic Expression Tree Formatter",
    "A string formatter that converts symbolic expression trees to infix expressions.")]
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
    public string Format(ISymbolicExpressionTree symbolicExpressionTree, NumberFormatInfo numberFormat,
                         string formatString = "G") {
      // skip root and start symbols
      StringBuilder strBuilder = new StringBuilder();
      BaseInfixExpressionFormatter.FormatRecursively(symbolicExpressionTree.Root.GetSubtree(0).GetSubtree(0),
        strBuilder, numberFormat, formatString);
      return strBuilder.ToString();
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      return Format(symbolicExpressionTree, NumberFormatInfo.InvariantInfo);
    }
  }

  [StorableType("54D917E8-134E-4066-9A60-2737C12D81DC")]
  [Item("Infix String Formater", "Formatter for symbolic expressions, which produces an infix expression " +
                                 "as well as a list of all coefficient values")]
  public sealed class InfixExpressionStringFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {
    [StorableConstructor]
    private InfixExpressionStringFormatter(StorableConstructorFlag _) : base(_) { }

    private InfixExpressionStringFormatter(InfixExpressionStringFormatter original, Cloner cloner) : base(original, cloner) { }

    public InfixExpressionStringFormatter() : base() {
      Name = ItemName;
      Description = ItemDescription;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new InfixExpressionStringFormatter(this, cloner);
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      StringBuilder strBuilder = new StringBuilder();
      var constants = new List<KeyValuePair<string, double>>();
      BaseInfixExpressionFormatter.FormatRecursively(symbolicExpressionTree.Root.GetSubtree(0).GetSubtree(0),
        strBuilder, NumberFormatInfo.InvariantInfo, "G", constants);
      strBuilder.Append($"{Environment.NewLine}{Environment.NewLine}");

      int maxDigits = GetDigits(constants.Count);
      int padding = constants.Max(x => x.Value.ToString("F12", CultureInfo.InvariantCulture).Length);
      foreach (var constant in constants) {
        int digits = GetDigits(Int32.Parse(constant.Key.Substring(2)));
        strBuilder.Append($"{constant.Key}{new String(' ', maxDigits - digits)} = " +
                          string.Format($"{{0,{padding}:F12}}", constant.Value, CultureInfo.InvariantCulture) +
                          Environment.NewLine);
      }

      return strBuilder.ToString();
    }

    private int GetDigits(int x) {
      if (x == 0) return 1;
      return (int)Math.Floor(Math.Log10(x) + 1);
    }
  }
}
