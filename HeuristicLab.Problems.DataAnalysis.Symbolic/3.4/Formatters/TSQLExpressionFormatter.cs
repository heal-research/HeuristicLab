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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  internal static class StringBuilderExtensions {
    internal static void AppendIndented(this StringBuilder strBuilder, int level, string text) {
      strBuilder.Append(new string(' ', level * 2));
      strBuilder.Append(text);
    }
    internal static void AppendLineIndented(this StringBuilder strBuilder, int level, string text) {
      strBuilder.Append(new string(' ', level * 2));
      strBuilder.AppendLine(text);
    }
  }

  [Item("TSQL String Formatter", "String formatter for string representations of symbolic data analysis expressions in TSQL syntax.")]
  [StorableClass]
  public sealed class TSQLExpressionFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {
    [StorableConstructor]
    private TSQLExpressionFormatter(bool deserializing) : base(deserializing) { }
    private TSQLExpressionFormatter(TSQLExpressionFormatter original, Cloner cloner) : base(original, cloner) { }
    public TSQLExpressionFormatter()
      : base() {
      Name = ItemName;
      Description = ItemDescription;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSQLExpressionFormatter(this, cloner);
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      // skip root and start symbols
      StringBuilder strBuilder = new StringBuilder();
      GenerateHeader(strBuilder, symbolicExpressionTree);

      //generate function body
      FormatRecursively(1, symbolicExpressionTree.Root.GetSubtree(0).GetSubtree(0), strBuilder);

      GenerateFooter(strBuilder);
      return strBuilder.ToString();
    }

    private void GenerateHeader(StringBuilder strBuilder, ISymbolicExpressionTree symbolicExpressionTree) {
      HashSet<string> floatVarNames = new HashSet<string>();
      foreach (var node in symbolicExpressionTree.IterateNodesPostfix().Where(x => x is VariableTreeNode || x is VariableConditionTreeNode)) {
        floatVarNames.Add(((IVariableTreeNode)node).VariableName);
      }
      var sortedFloatIdentifiers = floatVarNames.OrderBy(n => n, new NaturalStringComparer()).Select(n => VariableName2Identifier(n)).ToList();

      HashSet<string> varcharVarNames = new HashSet<string>();
      foreach (var node in symbolicExpressionTree.IterateNodesPostfix().Where(x => x is BinaryFactorVariableTreeNode || x is FactorVariableTreeNode)) {
        varcharVarNames.Add(((IVariableTreeNode)node).VariableName);
      }
      var sortedVarcharIdentifiers = varcharVarNames.OrderBy(n => n, new NaturalStringComparer()).Select(n => VariableName2Identifier(n)).ToList();

      //Generate comment and instructions
      strBuilder.Append("-- generated. created function can be used like 'SELECT dbo.REGRESSIONMODEL(");
      strBuilder.Append(string.Join(", ", sortedVarcharIdentifiers));
      if (varcharVarNames.Any() && floatVarNames.Any())
        strBuilder.Append(",");
      strBuilder.Append(string.Join(", ", sortedFloatIdentifiers));
      strBuilder.AppendLine(")'");
      strBuilder.AppendLine("-- use the expression after the RETURN statement if you want to incorporate the model in a query without creating a function.");

      //Generate function header
      strBuilder.Append("CREATE FUNCTION dbo.REGRESSIONMODEL(");
      strBuilder.Append(string.Join(", ", sortedVarcharIdentifiers.Select(n => string.Format("{0} NVARCHAR(max)",n))));
      if (varcharVarNames.Any() && floatVarNames.Any())
        strBuilder.Append(",");
      strBuilder.Append(string.Join(", ", sortedFloatIdentifiers.Select(n => string.Format("{0} FLOAT",n))));
      strBuilder.AppendLine(")");

      //start function body
      strBuilder.AppendLine("RETURNS FLOAT");
      strBuilder.AppendLine("BEGIN");

      //add variable declaration for convenience
      strBuilder.AppendLineIndented(1, "-- added variable declaration for convenience");
      foreach (var name in sortedVarcharIdentifiers)
        strBuilder.AppendLineIndented(1, string.Format("-- DECLARE {0} NVARCHAR(max) = ''", name));
      foreach (var name in sortedFloatIdentifiers)
        strBuilder.AppendLineIndented(1, string.Format("-- DECLARE {0} FLOAT = 0.0", name));
      strBuilder.AppendLineIndented(1, "-- SELECT");
      strBuilder.AppendLine("RETURN ");
    }

    private void FormatRecursively(int level, ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.Subtrees.Any()) {
        if (node.Symbol is Addition) {
          FormatOperator(level, node, "+", strBuilder);
        } else if (node.Symbol is And) {
          FormatOperator(level, node, "AND", strBuilder);
        } else if (node.Symbol is Average) {
          FormatAverage(level, node, strBuilder);
        } else if (node.Symbol is Cosine) {
          FormatFunction(level, node, "COS", strBuilder);
        } else if (node.Symbol is Division) {
          FormatDivision(level, node, strBuilder);
        } else if (node.Symbol is Exponential) {
          FormatFunction(level, node, "EXP", strBuilder);
        } else if (node.Symbol is GreaterThan) {
          FormatOperator(level, node, ">", strBuilder);
        } else if (node.Symbol is IfThenElse) {
          FormatIfThenElse(level, node, strBuilder);
        } else if (node.Symbol is LessThan) {
          FormatOperator(level, node, "<", strBuilder);
        } else if (node.Symbol is Logarithm) {
          FormatFunction(level, node, "LOG", strBuilder);
        } else if (node.Symbol is Multiplication) {
          FormatOperator(level, node, "*", strBuilder);
        } else if (node.Symbol is Not) {
          FormatOperator(level, node, "NOT LIKE", strBuilder);
        } else if (node.Symbol is Or) {
          FormatOperator(level, node, "OR", strBuilder);
        } else if (node.Symbol is Xor) {
          throw new NotSupportedException(string.Format("Symbol {0} not yet supported.", node.Symbol.GetType().Name));
        } else if (node.Symbol is Sine) {
          FormatFunction(level, node, "SIN", strBuilder);
        } else if (node.Symbol is Subtraction) {
          FormatSubtraction(level, node, strBuilder);
        } else if (node.Symbol is Tangent) {
          FormatFunction(level, node, "TAN", strBuilder);
        } else if (node.Symbol is Square) {
          FormatFunction(level, node, "SQUARE", strBuilder);
        } else if (node.Symbol is SquareRoot) {
          FormatFunction(level, node, "SQRT", strBuilder);
        } else if (node.Symbol is Power) {
          FormatFunction(level, node, "POWER", strBuilder);
        } else if (node.Symbol is Root) {
          FormatRoot(level, node, strBuilder);
        } else {
          throw new NotSupportedException("Formatting of symbol: " + node.Symbol + " not supported for TSQL symbolic expression tree formatter.");
        }
      } else {
        if (node is VariableTreeNode) {
          var varNode = node as VariableTreeNode;
          strBuilder.AppendFormat("{0} * {1}", VariableName2Identifier(varNode.VariableName), varNode.Weight.ToString("g17", CultureInfo.InvariantCulture));
        } else if (node is ConstantTreeNode) {
          var constNode = node as ConstantTreeNode;
          strBuilder.Append(constNode.Value.ToString("g17", CultureInfo.InvariantCulture));
        } else if (node.Symbol is FactorVariable) {
          var factorNode = node as FactorVariableTreeNode;
          FormatFactor(level, factorNode, strBuilder);
        } else if (node.Symbol is BinaryFactorVariable) {
          var binFactorNode = node as BinaryFactorVariableTreeNode;
          throw new NotSupportedException(string.Format("Symbol {0} not yet supported.", node.Symbol.GetType().Name));
        } else {
          throw new NotSupportedException("Formatting of symbol: " + node.Symbol + " not supported for TSQL symbolic expression tree formatter.");
        }
      }
    }

    private void FormatIfThenElse(int level, ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("CASE ISNULL((SELECT 1 WHERE");
      FormatRecursively(level,node.GetSubtree(0), strBuilder);
      strBuilder.AppendLine("),0)");
      strBuilder.AppendIndented(level,"WHEN 1 THEN ");
      FormatRecursively(level, node.GetSubtree(1), strBuilder);
      strBuilder.AppendLine();
      strBuilder.AppendIndented(level, "WHEN 0 THEN ");
      FormatRecursively(level, node.GetSubtree(2), strBuilder);
      strBuilder.AppendLine();
      strBuilder.AppendIndented(level, "END");
    }

    private void FormatAverage(int level, ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("(");
      foreach (var child in node.Subtrees) {
        FormatRecursively(level, child, strBuilder);
        if (child != node.Subtrees.Last())
          strBuilder.Append(" + ");
      }
      strBuilder.AppendFormat(") / {0}", node.Subtrees.Count());
    }

    private string VariableName2Identifier(string variableName) {
      return "@"+variableName.Replace(' ', '_');
    }

    private void GenerateFooter(StringBuilder strBuilder) {
      strBuilder.Append(Environment.NewLine);
      strBuilder.AppendLine("END");
    }


    private void FormatOperator(int level, ISymbolicExpressionTreeNode node, string symbol, StringBuilder strBuilder) {
      strBuilder.Append("(");
      foreach (var child in node.Subtrees) {
        FormatRecursively(level, child, strBuilder);
        if (child != node.Subtrees.Last())
          strBuilder.Append(" " + symbol + " ");
      }
      strBuilder.Append(")");
    }

    private void FormatFunction(int level, ISymbolicExpressionTreeNode node, string function, StringBuilder strBuilder) {
      strBuilder.Append(function + "(");
      foreach (var child in node.Subtrees) {
        FormatRecursively(level++, child, strBuilder);
        if (child != node.Subtrees.Last())
          strBuilder.Append(", ");
      }
      strBuilder.Append(")");
    }

    private void FormatDivision(int level, ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.SubtreeCount == 1) {
        strBuilder.Append("1.0 / ");
        FormatRecursively(level, node.GetSubtree(0), strBuilder);
      } else {
        FormatRecursively(level, node.GetSubtree(0), strBuilder);
        strBuilder.Append("/ (");
        for (int i = 1; i < node.SubtreeCount; i++) {
          if (i > 1) strBuilder.Append(" * ");
          FormatRecursively(level, node.GetSubtree(i), strBuilder);
        }
        strBuilder.Append(")");
      }
    }

    private void FormatSubtraction(int level, ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.SubtreeCount == 1) {
        strBuilder.Append("-");
        FormatRecursively(level, node.GetSubtree(0), strBuilder);
        return;
      }
      //Default case: more than 1 child
      FormatOperator(level, node, "-", strBuilder);
    }

    private void FormatRoot(int level, ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.AppendLine("POWER(");
      FormatRecursively(level, node.GetSubtree(0), strBuilder);
      strBuilder.AppendLineIndented(level, " , 1.0 / (");
      FormatRecursively(level, node.GetSubtree(1), strBuilder);
      strBuilder.AppendLineIndented(level, "))");
    }

    private void FormatFactor(int level, FactorVariableTreeNode node, StringBuilder strBuilder) {
      strBuilder.AppendLine("( ");
      strBuilder.AppendLineIndented(level + 1, string.Format("CASE {0}", VariableName2Identifier(node.VariableName)));
      foreach (var name in node.Symbol.GetVariableValues(node.VariableName)) {
        strBuilder.AppendLineIndented(level + 2, string.Format("WHEN '{0}' THEN {1}", name, node.GetValue(name).ToString(CultureInfo.InvariantCulture)));
      }
      strBuilder.AppendLineIndented(level + 1, "ELSE NULL END");
      strBuilder.AppendIndented(level, ")");
    }

  }
}
