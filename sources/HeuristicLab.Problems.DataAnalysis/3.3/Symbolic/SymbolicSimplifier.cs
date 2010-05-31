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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
using System.Diagnostics;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Simplistic simplifier for arithmetic expressions
  /// Rules:
  ///  * Constants are always the last argument to any function
  ///  * f(c1, c2) => c3 (constant expression folding)
  ///  * c1 / ( c2 * Var) => ( var * ( c2 / c1))
  /// </summary>
  public class SymbolicSimplifier {
    private Addition addSymbol = new Addition();
    private Multiplication mulSymbol = new Multiplication();
    private Division divSymbol = new Division();
    private Constant constSymbol = new Constant();
    private Variable varSymbol = new Variable();

    public SymbolicExpressionTree Simplify(SymbolicExpressionTree originalTree) {
      var clone = (SymbolicExpressionTreeNode)originalTree.Root.Clone();
      // macro expand (initially no argument trees)
      var macroExpandedTree = MacroExpand(clone, clone.SubTrees[0], new List<SymbolicExpressionTreeNode>());
      return new SymbolicExpressionTree(GetSimplifiedTree(macroExpandedTree));
    }

    // the argumentTrees list contains already expanded trees used as arguments for invocations
    private SymbolicExpressionTreeNode MacroExpand(SymbolicExpressionTreeNode root, SymbolicExpressionTreeNode node, IList<SymbolicExpressionTreeNode> argumentTrees) {
      List<SymbolicExpressionTreeNode> subtrees = new List<SymbolicExpressionTreeNode>(node.SubTrees);
      while (node.SubTrees.Count > 0) node.SubTrees.RemoveAt(0);
      if (node.Symbol is InvokeFunction) {
        var invokeSym = node.Symbol as InvokeFunction;
        var defunNode = FindFunctionDefinition(root, invokeSym.FunctionName);
        var macroExpandedArguments = new List<SymbolicExpressionTreeNode>();
        foreach (var subtree in subtrees) {
          macroExpandedArguments.Add(MacroExpand(root, subtree, argumentTrees));
        }
        return MacroExpand(root, defunNode, macroExpandedArguments);
      } else if (node.Symbol is Argument) {
        var argSym = node.Symbol as Argument;
        // return the correct argument sub-tree (already macro-expanded)
        return (SymbolicExpressionTreeNode)argumentTrees[argSym.ArgumentIndex].Clone();
      } else if (node.Symbol is StartSymbol) {
        return MacroExpand(root, subtrees[0], argumentTrees);
      } else {
        // recursive application
        foreach (var subtree in subtrees) {
          node.AddSubTree(MacroExpand(root, subtree, argumentTrees));
        }
        return node;
      }
    }

    private SymbolicExpressionTreeNode FindFunctionDefinition(SymbolicExpressionTreeNode root, string functionName) {
      foreach (var subtree in root.SubTrees.OfType<DefunTreeNode>()) {
        if (subtree.FunctionName == functionName) return subtree.SubTrees[0];
      }

      throw new ArgumentException("Definition of function " + functionName + " not found.");
    }

    /// <summary>
    /// Creates a new simplified tree
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public SymbolicExpressionTreeNode GetSimplifiedTree(SymbolicExpressionTreeNode original) {
      if (IsConstant(original) || IsVariable(original)) {
        return (SymbolicExpressionTreeNode)original.Clone();
      } else if (IsAddition(original)) {
        if (original.SubTrees.Count == 1) {
          return GetSimplifiedTree(original.SubTrees[0]);
        } else {
          // simplify expression x0..xn
          // make addition (x0..xn)
          Trace.Assert(original.SubTrees.Count > 1);
          return original.SubTrees
            .Select(x => GetSimplifiedTree(x))
            .Aggregate((a, b) => MakeAddition(a, b));
        }
      } else if (IsSubtraction(original)) {
        if (original.SubTrees.Count == 1) {
          return Negate(GetSimplifiedTree(original.SubTrees[0]));
        } else {
          // simplify expressions x0..xn
          // make addition (x0,-x1..-xn)
          Trace.Assert(original.SubTrees.Count > 1);
          var simplifiedTrees = original.SubTrees.Select(x => GetSimplifiedTree(x));
          return simplifiedTrees.Take(1)
            .Concat(simplifiedTrees.Skip(1).Select(x => Negate(x)))
            .Aggregate((a, b) => MakeAddition(a, b));
        }
      } else if (IsMultiplication(original)) {
        if (original.SubTrees.Count == 1) {
          return GetSimplifiedTree(original.SubTrees[0]);
        } else {
          Trace.Assert(original.SubTrees.Count > 1);
          return original.SubTrees
            .Select(x => GetSimplifiedTree(x))
            .Aggregate((a, b) => MakeMultiplication(a, b));
        }
      } else if (IsDivision(original)) {
        if (original.SubTrees.Count == 1) {
          return Invert(GetSimplifiedTree(original.SubTrees[0]));
        } else {
          // simplify expressions x0..xn
          // make multiplication (x0 * 1/(x1 * x1 * .. * xn))
          Trace.Assert(original.SubTrees.Count > 1);
          var simplifiedTrees = original.SubTrees.Select(x => GetSimplifiedTree(x));
          return
            MakeMultiplication(simplifiedTrees.First(), Invert(simplifiedTrees.Skip(1).Aggregate((a, b) => MakeMultiplication(a, b))));
        }
      } else if (IsAverage(original)) {
        if (original.SubTrees.Count == 1) {
          return GetSimplifiedTree(original.SubTrees[0]);
        } else {
          // simpliy expressions x0..xn
          // make sum(x0..xn) / n
          Trace.Assert(original.SubTrees.Count > 1);
          var sum = original.SubTrees
            .Select(x => GetSimplifiedTree(x))
            .Aggregate((a, b) => MakeAddition(a, b));
          return MakeDivision(sum, MakeConstant(original.SubTrees.Count));
        }
      } else {
        // can't simplify this function but simplify all subtrees 
        // TODO evaluate the function if original is a constant expression
        List<SymbolicExpressionTreeNode> subTrees = new List<SymbolicExpressionTreeNode>(original.SubTrees);
        while (original.SubTrees.Count > 0) original.RemoveSubTree(0);
        var clone = (SymbolicExpressionTreeNode)original.Clone();
        foreach (var subTree in subTrees) {
          clone.AddSubTree(GetSimplifiedTree(subTree));
          original.AddSubTree(subTree);
        }
        return clone;
      }
    }

    /// <summary>
    /// x => x * -1
    /// Doesn't create new trees and manipulates x
    /// </summary>
    /// <param name="x"></param>
    /// <returns>-x</returns>
    private SymbolicExpressionTreeNode Negate(SymbolicExpressionTreeNode x) {
      if (IsConstant(x)) {
        ((ConstantTreeNode)x).Value *= -1;
      } else if (IsVariable(x)) {
        var variableTree = (VariableTreeNode)x;
        variableTree.Weight *= -1.0;
      } else if (IsAddition(x)) {
        // (x0 + x1 + .. + xn) * -1 => (-x0 + -x1 + .. + -xn)        
        foreach (var subTree in x.SubTrees) {
          Negate(subTree);
        }
      } else if (IsMultiplication(x) || IsDivision(x)) {
        // x0 * x1 * .. * xn * -1 => x0 * x1 * .. * -xn
        Negate(x.SubTrees.Last()); // last is maybe a constant, prefer to negate the constant
      } else {
        // any other function
        return MakeMultiplication(x, MakeConstant(-1));
      }
      return x;
    }

    /// <summary>
    /// x => 1/x
    /// Doesn't create new trees and manipulates x
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private SymbolicExpressionTreeNode Invert(SymbolicExpressionTreeNode x) {
      if (IsConstant(x)) {
        ((ConstantTreeNode)x).Value = 1.0 / ((ConstantTreeNode)x).Value;
      } else {
        // any other function
        return MakeDivision(MakeConstant(1), x);
      }
      return x;
    }

    private SymbolicExpressionTreeNode MakeDivision(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        return MakeConstant(((ConstantTreeNode)a).Value / ((ConstantTreeNode)b).Value);
      } else if (IsVariable(a) && IsConstant(b)) {
        var constB = ((ConstantTreeNode)b).Value;
        ((VariableTreeNode)a).Weight /= constB;
        return a;
      } else {
        var div = divSymbol.CreateTreeNode();
        div.SubTrees.Add(a);
        div.SubTrees.Add(b);
        return div;
      }
    }

    private SymbolicExpressionTreeNode MakeAddition(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        // merge constants
        ((ConstantTreeNode)a).Value += ((ConstantTreeNode)b).Value;
        return a;
      } else if (IsConstant(a)) {
        // c + x => x + c
        // b is not constant => make sure constant is on the right
        return MakeAddition(b, a);
      } else if (IsConstant(b) && ((ConstantTreeNode)b).Value.IsAlmost(0.0)) {
        // x + 0 => x
        return a;
      } else if (IsAddition(a) && IsAddition(b)) {
        // merge additions
        var add = addSymbol.CreateTreeNode();
        for (int i = 0; i < a.SubTrees.Count - 1; i++) add.AddSubTree(a.SubTrees[i]);
        for (int i = 0; i < b.SubTrees.Count - 1; i++) add.AddSubTree(b.SubTrees[i]);
        if (IsConstant(a.SubTrees.Last()) && IsConstant(b.SubTrees.Last())) {
          add.AddSubTree(MakeAddition(a.SubTrees.Last(), b.SubTrees.Last()));
        } else if (IsConstant(a.SubTrees.Last())) {
          add.AddSubTree(b.SubTrees.Last());
          add.AddSubTree(a.SubTrees.Last());
        } else {
          add.AddSubTree(a.SubTrees.Last());
          add.AddSubTree(b.SubTrees.Last());
        }
        MergeVariables(add);
        return add;
      } else if (IsAddition(b)) {
        return MakeAddition(b, a);
      } else if (IsAddition(a) && IsConstant(b)) {
        var add = addSymbol.CreateTreeNode();
        for (int i = 0; i < a.SubTrees.Count - 1; i++) add.AddSubTree(a.SubTrees[i]);
        if (IsConstant(a.SubTrees.Last()))
          add.AddSubTree(MakeAddition(a.SubTrees.Last(), b));
        else {
          add.AddSubTree(a.SubTrees.Last());
          add.AddSubTree(b);
        }
        return add;
      } else if (IsAddition(a)) {
        var add = addSymbol.CreateTreeNode();
        add.AddSubTree(b);
        foreach (var subTree in a.SubTrees) {
          add.AddSubTree(subTree);
        }
        MergeVariables(add);
        return add;
      } else {
        var add = addSymbol.CreateTreeNode();
        add.SubTrees.Add(a);
        add.SubTrees.Add(b);
        MergeVariables(add);
        return add;
      }
    }

    private void MergeVariables(SymbolicExpressionTreeNode add) {
      var subtrees = new List<SymbolicExpressionTreeNode>(add.SubTrees);
      while (add.SubTrees.Count > 0) add.RemoveSubTree(0);
      var groupedVarNodes = from node in subtrees.OfType<VariableTreeNode>()
                            group node by node.VariableName into g
                            select g;
      var unchangedSubTrees = subtrees.Where(t => !(t is VariableTreeNode));

      foreach (var variableNodeGroup in groupedVarNodes) {
        var weightSum = variableNodeGroup.Select(t => t.Weight).Sum();
        var representative = variableNodeGroup.First();
        representative.Weight = weightSum;
        add.AddSubTree(representative);
      }
      foreach (var unchangedSubtree in unchangedSubTrees)
        add.AddSubTree(unchangedSubtree);
    }

    private SymbolicExpressionTreeNode MakeMultiplication(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        ((ConstantTreeNode)a).Value *= ((ConstantTreeNode)b).Value;
        return a;
      } else if (IsConstant(a)) {
        return MakeMultiplication(b, a);
      } else if (IsConstant(b) && ((ConstantTreeNode)b).Value.IsAlmost(1.0)) {
        return a;
      } else if (IsConstant(b) && IsVariable(a)) {
        ((VariableTreeNode)a).Weight *= ((ConstantTreeNode)b).Value;
        return a;
      } else if (IsConstant(b) && IsAddition(a)) {
        return a.SubTrees.Select(x => MakeMultiplication(x, b)).Aggregate((c, d) => MakeAddition(c, d));
      } else if(IsDivision(a) && IsDivision(b)) {
        return MakeDivision(MakeMultiplication(a.SubTrees[0], b.SubTrees[0]), MakeMultiplication(a.SubTrees[1], b.SubTrees[1]));
      } else if (IsDivision(a)) {
        Trace.Assert(a.SubTrees.Count == 2);
        return MakeDivision(MakeMultiplication(a.SubTrees[0], b), a.SubTrees[1]);
      } else if (IsDivision(b)) {
        Trace.Assert(b.SubTrees.Count == 2);
        return MakeDivision(MakeMultiplication(b.SubTrees[0], a), b.SubTrees[1]);
      } else if (IsMultiplication(a) && IsMultiplication(b)) {
        var mul = mulSymbol.CreateTreeNode();
        for (int i = 0; i < a.SubTrees.Count - 1; i++) mul.AddSubTree(a.SubTrees[i]);
        for (int i = 0; i < b.SubTrees.Count - 1; i++) mul.AddSubTree(b.SubTrees[i]);
        mul.AddSubTree(MakeMultiplication(a.SubTrees.Last(), b.SubTrees.Last()));
        return mul;
      } else if (IsMultiplication(a)) {
        var mul = mulSymbol.CreateTreeNode();
        for (int i = 0; i < a.SubTrees.Count - 1; i++) mul.AddSubTree(a.SubTrees[i]);
        mul.AddSubTree(MakeMultiplication(a.SubTrees.Last(), b));
        return mul;
      } else if (IsMultiplication(b)) {
        var mul = mulSymbol.CreateTreeNode();
        for (int i = 0; i < b.SubTrees.Count - 1; i++) mul.AddSubTree(b.SubTrees[i]);
        mul.AddSubTree(MakeMultiplication(b.SubTrees.Last(), a));
        return mul;
      } else {
        var mul = mulSymbol.CreateTreeNode();
        mul.SubTrees.Add(a);
        mul.SubTrees.Add(b);
        return mul;
      }
    }

    #region is symbol ?
    private bool IsDivision(SymbolicExpressionTreeNode original) {
      return original.Symbol is Division;
    }

    private bool IsMultiplication(SymbolicExpressionTreeNode original) {
      return original.Symbol is Multiplication;
    }

    private bool IsSubtraction(SymbolicExpressionTreeNode original) {
      return original.Symbol is Subtraction;
    }

    private bool IsAddition(SymbolicExpressionTreeNode original) {
      return original.Symbol is Addition;
    }

    private bool IsVariable(SymbolicExpressionTreeNode original) {
      return original.Symbol is Variable;
    }

    private bool IsConstant(SymbolicExpressionTreeNode original) {
      return original.Symbol is Constant;
    }

    private bool IsAverage(SymbolicExpressionTreeNode original) {
      return original.Symbol is Average;
    }
    #endregion

    private SymbolicExpressionTreeNode MakeConstant(double value) {
      ConstantTreeNode constantTreeNode = (ConstantTreeNode)(constSymbol.CreateTreeNode());
      constantTreeNode.Value = value;
      return (SymbolicExpressionTreeNode)constantTreeNode;
    }

    private SymbolicExpressionTreeNode MakeVariable(double weight, string name) {
      var tree = (VariableTreeNode)varSymbol.CreateTreeNode();
      tree.Weight = weight;
      tree.VariableName = name;
      return tree;
    }
  }
}
