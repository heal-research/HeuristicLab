#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq.Expressions;
using System.Reflection;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SymbolicDataAnalysisExpressionCompiledTreeInterpreter", "Interpreter that converts the tree into a Linq.Expression then compiles it.")]
  public sealed class SymbolicDataAnalysisExpressionCompiledTreeInterpreter : ParameterizedNamedItem, ISymbolicDataAnalysisExpressionTreeInterpreter {
    private const string CheckExpressionsWithIntervalArithmeticParameterName = "CheckExpressionsWithIntervalArithmetic";
    private const string CheckExpressionsWithIntervalArithmeticParameterDescription = "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.";
    private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";

    #region method info for the commonly called functions
    private static readonly MethodInfo Sin = typeof(Math).GetMethod("Sin", new[] { typeof(double) });
    private static readonly MethodInfo Cos = typeof(Math).GetMethod("Cos", new[] { typeof(double) });
    private static readonly MethodInfo Tan = typeof(Math).GetMethod("Tan", new[] { typeof(double) });
    private static readonly MethodInfo Sqrt = typeof(Math).GetMethod("Sqrt", new[] { typeof(double) });
    private static readonly MethodInfo Floor = typeof(Math).GetMethod("Floor", new[] { typeof(double) });
    private static readonly MethodInfo Exp = typeof(Math).GetMethod("Exp", new[] { typeof(double) });
    private static readonly MethodInfo Log = typeof(Math).GetMethod("Log", new[] { typeof(double) });
    private static readonly MethodInfo IsNaN = typeof(double).GetMethod("IsNaN");
    private static readonly MethodInfo Gamma = typeof(alglib).GetMethod("gammafunction", new[] { typeof(double) });
    private static readonly MethodInfo Psi = typeof(alglib).GetMethod("psi", new[] { typeof(double) });
    private static readonly MethodInfo DawsonIntegral = typeof(alglib).GetMethod("dawsonintegral", new[] { typeof(double) });
    private static readonly MethodInfo ExponentialIntegralEi = typeof(alglib).GetMethod("exponentialintegralei", new[] { typeof(double) });
    private static readonly MethodInfo SineCosineIntegrals = typeof(alglib).GetMethod("sinecosineintegrals", new[] { typeof(double), typeof(double).MakeByRefType(), typeof(double).MakeByRefType() });
    private static readonly MethodInfo HyperbolicSineCosineIntegrals = typeof(alglib).GetMethod("hyperbolicsinecosineintegrals", new[] { typeof(double), typeof(double).MakeByRefType(), typeof(double).MakeByRefType() });
    private static readonly MethodInfo FresnelIntegral = typeof(alglib).GetMethod("fresnelintegral", new[] { typeof(double), typeof(double).MakeByRefType(), typeof(double).MakeByRefType() });
    private static readonly MethodInfo Airy = typeof(alglib).GetMethod("airy", new[] { typeof(double), typeof(double).MakeByRefType(), typeof(double).MakeByRefType(), typeof(double).MakeByRefType(), typeof(double).MakeByRefType() });
    private static readonly MethodInfo NormalDistribution = typeof(alglib).GetMethod("normaldistribution", new[] { typeof(double) });
    private static readonly MethodInfo ErrorFunction = typeof(alglib).GetMethod("errorfunction", new[] { typeof(double) });
    private static readonly MethodInfo Bessel = typeof(alglib).GetMethod("besseli0", new[] { typeof(double) });
    #endregion

    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }

    #region parameter properties
    public IFixedValueParameter<BoolValue> CheckExpressionsWithIntervalArithmeticParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[CheckExpressionsWithIntervalArithmeticParameterName]; }
    }

    public IFixedValueParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[EvaluatedSolutionsParameterName]; }
    }
    #endregion

    #region properties
    public bool CheckExpressionsWithIntervalArithmetic {
      get { return CheckExpressionsWithIntervalArithmeticParameter.Value.Value; }
      set { CheckExpressionsWithIntervalArithmeticParameter.Value.Value = value; }
    }
    public int EvaluatedSolutions {
      get { return EvaluatedSolutionsParameter.Value.Value; }
      set { EvaluatedSolutionsParameter.Value.Value = value; }
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionCompiledTreeInterpreter(this, cloner);
    }

    private SymbolicDataAnalysisExpressionCompiledTreeInterpreter(SymbolicDataAnalysisExpressionCompiledTreeInterpreter original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionCompiledTreeInterpreter(bool deserializing)
      : base(deserializing) {
    }

    public SymbolicDataAnalysisExpressionCompiledTreeInterpreter() :
      base("SymbolicDataAnalysisExpressionCompiledTreeInterpreter", "Interpreter which compiles the tree into a lambda") {
      Parameters.Add(new FixedValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, CheckExpressionsWithIntervalArithmeticParameterDescription, new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
    }

    public SymbolicDataAnalysisExpressionCompiledTreeInterpreter(string name, string description) :
      base(name, description) {
      Parameters.Add(new FixedValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, CheckExpressionsWithIntervalArithmeticParameterDescription, new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
    }

    public void InitializeState() {
      EvaluatedSolutions = 0;
    }

    public void ClearState() { }

    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows) {
      if (CheckExpressionsWithIntervalArithmetic)
        throw new NotSupportedException("Interval arithmetic is not yet supported in the symbolic data analysis interpreter.");

      lock (EvaluatedSolutionsParameter.Value) {
        EvaluatedSolutions++; // increment the evaluated solutions counter
      }
      var columns = dataset.DoubleVariables.Select(x => (IList<double>)dataset.GetReadOnlyDoubleValues(x)).ToArray();
      var compiled = CompileTree(tree, dataset);
      return rows.Select(x => compiled(x, columns));
    }

    public static Func<int, IList<double>[], double> CompileTree(ISymbolicExpressionTree tree, IDataset dataset) {
      var row = Expression.Parameter(typeof(int));
      var columns = Expression.Parameter(typeof(IList<double>[]));
      var variableIndices = dataset.DoubleVariables.Select((x, i) => new { x, i }).ToDictionary(e => e.x, e => e.i);
      var expr = MakeExpr(tree, variableIndices, row, columns);
      var lambda = Expression.Lambda<Func<int, IList<double>[], double>>(expr, row, columns);
      return lambda.Compile();
    }

    private static Expression MakeExpr(ISymbolicExpressionTree tree, Dictionary<string, int> variableIndices, Expression row, Expression columns) {
      var actualRoot = tree.Root.GetSubtree(0).GetSubtree(0);
      return MakeExpr(actualRoot, variableIndices, row, columns);
    }

    private static readonly PropertyInfo Indexer = typeof(IList<double>).GetProperty("Item");
    private static Expression MakeExpr(ISymbolicExpressionTreeNode node, Dictionary<string, int> variableIndices, Expression row, Expression columns) {
      var opcode = OpCodes.MapSymbolToOpCode(node);
      #region switch opcode
      switch (opcode) {
        case OpCodes.Constant: {
            var constantTreeNode = (ConstantTreeNode)node;
            return Expression.Constant(constantTreeNode.Value);
          }
        case OpCodes.Variable: {
            var variableTreeNode = (VariableTreeNode)node;
            var variableWeight = Expression.Constant(variableTreeNode.Weight);
            var variableName = variableTreeNode.VariableName;
            var indexExpr = Expression.Constant(variableIndices[variableName]);
            var valuesExpr = Expression.ArrayIndex(columns, indexExpr);
            var variableValue = Expression.Property(valuesExpr, Indexer, row);
            return Expression.Multiply(variableWeight, variableValue);
          }
        case OpCodes.Add: {
            Expression result = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            for (int i = 1; i < node.SubtreeCount; ++i) {
              result = Expression.Add(result, MakeExpr(node.GetSubtree(i), variableIndices, row, columns));
            }
            return result;
          }
        case OpCodes.Sub: {
            Expression result = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            if (node.SubtreeCount == 1)
              return Expression.Negate(result);
            for (int i = 1; i < node.SubtreeCount; ++i) {
              result = Expression.Subtract(result, MakeExpr(node.GetSubtree(i), variableIndices, row, columns));
            }
            return result;
          }
        case OpCodes.Mul: {
            Expression result = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            for (int i = 1; i < node.SubtreeCount; ++i) {
              result = Expression.Multiply(result, MakeExpr(node.GetSubtree(i), variableIndices, row, columns));
            }
            return result;
          }
        case OpCodes.Div: {
            Expression result = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            if (node.SubtreeCount == 1)
              return Expression.Divide(Expression.Constant(1.0), result);
            for (int i = 1; i < node.SubtreeCount; ++i) {
              result = Expression.Divide(result, MakeExpr(node.GetSubtree(i), variableIndices, row, columns));
            }
            return result;
          }
        case OpCodes.Average: {
            Expression result = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            for (int i = 1; i < node.SubtreeCount; ++i) {
              result = Expression.Add(result, MakeExpr(node.GetSubtree(i), variableIndices, row, columns));
            }
            return Expression.Divide(result, Expression.Constant((double)node.SubtreeCount));
          }
        case OpCodes.Cos: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            return Expression.Call(Cos, arg);
          }
        case OpCodes.Sin: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            return Expression.Call(Sin, arg);
          }
        case OpCodes.Tan: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            return Expression.Call(Tan, arg);
          }
        case OpCodes.Square: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            return Expression.Power(arg, Expression.Constant(2));
          }
        case OpCodes.Power: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var power = MakeExpr(node.GetSubtree(1), variableIndices, row, columns);
            return Expression.Power(arg, Expression.Call(Floor, power));
          }
        case OpCodes.SquareRoot: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            return Expression.Call(Sqrt, arg);
          }
        case OpCodes.Root: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var power = MakeExpr(node.GetSubtree(1), variableIndices, row, columns);
            return Expression.Power(arg, Expression.Divide(Expression.Constant(1.0), power));
          }
        case OpCodes.Exp: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            return Expression.Call(Exp, arg);
          }
        case OpCodes.Log: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            return Expression.Call(Log, arg);
          }
        case OpCodes.Gamma: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var gamma = Expression.Call(Gamma, arg);

            var result = Expression.Variable(typeof(double));
            var expr = Expression.Block(
              new[] { result },
              Expression.IfThenElse(
                isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, gamma)
                ),
              result
              );
            return expr;
          }
        case OpCodes.Psi: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var psi = Expression.Call(Psi, arg);

            var result = Expression.Variable(typeof(double));
            var floor = Expression.Call(Floor, arg);
            var expr = Expression.Block(
              new[] { result },
              Expression.IfThenElse(
                isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.IfThenElse(
                  Expression.AndAlso(Expression.LessThanOrEqual(arg, Expression.Constant(0.0)),
                    Expression.Equal(Expression.Subtract(floor, arg), Expression.Constant(0.0))),
                  Expression.Assign(result, Expression.Constant(double.NaN)),
                  Expression.Assign(result, psi))
                ),
              result);

            return expr;
          }
        case OpCodes.Dawson: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var exprDawsonIntegral = Expression.Call(DawsonIntegral, arg);
            var result = Expression.Variable(typeof(double));

            var expr = Expression.Block(
              new[] { result },
              Expression.IfThenElse(isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, exprDawsonIntegral)),
              result
              );

            return expr;
          }
        case OpCodes.ExponentialIntegralEi: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var expIntegrapEi =
              Expression.Call(ExponentialIntegralEi, arg);
            var result = Expression.Variable(typeof(double));
            var expr = Expression.Block(
              new[] { result },
              Expression.IfThenElse(isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, expIntegrapEi)),
              result
              );

            return expr;
          }
        case OpCodes.SineIntegral: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var si = Expression.Variable(typeof(double));
            var ci = Expression.Variable(typeof(double));
            var sinCosIntegrals = Expression.Call(SineCosineIntegrals, arg, si, ci);
            var block = Expression.Block(
              new[] { si, ci },
              sinCosIntegrals,
              si
              );
            var result = Expression.Variable(typeof(double));
            var expr = Expression.Block(new[] { result },
              Expression.IfThenElse(isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, block)),
              result
              );

            return expr;
          }
        case OpCodes.CosineIntegral: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var si = Expression.Variable(typeof(double));
            var ci = Expression.Variable(typeof(double));
            var sinCosIntegrals = Expression.Call(SineCosineIntegrals, arg, si, ci);
            var block = Expression.Block(
              new[] { si, ci },
              sinCosIntegrals,
              ci
              );
            var result = Expression.Variable(typeof(double));
            var expr = Expression.Block(new[] { result },
              Expression.IfThenElse(isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, block)),
              result
              );

            return expr;
          }
        case OpCodes.HyperbolicSineIntegral: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var shi = Expression.Variable(typeof(double));
            var chi = Expression.Variable(typeof(double));
            var hypSinCosIntegrals = Expression.Call(HyperbolicSineCosineIntegrals, arg, shi, chi);
            var block = Expression.Block(
              new[] { shi, chi },
              hypSinCosIntegrals,
              shi
              );
            var result = Expression.Variable(typeof(double));
            var expr = Expression.Block(new[] { result },
              Expression.IfThenElse(isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, block)),
              result
              );

            return expr;
          }
        case OpCodes.HyperbolicCosineIntegral: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var shi = Expression.Variable(typeof(double));
            var chi = Expression.Variable(typeof(double));
            var hypSinCosIntegrals = Expression.Call(HyperbolicSineCosineIntegrals, arg, shi, chi);
            var block = Expression.Block(
              new[] { shi, chi },
              hypSinCosIntegrals,
              chi
              );
            var result = Expression.Variable(typeof(double));
            var expr = Expression.Block(new[] { result },
              Expression.IfThenElse(isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, block)),
              result
              );

            return expr;
          }
        case OpCodes.FresnelSineIntegral: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var s = Expression.Variable(typeof(double));
            var c = Expression.Variable(typeof(double));
            var fresnel = Expression.Call(FresnelIntegral, arg, c, s);
            var block = Expression.Block(new[] { s, c }, fresnel, s);
            var result = Expression.Variable(typeof(double));
            var expr = Expression.Block(new[] { result },
              Expression.IfThenElse(isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, block)),
              result
              );

            return expr;
          }
        case OpCodes.FresnelCosineIntegral: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var s = Expression.Variable(typeof(double));
            var c = Expression.Variable(typeof(double));
            var fresnel = Expression.Call(FresnelIntegral, arg, c, s);
            var block = Expression.Block(new[] { s, c }, fresnel, c);
            var result = Expression.Variable(typeof(double));
            var expr = Expression.Block(new[] { result },
              Expression.IfThenElse(isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, block)),
              result
              );

            return expr;
          }
        case OpCodes.AiryA: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var ai = Expression.Variable(typeof(double));
            var aip = Expression.Variable(typeof(double));
            var bi = Expression.Variable(typeof(double));
            var bip = Expression.Variable(typeof(double));
            var airy = Expression.Call(Airy, arg, ai, aip, bi, bip);
            var block = Expression.Block(new[] { ai, aip, bi, bip }, airy, ai);
            var result = Expression.Variable(typeof(double));
            var expr = Expression.Block(new[] { result },
              Expression.IfThenElse(isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, block)),
              result
              );

            return expr;
          }
        case OpCodes.AiryB: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var ai = Expression.Variable(typeof(double));
            var aip = Expression.Variable(typeof(double));
            var bi = Expression.Variable(typeof(double));
            var bip = Expression.Variable(typeof(double));
            var airy = Expression.Call(Airy, arg, ai, aip, bi, bip);
            var block = Expression.Block(new[] { ai, aip, bi, bip }, airy, bi);
            var result = Expression.Variable(typeof(double));
            var expr = Expression.Block(new[] { result },
              Expression.IfThenElse(isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, block)),
              result
              );

            return expr;
          }
        case OpCodes.Norm: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var result = Expression.Variable(typeof(double));
            var norm = Expression.Call(NormalDistribution, arg);

            var expr = Expression.Block(new[] { result },
              Expression.IfThenElse(isNaN, Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, norm)), result);

            return expr;
          }
        case OpCodes.Erf: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var result = Expression.Variable(typeof(double));
            var erf = Expression.Call(ErrorFunction, arg);

            var expr = Expression.Block(new[] { result },
              Expression.IfThenElse(isNaN, Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, erf)), result);

            return expr;
          }
        case OpCodes.Bessel: {
            var arg = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var isNaN = Expression.Call(IsNaN, arg);
            var result = Expression.Variable(typeof(double));
            var bessel = Expression.Call(Bessel, arg);
            var expr = Expression.Block(
              new[] { result },
              Expression.IfThenElse(
                isNaN,
                Expression.Assign(result, Expression.Constant(double.NaN)),
                Expression.Assign(result, bessel)),
              result);

            return expr;
          }
        case OpCodes.IfThenElse: {
            var test = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var result = Expression.Variable(typeof(double));
            var condition = Expression.IfThenElse(Expression.GreaterThan(test, Expression.Constant(0.0)),
              Expression.Assign(result, MakeExpr(node.GetSubtree(1), variableIndices, row, columns)),
              Expression.Assign(result, MakeExpr(node.GetSubtree(2), variableIndices, row, columns)));
            return Expression.Block(new[] { result }, condition, result);
          }
        case OpCodes.AND: {
            var result = Expression.Variable(typeof(double));
            var expr = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);

            for (int i = 1; i < node.SubtreeCount; ++i) {
              expr = Expression.Block(new[] { result },
                Expression.IfThenElse(
                  Expression.GreaterThan(expr, Expression.Constant(0.0)),
                  Expression.Assign(result, MakeExpr(node.GetSubtree(i), variableIndices, row, columns)),
                  Expression.Assign(result, expr)),
                result
                );
            }

            return Expression.Block(
              new[] { result },
              Expression.Assign(result, expr),
              Expression.IfThenElse(
                Expression.GreaterThan(result, Expression.Constant(0.0)),
                Expression.Assign(result, Expression.Constant(1.0)),
                Expression.Assign(result, Expression.Constant(-1.0))
                ),
              result
              );
          }
        case OpCodes.OR: {
            var result = Expression.Variable(typeof(double));
            var expr = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);

            for (int i = 1; i < node.SubtreeCount; ++i) {
              expr = Expression.Block(new[] { result },
                Expression.IfThenElse(
                  Expression.LessThanOrEqual(expr, Expression.Constant(0.0)),
                  Expression.Assign(result, MakeExpr(node.GetSubtree(i), variableIndices, row, columns)),
                  Expression.Assign(result, expr)),
                result
                );
            }

            return Expression.Block(
              new[] { result },
              Expression.Assign(result, expr),
              Expression.IfThenElse(
                Expression.GreaterThan(result, Expression.Constant(0.0)),
                Expression.Assign(result, Expression.Constant(1.0)),
                Expression.Assign(result, Expression.Constant(-1.0))
                ),
              result
              );
          }
        case OpCodes.NOT: {
            var value = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var result = Expression.Variable(typeof(double));
            var condition = Expression.IfThenElse(Expression.GreaterThan(value, Expression.Constant(0.0)),
              Expression.Assign(result, Expression.Constant(-1.0)),
              Expression.Assign(result, Expression.Constant(1.0)));
            return Expression.Block(new[] { result }, condition, result);
          }
        case OpCodes.XOR: {
            var ps = Expression.Variable(typeof(int));
            var block = Expression.Block(
              new[] { ps },
              Expression.Assign(ps, Expression.Constant(0)),
              ps
              );

            foreach (var subtree in node.Subtrees) {
              var expr = MakeExpr(subtree, variableIndices, row, columns);
              block = Expression.Block(
                new[] { ps },
                Expression.Assign(ps, block),
                Expression.IfThen(Expression.GreaterThan(expr, Expression.Constant(0.0)),
                  Expression.PostIncrementAssign(ps)),
                ps
                );
            }

            var result = Expression.Variable(typeof(double));
            var xorExpr = Expression.Block(
              new[] { result },
              Expression.IfThenElse(
                Expression.Equal(Expression.Modulo(block, Expression.Constant(2)), Expression.Constant(0)),
                Expression.Assign(result, Expression.Constant(-1.0)),
                Expression.Assign(result, Expression.Constant(1.0))
                ),
              result
              );
            return xorExpr;
          }
        case OpCodes.GT: {
            var left = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var right = MakeExpr(node.GetSubtree(1), variableIndices, row, columns);
            var result = Expression.Variable(typeof(double));

            var condition = Expression.IfThenElse(Expression.GreaterThan(left, right),
              Expression.Assign(result, Expression.Constant(1.0)), Expression.Assign(result, Expression.Constant(-1.0)));
            return Expression.Block(
              new[] { result },
              condition,
              result);
          }
        case OpCodes.LT: {
            var left = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var right = MakeExpr(node.GetSubtree(1), variableIndices, row, columns);
            var result = Expression.Variable(typeof(double));

            var condition = Expression.IfThenElse(Expression.LessThan(left, right),
              Expression.Assign(result, Expression.Constant(1.0)), Expression.Assign(result, Expression.Constant(-1.0)));
            return Expression.Block(new[] { result }, condition, result);
          }
        case OpCodes.VariableCondition: {
            var variableConditionTreeNode = (VariableConditionTreeNode)node;
            var variableName = variableConditionTreeNode.VariableName;
            var indexExpr = Expression.Constant(variableIndices[variableName]);
            var valuesExpr = Expression.ArrayIndex(columns, indexExpr);
            var variableValue = Expression.ArrayIndex(valuesExpr, row);
            var variableThreshold = Expression.Constant(variableConditionTreeNode.Threshold);
            var variableSlope = Expression.Constant(variableConditionTreeNode.Slope);

            var x = Expression.Subtract(variableValue, variableThreshold);
            var xSlope = Expression.Multiply(Expression.Negate(variableSlope), x);
            var xSlopeExp = Expression.Call(Exp, xSlope);
            var p = Expression.Divide(Expression.Constant(1), Expression.Add(Expression.Constant(1), xSlopeExp));
            var trueBranch = MakeExpr(node.GetSubtree(0), variableIndices, row, columns);
            var falseBranch = MakeExpr(node.GetSubtree(1), variableIndices, row, columns);
            return Expression.Add(
              Expression.Multiply(trueBranch, p),
              Expression.Multiply(falseBranch, Expression.Subtract(Expression.Constant(1), p))
              );
          }
        case OpCodes.LagVariable: {
            var laggedVariableTreeNode = (LaggedVariableTreeNode)node;
            var lag = Expression.Constant(laggedVariableTreeNode.Lag);
            var variableWeight = Expression.Constant(laggedVariableTreeNode.Weight);
            var variableName = laggedVariableTreeNode.VariableName;
            var indexExpr = Expression.Constant(variableIndices[variableName]);
            var valuesExpr = Expression.ArrayIndex(columns, indexExpr);
            var variableValue = Expression.Property(valuesExpr, Indexer, Expression.Add(row, lag));
            return Expression.Multiply(variableWeight, variableValue);
          }
        case OpCodes.TimeLag: {
            var timeLagTreeNode = (LaggedTreeNode)node;
            var lag = Expression.Constant(timeLagTreeNode.Lag);
            return MakeExpr(timeLagTreeNode.GetSubtree(0), variableIndices, Expression.Add(row, lag), columns);
          }
        case OpCodes.Integral: {
            var timeLagTreeNode = (LaggedTreeNode)node;
            var subtree = node.GetSubtree(0);
            var sum = MakeExpr(subtree, variableIndices, row, columns);
            var sign = Expression.Constant(Math.Sign(timeLagTreeNode.Lag));
            var lag = Expression.Add(row, sign);
            for (int i = 0; i < Math.Abs(timeLagTreeNode.Lag); ++i) {
              sum = Expression.Add(sum, MakeExpr(subtree, variableIndices, lag, columns));
              lag = Expression.Add(lag, sign);
            }
            return sum;
          }
        case OpCodes.Derivative: {
            var subtree = node.GetSubtree(0);
            var f0 = MakeExpr(subtree, variableIndices, row, columns);
            var f1 = MakeExpr(subtree, variableIndices, Expression.Subtract(row, Expression.Constant(1)), columns);
            var f3 = MakeExpr(subtree, variableIndices, Expression.Subtract(row, Expression.Constant(3)), columns);
            var f4 = MakeExpr(subtree, variableIndices, Expression.Subtract(row, Expression.Constant(4)), columns);

            var result = f0;
            result = Expression.Add(result, Expression.Multiply(Expression.Constant(2.0), f1));
            result = Expression.Subtract(result, Expression.Multiply(Expression.Constant(2.0), f3));
            result = Expression.Subtract(result, f4);
            return Expression.Divide(result, Expression.Constant(8.0));
          }
        default:
          throw new NotSupportedException("Unsupported symbol: " + node.Symbol);
      }
      #endregion
    }
    // util stuff
    private static Func<T, R> GetField<T, R>(string fieldName) {
      ParameterExpression param = Expression.Parameter(typeof(T), "arg");
      MemberExpression member = Expression.Field(param, fieldName);
      LambdaExpression lambda = Expression.Lambda(typeof(Func<T, R>), member, param);
      Func<T, R> compiled = (Func<T, R>)lambda.Compile();
      return compiled;
    }
  }
}
