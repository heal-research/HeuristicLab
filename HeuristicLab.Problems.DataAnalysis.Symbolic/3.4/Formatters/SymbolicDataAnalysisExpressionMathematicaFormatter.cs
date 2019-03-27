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

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("Mathematica Symbolic Expression Tree Formatter", "A string formatter that converts symbolic expression trees to Mathematica expressions.")]
  [StorableType("818A9294-FA95-41F6-A5F0-D7D050BDD076")]
  public sealed class SymbolicDataAnalysisExpressionMathematicaFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {
    [StorableConstructor]
    private SymbolicDataAnalysisExpressionMathematicaFormatter(StorableConstructorFlag _) : base(_) { }
    private SymbolicDataAnalysisExpressionMathematicaFormatter(SymbolicDataAnalysisExpressionMathematicaFormatter original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisExpressionMathematicaFormatter()
      : base() {
      Name = ItemName;
      Description = ItemDescription;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionMathematicaFormatter(this, cloner);
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      // skip root and start symbols
      StringBuilder strBuilder = new StringBuilder();
      FormatRecursively(symbolicExpressionTree.Root.GetSubtree(0).GetSubtree(0), strBuilder);
      return strBuilder.ToString();
    }

    private void FormatRecursively(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.Subtrees.Any()) {
        if (node.Symbol is Addition) {
          FormatFunction(node, "Plus", strBuilder);
        } else if (node.Symbol is Average) {
          FormatAverage(node, strBuilder);
        } else if (node.Symbol is Multiplication) {
          FormatFunction(node, "Times", strBuilder);
        } else if (node.Symbol is Subtraction) {
          FormatSubtraction(node, strBuilder);
        } else if (node.Symbol is Division) {
          FormatDivision(node, strBuilder);
        } else if (node.Symbol is Sine) {
          FormatFunction(node, "Sin", strBuilder);
        } else if (node.Symbol is Cosine) {
          FormatFunction(node, "Cos", strBuilder);
        } else if (node.Symbol is Tangent) {
          FormatFunction(node, "Tan", strBuilder);
        } else if (node.Symbol is HyperbolicTangent) {
          FormatFunction(node, "Tanh", strBuilder);
        } else if (node.Symbol is Exponential) {
          FormatFunction(node, "Exp", strBuilder);
        } else if (node.Symbol is Logarithm) {
          FormatFunction(node, "Log", strBuilder);
        } else if (node.Symbol is IfThenElse) {
          FormatIf(node, strBuilder);
        } else if (node.Symbol is GreaterThan) {
          strBuilder.Append("If[Greater[");
          FormatRecursively(node.GetSubtree(0), strBuilder);
          strBuilder.Append(",");
          FormatRecursively(node.GetSubtree(1), strBuilder);
          strBuilder.Append("], 1, -1]");
        } else if (node.Symbol is LessThan) {
          strBuilder.Append("If[Less[");
          FormatRecursively(node.GetSubtree(0), strBuilder);
          strBuilder.Append(",");
          FormatRecursively(node.GetSubtree(1), strBuilder);
          strBuilder.Append("], 1, -1]");
        } else if (node.Symbol is And) {
          FormatAnd(node, strBuilder);
        } else if (node.Symbol is Not) {
          strBuilder.Append("If[Greater[");
          FormatRecursively(node.GetSubtree(0), strBuilder);
          strBuilder.Append(", 0], -1, 1]");
        } else if (node.Symbol is Or) {
          FormatOr(node, strBuilder);
        } else if (node.Symbol is Xor) {
          FormatXor(node, strBuilder);
        } else if (node.Symbol is Square) {
          FormatSquare(node, strBuilder);
        } else if (node.Symbol is SquareRoot) {
          FormatFunction(node, "Sqrt", strBuilder);
        } else if (node.Symbol is Power) {
          FormatFunction(node, "Power", strBuilder);
        } else if (node.Symbol is Root) {
          FormatRoot(node, strBuilder);
        } else {
          throw new NotSupportedException("Formatting of symbol: " + node.Symbol + " is not supported.");
        }
      } else {
        // terminals
        if (node.Symbol is Variable) {
          var varNode = node as VariableTreeNode;
          strBuilder.AppendFormat("Times[{0}, {1}]", varNode.VariableName, varNode.Weight.ToString("G17", CultureInfo.InvariantCulture));
        } else if (node.Symbol is Constant) {
          var constNode = node as ConstantTreeNode;
          strBuilder.Append(constNode.Value.ToString("G17", CultureInfo.InvariantCulture));
        } else if (node.Symbol is FactorVariable) {
          var factorNode = node as FactorVariableTreeNode;
          strBuilder.AppendFormat("Switch[{0},", factorNode.VariableName);
          var varValues = factorNode.Symbol.GetVariableValues(factorNode.VariableName).ToArray();
          var weights = varValues.Select(factorNode.GetValue).ToArray();

          var weightStr = string.Join(", ",
            varValues.Zip(weights, (s, d) => string.Format(CultureInfo.InvariantCulture, "\"{0}\", {1:G17}", s, d)));
          strBuilder.Append(weightStr);
          strBuilder.Append("]");
        } else if (node.Symbol is BinaryFactorVariable) {
          var factorNode = node as BinaryFactorVariableTreeNode;
          strBuilder.AppendFormat(CultureInfo.InvariantCulture, "If[{0}==\"{1}\",{2:G17},0.0]",
            factorNode.VariableName, factorNode.VariableValue, factorNode.Weight);
        } else {
          throw new NotSupportedException("Formatting of symbol: " + node.Symbol + " is not supported.");
        }
      }
    }

    private void FormatXor(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("If[Xor[");
      foreach (var t in node.Subtrees) {
        strBuilder.Append("Greater[");
        FormatRecursively(t, strBuilder);
        strBuilder.Append(", 0]");
        if (t != node.Subtrees.Last()) strBuilder.Append(",");
      }
      strBuilder.Append("], 1, -1]");
    }

    private void FormatOr(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("If[Or[");
      foreach (var t in node.Subtrees) {
        strBuilder.Append("Greater[");
        FormatRecursively(t, strBuilder);
        strBuilder.Append(", 0]");
        if (t != node.Subtrees.Last()) strBuilder.Append(",");
      }
      strBuilder.Append("], 1, -1]");
    }

    private void FormatAnd(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("If[And[");
      foreach (var t in node.Subtrees) {
        strBuilder.Append("Greater[");
        FormatRecursively(t, strBuilder);
        strBuilder.Append(", 0]");
        if (t != node.Subtrees.Last()) strBuilder.Append(",");
      }
      strBuilder.Append("], 1, -1]");
    }

    private void FormatIf(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("If[Greater[");
      FormatRecursively(node.GetSubtree(0), strBuilder);
      strBuilder.Append(", 0], ");
      FormatRecursively(node.GetSubtree(1), strBuilder);
      strBuilder.Append(", ");
      FormatRecursively(node.GetSubtree(2), strBuilder);
      strBuilder.Append("]");
    }

    private void FormatAverage(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      // mean function needs a list of values
      strBuilder.Append("Mean[{");
      FormatRecursively(node.GetSubtree(0), strBuilder);
      for (int i = 1; i < node.SubtreeCount; i++) {
        strBuilder.Append(",");
        FormatRecursively(node.GetSubtree(i), strBuilder);
      }
      strBuilder.Append("}]");
    }

    private void FormatSubtraction(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("Subtract[");
      FormatRecursively(node.GetSubtree(0), strBuilder);
      strBuilder.Append(", Times[-1");
      foreach (var t in node.Subtrees) {
        strBuilder.Append(",");
        FormatRecursively(t, strBuilder);
      }
      strBuilder.Append("]]");
    }

    private void FormatSquare(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("Power[");
      FormatRecursively(node.GetSubtree(0), strBuilder);
      strBuilder.Append(", 2]");
    }

    private void FormatRoot(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("Power[");
      FormatRecursively(node.GetSubtree(0), strBuilder);
      strBuilder.Append(", Divide[1,");
      FormatRecursively(node.GetSubtree(1), strBuilder);
      strBuilder.Append("]]");
    }

    private void FormatDivision(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.SubtreeCount == 1) {
        strBuilder.Append("Divide[1, ");
        FormatRecursively(node.GetSubtree(0), strBuilder);
        strBuilder.Append("]");
      } else {
        strBuilder.Append("Divide[");
        FormatRecursively(node.GetSubtree(0), strBuilder);
        strBuilder.Append(", Times[");
        FormatRecursively(node.GetSubtree(1), strBuilder);
        for (int i = 2; i < node.SubtreeCount; i++) {
          strBuilder.Append(",");
          FormatRecursively(node.GetSubtree(i), strBuilder);
        }
        strBuilder.Append("]]");
      }
    }

    private void FormatFunction(ISymbolicExpressionTreeNode node, string function, StringBuilder strBuilder) {
      strBuilder.Append(function + "[");
      foreach (var child in node.Subtrees) {
        FormatRecursively(child, strBuilder);
        if (child != node.Subtrees.Last())
          strBuilder.Append(", ");
      }
      strBuilder.Append("]");
    }
  }
}
