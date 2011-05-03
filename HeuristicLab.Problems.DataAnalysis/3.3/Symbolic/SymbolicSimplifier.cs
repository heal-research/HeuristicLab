#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Simplistic simplifier for arithmetic expressions
  /// </summary>
  public class SymbolicSimplifier {
    private Addition addSymbol = new Addition();
    private Subtraction subSymbol = new Subtraction();
    private Multiplication mulSymbol = new Multiplication();
    private Division divSymbol = new Division();
    private Constant constSymbol = new Constant();
    private Variable varSymbol = new Variable();
    private Logarithm logSymbol = new Logarithm();
    private Exponential expSymbol = new Exponential();
    private Root rootSymbol = new Root();
    private Power powSymbol = new Power();
    private Sine sineSymbol = new Sine();
    private Cosine cosineSymbol = new Cosine();
    private Tangent tanSymbol = new Tangent();
    private IfThenElse ifThenElseSymbol = new IfThenElse();
    private And andSymbol = new And();
    private Or orSymbol = new Or();
    private Not notSymbol = new Not();
    private GreaterThan gtSymbol = new GreaterThan();
    private LessThan ltSymbol = new LessThan();

    public SymbolicExpressionTree Simplify(SymbolicExpressionTree originalTree) {
      var clone = (SymbolicExpressionTreeNode)originalTree.Root.Clone();
      // macro expand (initially no argument trees)
      var macroExpandedTree = MacroExpand(clone, clone.SubTrees[0], new List<SymbolicExpressionTreeNode>());
      SymbolicExpressionTreeNode rootNode = (new ProgramRootSymbol()).CreateTreeNode();
      rootNode.AddSubTree(GetSimplifiedTree(macroExpandedTree));
      return new SymbolicExpressionTree(rootNode);
    }

    // the argumentTrees list contains already expanded trees used as arguments for invocations
    private SymbolicExpressionTreeNode MacroExpand(SymbolicExpressionTreeNode root, SymbolicExpressionTreeNode node, IList<SymbolicExpressionTreeNode> argumentTrees) {
      List<SymbolicExpressionTreeNode> subtrees = new List<SymbolicExpressionTreeNode>(node.SubTrees);
      while (node.SubTrees.Count > 0) node.RemoveSubTree(0);
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


    #region symbol predicates
    // arithmetic
    private bool IsDivision(SymbolicExpressionTreeNode node) {
      return node.Symbol is Division;
    }

    private bool IsMultiplication(SymbolicExpressionTreeNode node) {
      return node.Symbol is Multiplication;
    }

    private bool IsSubtraction(SymbolicExpressionTreeNode node) {
      return node.Symbol is Subtraction;
    }

    private bool IsAddition(SymbolicExpressionTreeNode node) {
      return node.Symbol is Addition;
    }

    private bool IsAverage(SymbolicExpressionTreeNode node) {
      return node.Symbol is Average;
    }
    // exponential
    private bool IsLog(SymbolicExpressionTreeNode node) {
      return node.Symbol is Logarithm;
    }
    private bool IsExp(SymbolicExpressionTreeNode node) {
      return node.Symbol is Exponential;
    }
    private bool IsRoot(SymbolicExpressionTreeNode node) {
      return node.Symbol is Root;
    }
    private bool IsPower(SymbolicExpressionTreeNode node) {
      return node.Symbol is Power;
    }
    // trigonometric
    private bool IsSine(SymbolicExpressionTreeNode node) {
      return node.Symbol is Sine;
    }
    private bool IsCosine(SymbolicExpressionTreeNode node) {
      return node.Symbol is Cosine;
    }
    private bool IsTangent(SymbolicExpressionTreeNode node) {
      return node.Symbol is Tangent;
    }
    // boolean
    private bool IsIfThenElse(SymbolicExpressionTreeNode node) {
      return node.Symbol is IfThenElse;
    }
    private bool IsAnd(SymbolicExpressionTreeNode node) {
      return node.Symbol is And;
    }
    private bool IsOr(SymbolicExpressionTreeNode node) {
      return node.Symbol is Or;
    }
    private bool IsNot(SymbolicExpressionTreeNode node) {
      return node.Symbol is Not;
    }
    // comparison
    private bool IsGreaterThan(SymbolicExpressionTreeNode node) {
      return node.Symbol is GreaterThan;
    }
    private bool IsLessThan(SymbolicExpressionTreeNode node) {
      return node.Symbol is LessThan;
    }

    private bool IsBoolean(SymbolicExpressionTreeNode node) {
      return
        node.Symbol is GreaterThan ||
        node.Symbol is LessThan ||
        node.Symbol is And ||
        node.Symbol is Or;
    }

    // terminals
    private bool IsVariable(SymbolicExpressionTreeNode node) {
      return node.Symbol is Variable;
    }

    private bool IsConstant(SymbolicExpressionTreeNode node) {
      return node.Symbol is Constant;
    }

    #endregion

    /// <summary>
    /// Creates a new simplified tree
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public SymbolicExpressionTreeNode GetSimplifiedTree(SymbolicExpressionTreeNode original) {
      if (IsConstant(original) || IsVariable(original)) {
        return (SymbolicExpressionTreeNode)original.Clone();
      } else if (IsAddition(original)) {
        return SimplifyAddition(original);
      } else if (IsSubtraction(original)) {
        return SimplifySubtraction(original);
      } else if (IsMultiplication(original)) {
        return SimplifyMultiplication(original);
      } else if (IsDivision(original)) {
        return SimplifyDivision(original);
      } else if (IsAverage(original)) {
        return SimplifyAverage(original);
      } else if (IsLog(original)) {
        return SimplifyLog(original);
      } else if (IsExp(original)) {
        return SimplifyExp(original);
      } else if (IsRoot(original)) {
        return SimplifyRoot(original);
      } else if (IsPower(original)) {
        return SimplifyPower(original);
      } else if (IsSine(original)) {
        return SimplifySine(original);
      } else if (IsCosine(original)) {
        return SimplifyCosine(original);
      } else if (IsTangent(original)) {
        return SimplifyTangent(original);
      } else if (IsIfThenElse(original)) {
        return SimplifyIfThenElse(original);
      } else if (IsGreaterThan(original)) {
        return SimplifyGreaterThan(original);
      } else if (IsLessThan(original)) {
        return SimplifyLessThan(original);
      } else if (IsAnd(original)) {
        return SimplifyAnd(original);
      } else if (IsOr(original)) {
        return SimplifyOr(original);
      } else if (IsNot(original)) {
        return SimplifyNot(original);
      } else {
        return SimplifyAny(original);
      }
    }


    #region specific simplification routines
    private SymbolicExpressionTreeNode SimplifyAny(SymbolicExpressionTreeNode original) {
      // can't simplify this function but simplify all subtrees 
      List<SymbolicExpressionTreeNode> subTrees = new List<SymbolicExpressionTreeNode>(original.SubTrees);
      while (original.SubTrees.Count > 0) original.RemoveSubTree(0);
      var clone = (SymbolicExpressionTreeNode)original.Clone();
      List<SymbolicExpressionTreeNode> simplifiedSubTrees = new List<SymbolicExpressionTreeNode>();
      foreach (var subTree in subTrees) {
        simplifiedSubTrees.Add(GetSimplifiedTree(subTree));
        original.AddSubTree(subTree);
      }
      foreach (var simplifiedSubtree in simplifiedSubTrees) {
        clone.AddSubTree(simplifiedSubtree);
      }
      if (simplifiedSubTrees.TrueForAll(t => IsConstant(t))) {
        SimplifyConstantExpression(clone);
      }
      return clone;
    }

    private SymbolicExpressionTreeNode SimplifyConstantExpression(SymbolicExpressionTreeNode original) {
      // not yet implemented
      return original;
    }

    private SymbolicExpressionTreeNode SimplifyAverage(SymbolicExpressionTreeNode original) {
      if (original.SubTrees.Count == 1) {
        return GetSimplifiedTree(original.SubTrees[0]);
      } else {
        // simplify expressions x0..xn
        // make sum(x0..xn) / n
        Trace.Assert(original.SubTrees.Count > 1);
        var sum = original.SubTrees
          .Select(x => GetSimplifiedTree(x))
          .Aggregate((a, b) => MakeSum(a, b));
        return MakeFraction(sum, MakeConstant(original.SubTrees.Count));
      }
    }

    private SymbolicExpressionTreeNode SimplifyDivision(SymbolicExpressionTreeNode original) {
      if (original.SubTrees.Count == 1) {
        return Invert(GetSimplifiedTree(original.SubTrees[0]));
      } else {
        // simplify expressions x0..xn
        // make multiplication (x0 * 1/(x1 * x1 * .. * xn))
        Trace.Assert(original.SubTrees.Count > 1);
        var simplifiedTrees = original.SubTrees.Select(x => GetSimplifiedTree(x));
        return
          MakeProduct(simplifiedTrees.First(), Invert(simplifiedTrees.Skip(1).Aggregate((a, b) => MakeProduct(a, b))));
      }
    }

    private SymbolicExpressionTreeNode SimplifyMultiplication(SymbolicExpressionTreeNode original) {
      if (original.SubTrees.Count == 1) {
        return GetSimplifiedTree(original.SubTrees[0]);
      } else {
        Trace.Assert(original.SubTrees.Count > 1);
        return original.SubTrees
          .Select(x => GetSimplifiedTree(x))
          .Aggregate((a, b) => MakeProduct(a, b));
      }
    }

    private SymbolicExpressionTreeNode SimplifySubtraction(SymbolicExpressionTreeNode original) {
      if (original.SubTrees.Count == 1) {
        return Negate(GetSimplifiedTree(original.SubTrees[0]));
      } else {
        // simplify expressions x0..xn
        // make addition (x0,-x1..-xn)
        Trace.Assert(original.SubTrees.Count > 1);
        var simplifiedTrees = original.SubTrees.Select(x => GetSimplifiedTree(x));
        return simplifiedTrees.Take(1)
          .Concat(simplifiedTrees.Skip(1).Select(x => Negate(x)))
          .Aggregate((a, b) => MakeSum(a, b));
      }
    }

    private SymbolicExpressionTreeNode SimplifyAddition(SymbolicExpressionTreeNode original) {
      if (original.SubTrees.Count == 1) {
        return GetSimplifiedTree(original.SubTrees[0]);
      } else {
        // simplify expression x0..xn
        // make addition (x0..xn)
        Trace.Assert(original.SubTrees.Count > 1);
        return original.SubTrees
          .Select(x => GetSimplifiedTree(x))
          .Aggregate((a, b) => MakeSum(a, b));
      }
    }

    private SymbolicExpressionTreeNode SimplifyNot(SymbolicExpressionTreeNode original) {
      return MakeNot(GetSimplifiedTree(original.SubTrees[0]));
    }
    private SymbolicExpressionTreeNode SimplifyOr(SymbolicExpressionTreeNode original) {
      return original.SubTrees
        .Select(x => GetSimplifiedTree(x))
        .Aggregate((a, b) => MakeOr(a, b));
    }
    private SymbolicExpressionTreeNode SimplifyAnd(SymbolicExpressionTreeNode original) {
      return original.SubTrees
        .Select(x => GetSimplifiedTree(x))
        .Aggregate((a, b) => MakeAnd(a, b));
    }
    private SymbolicExpressionTreeNode SimplifyLessThan(SymbolicExpressionTreeNode original) {
      return MakeLessThan(GetSimplifiedTree(original.SubTrees[0]), GetSimplifiedTree(original.SubTrees[1]));
    }
    private SymbolicExpressionTreeNode SimplifyGreaterThan(SymbolicExpressionTreeNode original) {
      return MakeGreaterThan(GetSimplifiedTree(original.SubTrees[0]), GetSimplifiedTree(original.SubTrees[1]));
    }
    private SymbolicExpressionTreeNode SimplifyIfThenElse(SymbolicExpressionTreeNode original) {
      return MakeIfThenElse(GetSimplifiedTree(original.SubTrees[0]), GetSimplifiedTree(original.SubTrees[1]), GetSimplifiedTree(original.SubTrees[2]));
    }
    private SymbolicExpressionTreeNode SimplifyTangent(SymbolicExpressionTreeNode original) {
      return MakeTangent(GetSimplifiedTree(original.SubTrees[0]));
    }
    private SymbolicExpressionTreeNode SimplifyCosine(SymbolicExpressionTreeNode original) {
      return MakeCosine(GetSimplifiedTree(original.SubTrees[0]));
    }
    private SymbolicExpressionTreeNode SimplifySine(SymbolicExpressionTreeNode original) {
      return MakeSine(GetSimplifiedTree(original.SubTrees[0]));
    }
    private SymbolicExpressionTreeNode SimplifyExp(SymbolicExpressionTreeNode original) {
      return MakeExp(GetSimplifiedTree(original.SubTrees[0]));
    }

    private SymbolicExpressionTreeNode SimplifyLog(SymbolicExpressionTreeNode original) {
      return MakeLog(GetSimplifiedTree(original.SubTrees[0]));
    }
    private SymbolicExpressionTreeNode SimplifyRoot(SymbolicExpressionTreeNode original) {
      return MakeRoot(GetSimplifiedTree(original.SubTrees[0]), GetSimplifiedTree(original.SubTrees[1]));
    }

    private SymbolicExpressionTreeNode SimplifyPower(SymbolicExpressionTreeNode original) {
      return MakePower(GetSimplifiedTree(original.SubTrees[0]), GetSimplifiedTree(original.SubTrees[1]));
    }
    #endregion

    #region low level tree restructuring
    private SymbolicExpressionTreeNode MakeNot(SymbolicExpressionTreeNode t) {
      return MakeProduct(t, MakeConstant(-1.0));
    }

    private SymbolicExpressionTreeNode MakeOr(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        var constA = a as ConstantTreeNode;
        var constB = b as ConstantTreeNode;
        if (constA.Value > 0.0 || constB.Value > 0.0) {
          return MakeConstant(1.0);
        } else {
          return MakeConstant(-1.0);
        }
      } else if (IsConstant(a)) {
        return MakeOr(b, a);
      } else if (IsConstant(b)) {
        var constT = b as ConstantTreeNode;
        if (constT.Value > 0.0) {
          // boolean expression is necessarily true
          return MakeConstant(1.0);
        } else {
          // the constant value has no effect on the result of the boolean condition so we can drop the constant term
          var orNode = orSymbol.CreateTreeNode();
          orNode.AddSubTree(a);
          return orNode;
        }
      } else {
        var orNode = orSymbol.CreateTreeNode();
        orNode.AddSubTree(a);
        orNode.AddSubTree(b);
        return orNode;
      }
    }
    private SymbolicExpressionTreeNode MakeAnd(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        var constA = a as ConstantTreeNode;
        var constB = b as ConstantTreeNode;
        if (constA.Value > 0.0 && constB.Value > 0.0) {
          return MakeConstant(1.0);
        } else {
          return MakeConstant(-1.0);
        }
      } else if (IsConstant(a)) {
        return MakeAnd(b, a);
      } else if (IsConstant(b)) {
        var constB = b as ConstantTreeNode;
        if (constB.Value > 0.0) {
          // the constant value has no effect on the result of the boolean condition so we can drop the constant term
          var andNode = andSymbol.CreateTreeNode();
          andNode.AddSubTree(a);
          return andNode;
        } else {
          // boolean expression is necessarily false
          return MakeConstant(-1.0);
        }
      } else {
        var andNode = andSymbol.CreateTreeNode();
        andNode.AddSubTree(a);
        andNode.AddSubTree(b);
        return andNode;
      }
    }
    private SymbolicExpressionTreeNode MakeLessThan(SymbolicExpressionTreeNode leftSide, SymbolicExpressionTreeNode rightSide) {
      if (IsConstant(leftSide) && IsConstant(rightSide)) {
        var lsConst = leftSide as ConstantTreeNode;
        var rsConst = rightSide as ConstantTreeNode;
        if (lsConst.Value < rsConst.Value) return MakeConstant(1.0);
        else return MakeConstant(-1.0);
      } else {
        var ltNode = ltSymbol.CreateTreeNode();
        ltNode.AddSubTree(leftSide);
        ltNode.AddSubTree(rightSide);
        return ltNode;
      }
    }
    private SymbolicExpressionTreeNode MakeGreaterThan(SymbolicExpressionTreeNode leftSide, SymbolicExpressionTreeNode rightSide) {
      if (IsConstant(leftSide) && IsConstant(rightSide)) {
        var lsConst = leftSide as ConstantTreeNode;
        var rsConst = rightSide as ConstantTreeNode;
        if (lsConst.Value > rsConst.Value) return MakeConstant(1.0);
        else return MakeConstant(-1.0);
      } else {
        var gtNode = gtSymbol.CreateTreeNode();
        gtNode.AddSubTree(leftSide);
        gtNode.AddSubTree(rightSide);
        return gtNode;
      }
    }
    private SymbolicExpressionTreeNode MakeIfThenElse(SymbolicExpressionTreeNode condition, SymbolicExpressionTreeNode trueBranch, SymbolicExpressionTreeNode falseBranch) {
      if (IsConstant(condition)) {
        var constT = condition as ConstantTreeNode;
        if (constT.Value > 0.0) return trueBranch;
        else return falseBranch;
      } else {
        var ifNode = ifThenElseSymbol.CreateTreeNode();
        if (IsBoolean(condition)) {
          ifNode.AddSubTree(condition);
        } else {
          var gtNode = gtSymbol.CreateTreeNode();
          gtNode.AddSubTree(condition); gtNode.AddSubTree(MakeConstant(0.0));
          ifNode.AddSubTree(gtNode);
        }
        ifNode.AddSubTree(trueBranch);
        ifNode.AddSubTree(falseBranch);
        return ifNode;
      }
    }

    private SymbolicExpressionTreeNode MakeSine(SymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Sin(constT.Value));
      } else {
        var sineNode = sineSymbol.CreateTreeNode();
        sineNode.AddSubTree(node);
        return sineNode;
      }
    }
    private SymbolicExpressionTreeNode MakeTangent(SymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Tan(constT.Value));
      } else {
        var tanNode = tanSymbol.CreateTreeNode();
        tanNode.AddSubTree(node);
        return tanNode;
      }
    }
    private SymbolicExpressionTreeNode MakeCosine(SymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Cos(constT.Value));
      } else {
        var cosNode = cosineSymbol.CreateTreeNode();
        cosNode.AddSubTree(node);
        return cosNode;
      }
    }
    private SymbolicExpressionTreeNode MakeExp(SymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Exp(constT.Value));
      } else if (IsLog(node)) {
        return node.SubTrees[0];
      } else if (IsAddition(node)) {
        return node.SubTrees.Select(s => MakeExp(s)).Aggregate((s, t) => MakeProduct(s, t));
      } else if (IsSubtraction(node)) {
        return node.SubTrees.Select(s => MakeExp(s)).Aggregate((s, t) => MakeProduct(s, Negate(t)));
      } else {
        var expNode = expSymbol.CreateTreeNode();
        expNode.AddSubTree(node);
        return expNode;
      }
    }
    private SymbolicExpressionTreeNode MakeLog(SymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Log(constT.Value));
      } else if (IsExp(node)) {
        return node.SubTrees[0];
      } else if (IsMultiplication(node)) {
        return node.SubTrees.Select(s => MakeLog(s)).Aggregate((x, y) => MakeSum(x, y));
      } else if (IsDivision(node)) {
        var subtractionNode = subSymbol.CreateTreeNode();
        foreach (var subTree in node.SubTrees) {
          subtractionNode.AddSubTree(MakeLog(subTree));
        }
        return subtractionNode;
      } else {
        var logNode = logSymbol.CreateTreeNode();
        logNode.AddSubTree(node);
        return logNode;
      }
    }
    private SymbolicExpressionTreeNode MakeRoot(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        var constA = a as ConstantTreeNode;
        var constB = b as ConstantTreeNode;
        return MakeConstant(Math.Pow(constA.Value, 1.0 / Math.Round(constB.Value)));
      } else if (IsConstant(b)) {
        var constB = b as ConstantTreeNode;
        var constBValue = Math.Round(constB.Value);
        if (constBValue.IsAlmost(1.0)) {
          return a;
        } else if (constBValue.IsAlmost(0.0)) {
          return MakeConstant(1.0);
        } else if (constBValue.IsAlmost(-1.0)) {
          return MakeFraction(MakeConstant(1.0), a);
        } else if (constBValue < 0) {
          var rootNode = rootSymbol.CreateTreeNode();
          rootNode.AddSubTree(a);
          rootNode.AddSubTree(MakeConstant(-1.0 * constBValue));
          return MakeFraction(MakeConstant(1.0), rootNode);
        } else {
          var rootNode = rootSymbol.CreateTreeNode();
          rootNode.AddSubTree(a);
          rootNode.AddSubTree(MakeConstant(constBValue));
          return rootNode;
        }
      } else {
        var rootNode = rootSymbol.CreateTreeNode();
        rootNode.AddSubTree(a);
        rootNode.AddSubTree(b);
        return rootNode;
      }
    }
    private SymbolicExpressionTreeNode MakePower(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        var constA = a as ConstantTreeNode;
        var constB = b as ConstantTreeNode;
        return MakeConstant(Math.Pow(constA.Value, Math.Round(constB.Value)));
      } else if (IsConstant(b)) {
        var constB = b as ConstantTreeNode;
        double exponent = Math.Round(constB.Value);
        if (exponent.IsAlmost(0.0)) {
          return MakeConstant(1.0);
        } else if (exponent.IsAlmost(1.0)) {
          return a;
        } else if (exponent.IsAlmost(-1.0)) {
          return MakeFraction(MakeConstant(1.0), a);
        } else if (exponent < 0) {
          var powNode = powSymbol.CreateTreeNode();
          powNode.AddSubTree(a);
          powNode.AddSubTree(MakeConstant(-1.0 * exponent));
          return MakeFraction(MakeConstant(1.0), powNode);
        } else {
          var powNode = powSymbol.CreateTreeNode();
          powNode.AddSubTree(a);
          powNode.AddSubTree(MakeConstant(exponent));
          return powNode;
        }
      } else {
        var powNode = powSymbol.CreateTreeNode();
        powNode.AddSubTree(a);
        powNode.AddSubTree(b);
        return powNode;
      }
    }


    // MakeFraction, MakeProduct and MakeSum take two already simplified trees and create a new simplified tree
    private SymbolicExpressionTreeNode MakeFraction(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        // fold constants
        return MakeConstant(((ConstantTreeNode)a).Value / ((ConstantTreeNode)b).Value);
      } if (IsConstant(a) && !((ConstantTreeNode)a).Value.IsAlmost(1.0)) {
        return MakeFraction(MakeConstant(1.0), MakeProduct(b, Invert(a)));
      } else if (IsVariable(a) && IsConstant(b)) {
        // merge constant values into variable weights
        var constB = ((ConstantTreeNode)b).Value;
        ((VariableTreeNode)a).Weight /= constB;
        return a;
      } else if (IsVariable(a) && IsVariable(b) && AreSameVariable(a, b)) {
        // cancel variables
        var aVar = a as VariableTreeNode;
        var bVar = b as VariableTreeNode;
        return MakeConstant(aVar.Weight / bVar.Weight);
      } else if (IsAddition(a) && IsConstant(b)) {
        return a.SubTrees
          .Select(x => GetSimplifiedTree(x))
         .Select(x => MakeFraction(x, b))
         .Aggregate((c, d) => MakeSum(c, d));
      } else if (IsMultiplication(a) && IsConstant(b)) {
        return MakeProduct(a, Invert(b));
      } else if (IsDivision(a) && IsConstant(b)) {
        // (a1 / a2) / c => (a1 / (a2 * c))
        Trace.Assert(a.SubTrees.Count == 2);
        return MakeFraction(a.SubTrees[0], MakeProduct(a.SubTrees[1], b));
      } else if (IsDivision(a) && IsDivision(b)) {
        // (a1 / a2) / (b1 / b2) => 
        Trace.Assert(a.SubTrees.Count == 2);
        Trace.Assert(b.SubTrees.Count == 2);
        return MakeFraction(MakeProduct(a.SubTrees[0], b.SubTrees[1]), MakeProduct(a.SubTrees[1], b.SubTrees[0]));
      } else if (IsDivision(a)) {
        // (a1 / a2) / b => (a1 / (a2 * b))
        Trace.Assert(a.SubTrees.Count == 2);
        return MakeFraction(a.SubTrees[0], MakeProduct(a.SubTrees[1], b));
      } else if (IsDivision(b)) {
        // a / (b1 / b2) => (a * b2) / b1
        Trace.Assert(b.SubTrees.Count == 2);
        return MakeFraction(MakeProduct(a, b.SubTrees[1]), b.SubTrees[0]);
      } else {
        var div = divSymbol.CreateTreeNode();
        div.AddSubTree(a);
        div.AddSubTree(b);
        return div;
      }
    }

    private SymbolicExpressionTreeNode MakeSum(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        // fold constants
        ((ConstantTreeNode)a).Value += ((ConstantTreeNode)b).Value;
        return a;
      } else if (IsConstant(a)) {
        // c + x => x + c
        // b is not constant => make sure constant is on the right
        return MakeSum(b, a);
      } else if (IsConstant(b) && ((ConstantTreeNode)b).Value.IsAlmost(0.0)) {
        // x + 0 => x
        return a;
      } else if (IsAddition(a) && IsAddition(b)) {
        // merge additions
        var add = addSymbol.CreateTreeNode();
        for (int i = 0; i < a.SubTrees.Count - 1; i++) add.AddSubTree(a.SubTrees[i]);
        for (int i = 0; i < b.SubTrees.Count - 1; i++) add.AddSubTree(b.SubTrees[i]);
        if (IsConstant(a.SubTrees.Last()) && IsConstant(b.SubTrees.Last())) {
          add.AddSubTree(MakeSum(a.SubTrees.Last(), b.SubTrees.Last()));
        } else if (IsConstant(a.SubTrees.Last())) {
          add.AddSubTree(b.SubTrees.Last());
          add.AddSubTree(a.SubTrees.Last());
        } else {
          add.AddSubTree(a.SubTrees.Last());
          add.AddSubTree(b.SubTrees.Last());
        }
        MergeVariablesInSum(add);
        if (add.SubTrees.Count == 1) {
          return add.SubTrees[0];
        } else {
          return add;
        }
      } else if (IsAddition(b)) {
        return MakeSum(b, a);
      } else if (IsAddition(a) && IsConstant(b)) {
        // a is an addition and b is a constant => append b to a and make sure the constants are merged
        var add = addSymbol.CreateTreeNode();
        for (int i = 0; i < a.SubTrees.Count - 1; i++) add.AddSubTree(a.SubTrees[i]);
        if (IsConstant(a.SubTrees.Last()))
          add.AddSubTree(MakeSum(a.SubTrees.Last(), b));
        else {
          add.AddSubTree(a.SubTrees.Last());
          add.AddSubTree(b);
        }
        return add;
      } else if (IsAddition(a)) {
        // a is already an addition => append b
        var add = addSymbol.CreateTreeNode();
        add.AddSubTree(b);
        foreach (var subTree in a.SubTrees) {
          add.AddSubTree(subTree);
        }
        MergeVariablesInSum(add);
        if (add.SubTrees.Count == 1) {
          return add.SubTrees[0];
        } else {
          return add;
        }
      } else {
        var add = addSymbol.CreateTreeNode();
        add.AddSubTree(a);
        add.AddSubTree(b);
        MergeVariablesInSum(add);
        if (add.SubTrees.Count == 1) {
          return add.SubTrees[0];
        } else {
          return add;
        }
      }
    }

    // makes sure variable symbols in sums are combined
    // possible improvement: combine sums of products where the products only reference the same variable
    private void MergeVariablesInSum(SymbolicExpressionTreeNode sum) {
      var subtrees = new List<SymbolicExpressionTreeNode>(sum.SubTrees);
      while (sum.SubTrees.Count > 0) sum.RemoveSubTree(0);
      var groupedVarNodes = from node in subtrees.OfType<VariableTreeNode>()
                            let lag = (node is LaggedVariableTreeNode) ? ((LaggedVariableTreeNode)node).Lag : 0
                            group node by node.VariableName + lag into g
                            select g;
      var unchangedSubTrees = subtrees.Where(t => !(t is VariableTreeNode));

      foreach (var variableNodeGroup in groupedVarNodes) {
        var weightSum = variableNodeGroup.Select(t => t.Weight).Sum();
        var representative = variableNodeGroup.First();
        representative.Weight = weightSum;
        sum.AddSubTree(representative);
      }
      foreach (var unchangedSubtree in unchangedSubTrees)
        sum.AddSubTree(unchangedSubtree);
    }


    private SymbolicExpressionTreeNode MakeProduct(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        // fold constants
        ((ConstantTreeNode)a).Value *= ((ConstantTreeNode)b).Value;
        return a;
      } else if (IsConstant(a)) {
        // a * $ => $ * a
        return MakeProduct(b, a);
      } else if (IsConstant(b) && ((ConstantTreeNode)b).Value.IsAlmost(1.0)) {
        // $ * 1.0 => $
        return a;
      } else if (IsConstant(b) && IsVariable(a)) {
        // multiply constants into variables weights
        ((VariableTreeNode)a).Weight *= ((ConstantTreeNode)b).Value;
        return a;
      } else if (IsConstant(b) && IsAddition(a)) {
        // multiply constants into additions
        return a.SubTrees.Select(x => MakeProduct(x, b)).Aggregate((c, d) => MakeSum(c, d));
      } else if (IsDivision(a) && IsDivision(b)) {
        // (a1 / a2) * (b1 / b2) => (a1 * b1) / (a2 * b2)
        Trace.Assert(a.SubTrees.Count == 2);
        Trace.Assert(b.SubTrees.Count == 2);
        return MakeFraction(MakeProduct(a.SubTrees[0], b.SubTrees[0]), MakeProduct(a.SubTrees[1], b.SubTrees[1]));
      } else if (IsDivision(a)) {
        // (a1 / a2) * b => (a1 * b) / a2
        Trace.Assert(a.SubTrees.Count == 2);
        return MakeFraction(MakeProduct(a.SubTrees[0], b), a.SubTrees[1]);
      } else if (IsDivision(b)) {
        // a * (b1 / b2) => (b1 * a) / b2
        Trace.Assert(b.SubTrees.Count == 2);
        return MakeFraction(MakeProduct(b.SubTrees[0], a), b.SubTrees[1]);
      } else if (IsMultiplication(a) && IsMultiplication(b)) {
        // merge multiplications (make sure constants are merged)
        var mul = mulSymbol.CreateTreeNode();
        for (int i = 0; i < a.SubTrees.Count; i++) mul.AddSubTree(a.SubTrees[i]);
        for (int i = 0; i < b.SubTrees.Count; i++) mul.AddSubTree(b.SubTrees[i]);
        MergeVariablesAndConstantsInProduct(mul);
        return mul;
      } else if (IsMultiplication(b)) {
        return MakeProduct(b, a);
      } else if (IsMultiplication(a)) {
        // a is already an multiplication => append b
        a.AddSubTree(b);
        MergeVariablesAndConstantsInProduct(a);
        return a;
      } else {
        var mul = mulSymbol.CreateTreeNode();
        mul.SubTrees.Add(a);
        mul.SubTrees.Add(b);
        MergeVariablesAndConstantsInProduct(mul);
        return mul;
      }
    }
    #endregion


    #region helper functions

    private bool AreSameVariable(SymbolicExpressionTreeNode a, SymbolicExpressionTreeNode b) {
      var aLaggedVar = a as LaggedVariableTreeNode;
      var bLaggedVar = b as LaggedVariableTreeNode;
      if (aLaggedVar != null && bLaggedVar != null) {
        return aLaggedVar.VariableName == bLaggedVar.VariableName &&
          aLaggedVar.Lag == bLaggedVar.Lag;
      }
      var aVar = a as VariableTreeNode;
      var bVar = b as VariableTreeNode;
      if (aVar != null && bVar != null) {
        return aVar.VariableName == bVar.VariableName;
      }
      return false;
    }

    // helper to combine the constant factors in products and to combine variables (powers of 2, 3...)
    private void MergeVariablesAndConstantsInProduct(SymbolicExpressionTreeNode prod) {
      var subtrees = new List<SymbolicExpressionTreeNode>(prod.SubTrees);
      while (prod.SubTrees.Count > 0) prod.RemoveSubTree(0);
      var groupedVarNodes = from node in subtrees.OfType<VariableTreeNode>()
                            let lag = (node is LaggedVariableTreeNode) ? ((LaggedVariableTreeNode)node).Lag : 0
                            group node by node.VariableName + lag into g
                            orderby g.Count()
                            select g;
      var constantProduct = (from node in subtrees.OfType<VariableTreeNode>()
                             select node.Weight)
                            .Concat(from node in subtrees.OfType<ConstantTreeNode>()
                                    select node.Value)
                            .DefaultIfEmpty(1.0)
                            .Aggregate((c1, c2) => c1 * c2);

      var unchangedSubTrees = from tree in subtrees
                              where !(tree is VariableTreeNode)
                              where !(tree is ConstantTreeNode)
                              select tree;

      foreach (var variableNodeGroup in groupedVarNodes) {
        var representative = variableNodeGroup.First();
        representative.Weight = 1.0;
        if (variableNodeGroup.Count() > 1) {
          var poly = mulSymbol.CreateTreeNode();
          for (int p = 0; p < variableNodeGroup.Count(); p++) {
            poly.AddSubTree((SymbolicExpressionTreeNode)representative.Clone());
          }
          prod.AddSubTree(poly);
        } else {
          prod.AddSubTree(representative);
        }
      }

      foreach (var unchangedSubtree in unchangedSubTrees)
        prod.AddSubTree(unchangedSubtree);

      if (!constantProduct.IsAlmost(1.0)) {
        prod.AddSubTree(MakeConstant(constantProduct));
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
        for (int i = 0; i < x.SubTrees.Count; i++)
          x.SubTrees[i] = Negate(x.SubTrees[i]);
      } else if (IsMultiplication(x) || IsDivision(x)) {
        // x0 * x1 * .. * xn * -1 => x0 * x1 * .. * -xn
        x.SubTrees[x.SubTrees.Count - 1] = Negate(x.SubTrees.Last()); // last is maybe a constant, prefer to negate the constant
      } else {
        // any other function
        return MakeProduct(x, MakeConstant(-1));
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
        return MakeConstant(1.0 / ((ConstantTreeNode)x).Value);
      } else if (IsDivision(x)) {
        Trace.Assert(x.SubTrees.Count == 2);
        return MakeFraction(x.SubTrees[1], x.SubTrees[0]);
      } else {
        // any other function
        return MakeFraction(MakeConstant(1), x);
      }
    }

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
    #endregion
  }
}
