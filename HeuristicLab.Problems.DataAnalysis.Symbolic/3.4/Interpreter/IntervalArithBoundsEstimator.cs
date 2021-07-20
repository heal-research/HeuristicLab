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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("C8539434-6FB0-47D0-9F5A-2CAE5D8B8B4F")]
  [Item("Interval Arithmetic Bounds Estimator", "Interpreter for calculation of intervals of symbolic models.")]
  public sealed class IntervalArithBoundsEstimator : ParameterizedNamedItem, IBoundsEstimator {
    #region Parameters

    private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";

    public IFixedValueParameter<IntValue> EvaluatedSolutionsParameter =>
      (IFixedValueParameter<IntValue>)Parameters[EvaluatedSolutionsParameterName];

    public int EvaluatedSolutions {
      get => EvaluatedSolutionsParameter.Value.Value;
      set => EvaluatedSolutionsParameter.Value.Value = value;
    }
    #endregion

    #region Constructors

    [StorableConstructor]
    private IntervalArithBoundsEstimator(StorableConstructorFlag _) : base(_) { }

    protected IntervalArithBoundsEstimator(IntervalArithBoundsEstimator original, Cloner cloner) : base(original, cloner) { }

    public IntervalArithBoundsEstimator() : base("Interval Arithmetic Bounds Estimator",
      "Estimates the bounds of the model with interval arithmetic") {
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName,
        "A counter for the total number of solutions the estimator has evaluated.", new IntValue(0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntervalArithBoundsEstimator(this, cloner);
    }

    #endregion

    #region IStatefulItem Members

    private readonly object syncRoot = new object();

    public void InitializeState() {
      EvaluatedSolutions = 0;
    }

    public void ClearState() { }

    #endregion

    #region Evaluation

    private static Instruction[] PrepareInterpreterState(
      ISymbolicExpressionTree tree,
      IDictionary<string, Interval> variableRanges) {
      if (variableRanges == null)
        throw new ArgumentNullException("No variable ranges are present!", nameof(variableRanges));

      // Check if all variables used in the tree are present in the dataset
      foreach (var variable in tree.IterateNodesPrefix().OfType<VariableTreeNode>().Select(n => n.VariableName)
                                   .Distinct())
        if (!variableRanges.ContainsKey(variable))
          throw new InvalidOperationException($"No ranges for variable {variable} is present");

      var code = SymbolicExpressionTreeCompiler.Compile(tree, OpCodes.MapSymbolToOpCode);
      foreach (var instr in code.Where(i => i.opCode == OpCodes.Variable)) {
        var variableTreeNode = (VariableTreeNode)instr.dynamicNode;
        instr.data = variableRanges[variableTreeNode.VariableName];
      }

      return code;
    }

    // Use ref parameter, because the tree will be iterated through recursively from the left-side branch to the right side
    // Update instructionCounter, whenever Evaluate is called
    public static Interval Evaluate(
      Instruction[] instructions, ref int instructionCounter,
      IDictionary<ISymbolicExpressionTreeNode, Interval> nodeIntervals = null,
      IDictionary<string, Interval> variableIntervals = null) {
      var currentInstr = instructions[instructionCounter];
      instructionCounter++;
      Interval result;

      switch (currentInstr.opCode) {
        case OpCodes.Variable: {
            var variableTreeNode = (VariableTreeNode)currentInstr.dynamicNode;
            var weightInterval = new Interval(variableTreeNode.Weight, variableTreeNode.Weight);

            Interval variableInterval;
            if (variableIntervals != null && variableIntervals.ContainsKey(variableTreeNode.VariableName))
              variableInterval = variableIntervals[variableTreeNode.VariableName];
            else
              variableInterval = (Interval)currentInstr.data;

            result = Interval.Multiply(variableInterval, weightInterval);
            break;
          }
        case OpCodes.Constant: {
            var constTreeNode = (ConstantTreeNode)currentInstr.dynamicNode;
            result = new Interval(constTreeNode.Value, constTreeNode.Value);
            break;
          }
        case OpCodes.Add: {
            result = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            for (var i = 1; i < currentInstr.nArguments; i++) {
              var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
              result = Interval.Add(result, argumentInterval);
            }

            break;
          }
        case OpCodes.Sub: {
            result = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            if (currentInstr.nArguments == 1)
              result = Interval.Multiply(new Interval(-1, -1), result);

            for (var i = 1; i < currentInstr.nArguments; i++) {
              var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
              result = Interval.Subtract(result, argumentInterval);
            }

            break;
          }
        case OpCodes.Mul: {
            result = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            for (var i = 1; i < currentInstr.nArguments; i++) {
              var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
              result = Interval.Multiply(result, argumentInterval);
            }

            break;
          }
        case OpCodes.Div: {
            result = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            if (currentInstr.nArguments == 1)
              result = Interval.Divide(new Interval(1, 1), result);

            for (var i = 1; i < currentInstr.nArguments; i++) {
              var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
              result = Interval.Divide(result, argumentInterval);
            }

            break;
          }
        case OpCodes.Sin: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.Sine(argumentInterval);
            break;
          }
        case OpCodes.Cos: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.Cosine(argumentInterval);
            break;
          }
        case OpCodes.Tan: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.Tangens(argumentInterval);
            break;
          }
        case OpCodes.Tanh: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.HyperbolicTangent(argumentInterval);
            break;
          }
        case OpCodes.Log: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.Logarithm(argumentInterval);
            break;
          }
        case OpCodes.Exp: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.Exponential(argumentInterval);
            break;
          }
        case OpCodes.Square: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.Square(argumentInterval);
            break;
          }
        case OpCodes.SquareRoot: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.SquareRoot(argumentInterval);
            break;
          }
        case OpCodes.Cube: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.Cube(argumentInterval);
            break;
          }
        case OpCodes.CubeRoot: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.CubicRoot(argumentInterval);
            break;
          }
        case OpCodes.Power: {
          var a = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
          var b = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
          // support only integer powers
          if (b.LowerBound == b.UpperBound && Math.Truncate(b.LowerBound) == b.LowerBound) {
            result = Interval.Power(a, (int)b.LowerBound);
          } else {
            throw new NotSupportedException("Interval is only supported for integer powers");
          }
          break;
        }
        case OpCodes.Absolute: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            result = Interval.Absolute(argumentInterval);
            break;
          }
        case OpCodes.AnalyticQuotient: {
            result = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
            for (var i = 1; i < currentInstr.nArguments; i++) {
              var argumentInterval = Evaluate(instructions, ref instructionCounter, nodeIntervals, variableIntervals);
              result = Interval.AnalyticQuotient(result, argumentInterval);
            }

            break;
          }
        default:
          throw new NotSupportedException(
            $"The tree contains the unknown symbol {currentInstr.dynamicNode.Symbol}");
      }

      if (!(nodeIntervals == null || nodeIntervals.ContainsKey(currentInstr.dynamicNode)))
        nodeIntervals.Add(currentInstr.dynamicNode, result);

      return result;
    }

    #endregion

    #region Helpers

    private static IDictionary<string, Interval> GetOccurringVariableRanges(
      ISymbolicExpressionTree tree, IntervalCollection variableRanges) {
      var variables = tree.IterateNodesPrefix().OfType<VariableTreeNode>().Select(v => v.VariableName).Distinct()
                          .ToList();

      return variables.ToDictionary(x => x, x => variableRanges.GetReadonlyDictionary()[x]);
    }

    #endregion
  
    public Interval GetModelBound(ISymbolicExpressionTree tree, IntervalCollection variableRanges) {
      lock (syncRoot) {
        EvaluatedSolutions++;
      }

      var occuringVariableRanges = GetOccurringVariableRanges(tree, variableRanges);
      var instructions = PrepareInterpreterState(tree, occuringVariableRanges);
      Interval resultInterval;
      var instructionCounter = 0;
      resultInterval = Evaluate(instructions, ref instructionCounter, variableIntervals: occuringVariableRanges);

      // because of numerical errors the bounds might be incorrect
      if (resultInterval.IsInfiniteOrUndefined || resultInterval.LowerBound <= resultInterval.UpperBound)
        return resultInterval;

      return new Interval(resultInterval.UpperBound, resultInterval.LowerBound);
    }

    public IDictionary<ISymbolicExpressionTreeNode, Interval> GetModelNodeBounds(
      ISymbolicExpressionTree tree, IntervalCollection variableRanges) {
      throw new NotImplementedException();
    }

    public double GetConstraintViolation(
      ISymbolicExpressionTree tree, IntervalCollection variableRanges, ShapeConstraint constraint) {
      var occuringVariableRanges = GetOccurringVariableRanges(tree, variableRanges);
      var instructions = PrepareInterpreterState(tree, occuringVariableRanges);
      var instructionCounter = 0;
      var modelBound = Evaluate(instructions, ref instructionCounter, variableIntervals: occuringVariableRanges);
      if (constraint.Interval.Contains(modelBound)) return 0.0;


      var error = 0.0;

      if (!constraint.Interval.Contains(modelBound.LowerBound)) {
        error += Math.Abs(modelBound.LowerBound - constraint.Interval.LowerBound);
      }

      if (!constraint.Interval.Contains(modelBound.UpperBound)) {
        error += Math.Abs(modelBound.UpperBound - constraint.Interval.UpperBound);
      }

      return error;
    }


    public bool IsCompatible(ISymbolicExpressionTree tree) {
      var containsUnknownSymbols = (
        from n in tree.Root.GetSubtree(0).IterateNodesPrefix()
        where
          !(n.Symbol is Variable) &&
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
          !(n.Symbol is Power) &&
          !(n.Symbol is Absolute) &&
          !(n.Symbol is AnalyticQuotient)
        select n).Any();
      return !containsUnknownSymbols;
    }
  }
}