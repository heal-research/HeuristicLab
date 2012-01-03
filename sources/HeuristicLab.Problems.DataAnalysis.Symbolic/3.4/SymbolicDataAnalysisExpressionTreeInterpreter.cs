#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SymbolicDataAnalysisExpressionTreeInterpreter", "Interpreter for symbolic expression trees including automatically defined functions.")]
  public sealed class SymbolicDataAnalysisExpressionTreeInterpreter : ParameterizedNamedItem, ISymbolicDataAnalysisExpressionTreeInterpreter {
    private const string CheckExpressionsWithIntervalArithmeticParameterName = "CheckExpressionsWithIntervalArithmetic";
    #region private classes
    private class InterpreterState {
      private double[] argumentStack;
      private int argumentStackPointer;
      private Instruction[] code;
      private int pc;
      public int ProgramCounter {
        get { return pc; }
        set { pc = value; }
      }
      internal InterpreterState(Instruction[] code, int argumentStackSize) {
        this.code = code;
        this.pc = 0;
        if (argumentStackSize > 0) {
          this.argumentStack = new double[argumentStackSize];
        }
        this.argumentStackPointer = 0;
      }

      internal void Reset() {
        this.pc = 0;
        this.argumentStackPointer = 0;
      }

      internal Instruction NextInstruction() {
        return code[pc++];
      }
      private void Push(double val) {
        argumentStack[argumentStackPointer++] = val;
      }
      private double Pop() {
        return argumentStack[--argumentStackPointer];
      }

      internal void CreateStackFrame(double[] argValues) {
        // push in reverse order to make indexing easier
        for (int i = argValues.Length - 1; i >= 0; i--) {
          argumentStack[argumentStackPointer++] = argValues[i];
        }
        Push(argValues.Length);
      }

      internal void RemoveStackFrame() {
        int size = (int)Pop();
        argumentStackPointer -= size;
      }

      internal double GetStackFrameValue(ushort index) {
        // layout of stack:
        // [0]   <- argumentStackPointer
        // [StackFrameSize = N + 1]
        // [Arg0] <- argumentStackPointer - 2 - 0
        // [Arg1] <- argumentStackPointer - 2 - 1
        // [...]
        // [ArgN] <- argumentStackPointer - 2 - N
        // <Begin of stack frame>
        return argumentStack[argumentStackPointer - index - 2];
      }
    }
    private class OpCodes {
      public const byte Add = 1;
      public const byte Sub = 2;
      public const byte Mul = 3;
      public const byte Div = 4;

      public const byte Sin = 5;
      public const byte Cos = 6;
      public const byte Tan = 7;

      public const byte Log = 8;
      public const byte Exp = 9;

      public const byte IfThenElse = 10;

      public const byte GT = 11;
      public const byte LT = 12;

      public const byte AND = 13;
      public const byte OR = 14;
      public const byte NOT = 15;


      public const byte Average = 16;

      public const byte Call = 17;

      public const byte Variable = 18;
      public const byte LagVariable = 19;
      public const byte Constant = 20;
      public const byte Arg = 21;

      public const byte Power = 22;
      public const byte Root = 23;
      public const byte TimeLag = 24;
      public const byte Integral = 25;
      public const byte Derivative = 26;

      public const byte VariableCondition = 27;
    }
    #endregion

    private Dictionary<Type, byte> symbolToOpcode = new Dictionary<Type, byte>() {
      { typeof(Addition), OpCodes.Add },
      { typeof(Subtraction), OpCodes.Sub },
      { typeof(Multiplication), OpCodes.Mul },
      { typeof(Division), OpCodes.Div },
      { typeof(Sine), OpCodes.Sin },
      { typeof(Cosine), OpCodes.Cos },
      { typeof(Tangent), OpCodes.Tan },
      { typeof(Logarithm), OpCodes.Log },
      { typeof(Exponential), OpCodes.Exp },
      { typeof(IfThenElse), OpCodes.IfThenElse },
      { typeof(GreaterThan), OpCodes.GT },
      { typeof(LessThan), OpCodes.LT },
      { typeof(And), OpCodes.AND },
      { typeof(Or), OpCodes.OR },
      { typeof(Not), OpCodes.NOT},
      { typeof(Average), OpCodes.Average},
      { typeof(InvokeFunction), OpCodes.Call },
      { typeof(HeuristicLab.Problems.DataAnalysis.Symbolic.Variable), OpCodes.Variable },
      { typeof(LaggedVariable), OpCodes.LagVariable },
      { typeof(Constant), OpCodes.Constant },
      { typeof(Argument), OpCodes.Arg },
      { typeof(Power),OpCodes.Power},
      { typeof(Root),OpCodes.Root},
      { typeof(TimeLag), OpCodes.TimeLag}, 
      { typeof(Integral), OpCodes.Integral},
      { typeof(Derivative), OpCodes.Derivative},
      { typeof(VariableCondition),OpCodes.VariableCondition}
    };

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
    #endregion

    #region properties
    public BoolValue CheckExpressionsWithIntervalArithmetic {
      get { return CheckExpressionsWithIntervalArithmeticParameter.Value; }
      set { CheckExpressionsWithIntervalArithmeticParameter.Value = value; }
    }
    #endregion


    [StorableConstructor]
    private SymbolicDataAnalysisExpressionTreeInterpreter(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisExpressionTreeInterpreter(SymbolicDataAnalysisExpressionTreeInterpreter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeInterpreter(this, cloner);
    }

    public SymbolicDataAnalysisExpressionTreeInterpreter()
      : base("SymbolicDataAnalysisExpressionTreeInterpreter", "Interpreter for symbolic expression trees including automatically defined functions.") {
      Parameters.Add(new ValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.", new BoolValue(false)));
    }

    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows) {
      if (CheckExpressionsWithIntervalArithmetic.Value)
        throw new NotSupportedException("Interval arithmetic is not yet supported in the symbolic data analysis interpreter.");
      var compiler = new SymbolicExpressionTreeCompiler();
      Instruction[] code = compiler.Compile(tree, MapSymbolToOpCode);
      int necessaryArgStackSize = 0;
      for (int i = 0; i < code.Length; i++) {
        Instruction instr = code[i];
        if (instr.opCode == OpCodes.Variable) {
          var variableTreeNode = instr.dynamicNode as VariableTreeNode;
          instr.iArg0 = dataset.GetReadOnlyDoubleValues(variableTreeNode.VariableName);
          code[i] = instr;
        } else if (instr.opCode == OpCodes.LagVariable) {
          var laggedVariableTreeNode = instr.dynamicNode as LaggedVariableTreeNode;
          instr.iArg0 = dataset.GetReadOnlyDoubleValues(laggedVariableTreeNode.VariableName);
          code[i] = instr;
        } else if (instr.opCode == OpCodes.VariableCondition) {
          var variableConditionTreeNode = instr.dynamicNode as VariableConditionTreeNode;
          instr.iArg0 = dataset.GetReadOnlyDoubleValues(variableConditionTreeNode.VariableName);
        } else if (instr.opCode == OpCodes.Call) {
          necessaryArgStackSize += instr.nArguments + 1;
        }
      }
      var state = new InterpreterState(code, necessaryArgStackSize);

      foreach (var rowEnum in rows) {
        int row = rowEnum;
        state.Reset();
        yield return Evaluate(dataset, ref row, state);
      }
    }

    private double Evaluate(Dataset dataset, ref int row, InterpreterState state) {
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
            if (currentInstr.nArguments == 1) s = -s;
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
            if (currentInstr.nArguments == 1) p = 1.0 / p;
            return p;
          }
        case OpCodes.Average: {
            double sum = Evaluate(dataset, ref row, state);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              sum += Evaluate(dataset, ref row, state);
            }
            return sum / currentInstr.nArguments;
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
        case OpCodes.Power: {
            double x = Evaluate(dataset, ref row, state);
            double y = Math.Round(Evaluate(dataset, ref row, state));
            return Math.Pow(x, y);
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
        case OpCodes.IfThenElse: {
            double condition = Evaluate(dataset, ref row, state);
            double result;
            if (condition > 0.0) {
              result = Evaluate(dataset, ref row, state); SkipInstructions(state);
            } else {
              SkipInstructions(state); result = Evaluate(dataset, ref row, state);
            }
            return result;
          }
        case OpCodes.AND: {
            double result = Evaluate(dataset, ref row, state);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              if (result > 0.0) result = Evaluate(dataset, ref row, state);
              else {
                SkipInstructions(state);
              }
            }
            return result > 0.0 ? 1.0 : -1.0;
          }
        case OpCodes.OR: {
            double result = Evaluate(dataset, ref row, state);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              if (result <= 0.0) result = Evaluate(dataset, ref row, state);
              else {
                SkipInstructions(state);
              }
            }
            return result > 0.0 ? 1.0 : -1.0;
          }
        case OpCodes.NOT: {
            return Evaluate(dataset, ref row, state) > 0.0 ? -1.0 : 1.0;
          }
        case OpCodes.GT: {
            double x = Evaluate(dataset, ref row, state);
            double y = Evaluate(dataset, ref row, state);
            if (x > y) return 1.0;
            else return -1.0;
          }
        case OpCodes.LT: {
            double x = Evaluate(dataset, ref row, state);
            double y = Evaluate(dataset, ref row, state);
            if (x < y) return 1.0;
            else return -1.0;
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
            state.ProgramCounter = (ushort)currentInstr.iArg0;
            // evaluate the function
            double v = Evaluate(dataset, ref row, state);

            // delete the stack frame
            state.RemoveStackFrame();

            // restore the pc => evaluation will continue at point after my subtrees  
            state.ProgramCounter = savedPc;
            return v;
          }
        case OpCodes.Arg: {
            return state.GetStackFrameValue((ushort)currentInstr.iArg0);
          }
        case OpCodes.Variable: {
            if (row < 0 || row >= dataset.Rows)
              return double.NaN;
            var variableTreeNode = (VariableTreeNode)currentInstr.dynamicNode;
            return ((IList<double>)currentInstr.iArg0)[row] * variableTreeNode.Weight;
          }
        case OpCodes.LagVariable: {
            var laggedVariableTreeNode = (LaggedVariableTreeNode)currentInstr.dynamicNode;
            int actualRow = row + laggedVariableTreeNode.Lag;
            if (actualRow < 0 || actualRow >= dataset.Rows)
              return double.NaN;
            return ((IList<double>)currentInstr.iArg0)[actualRow] * laggedVariableTreeNode.Weight;
          }
        case OpCodes.Constant: {
            var constTreeNode = currentInstr.dynamicNode as ConstantTreeNode;
            return constTreeNode.Value;
          }

        //mkommend: this symbol uses the logistic function f(x) = 1 / (1 + e^(-alpha * x) ) 
        //to determine the relative amounts of the true and false branch see http://en.wikipedia.org/wiki/Logistic_function
        case OpCodes.VariableCondition: {
            if (row < 0 || row >= dataset.Rows)
              return double.NaN;
            var variableConditionTreeNode = (VariableConditionTreeNode)currentInstr.dynamicNode;
            double variableValue = ((IList<double>)currentInstr.iArg0)[row];
            double x = variableValue - variableConditionTreeNode.Threshold;
            double p = 1 / (1 + Math.Exp(-variableConditionTreeNode.Slope * x));

            double trueBranch = Evaluate(dataset, ref row, state);
            double falseBranch = Evaluate(dataset, ref row, state);

            return trueBranch * p + falseBranch * (1 - p);
          }
        default: throw new NotSupportedException();
      }
    }

    private byte MapSymbolToOpCode(ISymbolicExpressionTreeNode treeNode) {
      if (symbolToOpcode.ContainsKey(treeNode.Symbol.GetType()))
        return symbolToOpcode[treeNode.Symbol.GetType()];
      else
        throw new NotSupportedException("Symbol: " + treeNode.Symbol);
    }

    // skips a whole branch
    private void SkipInstructions(InterpreterState state) {
      int i = 1;
      while (i > 0) {
        i += state.NextInstruction().nArguments;
        i--;
      }
    }
  }
}
