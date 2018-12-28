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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("426718E3-2A57-4CA4-98A1-65EDD0B0BDBF")]
  [Item("SymbolicDataAnalysisExpressionTreeILEmittingInterpreter", "Interpreter for symbolic expression trees.")]
  public sealed class SymbolicDataAnalysisExpressionTreeILEmittingInterpreter : ParameterizedNamedItem, ISymbolicDataAnalysisExpressionTreeInterpreter {
    private static readonly Type thisType = typeof(SymbolicDataAnalysisExpressionTreeILEmittingInterpreter);
    internal delegate double CompiledFunction(int sampleIndex, IList<double>[] columns);

    #region method infos
    private static MethodInfo listGetValue = typeof(IList<double>).GetProperty("Item", new Type[] { typeof(int) }).GetGetMethod();

    private static MethodInfo cos = typeof(Math).GetMethod("Cos", new Type[] { typeof(double) });
    private static MethodInfo sin = typeof(Math).GetMethod("Sin", new Type[] { typeof(double) });
    private static MethodInfo tan = typeof(Math).GetMethod("Tan", new Type[] { typeof(double) });
    private static MethodInfo exp = typeof(Math).GetMethod("Exp", new Type[] { typeof(double) });
    private static MethodInfo log = typeof(Math).GetMethod("Log", new Type[] { typeof(double) });
    private static MethodInfo power = typeof(Math).GetMethod("Pow", new Type[] { typeof(double), typeof(double) });
    private static MethodInfo round = typeof(Math).GetMethod("Round", new Type[] { typeof(double) });
    private static MethodInfo sqrt = typeof(Math).GetMethod("Sqrt", new Type[] { typeof(double) });
    private static MethodInfo abs = typeof(Math).GetMethod("Abs", new Type[] { typeof(double) });

    private static MethodInfo airyA = thisType.GetMethod("AiryA", new Type[] { typeof(double) });
    private static MethodInfo airyB = thisType.GetMethod("AiryB", new Type[] { typeof(double) });
    private static MethodInfo gamma = thisType.GetMethod("Gamma", new Type[] { typeof(double) });
    private static MethodInfo psi = thisType.GetMethod("Psi", new Type[] { typeof(double) });
    private static MethodInfo dawson = thisType.GetMethod("Dawson", new Type[] { typeof(double) });
    private static MethodInfo expIntegralEi = thisType.GetMethod("ExpIntegralEi", new Type[] { typeof(double) });
    private static MethodInfo sinIntegral = thisType.GetMethod("SinIntegral", new Type[] { typeof(double) });
    private static MethodInfo cosIntegral = thisType.GetMethod("CosIntegral", new Type[] { typeof(double) });
    private static MethodInfo hypSinIntegral = thisType.GetMethod("HypSinIntegral", new Type[] { typeof(double) });
    private static MethodInfo hypCosIntegral = thisType.GetMethod("HypCosIntegral", new Type[] { typeof(double) });
    private static MethodInfo fresnelCosIntegral = thisType.GetMethod("FresnelCosIntegral", new Type[] { typeof(double) });
    private static MethodInfo fresnelSinIntegral = thisType.GetMethod("FresnelSinIntegral", new Type[] { typeof(double) });
    private static MethodInfo norm = thisType.GetMethod("Norm", new Type[] { typeof(double) });
    private static MethodInfo erf = thisType.GetMethod("Erf", new Type[] { typeof(double) });
    private static MethodInfo bessel = thisType.GetMethod("Bessel", new Type[] { typeof(double) });
    private static MethodInfo string_eq = typeof(string).GetMethod("Equals", new Type[] { typeof(string) });
    #endregion

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
    private SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(StorableConstructorFlag _) : base(_) { }

    private SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(SymbolicDataAnalysisExpressionTreeILEmittingInterpreter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(this, cloner);
    }

    public SymbolicDataAnalysisExpressionTreeILEmittingInterpreter()
      : base("SymbolicDataAnalysisExpressionTreeILEmittingInterpreter", "Interpreter for symbolic expression trees.") {
      Parameters.Add(new FixedValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName,
        "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>(EvaluatedSolutionsParameterName, "A counter for the total number of solutions the interpreter has evaluated", new IntValue(0)));
    }

    public SymbolicDataAnalysisExpressionTreeILEmittingInterpreter(string name, string description)
      : base(name, description) {
      Parameters.Add(new FixedValueParameter<BoolValue>(CheckExpressionsWithIntervalArithmeticParameterName,
        "Switch that determines if the interpreter checks the validity of expressions with interval arithmetic before evaluating the expression.", new BoolValue(false)));
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

    public void ClearState() {
    }
    #endregion

    private readonly object syncRoot = new object();
    public IEnumerable<double> GetSymbolicExpressionTreeValues(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows) {
      if (CheckExpressionsWithIntervalArithmetic)
        throw new NotSupportedException("Interval arithmetic is not yet supported in the symbolic data analysis interpreter.");

      lock (syncRoot) {
        EvaluatedSolutions++; // increment the evaluated solutions counter
      }
      var state = PrepareInterpreterState(tree, dataset);

      Type[] methodArgs = { typeof(int), typeof(IList<double>[]) };
      DynamicMethod testFun = new DynamicMethod("TestFun", typeof(double), methodArgs, typeof(SymbolicDataAnalysisExpressionTreeILEmittingInterpreter).Module);

      ILGenerator il = testFun.GetILGenerator();
      CompileInstructions(il, state, dataset);
      il.Emit(System.Reflection.Emit.OpCodes.Conv_R8);
      il.Emit(System.Reflection.Emit.OpCodes.Ret);
      var function = (CompiledFunction)testFun.CreateDelegate(typeof(CompiledFunction));

      IList<double>[] columns = dataset.DoubleVariables.Select(v => dataset.GetReadOnlyDoubleValues(v)).ToArray();

      foreach (var row in rows) {
        yield return function(row, columns);
      }
    }

    private InterpreterState PrepareInterpreterState(ISymbolicExpressionTree tree, IDataset dataset) {
      Instruction[] code = SymbolicExpressionTreeCompiler.Compile(tree, OpCodes.MapSymbolToOpCode);
      Dictionary<string, int> doubleVariableNames = dataset.DoubleVariables.Select((x, i) => new { x, i }).ToDictionary(e => e.x, e => e.i);
      int necessaryArgStackSize = 0;
      foreach (Instruction instr in code) {
        if (instr.opCode == OpCodes.Variable) {
          var variableTreeNode = (VariableTreeNode)instr.dynamicNode;
          instr.data = doubleVariableNames[variableTreeNode.VariableName];
        } else if (instr.opCode == OpCodes.LagVariable) {
          var laggedVariableTreeNode = (LaggedVariableTreeNode)instr.dynamicNode;
          instr.data = doubleVariableNames[laggedVariableTreeNode.VariableName];
        } else if (instr.opCode == OpCodes.VariableCondition) {
          var variableConditionTreeNode = (VariableConditionTreeNode)instr.dynamicNode;
          instr.data = doubleVariableNames[variableConditionTreeNode.VariableName];
        } else if (instr.opCode == OpCodes.Call) {
          necessaryArgStackSize += instr.nArguments + 1;
        }
      }
      return new InterpreterState(code, necessaryArgStackSize);
    }

    private void CompileInstructions(ILGenerator il, InterpreterState state, IDataset ds) {
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
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, (double)nArgs);
            il.Emit(System.Reflection.Emit.OpCodes.Div);
            return;
          }
        case OpCodes.Absolute: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, abs);
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
        case OpCodes.Square: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 2.0);
            il.Emit(System.Reflection.Emit.OpCodes.Call, power);
            return;
          }
        case OpCodes.Cube: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 3.0);
            il.Emit(System.Reflection.Emit.OpCodes.Call, power);
            return;
          }
        case OpCodes.SquareRoot: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, sqrt);
            return;
          }
        case OpCodes.CubeRoot: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0 / 3.0);
            il.Emit(System.Reflection.Emit.OpCodes.Call, power);
            return;
          }
        case OpCodes.AiryA: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, airyA);
            return;
          }
        case OpCodes.AiryB: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, airyB);
            return;
          }
        case OpCodes.Bessel: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, bessel);
            return;
          }
        case OpCodes.CosineIntegral: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, cosIntegral);
            return;
          }
        case OpCodes.Dawson: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, dawson);
            return;
          }
        case OpCodes.Erf: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, erf);
            return;
          }
        case OpCodes.ExponentialIntegralEi: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, expIntegralEi);
            return;
          }
        case OpCodes.FresnelCosineIntegral: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, fresnelCosIntegral);
            return;
          }
        case OpCodes.FresnelSineIntegral: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, fresnelSinIntegral);
            return;
          }
        case OpCodes.Gamma: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, gamma);
            return;
          }
        case OpCodes.HyperbolicCosineIntegral: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, hypCosIntegral);
            return;
          }
        case OpCodes.HyperbolicSineIntegral: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, hypSinIntegral);
            return;
          }
        case OpCodes.Norm: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, norm);
            return;
          }
        case OpCodes.Psi: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, psi);
            return;
          }
        case OpCodes.SineIntegral: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Call, sinIntegral);
            return;
          }
        case OpCodes.AnalyticQuotient: {
            CompileInstructions(il, state, ds); // x1
            CompileInstructions(il, state, ds); // x2

            il.Emit(System.Reflection.Emit.OpCodes.Dup);
            il.Emit(System.Reflection.Emit.OpCodes.Mul); // x2*x2
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0);
            il.Emit(System.Reflection.Emit.OpCodes.Mul); // 1+x2*x2
            il.Emit(System.Reflection.Emit.OpCodes.Call, sqrt);
            il.Emit(System.Reflection.Emit.OpCodes.Div);
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
            il.Emit(System.Reflection.Emit.OpCodes.Conv_R8); // convert to float64
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 2.0); // * 2
            il.Emit(System.Reflection.Emit.OpCodes.Mul);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // - 1
            il.Emit(System.Reflection.Emit.OpCodes.Sub);
            il.Emit(System.Reflection.Emit.OpCodes.Neg); // * -1
            return;
          }
        case OpCodes.XOR: {
            CompileInstructions(il, state, ds);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
            il.Emit(System.Reflection.Emit.OpCodes.Cgt);// > 0

            for (int i = 1; i < nArgs; i++) {
              CompileInstructions(il, state, ds);
              il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
              il.Emit(System.Reflection.Emit.OpCodes.Cgt);// > 0
              il.Emit(System.Reflection.Emit.OpCodes.Xor);
            }
            il.Emit(System.Reflection.Emit.OpCodes.Conv_R8); // convert to float64

            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 2.0); // * 2
            il.Emit(System.Reflection.Emit.OpCodes.Mul);
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, 1.0); // - 1
            il.Emit(System.Reflection.Emit.OpCodes.Sub);
            return;
          }
        case OpCodes.GT: {
            CompileInstructions(il, state, ds);
            CompileInstructions(il, state, ds);

            il.Emit(System.Reflection.Emit.OpCodes.Cgt); // 1 (>) / 0 (otherwise)
            il.Emit(System.Reflection.Emit.OpCodes.Conv_R8); // convert to float64
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
            il.Emit(System.Reflection.Emit.OpCodes.Conv_R8); // convert to float64
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
            throw new NotSupportedException(
              "Automatically defined functions are not supported by the SymbolicDataAnalysisTreeILEmittingInterpreter. Either turn of ADFs or change the interpeter.");
          }
        case OpCodes.Arg: {
            throw new NotSupportedException(
              "Automatically defined functions are not supported by the SymbolicDataAnalysisTreeILEmittingInterpreter. Either turn of ADFs or change the interpeter.");
          }
        case OpCodes.Variable: {
            VariableTreeNode varNode = (VariableTreeNode)currentInstr.dynamicNode;
            il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1); // load columns array
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, (int)currentInstr.data);
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
            il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, (int)currentInstr.data);
            // load correct column of the current variable
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
            throw new NotSupportedException("Interpretation of symbol " + currentInstr.dynamicNode.Symbol.Name +
                                            " is not supported by the SymbolicDataAnalysisTreeILEmittingInterpreter");
          }
        default:
          throw new NotSupportedException("Interpretation of symbol " + currentInstr.dynamicNode.Symbol.Name +
                                          " is not supported by the SymbolicDataAnalysisTreeILEmittingInterpreter");
      }
    }

    public static double AiryA(double x) {
      if (double.IsNaN(x)) return double.NaN;
      double ai, aip, bi, bip;
      alglib.airy(x, out ai, out aip, out bi, out bip);
      return ai;
    }

    public static double AiryB(double x) {
      if (double.IsNaN(x)) return double.NaN;
      double ai, aip, bi, bip;
      alglib.airy(x, out ai, out aip, out bi, out bip);
      return bi;
    }
    public static double Dawson(double x) {
      if (double.IsNaN(x)) return double.NaN;
      return alglib.dawsonintegral(x);
    }

    public static double Gamma(double x) {
      if (double.IsNaN(x)) return double.NaN;
      return alglib.gammafunction(x);
    }

    public static double Psi(double x) {
      if (double.IsNaN(x)) return double.NaN;
      else if (x <= 0 && (Math.Floor(x) - x).IsAlmost(0)) return double.NaN;
      return alglib.psi(x);
    }

    public static double ExpIntegralEi(double x) {
      if (double.IsNaN(x)) return double.NaN;
      return alglib.exponentialintegralei(x);
    }

    public static double SinIntegral(double x) {
      if (double.IsNaN(x)) return double.NaN;
      double si, ci;
      alglib.sinecosineintegrals(x, out si, out ci);
      return si;
    }

    public static double CosIntegral(double x) {
      if (double.IsNaN(x)) return double.NaN;
      double si, ci;
      alglib.sinecosineintegrals(x, out si, out ci);
      return ci;
    }

    public static double HypSinIntegral(double x) {
      if (double.IsNaN(x)) return double.NaN;
      double shi, chi;
      alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
      return shi;
    }

    public static double HypCosIntegral(double x) {
      if (double.IsNaN(x)) return double.NaN;
      double shi, chi;
      alglib.hyperbolicsinecosineintegrals(x, out shi, out chi);
      return chi;
    }

    public static double FresnelCosIntegral(double x) {
      if (double.IsNaN(x)) return double.NaN;
      double c = 0, s = 0;
      alglib.fresnelintegral(x, ref c, ref s);
      return c;
    }

    public static double FresnelSinIntegral(double x) {
      if (double.IsNaN(x)) return double.NaN;
      double c = 0, s = 0;
      alglib.fresnelintegral(x, ref c, ref s);
      return s;
    }

    public static double Norm(double x) {
      if (double.IsNaN(x)) return double.NaN;
      return alglib.normaldistribution(x);
    }

    public static double Erf(double x) {
      if (double.IsNaN(x)) return double.NaN;
      return alglib.errorfunction(x);
    }

    public static double Bessel(double x) {
      if (double.IsNaN(x)) return double.NaN;
      return alglib.besseli0(x);
    }
  }
}
