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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("C27A8B7C-6855-4850-A31E-EACFEED68C1C")]
  [Item("Modal Arithmetic Bounds Estimator", "Interpreter for calculation of inner and outer approximation of symbolic models")]
  public sealed class ModalArithmeticBoundEstimator : ParameterizedNamedItem, IBoundsEstimator {
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
    private ModalArithmeticBoundEstimator(StorableConstructorFlag _) : base(_) { }
    private ModalArithmeticBoundEstimator(ModalArithmeticBoundEstimator original, Cloner cloner) : base(original, cloner) { }
    public ModalArithmeticBoundEstimator() : base("Modal Arithmetic Bounds Estimator", "Estimates the bounds of the model with modal arithmetic.") {
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName,
        "A counter for the total number of solutions the estimator has evaluated.", new IntValue(0)));
    }
    #endregion

    #region IStatefulItem Members

    private readonly object syncRoot = new object();

    public void InitializeState() {
      EvaluatedSolutions = 0;
    }

    public void ClearState() { }

    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ModalArithmeticBoundEstimator(this, cloner);
    }

    #region Evaluation
    private static Instruction[] PrepareInterpreterState(
  ISymbolicExpressionTree tree,
  IDictionary<string, ModalInterval> variableRanges) {
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
    public static ModalInterval Evaluate(
  Instruction[] instructions, ref int instructionCounter,
  IDictionary<string, ModalInterval> variableIntervals = null) {
      var currentInstr = instructions[instructionCounter];
      instructionCounter++;
      ModalInterval result;

      switch (currentInstr.opCode) {
        case OpCodes.Variable: {
            var variableTreeNode = (VariableTreeNode)currentInstr.dynamicNode;
            var weightInterval = new ModalInterval(variableTreeNode.Weight, variableTreeNode.Weight);

            ModalInterval variableInterval;
            if (variableIntervals != null && variableIntervals.ContainsKey(variableTreeNode.VariableName))
              variableInterval = variableIntervals[variableTreeNode.VariableName];
            else
              variableInterval = (ModalInterval)currentInstr.data;

            result = ModalInterval.Multiply(variableInterval, weightInterval);
            break;
          }
        case OpCodes.Constant: // fall through
        case OpCodes.Number: {
            var numericTreeNode = (INumericTreeNode)currentInstr.dynamicNode;
            result = new ModalInterval(numericTreeNode.Value, numericTreeNode.Value);
            break;
          }
        case OpCodes.Add: {
            result = Evaluate(instructions, ref instructionCounter, variableIntervals);
            for (var i = 1; i < currentInstr.nArguments; i++) {
              var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
              result = ModalInterval.Add(result, argumentInterval);
            }

            break;
          }
        case OpCodes.Sub: {
            result = Evaluate(instructions, ref instructionCounter, variableIntervals);
            if (currentInstr.nArguments == 1)
              result = ModalInterval.Multiply(new ModalInterval(-1, -1), result);

            for (var i = 1; i < currentInstr.nArguments; i++) {
              var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
              result = ModalInterval.Subtract(result, argumentInterval);
            }

            break;
          }
        case OpCodes.Mul: {
            result = Evaluate(instructions, ref instructionCounter, variableIntervals);
            for (var i = 1; i < currentInstr.nArguments; i++) {
              var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
              result = ModalInterval.Multiply(result, argumentInterval);
            }

            break;
          }
        case OpCodes.Div: {
            result = Evaluate(instructions, ref instructionCounter, variableIntervals);
            if (currentInstr.nArguments == 1)
              result = ModalInterval.Divide(new ModalInterval(1, 1), result);

            for (var i = 1; i < currentInstr.nArguments; i++) {
              var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
              result = ModalInterval.Divide(result, argumentInterval);
            }

            break;
          }
        case OpCodes.Sin: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.Sine(argumentInterval);
            break;
          }
        case OpCodes.Cos: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.Cosine(argumentInterval);
            break;
          }
        case OpCodes.Tan: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.Tangens(argumentInterval);
            break;
          }
        case OpCodes.Tanh: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.HyperbolicTangent(argumentInterval);
            break;
          }
        case OpCodes.Log: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.Logarithm(argumentInterval);
            break;
          }
        case OpCodes.Exp: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.Exponential(argumentInterval);
            break;
          }
        case OpCodes.Square: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.Square(argumentInterval);
            break;
          }
        case OpCodes.SquareRoot: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.SquareRoot(argumentInterval);
            break;
          }
        case OpCodes.Cube: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.Cube(argumentInterval);
            break;
          }
        case OpCodes.CubeRoot: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.CubicRoot(argumentInterval);
            break;
          }
        case OpCodes.Power: {
            var a = Evaluate(instructions, ref instructionCounter, variableIntervals);
            var b = Evaluate(instructions, ref instructionCounter, variableIntervals);
            // support only integer powers
            if (b.LowerBound == b.UpperBound && Math.Truncate(b.LowerBound) == b.LowerBound) {
              result = ModalInterval.Power(a, (int)b.LowerBound);
            } else {
              throw new NotSupportedException("Interval is only supported for integer powers");
            }
            break;
          }
        case OpCodes.Absolute: {
            var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
            result = ModalInterval.Absolute(argumentInterval);
            break;
          }
        case OpCodes.AnalyticQuotient: {
            result = Evaluate(instructions, ref instructionCounter, variableIntervals);
            for (var i = 1; i < currentInstr.nArguments; i++) {
              var argumentInterval = Evaluate(instructions, ref instructionCounter, variableIntervals);
              result = ModalInterval.AnalyticQuotient(result, argumentInterval);
            }

            break;
          }
        case OpCodes.SubFunction: {
            result = Evaluate(instructions, ref instructionCounter, variableIntervals);
            break;
          }
        default:
          throw new NotSupportedException(
            $"The tree contains the unknown symbol {currentInstr.dynamicNode.Symbol}");
      }

      return result;
    }
    #endregion  

    public ModalInterval OuterApproximation(ISymbolicExpressionTree tree, IntervalCollection variableRanges) {
      var occurrenceTree = GenerateOccurrenceTree(tree, variableRanges);
      //Get all variables which occur multiple times in the tree
      var multipleOccurrenceVariables = GetOccurrencVariables(tree);

      //Call dualizer for each variable occurring more than one time
      foreach (var variable in multipleOccurrenceVariables) {
        Dualizer(tree, occurrenceTree.Item1, variable, variableRanges, occurrenceTree.Item2);
      }
      var instructions = PrepareInterpreterState(occurrenceTree.Item1, occurrenceTree.Item2);
      var instructionsCounter = 0;

      var result = Evaluate(instructions, ref instructionsCounter, occurrenceTree.Item2);
      return result;
    }

    public ModalInterval InnerApproximation(ISymbolicExpressionTree tree, IntervalCollection variableRanges) {
      var occurrenceTree = GenerateOccurrenceTree(tree, variableRanges);
      //Get all variables which occur multiple times in the tree
      var multipleOccurrenceVariables = GetOccurrencVariables(tree);

      //Call dualizer for each variable occurring more than one time
      foreach (var variable in multipleOccurrenceVariables) {
        var totalMonotonic = Dualizer(tree, occurrenceTree.Item1, variable, variableRanges, occurrenceTree.Item2);
        if (!totalMonotonic) {
          var interval = variableRanges.GetInterval(variable);
          var middle = ModalInterval.Mid(new ModalInterval(interval.LowerBound, interval.UpperBound));
          foreach (var x in occurrenceTree.Item2.Keys.Where(x => x.StartsWith(variable + "_")).ToList()) {
            occurrenceTree.Item2[x] = middle;
          }
        }
      }
      var instructions = PrepareInterpreterState(occurrenceTree.Item1, occurrenceTree.Item2);
      var instructionsCounter = 0;

      var result = Evaluate(instructions, ref instructionsCounter, occurrenceTree.Item2);
      return result;
    }

    //Generate the occurrence tree KTree and returns the tree and the corresponding variable ranges with modal intervals
    private Tuple<ISymbolicExpressionTree, Dictionary<string, ModalInterval>> GenerateOccurrenceTree(ISymbolicExpressionTree tree, IntervalCollection variableRanges) {
      var kTree = (ISymbolicExpressionTree)tree.Clone();
      var variables = tree.IterateNodesPrefix().OfType<VariableTreeNode>().Select(x => x.VariableName).Distinct().ToList();
      var modalRanges = new Dictionary<string, ModalInterval>();

      foreach (var variable in variables) {
        var interval = variableRanges.GetInterval(variable);
        var count = 1;
        foreach (var node in kTree.IterateNodesPrefix().OfType<VariableTreeNode>().Where(x => x.VariableName == variable)) {
          var newName = node.VariableName + "_" + count;
          node.VariableName = newName;
          count++;

          modalRanges.Add(newName, new ModalInterval(interval.LowerBound, interval.UpperBound));
        }
      }

      return Tuple.Create(kTree, modalRanges);
    }

    //Returns true if the dualizer transformed the tree kt, returns false if there wos no transformation
    public bool Dualizer(ISymbolicExpressionTree tree, ISymbolicExpressionTree kTree, string variable, IntervalCollection variableRanges, Dictionary<string, ModalInterval> modalIntervals) {
      var derivedTree = DerivativeCalculator.Derive(tree, variable);
      var intervalEstimator = new IntervalArithBoundsEstimator();
      var outerDict = new Dictionary<string, ModalInterval>();
      var derivedResult = intervalEstimator.GetModelBound(derivedTree, variableRanges);
      var newModalintervals = new Dictionary<string, ModalInterval>();

      //Check if the given variable is monotonic
      if (IsMonotonic(derivedResult)) {
        //Get all occurrences of variable in the occurrence tree and evaluate the monotonicity of each
        foreach (var node in kTree.IterateNodesPrefix().OfType<VariableTreeNode>().Where(x => x.VariableName.StartsWith(variable + "_"))) {
          derivedTree = DerivativeCalculator.Derive(kTree, node.VariableName);
          var instruction = 0;
          var evaluated = Evaluate(PrepareInterpreterState(derivedTree, modalIntervals), ref instruction, modalIntervals);
          //!TODO check if we can check this for modalIntervals instead of change to normal intervals
          var evaluatedInterval = new Interval(evaluated.LowerBound, evaluated.UpperBound);
          if (!IsMonotonic(evaluatedInterval)) {
            return false;
          }
          if (GetSign(derivedResult) != GetSign(evaluatedInterval)) {
            newModalintervals.Add(node.VariableName, ModalInterval.Dual(modalIntervals[node.VariableName]));
          }
        }
        //if the variable is total monotonic change the occurrence of xij accordingly
        foreach (var kvp in newModalintervals) {
          modalIntervals[kvp.Key] = kvp.Value;
        }
        return true;
      }

      return false;
    }

    //Checks wheater an interval is monotinic increasing or decreasing
    private bool IsMonotonic(Interval interval) {
      return GetSign(interval) == -1 || GetSign(interval) == 1;
    }

    //Returns -1 if it is monotnic decreasing, 1 if it is monotonic increasing and 0 if its not monotonic
    private int GetSign(Interval interval) {
      var dec = new Interval(double.NegativeInfinity, 0);
      var inc = new Interval(0, double.PositiveInfinity);

      if (dec.Contains(interval)) return -1;
      if (inc.Contains(interval)) return 1;

      return 0;
    }

    public double GetConstraintViolation(ISymbolicExpressionTree tree, IntervalCollection variableRanges, ShapeConstraint constraint) {
      var modelBound = ModalInterval.ToInterval(OuterApproximation(tree, variableRanges));
      if (constraint.Interval.Contains(modelBound)) return 0.0;

      var error = 0.0;

      if (!constraint.Interval.Contains(modelBound.LowerBound)) {
        error = Math.Abs(modelBound.LowerBound - constraint.Interval.LowerBound);
      }
      if (!constraint.Interval.Contains(modelBound.UpperBound)) {
        error = Math.Abs(modelBound.UpperBound - constraint.Interval.UpperBound);
      }
      return error;
    }

    public Interval GetModelBound(ISymbolicExpressionTree tree, IntervalCollection variableRanges) {
      var bounds = OuterApproximation(tree, variableRanges);

      return new Interval(bounds.LowerBound, bounds.UpperBound);
    }

    public IDictionary<ISymbolicExpressionTreeNode, Interval> GetModelNodeBounds(ISymbolicExpressionTree tree, IntervalCollection variableRanges) {
      throw new NotImplementedException();
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
          !(n.Symbol is Power) &&
          !(n.Symbol is Absolute) &&
          !(n.Symbol is AnalyticQuotient) &&
          !(n.Symbol is SubFunctionSymbol)
        select n).Any();
      return !containsUnknownSymbols;
    }

    private List<string> GetOccurrencVariables(ISymbolicExpressionTree tree) => tree.IterateNodesPrefix().OfType<VariableTreeNode>().GroupBy(x => x.VariableName).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
  }
}
