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
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using Variable = HeuristicLab.Problems.DataAnalysis.Symbolic.Variable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Formatters {

  [Item("Smalltalk String Formatter", "String formatter for string representations of symbolic expression trees in Smalltalk syntax.")]
  public class SymbolicExpressionTreeSmalltalkStringFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {

    protected SymbolicExpressionTreeSmalltalkStringFormatter(SymbolicExpressionTreeSmalltalkStringFormatter original, Cloner cloner) : base(original, cloner) { }
    public SymbolicExpressionTreeSmalltalkStringFormatter()
      : base() {
      Name = "Smalltalk String Formatter";
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      return FormatRecursively(symbolicExpressionTree.Root);
    }

    private string FormatRecursively(ISymbolicExpressionTreeNode node) {

      ISymbol symbol = node.Symbol;

      if (symbol is ProgramRootSymbol || symbol is StartSymbol)
        return FormatRecursively(node.GetSubtree(0));

      StringBuilder stringBuilder = new StringBuilder();

      stringBuilder.Append("(");

      if (symbol is Addition) {
        for (int i = 0; i < node.SubtreeCount; i++) {
          if (i > 0) stringBuilder.Append("+");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
        }
      } else if (symbol is And) {
        stringBuilder.Append("(");
        for (int i = 0; i < node.SubtreeCount; i++) {
          if (i > 0) stringBuilder.Append("&");
          stringBuilder.Append("((");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
          stringBuilder.Append(")>=0)");
        }
        stringBuilder.Append(") ifTrue:[1] ifFalse:[-1]");
      } else if (symbol is Average) {
        stringBuilder.Append("(1/");
        stringBuilder.Append(node.SubtreeCount);
        stringBuilder.Append(")*(");
        for (int i = 0; i < node.SubtreeCount; i++) {
          if (i > 0) stringBuilder.Append("+");
          stringBuilder.Append("(");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
          stringBuilder.Append(")");
        }
        stringBuilder.Append(")");
      } else if (symbol is Constant) {
        ConstantTreeNode constantTreeNode = node as ConstantTreeNode;
        stringBuilder.Append(constantTreeNode.Value.ToString().Replace(",", "."));
      } else if (symbol is Cosine) {
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(" cos");
      } else if (symbol is Division) {
        if (node.SubtreeCount == 1) {
          stringBuilder.Append("1/");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        } else {
          stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
          stringBuilder.Append("/(");
          for (int i = 1; i < node.SubtreeCount; i++) {
            if (i > 1) stringBuilder.Append("*");
            stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
          }
          stringBuilder.Append(")");
        }
      } else if (symbol is Exponential) {
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(" exp");
      } else if (symbol is GreaterThan) {
        stringBuilder.Append("(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(" > ");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(1)));
        stringBuilder.Append(") ifTrue: [1] ifFalse: [-1]");
      } else if (symbol is IfThenElse) {
        stringBuilder.Append("(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(" > 0) ifTrue: [");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(1)));
        stringBuilder.Append("] ifFalse: [");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(2)));
        stringBuilder.Append("]");
      } else if (symbol is LaggedVariable) {
        stringBuilder.Append("not implemented");
      } else if (symbol is LessThan) {
        stringBuilder.Append("(");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(" < ");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(1)));
        stringBuilder.Append(") ifTrue: [1] ifFalse: [-1]");
      } else if (symbol is Logarithm) {
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append("ln");
      } else if (symbol is Multiplication) {
        for (int i = 0; i < node.SubtreeCount; i++) {
          if (i > 0) stringBuilder.Append("*");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
        }
      } else if (symbol is Not) {
        stringBuilder.Append("-1*");
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
      } else if (symbol is Or) {
        stringBuilder.Append("(");
        for (int i = 0; i < node.SubtreeCount; i++) {
          if (i > 0) stringBuilder.Append("|");
          stringBuilder.Append("((");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
          stringBuilder.Append(")>=0)");
        }
        stringBuilder.Append(") ifTrue:[1] ifFalse:[-1]");
      } else if (symbol is Sine) {
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append(" sin");
      } else if (symbol is Subtraction) {
        if (node.SubtreeCount == 1) {
          stringBuilder.Append("-1*");
          stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        } else {
          stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
          for (int i = 1; i < node.SubtreeCount; i++) {
            stringBuilder.Append("-");
            stringBuilder.Append(FormatRecursively(node.GetSubtree(i)));
          }
        }
      } else if (symbol is Tangent) {
        stringBuilder.Append(FormatRecursively(node.GetSubtree(0)));
        stringBuilder.Append("tan");
      } else if (symbol is Variable) {
        VariableTreeNode variableTreeNode = node as VariableTreeNode;
        stringBuilder.Append(variableTreeNode.Weight.ToString().Replace(",", "."));
        stringBuilder.Append("*");
        stringBuilder.Append(variableTreeNode.VariableName);
      } else {
        stringBuilder.Append("ERROR");
      }

      stringBuilder.Append(")");

      return stringBuilder.ToString();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeSmalltalkStringFormatter(this, cloner);
    }
  }
}
