#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Simplifier for symbolic expressions
  /// </summary>
  public class TreeSimplifier {
    private static readonly Addition addSymbol = new Addition();
    private static readonly Multiplication mulSymbol = new Multiplication();
    private static readonly Division divSymbol = new Division();
    private static readonly Constant constSymbol = new Constant();
    private static readonly Absolute absSymbol = new Absolute();
    private static readonly Logarithm logSymbol = new Logarithm();
    private static readonly Exponential expSymbol = new Exponential();
    private static readonly Root rootSymbol = new Root();
    private static readonly Square sqrSymbol = new Square();
    private static readonly SquareRoot sqrtSymbol = new SquareRoot();
    private static readonly AnalyticalQuotient aqSymbol = new AnalyticalQuotient();
    private static readonly Cube cubeSymbol = new Cube();
    private static readonly CubeRoot cubeRootSymbol = new CubeRoot();
    private static readonly Power powSymbol = new Power();
    private static readonly Sine sineSymbol = new Sine();
    private static readonly Cosine cosineSymbol = new Cosine();
    private static readonly Tangent tanSymbol = new Tangent();
    private static readonly IfThenElse ifThenElseSymbol = new IfThenElse();
    private static readonly And andSymbol = new And();
    private static readonly Or orSymbol = new Or();
    private static readonly Not notSymbol = new Not();
    private static readonly GreaterThan gtSymbol = new GreaterThan();
    private static readonly LessThan ltSymbol = new LessThan();
    private static readonly Integral integralSymbol = new Integral();
    private static readonly LaggedVariable laggedVariableSymbol = new LaggedVariable();
    private static readonly TimeLag timeLagSymbol = new TimeLag();

    [Obsolete("Use static method TreeSimplifier.Simplify instead")]
    public TreeSimplifier() { }

    public static ISymbolicExpressionTree Simplify(ISymbolicExpressionTree originalTree) {
      var clone = (ISymbolicExpressionTreeNode)originalTree.Root.Clone();
      // macro expand (initially no argument trees)
      var macroExpandedTree = MacroExpand(clone, clone.GetSubtree(0), new List<ISymbolicExpressionTreeNode>());
      ISymbolicExpressionTreeNode rootNode = (new ProgramRootSymbol()).CreateTreeNode();
      rootNode.AddSubtree(GetSimplifiedTree(macroExpandedTree));

#if DEBUG
      // check that each node is only referenced once
      var nodes = rootNode.IterateNodesPrefix().ToArray();
      foreach (var n in nodes) if (nodes.Count(ni => ni == n) > 1) throw new InvalidOperationException();
#endif
      return new SymbolicExpressionTree(rootNode);
    }

    // the argumentTrees list contains already expanded trees used as arguments for invocations
    private static ISymbolicExpressionTreeNode MacroExpand(ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode node,
      IList<ISymbolicExpressionTreeNode> argumentTrees) {
      List<ISymbolicExpressionTreeNode> subtrees = new List<ISymbolicExpressionTreeNode>(node.Subtrees);
      while (node.SubtreeCount > 0) node.RemoveSubtree(0);
      if (node.Symbol is InvokeFunction) {
        var invokeSym = node.Symbol as InvokeFunction;
        var defunNode = FindFunctionDefinition(root, invokeSym.FunctionName);
        var macroExpandedArguments = new List<ISymbolicExpressionTreeNode>();
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
          node.AddSubtree(MacroExpand(root, subtree, argumentTrees));
        }
        return node;
      }
    }

    private static ISymbolicExpressionTreeNode FindFunctionDefinition(ISymbolicExpressionTreeNode root, string functionName) {
      foreach (var subtree in root.Subtrees.OfType<DefunTreeNode>()) {
        if (subtree.FunctionName == functionName) return subtree.GetSubtree(0);
      }

      throw new ArgumentException("Definition of function " + functionName + " not found.");
    }

    #region symbol predicates

    // arithmetic
    private static bool IsDivision(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Division;
    }

    private static bool IsMultiplication(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Multiplication;
    }

    private static bool IsSubtraction(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Subtraction;
    }

    private static bool IsAddition(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Addition;
    }

    private static bool IsAverage(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Average;
    }

    private static bool IsAbsolute(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Absolute;
    }

    // exponential
    private static bool IsLog(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Logarithm;
    }

    private static bool IsExp(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Exponential;
    }

    private static bool IsRoot(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Root;
    }

    private static bool IsSquare(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Square;
    }

    private static bool IsSquareRoot(ISymbolicExpressionTreeNode node) {
      return node.Symbol is SquareRoot;
    }

    private static bool IsCube(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Cube;
    }

    private static bool IsCubeRoot(ISymbolicExpressionTreeNode node) {
      return node.Symbol is CubeRoot;
    }

    private static bool IsPower(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Power;
    }

    // trigonometric
    private static bool IsSine(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Sine;
    }

    private static bool IsCosine(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Cosine;
    }

    private static bool IsTangent(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Tangent;
    }

    private static bool IsAnalyticalQuotient(ISymbolicExpressionTreeNode node) {
      return node.Symbol is AnalyticalQuotient;
    }

    // boolean
    private static bool IsIfThenElse(ISymbolicExpressionTreeNode node) {
      return node.Symbol is IfThenElse;
    }

    private static bool IsAnd(ISymbolicExpressionTreeNode node) {
      return node.Symbol is And;
    }

    private static bool IsOr(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Or;
    }

    private static bool IsNot(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Not;
    }

    // comparison
    private static bool IsGreaterThan(ISymbolicExpressionTreeNode node) {
      return node.Symbol is GreaterThan;
    }

    private static bool IsLessThan(ISymbolicExpressionTreeNode node) {
      return node.Symbol is LessThan;
    }

    private static bool IsBoolean(ISymbolicExpressionTreeNode node) {
      return
        node.Symbol is GreaterThan ||
        node.Symbol is LessThan ||
        node.Symbol is And ||
        node.Symbol is Or;
    }

    // terminals
    private static bool IsVariable(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Variable;
    }

    private static bool IsVariableBase(ISymbolicExpressionTreeNode node) {
      return node is VariableTreeNodeBase;
    }

    private static bool IsFactor(ISymbolicExpressionTreeNode node) {
      return node is FactorVariableTreeNode;
    }

    private static bool IsBinFactor(ISymbolicExpressionTreeNode node) {
      return node is BinaryFactorVariableTreeNode;
    }

    private static bool IsConstant(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Constant;
    }

    // dynamic
    private static bool IsTimeLag(ISymbolicExpressionTreeNode node) {
      return node.Symbol is TimeLag;
    }

    private static bool IsIntegral(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Integral;
    }

    #endregion

    /// <summary>
    /// Creates a new simplified tree
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public static ISymbolicExpressionTreeNode GetSimplifiedTree(ISymbolicExpressionTreeNode original) {
      if (IsConstant(original) || IsVariableBase(original)) {
        return (ISymbolicExpressionTreeNode)original.Clone();
      } else if (IsAbsolute(original)) {
        return SimplifyAbsolute(original);
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
      } else if (IsSquare(original)) {
        return SimplifySquare(original);
      } else if (IsSquareRoot(original)) {
        return SimplifySquareRoot(original);
      } else if (IsCube(original)) {
        return SimplifyCube(original);
      } else if (IsCubeRoot(original)) {
        return SimplifyCubeRoot(original);
      } else if (IsPower(original)) {
        return SimplifyPower(original);
      } else if (IsRoot(original)) {
        return SimplifyRoot(original);
      } else if (IsSine(original)) {
        return SimplifySine(original);
      } else if (IsCosine(original)) {
        return SimplifyCosine(original);
      } else if (IsTangent(original)) {
        return SimplifyTangent(original);
      } else if (IsAnalyticalQuotient(original)) {
        return SimplifyAnalyticalQuotient(original);
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
      } else if (IsTimeLag(original)) {
        return SimplifyTimeLag(original);
      } else if (IsIntegral(original)) {
        return SimplifyIntegral(original);
      } else {
        return SimplifyAny(original);
      }
    }

    #region specific simplification routines

    private static ISymbolicExpressionTreeNode SimplifyAny(ISymbolicExpressionTreeNode original) {
      // can't simplify this function but simplify all subtrees 
      List<ISymbolicExpressionTreeNode> subtrees = new List<ISymbolicExpressionTreeNode>(original.Subtrees);
      while (original.Subtrees.Count() > 0) original.RemoveSubtree(0);
      var clone = (SymbolicExpressionTreeNode)original.Clone();
      List<ISymbolicExpressionTreeNode> simplifiedSubtrees = new List<ISymbolicExpressionTreeNode>();
      foreach (var subtree in subtrees) {
        simplifiedSubtrees.Add(GetSimplifiedTree(subtree));
        original.AddSubtree(subtree);
      }
      foreach (var simplifiedSubtree in simplifiedSubtrees) {
        clone.AddSubtree(simplifiedSubtree);
      }
      if (simplifiedSubtrees.TrueForAll(t => IsConstant(t))) {
        SimplifyConstantExpression(clone);
      }
      return clone;
    }

    private static ISymbolicExpressionTreeNode SimplifyConstantExpression(ISymbolicExpressionTreeNode original) {
      // not yet implemented
      return original;
    }

    private static ISymbolicExpressionTreeNode SimplifyAverage(ISymbolicExpressionTreeNode original) {
      if (original.Subtrees.Count() == 1) {
        return GetSimplifiedTree(original.GetSubtree(0));
      } else {
        // simplify expressions x0..xn
        // make sum(x0..xn) / n
        var sum = original.Subtrees
          .Select(GetSimplifiedTree)
          .Aggregate(MakeSum);
        return MakeFraction(sum, MakeConstant(original.Subtrees.Count()));
      }
    }

    private static ISymbolicExpressionTreeNode SimplifyDivision(ISymbolicExpressionTreeNode original) {
      if (original.Subtrees.Count() == 1) {
        return Invert(GetSimplifiedTree(original.GetSubtree(0)));
      } else {
        // simplify expressions x0..xn
        // make multiplication (x0 * 1/(x1 * x1 * .. * xn))
        var first = original.GetSubtree(0);
        var second = original.GetSubtree(1);
        var remaining = original.Subtrees.Skip(2);
        return
          MakeProduct(GetSimplifiedTree(first),
            Invert(remaining.Aggregate(GetSimplifiedTree(second), (a, b) => MakeProduct(a, GetSimplifiedTree(b)))));
      }
    }

    private static ISymbolicExpressionTreeNode SimplifyMultiplication(ISymbolicExpressionTreeNode original) {
      if (original.Subtrees.Count() == 1) {
        return GetSimplifiedTree(original.GetSubtree(0));
      } else {
        return original.Subtrees
          .Select(GetSimplifiedTree)
          .Aggregate(MakeProduct);
      }
    }

    private static ISymbolicExpressionTreeNode SimplifySubtraction(ISymbolicExpressionTreeNode original) {
      if (original.Subtrees.Count() == 1) {
        return Negate(GetSimplifiedTree(original.GetSubtree(0)));
      } else {
        // simplify expressions x0..xn
        // make addition (x0,-x1..-xn)
        var first = original.Subtrees.First();
        var remaining = original.Subtrees.Skip(1);
        return remaining.Aggregate(GetSimplifiedTree(first), (a, b) => MakeSum(a, Negate(GetSimplifiedTree(b))));
      }
    }

    private static ISymbolicExpressionTreeNode SimplifyAddition(ISymbolicExpressionTreeNode original) {
      if (original.Subtrees.Count() == 1) {
        return GetSimplifiedTree(original.GetSubtree(0));
      } else {
        // simplify expression x0..xn
        // make addition (x0..xn)
        return original.Subtrees
          .Select(GetSimplifiedTree)
          .Aggregate(MakeSum);
      }
    }

    private static ISymbolicExpressionTreeNode SimplifyAbsolute(ISymbolicExpressionTreeNode original) {
      return MakeAbs(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyNot(ISymbolicExpressionTreeNode original) {
      return MakeNot(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyOr(ISymbolicExpressionTreeNode original) {
      return original.Subtrees
        .Select(GetSimplifiedTree)
        .Aggregate(MakeOr);
    }

    private static ISymbolicExpressionTreeNode SimplifyAnd(ISymbolicExpressionTreeNode original) {
      return original.Subtrees
        .Select(GetSimplifiedTree)
        .Aggregate(MakeAnd);
    }

    private static ISymbolicExpressionTreeNode SimplifyLessThan(ISymbolicExpressionTreeNode original) {
      return MakeLessThan(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)));
    }

    private static ISymbolicExpressionTreeNode SimplifyGreaterThan(ISymbolicExpressionTreeNode original) {
      return MakeGreaterThan(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)));
    }

    private static ISymbolicExpressionTreeNode SimplifyIfThenElse(ISymbolicExpressionTreeNode original) {
      return MakeIfThenElse(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)),
        GetSimplifiedTree(original.GetSubtree(2)));
    }

    private static ISymbolicExpressionTreeNode SimplifyTangent(ISymbolicExpressionTreeNode original) {
      return MakeTangent(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyCosine(ISymbolicExpressionTreeNode original) {
      return MakeCosine(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifySine(ISymbolicExpressionTreeNode original) {
      return MakeSine(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyExp(ISymbolicExpressionTreeNode original) {
      return MakeExp(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifySquare(ISymbolicExpressionTreeNode original) {
      return MakeSquare(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifySquareRoot(ISymbolicExpressionTreeNode original) {
      return MakeSquareRoot(GetSimplifiedTree(original.GetSubtree(0)));
    }
    private static ISymbolicExpressionTreeNode SimplifyCube(ISymbolicExpressionTreeNode original) {
      return MakeCube(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyCubeRoot(ISymbolicExpressionTreeNode original) {
      return MakeCubeRoot(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyLog(ISymbolicExpressionTreeNode original) {
      return MakeLog(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyRoot(ISymbolicExpressionTreeNode original) {
      return MakeRoot(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)));
    }

    private static ISymbolicExpressionTreeNode SimplifyPower(ISymbolicExpressionTreeNode original) {
      return MakePower(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)));
    }

    private static ISymbolicExpressionTreeNode SimplifyAnalyticalQuotient(ISymbolicExpressionTreeNode original) {
      return MakeAnalyticalQuotient(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)));
    }

    private static ISymbolicExpressionTreeNode SimplifyTimeLag(ISymbolicExpressionTreeNode original) {
      var laggedTreeNode = original as ILaggedTreeNode;
      var simplifiedSubtree = GetSimplifiedTree(original.GetSubtree(0));
      if (!ContainsVariableCondition(simplifiedSubtree)) {
        return AddLagToDynamicNodes(simplifiedSubtree, laggedTreeNode.Lag);
      } else {
        return MakeTimeLag(simplifiedSubtree, laggedTreeNode.Lag);
      }
    }

    private static ISymbolicExpressionTreeNode SimplifyIntegral(ISymbolicExpressionTreeNode original) {
      var laggedTreeNode = original as ILaggedTreeNode;
      var simplifiedSubtree = GetSimplifiedTree(original.GetSubtree(0));
      if (IsConstant(simplifiedSubtree)) {
        return GetSimplifiedTree(MakeProduct(simplifiedSubtree, MakeConstant(-laggedTreeNode.Lag)));
      } else {
        return MakeIntegral(simplifiedSubtree, laggedTreeNode.Lag);
      }
    }

    #endregion

    #region low level tree restructuring

    private static ISymbolicExpressionTreeNode MakeTimeLag(ISymbolicExpressionTreeNode subtree, int lag) {
      if (lag == 0) return subtree;
      if (IsConstant(subtree)) return subtree;
      var lagNode = (LaggedTreeNode)timeLagSymbol.CreateTreeNode();
      lagNode.Lag = lag;
      lagNode.AddSubtree(subtree);
      return lagNode;
    }

    private static ISymbolicExpressionTreeNode MakeIntegral(ISymbolicExpressionTreeNode subtree, int lag) {
      if (lag == 0) return subtree;
      else if (lag == -1 || lag == 1) {
        return MakeSum(subtree, AddLagToDynamicNodes((ISymbolicExpressionTreeNode)subtree.Clone(), lag));
      } else {
        var node = (LaggedTreeNode)integralSymbol.CreateTreeNode();
        node.Lag = lag;
        node.AddSubtree(subtree);
        return node;
      }
    }

    private static ISymbolicExpressionTreeNode MakeNot(ISymbolicExpressionTreeNode t) {
      if (IsConstant(t)) {
        var constNode = t as ConstantTreeNode;
        if (constNode.Value > 0) return MakeConstant(-1.0);
        else return MakeConstant(1.0);
      } else if (IsNot(t)) {
        return t.GetSubtree(0);
      } else if (!IsBoolean(t)) {
        var gtNode = gtSymbol.CreateTreeNode();
        gtNode.AddSubtree(t);
        gtNode.AddSubtree(MakeConstant(0.0));
        var notNode = notSymbol.CreateTreeNode();
        notNode.AddSubtree(gtNode);
        return notNode;
      } else {
        var notNode = notSymbol.CreateTreeNode();
        notNode.AddSubtree(t);
        return notNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeOr(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
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
          orNode.AddSubtree(a);
          return orNode;
        }
      } else {
        var orNode = orSymbol.CreateTreeNode();
        orNode.AddSubtree(a);
        orNode.AddSubtree(b);
        return orNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeAnd(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
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
          andNode.AddSubtree(a);
          return andNode;
        } else {
          // boolean expression is necessarily false
          return MakeConstant(-1.0);
        }
      } else {
        var andNode = andSymbol.CreateTreeNode();
        andNode.AddSubtree(a);
        andNode.AddSubtree(b);
        return andNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeLessThan(ISymbolicExpressionTreeNode leftSide,
      ISymbolicExpressionTreeNode rightSide) {
      if (IsConstant(leftSide) && IsConstant(rightSide)) {
        var lsConst = leftSide as ConstantTreeNode;
        var rsConst = rightSide as ConstantTreeNode;
        if (lsConst.Value < rsConst.Value) return MakeConstant(1.0);
        else return MakeConstant(-1.0);
      } else {
        var ltNode = ltSymbol.CreateTreeNode();
        ltNode.AddSubtree(leftSide);
        ltNode.AddSubtree(rightSide);
        return ltNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeGreaterThan(ISymbolicExpressionTreeNode leftSide,
      ISymbolicExpressionTreeNode rightSide) {
      if (IsConstant(leftSide) && IsConstant(rightSide)) {
        var lsConst = leftSide as ConstantTreeNode;
        var rsConst = rightSide as ConstantTreeNode;
        if (lsConst.Value > rsConst.Value) return MakeConstant(1.0);
        else return MakeConstant(-1.0);
      } else {
        var gtNode = gtSymbol.CreateTreeNode();
        gtNode.AddSubtree(leftSide);
        gtNode.AddSubtree(rightSide);
        return gtNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeIfThenElse(ISymbolicExpressionTreeNode condition,
      ISymbolicExpressionTreeNode trueBranch, ISymbolicExpressionTreeNode falseBranch) {
      if (IsConstant(condition)) {
        var constT = condition as ConstantTreeNode;
        if (constT.Value > 0.0) return trueBranch;
        else return falseBranch;
      } else {
        var ifNode = ifThenElseSymbol.CreateTreeNode();
        if (IsBoolean(condition)) {
          ifNode.AddSubtree(condition);
        } else {
          var gtNode = gtSymbol.CreateTreeNode();
          gtNode.AddSubtree(condition);
          gtNode.AddSubtree(MakeConstant(0.0));
          ifNode.AddSubtree(gtNode);
        }
        ifNode.AddSubtree(trueBranch);
        ifNode.AddSubtree(falseBranch);
        return ifNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeSine(ISymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Sin(constT.Value));
      } else if (IsFactor(node)) {
        var factor = node as FactorVariableTreeNode;
        return MakeFactor(factor.Symbol, factor.VariableName, factor.Weights.Select(Math.Sin));
      } else if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Sin(binFactor.Weight));
      } else {
        var sineNode = sineSymbol.CreateTreeNode();
        sineNode.AddSubtree(node);
        return sineNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeTangent(ISymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Tan(constT.Value));
      } else if (IsFactor(node)) {
        var factor = node as FactorVariableTreeNode;
        return MakeFactor(factor.Symbol, factor.VariableName, factor.Weights.Select(Math.Tan));
      } else if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Tan(binFactor.Weight));
      } else {
        var tanNode = tanSymbol.CreateTreeNode();
        tanNode.AddSubtree(node);
        return tanNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeCosine(ISymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Cos(constT.Value));
      } else if (IsFactor(node)) {
        var factor = node as FactorVariableTreeNode;
        return MakeFactor(factor.Symbol, factor.VariableName, factor.Weights.Select(Math.Cos));
      } else if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        // cos(0) = 1 see similar case for Exp(binfactor)
        return MakeSum(MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Cos(binFactor.Weight) - 1),
          MakeConstant(1.0));
      } else {
        var cosNode = cosineSymbol.CreateTreeNode();
        cosNode.AddSubtree(node);
        return cosNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeExp(ISymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Exp(constT.Value));
      } else if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Exp(w)));
      } else if (IsBinFactor(node)) {
        // exp( binfactor w val=a) = if(val=a) exp(w) else exp(0) = binfactor( (exp(w) - 1) val a) + 1
        var binFactor = node as BinaryFactorVariableTreeNode;
        return
          MakeSum(MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Exp(binFactor.Weight) - 1), MakeConstant(1.0));
      } else if (IsLog(node)) {
        return node.GetSubtree(0);
      } else if (IsAddition(node)) {
        return node.Subtrees.Select(s => MakeExp(s)).Aggregate((s, t) => MakeProduct(s, t));
      } else if (IsSubtraction(node)) {
        return node.Subtrees.Select(s => MakeExp(s)).Aggregate((s, t) => MakeProduct(s, Negate(t)));
      } else {
        var expNode = expSymbol.CreateTreeNode();
        expNode.AddSubtree(node);
        return expNode;
      }
    }
    private static ISymbolicExpressionTreeNode MakeLog(ISymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Log(constT.Value));
      } else if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Log(w)));
      } else if (IsExp(node)) {
        return node.GetSubtree(0);
      } else if (IsSquareRoot(node)) {
        return MakeFraction(MakeLog(node.GetSubtree(0)), MakeConstant(2.0));
      } else {
        var logNode = logSymbol.CreateTreeNode();
        logNode.AddSubtree(node);
        return logNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeSquare(ISymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(constT.Value * constT.Value);
      } else if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => w * w));
      } else if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, binFactor.Weight * binFactor.Weight);
      } else if (IsSquareRoot(node)) {
        return node.GetSubtree(0);
      } else if (IsMultiplication(node)) {
        // sqr( x * y ) = sqr(x) * sqr(y)
        var mulNode = mulSymbol.CreateTreeNode();
        foreach (var subtree in node.Subtrees) {
          mulNode.AddSubtree(MakeSquare(subtree));
        }
        return mulNode;
      } else if (IsAbsolute(node)) {
        return MakeSquare(node.GetSubtree(0)); // sqr(abs(x)) = sqr(x)
      } else if (IsExp(node)) {
        return MakeExp(MakeProduct(node.GetSubtree(0), MakeConstant(2.0))); // sqr(exp(x)) = exp(2x)
      } else if (IsCube(node)) {
        return MakePower(node.GetSubtree(0), MakeConstant(6));
      } else {
        var sqrNode = sqrSymbol.CreateTreeNode();
        sqrNode.AddSubtree(node);
        return sqrNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeCube(ISymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(constT.Value * constT.Value * constT.Value);
      } else if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => w * w * w));
      } else if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, binFactor.Weight * binFactor.Weight * binFactor.Weight);
      } else if (IsCubeRoot(node)) {
        return node.GetSubtree(0); // NOTE: not really accurate because cuberoot(x) with negative x is evaluated to NaN and after this simplification we evaluate as x
      } else if (IsExp(node)) {
        return MakeExp(MakeProduct(node.GetSubtree(0), MakeConstant(3)));
      } else if (IsSquare(node)) {
        return MakePower(node.GetSubtree(0), MakeConstant(6));
      } else {
        var cubeNode = cubeSymbol.CreateTreeNode();
        cubeNode.AddSubtree(node);
        return cubeNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeAbs(ISymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Abs(constT.Value));
      } else if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Abs(w)));
      } else if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Abs(binFactor.Weight));
      } else if (IsSquare(node) || IsExp(node) || IsSquareRoot(node) || IsCubeRoot(node)) {
        return node; // abs(sqr(x)) = sqr(x), abs(exp(x)) = exp(x) ...
      } else if (IsMultiplication(node)) {
        var mul = mulSymbol.CreateTreeNode();
        foreach (var st in node.Subtrees) {
          mul.AddSubtree(MakeAbs(st));
        }
        return mul;
      } else if (IsDivision(node)) {
        var div = divSymbol.CreateTreeNode();
        foreach (var st in node.Subtrees) {
          div.AddSubtree(MakeAbs(st));
        }
        return div;
      } else {
        var absNode = absSymbol.CreateTreeNode();
        absNode.AddSubtree(node);
        return absNode;
      }
    }

    // constant folding only
    private static ISymbolicExpressionTreeNode MakeAnalyticalQuotient(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (IsConstant(b)) {
        var c = b as ConstantTreeNode;
        return MakeFraction(a, MakeConstant(Math.Sqrt(1.0 + c.Value * c.Value)));
      } else if (IsFactor(b)) {
        var factNode = b as FactorVariableTreeNode;
        return MakeFraction(a, MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Sqrt(1.0 + w * w))));
      } else if (IsBinFactor(b)) {
        var binFactor = b as BinaryFactorVariableTreeNode;
        return MakeFraction(a, MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Sqrt(1.0 + binFactor.Weight * binFactor.Weight)));
      } else {
        var aqNode = aqSymbol.CreateTreeNode();
        aqNode.AddSubtree(a);
        aqNode.AddSubtree(b);
        return aqNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeSquareRoot(ISymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Sqrt(constT.Value));
      } else if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Sqrt(w)));
      } else if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Sqrt(binFactor.Weight));
      } else if (IsSquare(node)) {
        return node.GetSubtree(0); // NOTE: not really accurate because sqrt(x) with negative x is evaluated to NaN and after this simplification we evaluate as x
      } else {
        var sqrtNode = sqrtSymbol.CreateTreeNode();
        sqrtNode.AddSubtree(node);
        return sqrtNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeCubeRoot(ISymbolicExpressionTreeNode node) {
      if (IsConstant(node)) {
        var constT = node as ConstantTreeNode;
        return MakeConstant(Math.Pow(constT.Value, 1.0 / 3.0));
      } else if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Pow(w, 1.0 / 3.0)));
      } else if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Sqrt(Math.Pow(binFactor.Weight, 1.0 / 3.0)));
      } else if (IsCube(node)) {
        return node.GetSubtree(0);
      } else {
        var cubeRootNode = cubeRootSymbol.CreateTreeNode();
        cubeRootNode.AddSubtree(node);
        return cubeRootNode;
      }
    }

    private static ISymbolicExpressionTreeNode MakeRoot(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        var constA = a as ConstantTreeNode;
        var constB = b as ConstantTreeNode;
        return MakeConstant(Math.Pow(constA.Value, 1.0 / Math.Round(constB.Value)));
      } else if (IsFactor(a) && IsConstant(b)) {
        var factNode = a as FactorVariableTreeNode;
        var constNode = b as ConstantTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName,
          factNode.Weights.Select(w => Math.Pow(w, 1.0 / Math.Round(constNode.Value))));
      } else if (IsBinFactor(a) && IsConstant(b)) {
        var binFactor = a as BinaryFactorVariableTreeNode;
        var constNode = b as ConstantTreeNode;
        return MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Pow(binFactor.Weight, 1.0 / Math.Round(constNode.Value)));
      } else if (IsConstant(a) && IsFactor(b)) {
        var constNode = a as ConstantTreeNode;
        var factNode = b as FactorVariableTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Pow(constNode.Value, 1.0 / Math.Round(w))));
      } else if (IsConstant(a) && IsBinFactor(b)) {
        var constNode = a as ConstantTreeNode;
        var factNode = b as BinaryFactorVariableTreeNode;
        return MakeBinFactor(factNode.Symbol, factNode.VariableName, factNode.VariableValue, Math.Pow(constNode.Value, 1.0 / Math.Round(factNode.Weight)));
      } else if (IsFactor(a) && IsFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as FactorVariableTreeNode;
        return MakeFactor(node0.Symbol, node0.VariableName, node0.Weights.Zip(node1.Weights, (u, v) => Math.Pow(u, 1.0 / Math.Round(v))));
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
          rootNode.AddSubtree(a);
          rootNode.AddSubtree(MakeConstant(-1.0 * constBValue));
          return MakeFraction(MakeConstant(1.0), rootNode);
        } else {
          var rootNode = rootSymbol.CreateTreeNode();
          rootNode.AddSubtree(a);
          rootNode.AddSubtree(MakeConstant(constBValue));
          return rootNode;
        }
      } else {
        var rootNode = rootSymbol.CreateTreeNode();
        rootNode.AddSubtree(a);
        rootNode.AddSubtree(b);
        return rootNode;
      }
    }


    private static ISymbolicExpressionTreeNode MakePower(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        var constA = a as ConstantTreeNode;
        var constB = b as ConstantTreeNode;
        return MakeConstant(Math.Pow(constA.Value, Math.Round(constB.Value)));
      } else if (IsFactor(a) && IsConstant(b)) {
        var factNode = a as FactorVariableTreeNode;
        var constNode = b as ConstantTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Pow(w, Math.Round(constNode.Value))));
      } else if (IsBinFactor(a) && IsConstant(b)) {
        var binFactor = a as BinaryFactorVariableTreeNode;
        var constNode = b as ConstantTreeNode;
        return MakeBinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Pow(binFactor.Weight, Math.Round(constNode.Value)));
      } else if (IsConstant(a) && IsFactor(b)) {
        var constNode = a as ConstantTreeNode;
        var factNode = b as FactorVariableTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Pow(constNode.Value, Math.Round(w))));
      } else if (IsConstant(a) && IsBinFactor(b)) {
        var constNode = a as ConstantTreeNode;
        var factNode = b as BinaryFactorVariableTreeNode;
        return MakeBinFactor(factNode.Symbol, factNode.VariableName, factNode.VariableValue, Math.Pow(constNode.Value, Math.Round(factNode.Weight)));
      } else if (IsFactor(a) && IsFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as FactorVariableTreeNode;
        return MakeFactor(node0.Symbol, node0.VariableName, node0.Weights.Zip(node1.Weights, (u, v) => Math.Pow(u, Math.Round(v))));
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
          powNode.AddSubtree(a);
          powNode.AddSubtree(MakeConstant(-1.0 * exponent));
          return MakeFraction(MakeConstant(1.0), powNode);
        } else {
          var powNode = powSymbol.CreateTreeNode();
          powNode.AddSubtree(a);
          powNode.AddSubtree(MakeConstant(exponent));
          return powNode;
        }
      } else {
        var powNode = powSymbol.CreateTreeNode();
        powNode.AddSubtree(a);
        powNode.AddSubtree(b);
        return powNode;
      }
    }


    // MakeFraction, MakeProduct and MakeSum take two already simplified trees and create a new simplified tree
    private static ISymbolicExpressionTreeNode MakeFraction(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        // fold constants
        return MakeConstant(((ConstantTreeNode)a).Value / ((ConstantTreeNode)b).Value);
      } else if ((IsConstant(a) && !((ConstantTreeNode)a).Value.IsAlmost(1.0))) {
        return MakeFraction(MakeConstant(1.0), MakeProduct(b, Invert(a)));
      } else if (IsVariableBase(a) && IsConstant(b)) {
        // merge constant values into variable weights
        var constB = ((ConstantTreeNode)b).Value;
        ((VariableTreeNodeBase)a).Weight /= constB;
        return a;
      } else if (IsFactor(a) && IsConstant(b)) {
        var factNode = a as FactorVariableTreeNode;
        var constNode = b as ConstantTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => w / constNode.Value));
      } else if (IsBinFactor(a) && IsConstant(b)) {
        var factNode = a as BinaryFactorVariableTreeNode;
        var constNode = b as ConstantTreeNode;
        return MakeBinFactor(factNode.Symbol, factNode.VariableName, factNode.VariableValue, factNode.Weight / constNode.Value);
      } else if (IsFactor(a) && IsFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as FactorVariableTreeNode;
        return MakeFactor(node0.Symbol, node0.VariableName, node0.Weights.Zip(node1.Weights, (u, v) => u / v));
      } else if (IsFactor(a) && IsBinFactor(b) && ((IVariableTreeNode)a).VariableName == ((IVariableTreeNode)b).VariableName) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as BinaryFactorVariableTreeNode;
        var varValues = node0.Symbol.GetVariableValues(node0.VariableName).ToArray();
        var wi = Array.IndexOf(varValues, node1.VariableValue);
        if (wi < 0) throw new ArgumentException();
        var newWeighs = new double[varValues.Length];
        node0.Weights.CopyTo(newWeighs, 0);
        for (int i = 0; i < newWeighs.Length; i++)
          if (wi == i) newWeighs[i] /= node1.Weight;
          else newWeighs[i] /= 0.0;
        return MakeFactor(node0.Symbol, node0.VariableName, newWeighs);
      } else if (IsFactor(a)) {
        return MakeFraction(MakeConstant(1.0), MakeProduct(b, Invert(a)));
      } else if (IsVariableBase(a) && IsVariableBase(b) && AreSameTypeAndVariable(a, b) && !IsBinFactor(b)) {
        // cancel variables (not allowed for bin factors because of division by zero)
        var aVar = a as VariableTreeNode;
        var bVar = b as VariableTreeNode;
        return MakeConstant(aVar.Weight / bVar.Weight);
      } else if (IsAddition(a) && IsConstant(b)) {
        return a.Subtrees
          .Select(x => GetSimplifiedTree(x))
          .Select(x => MakeFraction(x, GetSimplifiedTree(b)))
          .Aggregate((c, d) => MakeSum(c, d));
      } else if (IsMultiplication(a) && IsConstant(b)) {
        return MakeProduct(a, Invert(b));
      } else if (IsDivision(a) && IsConstant(b)) {
        // (a1 / a2) / c => (a1 / (a2 * c))
        return MakeFraction(a.GetSubtree(0), MakeProduct(a.GetSubtree(1), b));
      } else if (IsDivision(a) && IsDivision(b)) {
        // (a1 / a2) / (b1 / b2) => 
        return MakeFraction(MakeProduct(a.GetSubtree(0), b.GetSubtree(1)), MakeProduct(a.GetSubtree(1), b.GetSubtree(0)));
      } else if (IsDivision(a)) {
        // (a1 / a2) / b => (a1 / (a2 * b))
        return MakeFraction(a.GetSubtree(0), MakeProduct(a.GetSubtree(1), b));
      } else if (IsDivision(b)) {
        // a / (b1 / b2) => (a * b2) / b1
        return MakeFraction(MakeProduct(a, b.GetSubtree(1)), b.GetSubtree(0));
      } else if (IsAnalyticalQuotient(a)) {
        return MakeAnalyticalQuotient(a.GetSubtree(0), MakeProduct(a.GetSubtree(1), b));
      } else {
        var div = divSymbol.CreateTreeNode();
        div.AddSubtree(a);
        div.AddSubtree(b);
        return div;
      }
    }

    private static ISymbolicExpressionTreeNode MakeSum(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
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
      } else if (IsFactor(a) && IsConstant(b)) {
        var factNode = a as FactorVariableTreeNode;
        var constNode = b as ConstantTreeNode;
        return MakeFactor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select((w) => w + constNode.Value));
      } else if (IsFactor(a) && IsFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as FactorVariableTreeNode;
        return MakeFactor(node0.Symbol, node0.VariableName, node0.Weights.Zip(node1.Weights, (u, v) => u + v));
      } else if (IsBinFactor(a) && IsFactor(b)) {
        return MakeSum(b, a);
      } else if (IsFactor(a) && IsBinFactor(b) &&
        ((IVariableTreeNode)a).VariableName == ((IVariableTreeNode)b).VariableName) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as BinaryFactorVariableTreeNode;
        var varValues = node0.Symbol.GetVariableValues(node0.VariableName).ToArray();
        var wi = Array.IndexOf(varValues, node1.VariableValue);
        if (wi < 0) throw new ArgumentException();
        var newWeighs = new double[varValues.Length];
        node0.Weights.CopyTo(newWeighs, 0);
        newWeighs[wi] += node1.Weight;
        return MakeFactor(node0.Symbol, node0.VariableName, newWeighs);
      } else if (IsAddition(a) && IsAddition(b)) {
        // merge additions
        var add = addSymbol.CreateTreeNode();
        // add all sub trees except for the last
        for (int i = 0; i < a.Subtrees.Count() - 1; i++) add.AddSubtree(a.GetSubtree(i));
        for (int i = 0; i < b.Subtrees.Count() - 1; i++) add.AddSubtree(b.GetSubtree(i));
        if (IsConstant(a.Subtrees.Last()) && IsConstant(b.Subtrees.Last())) {
          add.AddSubtree(MakeSum(a.Subtrees.Last(), b.Subtrees.Last()));
        } else if (IsConstant(a.Subtrees.Last())) {
          add.AddSubtree(b.Subtrees.Last());
          add.AddSubtree(a.Subtrees.Last());
        } else {
          add.AddSubtree(a.Subtrees.Last());
          add.AddSubtree(b.Subtrees.Last());
        }
        MergeVariablesInSum(add);
        if (add.Subtrees.Count() == 1) {
          return add.GetSubtree(0);
        } else {
          return add;
        }
      } else if (IsAddition(b)) {
        return MakeSum(b, a);
      } else if (IsAddition(a) && IsConstant(b)) {
        // a is an addition and b is a constant => append b to a and make sure the constants are merged
        var add = addSymbol.CreateTreeNode();
        // add all sub trees except for the last
        for (int i = 0; i < a.Subtrees.Count() - 1; i++) add.AddSubtree(a.GetSubtree(i));
        if (IsConstant(a.Subtrees.Last()))
          add.AddSubtree(MakeSum(a.Subtrees.Last(), b));
        else {
          add.AddSubtree(a.Subtrees.Last());
          add.AddSubtree(b);
        }
        return add;
      } else if (IsAddition(a)) {
        // a is already an addition => append b
        var add = addSymbol.CreateTreeNode();
        add.AddSubtree(b);
        foreach (var subtree in a.Subtrees) {
          add.AddSubtree(subtree);
        }
        MergeVariablesInSum(add);
        if (add.Subtrees.Count() == 1) {
          return add.GetSubtree(0);
        } else {
          return add;
        }
      } else {
        var add = addSymbol.CreateTreeNode();
        add.AddSubtree(a);
        add.AddSubtree(b);
        MergeVariablesInSum(add);
        if (add.Subtrees.Count() == 1) {
          return add.GetSubtree(0);
        } else {
          return add;
        }
      }
    }

    // makes sure variable symbols in sums are combined
    private static void MergeVariablesInSum(ISymbolicExpressionTreeNode sum) {
      var subtrees = new List<ISymbolicExpressionTreeNode>(sum.Subtrees);
      while (sum.Subtrees.Any()) sum.RemoveSubtree(0);
      var groupedVarNodes = from node in subtrees.OfType<IVariableTreeNode>()
                            where node.SubtreeCount == 0
                            group node by GroupId(node) into g
                            select g;
      var constant = (from node in subtrees.OfType<ConstantTreeNode>()
                      select node.Value).DefaultIfEmpty(0.0).Sum();
      var unchangedSubtrees = subtrees.Where(t => t.SubtreeCount > 0 || !(t is IVariableTreeNode) && !(t is ConstantTreeNode));

      foreach (var variableNodeGroup in groupedVarNodes) {
        var firstNode = variableNodeGroup.First();
        if (firstNode is VariableTreeNodeBase) {
          var representative = firstNode as VariableTreeNodeBase;
          var weightSum = variableNodeGroup.Cast<VariableTreeNodeBase>().Select(t => t.Weight).Sum();
          representative.Weight = weightSum;
          sum.AddSubtree(representative);
        } else if (firstNode is FactorVariableTreeNode) {
          var representative = firstNode as FactorVariableTreeNode;
          foreach (var node in variableNodeGroup.Skip(1).Cast<FactorVariableTreeNode>()) {
            for (int j = 0; j < representative.Weights.Length; j++) {
              representative.Weights[j] += node.Weights[j];
            }
          }
          sum.AddSubtree(representative);
        }
      }
      foreach (var unchangedSubtree in unchangedSubtrees)
        sum.AddSubtree(unchangedSubtree);
      if (!constant.IsAlmost(0.0)) {
        sum.AddSubtree(MakeConstant(constant));
      }
    }

    // nodes referencing variables can be grouped if they have
    private static string GroupId(IVariableTreeNode node) {
      var binaryFactorNode = node as BinaryFactorVariableTreeNode;
      var factorNode = node as FactorVariableTreeNode;
      var variableNode = node as VariableTreeNode;
      var laggedVarNode = node as LaggedVariableTreeNode;
      if (variableNode != null) {
        return "var " + variableNode.VariableName;
      } else if (binaryFactorNode != null) {
        return "binfactor " + binaryFactorNode.VariableName + " " + binaryFactorNode.VariableValue;
      } else if (factorNode != null) {
        return "factor " + factorNode.VariableName;
      } else if (laggedVarNode != null) {
        return "lagged " + laggedVarNode.VariableName + " " + laggedVarNode.Lag;
      } else {
        throw new NotSupportedException();
      }
    }


    private static ISymbolicExpressionTreeNode MakeProduct(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (IsConstant(a) && IsConstant(b)) {
        // fold constants
        return MakeConstant(((ConstantTreeNode)a).Value * ((ConstantTreeNode)b).Value);
      } else if (IsConstant(a)) {
        // a * $ => $ * a
        return MakeProduct(b, a);
      } else if (IsFactor(a) && IsFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as FactorVariableTreeNode;
        return MakeFactor(node0.Symbol, node0.VariableName, node0.Weights.Zip(node1.Weights, (u, v) => u * v));
      } else if (IsBinFactor(a) && IsBinFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as BinaryFactorVariableTreeNode;
        var node1 = b as BinaryFactorVariableTreeNode;
        return MakeBinFactor(node0.Symbol, node0.VariableName, node0.VariableValue, node0.Weight * node1.Weight);
      } else if (IsFactor(a) && IsConstant(b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as ConstantTreeNode;
        return MakeFactor(node0.Symbol, node0.VariableName, node0.Weights.Select(w => w * node1.Value));
      } else if (IsBinFactor(a) && IsConstant(b)) {
        var node0 = a as BinaryFactorVariableTreeNode;
        var node1 = b as ConstantTreeNode;
        return MakeBinFactor(node0.Symbol, node0.VariableName, node0.VariableValue, node0.Weight * node1.Value);
      } else if (IsBinFactor(a) && IsFactor(b)) {
        return MakeProduct(b, a);
      } else if (IsFactor(a) && IsBinFactor(b) &&
        ((IVariableTreeNode)a).VariableName == ((IVariableTreeNode)b).VariableName) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as BinaryFactorVariableTreeNode;
        var varValues = node0.Symbol.GetVariableValues(node0.VariableName).ToArray();
        var wi = Array.IndexOf(varValues, node1.VariableValue);
        if (wi < 0) throw new ArgumentException();
        return MakeBinFactor(node1.Symbol, node1.VariableName, node1.VariableValue, node1.Weight * node0.Weights[wi]);
      } else if (IsConstant(b) && ((ConstantTreeNode)b).Value.IsAlmost(1.0)) {
        // $ * 1.0 => $
        return a;
      } else if (IsConstant(b) && ((ConstantTreeNode)b).Value.IsAlmost(0.0)) {
        return MakeConstant(0);
      } else if (IsConstant(b) && IsVariableBase(a)) {
        // multiply constants into variables weights
        ((VariableTreeNodeBase)a).Weight *= ((ConstantTreeNode)b).Value;
        return a;
      } else if (IsConstant(b) && IsAddition(a) ||
          IsFactor(b) && IsAddition(a) ||
          IsBinFactor(b) && IsAddition(a)) {
        // multiply constants into additions
        return a.Subtrees.Select(x => MakeProduct(GetSimplifiedTree(x), GetSimplifiedTree(b))).Aggregate((c, d) => MakeSum(c, d));
      } else if (IsDivision(a) && IsDivision(b)) {
        // (a1 / a2) * (b1 / b2) => (a1 * b1) / (a2 * b2)
        return MakeFraction(MakeProduct(a.GetSubtree(0), b.GetSubtree(0)), MakeProduct(a.GetSubtree(1), b.GetSubtree(1)));
      } else if (IsDivision(a)) {
        // (a1 / a2) * b => (a1 * b) / a2
        return MakeFraction(MakeProduct(a.GetSubtree(0), b), a.GetSubtree(1));
      } else if (IsDivision(b)) {
        // a * (b1 / b2) => (b1 * a) / b2
        return MakeFraction(MakeProduct(b.GetSubtree(0), a), b.GetSubtree(1));
      } else if (IsMultiplication(a) && IsMultiplication(b)) {
        // merge multiplications (make sure constants are merged)
        var mul = mulSymbol.CreateTreeNode();
        for (int i = 0; i < a.Subtrees.Count(); i++) mul.AddSubtree(a.GetSubtree(i));
        for (int i = 0; i < b.Subtrees.Count(); i++) mul.AddSubtree(b.GetSubtree(i));
        MergeVariablesAndConstantsInProduct(mul);
        return mul;
      } else if (IsMultiplication(b)) {
        return MakeProduct(b, a);
      } else if (IsMultiplication(a)) {
        // a is already an multiplication => append b
        a.AddSubtree(GetSimplifiedTree(b));
        MergeVariablesAndConstantsInProduct(a);
        return a;
      } else if (IsAbsolute(a) && IsAbsolute(b)) {
        return MakeAbs(MakeProduct(a.GetSubtree(0), b.GetSubtree(0)));
      } else if (IsAbsolute(a) && IsConstant(b)) {
        var constNode = b as ConstantTreeNode;
        var posF = Math.Abs(constNode.Value);
        if (constNode.Value > 0) {
          return MakeAbs(MakeProduct(a.GetSubtree(0), MakeConstant(posF)));
        } else {
          var mul = mulSymbol.CreateTreeNode();
          mul.AddSubtree(MakeAbs(MakeProduct(a.GetSubtree(0), MakeConstant(posF))));
          mul.AddSubtree(MakeConstant(-1.0));
          return mul;
        }
      } else if (IsAnalyticalQuotient(a)) {
        return MakeAnalyticalQuotient(MakeProduct(a.GetSubtree(0), b), a.GetSubtree(1));
      } else {
        var mul = mulSymbol.CreateTreeNode();
        mul.AddSubtree(a);
        mul.AddSubtree(b);
        MergeVariablesAndConstantsInProduct(mul);
        return mul;
      }
    }

    #endregion

    #region helper functions

    private static bool ContainsVariableCondition(ISymbolicExpressionTreeNode node) {
      if (node.Symbol is VariableCondition) return true;
      foreach (var subtree in node.Subtrees)
        if (ContainsVariableCondition(subtree)) return true;
      return false;
    }

    private static ISymbolicExpressionTreeNode AddLagToDynamicNodes(ISymbolicExpressionTreeNode node, int lag) {
      var laggedTreeNode = node as ILaggedTreeNode;
      var variableNode = node as VariableTreeNode;
      var variableConditionNode = node as VariableConditionTreeNode;
      if (laggedTreeNode != null)
        laggedTreeNode.Lag += lag;
      else if (variableNode != null) {
        var laggedVariableNode = (LaggedVariableTreeNode)laggedVariableSymbol.CreateTreeNode();
        laggedVariableNode.Lag = lag;
        laggedVariableNode.VariableName = variableNode.VariableName;
        return laggedVariableNode;
      } else if (variableConditionNode != null) {
        throw new NotSupportedException("Removal of time lags around variable condition symbols is not allowed.");
      }
      var subtrees = new List<ISymbolicExpressionTreeNode>(node.Subtrees);
      while (node.SubtreeCount > 0) node.RemoveSubtree(0);
      foreach (var subtree in subtrees) {
        node.AddSubtree(AddLagToDynamicNodes(subtree, lag));
      }
      return node;
    }

    private static bool AreSameTypeAndVariable(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      return GroupId((IVariableTreeNode)a) == GroupId((IVariableTreeNode)b);
    }

    // helper to combine the constant factors in products and to combine variables (powers of 2, 3...)
    private static void MergeVariablesAndConstantsInProduct(ISymbolicExpressionTreeNode prod) {
      var subtrees = new List<ISymbolicExpressionTreeNode>(prod.Subtrees);
      while (prod.Subtrees.Any()) prod.RemoveSubtree(0);
      var groupedVarNodes = from node in subtrees.OfType<IVariableTreeNode>()
                            where node.SubtreeCount == 0
                            group node by GroupId(node) into g
                            orderby g.Count()
                            select g;
      var constantProduct = (from node in subtrees.OfType<VariableTreeNodeBase>()
                             select node.Weight)
        .Concat(from node in subtrees.OfType<ConstantTreeNode>()
                select node.Value)
        .DefaultIfEmpty(1.0)
        .Aggregate((c1, c2) => c1 * c2);

      var unchangedSubtrees = from tree in subtrees
                              where tree.SubtreeCount > 0 || !(tree is IVariableTreeNode) && !(tree is ConstantTreeNode)
                              select tree;

      foreach (var variableNodeGroup in groupedVarNodes) {
        var firstNode = variableNodeGroup.First();
        if (firstNode is VariableTreeNodeBase) {
          var representative = (VariableTreeNodeBase)firstNode;
          representative.Weight = 1.0;
          if (variableNodeGroup.Count() > 1) {
            var poly = mulSymbol.CreateTreeNode();
            for (int p = 0; p < variableNodeGroup.Count(); p++) {
              poly.AddSubtree((ISymbolicExpressionTreeNode)representative.Clone());
            }
            prod.AddSubtree(poly);
          } else {
            prod.AddSubtree(representative);
          }
        } else if (firstNode is FactorVariableTreeNode) {
          var representative = (FactorVariableTreeNode)firstNode;
          foreach (var node in variableNodeGroup.Skip(1).Cast<FactorVariableTreeNode>()) {
            for (int j = 0; j < representative.Weights.Length; j++) {
              representative.Weights[j] *= node.Weights[j];
            }
          }
          for (int j = 0; j < representative.Weights.Length; j++) {
            representative.Weights[j] *= constantProduct;
          }
          constantProduct = 1.0;
          // if the product already contains a factor it is not necessary to multiply a constant below
          prod.AddSubtree(representative);
        }
      }

      foreach (var unchangedSubtree in unchangedSubtrees)
        prod.AddSubtree(unchangedSubtree);

      if (!constantProduct.IsAlmost(1.0)) {
        prod.AddSubtree(MakeConstant(constantProduct));
      }
    }


    /// <summary>
    /// x => x * -1
    /// Is only used in cases where it is not necessary to create new tree nodes. Manipulates x directly.
    /// </summary>
    /// <param name="x"></param>
    /// <returns>-x</returns>
    private static ISymbolicExpressionTreeNode Negate(ISymbolicExpressionTreeNode x) {
      if (IsConstant(x)) {
        ((ConstantTreeNode)x).Value *= -1;
      } else if (IsVariableBase(x)) {
        var variableTree = (VariableTreeNodeBase)x;
        variableTree.Weight *= -1.0;
      } else if (IsFactor(x)) {
        var factorNode = (FactorVariableTreeNode)x;
        for (int i = 0; i < factorNode.Weights.Length; i++) factorNode.Weights[i] *= -1;
      } else if (IsBinFactor(x)) {
        var factorNode = (BinaryFactorVariableTreeNode)x;
        factorNode.Weight *= -1;
      } else if (IsAddition(x)) {
        // (x0 + x1 + .. + xn) * -1 => (-x0 + -x1 + .. + -xn)        
        var subtrees = new List<ISymbolicExpressionTreeNode>(x.Subtrees);
        while (x.Subtrees.Any()) x.RemoveSubtree(0);
        foreach (var subtree in subtrees) {
          x.AddSubtree(Negate(subtree));
        }
      } else if (IsMultiplication(x) || IsDivision(x)) {
        // x0 * x1 * .. * xn * -1 => x0 * x1 * .. * -xn
        var lastSubTree = x.Subtrees.Last();
        x.RemoveSubtree(x.SubtreeCount - 1);
        x.AddSubtree(Negate(lastSubTree)); // last is maybe a constant, prefer to negate the constant
      } else {
        // any other function
        return MakeProduct(x, MakeConstant(-1));
      }
      return x;
    }

    /// <summary>
    /// x => 1/x
    /// Must create new tree nodes
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static ISymbolicExpressionTreeNode Invert(ISymbolicExpressionTreeNode x) {
      if (IsConstant(x)) {
        return MakeConstant(1.0 / ((ConstantTreeNode)x).Value);
      } else if (IsFactor(x)) {
        var factorNode = (FactorVariableTreeNode)x;
        return MakeFactor(factorNode.Symbol, factorNode.VariableName, factorNode.Weights.Select(w => 1.0 / w));
      } else if (IsDivision(x)) {
        return MakeFraction(x.GetSubtree(1), x.GetSubtree(0));
      } else {
        // any other function
        return MakeFraction(MakeConstant(1), x);
      }
    }

    private static ISymbolicExpressionTreeNode MakeConstant(double value) {
      ConstantTreeNode constantTreeNode = (ConstantTreeNode)(constSymbol.CreateTreeNode());
      constantTreeNode.Value = value;
      return constantTreeNode;
    }

    private static ISymbolicExpressionTreeNode MakeFactor(FactorVariable sy, string variableName, IEnumerable<double> weights) {
      var tree = (FactorVariableTreeNode)sy.CreateTreeNode();
      tree.VariableName = variableName;
      tree.Weights = weights.ToArray();
      return tree;
    }
    private static ISymbolicExpressionTreeNode MakeBinFactor(BinaryFactorVariable sy, string variableName, string variableValue, double weight) {
      var tree = (BinaryFactorVariableTreeNode)sy.CreateTreeNode();
      tree.VariableName = variableName;
      tree.VariableValue = variableValue;
      tree.Weight = weight;
      return tree;
    }


    #endregion
  }
}
