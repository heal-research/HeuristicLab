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
  [Item("Python String Formatter", "String formatter for string representations of symbolic data analysis expressions in Python syntax.")]
  [StorableType("37C1E1DD-437F-414B-AA96-9C6A0F6FEE46")]
  public sealed class SymbolicDataAnalysisExpressionPythonFormatter : NamedItem, ISymbolicExpressionTreeStringFormatter {

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionPythonFormatter(StorableConstructorFlag _) : base(_) { }
    private SymbolicDataAnalysisExpressionPythonFormatter(SymbolicDataAnalysisExpressionPythonFormatter original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisExpressionPythonFormatter()
      : base() {
      Name = ItemName;
      Description = ItemDescription;
    }

    public override IDeepCloneable Clone(Cloner cloner) => new SymbolicDataAnalysisExpressionPythonFormatter(this, cloner);

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      StringBuilder strBuilderModel = new StringBuilder();
      var header = GenerateHeader(symbolicExpressionTree);
      FormatRecursively(symbolicExpressionTree.Root, strBuilderModel);
      return $"{header}{strBuilderModel}";
    }

    public static string FormatTree(ISymbolicExpressionTree symbolicExpressionTree) {
      var formatter = new SymbolicDataAnalysisExpressionPythonFormatter();
      return formatter.Format(symbolicExpressionTree);
    }

    private static string GenerateHeader(ISymbolicExpressionTree symbolicExpressionTree) {
      StringBuilder strBuilder = new StringBuilder();

      ISet<string> variables = new HashSet<string>();
      int mathLibCounter = 0;
      int statisticLibCounter = 0;
      int evaluateIfCounter = 0;

      // iterate tree and search for necessary imports and variable names
      foreach (var node in symbolicExpressionTree.IterateNodesPostfix()) {
        ISymbol symbol = node.Symbol;
        if (symbol is Average) statisticLibCounter++;
        else if (symbol is IfThenElse) evaluateIfCounter++;
        else if (symbol is Cosine) mathLibCounter++;
        else if (symbol is Exponential) mathLibCounter++;
        else if (symbol is Logarithm) mathLibCounter++;
        else if (symbol is Sine) mathLibCounter++;
        else if (symbol is Tangent) mathLibCounter++;
        else if (symbol is HyperbolicTangent) mathLibCounter++;
        else if (symbol is SquareRoot) mathLibCounter++;
        else if (symbol is Power) mathLibCounter++;
        else if (symbol is AnalyticQuotient) mathLibCounter++;
        else if (node is VariableTreeNode) {
          var varNode = node as VariableTreeNode;
          var formattedVariable = VariableName2Identifier(varNode.VariableName);
          variables.Add(formattedVariable);
        }
      }

      // generate import section (if necessary)
      var importSection = GenerateNecessaryImports(mathLibCounter, statisticLibCounter);
      strBuilder.Append(importSection);

      // generate if-then-else helper construct (if necessary)
      var ifThenElseSourceSection = GenerateIfThenElseSource(evaluateIfCounter);
      strBuilder.Append(ifThenElseSourceSection);

      // generate model evaluation function
      var modelEvaluationFunctionSection = GenerateModelEvaluationFunction(variables);
      strBuilder.Append(modelEvaluationFunctionSection);

      return strBuilder.ToString();
    }

    private static string GenerateNecessaryImports(int mathLibCounter, int statisticLibCounter) {
      StringBuilder strBuilder = new StringBuilder();
      if (mathLibCounter > 0 || statisticLibCounter > 0) {
        strBuilder.AppendLine("# imports");
        if (mathLibCounter > 0)
          strBuilder.AppendLine("import math");
        if (statisticLibCounter > 0)
          strBuilder.AppendLine("import statistics");
        strBuilder.AppendLine();
      }
      return strBuilder.ToString();
    }

    private static string GenerateIfThenElseSource(int evaluateIfCounter) {
      StringBuilder strBuilder = new StringBuilder();
      if (evaluateIfCounter > 0) {
        strBuilder.AppendLine("# condition helper function");
        strBuilder.AppendLine("def evaluate_if(condition, then_path, else_path): ");
        strBuilder.AppendLine("\tif condition:");
        strBuilder.AppendLine("\t\treturn then_path");
        strBuilder.AppendLine("\telse:");
        strBuilder.AppendLine("\t\treturn else_path");
      }
      return strBuilder.ToString();
    }

    private static string GenerateModelEvaluationFunction(ISet<string> variables) {
      StringBuilder strBuilder = new StringBuilder();
      strBuilder.Append("def evaluate(");
      var orderedVariables = variables.OrderBy(n => n, new NaturalStringComparer());
      foreach (var variable in orderedVariables) {
        strBuilder.Append($"{variable}");
        if (variable != orderedVariables.Last())
          strBuilder.Append(", ");
      }
      strBuilder.AppendLine("):");
      strBuilder.Append("\treturn ");
      return strBuilder.ToString();
    }

    private static void FormatRecursively(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      ISymbol symbol = node.Symbol;
      if (symbol is ProgramRootSymbol)
        FormatRecursively(node.GetSubtree(0), strBuilder);
      else if (symbol is StartSymbol)
        FormatRecursively(node.GetSubtree(0), strBuilder);
      else if (symbol is Absolute)
        FormatNode(node, strBuilder, "abs");
      else if (symbol is Addition)
        FormatNode(node, strBuilder, infixSymbol: " + ");
      else if (symbol is Subtraction)
        FormatSubtraction(node, strBuilder);
      else if (symbol is Multiplication)
        FormatNode(node, strBuilder, infixSymbol: " * ");
      else if (symbol is Division)
        FormatDivision(node, strBuilder);
      else if (symbol is Average)
        FormatNode(node, strBuilder, prefixSymbol: "statistics.mean", openingSymbol: "([", closingSymbol: "])");
      else if (symbol is Sine)
        FormatNode(node, strBuilder, "math.sin");
      else if (symbol is Cosine)
        FormatNode(node, strBuilder, "math.cos");
      else if (symbol is Tangent)
        FormatNode(node, strBuilder, "math.tan");
      else if (symbol is HyperbolicTangent)
        FormatNode(node, strBuilder, "math.tanh");
      else if (symbol is Exponential)
        FormatNode(node, strBuilder, "math.exp");
      else if (symbol is Logarithm)
        FormatNode(node, strBuilder, "math.log");
      else if (symbol is Power)
        FormatNode(node, strBuilder, "math.pow");
      else if (symbol is Root)
        FormatRoot(node, strBuilder);
      else if (symbol is Square)
        FormatPower(node, strBuilder, "2");
      else if (symbol is SquareRoot)
        FormatNode(node, strBuilder, "math.sqrt");
      else if (symbol is Cube)
        FormatPower(node, strBuilder, "3");
      else if (symbol is CubeRoot)
        FormatNode(node, strBuilder, closingSymbol: " ** (1. / 3))");
      else if (symbol is AnalyticQuotient)
        FormatAnalyticQuotient(node, strBuilder);
      else if (symbol is And)
        FormatNode(node, strBuilder, infixSymbol: " and ");
      else if (symbol is Or)
        FormatNode(node, strBuilder, infixSymbol: " or ");
      else if (symbol is Xor)
        FormatNode(node, strBuilder, infixSymbol: " ^ ");
      else if (symbol is Not)
        FormatNode(node, strBuilder, "not");
      else if (symbol is IfThenElse)
        FormatNode(node, strBuilder, "evaluate_if");
      else if (symbol is GreaterThan)
        FormatNode(node, strBuilder, infixSymbol: " > ");
      else if (symbol is LessThan)
        FormatNode(node, strBuilder, infixSymbol: " < ");
      else if (node is VariableTreeNode)
        FormatVariableTreeNode(node, strBuilder);
      else if (node is INumericTreeNode)
        FormatNumericTreeNode(node, strBuilder);
      else if (symbol is SubFunctionSymbol)
        FormatRecursively(node.GetSubtree(0), strBuilder);
      else
        throw new NotSupportedException("Formatting of symbol: " + symbol + " not supported for Python symbolic expression tree formatter.");
    }

    private static string VariableName2Identifier(string variableName) => variableName.Replace(" ", "_");

    private static void FormatNode(ISymbolicExpressionTreeNode node, StringBuilder strBuilder, string prefixSymbol = "", string openingSymbol = "(", string closingSymbol = ")", string infixSymbol = ",") {
      strBuilder.Append($"{prefixSymbol}{openingSymbol}");
      foreach (var child in node.Subtrees) {
        FormatRecursively(child, strBuilder);
        if (child != node.Subtrees.Last())
          strBuilder.Append(infixSymbol);
      }
      strBuilder.Append(closingSymbol);
    }

    private static void FormatVariableTreeNode(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      var varNode = node as VariableTreeNode;
      var formattedVariable = VariableName2Identifier(varNode.VariableName);
      var variableWeight = varNode.Weight.ToString("g17", CultureInfo.InvariantCulture);
      strBuilder.Append($"{formattedVariable} * {variableWeight}");
    }

    private static void FormatNumericTreeNode(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      var numNode = node as INumericTreeNode;
      strBuilder.Append(numNode.Value.ToString("g17", CultureInfo.InvariantCulture));
    }

    private static void FormatPower(ISymbolicExpressionTreeNode node, StringBuilder strBuilder, string exponent) {
      strBuilder.Append("math.pow(");
      FormatRecursively(node.GetSubtree(0), strBuilder);
      strBuilder.Append($", {exponent})");
    }

    private static void FormatRoot(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("math.pow(");
      FormatRecursively(node.GetSubtree(0), strBuilder);
      strBuilder.Append(", 1.0 / (");
      FormatRecursively(node.GetSubtree(1), strBuilder);
      strBuilder.Append("))");
    }

    private static void FormatSubtraction(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      if (node.SubtreeCount == 1) {
        strBuilder.Append("-");
        FormatRecursively(node.GetSubtree(0), strBuilder);
        return;
      }
      //Default case: more than 1 child
      FormatNode(node, strBuilder, infixSymbol: " - ");
    }

    private static void FormatDivision(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("(");
      if (node.SubtreeCount == 1) {
        strBuilder.Append("1.0 / ");
        FormatRecursively(node.GetSubtree(0), strBuilder);
      } else {
        FormatRecursively(node.GetSubtree(0), strBuilder);
        strBuilder.Append(" / (");
        for (int i = 1; i < node.SubtreeCount; i++) {
          if (i > 1) strBuilder.Append(" * ");
          FormatRecursively(node.GetSubtree(i), strBuilder);
        }
        strBuilder.Append(")");
      }
      strBuilder.Append(")");
    }

    private static void FormatAnalyticQuotient(ISymbolicExpressionTreeNode node, StringBuilder strBuilder) {
      strBuilder.Append("(");
      FormatRecursively(node.GetSubtree(0), strBuilder);
      strBuilder.Append(" / math.sqrt(1 + math.pow(");
      FormatRecursively(node.GetSubtree(1), strBuilder);
      strBuilder.Append(" , 2) ) )");
    }
  }
}
