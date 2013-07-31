#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";

    private SymbolicDataAnalysisExpressionTreeInterpreter interpreter;

    public override bool CanChangeName {
      get { return false; }
    }

    public override bool CanChangeDescription {
      get { return false; }
    }

    #region parameter properties
    public IValueParameter<BoolValue> CheckExpressionsWithIntervalArithmeticParameter {
      get { return (IValueParameter<BoolValue>)Parameters[CheckExpressionsWithIntervalArithmeticParameterName]; }
    }

    public IValueParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (IValueParameter<IntValue>)Parameters[EvaluatedSolutionsParameterName]; }
    }
    #endregion

    #region properties
    public BoolValue CheckExpressionsWithIntervalArithmetic {
      get { return CheckExpressionsWithIntervalArithmeticParameter.Value; }
      set { CheckExpressionsWithIntervalArithmeticParameter.Value = value; }
    }
    public IntValue EvaluatedSolutions {
      get { return EvaluatedSolutionsParameter.Value; }
      set { EvaluatedSolutionsParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionTreeLinearInterpreter(bool deserializing)
      : base(deserializing) {
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
      Parameters.Add(new ValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
      interpreter = new SymbolicDataAnalysisExpressionTreeInterpreter();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (interpreter == null) interpreter = new SymbolicDataAnalysisExpressionTreeInterpreter();
    }

    #region IStatefulItem
    public void InitializeState() {
      EvaluatedSolutions.Value = 0;
    }

    public void ClearState() { }
    #endregion

    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows) {
      if (CheckExpressionsWithIntervalArithmetic.Value)
        throw new NotSupportedException("Interval arithmetic is not yet supported in the symbolic data analysis interpreter.");

      lock (EvaluatedSolutions) {
        EvaluatedSolutions.Value++; // increment the evaluated solutions counter
      }

      var code = SymbolicExpressionTreeLinearCompiler.Compile(tree, OpCodes.MapSymbolToOpCode);
      PrepareInstructions(code, dataset);
      return rows.Select(row => Evaluate(dataset, row, code));
    }

    private double Evaluate(Dataset dataset, int row, LinearInstruction[] code) {
      for (int i = code.Length - 1; i >= 0; --i) {
        if (code[i].skip) continue;
        #region opcode switch
        var instr = code[i];
        switch (instr.opCode) {
          case OpCodes.Variable: {
              if (row < 0 || row >= dataset.Rows) instr.value = double.NaN;
              var variableTreeNode = (VariableTreeNode)instr.dynamicNode;
              instr.value = ((IList<double>)instr.data)[row] * variableTreeNode.Weight;
            }
            break;
          case OpCodes.LagVariable: {
              var laggedVariableTreeNode = (LaggedVariableTreeNode)instr.dynamicNode;
              int actualRow = row + laggedVariableTreeNode.Lag;
              if (actualRow < 0 || actualRow >= dataset.Rows)
                instr.value = double.NaN;
              else
                instr.value = ((IList<double>)instr.data)[actualRow] * laggedVariableTreeNode.Weight;
            }
            break;
          case OpCodes.VariableCondition: {
              if (row < 0 || row >= dataset.Rows) instr.value = double.NaN;
              var variableConditionTreeNode = (VariableConditionTreeNode)instr.dynamicNode;
              double variableValue = ((IList<double>)instr.data)[row];
              double x = variableValue - variableConditionTreeNode.Threshold;
              double p = 1 / (1 + Math.Exp(-variableConditionTreeNode.Slope * x));

              double trueBranch = code[instr.childIndex].value;
              double falseBranch = code[instr.childIndex + 1].value;

              instr.value = trueBranch * p + falseBranch * (1 - p);
            }
            break;
          case OpCodes.Add: {
              double s = code[instr.childIndex].value;
              for (int j = 1; j != instr.nArguments; ++j) {
                s += code[instr.childIndex + j].value;
              }
              instr.value = s;
            }
            break;
          case OpCodes.Sub: {
              double s = code[instr.childIndex].value;
              for (int j = 1; j != instr.nArguments; ++j) {
                s -= code[instr.childIndex + j].value;
              }
              if (instr.nArguments == 1) s = -s;
              instr.value = s;
            }
            break;
          case OpCodes.Mul: {
              double p = code[instr.childIndex].value;
              for (int j = 1; j != instr.nArguments; ++j) {
                p *= code[instr.childIndex + j].value;
              }
              instr.value = p;
            }
            break;
          case OpCodes.Div: {
              double p = code[instr.childIndex].value;
              for (int j = 1; j != instr.nArguments; ++j) {
                p /= code[instr.childIndex + j].value;
              }
              if (instr.nArguments == 1) p = 1.0 / p;
              instr.value = p;
            }
            break;
          case OpCodes.Average: {
              double s = code[instr.childIndex].value;
              for (int j = 1; j != instr.nArguments; ++j) {
                s += code[instr.childIndex + j].value;
              }
              instr.value = s / instr.nArguments;
            }
            break;
          case OpCodes.Cos: {
              instr.value = Math.Cos(code[instr.childIndex].value);
            }
            break;
          case OpCodes.Sin: {
              instr.value = Math.Sin(code[instr.childIndex].value);
            }
            break;
          case OpCodes.Tan: {
              instr.value = Math.Tan(code[instr.childIndex].value);
            }
            break;
          case OpCodes.Square: {
              instr.value = Math.Pow(code[instr.childIndex].value, 2);
            }
            break;
          case OpCodes.Power: {
              double x = code[instr.childIndex].value;
              double y = Math.Round(code[instr.childIndex + 1].value);
              instr.value = Math.Pow(x, y);
            }
            break;
          case OpCodes.SquareRoot: {
              instr.value = Math.Sqrt(code[instr.childIndex].value);
            }
            break;
          case OpCodes.Root: {
              double x = code[instr.childIndex].value;
              double y = code[instr.childIndex + 1].value;
              instr.value = Math.Pow(x, 1 / y);
            }
            break;
          case OpCodes.Exp: {
              instr.value = Math.Exp(code[instr.childIndex].value);
            }
            break;
          case OpCodes.Log: {
              instr.value = Math.Log(code[instr.childIndex].value);
            }
            break;
          case OpCodes.Gamma: {
              var x = code[instr.childIndex].value;
              instr.value = double.IsNaN(x) ? double.NaN : alglib.gammafunction(x);
            }
            break;
          case OpCodes.Psi: {
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else if (x <= 0 && (Math.Floor(x) - x).IsAlmost(0)) instr.value = double.NaN;
              else instr.value = alglib.psi(x);
            }
            break;
          case OpCodes.Dawson: {
              var x = code[instr.childIndex].value;
              instr.value = double.IsNaN(x) ? double.NaN : alglib.dawsonintegral(x);
            }
            break;
          case OpCodes.ExponentialIntegralEi: {
              var x = code[instr.childIndex].value;
              instr.value = double.IsNaN(x) ? double.NaN : alglib.exponentialintegralei(x);
            }
            break;
          case OpCodes.SineIntegral: {
              double si, ci;
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else {
                alglib.sinecosineintegrals(x, out si, out ci);
                instr.value = si;
              }
            }
            break;
          case OpCodes.CosineIntegral: {
              double si, ci;
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else {
                alglib.sinecosineintegrals(x, out si, out ci);
                instr.value = ci;
              }
            }
            break;
          case OpCodes.HyperbolicSineIntegral: {
              double shi, chi;
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else {
                alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
                instr.value = shi;
              }
            }
            break;
          case OpCodes.HyperbolicCosineIntegral: {
              double shi, chi;
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else {
                alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
                instr.value = chi;
              }
            }
            break;
          case OpCodes.FresnelCosineIntegral: {
              double c = 0, s = 0;
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else {
                alglib.fresnelintegral(x, ref c, ref s);
                instr.value = c;
              }
            }
            break;
          case OpCodes.FresnelSineIntegral: {
              double c = 0, s = 0;
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else {
                alglib.fresnelintegral(x, ref c, ref s);
                instr.value = s;
              }
            }
            break;
          case OpCodes.AiryA: {
              double ai, aip, bi, bip;
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else {
                alglib.airy(x, out ai, out aip, out bi, out bip);
                instr.value = ai;
              }
            }
            break;
          case OpCodes.AiryB: {
              double ai, aip, bi, bip;
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else {
                alglib.airy(x, out ai, out aip, out bi, out bip);
                instr.value = bi;
              }
            }
            break;
          case OpCodes.Norm: {
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else instr.value = alglib.normaldistribution(x);
            }
            break;
          case OpCodes.Erf: {
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else instr.value = alglib.errorfunction(x);
            }
            break;
          case OpCodes.Bessel: {
              var x = code[instr.childIndex].value;
              if (double.IsNaN(x)) instr.value = double.NaN;
              else instr.value = alglib.besseli0(x);
            }
            break;
          case OpCodes.IfThenElse: {
              double condition = code[instr.childIndex].value;
              double result;
              if (condition > 0.0) {
                result = code[instr.childIndex + 1].value;
              } else {
                result = code[instr.childIndex + 2].value;
              }
              instr.value = result;
            }
            break;
          case OpCodes.AND: {
              double result = code[instr.childIndex].value;
              for (int j = 1; j < instr.nArguments; j++) {
                if (result > 0.0) result = code[instr.childIndex + j].value;
                else break;
              }
              instr.value = result > 0.0 ? 1.0 : -1.0;
            }
            break;
          case OpCodes.OR: {
              double result = code[instr.childIndex].value;
              for (int j = 1; j < instr.nArguments; j++) {
                if (result <= 0.0) result = code[instr.childIndex + j].value;
                else break;
              }
              instr.value = result > 0.0 ? 1.0 : -1.0;
            }
            break;
          case OpCodes.NOT: {
              instr.value = code[instr.childIndex].value > 0.0 ? -1.0 : 1.0;
            }
            break;
          case OpCodes.GT: {
              double x = code[instr.childIndex].value;
              double y = code[instr.childIndex + 1].value;
              instr.value = x > y ? 1.0 : -1.0;
            }
            break;
          case OpCodes.LT: {
              double x = code[instr.childIndex].value;
              double y = code[instr.childIndex + 1].value;
              instr.value = x < y ? 1.0 : -1.0;
            }
            break;
          case OpCodes.TimeLag:
          case OpCodes.Integral:
          case OpCodes.Derivative: {
              var state = (InterpreterState)instr.data;
              state.Reset();
              instr.value = interpreter.Evaluate(dataset, ref row, state);
            }
            break;
          default:
            var errorText = string.Format("The {0} symbol is not supported by the linear interpreter. To support this symbol, please use the SymbolicDataAnalysisExpressionTreeInterpreter.", instr.dynamicNode.Symbol.Name);
            throw new NotSupportedException(errorText);
        }
        #endregion
      }
      return code[0].value;
    }

    private static LinearInstruction[] GetPrefixSequence(LinearInstruction[] code, int startIndex) {
      var list = new List<LinearInstruction>();
      int i = startIndex;
      while (i != code.Length) {
        var instr = code[i];
        list.Add(instr);
        i = instr.nArguments > 0 ? instr.childIndex : i + 1;
      }
      return list.ToArray();
    }

    private static void PrepareInstructions(LinearInstruction[] code, Dataset dataset) {
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
            }
            break;
        }
        #endregion
      }
    }
  }
}
