#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("FB94F333-B32A-44FB-A561-CBDE76693D20")]
  [Item("SymbolicDataAnalysisExpressionTreeInterpreter", "Interpreter for symbolic expression trees including automatically defined functions.")]
  public class SymbolicDataAnalysisExpressionTreeInterpreter : ParameterizedNamedItem,
    ISymbolicDataAnalysisExpressionTreeInterpreter {
    private const string CheckExpressionsWithIntervalArithmeticParameterName = "CheckExpressionsWithIntervalArithmetic";
    private const string CheckExpressionsWithIntervalArithmeticParameterDescription = "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.";
    private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";

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
    protected SymbolicDataAnalysisExpressionTreeInterpreter(StorableConstructorFlag _) : base(_) { }

    protected SymbolicDataAnalysisExpressionTreeInterpreter(SymbolicDataAnalysisExpressionTreeInterpreter original,
      Cloner cloner)
      : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeInterpreter(this, cloner);
    }

    public SymbolicDataAnalysisExpressionTreeInterpreter()
      : base("SymbolicDataAnalysisExpressionTreeInterpreter", "Interpreter for symbolic expression trees including automatically defined functions.") {
      Parameters.Add(new FixedValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
    }

    protected SymbolicDataAnalysisExpressionTreeInterpreter(string name, string description)
      : base(name, description) {
      Parameters.Add(new FixedValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
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
    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset,
      IEnumerable<int> rows) {
      if (CheckExpressionsWithIntervalArithmetic) {
        throw new NotSupportedException("Interval arithmetic is not yet supported in the symbolic data analysis interpreter.");
      }

      lock (syncRoot) {
        EvaluatedSolutions++; // increment the evaluated solutions counter
      }
      var state = PrepareInterpreterState(tree, dataset);

      foreach (var rowEnum in rows) {
        int row = rowEnum;
        yield return Evaluate(dataset, ref row, state);
        state.Reset();
      }
    }

    private static InterpreterState PrepareInterpreterState(ISymbolicExpressionTree tree, IDataset dataset) {
      Instruction[] code = SymbolicExpressionTreeCompiler.Compile(tree, OpCodes.MapSymbolToOpCode);
      int necessaryArgStackSize = 0;
      foreach (Instruction instr in code) {
        if (instr.opCode == OpCodes.Variable) {
          var variableTreeNode = (VariableTreeNode)instr.dynamicNode;
          instr.data = dataset.GetReadOnlyDoubleValues(variableTreeNode.VariableName);
        } else if (instr.opCode == OpCodes.FactorVariable) {
          var factorTreeNode = instr.dynamicNode as FactorVariableTreeNode;
          instr.data = dataset.GetReadOnlyStringValues(factorTreeNode.VariableName);
        } else if (instr.opCode == OpCodes.BinaryFactorVariable) {
          var factorTreeNode = instr.dynamicNode as BinaryFactorVariableTreeNode;
          instr.data = dataset.GetReadOnlyStringValues(factorTreeNode.VariableName);
        } else if (instr.opCode == OpCodes.LagVariable) {
          var laggedVariableTreeNode = (LaggedVariableTreeNode)instr.dynamicNode;
          instr.data = dataset.GetReadOnlyDoubleValues(laggedVariableTreeNode.VariableName);
        } else if (instr.opCode == OpCodes.VariableCondition) {
          var variableConditionTreeNode = (VariableConditionTreeNode)instr.dynamicNode;
          instr.data = dataset.GetReadOnlyDoubleValues(variableConditionTreeNode.VariableName);
        } else if (instr.opCode == OpCodes.Call) {
          necessaryArgStackSize += instr.nArguments + 1;
        }
      }
      return new InterpreterState(code, necessaryArgStackSize);
    }

    public virtual double Evaluate(IDataset dataset, ref int row, InterpreterState state) {
      Instruction currentInstr = state.NextInstruction();
      switch (currentInstr.opCode) {
        case OpCodes.Add: {
            double s = Evaluate(dataset, ref row, state);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              s += Evaluate(dataset, ref row, state);
            }
            return s;
          }
        case OpCodes.Sub: {
            double s = Evaluate(dataset, ref row, state);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              s -= Evaluate(dataset, ref row, state);
            }
            if (currentInstr.nArguments == 1) { s = -s; }
            return s;
          }
        case OpCodes.Mul: {
            double p = Evaluate(dataset, ref row, state);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              p *= Evaluate(dataset, ref row, state);
            }
            return p;
          }
        case OpCodes.Div: {
            double p = Evaluate(dataset, ref row, state);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              p /= Evaluate(dataset, ref row, state);
            }
            if (currentInstr.nArguments == 1) { p = 1.0 / p; }
            return p;
          }
        case OpCodes.Average: {
            double sum = Evaluate(dataset, ref row, state);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              sum += Evaluate(dataset, ref row, state);
            }
            return sum / currentInstr.nArguments;
          }
        case OpCodes.Absolute: {
            return Math.Abs(Evaluate(dataset, ref row, state));
          }
        case OpCodes.Tanh: {
            return Math.Tanh(Evaluate(dataset, ref row, state));
          }
        case OpCodes.Cos: {
            return Math.Cos(Evaluate(dataset, ref row, state));
          }
        case OpCodes.Sin: {
            return Math.Sin(Evaluate(dataset, ref row, state));
          }
        case OpCodes.Tan: {
            return Math.Tan(Evaluate(dataset, ref row, state));
          }
        case OpCodes.Square: {
            return Math.Pow(Evaluate(dataset, ref row, state), 2);
          }
        case OpCodes.Cube: {
            return Math.Pow(Evaluate(dataset, ref row, state), 3);
          }
        case OpCodes.Power: {
            double x = Evaluate(dataset, ref row, state);
            double y = Math.Round(Evaluate(dataset, ref row, state));
            return Math.Pow(x, y);
          }
        case OpCodes.SquareRoot: {
            return Math.Sqrt(Evaluate(dataset, ref row, state));
          }
        case OpCodes.CubeRoot: {
            return Math.Pow(Evaluate(dataset, ref row, state), 1.0 / 3.0);
          }
        case OpCodes.Root: {
            double x = Evaluate(dataset, ref row, state);
            double y = Math.Round(Evaluate(dataset, ref row, state));
            return Math.Pow(x, 1 / y);
          }
        case OpCodes.Exp: {
            return Math.Exp(Evaluate(dataset, ref row, state));
          }
        case OpCodes.Log: {
            return Math.Log(Evaluate(dataset, ref row, state));
          }
        case OpCodes.Gamma: {
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) { return double.NaN; } else { return alglib.gammafunction(x); }
          }
        case OpCodes.Psi: {
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else if (x <= 0 && (Math.Floor(x) - x).IsAlmost(0)) return double.NaN;
            return alglib.psi(x);
          }
        case OpCodes.Dawson: {
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) { return double.NaN; }
            return alglib.dawsonintegral(x);
          }
        case OpCodes.ExponentialIntegralEi: {
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) { return double.NaN; }
            return alglib.exponentialintegralei(x);
          }
        case OpCodes.SineIntegral: {
            double si, ci;
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else {
              alglib.sinecosineintegrals(x, out si, out ci);
              return si;
            }
          }
        case OpCodes.CosineIntegral: {
            double si, ci;
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else {
              alglib.sinecosineintegrals(x, out si, out ci);
              return ci;
            }
          }
        case OpCodes.HyperbolicSineIntegral: {
            double shi, chi;
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else {
              alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
              return shi;
            }
          }
        case OpCodes.HyperbolicCosineIntegral: {
            double shi, chi;
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else {
              alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
              return chi;
            }
          }
        case OpCodes.FresnelCosineIntegral: {
            double c = 0, s = 0;
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else {
              alglib.fresnelintegral(x, ref c, ref s);
              return c;
            }
          }
        case OpCodes.FresnelSineIntegral: {
            double c = 0, s = 0;
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else {
              alglib.fresnelintegral(x, ref c, ref s);
              return s;
            }
          }
        case OpCodes.AiryA: {
            double ai, aip, bi, bip;
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else {
              alglib.airy(x, out ai, out aip, out bi, out bip);
              return ai;
            }
          }
        case OpCodes.AiryB: {
            double ai, aip, bi, bip;
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else {
              alglib.airy(x, out ai, out aip, out bi, out bip);
              return bi;
            }
          }
        case OpCodes.Norm: {
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else return alglib.normaldistribution(x);
          }
        case OpCodes.Erf: {
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else return alglib.errorfunction(x);
          }
        case OpCodes.Bessel: {
            var x = Evaluate(dataset, ref row, state);
            if (double.IsNaN(x)) return double.NaN;
            else return alglib.besseli0(x);
          }

        case OpCodes.AnalyticQuotient: {
            var x1 = Evaluate(dataset, ref row, state);
            var x2 = Evaluate(dataset, ref row, state);
            return x1 / Math.Pow(1 + x2 * x2, 0.5);
          }
        case OpCodes.IfThenElse: {
            double condition = Evaluate(dataset, ref row, state);
            double result;
            if (condition > 0.0) {
              result = Evaluate(dataset, ref row, state); state.SkipInstructions();
            } else {
              state.SkipInstructions(); result = Evaluate(dataset, ref row, state);
            }
            return result;
          }
        case OpCodes.AND: {
            double result = Evaluate(dataset, ref row, state);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              if (result > 0.0) result = Evaluate(dataset, ref row, state);
              else {
                state.SkipInstructions();
              }
            }
            return result > 0.0 ? 1.0 : -1.0;
          }
        case OpCodes.OR: {
            double result = Evaluate(dataset, ref row, state);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              if (result <= 0.0) result = Evaluate(dataset, ref row, state);
              else {
                state.SkipInstructions();
              }
            }
            return result > 0.0 ? 1.0 : -1.0;
          }
        case OpCodes.NOT: {
            return Evaluate(dataset, ref row, state) > 0.0 ? -1.0 : 1.0;
          }
        case OpCodes.XOR: {
            //mkommend: XOR on multiple inputs is defined as true if the number of positive signals is odd
            // this is equal to a consecutive execution of binary XOR operations.
            int positiveSignals = 0;
            for (int i = 0; i < currentInstr.nArguments; i++) {
              if (Evaluate(dataset, ref row, state) > 0.0) { positiveSignals++; }
            }
            return positiveSignals % 2 != 0 ? 1.0 : -1.0;
          }
        case OpCodes.GT: {
            double x = Evaluate(dataset, ref row, state);
            double y = Evaluate(dataset, ref row, state);
            if (x > y) { return 1.0; } else { return -1.0; }
          }
        case OpCodes.LT: {
            double x = Evaluate(dataset, ref row, state);
            double y = Evaluate(dataset, ref row, state);
            if (x < y) { return 1.0; } else { return -1.0; }
          }
        case OpCodes.TimeLag: {
            var timeLagTreeNode = (LaggedTreeNode)currentInstr.dynamicNode;
            row += timeLagTreeNode.Lag;
            double result = Evaluate(dataset, ref row, state);
            row -= timeLagTreeNode.Lag;
            return result;
          }
        case OpCodes.Integral: {
            int savedPc = state.ProgramCounter;
            var timeLagTreeNode = (LaggedTreeNode)currentInstr.dynamicNode;
            double sum = 0.0;
            for (int i = 0; i < Math.Abs(timeLagTreeNode.Lag); i++) {
              row += Math.Sign(timeLagTreeNode.Lag);
              sum += Evaluate(dataset, ref row, state);
              state.ProgramCounter = savedPc;
            }
            row -= timeLagTreeNode.Lag;
            sum += Evaluate(dataset, ref row, state);
            return sum;
          }

        //mkommend: derivate calculation taken from: 
        //http://www.holoborodko.com/pavel/numerical-methods/numerical-derivative/smooth-low-noise-differentiators/
        //one sided smooth differentiatior, N = 4
        // y' = 1/8h (f_i + 2f_i-1, -2 f_i-3 - f_i-4)
        case OpCodes.Derivative: {
            int savedPc = state.ProgramCounter;
            double f_0 = Evaluate(dataset, ref row, state); row--;
            state.ProgramCounter = savedPc;
            double f_1 = Evaluate(dataset, ref row, state); row -= 2;
            state.ProgramCounter = savedPc;
            double f_3 = Evaluate(dataset, ref row, state); row--;
            state.ProgramCounter = savedPc;
            double f_4 = Evaluate(dataset, ref row, state);
            row += 4;

            return (f_0 + 2 * f_1 - 2 * f_3 - f_4) / 8; // h = 1
          }
        case OpCodes.Call: {
            // evaluate sub-trees
            double[] argValues = new double[currentInstr.nArguments];
            for (int i = 0; i < currentInstr.nArguments; i++) {
              argValues[i] = Evaluate(dataset, ref row, state);
            }
            // push on argument values on stack 
            state.CreateStackFrame(argValues);

            // save the pc
            int savedPc = state.ProgramCounter;
            // set pc to start of function  
            state.ProgramCounter = (ushort)currentInstr.data;
            // evaluate the function
            double v = Evaluate(dataset, ref row, state);

            // delete the stack frame
            state.RemoveStackFrame();

            // restore the pc => evaluation will continue at point after my subtrees  
            state.ProgramCounter = savedPc;
            return v;
          }
        case OpCodes.Arg: {
            return state.GetStackFrameValue((ushort)currentInstr.data);
          }
        case OpCodes.Variable: {
            if (row < 0 || row >= dataset.Rows) return double.NaN;
            var variableTreeNode = (VariableTreeNode)currentInstr.dynamicNode;
            return ((IList<double>)currentInstr.data)[row] * variableTreeNode.Weight;
          }
        case OpCodes.BinaryFactorVariable: {
            if (row < 0 || row >= dataset.Rows) return double.NaN;
            var factorVarTreeNode = currentInstr.dynamicNode as BinaryFactorVariableTreeNode;
            return ((IList<string>)currentInstr.data)[row] == factorVarTreeNode.VariableValue ? factorVarTreeNode.Weight : 0;
          }
        case OpCodes.FactorVariable: {
            if (row < 0 || row >= dataset.Rows) return double.NaN;
            var factorVarTreeNode = currentInstr.dynamicNode as FactorVariableTreeNode;
            return factorVarTreeNode.GetValue(((IList<string>)currentInstr.data)[row]);
          }
        case OpCodes.LagVariable: {
            var laggedVariableTreeNode = (LaggedVariableTreeNode)currentInstr.dynamicNode;
            int actualRow = row + laggedVariableTreeNode.Lag;
            if (actualRow < 0 || actualRow >= dataset.Rows) { return double.NaN; }
            return ((IList<double>)currentInstr.data)[actualRow] * laggedVariableTreeNode.Weight;
          }
        case OpCodes.Constant: {
            var constTreeNode = (ConstantTreeNode)currentInstr.dynamicNode;
            return constTreeNode.Value;
          }

        //mkommend: this symbol uses the logistic function f(x) = 1 / (1 + e^(-alpha * x) ) 
        //to determine the relative amounts of the true and false branch see http://en.wikipedia.org/wiki/Logistic_function
        case OpCodes.VariableCondition: {
            if (row < 0 || row >= dataset.Rows) return double.NaN;
            var variableConditionTreeNode = (VariableConditionTreeNode)currentInstr.dynamicNode;
            if (!variableConditionTreeNode.Symbol.IgnoreSlope) {
              double variableValue = ((IList<double>)currentInstr.data)[row];
              double x = variableValue - variableConditionTreeNode.Threshold;
              double p = 1 / (1 + Math.Exp(-variableConditionTreeNode.Slope * x));

              double trueBranch = Evaluate(dataset, ref row, state);
              double falseBranch = Evaluate(dataset, ref row, state);

              return trueBranch * p + falseBranch * (1 - p);
            } else {
              // strict threshold
              double variableValue = ((IList<double>)currentInstr.data)[row];
              if (variableValue <= variableConditionTreeNode.Threshold) {
                var left = Evaluate(dataset, ref row, state);
                state.SkipInstructions();
                return left;
              } else {
                state.SkipInstructions();
                return Evaluate(dataset, ref row, state);
              }
            }
          }
        default:
          throw new NotSupportedException();
      }
    }
  }
}