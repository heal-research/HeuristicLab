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
    /// <summary>
    ///  Performs some basic re-writing steps to simplify the code for formatting. Tree is changed.
    ///  Removes single-argument +, * which have no effect
    ///  Removes SubFunctions (no effect)
    ///  Replaces variables with coefficients by an explicitly multiplication
    ///  Replaces single-argument / with 1 / (..)
    ///  Replaces multi-argument +, *, /, - with nested binary operations
    ///  Rotates operations to remove necessity to group sub-expressions
    /// </summary>
    /// <param name="tree">The tree is changed</param>
    public static void ConvertToBinaryLeftAssoc(ISymbolicExpressionTree tree) {
      ConvertToBinaryLeftAssocRec(tree.Root.GetSubtree(0), tree.Root.GetSubtree(0).GetSubtree(0));
    }

    private static void ConvertToBinaryLeftAssocRec(ISymbolicExpressionTreeNode parent, ISymbolicExpressionTreeNode n) {
      // recurse post-order
      foreach (var subtree in n.Subtrees.ToArray()) ConvertToBinaryLeftAssocRec(n, subtree); // ToArray required as n.Subtrees is changed in method

      if (n is VariableTreeNode varTreeNode && varTreeNode.Weight != 1.0) {
        var mul = new Multiplication().CreateTreeNode();
        var num = (NumberTreeNode)new Number().CreateTreeNode();
        num.Value = varTreeNode.Weight;
        varTreeNode.Weight = 1.0;
        parent.ReplaceSubtree(n, mul);
        mul.AddSubtree(num);
        mul.AddSubtree(varTreeNode);
      } else if (n.Symbol is SubFunctionSymbol) {
        parent.ReplaceSubtree(n, n.GetSubtree(0));
      } else if (n.SubtreeCount == 1 && (n.Symbol is Addition || n.Symbol is Multiplication || n.Symbol is And || n.Symbol is Or || n.Symbol is Xor)) {
        // single-argument addition or multiplication has no effect -> remove
        parent.ReplaceSubtree(n, n.GetSubtree(0));
      } else if (n.SubtreeCount == 1 && (n.Symbol is Division)) {
        // single-argument division is 1/f(x)
        n.InsertSubtree(0, new Constant() { Value = 1.0 }.CreateTreeNode());
      } else if (n.SubtreeCount > 2 && IsLeftAssocOp(n.Symbol)) {
        // multi-argument +, -, *, / are the same as multiple binary operations (left-associative)
        var sy = n.Symbol;

        var additionalTrees = n.Subtrees.Skip(2).ToArray();
        while (n.SubtreeCount > 2) n.RemoveSubtree(2); // keep only the first two arguments

        var childIdx = parent.IndexOfSubtree(n);
        parent.RemoveSubtree(childIdx);
        var newChild = n;
        // build a tree bottom to top, each time adding a subtree on the right
        for (int i = 0; i < additionalTrees.Length; i++) {
          var newOp = sy.CreateTreeNode();
          newOp.AddSubtree(newChild);
          newOp.AddSubtree(additionalTrees[i]);
          newChild = newOp;
        }
        parent.InsertSubtree(childIdx, newChild);
      } else if (n.SubtreeCount == 2 && n.GetSubtree(1).SubtreeCount == 2 &&
                 IsAssocOp(n.Symbol) && IsOperator(n.GetSubtree(1).Symbol) &&
                 Priority(n.Symbol) == Priority(n.GetSubtree(1).Symbol)) {
        // f(x) <op> (g(x) <op> h(x))) is the same as  (f(x) <op> g(x)) <op> h(x) for associative <op>
        // which is the same as f(x) <op> g(x) <op> h(x) for left-associative <op>
        // The latter version is preferred because we do not need to write the parentheses.
        // rotation:
        //     (op1)              (op2)
        //     /   \              /  \
        //    a    (op2)       (op1)  c 
        //         /  \        /  \
        //        b    c      a    b      
        var op1 = n;
        var op2 = n.GetSubtree(1);
        var b = op2.GetSubtree(0); op2.RemoveSubtree(0);
        op1.ReplaceSubtree(op2, b);
        parent.ReplaceSubtree(op1, op2);
        op2.InsertSubtree(0, op1);
      }
    }
    public static void FormatRecursively(ISymbolicExpressionTreeNode node, StringBuilder strBuilder,
                                          NumberFormatInfo numberFormat, string formatString, List<KeyValuePair<string, double>> parameters = null) {
      // This method assumes that the tree has been converted to binary and left-assoc form (see ConvertToBinaryLeftAssocRec). 
      if (node.SubtreeCount == 0) {
        // no subtrees
        if (node.Symbol is Variable) {
          var varNode = node as VariableTreeNode;
          if (varNode.Weight != 1.0) { throw new NotSupportedException("Infix formatter does not support variables with coefficients."); }
          AppendVariableName(strBuilder, varNode.VariableName);
        } else if (node is INumericTreeNode numNode) {
          var parenthesisRequired = RequiresParenthesis(node.Parent, node);
          if (parenthesisRequired) strBuilder.Append("(");

          AppendNumber(strBuilder, parameters, numNode.Value, formatString, numberFormat);
          if (parenthesisRequired) strBuilder.Append(")");
        } else if (node.Symbol is LaggedVariable) {
          var varNode = node as LaggedVariableTreeNode;
          if (!varNode.Weight.IsAlmost(1.0)) {
            AppendNumber(strBuilder, parameters, varNode.Weight, formatString, numberFormat);
            strBuilder.Append("*");
          }

          strBuilder.Append("LAG(");
          AppendVariableName(strBuilder, varNode.VariableName);
          strBuilder.Append(", ")
                    .AppendFormat(numberFormat, "{0}", varNode.Lag)
                    .Append(")");
        } else if (node.Symbol is FactorVariable) {
          var factorNode = node as FactorVariableTreeNode;
          AppendVariableName(strBuilder, factorNode.VariableName);

          strBuilder.Append("[");
          for (int i = 0; i < factorNode.Weights.Length; i++) {
            if (i > 0) strBuilder.Append(", ");
            AppendNumber(strBuilder, parameters, factorNode.Weights[i], formatString, numberFormat);
          }
          strBuilder.Append("]");
        } else if (node.Symbol is BinaryFactorVariable) {
          var factorNode = node as BinaryFactorVariableTreeNode;
          if (!factorNode.Weight.IsAlmost(1.0)) {
            AppendNumber(strBuilder, parameters, factorNode.Weight, formatString, numberFormat);
            strBuilder.Append("*");
          }
          AppendVariableName(strBuilder, factorNode.VariableName);
          strBuilder.Append(" = ");
          AppendVariableName(strBuilder, factorNode.VariableValue);
        }
      } else if (node.SubtreeCount == 1) {
        // only functions and single-argument subtraction (=negation) or NOT are possible here.
        var token = GetToken(node.Symbol);
        // the only operators that are allowed with a single argument
        if (node.Symbol is Subtraction || node.Symbol is Not) {
          if (RequiresParenthesis(node.Parent, node)) {
            strBuilder.Append("(");
          }
          strBuilder.Append(token);
          FormatRecursively(node.GetSubtree(0), strBuilder, numberFormat, formatString, parameters);
          if (RequiresParenthesis(node.Parent, node)) {
            strBuilder.Append(")");
          }
        } else if (IsOperator(node.Symbol)) {
          throw new FormatException("Single-argument version of " + node.Symbol.Name + " is not supported.");
        } else {
          // function with only one argument
          strBuilder.Append(token);
          strBuilder.Append("(");
          FormatRecursively(node.GetSubtree(0), strBuilder, numberFormat, formatString, parameters);
          strBuilder.Append(")");
        }
      } else if (node.SubtreeCount > 1) {
        var token = GetToken(node.Symbol);
        // operators
        if (IsOperator(node.Symbol)) {
          var parenthesisRequired = RequiresParenthesis(node.Parent, node);
          if (parenthesisRequired) strBuilder.Append("(");
          FormatRecursively(node.Subtrees.First(), strBuilder, numberFormat, formatString, parameters);

          foreach (var subtree in node.Subtrees.Skip(1)) {
            strBuilder.Append(" ").Append(token).Append(" ");
            FormatRecursively(subtree, strBuilder, numberFormat, formatString, parameters);
          }

          if (parenthesisRequired) strBuilder.Append(")");
        } else {
          // function with multiple arguments (AQ)

          strBuilder.Append(token);
          strBuilder.Append("(");

          FormatRecursively(node.Subtrees.First(), strBuilder, numberFormat, formatString, parameters);
          foreach (var subtree in node.Subtrees.Skip(1)) {
            strBuilder.Append(", ");
            FormatRecursively(subtree, strBuilder, numberFormat, formatString, parameters);
          }
          strBuilder.Append(")");
        }
      }
    }

    private static int Priority(ISymbol symbol) {
      if (symbol is Addition || symbol is Subtraction || symbol is Or || symbol is Xor) return 1;
      if (symbol is Division || symbol is Multiplication || symbol is And) return 2;
      if (symbol is Power || symbol is Not) return 3;
      throw new NotSupportedException();
    }

    private static bool RequiresParenthesis(ISymbolicExpressionTreeNode parent, ISymbolicExpressionTreeNode child) {
      if (child.SubtreeCount > 2 && IsOperator(child.Symbol)) throw new NotSupportedException("Infix formatter does not support operators with more than two children.");

      // Basically: We need a parenthesis for child if the parent symbol binds stronger than child symbol.
      if (parent.Symbol == null || parent.Symbol is ProgramRootSymbol || parent.Symbol is StartSymbol) return false;
      if (IsFunction(parent.Symbol)) return false;
      if (parent.SubtreeCount == 1 && (parent.Symbol is Subtraction)) return true;
      if (child.SubtreeCount == 0) return false;
      var parentPrio = Priority(parent.Symbol);
      var childPrio = Priority(child.Symbol);
      if (parentPrio > childPrio) return true;
      else if (parentPrio == childPrio) {
        if (IsLeftAssocOp(child.Symbol)) return parent.GetSubtree(0) != child; // (..) required only for right child for left-assoc op
        if (IsRightAssocOp(child.Symbol)) return parent.GetSubtree(1) != child;
      }
      return false;
    }

    private static bool IsFunction(ISymbol symbol) {
      // functions are formatted in prefix form e.g. sin(...)
      return !IsOperator(symbol) && !IsLeaf(symbol);
    }

    private static bool IsLeaf(ISymbol symbol) {
      return symbol.MaximumArity == 0;
    }

    private static bool IsOperator(ISymbol symbol) {
      return IsLeftAssocOp(symbol) || IsRightAssocOp(symbol);
    }

    private static bool IsAssocOp(ISymbol symbol) {
      // (a <op> b) <op> c = a <op> (b <op> c)
      return symbol is Addition ||
        symbol is Multiplication ||
        symbol is And ||
        symbol is Or ||
        symbol is Xor;
    }

    private static bool IsLeftAssocOp(ISymbol symbol) {
      // a <op> b <op> c = (a <op> b) <op> c
      return symbol is Addition ||
        symbol is Subtraction ||
        symbol is Multiplication ||
        symbol is Division ||
        symbol is And ||
        symbol is Or ||
        symbol is Xor;
    }

    private static bool IsRightAssocOp(ISymbol symbol) {
      // a <op> b <op> c = a <op> (b <op> c)
      // Negation (single-argument subtraction) is also right-assoc, but we do not have a separate symbol for negation.
      return symbol is Not ||
             symbol is Power; // x^y^z = x^(y^z) (as in Fortran or Mathematica)
    }

    private static void AppendNumber(StringBuilder strBuilder, List<KeyValuePair<string, double>> parameters, double value, string formatString, NumberFormatInfo numberFormat) {
      if (parameters != null) {
        string paramKey = $"c_{parameters.Count}";
        strBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}", paramKey);
        parameters.Add(new KeyValuePair<string, double>(paramKey, value));
      } else {
        strBuilder.Append(value.ToString(formatString, numberFormat));
      }
    }

    private static void AppendVariableName(StringBuilder strBuilder, string name) {
      if (name.Contains("'"))
        strBuilder.AppendFormat("\"{0}\"", name);
      else
        strBuilder.AppendFormat("'{0}'", name);
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
    /// <param name="numberFormat">Number format that should be used for parameters (e.g. NumberFormatInfo.InvariantInfo (default)).</param>
    /// <param name="formatString">The format string for parameters (e.g. \"G4\" to limit to 4 digits, default is \"G\")</param>
    /// <returns>Infix expression</returns>
    public string Format(ISymbolicExpressionTree symbolicExpressionTree, NumberFormatInfo numberFormat,
                         string formatString = "G") {
      // skip root and start symbols
      StringBuilder strBuilder = new StringBuilder();
      var cleanTree = (ISymbolicExpressionTree)symbolicExpressionTree.Clone();
      BaseInfixExpressionFormatter.ConvertToBinaryLeftAssoc(cleanTree);
      BaseInfixExpressionFormatter.FormatRecursively(cleanTree.Root.GetSubtree(0).GetSubtree(0),
        strBuilder, numberFormat, formatString);
      return strBuilder.ToString();
    }

    public string Format(ISymbolicExpressionTree symbolicExpressionTree) {
      return Format(symbolicExpressionTree, NumberFormatInfo.InvariantInfo);
    }
  }

  [StorableType("54D917E8-134E-4066-9A60-2737C12D81DC")]
  [Item("Infix String Formatter", "Formatter for symbolic expressions, which produces an infix expression " +
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
      var parameters = new List<KeyValuePair<string, double>>();
      var cleanTree = (ISymbolicExpressionTree)symbolicExpressionTree.Clone();
      BaseInfixExpressionFormatter.ConvertToBinaryLeftAssoc(cleanTree);
      BaseInfixExpressionFormatter.FormatRecursively(cleanTree.Root.GetSubtree(0).GetSubtree(0),
        strBuilder, NumberFormatInfo.InvariantInfo, "G", parameters);
      strBuilder.Append($"{Environment.NewLine}{Environment.NewLine}");

      int maxDigits = GetDigits(parameters.Count);
      int padding = parameters.Max(x => x.Value.ToString("F12", CultureInfo.InvariantCulture).Length);
      foreach (var param in parameters) {
        int digits = GetDigits(int.Parse(param.Key.Substring(2)));
        strBuilder.Append($"{param.Key}{new string(' ', maxDigits - digits)} = " +
                          string.Format($"{{0,{padding}:F12}}", param.Value, CultureInfo.InvariantCulture) +
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
