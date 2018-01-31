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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SymbolicDataAnalysisExpressionTreeLinearInterpreter", "Fast linear (non-recursive) interpreter for symbolic expression trees. Does not support ADFs.")]
  public sealed class SymbolicDataAnalysisExpressionTreeLinearInterpreter : ParameterizedNamedItem, ISymbolicDataAnalysisExpressionTreeInterpreter {
    private const string CheckExpressionsWithIntervalArithmeticParameterName = "CheckExpressionsWithIntervalArithmetic";
    private const string CheckExpressionsWithIntervalArithmeticParameterDescription = "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.";
    private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";

    private readonly SymbolicDataAnalysisExpressionTreeInterpreter interpreter;

    public override bool CanChangeName {
      get { return false; }
    }

    public override bool CanChangeDescription {
      get { return false; }
    }

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

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionTreeLinearInterpreter(bool deserializing)
      : base(deserializing) {
      interpreter = new SymbolicDataAnalysisExpressionTreeInterpreter();
    }

    private SymbolicDataAnalysisExpressionTreeLinearInterpreter(SymbolicDataAnalysisExpressionTreeLinearInterpreter original, Cloner cloner)
      : base(original, cloner) {
      interpreter = cloner.Clone(original.interpreter);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeLinearInterpreter(this, cloner);
    }

    public SymbolicDataAnalysisExpressionTreeLinearInterpreter()
      : base("SymbolicDataAnalysisExpressionTreeLinearInterpreter", "Linear (non-recursive) interpreter for symbolic expression trees (does not support ADFs).") {
      Parameters.Add(new FixedValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, CheckExpressionsWithIntervalArithmeticParameterDescription, new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
      interpreter = new SymbolicDataAnalysisExpressionTreeInterpreter();
    }

    public SymbolicDataAnalysisExpressionTreeLinearInterpreter(string name, string description)
      : base(name, description) {
      Parameters.Add(new FixedValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, CheckExpressionsWithIntervalArithmeticParameterDescription, new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
      interpreter = new SymbolicDataAnalysisExpressionTreeInterpreter();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      var evaluatedSolutions = new IntValue(0);
      var checkExpressionsWithIntervalArithmetic = new BoolValue(false);
      if (Parameters.ContainsKey(EvaluatedSolutionsParameterName)) {
        var evaluatedSolutionsParameter = (IValueParameter<IntValue>)Parameters[EvaluatedSolutionsParameterName];
        evaluatedSolutions = evaluatedSolutionsParameter.Value;
        Parameters.Remove(EvaluatedSolutionsParameterName);
      }
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", evaluatedSolutions));
      if (Parameters.ContainsKey(CheckExpressionsWithIntervalArithmeticParameterName)) {
        var checkExpressionsWithIntervalArithmeticParameter = (IValueParameter<BoolValue>)Parameters[CheckExpressionsWithIntervalArithmeticParameterName];
        Parameters.Remove(CheckExpressionsWithIntervalArithmeticParameterName);
        checkExpressionsWithIntervalArithmetic = checkExpressionsWithIntervalArithmeticParameter.Value;
      }
      Parameters.Add(new FixedValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, CheckExpressionsWithIntervalArithmeticParameterDescription, checkExpressionsWithIntervalArithmetic));
    }

    #region IStatefulItem
    public void InitializeState() {
      EvaluatedSolutions = 0;
    }

    public void ClearState() { }
    #endregion

    private readonly object syncRoot = new object();
    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows) {
      if (!rows.Any()) return Enumerable.Empty<double>();
      if (CheckExpressionsWithIntervalArithmetic)
        throw new NotSupportedException("Interval arithmetic is not yet supported in the symbolic data analysis interpreter.");

      lock (syncRoot) {
        EvaluatedSolutions++; // increment the evaluated solutions counter
      }

      var code = SymbolicExpressionTreeLinearCompiler.Compile(tree, OpCodes.MapSymbolToOpCode);
      PrepareInstructions(code, dataset);
      return rows.Select(row => Evaluate(dataset, row, code));
    }

    private double Evaluate(IDataset dataset, int row, LinearInstruction[] code) {
      for (int i = code.Length - 1; i >= 0; --i) {
        if (code[i].skip) continue;
        #region opcode if
        var instr = code[i];
        if (instr.opCode == OpCodes.Variable) {
          if (row < 0 || row >= dataset.Rows) instr.value = double.NaN;
          else {
            var variableTreeNode = (VariableTreeNode)instr.dynamicNode;
            instr.value = ((IList<double>)instr.data)[row] * variableTreeNode.Weight;
          }
        } else if (instr.opCode == OpCodes.BinaryFactorVariable) {
          if (row < 0 || row >= dataset.Rows) instr.value = double.NaN;
          else {
            var factorTreeNode = instr.dynamicNode as BinaryFactorVariableTreeNode;
            instr.value = ((IList<string>)instr.data)[row] == factorTreeNode.VariableValue ? factorTreeNode.Weight : 0;
          }
        } else if (instr.opCode == OpCodes.FactorVariable) {
          if (row < 0 || row >= dataset.Rows) instr.value = double.NaN;
          else {
            var factorTreeNode = instr.dynamicNode as FactorVariableTreeNode;
            instr.value = factorTreeNode.GetValue(((IList<string>)instr.data)[row]);
          }
        } else if (instr.opCode == OpCodes.LagVariable) {
          var laggedVariableTreeNode = (LaggedVariableTreeNode)instr.dynamicNode;
          int actualRow = row + laggedVariableTreeNode.Lag;
          if (actualRow < 0 || actualRow >= dataset.Rows)
            instr.value = double.NaN;
          else
            instr.value = ((IList<double>)instr.data)[actualRow] * laggedVariableTreeNode.Weight;
        } else if (instr.opCode == OpCodes.VariableCondition) {
          if (row < 0 || row >= dataset.Rows) instr.value = double.NaN;
          var variableConditionTreeNode = (VariableConditionTreeNode)instr.dynamicNode;
          if (!variableConditionTreeNode.Symbol.IgnoreSlope) {
            double variableValue = ((IList<double>)instr.data)[row];
            double x = variableValue - variableConditionTreeNode.Threshold;
            double p = 1 / (1 + Math.Exp(-variableConditionTreeNode.Slope * x));

            double trueBranch = code[instr.childIndex].value;
            double falseBranch = code[instr.childIndex + 1].value;

            instr.value = trueBranch * p + falseBranch * (1 - p);
          } else {
            double variableValue = ((IList<double>)instr.data)[row];
            if (variableValue <= variableConditionTreeNode.Threshold) {
              instr.value = code[instr.childIndex].value;
            } else {
              instr.value = code[instr.childIndex + 1].value;
            }
          }
        } else if (instr.opCode == OpCodes.Add) {
          double s = code[instr.childIndex].value;
          for (int j = 1; j != instr.nArguments; ++j) {
            s += code[instr.childIndex + j].value;
          }
          instr.value = s;
        } else if (instr.opCode == OpCodes.Sub) {
          double s = code[instr.childIndex].value;
          for (int j = 1; j != instr.nArguments; ++j) {
            s -= code[instr.childIndex + j].value;
          }
          if (instr.nArguments == 1) s = -s;
          instr.value = s;
        } else if (instr.opCode == OpCodes.Mul) {
          double p = code[instr.childIndex].value;
          for (int j = 1; j != instr.nArguments; ++j) {
            p *= code[instr.childIndex + j].value;
          }
          instr.value = p;
        } else if (instr.opCode == OpCodes.Div) {
          double p = code[instr.childIndex].value;
          for (int j = 1; j != instr.nArguments; ++j) {
            p /= code[instr.childIndex + j].value;
          }
          if (instr.nArguments == 1) p = 1.0 / p;
          instr.value = p;
        } else if (instr.opCode == OpCodes.Average) {
          double s = code[instr.childIndex].value;
          for (int j = 1; j != instr.nArguments; ++j) {
            s += code[instr.childIndex + j].value;
          }
          instr.value = s / instr.nArguments;
        } else if (instr.opCode == OpCodes.Cos) {
          instr.value = Math.Cos(code[instr.childIndex].value);
        } else if (instr.opCode == OpCodes.Sin) {
          instr.value = Math.Sin(code[instr.childIndex].value);
        } else if (instr.opCode == OpCodes.Tan) {
          instr.value = Math.Tan(code[instr.childIndex].value);
        } else if (instr.opCode == OpCodes.Square) {
          instr.value = Math.Pow(code[instr.childIndex].value, 2);
        } else if (instr.opCode == OpCodes.Power) {
          double x = code[instr.childIndex].value;
          double y = Math.Round(code[instr.childIndex + 1].value);
          instr.value = Math.Pow(x, y);
        } else if (instr.opCode == OpCodes.SquareRoot) {
          instr.value = Math.Sqrt(code[instr.childIndex].value);
        } else if (instr.opCode == OpCodes.Root) {
          double x = code[instr.childIndex].value;
          double y = Math.Round(code[instr.childIndex + 1].value);
          instr.value = Math.Pow(x, 1 / y);
        } else if (instr.opCode == OpCodes.Exp) {
          instr.value = Math.Exp(code[instr.childIndex].value);
        } else if (instr.opCode == OpCodes.Log) {
          instr.value = Math.Log(code[instr.childIndex].value);
        } else if (instr.opCode == OpCodes.Gamma) {
          var x = code[instr.childIndex].value;
          instr.value = double.IsNaN(x) ? double.NaN : alglib.gammafunction(x);
        } else if (instr.opCode == OpCodes.Psi) {
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else if (x <= 0 && (Math.Floor(x) - x).IsAlmost(0)) instr.value = double.NaN;
          else instr.value = alglib.psi(x);
        } else if (instr.opCode == OpCodes.Dawson) {
          var x = code[instr.childIndex].value;
          instr.value = double.IsNaN(x) ? double.NaN : alglib.dawsonintegral(x);
        } else if (instr.opCode == OpCodes.ExponentialIntegralEi) {
          var x = code[instr.childIndex].value;
          instr.value = double.IsNaN(x) ? double.NaN : alglib.exponentialintegralei(x);
        } else if (instr.opCode == OpCodes.SineIntegral) {
          double si, ci;
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else {
            alglib.sinecosineintegrals(x, out si, out ci);
            instr.value = si;
          }
        } else if (instr.opCode == OpCodes.CosineIntegral) {
          double si, ci;
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else {
            alglib.sinecosineintegrals(x, out si, out ci);
            instr.value = ci;
          }
        } else if (instr.opCode == OpCodes.HyperbolicSineIntegral) {
          double shi, chi;
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else {
            alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
            instr.value = shi;
          }
        } else if (instr.opCode == OpCodes.HyperbolicCosineIntegral) {
          double shi, chi;
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else {
            alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
            instr.value = chi;
          }
        } else if (instr.opCode == OpCodes.FresnelCosineIntegral) {
          double c = 0, s = 0;
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else {
            alglib.fresnelintegral(x, ref c, ref s);
            instr.value = c;
          }
        } else if (instr.opCode == OpCodes.FresnelSineIntegral) {
          double c = 0, s = 0;
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else {
            alglib.fresnelintegral(x, ref c, ref s);
            instr.value = s;
          }
        } else if (instr.opCode == OpCodes.AiryA) {
          double ai, aip, bi, bip;
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else {
            alglib.airy(x, out ai, out aip, out bi, out bip);
            instr.value = ai;
          }
        } else if (instr.opCode == OpCodes.AiryB) {
          double ai, aip, bi, bip;
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else {
            alglib.airy(x, out ai, out aip, out bi, out bip);
            instr.value = bi;
          }
        } else if (instr.opCode == OpCodes.Norm) {
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else instr.value = alglib.normaldistribution(x);
        } else if (instr.opCode == OpCodes.Erf) {
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else instr.value = alglib.errorfunction(x);
        } else if (instr.opCode == OpCodes.Bessel) {
          var x = code[instr.childIndex].value;
          if (double.IsNaN(x)) instr.value = double.NaN;
          else instr.value = alglib.besseli0(x);
        } else if (instr.opCode == OpCodes.IfThenElse) {
          double condition = code[instr.childIndex].value;
          double result;
          if (condition > 0.0) {
            result = code[instr.childIndex + 1].value;
          } else {
            result = code[instr.childIndex + 2].value;
          }
          instr.value = result;
        } else if (instr.opCode == OpCodes.AND) {
          double result = code[instr.childIndex].value;
          for (int j = 1; j < instr.nArguments; j++) {
            if (result > 0.0) result = code[instr.childIndex + j].value;
            else break;
          }
          instr.value = result > 0.0 ? 1.0 : -1.0;
        } else if (instr.opCode == OpCodes.OR) {
          double result = code[instr.childIndex].value;
          for (int j = 1; j < instr.nArguments; j++) {
            if (result <= 0.0) result = code[instr.childIndex + j].value;
            else break;
          }
          instr.value = result > 0.0 ? 1.0 : -1.0;
        } else if (instr.opCode == OpCodes.NOT) {
          instr.value = code[instr.childIndex].value > 0.0 ? -1.0 : 1.0;
        } else if (instr.opCode == OpCodes.XOR) {
          int positiveSignals = 0;
          for (int j = 0; j < instr.nArguments; j++) {
            if (code[instr.childIndex + j].value > 0.0) positiveSignals++;
          }
          instr.value = positiveSignals % 2 != 0 ? 1.0 : -1.0;
        } else if (instr.opCode == OpCodes.GT) {
          double x = code[instr.childIndex].value;
          double y = code[instr.childIndex + 1].value;
          instr.value = x > y ? 1.0 : -1.0;
        } else if (instr.opCode == OpCodes.LT) {
          double x = code[instr.childIndex].value;
          double y = code[instr.childIndex + 1].value;
          instr.value = x < y ? 1.0 : -1.0;
        } else if (instr.opCode == OpCodes.TimeLag || instr.opCode == OpCodes.Derivative || instr.opCode == OpCodes.Integral) {
          var state = (InterpreterState)instr.data;
          state.Reset();
          instr.value = interpreter.Evaluate(dataset, ref row, state);
        } else {
          var errorText = string.Format("The {0} symbol is not supported by the linear interpreter. To support this symbol, please use the SymbolicDataAnalysisExpressionTreeInterpreter.", instr.dynamicNode.Symbol.Name);
          throw new NotSupportedException(errorText);
        }
        #endregion
      }
      return code[0].value;
    }

    private static LinearInstruction[] GetPrefixSequence(LinearInstruction[] code, int startIndex) {
      var s = new Stack<int>();
      var list = new List<LinearInstruction>();
      s.Push(startIndex);
      while (s.Any()) {
        int i = s.Pop();
        var instr = code[i];
        // push instructions in reverse execution order
        for (int j = instr.nArguments - 1; j >= 0; j--) s.Push(instr.childIndex + j);
        list.Add(instr);
      }
      return list.ToArray();
    }

    public static void PrepareInstructions(LinearInstruction[] code, IDataset dataset) {
      for (int i = 0; i != code.Length; ++i) {
        var instr = code[i];
        #region opcode switch
        switch (instr.opCode) {
          case OpCodes.Constant: {
              var constTreeNode = (ConstantTreeNode)instr.dynamicNode;
              instr.value = constTreeNode.Value;
              instr.skip = true; // the value is already set so this instruction should be skipped in the evaluation phase
            }
            break;
          case OpCodes.Variable: {
              var variableTreeNode = (VariableTreeNode)instr.dynamicNode;
              instr.data = dataset.GetReadOnlyDoubleValues(variableTreeNode.VariableName);
            }
            break;
          case OpCodes.BinaryFactorVariable: {
              var factorVariableTreeNode = instr.dynamicNode as BinaryFactorVariableTreeNode;
              instr.data = dataset.GetReadOnlyStringValues(factorVariableTreeNode.VariableName);
            }
            break;
          case OpCodes.FactorVariable: {
              var factorVariableTreeNode = instr.dynamicNode as FactorVariableTreeNode;
              instr.data = dataset.GetReadOnlyStringValues(factorVariableTreeNode.VariableName);
            }
            break;
          case OpCodes.LagVariable: {
              var laggedVariableTreeNode = (LaggedVariableTreeNode)instr.dynamicNode;
              instr.data = dataset.GetReadOnlyDoubleValues(laggedVariableTreeNode.VariableName);
            }
            break;
          case OpCodes.VariableCondition: {
              var variableConditionTreeNode = (VariableConditionTreeNode)instr.dynamicNode;
              instr.data = dataset.GetReadOnlyDoubleValues(variableConditionTreeNode.VariableName);
            }
            break;
          case OpCodes.TimeLag:
          case OpCodes.Integral:
          case OpCodes.Derivative: {
              var seq = GetPrefixSequence(code, i);
              var interpreterState = new InterpreterState(seq, 0);
              instr.data = interpreterState;
              for (int j = 1; j != seq.Length; ++j)
                seq[j].skip = true;
              break;
            }
        }
        #endregion
      }
    }
  }
}
