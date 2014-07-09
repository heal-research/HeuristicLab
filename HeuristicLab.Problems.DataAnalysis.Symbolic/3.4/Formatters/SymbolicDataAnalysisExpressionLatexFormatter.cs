#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("LaTeX String Formatter", "Formatter for symbolic expression trees for import into LaTeX documents.")]
  [StorableClass]
  public sealed class SymbolicDataAnalysisExpressionLatexFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {
    private readonly List<double> constants;
    private int targetCount;
    private int currentLag;

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionLatexFormatter(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisExpressionLatexFormatter(SymbolicDataAnalysisExpressionLatexFormatter original, Cloner cloner)
      : base(original, cloner) {
      constants = new List<double>(original.constants);
    }
    public SymbolicDataAnalysisExpressionLatexFormatter()
      : base() {
      Name = ItemName;
      Description = ItemDescription;
      constants = new List<double>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionLatexFormatter(this, cloner);
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      try {
        StringBuilder strBuilder = new StringBuilder();
        constants.Clear();
        strBuilder.AppendLine(FormatRecursively(symbolicExpressionTree.Root));
        return strBuilder.ToString();
      }
      catch (NotImplementedException ex) {
        return ex.Message + Environment.NewLine + ex.StackTrace;
      }
    }

    private string FormatRecursively(ISymbolicExpressionTreeNode node) {
      StringBuilder strBuilder = new StringBuilder();
      currentLag = 0;
      FormatBegin(node, strBuilder);

      if (node.SubtreeCount > 0) {
        strBuilder.Append(FormatRecursively(node.GetSubtree(0)));
      }
      int i = 1;
      foreach (SymbolicExpressionTreeNode subTree in node.Subtrees.Skip(1)) {
        FormatSep(node, strBuilder, i);
        // format the whole subtree
        strBuilder.Append(FormatRecursively(subTree));
        i++;
      }

      FormatEnd(node, strBuilder);

      return strBuilder.ToString();
    }

    private void FormatBegin(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.Symbol is Addition) {
        strBuilder.Append(@" \left( ");
      } else if (node.Symbol is Subtraction) {
        if (node.SubtreeCount == 1) {
          strBuilder.Append(@"- \left( ");
        } else {
          strBuilder.Append(@" \left( ");
        }
      } else if (node.Symbol is Multiplication) {
      } else if (node.Symbol is Division) {
        if (node.SubtreeCount == 1) {
          strBuilder.Append(@" \cfrac{1");
        } else {
          strBuilder.Append(@" \cfrac{ ");
        }
      } else if (node.Symbol is Average) {
        // skip output of (1/1) if only one subtree
        if (node.SubtreeCount > 1) {
          strBuilder.Append(@" \cfrac{1}{" + node.SubtreeCount + @"}");
        }
        strBuilder.Append(@" \left( ");
      } else if (node.Symbol is Logarithm) {
        strBuilder.Append(@"\log \left( ");
      } else if (node.Symbol is Exponential) {
        strBuilder.Append(@"\exp \left( ");
      } else if (node.Symbol is Square) {
        strBuilder.Append(@"\left(");
      } else if (node.Symbol is SquareRoot) {
        strBuilder.Append(@"\sqrt{");
      } else if (node.Symbol is Sine) {
        strBuilder.Append(@"\sin \left( ");
      } else if (node.Symbol is Cosine) {
        strBuilder.Append(@"\cos \left( ");
      } else if (node.Symbol is Tangent) {
        strBuilder.Append(@"\tan \left( ");
      } else if (node.Symbol is AiryA) {
        strBuilder.Append(@"\operatorname{airy}_a \left( ");
      } else if (node.Symbol is AiryB) {
        strBuilder.Append(@"\operatorname{airy}_b \left( ");
      } else if (node.Symbol is Bessel) {
        strBuilder.Append(@"\operatorname{bessel}_1 \left( ");
      } else if (node.Symbol is CosineIntegral) {
        strBuilder.Append(@"\operatorname{cosInt} \left( ");
      } else if (node.Symbol is Dawson) {
        strBuilder.Append(@"\operatorname{dawson} \left( ");
      } else if (node.Symbol is Erf) {
        strBuilder.Append(@"\operatorname{erf} \left( ");
      } else if (node.Symbol is ExponentialIntegralEi) {
        strBuilder.Append(@"\operatorname{expInt}_i \left( ");
      } else if (node.Symbol is FresnelCosineIntegral) {
        strBuilder.Append(@"\operatorname{fresnel}_\operatorname{cosInt} \left( ");
      } else if (node.Symbol is FresnelSineIntegral) {
        strBuilder.Append(@"\operatorname{fresnel}_\operatorname{sinInt} \left( ");
      } else if (node.Symbol is Gamma) {
        strBuilder.Append(@"\Gamma \left( ");
      } else if (node.Symbol is HyperbolicCosineIntegral) {
        strBuilder.Append(@"\operatorname{hypCosInt} \left( ");
      } else if (node.Symbol is HyperbolicSineIntegral) {
        strBuilder.Append(@"\operatorname{hypSinInt} \left( ");
      } else if (node.Symbol is Norm) {
        strBuilder.Append(@"\operatorname{norm} \left( ");
      } else if (node.Symbol is Psi) {
        strBuilder.Append(@"\operatorname{digamma} \left( ");
      } else if (node.Symbol is SineIntegral) {
        strBuilder.Append(@"\operatorname{sinInt} \left( ");
      } else if (node.Symbol is GreaterThan) {
        strBuilder.Append(@"  \left( ");
      } else if (node.Symbol is LessThan) {
        strBuilder.Append(@"  \left( ");
      } else if (node.Symbol is And) {
        strBuilder.Append(@"  \left( \left( ");
      } else if (node.Symbol is Or) {
        strBuilder.Append(@"   \left( \left( ");
      } else if (node.Symbol is Not) {
        strBuilder.Append(@" \neg \left( ");
      } else if (node.Symbol is IfThenElse) {
        strBuilder.Append(@" \operatorname{if}  \left( ");
      } else if (node.Symbol is Constant) {
        strBuilder.Append("c_{" + constants.Count + "} ");
        var constNode = node as ConstantTreeNode;
        constants.Add(constNode.Value);
      } else if (node.Symbol is LaggedVariable) {
        var laggedVarNode = node as LaggedVariableTreeNode;
        if (!laggedVarNode.Weight.IsAlmost(1.0)) {
          strBuilder.Append("c_{" + constants.Count + "} \\cdot ");
          constants.Add(laggedVarNode.Weight);
        }
        strBuilder.Append(EscapeLatexString(laggedVarNode.VariableName));
        strBuilder.Append(LagToString(currentLag + laggedVarNode.Lag));

      } else if (node.Symbol is Variable) {
        var varNode = node as VariableTreeNode;
        if (!varNode.Weight.IsAlmost((1.0))) {
          strBuilder.Append("c_{" + constants.Count + "} \\cdot ");
          constants.Add(varNode.Weight);
        }
        strBuilder.Append(EscapeLatexString(varNode.VariableName));
        strBuilder.Append(LagToString(currentLag));
      } else if (node.Symbol is ProgramRootSymbol) {
        strBuilder
          .AppendLine("\\begin{align*}")
          .AppendLine("\\nonumber");
      } else if (node.Symbol is Defun) {
        var defunNode = node as DefunTreeNode;
        strBuilder.Append(defunNode.FunctionName + " & = ");
      } else if (node.Symbol is InvokeFunction) {
        var invokeNode = node as InvokeFunctionTreeNode;
        strBuilder.Append(invokeNode.Symbol.FunctionName + @" \left( ");
      } else if (node.Symbol is StartSymbol) {
        strBuilder.Append("target_" + (targetCount++) + "(t) & = ");
      } else if (node.Symbol is Argument) {
        var argSym = node.Symbol as Argument;
        strBuilder.Append(" ARG+" + argSym.ArgumentIndex + " ");
      } else if (node.Symbol is Derivative) {
        strBuilder.Append(@" \cfrac{d \left( ");
      } else if (node.Symbol is TimeLag) {
        var laggedNode = node as ILaggedTreeNode;
        currentLag += laggedNode.Lag;
      } else if (node.Symbol is Power) {
        strBuilder.Append(@" \left( ");
      } else if (node.Symbol is Root) {
        strBuilder.Append(@" \left( ");
      } else if (node.Symbol is Integral) {
        // actually a new variable for t is needed in all subtrees (TODO)
        var laggedTreeNode = node as ILaggedTreeNode;
        strBuilder.Append(@"\sum_{t=" + (laggedTreeNode.Lag + currentLag) + @"}^0 \left( ");
      } else if (node.Symbol is VariableCondition) {
        var conditionTreeNode = node as VariableConditionTreeNode;
        string p = @"1 /  1 + \exp  - c_{" + constants.Count + "} ";
        constants.Add(conditionTreeNode.Slope);
        p += @" \cdot " + EscapeLatexString(conditionTreeNode.VariableName) + LagToString(currentLag) + " - c_{" + constants.Count + @"}   ";
        constants.Add(conditionTreeNode.Threshold);
        strBuilder.Append(@" \left( " + p + @"\cdot ");
      } else {
        throw new NotImplementedException("Export of " + node.Symbol + " is not implemented.");
      }
    }

    private void FormatSep(ISymbolicExpressionTreeNode node, StringBuilder strBuilder, int step) {
      if (node.Symbol is Addition) {
        strBuilder.Append(" + ");
      } else if (node.Symbol is Subtraction) {
        strBuilder.Append(" - ");
      } else if (node.Symbol is Multiplication) {
        strBuilder.Append(@" \cdot ");
      } else if (node.Symbol is Division) {
        if (step + 1 == node.SubtreeCount)
          strBuilder.Append(@"}{");
        else
          strBuilder.Append(@" }{ \cfrac{ ");
      } else if (node.Symbol is Average) {
        strBuilder.Append(@" + ");
      } else if (node.Symbol is Logarithm) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Exponential) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Square) {
        throw new InvalidOperationException();
      } else if (node.Symbol is SquareRoot) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Sine) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Cosine) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Tangent) {
        throw new InvalidOperationException();
      } else if (node.Symbol is AiryA) {
        throw new InvalidOperationException();
      } else if (node.Symbol is AiryB) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Bessel) {
        throw new InvalidOperationException();
      } else if (node.Symbol is CosineIntegral) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Dawson) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Erf) {
        throw new InvalidOperationException();
      } else if (node.Symbol is ExponentialIntegralEi) {
        throw new InvalidOperationException();
      } else if (node.Symbol is FresnelCosineIntegral) {
        throw new InvalidOperationException();
      } else if (node.Symbol is FresnelSineIntegral) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Gamma) {
        throw new InvalidOperationException();
      } else if (node.Symbol is HyperbolicCosineIntegral) {
        throw new InvalidOperationException();
      } else if (node.Symbol is HyperbolicSineIntegral) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Norm) {
        throw new InvalidOperationException();
      } else if (node.Symbol is Psi) {
        throw new InvalidOperationException();
      } else if (node.Symbol is SineIntegral) {
        throw new InvalidOperationException();
      } else if (node.Symbol is GreaterThan) {
        strBuilder.Append(@" > ");
      } else if (node.Symbol is LessThan) {
        strBuilder.Append(@" < ");
      } else if (node.Symbol is And) {
        strBuilder.Append(@" > 0  \right) \land \left(");
      } else if (node.Symbol is Or) {
        strBuilder.Append(@" > 0  \right) \lor \left(");
      } else if (node.Symbol is Not) {
        throw new InvalidOperationException();
      } else if (node.Symbol is IfThenElse) {
        strBuilder.Append(@" , ");
      } else if (node.Symbol is ProgramRootSymbol) {
        strBuilder.Append(@"\\" + Environment.NewLine);
      } else if (node.Symbol is Defun) {
      } else if (node.Symbol is InvokeFunction) {
        strBuilder.Append(" , ");
      } else if (node.Symbol is StartSymbol) {
        strBuilder.Append(@"\\" + Environment.NewLine);
        strBuilder.Append("target_" + (targetCount++) + "(t) & = ");
      } else if (node.Symbol is Power) {
        strBuilder.Append(@"\right) ^ { \operatorname{round} \left(");
      } else if (node.Symbol is Root) {
        strBuilder.Append(@"\right) ^ {  \cfrac{1}{ \operatorname{round} \left(");
      } else if (node.Symbol is VariableCondition) {
        var conditionTreeNode = node as VariableConditionTreeNode;
        string p = @"1 / \left( 1 + \exp \left( - c_{" + constants.Count + "} ";
        constants.Add(conditionTreeNode.Slope);
        p += @" \cdot " + EscapeLatexString(conditionTreeNode.VariableName) + LagToString(currentLag) + " - c_{" + constants.Count + @"} \right) \right) \right)   ";
        constants.Add(conditionTreeNode.Threshold);
        strBuilder.Append(@" +  \left( 1 - " + p + @" \right) \cdot ");
      } else {
        throw new NotImplementedException("Export of " + node.Symbol + " is not implemented.");
      }
    }

    private void FormatEnd(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.Symbol is Addition) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Subtraction) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Multiplication) {
      } else if (node.Symbol is Division) {
        strBuilder.Append(" } ");
        for (int i = 2; i < node.SubtreeCount; i++)
          strBuilder.Append(" } ");
      } else if (node.Symbol is Average) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Logarithm) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Exponential) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Square) {
        strBuilder.Append(@"\right)^2");
      } else if (node.Symbol is SquareRoot) {
        strBuilder.Append(@"}");
      } else if (node.Symbol is Sine) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Cosine) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Tangent) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is AiryA) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is AiryB) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Bessel) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is CosineIntegral) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Dawson) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Erf) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is ExponentialIntegralEi) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is FresnelCosineIntegral) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is FresnelSineIntegral) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Gamma) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is HyperbolicCosineIntegral) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is HyperbolicSineIntegral) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Norm) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Psi) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is SineIntegral) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is GreaterThan) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is LessThan) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is And) {
        strBuilder.Append(@" > 0 \right) \right) ");
      } else if (node.Symbol is Or) {
        strBuilder.Append(@" > 0 \right) \right) ");
      } else if (node.Symbol is Not) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is IfThenElse) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is Constant) {
      } else if (node.Symbol is LaggedVariable) {
      } else if (node.Symbol is Variable) {
      } else if (node.Symbol is ProgramRootSymbol) {
        strBuilder
          .AppendLine("\\end{align*}")
          .AppendLine("\\begin{align*}")
          .AppendLine("\\nonumber");
        // output all constant values
        if (constants.Count > 0) {
          int i = 0;
          foreach (var constant in constants) {
            // replace "." with ".&" to align decimal points
            var constStr = string.Format(System.Globalization.NumberFormatInfo.InvariantInfo, "{0:G5}", constant);
            if (!constStr.Contains(".")) constStr = constStr + ".0";
            constStr = constStr.Replace(".", "\\negthickspace&.");  // fix problem in rendering of aligned expressions
            strBuilder.Append("c_{" + i + "}& = & " + constStr);
            strBuilder.Append(@"\\");
            i++;
          }
        }
        strBuilder.AppendLine("\\end{align*}");
      } else if (node.Symbol is Defun) {
      } else if (node.Symbol is InvokeFunction) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is StartSymbol) {
      } else if (node.Symbol is Argument) {
      } else if (node.Symbol is Derivative) {
        strBuilder.Append(@" \right) }{dt} ");
      } else if (node.Symbol is TimeLag) {
        var laggedNode = node as ILaggedTreeNode;
        currentLag -= laggedNode.Lag;
      } else if (node.Symbol is Power) {
        strBuilder.Append(@" \right) } ");
      } else if (node.Symbol is Root) {
        strBuilder.Append(@" \right) } } ");
      } else if (node.Symbol is Integral) {
        strBuilder.Append(@" \right) ");
      } else if (node.Symbol is VariableCondition) {
        strBuilder.Append(@"\right) ");
      } else {
        throw new NotImplementedException("Export of " + node.Symbol + " is not implemented.");
      }
    }

    private string LagToString(int lag) {
      if (lag < 0) {
        return "(t" + lag + ")";
      } else if (lag > 0) {
        return "(t+" + lag + ")";
      } else return "";
    }

    private string EscapeLatexString(string s) {
      return "\\text{" +
        s
         .Replace("\\", "\\\\")
         .Replace("{", "\\{")
         .Replace("}", "\\}")
        + "}";
    }
  }
}
