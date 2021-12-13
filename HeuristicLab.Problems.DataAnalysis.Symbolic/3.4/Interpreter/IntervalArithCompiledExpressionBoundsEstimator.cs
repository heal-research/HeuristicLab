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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("60015D64-5D8B-408A-90A1-E4111BC114D4")]
  [Item("Interval Arithmetic Compiled Expression Bounds Estimator", "Compile a symbolic model into a lambda and use it to evaluate model bounds.")]
  public class IntervalArithCompiledExpressionBoundsEstimator : ParameterizedNamedItem, IBoundsEstimator {
    // interval method names
    private static readonly Dictionary<byte, string> methodName = new Dictionary<byte, string>() {
      { OpCodes.Add, "Add" },
      { OpCodes.Sub, "Subtract" },
      { OpCodes.Mul, "Multiply" },
      { OpCodes.Div, "Divide" },
      { OpCodes.Sin, "Sine" },
      { OpCodes.Cos, "Cosine" },
      { OpCodes.Tan, "Tangens" },
      { OpCodes.Tanh, "HyperbolicTangent" },
      { OpCodes.Log, "Logarithm" },
      { OpCodes.Exp, "Exponential" },
      { OpCodes.Square, "Square" },
      { OpCodes.Cube, "Cube" },
      { OpCodes.SquareRoot, "SquareRoot" },
      { OpCodes.CubeRoot, "CubicRoot" },
      { OpCodes.Absolute, "Absolute" },
      { OpCodes.AnalyticQuotient, "AnalyticalQuotient" },
    };

    private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";
    public IFixedValueParameter<IntValue> EvaluatedSolutionsParameter {
      get => (IFixedValueParameter<IntValue>)Parameters[EvaluatedSolutionsParameterName];
    }
    public int EvaluatedSolutions {
      get => EvaluatedSolutionsParameter.Value.Value;
      set => EvaluatedSolutionsParameter.Value.Value = value;
    }

    private readonly object syncRoot = new object();

    public IntervalArithCompiledExpressionBoundsEstimator() : base("Interval Arithmetic Compiled Expression Bounds Estimator",
      "Estimates the bounds of the model with interval arithmetic, by first compiling the model into a lambda.") {
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName,
        "A counter for the total number of solutions the estimator has evaluated.", new IntValue(0)));
    }

    [StorableConstructor]
    protected IntervalArithCompiledExpressionBoundsEstimator(StorableConstructorFlag _) : base(_) { }

    protected IntervalArithCompiledExpressionBoundsEstimator(IntervalArithCompiledExpressionBoundsEstimator original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntervalArithCompiledExpressionBoundsEstimator(this, cloner);
    }

    public double GetConstraintViolation(ISymbolicExpressionTree tree, IntervalCollection variableRanges, ShapeConstraint constraint) {
      var modelBound = GetModelBound(tree, variableRanges);
      if (constraint.Interval.Contains(modelBound)) return 0.0;
      return Math.Abs(modelBound.LowerBound - constraint.Interval.LowerBound) +
             Math.Abs(modelBound.UpperBound - constraint.Interval.UpperBound);
    }

    public void ClearState() {
      EvaluatedSolutions = 0;
    }

    public Interval GetModelBound(ISymbolicExpressionTree tree, IntervalCollection variableRanges) {
      lock (syncRoot) { EvaluatedSolutions++; }
      var resultInterval = EstimateBounds(tree, variableRanges.GetReadonlyDictionary());

      if (resultInterval.IsInfiniteOrUndefined || resultInterval.LowerBound <= resultInterval.UpperBound)
        return resultInterval;
      return new Interval(resultInterval.UpperBound, resultInterval.LowerBound);
    }

    public IDictionary<ISymbolicExpressionTreeNode, Interval> GetModelNodeBounds(ISymbolicExpressionTree tree, IntervalCollection variableRanges) {
      throw new NotSupportedException("Model nodes bounds are not supported.");
    }

    public void InitializeState() {
      EvaluatedSolutions = 0;
    }

    public bool IsCompatible(ISymbolicExpressionTree tree) {
      var containsUnknownSymbols = (
        from n in tree.Root.GetSubtree(0).IterateNodesPrefix()
        where
          !(n.Symbol is Variable) &&
          !(n.Symbol is Number) &&
          !(n.Symbol is Constant) &&
          !(n.Symbol is StartSymbol) &&
          !(n.Symbol is Addition) &&
          !(n.Symbol is Subtraction) &&
          !(n.Symbol is Multiplication) &&
          !(n.Symbol is Division) &&
          !(n.Symbol is Sine) &&
          !(n.Symbol is Cosine) &&
          !(n.Symbol is Tangent) &&
          !(n.Symbol is HyperbolicTangent) &&
          !(n.Symbol is Logarithm) &&
          !(n.Symbol is Exponential) &&
          !(n.Symbol is Square) &&
          !(n.Symbol is SquareRoot) &&
          !(n.Symbol is Cube) &&
          !(n.Symbol is CubeRoot) &&
          !(n.Symbol is Absolute) &&
          !(n.Symbol is AnalyticQuotient)
        select n).Any();
      return !containsUnknownSymbols;
    }

    #region compile a tree into a IA arithmetic lambda and estimate bounds
    static Expression MakeExpr(ISymbolicExpressionTreeNode node, IReadOnlyDictionary<string, Interval> variableRanges, IReadOnlyDictionary<string, int> variableIndices, Expression args) {
      Expression expr(ISymbolicExpressionTreeNode n) => MakeExpr(n, variableRanges, variableIndices, args);
      var opCode = OpCodes.MapSymbolToOpCode(node);

      switch (opCode) {
        case OpCodes.Variable: {
            var name = (node as VariableTreeNode).VariableName;
            var weight = (node as VariableTreeNode).Weight;
            var index = variableIndices[name];
            return Expression.Multiply(
              Expression.Constant(weight, typeof(double)),
              Expression.ArrayIndex(args, Expression.Constant(index, typeof(int)))
            );
          }
        case OpCodes.Constant: // fall through
        case OpCodes.Number: {
            var v = (node as INumericTreeNode).Value;
            // we have to make an interval out of the number because this may be the root of the tree (and we are expected to return an Interval)
            return Expression.Constant(new Interval(v, v), typeof(Interval));
          }
        case OpCodes.Add: {
            var e = expr(node.GetSubtree(0));
            foreach (var s in node.Subtrees.Skip(1)) {
              e = Expression.Add(e, expr(s));
            }
            return e;
          }
        case OpCodes.Sub: {
            var e = expr(node.GetSubtree(0));
            if (node.SubtreeCount == 1) {
              return Expression.Subtract(Expression.Constant(0.0, typeof(double)), e);
            }
            foreach (var s in node.Subtrees.Skip(1)) {
              e = Expression.Subtract(e, expr(s));
            }
            return e;
          }
        case OpCodes.Mul: {
            var e = expr(node.GetSubtree(0));
            foreach (var s in node.Subtrees.Skip(1)) {
              e = Expression.Multiply(e, expr(s));
            }
            return e;
          }
        case OpCodes.Div: {
            var e1 = expr(node.GetSubtree(0));
            if (node.SubtreeCount == 1) {
              return Expression.Divide(Expression.Constant(1.0, typeof(double)), e1);
            }
            // division is more expensive than multiplication so we use this construct
            var e2 = expr(node.GetSubtree(1));
            foreach (var s in node.Subtrees.Skip(2)) {
              e2 = Expression.Multiply(e2, expr(s));
            }
            return Expression.Divide(e1, e2);
          }
        case OpCodes.AnalyticQuotient: {
            var a = expr(node.GetSubtree(0));
            var b = expr(node.GetSubtree(1));
            var fun = typeof(Interval).GetMethod(methodName[opCode], new[] { a.Type, b.Type });
            return Expression.Call(fun, a, b);
          }
        // all these cases share the same code: get method info by name, emit call expression
        case OpCodes.Exp:
        case OpCodes.Log:
        case OpCodes.Sin:
        case OpCodes.Cos:
        case OpCodes.Tan:
        case OpCodes.Tanh:
        case OpCodes.Square:
        case OpCodes.Cube:
        case OpCodes.SquareRoot:
        case OpCodes.CubeRoot:
        case OpCodes.Absolute: {
            var arg = expr(node.GetSubtree(0));
            var fun = typeof(Interval).GetMethod(methodName[opCode], new[] { arg.Type });
            return Expression.Call(fun, arg);
          }
        default: {
            throw new Exception($"Unsupported OpCode {opCode} encountered.");
          }
      }
    }

    public static IReadOnlyDictionary<string, int> GetVariableIndices(ISymbolicExpressionTree tree, IReadOnlyDictionary<string, Interval> variableIntervals, out Interval[] inputIntervals) {
      var variableIndices = new Dictionary<string, int>();
      var root = tree.Root;
      while (root.Symbol is ProgramRootSymbol || root.Symbol is StartSymbol) {
        root = root.GetSubtree(0);
      }
      inputIntervals = new Interval[variableIntervals.Count];
      int count = 0;
      foreach (var node in root.IterateNodesPrefix()) {
        if (node is VariableTreeNode varNode) {
          var name = varNode.VariableName;
          if (!variableIndices.ContainsKey(name)) {
            variableIndices[name] = count;
            inputIntervals[count] = variableIntervals[name];
            ++count;
          }
        }
      }
      Array.Resize(ref inputIntervals, count);
      return variableIndices;
    }

    public static Func<Interval[], Interval> Compile(ISymbolicExpressionTree tree, IReadOnlyDictionary<string, Interval> variableRanges, IReadOnlyDictionary<string, int> variableIndices) {
      var root = tree.Root.GetSubtree(0).GetSubtree(0);
      var args = Expression.Parameter(typeof(Interval[]));
      var expr = MakeExpr(root, variableRanges, variableIndices, args);
      return Expression.Lambda<Func<Interval[], Interval>>(expr, args).Compile();
    }

    public static Interval EstimateBounds(ISymbolicExpressionTree tree, IReadOnlyDictionary<string, Interval> variableRanges) {
      var variableIndices = GetVariableIndices(tree, variableRanges, out Interval[] x);
      var f = Compile(tree, variableRanges, variableIndices);
      return f(x);
    }
    #endregion
  }
}
