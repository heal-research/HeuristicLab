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
using System.Linq;
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
    private static MethodInfo listGetValue = typeof(IList<double>).GetProperty("Item", new Type[] { typeof(int) }).GetGetMethod();
    private static MethodInfo cos = typeof(Math).GetMethod("Cos", new Type[] { typeof(double) });
    private static MethodInfo sin = typeof(Math).GetMethod("Sin", new Type[] { typeof(double) });
    private static MethodInfo tan = typeof(Math).GetMethod("Tan", new Type[] { typeof(double) });
    private static MethodInfo exp = typeof(Math).GetMethod("Exp", new Type[] { typeof(double) });
    private static MethodInfo log = typeof(Math).GetMethod("Log", new Type[] { typeof(double) });
    private static MethodInfo power = typeof(Math).GetMethod("Pow", new Type[] { typeof(double), typeof(double) });
    private static MethodInfo round = typeof(Math).GetMethod("Round", new Type[] { typeof(double) });

    internal delegate double CompiledFunction(int sampleIndex, IList<double>[] columns);
    private const string CheckExpressionsWithIntervalArithmeticParameterName = "CheckExpressionsWithIntervalArithmetic";
    private const string EvaluatedSolutionsParameterName = "EvaluatedSolutions";
    #region private classes
    private class InterpreterState {
      private Instruction[] code;
      private int pc;

      public int ProgramCounter {
        get { return pc; }
        set { pc = value; }
      }

      private bool inLaggedContext;
      public bool InLaggedContext {
        get { return inLaggedContext; }
        set { inLaggedContext = value; }
      }
      internal InterpreterState(Instruction[] code) {
        this.inLaggedContext = false;
        this.code = code;
        this.pc = 0;
      }

      internal Instruction NextInstruction() {
        return code[pc++];
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
    private SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(SymbolicDataAnalysisExpressionTreeILEmittingInterpreter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(this, cloner);
    }

    public SymbolicDataAnalysisExpressionTreeILEmittingInterpreter()
      : base("SymbolicDataAnalysisExpressionTreeILEmittingInterpreter", "Interpreter for symbolic expression trees.") {
      Parameters.Add(new ValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName, "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(EvaluatedSolutionsParameterName))
        Parameters.Add(new ValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
    }

    #region IStatefulItem
    public void InitializeState() {
      EvaluatedSolutions.Value = 0;
    }

    public void ClearState() {
      EvaluatedSolutions.Value = 0;
    }
    #endregion

    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows) {
      if (CheckExpressionsWithIntervalArithmetic.Value)
        throw new NotSupportedException("Interval arithmetic is not yet supported in the symbolic data analysis interpreter.");
      EvaluatedSolutions.Value++; // increment the evaluated solutions counter
      var compiler = new SymbolicExpressionTreeCompiler();
      Instruction[] code = compiler.Compile(tree, MapSymbolToOpCode);
      int necessaryArgStackSize = 0;

      Dictionary<string, int> doubleVariableNames = dataset.DoubleVariables.Select((x, i) => new { x, i }).ToDictionary(e => e.x, e => e.i);
      IList<double>[] columns = (from v in doubleVariableNames.Keys
                                 select dataset.GetReadOnlyDoubleValues(v))
                                .ToArray();

      for (int i = 0; i < code.Length; i++) {
        Instruction instr = code[i];
        if (instr.opCode == OpCodes.Variable) {
          var variableTreeNode = instr.dynamicNode as VariableTreeNode;
          instr.iArg0 = doubleVariableNames[variableTreeNode.VariableName];
          code[i] = instr;
        } else if (instr.opCode == OpCodes.LagVariable) {
          var variableTreeNode = instr.dynamicNode as LaggedVariableTreeNode;
          instr.iArg0 = doubleVariableNames[variableTreeNode.VariableName];
          code[i] = instr;
        } else if (instr.opCode == OpCodes.VariableCondition) {
          var variableConditionTreeNode = instr.dynamicNode as VariableConditionTreeNode;
          instr.iArg0 = doubleVariableNames[variableConditionTreeNode.VariableName];
        } else if (instr.opCode == OpCodes.Call) {
          necessaryArgStackSize += instr.nArguments + 1;
        }
      }
      var state = new InterpreterState(code);

      Type[] methodArgs = { typeof(int), typeof(IList<double>[]) };
      DynamicMethod testFun = new DynamicMethod("TestFun", typeof(double), methodArgs, typeof(SymbolicDataAnalysisExpressionTreeILEmittingInterpreter).Module);

      ILGenerator il = testFun.GetILGenerator();
      CompileInstructions(il, state, dataset);
      il.Emit(System.Reflection.Emit.OpCodes.Conv_R8);
      il.Emit(System.Reflection.Emit.OpCodes.Ret);
      var function = (CompiledFunction)testFun.CreateDelegate(typeof(CompiledFunction));

      foreach (var row in rows) {
        yield return function(row, columns);
      }
    }

    private void CompileInstructions(ILGenerator il, InterpreterState state, Dataset ds) {
      Instruction currentInstr = state.NextInstruction();
      int nArgs = currentInstr.nArguments;

      switch (currentInstr.opCode) {
        case OpCodes.Add: {
            if (nArgs > 0) {
              CompileInstructions(il, state, ds);
            }
            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state, ds);
              il.Emit(System.Reflection.Emit.OpCodes.Add);
            }
            return;
          }
        case OpCodes.Sub: {
            if (nArgs == 1) {
              CompileInstructions(il, state, ds);
              il.Emit(System.Reflection.Emit.OpCodes.Neg);
              return;
            }
            if (nArgs > 0) {
              CompileInstructions(il, state, ds);
            }
            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state, ds);
              il.Emit(System.Reflection.Emit.OpCodes.Sub);
            }
            return;
          }
        case OpCodes.Mul: {
            if (nArgs > 0) {
              CompileInstructions(il, state, ds);
            }
            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state, ds);
              il.Emit(System.Reflection.Emit.OpCodes.Mul);
            }
            return;
          }
        case OpCodes.Div: {
            if (nArgs == 1) {
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0);
              CompileInstructions(il, state, ds);
              il.Emit(System.Reflection.Emit.OpCodes.Div);
              return;
            }
            if (nArgs > 0) {
              CompileInstructions(il, state, ds);
            }
            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state, ds);
              il.Emit(System.Reflection.Emit.OpCodes.Div);
            }
            return;
          }
        case OpCodes.Average: {
            CompileInstructions(il, state, ds);
            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state, ds);
              il.Emit(System.Reflection.Emit.OpCodes.Add);
            }
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, nArgs);
            il.Emit(System.Reflection.Emit.OpCodes.Div);
            return;
          }
        case OpCodes.Cos: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, cos);
            return;
          }
        case OpCodes.Sin: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, sin);
            return;
          }
        case OpCodes.Tan: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, tan);
            return;
          }
        case OpCodes.Power: {
            CompileInstructions(il, state, ds);
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, round);
            il.Emit(System.Reflection.Emit.OpCodes.Call, power);
            return;
          }
        case OpCodes.Root: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // 1 / round(...)
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, round);
            il.Emit(System.Reflection.Emit.OpCodes.Div);
            il.Emit(System.Reflection.Emit.OpCodes.Call, power);
            return;
          }
        case OpCodes.Exp: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, exp);
            return;
          }
        case OpCodes.Log: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, log);
            return;
          }
        case OpCodes.IfThenElse: {
            Label end = il.DefineLabel();
            Label c1 = il.DefineLabel();
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0); // > 0
            il.Emit(System.Reflection.Emit.OpCodes.Cgt);
            il.Emit(System.Reflection.Emit.OpCodes.Brfalse, c1);
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Br, end);
            il.MarkLabel(c1);
            CompileInstructions(il, state, ds);
            il.MarkLabel(end);
            return;
          }
        case OpCodes.AND: {
            Label falseBranch = il.DefineLabel();
            Label end = il.DefineLabel();
            CompileInstructions(il, state, ds);
            for (int i = 1; i < nArgs; i++) {
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0); // > 0
              il.Emit(System.Reflection.Emit.OpCodes.Cgt);
              il.Emit(System.Reflection.Emit.OpCodes.Brfalse, falseBranch);
              CompileInstructions(il, state, ds);
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
            CompileInstructions(il, state, ds);
            for (int i = 1; i < nArgs; i++) {
              Label nextArgBranch = il.DefineLabel();
              // complex definition because of special properties of NaN  
              il.Emit(System.Reflection.Emit.OpCodes.Dup);
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0); // <= 0        
              il.Emit(System.Reflection.Emit.OpCodes.Ble, nextArgBranch);
              il.Emit(System.Reflection.Emit.OpCodes.Br, resultBranch);
              il.MarkLabel(nextArgBranch);
              il.Emit(System.Reflection.Emit.OpCodes.Pop);
              CompileInstructions(il, state, ds);
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
            CompileInstructions(il, state, ds);
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
            CompileInstructions(il, state, ds);
            CompileInstructions(il, state, ds);

            il.Emit(System.Reflection.Emit.OpCodes.Cgt); // 1 (>) / 0 (otherwise)
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 2.0); // * 2
            il.Emit(System.Reflection.Emit.OpCodes.Mul);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // - 1
            il.Emit(System.Reflection.Emit.OpCodes.Sub);
            return;
          }
        case OpCodes.LT: {
            CompileInstructions(il, state, ds);
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Clt);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 2.0); // * 2
            il.Emit(System.Reflection.Emit.OpCodes.Mul);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // - 1
            il.Emit(System.Reflection.Emit.OpCodes.Sub);
            return;
          }
        case OpCodes.TimeLag: {
            LaggedTreeNode laggedTreeNode = (LaggedTreeNode)currentInstr.dynamicNode;
            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // row -= lag
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, laggedTreeNode.Lag);
            il.Emit(System.Reflection.Emit.OpCodes.Add);
            il.Emit(System.Reflection.Emit.OpCodes.Starg, 0);
            var prevLaggedContext = state.InLaggedContext;
            state.InLaggedContext = true;
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // row += lag
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, laggedTreeNode.Lag);
            il.Emit(System.Reflection.Emit.OpCodes.Sub);
            il.Emit(System.Reflection.Emit.OpCodes.Starg, 0);
            state.InLaggedContext = prevLaggedContext;
            return;
          }
        case OpCodes.Integral: {
            int savedPc = state.ProgramCounter;
            LaggedTreeNode laggedTreeNode = (LaggedTreeNode)currentInstr.dynamicNode;
            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // row -= lag
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, laggedTreeNode.Lag);
            il.Emit(System.Reflection.Emit.OpCodes.Add);
            il.Emit(System.Reflection.Emit.OpCodes.Starg, 0);
            var prevLaggedContext = state.InLaggedContext;
            state.InLaggedContext = true;
            CompileInstructions(il, state, ds);
            for (int l = laggedTreeNode.Lag; l < 0; l++) {
              il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // row += lag
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
              il.Emit(System.Reflection.Emit.OpCodes.Add);
              il.Emit(System.Reflection.Emit.OpCodes.Starg, 0);
              state.ProgramCounter = savedPc;
              CompileInstructions(il, state, ds);
              il.Emit(System.Reflection.Emit.OpCodes.Add);
            }
            state.InLaggedContext = prevLaggedContext;
            return;
          }

        //mkommend: derivate calculation taken from: 
        //http://www.holoborodko.com/pavel/numerical-methods/numerical-derivative/smooth-low-noise-differentiators/
        //one sided smooth differentiatior, N = 4
        // y' = 1/8h (f_i + 2f_i-1, -2 f_i-3 - f_i-4)
        case OpCodes.Derivative: {
            int savedPc = state.ProgramCounter;
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // row --
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_M1);
            il.Emit(System.Reflection.Emit.OpCodes.Add);
            il.Emit(System.Reflection.Emit.OpCodes.Starg, 0);
            state.ProgramCounter = savedPc;
            var prevLaggedContext = state.InLaggedContext;
            state.InLaggedContext = true;
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 2.0); // f_0 + 2 * f_1
            il.Emit(System.Reflection.Emit.OpCodes.Mul);
            il.Emit(System.Reflection.Emit.OpCodes.Add);

            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // row -=2
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_2);
            il.Emit(System.Reflection.Emit.OpCodes.Sub);
            il.Emit(System.Reflection.Emit.OpCodes.Starg, 0);
            state.ProgramCounter = savedPc;
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 2.0); // f_0 + 2 * f_1 - 2 * f_3
            il.Emit(System.Reflection.Emit.OpCodes.Mul);
            il.Emit(System.Reflection.Emit.OpCodes.Sub);

            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // row --
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_M1);
            il.Emit(System.Reflection.Emit.OpCodes.Add);
            il.Emit(System.Reflection.Emit.OpCodes.Starg, 0);
            state.ProgramCounter = savedPc;
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Sub); // f_0 + 2 * f_1 - 2 * f_3 - f_4
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 8.0); // / 8
            il.Emit(System.Reflection.Emit.OpCodes.Div);

            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // row +=4
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_4);
            il.Emit(System.Reflection.Emit.OpCodes.Add);
            il.Emit(System.Reflection.Emit.OpCodes.Starg, 0);
            state.InLaggedContext = prevLaggedContext;
            return;
          }
        case OpCodes.Call: {
            throw new NotSupportedException("Automatically defined functions are not supported by the SymbolicDataAnalysisTreeILEmittingInterpreter. Either turn of ADFs or change the interpeter.");
          }
        case OpCodes.Arg: {
            throw new NotSupportedException("Automatically defined functions are not supported by the SymbolicDataAnalysisTreeILEmittingInterpreter. Either turn of ADFs or change the interpeter.");
          }
        case OpCodes.Variable: {
            VariableTreeNode varNode = (VariableTreeNode)currentInstr.dynamicNode;
            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1); // load columns array
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, (int)currentInstr.iArg0);
            // load correct column of the current variable
            il.Emit(System.Reflection.Emit.OpCodes.Ldelem_Ref);
            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // rowIndex
            if (!state.InLaggedContext) {
              il.Emit(System.Reflection.Emit.OpCodes.Call, listGetValue);
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, varNode.Weight); // load weight
              il.Emit(System.Reflection.Emit.OpCodes.Mul);
            } else {
              var nanResult = il.DefineLabel();
              var normalResult = il.DefineLabel();
              il.Emit(System.Reflection.Emit.OpCodes.Dup);
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
              il.Emit(System.Reflection.Emit.OpCodes.Blt, nanResult);
              il.Emit(System.Reflection.Emit.OpCodes.Dup);
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, ds.Rows);
              il.Emit(System.Reflection.Emit.OpCodes.Bge, nanResult);
              il.Emit(System.Reflection.Emit.OpCodes.Call, listGetValue);
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, varNode.Weight); // load weight
              il.Emit(System.Reflection.Emit.OpCodes.Mul);
              il.Emit(System.Reflection.Emit.OpCodes.Br, normalResult);
              il.MarkLabel(nanResult);
              il.Emit(System.Reflection.Emit.OpCodes.Pop); // rowIndex
              il.Emit(System.Reflection.Emit.OpCodes.Pop); // column reference
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, double.NaN);
              il.MarkLabel(normalResult);
            }
            return;
          }
        case OpCodes.LagVariable: {
            var nanResult = il.DefineLabel();
            var normalResult = il.DefineLabel();
            LaggedVariableTreeNode varNode = (LaggedVariableTreeNode)currentInstr.dynamicNode;
            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1); // load columns array
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, (int)currentInstr.iArg0); // load correct column of the current variable
            il.Emit(System.Reflection.Emit.OpCodes.Ldelem_Ref);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, varNode.Lag); // lag
            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // rowIndex
            il.Emit(System.Reflection.Emit.OpCodes.Add); // actualRowIndex = rowIndex + sampleOffset
            il.Emit(System.Reflection.Emit.OpCodes.Dup);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
            il.Emit(System.Reflection.Emit.OpCodes.Blt, nanResult);
            il.Emit(System.Reflection.Emit.OpCodes.Dup);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, ds.Rows);
            il.Emit(System.Reflection.Emit.OpCodes.Bge, nanResult);
            il.Emit(System.Reflection.Emit.OpCodes.Call, listGetValue);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, varNode.Weight); // load weight
            il.Emit(System.Reflection.Emit.OpCodes.Mul);
            il.Emit(System.Reflection.Emit.OpCodes.Br, normalResult);
            il.MarkLabel(nanResult);
            il.Emit(System.Reflection.Emit.OpCodes.Pop); // sample index
            il.Emit(System.Reflection.Emit.OpCodes.Pop); // column reference
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, double.NaN);
            il.MarkLabel(normalResult);
            return;
          }
        case OpCodes.Constant: {
            ConstantTreeNode constNode = (ConstantTreeNode)currentInstr.dynamicNode;
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, constNode.Value);
            return;
          }

        //mkommend: this symbol uses the logistic function f(x) = 1 / (1 + e^(-alpha * x) ) 
        //to determine the relative amounts of the true and false branch see http://en.wikipedia.org/wiki/Logistic_function
        case OpCodes.VariableCondition: {
            throw new NotSupportedException("Interpretation of symbol " + currentInstr.dynamicNode.Symbol.Name + " is not supported by the SymbolicDataAnalysisTreeILEmittingInterpreter");
          }
        default: throw new NotSupportedException("Interpretation of symbol " + currentInstr.dynamicNode.Symbol.Name + " is not supported by the SymbolicDataAnalysisTreeILEmittingInterpreter");
      }
    }

    private byte MapSymbolToOpCode(ISymbolicExpressionTreeNode treeNode) {
      if (symbolToOpcode.ContainsKey(treeNode.Symbol.GetType()))
        return symbolToOpcode[treeNode.Symbol.GetType()];
      else
        throw new NotSupportedException("Symbol: " + treeNode.Symbol);
    }
  }
}
