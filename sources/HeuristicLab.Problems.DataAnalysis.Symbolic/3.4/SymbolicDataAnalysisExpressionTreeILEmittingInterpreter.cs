#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Reflection;
using System.Reflection.Emit;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SymbolicDataAnalysisExpressionTreeILEmittingInterpreter", "Interpreter for symbolic expression trees.")]
  public sealed class SymbolicDataAnalysisExpressionTreeILEmittingInterpreter : ParameterizedNamedItem, ISymbolicDataAnalysisExpressionTreeInterpreter {
    private static MethodInfo datasetGetValue = typeof(ThresholdCalculator).Assembly.GetType("HeuristicLab.Problems.DataAnalysis.Dataset").GetProperty("Item", new Type[] { typeof(int), typeof(int) }).GetGetMethod();
    private static MethodInfo cos = typeof(Math).GetMethod("Cos", new Type[] { typeof(double) });
    private static MethodInfo sin = typeof(Math).GetMethod("Sin", new Type[] { typeof(double) });
    private static MethodInfo tan = typeof(Math).GetMethod("Tan", new Type[] { typeof(double) });
    private static MethodInfo exp = typeof(Math).GetMethod("Exp", new Type[] { typeof(double) });
    private static MethodInfo log = typeof(Math).GetMethod("Log", new Type[] { typeof(double) });
    private static MethodInfo sign = typeof(Math).GetMethod("Sign", new Type[] { typeof(double) });
    private static MethodInfo power = typeof(Math).GetMethod("Pow", new Type[] { typeof(double), typeof(double) });
    private static MethodInfo sqrt = typeof(Math).GetMethod("Sqrt", new Type[] { typeof(double) });
    private static MethodInfo isNan = typeof(double).GetMethod("IsNaN", new Type[] { typeof(double) });
    private static MethodInfo abs = typeof(Math).GetMethod("Abs", new Type[] { typeof(double) });
    private const double EPSILON = 1.0E-6;

    internal delegate double CompiledFunction(Dataset dataset, int sampleIndex);
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
    private SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(SymbolicDataAnalysisExpressionTreeILEmittingInterpreter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(this, cloner);
    }

    public SymbolicDataAnalysisExpressionTreeILEmittingInterpreter()
      : base("SymbolicDataAnalysisExpressionTreeILEmittingInterpreter", "Interpreter for symbolic expression trees.") {
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
          var variableTreeNode = instr.dynamicNode as LaggedVariableTreeNode;
          instr.iArg0 = dataset.GetReadOnlyDoubleValues(variableTreeNode.VariableName);
          code[i] = instr;
        } else if (instr.opCode == OpCodes.VariableCondition) {
          var variableConditionTreeNode = instr.dynamicNode as VariableConditionTreeNode;
          instr.iArg0 = dataset.GetReadOnlyDoubleValues(variableConditionTreeNode.VariableName);
        } else if (instr.opCode == OpCodes.Call) {
          necessaryArgStackSize += instr.nArguments + 1;
        }
      }
      var state = new InterpreterState(code, necessaryArgStackSize);

      Type[] methodArgs = { typeof(Dataset), typeof(int) };
      DynamicMethod testFun = new DynamicMethod("TestFun", typeof(double), methodArgs, typeof(SymbolicDataAnalysisExpressionTreeILEmittingInterpreter).Module);

      ILGenerator il = testFun.GetILGenerator();
      CompileInstructions(il, state);
      il.Emit(System.Reflection.Emit.OpCodes.Conv_R8);
      il.Emit(System.Reflection.Emit.OpCodes.Ret);
      var function = (CompiledFunction)testFun.CreateDelegate(typeof(CompiledFunction));

      foreach (var row in rows) {
        yield return function(dataset, row);
      }
    }

    private void CompileInstructions(ILGenerator il, InterpreterState state) {
      Instruction currentInstr = state.NextInstruction();
      int nArgs = currentInstr.nArguments;

      switch (currentInstr.opCode) {
        case OpCodes.Add: {
            if (nArgs > 0) {
              CompileInstructions(il, state);
            }
            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state);
              il.Emit(System.Reflection.Emit.OpCodes.Add);
            }
            return;
          }
        case OpCodes.Sub: {
            if (nArgs == 1) {
              CompileInstructions(il, state);
              il.Emit(System.Reflection.Emit.OpCodes.Neg);
              return;
            }
            if (nArgs > 0) {
              CompileInstructions(il, state);
            }
            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state);
              il.Emit(System.Reflection.Emit.OpCodes.Sub);
            }
            return;
          }
        case OpCodes.Mul: {
            if (nArgs > 0) {
              CompileInstructions(il, state);
            }
            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state);
              il.Emit(System.Reflection.Emit.OpCodes.Mul);
            }
            return;
          }
        case OpCodes.Div: {
            if (nArgs == 1) {
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0);
              CompileInstructions(il, state);
              il.Emit(System.Reflection.Emit.OpCodes.Div);
              return;
            }
            if (nArgs > 0) {
              CompileInstructions(il, state);
            }
            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state);
              il.Emit(System.Reflection.Emit.OpCodes.Div);
            }
            return;
          }
        case OpCodes.Average: {
            CompileInstructions(il, state);
            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state);
              il.Emit(System.Reflection.Emit.OpCodes.Add);
            }
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, nArgs);
            il.Emit(System.Reflection.Emit.OpCodes.Div);
            return;
          }
        case OpCodes.Cos: {
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Call, cos);
            return;
          }
        case OpCodes.Sin: {
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Call, sin);
            return;
          }
        case OpCodes.Tan: {
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Call, tan);
            return;
          }
        case OpCodes.Power: {
            CompileInstructions(il, state);
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Call, power);
            return;
          }
        case OpCodes.Root: {
            throw new NotImplementedException();
          }
        case OpCodes.Exp: {
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Call, exp);
            return;
          }
        case OpCodes.Log: {
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Call, log);
            return;
          }
        case OpCodes.IfThenElse: {
            Label end = il.DefineLabel();
            Label c1 = il.DefineLabel();
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0); // > 0
            il.Emit(System.Reflection.Emit.OpCodes.Cgt);
            il.Emit(System.Reflection.Emit.OpCodes.Brfalse, c1);
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Br, end);
            il.MarkLabel(c1);
            CompileInstructions(il, state);
            il.MarkLabel(end);
            return;
          }
        case OpCodes.AND: {
            Label falseBranch = il.DefineLabel();
            Label end = il.DefineLabel();
            CompileInstructions(il, state);
            for (int i = 1; i < nArgs; i++) {
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0); // > 0
              il.Emit(System.Reflection.Emit.OpCodes.Cgt);
              il.Emit(System.Reflection.Emit.OpCodes.Brfalse, falseBranch);
              CompileInstructions(il, state);
            }
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0); // > 0
            il.Emit(System.Reflection.Emit.OpCodes.Cgt);
            il.Emit(System.Reflection.Emit.OpCodes.Brfalse, falseBranch);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // 1
            il.Emit(System.Reflection.Emit.OpCodes.Br, end);
            il.MarkLabel(falseBranch);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // -1
            il.Emit(System.Reflection.Emit.OpCodes.Neg);
            il.MarkLabel(end);
            return;
          }
        case OpCodes.OR: {
            Label trueBranch = il.DefineLabel();
            Label end = il.DefineLabel();
            Label resultBranch = il.DefineLabel();
            CompileInstructions(il, state);
            for (int i = 1; i < nArgs; i++) {
              Label nextArgBranch = il.DefineLabel();
              // complex definition because of special properties of NaN  
              il.Emit(System.Reflection.Emit.OpCodes.Dup);
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0); // <= 0        
              il.Emit(System.Reflection.Emit.OpCodes.Ble, nextArgBranch);
              il.Emit(System.Reflection.Emit.OpCodes.Br, resultBranch);
              il.MarkLabel(nextArgBranch);
              il.Emit(System.Reflection.Emit.OpCodes.Pop);
              CompileInstructions(il, state);
            }
            il.MarkLabel(resultBranch);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0); // > 0
            il.Emit(System.Reflection.Emit.OpCodes.Cgt);
            il.Emit(System.Reflection.Emit.OpCodes.Brtrue, trueBranch);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // -1
            il.Emit(System.Reflection.Emit.OpCodes.Neg);
            il.Emit(System.Reflection.Emit.OpCodes.Br, end);
            il.MarkLabel(trueBranch);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // 1
            il.MarkLabel(end);
            return;
          }
        case OpCodes.NOT: {
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0); // > 0
            il.Emit(System.Reflection.Emit.OpCodes.Cgt);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 2.0); // * 2
            il.Emit(System.Reflection.Emit.OpCodes.Mul);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // - 1
            il.Emit(System.Reflection.Emit.OpCodes.Sub);
            il.Emit(System.Reflection.Emit.OpCodes.Neg); // * -1
            return;
          }
        case OpCodes.GT: {
            CompileInstructions(il, state);
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Cgt); // 1 (>) / 0 (otherwise)
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 2.0); // * 2
            il.Emit(System.Reflection.Emit.OpCodes.Mul);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // - 1
            il.Emit(System.Reflection.Emit.OpCodes.Sub);
            return;
          }
        case OpCodes.LT: {
            CompileInstructions(il, state);
            CompileInstructions(il, state);
            il.Emit(System.Reflection.Emit.OpCodes.Clt);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 2.0); // * 2
            il.Emit(System.Reflection.Emit.OpCodes.Mul);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // - 1
            il.Emit(System.Reflection.Emit.OpCodes.Sub);
            return;
          }
        case OpCodes.TimeLag: {
            throw new NotImplementedException();
          }
        case OpCodes.Integral: {
            throw new NotImplementedException();
          }

        //mkommend: derivate calculation taken from: 
        //http://www.holoborodko.com/pavel/numerical-methods/numerical-derivative/smooth-low-noise-differentiators/
        //one sided smooth differentiatior, N = 4
        // y' = 1/8h (f_i + 2f_i-1, -2 f_i-3 - f_i-4)
        case OpCodes.Derivative: {
            throw new NotImplementedException();
          }
        case OpCodes.Call: {
            throw new NotImplementedException();
          }
        case OpCodes.Arg: {
            throw new NotImplementedException();
          }
        case OpCodes.Variable: {
            //VariableTreeNode varNode = (VariableTreeNode)currentInstr.dynamicNode;
            //il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // load dataset
            //il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, 0); // sampleOffset
            //il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1); // sampleIndex
            //il.Emit(System.Reflection.Emit.OpCodes.Add); // row = sampleIndex + sampleOffset
            //il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, currentInstr.iArg0); // load var
            //il.Emit(System.Reflection.Emit.OpCodes.Call, datasetGetValue); // dataset.GetValue
            //il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, varNode.Weight); // load weight
            //il.Emit(System.Reflection.Emit.OpCodes.Mul);
            return;
          }
        case OpCodes.LagVariable: {
            throw new NotImplementedException();
          }
        case OpCodes.Constant: {
            ConstantTreeNode constNode = (ConstantTreeNode)currentInstr.dynamicNode;
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, constNode.Value);
            return;
          }

        //mkommend: this symbol uses the logistic function f(x) = 1 / (1 + e^(-alpha * x) ) 
        //to determine the relative amounts of the true and false branch see http://en.wikipedia.org/wiki/Logistic_function
        case OpCodes.VariableCondition: {
            throw new NotImplementedException();
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
