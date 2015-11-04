#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("C# Symbolic Expression Tree Formatter", "A string formatter that converts symbolic expression trees to C# code.")]
  [StorableClass]
  public sealed class CSharpSymbolicExpressionTreeStringFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {
    [StorableConstructor]
    private CSharpSymbolicExpressionTreeStringFormatter(bool deserializing) : base(deserializing) { }
    private CSharpSymbolicExpressionTreeStringFormatter(CSharpSymbolicExpressionTreeStringFormatter original, Cloner cloner) : base(original, cloner) { }
    public CSharpSymbolicExpressionTreeStringFormatter()
      : base() {
      Name = ItemName;
      Description = ItemDescription;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CSharpSymbolicExpressionTreeStringFormatter(this, cloner);
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      // skip root and start symbols
      StringBuilder strBuilder = new StringBuilder();
      GenerateHeader(strBuilder, symbolicExpressionTree);
      FormatRecursively(symbolicExpressionTree.Root.GetSubtree(0).GetSubtree(0), strBuilder);
      GenerateFooter(strBuilder);
      return strBuilder.ToString();
    }

    private void FormatRecursively(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      // TODO: adapt to interpreter semantics. The HL interpreter also allows Boolean operations on reals
      if (node.Subtrees.Any()) {
        if (node.Symbol is Addition) {
          FormatOperator(node, "+", strBuilder);
        } else if (node.Symbol is And) {
          FormatOperator(node, "&&", strBuilder);
        } else if (node.Symbol is Average) {
          FormatFunction(node, "Average", strBuilder);
        } else if (node.Symbol is Cosine) {
          FormatFunction(node, "Math.Cos", strBuilder);
        } else if (node.Symbol is Division) {
          FormatDivision(node, strBuilder);
        } else if (node.Symbol is Exponential) {
          FormatFunction(node, "Math.Exp", strBuilder);
        } else if (node.Symbol is GreaterThan) {
          FormatOperator(node, ">", strBuilder);
        } else if (node.Symbol is IfThenElse) {
          FormatFunction(node, "EvaluateIf", strBuilder);
        } else if (node.Symbol is LessThan) {
          FormatOperator(node, "<", strBuilder);
        } else if (node.Symbol is Logarithm) {
          FormatFunction(node, "Math.Log", strBuilder);
        } else if (node.Symbol is Multiplication) {
          FormatOperator(node, "*", strBuilder);
        } else if (node.Symbol is Not) {
          FormatOperator(node, "!", strBuilder);
        } else if (node.Symbol is Or) {
          FormatOperator(node, "||", strBuilder);
        } else if (node.Symbol is Xor) {
          FormatOperator(node, "^", strBuilder);
        } else if (node.Symbol is Sine) {
          FormatFunction(node, "Math.Sin", strBuilder);
        } else if (node.Symbol is Subtraction) {
          FormatSubtraction(node, strBuilder);
        } else if (node.Symbol is Tangent) {
          FormatFunction(node, "Math.Tan", strBuilder);
        } else if (node.Symbol is Square) {
          FormatSquare(node, strBuilder);
        } else if (node.Symbol is SquareRoot) {
          FormatFunction(node, "Math.Sqrt", strBuilder);
        } else if (node.Symbol is Power) {
          FormatFunction(node, "Math.Pow", strBuilder);
        } else if (node.Symbol is Root) {
          FormatRoot(node, strBuilder);
        } else {
          throw new NotSupportedException("Formatting of symbol: " + node.Symbol + " not supported for C# symbolic expression tree formatter.");
        }
      } else {
        if (node is VariableTreeNode) {
          var varNode = node as VariableTreeNode;
          strBuilder.AppendFormat("{0} * {1}", varNode.VariableName, varNode.Weight.ToString("g17", CultureInfo.InvariantCulture));
        } else if (node is ConstantTreeNode) {
          var constNode = node as ConstantTreeNode;
          strBuilder.Append(constNode.Value.ToString("g17", CultureInfo.InvariantCulture));
        } else {
          throw new NotSupportedException("Formatting of symbol: " + node.Symbol + " not supported for C# symbolic expression tree formatter.");
        }
      }
    }

    private void FormatSquare(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("Math.Pow(");
      FormatRecursively(node.GetSubtree(0), strBuilder);
      strBuilder.Append(", 2)");
    }

    private void FormatRoot(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("Math.Pow(");
      FormatRecursively(node.GetSubtree(0), strBuilder);
      strBuilder.Append(", 1.0 / (");
      FormatRecursively(node.GetSubtree(1), strBuilder);
      strBuilder.Append("))");
    }

    private void FormatDivision(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.SubtreeCount == 1) {
        strBuilder.Append("1.0 / ");
        FormatRecursively(node.GetSubtree(0), strBuilder);
      } else {
        FormatRecursively(node.GetSubtree(0), strBuilder);
        strBuilder.Append("/ (");
        for (int i = 1; i < node.SubtreeCount; i++) {
          if (i > 1) strBuilder.Append(" * ");
          FormatRecursively(node.GetSubtree(i), strBuilder);
        }
        strBuilder.Append(")");
      }
    }

    private void FormatSubtraction(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.SubtreeCount == 1) {
        strBuilder.Append("-");
        FormatRecursively(node.GetSubtree(0), strBuilder);
        return;
      }
      //Default case: more than 1 child
      FormatOperator(node, "-", strBuilder);
    }

    private void FormatOperator(ISymbolicExpressionTreeNode node, string symbol, StringBuilder strBuilder) {
      strBuilder.Append("(");
      foreach (var child in node.Subtrees) {
        FormatRecursively(child, strBuilder);
        if (child != node.Subtrees.Last())
          strBuilder.Append(" " + symbol + " ");
      }
      strBuilder.Append(")");
    }

    private void FormatFunction(ISymbolicExpressionTreeNode node, string function, StringBuilder strBuilder) {
      strBuilder.Append(function + "(");
      foreach (var child in node.Subtrees) {
        FormatRecursively(child, strBuilder);
        if (child != node.Subtrees.Last())
          strBuilder.Append(", ");
      }
      strBuilder.Append(")");
    }

    private void GenerateHeader(StringBuilder strBuilder, ISymbolicExpressionTree symbolicExpressionTree) {
      strBuilder.AppendLine("using System;");
      strBuilder.AppendLine("using System.Linq;" + Environment.NewLine);
      strBuilder.AppendLine("namespace HeuristicLab.Models {");
      strBuilder.AppendLine("public static class Model {");
      GenerateAverageSource(strBuilder);
      GenerateIfThenElseSource(strBuilder);
      strBuilder.Append(Environment.NewLine + "public static double Evaluate (");

      HashSet<string> varNames = new HashSet<string>();
      foreach (var node in symbolicExpressionTree.IterateNodesPostfix().Where(x => x is VariableTreeNode)) {
        varNames.Add(((VariableTreeNode)node).VariableName);
      }

      var orderedNames = varNames.OrderBy(n => n, new NaturalStringComparer()).Select(n => "double " + n);
      strBuilder.Append(string.Join(", ", orderedNames));

      strBuilder.AppendLine(") {");
      strBuilder.Append("double result = ");
    }

    private void GenerateFooter(StringBuilder strBuilder) {
      strBuilder.AppendLine(";");
      strBuilder.AppendLine("return result;");
      strBuilder.AppendLine("}");
      strBuilder.AppendLine("}");
      strBuilder.AppendLine("}");
    }

    private void GenerateAverageSource(StringBuilder strBuilder) {
      strBuilder.AppendLine("private static double Average(params double[] values) {");
      strBuilder.AppendLine("  return values.Average();");
      strBuilder.AppendLine("}");
    }

    private void GenerateIfThenElseSource(StringBuilder strBuilder) {
      strBuilder.AppendLine("private static double EvaluateIf(bool condition, double then, double @else) {");
      strBuilder.AppendLine("   return condition ? then : @else;");
      strBuilder.AppendLine("}");
    }
  }
}
