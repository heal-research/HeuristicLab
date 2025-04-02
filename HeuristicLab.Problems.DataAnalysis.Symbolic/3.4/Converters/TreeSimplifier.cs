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
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Simplifier for symbolic expressions
  /// </summary>
  public class TreeSimplifier {
    private static readonly Addition AddSymbol = new Addition();
    private static readonly Multiplication MulSymbol = new Multiplication();
    private static readonly Division DivSymbol = new Division();
    private static readonly Number NumberSymbol = new Number();
    private static readonly Absolute AbsSymbol = new Absolute();
    private static readonly Logarithm LogSymbol = new Logarithm();
    private static readonly Exponential ExpSymbol = new Exponential();
    private static readonly Root RootSymbol = new Root();
    private static readonly Square SqrSymbol = new Square();
    private static readonly SquareRoot SqrtSymbol = new SquareRoot();
    private static readonly AnalyticQuotient AqSymbol = new AnalyticQuotient();
    private static readonly Cube CubeSymbol = new Cube();
    private static readonly CubeRoot CubeRootSymbol = new CubeRoot();
    private static readonly Power PowSymbol = new Power();
    private static readonly Sine SineSymbol = new Sine();
    private static readonly Cosine CosineSymbol = new Cosine();
    private static readonly Tangent TanSymbol = new Tangent();
    private static readonly IfThenElse IfThenElseSymbol = new IfThenElse();
    private static readonly And AndSymbol = new And();
    private static readonly Or OrSymbol = new Or();
    private static readonly Not NotSymbol = new Not();
    private static readonly GreaterThan GtSymbol = new GreaterThan();
    private static readonly LessThan LtSymbol = new LessThan();
    private static readonly Integral IntegralSymbol = new Integral();
    private static readonly LaggedVariable LaggedVariableSymbol = new LaggedVariable();
    private static readonly TimeLag TimeLagSymbol = new TimeLag();

    [Obsolete("Use static method TreeSimplifier.Simplify instead")]
    public TreeSimplifier() { }

    public static ISymbolicExpressionTree Simplify(ISymbolicExpressionTree originalTree) {
      var clone = (ISymbolicExpressionTreeNode)originalTree.Root.Clone();
      // macro expand (initially no argument trees)
      var macroExpandedTree = MacroExpand(clone, clone.GetSubtree(0), new List<ISymbolicExpressionTreeNode>());
      var rootNode = (new ProgramRootSymbol()).CreateTreeNode();
      rootNode.AddSubtree(GetSimplifiedTree(macroExpandedTree));

#if DEBUG
      // check that each node is only referenced once
      var nodes = rootNode.IterateNodesPrefix().ToArray();
      if (nodes.Any(n => nodes.Count(ni => ni == n) > 1)) {
        throw new InvalidOperationException();
      }
#endif
      return new SymbolicExpressionTree(rootNode);
    }

    // the argumentTrees list contains already expanded trees used as arguments for invocations
    private static ISymbolicExpressionTreeNode MacroExpand(ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode node,
      IList<ISymbolicExpressionTreeNode> argumentTrees) {
      var subtrees = new List<ISymbolicExpressionTreeNode>(node.Subtrees);
      while (node.SubtreeCount > 0) node.RemoveSubtree(0);
      switch (node.Symbol) {
        case InvokeFunction invokeSym: {
          var defunNode = FindFunctionDefinition(root, invokeSym.FunctionName);
          var macroExpandedArguments = subtrees.Select(subtree => MacroExpand(root, subtree, argumentTrees)).ToList();
          return MacroExpand(root, defunNode, macroExpandedArguments);
        }
        case Argument argSym:
          // return the correct argument sub-tree (already macro-expanded)
          return (SymbolicExpressionTreeNode)argumentTrees[argSym.ArgumentIndex].Clone();
      }

      // recursive application
      foreach (var subtree in subtrees) {
        node.AddSubtree(MacroExpand(root, subtree, argumentTrees));
      }
      return node;
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
      return node.Symbol is AnalyticQuotient;
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

    private static bool IsVariableBase(ISymbolicExpressionTreeNode node) {
      return node is VariableTreeNodeBase;
    }

    private static bool IsFactor(ISymbolicExpressionTreeNode node) {
      return node is FactorVariableTreeNode;
    }

    private static bool IsBinFactor(ISymbolicExpressionTreeNode node) {
      return node is BinaryFactorVariableTreeNode;
    }

    private static bool IsNumber(ISymbolicExpressionTreeNode node) {
      return node.Symbol is Number;
    }

    private static bool IsConstantOrNumber(ISymbolicExpressionTreeNode node) {
      return node is INumericTreeNode;
    }

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
      if (IsConstantOrNumber(original) || IsVariableBase(original)) {
        return (ISymbolicExpressionTreeNode)original.Clone();
      }

      if (IsAbsolute(original)) {
        return SimplifyAbsolute(original);
      }

      if (IsAddition(original)) {
        return SimplifyAddition(original);
      }

      if (IsSubtraction(original)) {
        return SimplifySubtraction(original);
      }

      if (IsMultiplication(original)) {
        return SimplifyMultiplication(original);
      }

      if (IsDivision(original)) {
        return SimplifyDivision(original);
      }

      if (IsAverage(original)) {
        return SimplifyAverage(original);
      }

      if (IsLog(original)) {
        return SimplifyLog(original);
      }

      if (IsExp(original)) {
        return SimplifyExp(original);
      }

      if (IsSquare(original)) {
        return SimplifySquare(original);
      }

      if (IsSquareRoot(original)) {
        return SimplifySquareRoot(original);
      }

      if (IsCube(original)) {
        return SimplifyCube(original);
      }

      if (IsCubeRoot(original)) {
        return SimplifyCubeRoot(original);
      }

      if (IsPower(original)) {
        return SimplifyPower(original);
      }

      if (IsRoot(original)) {
        return SimplifyRoot(original);
      }

      if (IsSine(original)) {
        return SimplifySine(original);
      }

      if (IsCosine(original)) {
        return SimplifyCosine(original);
      }

      if (IsTangent(original)) {
        return SimplifyTangent(original);
      }

      if (IsAnalyticalQuotient(original)) {
        return SimplifyAnalyticalQuotient(original);
      }

      if (IsIfThenElse(original)) {
        return SimplifyIfThenElse(original);
      }

      if (IsGreaterThan(original)) {
        return SimplifyGreaterThan(original);
      }

      if (IsLessThan(original)) {
        return SimplifyLessThan(original);
      }

      if (IsAnd(original)) {
        return SimplifyAnd(original);
      }

      if (IsOr(original)) {
        return SimplifyOr(original);
      }

      if (IsNot(original)) {
        return SimplifyNot(original);
      }

      if (IsTimeLag(original)) {
        return SimplifyTimeLag(original);
      }

      if (IsIntegral(original)) {
        return SimplifyIntegral(original);
      }

      return SimplifyAny(original);
    }

    #region specific simplification routines

    private static ISymbolicExpressionTreeNode SimplifyAny(ISymbolicExpressionTreeNode original) {
      // can't simplify this function but simplify all subtrees 
      var subtrees = new List<ISymbolicExpressionTreeNode>(original.Subtrees);
      while (original.Subtrees.Count() > 0) original.RemoveSubtree(0);
      var clone = (SymbolicExpressionTreeNode)original.Clone();
      var simplifiedSubtrees = new List<ISymbolicExpressionTreeNode>();
      foreach (var subtree in subtrees) {
        simplifiedSubtrees.Add(GetSimplifiedTree(subtree));
        original.AddSubtree(subtree);
      }
      foreach (var simplifiedSubtree in simplifiedSubtrees) {
        clone.AddSubtree(simplifiedSubtree);
      }
      if (simplifiedSubtrees.TrueForAll(IsNumber)) {
        FoldNumbers(clone);
      }
      return clone;
    }

    private static void FoldNumbers(ISymbolicExpressionTreeNode original) {
      // TODO not implemented
    }

    private static ISymbolicExpressionTreeNode SimplifyAverage(ISymbolicExpressionTreeNode original) {
      if (original.Subtrees.Count() == 1) {
        return GetSimplifiedTree(original.GetSubtree(0));
      }

      // simplify expressions x0..xn
      // make sum(x0..xn) / n
      var sum = original.Subtrees
                        .Select(GetSimplifiedTree)
                        .Aggregate(Sum);
      return Fraction(sum, Number(original.Subtrees.Count()));
    }

    private static ISymbolicExpressionTreeNode SimplifyDivision(ISymbolicExpressionTreeNode original) {
      if (original.Subtrees.Count() == 1) {
        return Invert(GetSimplifiedTree(original.GetSubtree(0)));
      }

      // simplify expressions x0..xn
      // make multiplication (x0 * 1/(x1 * x1 * .. * xn))
      var first = original.GetSubtree(0);
      var second = original.GetSubtree(1);
      var remaining = original.Subtrees.Skip(2);
      return
        Product(GetSimplifiedTree(first),
          Invert(remaining.Aggregate(GetSimplifiedTree(second), (a, b) => Product(a, GetSimplifiedTree(b)))));
    }

    private static ISymbolicExpressionTreeNode SimplifyMultiplication(ISymbolicExpressionTreeNode original) {
      if (original.Subtrees.Count() == 1) {
        return GetSimplifiedTree(original.GetSubtree(0));
      }

      return original.Subtrees
                     .Select(GetSimplifiedTree)
                     .Aggregate(Product);
    }

    private static ISymbolicExpressionTreeNode SimplifySubtraction(ISymbolicExpressionTreeNode original) {
      if (original.Subtrees.Count() == 1) {
        return Negate(GetSimplifiedTree(original.GetSubtree(0)));
      }

      // simplify expressions x0..xn
      // make addition (x0,-x1..-xn)
      var first = original.Subtrees.First();
      var remaining = original.Subtrees.Skip(1);
      return remaining.Aggregate(GetSimplifiedTree(first), (a, b) => Sum(a, Negate(GetSimplifiedTree(b))));
    }

    private static ISymbolicExpressionTreeNode SimplifyAddition(ISymbolicExpressionTreeNode original) {
      if (original.Subtrees.Count() == 1) {
        return GetSimplifiedTree(original.GetSubtree(0));
      }

      // simplify expression x0..xn
      // make addition (x0..xn)
      return original.Subtrees
                     .Select(GetSimplifiedTree)
                     .Aggregate(Sum);
    }

    private static ISymbolicExpressionTreeNode SimplifyAbsolute(ISymbolicExpressionTreeNode original) {
      return Abs(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyNot(ISymbolicExpressionTreeNode original) {
      return Not(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyOr(ISymbolicExpressionTreeNode original) {
      return original.Subtrees
        .Select(GetSimplifiedTree)
        .Aggregate(Or);
    }

    private static ISymbolicExpressionTreeNode SimplifyAnd(ISymbolicExpressionTreeNode original) {
      return original.Subtrees
        .Select(GetSimplifiedTree)
        .Aggregate(And);
    }

    private static ISymbolicExpressionTreeNode SimplifyLessThan(ISymbolicExpressionTreeNode original) {
      return LessThan(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)));
    }

    private static ISymbolicExpressionTreeNode SimplifyGreaterThan(ISymbolicExpressionTreeNode original) {
      return GreaterThan(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)));
    }

    private static ISymbolicExpressionTreeNode SimplifyIfThenElse(ISymbolicExpressionTreeNode original) {
      return IfThenElse(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)),
        GetSimplifiedTree(original.GetSubtree(2)));
    }

    private static ISymbolicExpressionTreeNode SimplifyTangent(ISymbolicExpressionTreeNode original) {
      return Tangent(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyCosine(ISymbolicExpressionTreeNode original) {
      return Cosine(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifySine(ISymbolicExpressionTreeNode original) {
      return Sine(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyExp(ISymbolicExpressionTreeNode original) {
      return Exp(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifySquare(ISymbolicExpressionTreeNode original) {
      return Square(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifySquareRoot(ISymbolicExpressionTreeNode original) {
      return SquareRoot(GetSimplifiedTree(original.GetSubtree(0)));
    }
    private static ISymbolicExpressionTreeNode SimplifyCube(ISymbolicExpressionTreeNode original) {
      return Cube(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyCubeRoot(ISymbolicExpressionTreeNode original) {
      return CubeRoot(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyLog(ISymbolicExpressionTreeNode original) {
      return Log(GetSimplifiedTree(original.GetSubtree(0)));
    }

    private static ISymbolicExpressionTreeNode SimplifyRoot(ISymbolicExpressionTreeNode original) {
      return Root(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)));
    }

    private static ISymbolicExpressionTreeNode SimplifyPower(ISymbolicExpressionTreeNode original) {
      return Power(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)));
    }

    private static ISymbolicExpressionTreeNode SimplifyAnalyticalQuotient(ISymbolicExpressionTreeNode original) {
      return Aq(GetSimplifiedTree(original.GetSubtree(0)), GetSimplifiedTree(original.GetSubtree(1)));
    }

    private static ISymbolicExpressionTreeNode SimplifyTimeLag(ISymbolicExpressionTreeNode original) {
      var laggedTreeNode = original as ILaggedTreeNode;
      var simplifiedSubtree = GetSimplifiedTree(original.GetSubtree(0));
      return !ContainsVariableCondition(simplifiedSubtree) ? AddLagToDynamicNodes(simplifiedSubtree, laggedTreeNode.Lag) : Lag(simplifiedSubtree, laggedTreeNode.Lag);
    }

    private static ISymbolicExpressionTreeNode SimplifyIntegral(ISymbolicExpressionTreeNode original) {
      var laggedTreeNode = original as ILaggedTreeNode;
      var simplifiedSubtree = GetSimplifiedTree(original.GetSubtree(0));
      return IsConstantOrNumber(simplifiedSubtree) ? GetSimplifiedTree(Product(simplifiedSubtree, Number(-laggedTreeNode.Lag))) : Integral(simplifiedSubtree, laggedTreeNode.Lag);
    }

    #endregion

    #region low level tree restructuring

    private static ISymbolicExpressionTreeNode Lag(ISymbolicExpressionTreeNode subtree, int lag) {
      if (lag == 0) return subtree;
      if (IsConstantOrNumber(subtree)) return subtree;
      var lagNode = (LaggedTreeNode)TimeLagSymbol.CreateTreeNode();
      lagNode.Lag = lag;
      lagNode.AddSubtree(subtree);
      return lagNode;
    }

    private static ISymbolicExpressionTreeNode Integral(ISymbolicExpressionTreeNode subtree, int lag) {
      switch (lag) {
        case 0:
          return subtree;
        case -1:
        case 1:
          return Sum(subtree, AddLagToDynamicNodes((ISymbolicExpressionTreeNode)subtree.Clone(), lag));
      }

      var node = (LaggedTreeNode)IntegralSymbol.CreateTreeNode();
      node.Lag = lag;
      node.AddSubtree(subtree);
      return node;
    }

    private static ISymbolicExpressionTreeNode Not(ISymbolicExpressionTreeNode t) {
      if (IsNumber(t)) {
        var numNode = t as NumberTreeNode;
        return numNode.Value > 0 ? Number(-1.0) : Number(1.0);
      }

      if (IsNot(t)) {
        return t.GetSubtree(0);
      }

      if (!IsBoolean(t)) {
        var gtNode = GtSymbol.CreateTreeNode();
        gtNode.AddSubtree(t);
        gtNode.AddSubtree(Number(0.0));
        var notNode = NotSymbol.CreateTreeNode();
        notNode.AddSubtree(gtNode);
        return notNode;
      } else {
        var notNode = NotSymbol.CreateTreeNode();
        notNode.AddSubtree(t);
        return notNode;
      }
    }

    private static ISymbolicExpressionTreeNode Or(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      while (true) {
        if (IsNumber(a) && IsNumber(b)) {
          var aNode = a as NumberTreeNode;
          var bNode = b as NumberTreeNode;
          if (aNode.Value > 0.0 || bNode.Value > 0.0) {
            return Number(1.0);
          }

          return Number(-1.0);
        }

        if (IsNumber(a)) {
          (a, b) = (b, a);
          continue;
        }

        if (IsNumber(b)) {
          var bNode = b as NumberTreeNode;
          if (bNode.Value > 0.0) {
            // boolean expression is necessarily true
            return Number(1.0);
          }

          // the value has no effect on the result of the boolean condition so we can drop the number
          var orNode = OrSymbol.CreateTreeNode();
          orNode.AddSubtree(a);
          return orNode;
        } else {
          var orNode = OrSymbol.CreateTreeNode();
          orNode.AddSubtree(a);
          orNode.AddSubtree(b);
          return orNode;
        }
      }
    }

    private static ISymbolicExpressionTreeNode And(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      while (true) {
        if (IsNumber(a) && IsNumber(b)) {
          var aNode = a as NumberTreeNode;
          var bNode = b as NumberTreeNode;
          if (aNode.Value > 0.0 && bNode.Value > 0.0) {
            return Number(1.0);
          }

          return Number(-1.0);
        }

        if (IsNumber(a)) {
          (a, b) = (b, a);
          continue;
        }

        if (IsNumber(b)) {
          var bNode = b as NumberTreeNode;
          if (!(bNode.Value > 0.0)) return Number(-1.0);
          // the value has no effect on the result of the boolean condition so we can drop the number
          var andNode = AndSymbol.CreateTreeNode();
          andNode.AddSubtree(a);
          return andNode;

          // boolean expression is necessarily false
        }

        {
          var andNode = AndSymbol.CreateTreeNode();
          andNode.AddSubtree(a);
          andNode.AddSubtree(b);
          return andNode;
        }
      }
    }

    private static ISymbolicExpressionTreeNode LessThan(ISymbolicExpressionTreeNode leftSide,
                                                        ISymbolicExpressionTreeNode rightSide) {
      if (IsNumber(leftSide) && IsNumber(rightSide)) {
        var lsNode = leftSide as NumberTreeNode;
        var rsNode = rightSide as NumberTreeNode;
        return lsNode.Value < rsNode.Value ? Number(1.0) : Number(-1.0);
      }

      var ltNode = LtSymbol.CreateTreeNode();
      ltNode.AddSubtree(leftSide);
      ltNode.AddSubtree(rightSide);
      return ltNode;
    }

    private static ISymbolicExpressionTreeNode GreaterThan(ISymbolicExpressionTreeNode leftSide,
      ISymbolicExpressionTreeNode rightSide) {
      if (IsNumber(leftSide) && IsNumber(rightSide)) {
        var lsNode = leftSide as NumberTreeNode;
        var rsNode = rightSide as NumberTreeNode;
        return lsNode.Value > rsNode.Value ? Number(1.0) : Number(-1.0);
      }

      var gtNode = GtSymbol.CreateTreeNode();
      gtNode.AddSubtree(leftSide);
      gtNode.AddSubtree(rightSide);
      return gtNode;
    }

    private static ISymbolicExpressionTreeNode IfThenElse(ISymbolicExpressionTreeNode condition,
      ISymbolicExpressionTreeNode trueBranch, ISymbolicExpressionTreeNode falseBranch) {
      if (IsConstantOrNumber(condition)) {
        var condNode = condition as INumericTreeNode;
        return condNode.Value > 0.0 ? trueBranch : falseBranch;
      }

      var ifNode = IfThenElseSymbol.CreateTreeNode();
      if (IsBoolean(condition)) {
        ifNode.AddSubtree(condition);
      } else {
        var gtNode = GtSymbol.CreateTreeNode();
        gtNode.AddSubtree(condition);
        gtNode.AddSubtree(Number(0.0));
        ifNode.AddSubtree(gtNode);
      }
      ifNode.AddSubtree(trueBranch);
      ifNode.AddSubtree(falseBranch);
      return ifNode;
    }

    private static ISymbolicExpressionTreeNode Sine(ISymbolicExpressionTreeNode node) {
      if (IsNumber(node)) {
        var numNode = node as NumberTreeNode;
        return Number(Math.Sin(numNode.Value));
      }

      if (IsFactor(node)) {
        var factor = node as FactorVariableTreeNode;
        return Factor(factor.Symbol, factor.VariableName, factor.Weights.Select(Math.Sin));
      }

      if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Sin(binFactor.Weight));
      }

      var sineNode = SineSymbol.CreateTreeNode();
      sineNode.AddSubtree(node);
      return sineNode;
    }

    private static ISymbolicExpressionTreeNode Tangent(ISymbolicExpressionTreeNode node) {
      if (IsNumber(node)) {
        var numNode = node as NumberTreeNode;
        return Number(Math.Tan(numNode.Value));
      }

      if (IsFactor(node)) {
        var factor = node as FactorVariableTreeNode;
        return Factor(factor.Symbol, factor.VariableName, factor.Weights.Select(Math.Tan));
      }

      if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Tan(binFactor.Weight));
      }

      var tanNode = TanSymbol.CreateTreeNode();
      tanNode.AddSubtree(node);
      return tanNode;
    }

    private static ISymbolicExpressionTreeNode Cosine(ISymbolicExpressionTreeNode node) {
      if (IsNumber(node)) {
        var numNode = node as NumberTreeNode;
        return Number(Math.Cos(numNode.Value));
      }

      if (IsFactor(node)) {
        var factor = node as FactorVariableTreeNode;
        return Factor(factor.Symbol, factor.VariableName, factor.Weights.Select(Math.Cos));
      }

      if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        // cos(0) = 1 see similar case for Exp(binfactor)
        return Sum(BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Cos(binFactor.Weight) - 1),
          Number(1.0));
      }

      var cosNode = CosineSymbol.CreateTreeNode();
      cosNode.AddSubtree(node);
      return cosNode;
    }

    private static ISymbolicExpressionTreeNode Exp(ISymbolicExpressionTreeNode node) {
      if (IsNumber(node)) {
        var numNode = node as NumberTreeNode;
        return Number(Math.Exp(numNode.Value));
      }

      if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Exp(w)));
      }

      if (IsBinFactor(node)) {
        // exp( binfactor w val=a) = if(val=a) exp(w) else exp(0) = binfactor( (exp(w) - 1) val a) + 1
        var binFactor = node as BinaryFactorVariableTreeNode;
        return
          Sum(BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Exp(binFactor.Weight) - 1), Number(1.0));
      }

      if (IsLog(node)) {
        return node.GetSubtree(0);
      }

      if (IsAddition(node)) {
        return node.Subtrees.Select(Exp).Aggregate((s, t) => Product(s, t));
      }

      if (IsSubtraction(node)) {
        return node.Subtrees.Select(Exp).Aggregate((s, t) => Product(s, Negate(t)));
      }

      var expNode = ExpSymbol.CreateTreeNode();
      expNode.AddSubtree(node);
      return expNode;
    }
    private static ISymbolicExpressionTreeNode Log(ISymbolicExpressionTreeNode node) {
      if (IsNumber(node)) {
        var numNode = node as NumberTreeNode;
        return Number(Math.Log(numNode.Value));
      }

      if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Log(w)));
      }

      if (IsExp(node)) {
        return node.GetSubtree(0);
      }

      if (IsSquareRoot(node)) {
        return Fraction(Log(node.GetSubtree(0)), Number(2.0));
      }

      var logNode = LogSymbol.CreateTreeNode();
      logNode.AddSubtree(node);
      return logNode;
    }

    private static ISymbolicExpressionTreeNode Square(ISymbolicExpressionTreeNode node) {
      while (true) {
        if (IsNumber(node)) {
          var numNode = node as NumberTreeNode;
          return Number(numNode.Value * numNode.Value);
        }

        if (IsFactor(node)) {
          var factNode = node as FactorVariableTreeNode;
          return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => w * w));
        }

        if (IsBinFactor(node)) {
          var binFactor = node as BinaryFactorVariableTreeNode;
          return BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, binFactor.Weight * binFactor.Weight);
        }

        if (IsSquareRoot(node)) {
          return node.GetSubtree(0);
        }

        if (IsMultiplication(node)) {
          // sqr( x * y ) = sqr(x) * sqr(y)
          var mulNode = MulSymbol.CreateTreeNode();
          foreach (var subtree in node.Subtrees) {
            mulNode.AddSubtree(Square(subtree));
          }

          return mulNode;
        }

        if (IsAbsolute(node)) {
          node = node.GetSubtree(0);
          continue;
        }

        if (IsExp(node)) {
          return Exp(Product(node.GetSubtree(0), Number(2.0))); // sqr(exp(x)) = exp(2x)
        }

        if (IsSquare(node)) {
          return Power(node.GetSubtree(0), Number(4));
        }

        if (IsCube(node)) {
          return Power(node.GetSubtree(0), Number(6));
        }

        var sqrNode = SqrSymbol.CreateTreeNode();
        sqrNode.AddSubtree(node);
        return sqrNode;
        break;
      }
    }

    private static ISymbolicExpressionTreeNode Cube(ISymbolicExpressionTreeNode node) {
      if (IsNumber(node)) {
        var numNode = node as NumberTreeNode;
        return Number(numNode.Value * numNode.Value * numNode.Value);
      }

      if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => w * w * w));
      }

      if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, binFactor.Weight * binFactor.Weight * binFactor.Weight);
      }

      if (IsCubeRoot(node)) {
        return node.GetSubtree(0); // NOTE: not really accurate because cuberoot(x) with negative x is evaluated to NaN and after this simplification we evaluate as x
      }

      if (IsExp(node)) {
        return Exp(Product(node.GetSubtree(0), Number(3)));
      }

      if (IsSquare(node)) {
        return Power(node.GetSubtree(0), Number(6));
      }

      if (IsCube(node)) {
        return Power(node.GetSubtree(0), Number(9));
      }

      var cubeNode = CubeSymbol.CreateTreeNode();
      cubeNode.AddSubtree(node);
      return cubeNode;
    }

    private static ISymbolicExpressionTreeNode Abs(ISymbolicExpressionTreeNode node) {
      if (IsNumber(node)) {
        var numNode = node as NumberTreeNode;
        return Number(Math.Abs(numNode.Value));
      }

      if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Abs(w)));
      }

      if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Abs(binFactor.Weight));
      }

      if (IsSquare(node) || IsExp(node) || IsSquareRoot(node) || IsCubeRoot(node)) {
        return node; // abs(sqr(x)) = sqr(x), abs(exp(x)) = exp(x) ...
      }

      if (IsMultiplication(node)) {
        var mul = MulSymbol.CreateTreeNode();
        foreach (var st in node.Subtrees) {
          mul.AddSubtree(Abs(st));
        }
        return mul;
      }

      if (IsDivision(node)) {
        var div = DivSymbol.CreateTreeNode();
        foreach (var st in node.Subtrees) {
          div.AddSubtree(Abs(st));
        }
        return div;
      }

      var absNode = AbsSymbol.CreateTreeNode();
      absNode.AddSubtree(node);
      return absNode;
    }

    // constant folding only
    private static ISymbolicExpressionTreeNode Aq(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (IsNumber(b)) {
        var nNode = b as NumberTreeNode;
        return Fraction(a, Number(Math.Sqrt(1.0 + nNode.Value * nNode.Value)));
      }

      if (IsFactor(b)) {
        var factNode = b as FactorVariableTreeNode;
        return Fraction(a, Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Sqrt(1.0 + w * w))));
      }

      if (IsBinFactor(b)) {
        var binFactor = b as BinaryFactorVariableTreeNode;
        return Fraction(a, BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Sqrt(1.0 + binFactor.Weight * binFactor.Weight)));
      }

      var aqNode = AqSymbol.CreateTreeNode();
      aqNode.AddSubtree(a);
      aqNode.AddSubtree(b);
      return aqNode;
    }

    private static ISymbolicExpressionTreeNode SquareRoot(ISymbolicExpressionTreeNode node) {
      if (IsNumber(node)) {
        var numNode = node as NumberTreeNode;
        return Number(Math.Sqrt(numNode.Value));
      }

      if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Sqrt(w)));
      }

      if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Sqrt(binFactor.Weight));
      }

      if (IsSquare(node)) {
        return node.GetSubtree(0); // NOTE: not really accurate because sqrt(x) with negative x is evaluated to NaN and after this simplification we evaluate as x
      }

      var sqrtNode = SqrtSymbol.CreateTreeNode();
      sqrtNode.AddSubtree(node);
      return sqrtNode;
    }

    private static ISymbolicExpressionTreeNode CubeRoot(ISymbolicExpressionTreeNode node) {
      if (IsNumber(node)) {
        var numNode = node as NumberTreeNode;
        return Number(Math.Pow(numNode.Value, 1.0 / 3.0));
      }

      if (IsFactor(node)) {
        var factNode = node as FactorVariableTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Pow(w, 1.0 / 3.0)));
      }

      if (IsBinFactor(node)) {
        var binFactor = node as BinaryFactorVariableTreeNode;
        return BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Sqrt(Math.Pow(binFactor.Weight, 1.0 / 3.0)));
      }

      if (IsCube(node)) {
        return node.GetSubtree(0);
      }

      var cubeRootNode = CubeRootSymbol.CreateTreeNode();
      cubeRootNode.AddSubtree(node);
      return cubeRootNode;
    }

    private static ISymbolicExpressionTreeNode Root(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (IsNumber(a) && IsNumber(b)) {
        var aNode = a as NumberTreeNode;
        var bNode = b as NumberTreeNode;
        return Number(Math.Pow(aNode.Value, 1.0 / Math.Round(bNode.Value)));
      }

      if (IsFactor(a) && IsNumber(b)) {
        var factNode = a as FactorVariableTreeNode;
        var bNode = b as NumberTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName,
          factNode.Weights.Select(w => Math.Pow(w, 1.0 / Math.Round(bNode.Value))));
      }

      if (IsBinFactor(a) && IsNumber(b)) {
        var binFactor = a as BinaryFactorVariableTreeNode;
        var bNode = b as NumberTreeNode;
        return BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Pow(binFactor.Weight, 1.0 / Math.Round(bNode.Value)));
      }

      if (IsNumber(a) && IsFactor(b)) {
        var aNode = a as NumberTreeNode;
        var factNode = b as FactorVariableTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Pow(aNode.Value, 1.0 / Math.Round(w))));
      }

      if (IsNumber(a) && IsBinFactor(b)) {
        var aNode = a as NumberTreeNode;
        var factNode = b as BinaryFactorVariableTreeNode;
        return BinFactor(factNode.Symbol, factNode.VariableName, factNode.VariableValue, Math.Pow(aNode.Value, 1.0 / Math.Round(factNode.Weight)));
      }

      if (IsFactor(a) && IsFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as FactorVariableTreeNode;
        return Factor(node0.Symbol, node0.VariableName, node0.Weights.Zip(node1.Weights, (u, v) => Math.Pow(u, 1.0 / Math.Round(v))));
      }

      if (IsNumber(b)) {
        var bNode = b as NumberTreeNode;
        var bVal = Math.Round(bNode.Value);
        switch (bVal) {
          case 1.0:
            // root(a, 1) => a
            return a;
          case 0.0:
            // root(a, 0) is not defined 
            return Number(double.NaN);
          case -1.0:
            // root(a, -1) => a^(-1/1) => 1/a
            return Fraction(Number(1.0), a);
        }

        if (bVal < 0) {
          // root(a, -b) => a^(-1/b) => (1/a)^(1/b) => root(1, b) / root(a, b) => 1 / root(a, b)
          var rootNode = RootSymbol.CreateTreeNode();
          rootNode.AddSubtree(a);
          rootNode.AddSubtree(Number(-1.0 * bVal));
          return Fraction(Number(1.0), rootNode);
        } else {
          var rootNode = RootSymbol.CreateTreeNode();
          rootNode.AddSubtree(a);
          rootNode.AddSubtree(Number(bVal));
          return rootNode;
        }
      }

      {
        var rootNode = RootSymbol.CreateTreeNode();
        rootNode.AddSubtree(a);
        rootNode.AddSubtree(b);
        return rootNode;
      }
    }


    private static ISymbolicExpressionTreeNode Power(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (IsNumber(a) && IsNumber(b)) {
        var aNode = a as NumberTreeNode;
        var bNode = b as NumberTreeNode;
        return Number(Math.Pow(aNode.Value, Math.Round(bNode.Value)));
      }

      if (IsFactor(a) && IsNumber(b)) {
        var factNode = a as FactorVariableTreeNode;
        var bNode = b as NumberTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Pow(w, Math.Round(bNode.Value))));
      }

      if (IsBinFactor(a) && IsNumber(b)) {
        var binFactor = a as BinaryFactorVariableTreeNode;
        var bNode = b as NumberTreeNode;
        return BinFactor(binFactor.Symbol, binFactor.VariableName, binFactor.VariableValue, Math.Pow(binFactor.Weight, Math.Round(bNode.Value)));
      }

      if (IsNumber(a) && IsFactor(b)) {
        var aNode = a as NumberTreeNode;
        var factNode = b as FactorVariableTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => Math.Pow(aNode.Value, Math.Round(w))));
      }

      if (IsNumber(a) && IsBinFactor(b)) {
        var aNode = a as NumberTreeNode;
        var factNode = b as BinaryFactorVariableTreeNode;
        return BinFactor(factNode.Symbol, factNode.VariableName, factNode.VariableValue, Math.Pow(aNode.Value, Math.Round(factNode.Weight)));
      }

      if (IsFactor(a) && IsFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as FactorVariableTreeNode;
        return Factor(node0.Symbol, node0.VariableName, node0.Weights.Zip(node1.Weights, (u, v) => Math.Pow(u, Math.Round(v))));
      }

      if (IsNumber(b)) {
        var bNode = b as NumberTreeNode;
        var exponent = Math.Round(bNode.Value);
        switch (exponent) {
          case 0.0:
            // a^0 => 1
            return Number(1.0);
          case 1.0:
            // a^1 => a
            return a;
          case -1.0:
            // a^-1 => 1/a
            return Fraction(Number(1.0), a);
        }

        if (exponent < 0) {
          // a^-b => (1/a)^b => 1/(a^b)
          var powNode = PowSymbol.CreateTreeNode();
          powNode.AddSubtree(a);
          powNode.AddSubtree(Number(-1.0 * exponent));
          return Fraction(Number(1.0), powNode);
        } else {
          var powNode = PowSymbol.CreateTreeNode();
          powNode.AddSubtree(a);
          powNode.AddSubtree(Number(exponent));
          return powNode;
        }
      }

      {
        var powNode = PowSymbol.CreateTreeNode();
        powNode.AddSubtree(a);
        powNode.AddSubtree(b);
        return powNode;
      }
    }


    // Fraction, Product and Sum take two already simplified trees and create a new simplified tree
    private static ISymbolicExpressionTreeNode Fraction(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (IsNumber(a) && IsNumber(b)) {
        // fold constants
        return Number(((NumberTreeNode)a).Value / ((NumberTreeNode)b).Value);
      }

      if (IsNumber(a) && ((NumberTreeNode)a).Value != 1.0) {
        // a / x => (a * 1/a) / (x * 1/a) => 1 / (x * 1/a)
        return Fraction(Number(1.0), Product(b, Invert(a)));
      }

      if (IsVariableBase(a) && IsNumber(b)) {
        // merge constant values into variable weights
        var bVal = ((NumberTreeNode)b).Value;
        ((VariableTreeNodeBase)a).Weight /= bVal;
        return a;
      }

      if (IsFactor(a) && IsNumber(b)) {
        var factNode = a as FactorVariableTreeNode;
        var bNode = b as NumberTreeNode;
        return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select(w => w / bNode.Value));
      }

      if (IsBinFactor(a) && IsNumber(b)) {
        var factNode = a as BinaryFactorVariableTreeNode;
        var bNode = b as NumberTreeNode;
        return BinFactor(factNode.Symbol, factNode.VariableName, factNode.VariableValue, factNode.Weight / bNode.Value);
      }

      if (IsFactor(a) && IsFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as FactorVariableTreeNode;
        return Factor(node0.Symbol, node0.VariableName, node0.Weights.Zip(node1.Weights, (u, v) => u / v));
      }

      if (IsFactor(a) && IsBinFactor(b) && ((IVariableTreeNode)a).VariableName == ((IVariableTreeNode)b).VariableName) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as BinaryFactorVariableTreeNode;
        var varValues = node0.Symbol.GetVariableValues(node0.VariableName).ToArray();
        var wi = Array.IndexOf(varValues, node1.VariableValue);
        if (wi < 0) throw new ArgumentException();
        var newWeighs = new double[varValues.Length];
        node0.Weights.CopyTo(newWeighs, 0);
        for (var i = 0; i < newWeighs.Length; i++)
          if (wi == i) newWeighs[i] /= node1.Weight;
          else newWeighs[i] /= 0.0;
        return Factor(node0.Symbol, node0.VariableName, newWeighs);
      }

      if (IsFactor(a)) {
        return Fraction(Number(1.0), Product(b, Invert(a)));
      }

      if (IsVariableBase(a) && IsVariableBase(b) && AreSameTypeAndVariable(a, b) && !IsBinFactor(b)) {
        // cancel variables (not allowed for bin factors because of division by zero)
        var aVar = a as VariableTreeNode;
        var bVar = b as VariableTreeNode;
        return Number(aVar.Weight / bVar.Weight);
      }

      if (IsAddition(a) && IsNumber(b)) {
        return a.Subtrees
                .Select(GetSimplifiedTree)
                .Select(x => Fraction(x, GetSimplifiedTree(b)))
                .Aggregate(Sum);
      }

      if (IsMultiplication(a) && IsNumber(b)) {
        return Product(a, Invert(b));
      }

      if (IsDivision(a) && IsNumber(b)) {
        // (a1 / a2) / c => (a1 / (a2 * c))
        return Fraction(a.GetSubtree(0), Product(a.GetSubtree(1), b));
      }

      if (IsDivision(a) && IsDivision(b)) {
        // (a1 / a2) / (b1 / b2) => (a1*b2)/(a2*b1)
        return Fraction(Product(a.GetSubtree(0), b.GetSubtree(1)), Product(a.GetSubtree(1), b.GetSubtree(0)));
      }

      if (IsDivision(a)) {
        // (a1 / a2) / b => (a1 / (a2 * b))
        return Fraction(a.GetSubtree(0), Product(a.GetSubtree(1), b));
      }

      if (IsDivision(b)) {
        // a / (b1 / b2) => (a * b2) / b1
        return Fraction(Product(a, b.GetSubtree(1)), b.GetSubtree(0));
      }

      if (IsAnalyticalQuotient(a)) {
        return Aq(a.GetSubtree(0), Product(a.GetSubtree(1), b));
      }

      var div = DivSymbol.CreateTreeNode();
      div.AddSubtree(a);
      div.AddSubtree(b);
      return div;
    }

    private static ISymbolicExpressionTreeNode Sum(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      while (true) {
        if (IsNumber(a) && IsNumber(b)) {
          // fold constants
          ((NumberTreeNode)a).Value += ((NumberTreeNode)b).Value;
          return a;
        }

        if (IsNumber(a)) {
          // c + x => x + c
          // b is not constant => make sure constant is on the right
          (a, b) = (b, a);
          continue;
        }

        if (IsNumber(b) && ((NumberTreeNode)b).Value == 0.0) {
          // x + 0 => x
          return a;
        }

        if (IsFactor(a) && IsNumber(b)) {
          var factNode = a as FactorVariableTreeNode;
          var bNode = b as NumberTreeNode;
          return Factor(factNode.Symbol, factNode.VariableName, factNode.Weights.Select((w) => w + bNode.Value));
        }

        if (IsFactor(a) && IsFactor(b) && AreSameTypeAndVariable(a, b)) {
          var node0 = a as FactorVariableTreeNode;
          var node1 = b as FactorVariableTreeNode;
          return Factor(node0.Symbol, node0.VariableName, node0.Weights.Zip(node1.Weights, (u, v) => u + v));
        }

        if (IsBinFactor(a) && IsFactor(b)) {
          (a, b) = (b, a);
          continue;
        }

        if (IsFactor(a) && IsBinFactor(b) && ((IVariableTreeNode)a).VariableName == ((IVariableTreeNode)b).VariableName) {
          var node0 = a as FactorVariableTreeNode;
          var node1 = b as BinaryFactorVariableTreeNode;
          var varValues = node0.Symbol.GetVariableValues(node0.VariableName).ToArray();
          var wi = Array.IndexOf(varValues, node1.VariableValue);
          if (wi < 0) throw new ArgumentException();
          var newWeighs = new double[varValues.Length];
          node0.Weights.CopyTo(newWeighs, 0);
          newWeighs[wi] += node1.Weight;
          return Factor(node0.Symbol, node0.VariableName, newWeighs);
        }

        if (IsAddition(a) && IsAddition(b)) {
          // merge additions
          var add = AddSymbol.CreateTreeNode();
          // add all sub trees except for the last
          for (var i = 0; i < a.Subtrees.Count() - 1; i++) add.AddSubtree(a.GetSubtree(i));
          for (var i = 0; i < b.Subtrees.Count() - 1; i++) add.AddSubtree(b.GetSubtree(i));
          if (IsNumber(a.Subtrees.Last()) && IsNumber(b.Subtrees.Last())) {
            add.AddSubtree(Sum(a.Subtrees.Last(), b.Subtrees.Last()));
          } else if (IsNumber(a.Subtrees.Last())) {
            add.AddSubtree(b.Subtrees.Last());
            add.AddSubtree(a.Subtrees.Last());
          } else {
            add.AddSubtree(a.Subtrees.Last());
            add.AddSubtree(b.Subtrees.Last());
          }

          MergeVariablesInSum(add);
          return add.Subtrees.Count() == 1 ? add.GetSubtree(0) : add;
        }

        if (IsAddition(b)) {
          (a, b) = (b, a);
          continue;
        }

        if (IsAddition(a) && IsNumber(b)) {
          // a is an addition and b is a constant => append b to a and make sure the constants are merged
          var add = AddSymbol.CreateTreeNode();
          // add all sub trees except for the last
          for (var i = 0; i < a.Subtrees.Count() - 1; i++) add.AddSubtree(a.GetSubtree(i));
          if (IsNumber(a.Subtrees.Last()))
            add.AddSubtree(Sum(a.Subtrees.Last(), b));
          else {
            add.AddSubtree(a.Subtrees.Last());
            add.AddSubtree(b);
          }

          return add;
        }

        if (IsAddition(a)) {
          // a is already an addition => append b
          var add = AddSymbol.CreateTreeNode();
          add.AddSubtree(b);
          foreach (var subtree in a.Subtrees) {
            add.AddSubtree(subtree);
          }

          MergeVariablesInSum(add);
          return add.Subtrees.Count() == 1 ? add.GetSubtree(0) : add;
        } else {
          var add = AddSymbol.CreateTreeNode();
          add.AddSubtree(a);
          add.AddSubtree(b);
          MergeVariablesInSum(add);
          return add.Subtrees.Count() == 1 ? add.GetSubtree(0) : add;
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
      var sumNumbers = (from node in subtrees.OfType<NumberTreeNode>()
                        select node.Value).DefaultIfEmpty(0.0).Sum();
      var unchangedSubtrees = subtrees.Where(t => t.SubtreeCount > 0 || !(t is IVariableTreeNode) && !(t is NumberTreeNode));

      foreach (var variableNodeGroup in groupedVarNodes) {
        var firstNode = variableNodeGroup.First();
        switch (firstNode) {
          case VariableTreeNodeBase @base: {
            var weightSum = variableNodeGroup.Cast<VariableTreeNodeBase>().Select(t => t.Weight).Sum();
            @base.Weight = weightSum;
            sum.AddSubtree(@base);
            break;
          }
          case FactorVariableTreeNode representative: {
            foreach (var node in variableNodeGroup.Skip(1).Cast<FactorVariableTreeNode>()) {
              for (var j = 0; j < representative.Weights.Length; j++) {
                representative.Weights[j] += node.Weights[j];
              }
            }
            sum.AddSubtree(representative);
            break;
          }
        }
      }
      foreach (var unchangedSubtree in unchangedSubtrees)
        sum.AddSubtree(unchangedSubtree);
      if (sumNumbers != 0.0) {
        sum.AddSubtree(Number(sumNumbers));
      }
    }

    // nodes referencing variables can be grouped if they have
    private static string GroupId(IVariableTreeNode node) {
      switch (node) {
        case VariableTreeNode variableNode:
          return "var " + variableNode.VariableName;
        case BinaryFactorVariableTreeNode binaryFactorNode:
          return "binfactor " + binaryFactorNode.VariableName + " " + binaryFactorNode.VariableValue;
        case FactorVariableTreeNode factorNode:
          return "factor " + factorNode.VariableName;
        case LaggedVariableTreeNode laggedVarNode:
          return "lagged " + laggedVarNode.VariableName + " " + laggedVarNode.Lag;
        default:
          throw new NotSupportedException();
      }
    }


    private static ISymbolicExpressionTreeNode Product(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (IsNumber(a) && IsNumber(b)) {
        // fold constants
        return Number(((NumberTreeNode)a).Value * ((NumberTreeNode)b).Value);
      }

      if (IsNumber(a)) {
        // a * $ => $ * a
        return Product(b, a);
      }

      if (IsFactor(a) && IsFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as FactorVariableTreeNode;
        return Factor(node0.Symbol, node0.VariableName, node0.Weights.Zip(node1.Weights, (u, v) => u * v));
      }

      if (IsBinFactor(a) && IsBinFactor(b) && AreSameTypeAndVariable(a, b)) {
        var node0 = a as BinaryFactorVariableTreeNode;
        var node1 = b as BinaryFactorVariableTreeNode;
        return BinFactor(node0.Symbol, node0.VariableName, node0.VariableValue, node0.Weight * node1.Weight);
      }

      if (IsFactor(a) && IsNumber(b)) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as NumberTreeNode;
        return Factor(node0.Symbol, node0.VariableName, node0.Weights.Select(w => w * node1.Value));
      }

      if (IsBinFactor(a) && IsNumber(b)) {
        var node0 = a as BinaryFactorVariableTreeNode;
        var node1 = b as NumberTreeNode;
        return BinFactor(node0.Symbol, node0.VariableName, node0.VariableValue, node0.Weight * node1.Value);
      }

      if (IsBinFactor(a) && IsFactor(b)) {
        return Product(b, a);
      }

      if (IsFactor(a) && IsBinFactor(b) &&
          ((IVariableTreeNode)a).VariableName == ((IVariableTreeNode)b).VariableName) {
        var node0 = a as FactorVariableTreeNode;
        var node1 = b as BinaryFactorVariableTreeNode;
        var varValues = node0.Symbol.GetVariableValues(node0.VariableName).ToArray();
        var wi = Array.IndexOf(varValues, node1.VariableValue);
        if (wi < 0) throw new ArgumentException();
        return BinFactor(node1.Symbol, node1.VariableName, node1.VariableValue, node1.Weight * node0.Weights[wi]);
      }

      if (IsNumber(b) && ((NumberTreeNode)b).Value == 1.0) {
        // $ * 1.0 => $
        return a;
      }

      if (IsNumber(b) && ((NumberTreeNode)b).Value == 0.0) {
        return Number(0);
      }

      if (IsNumber(b) && IsVariableBase(a)) {
        // multiply constants into variables weights
        ((VariableTreeNodeBase)a).Weight *= ((NumberTreeNode)b).Value;
        return a;
      }

      if (IsNumber(b) && IsAddition(a) ||
          IsFactor(b) && IsAddition(a) ||
          IsBinFactor(b) && IsAddition(a)) {
        // multiply numbers into additions
        return a.Subtrees.Select(x => Product(GetSimplifiedTree(x), GetSimplifiedTree(b))).Aggregate((c, d) => Sum(c, d));
      }

      if (IsDivision(a) && IsDivision(b)) {
        // (a1 / a2) * (b1 / b2) => (a1 * b1) / (a2 * b2)
        return Fraction(Product(a.GetSubtree(0), b.GetSubtree(0)), Product(a.GetSubtree(1), b.GetSubtree(1)));
      }

      if (IsDivision(a)) {
        // (a1 / a2) * b => (a1 * b) / a2
        return Fraction(Product(a.GetSubtree(0), b), a.GetSubtree(1));
      }

      if (IsDivision(b)) {
        // a * (b1 / b2) => (b1 * a) / b2
        return Fraction(Product(b.GetSubtree(0), a), b.GetSubtree(1));
      }

      if (IsMultiplication(a) && IsMultiplication(b)) {
        // merge multiplications (make sure constants are merged)
        var mul = MulSymbol.CreateTreeNode();
        for (var i = 0; i < a.Subtrees.Count(); i++) mul.AddSubtree(a.GetSubtree(i));
        for (var i = 0; i < b.Subtrees.Count(); i++) mul.AddSubtree(b.GetSubtree(i));
        MergeVariablesAndConstantsInProduct(mul);
        return mul;
      }

      if (IsMultiplication(b)) {
        return Product(b, a);
      }

      if (IsMultiplication(a)) {
        // a is already a multiplication => append b
        a.AddSubtree(GetSimplifiedTree(b));
        MergeVariablesAndConstantsInProduct(a);
        return a;
      }

      if (IsAbsolute(a) && IsAbsolute(b)) {
        return Abs(Product(a.GetSubtree(0), b.GetSubtree(0)));
      }

      if (IsAbsolute(a) && IsNumber(b)) {
        var bNode = b as NumberTreeNode;
        var posF = Math.Abs(bNode.Value);
        if (bNode.Value > 0) {
          return Abs(Product(a.GetSubtree(0), Number(posF)));
        }

        var mul = MulSymbol.CreateTreeNode();
        mul.AddSubtree(Abs(Product(a.GetSubtree(0), Number(posF))));
        mul.AddSubtree(Number(-1.0));
        return mul;
      }

      if (IsAnalyticalQuotient(a)) {
        return Aq(Product(a.GetSubtree(0), b), a.GetSubtree(1));
      }

      {
        var mul = MulSymbol.CreateTreeNode();
        mul.AddSubtree(a);
        mul.AddSubtree(b);
        MergeVariablesAndConstantsInProduct(mul);
        return mul;
      }
    }

    #endregion

    #region helper functions

    private static bool ContainsVariableCondition(ISymbolicExpressionTreeNode node) {
      return node.Symbol is VariableCondition || node.Subtrees.Any(ContainsVariableCondition);
    }

    private static ISymbolicExpressionTreeNode AddLagToDynamicNodes(ISymbolicExpressionTreeNode node, int lag) {
      switch (node) {
        case ILaggedTreeNode laggedTreeNode:
          laggedTreeNode.Lag += lag;
          break;
        case VariableTreeNode variableNode: {
          var laggedVariableNode = (LaggedVariableTreeNode)LaggedVariableSymbol.CreateTreeNode();
          laggedVariableNode.Lag = lag;
          laggedVariableNode.VariableName = variableNode.VariableName;
          return laggedVariableNode;
        }
        case VariableConditionTreeNode _:
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
      var numberProduct = (from node in subtrees.OfType<VariableTreeNodeBase>()
                           select node.Weight)
                          .Concat(from node in subtrees.OfType<NumberTreeNode>()
                                  select node.Value)
                          .DefaultIfEmpty(1.0)
                          .Aggregate((c1, c2) => c1 * c2);

      var unchangedSubtrees = from tree in subtrees
                              where tree.SubtreeCount > 0 || !(tree is IVariableTreeNode) && !(tree is NumberTreeNode)
                              select tree;

      foreach (var variableNodeGroup in groupedVarNodes) {
        var firstNode = variableNodeGroup.First();
        switch (firstNode) {
          case VariableTreeNodeBase @base: {
            @base.Weight = 1.0;
            if (variableNodeGroup.Count() > 1) {
              var poly = MulSymbol.CreateTreeNode();
              for (var p = 0; p < variableNodeGroup.Count(); p++) {
                poly.AddSubtree((ISymbolicExpressionTreeNode)@base.Clone());
              }
              prod.AddSubtree(poly);
            } else {
              prod.AddSubtree(@base);
            }

            break;
          }
          case FactorVariableTreeNode representative: {
            foreach (var node in variableNodeGroup.Skip(1).Cast<FactorVariableTreeNode>()) {
              for (var j = 0; j < representative.Weights.Length; j++) {
                representative.Weights[j] *= node.Weights[j];
              }
            }
            for (var j = 0; j < representative.Weights.Length; j++) {
              representative.Weights[j] *= numberProduct;
            }
            numberProduct = 1.0;
            // if the product already contains a factor it is not necessary to multiply a constant below
            prod.AddSubtree(representative);
            break;
          }
        }
      }

      foreach (var unchangedSubtree in unchangedSubtrees)
        prod.AddSubtree(unchangedSubtree);

      if (numberProduct != 1.0) {
        prod.AddSubtree(Number(numberProduct));
      }
    }


    /// <summary>
    /// x => x * -1
    /// Is only used in cases where it is not necessary to create new tree nodes. Manipulates x directly.
    /// </summary>
    /// <param name="x"></param>
    /// <returns>-x</returns>
    private static ISymbolicExpressionTreeNode Negate(ISymbolicExpressionTreeNode x) {
      if (IsNumber(x)) {
        ((NumberTreeNode)x).Value *= -1;
      } else if (IsVariableBase(x)) {
        var variableTree = (VariableTreeNodeBase)x;
        variableTree.Weight *= -1.0;
      } else if (IsFactor(x)) {
        var factorNode = (FactorVariableTreeNode)x;
        for (var i = 0; i < factorNode.Weights.Length; i++) factorNode.Weights[i] *= -1;
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
        return Product(x, Number(-1));
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
      if (IsNumber(x))
        return Number(1.0 / ((NumberTreeNode)x).Value);

      if (!IsFactor(x))
        return IsDivision(x)
          ? Fraction(x.GetSubtree(1), x.GetSubtree(0))
          :
          // any other function
          Fraction(Number(1), x);
      var factorNode = (FactorVariableTreeNode)x;
      return Factor(factorNode.Symbol, factorNode.VariableName, factorNode.Weights.Select(w => 1.0 / w));

    }

    private static ISymbolicExpressionTreeNode Number(double value) {
      var numberTreeNode = (NumberTreeNode)NumberSymbol.CreateTreeNode();
      numberTreeNode.Value = value;
      return numberTreeNode;
    }

    private static ISymbolicExpressionTreeNode Factor(FactorVariable sy, string variableName, IEnumerable<double> weights) {
      var tree = (FactorVariableTreeNode)sy.CreateTreeNode();
      tree.VariableName = variableName;
      tree.Weights = weights.ToArray();
      return tree;
    }
    private static ISymbolicExpressionTreeNode BinFactor(BinaryFactorVariable sy, string variableName, string variableValue, double weight) {
      var tree = (BinaryFactorVariableTreeNode)sy.CreateTreeNode();
      tree.VariableName = variableName;
      tree.VariableValue = variableValue;
      tree.Weight = weight;
      return tree;
    }


    #endregion
  }
}
