#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Compiler;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SimpleArithmeticExpressionInterpreter", "Interpreter for arithmetic symbolic expression trees including function calls.")]
  public sealed class SimpleArithmeticExpressionInterpreter : NamedItem, ISymbolicExpressionTreeInterpreter {
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
    }

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
      { typeof(HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable), OpCodes.Variable },
      { typeof(LaggedVariable), OpCodes.LagVariable },
      { typeof(Constant), OpCodes.Constant },
      { typeof(Argument), OpCodes.Arg },
    };
    private const int ARGUMENT_STACK_SIZE = 1024;

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [StorableConstructor]
    private SimpleArithmeticExpressionInterpreter(bool deserializing) : base(deserializing) { }
    private SimpleArithmeticExpressionInterpreter(SimpleArithmeticExpressionInterpreter original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SimpleArithmeticExpressionInterpreter(this, cloner);
    }

    public SimpleArithmeticExpressionInterpreter()
      : base() {
    }

    public IEnumerable<double> GetSymbolicExpressionTreeValues(SymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows) {
      var compiler = new SymbolicExpressionTreeCompiler();
      Instruction[] code = compiler.Compile(tree, MapSymbolToOpCode);

      for (int i = 0; i < code.Length; i++) {
        Instruction instr = code[i];
        if (instr.opCode == OpCodes.Variable) {
          var variableTreeNode = instr.dynamicNode as VariableTreeNode;
          instr.iArg0 = (ushort)dataset.GetVariableIndex(variableTreeNode.VariableName);
          code[i] = instr;
        } else if (instr.opCode == OpCodes.LagVariable) {
          var variableTreeNode = instr.dynamicNode as LaggedVariableTreeNode;
          instr.iArg0 = (ushort)dataset.GetVariableIndex(variableTreeNode.VariableName);
          code[i] = instr;
        }
      }

      double[] argumentStack = new double[ARGUMENT_STACK_SIZE];
      foreach (var rowEnum in rows) {
        int row = rowEnum;
        int pc = 0;
        int argStackPointer = 0;
        yield return Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
      }
    }

    private double Evaluate(Dataset dataset, ref int row, Instruction[] code, ref int pc, double[] argumentStack, ref int argStackPointer) {
      Instruction currentInstr = code[pc++];
      switch (currentInstr.opCode) {
        case OpCodes.Add: {
            double s = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              s += Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            }
            return s;
          }
        case OpCodes.Sub: {
            double s = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              s -= Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            }
            if (currentInstr.nArguments == 1) s = -s;
            return s;
          }
        case OpCodes.Mul: {
            double p = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              p *= Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            }
            return p;
          }
        case OpCodes.Div: {
            double p = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              p /= Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            }
            if (currentInstr.nArguments == 1) p = 1.0 / p;
            return p;
          }
        case OpCodes.Average: {
            double sum = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              sum += Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            }
            return sum / currentInstr.nArguments;
          }
        case OpCodes.Cos: {
            return Math.Cos(Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer));
          }
        case OpCodes.Sin: {
            return Math.Sin(Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer));
          }
        case OpCodes.Tan: {
            return Math.Tan(Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer));
          }
        case OpCodes.Exp: {
            return Math.Exp(Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer));
          }
        case OpCodes.Log: {
            return Math.Log(Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer));
          }
        case OpCodes.IfThenElse: {
            double condition = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            double result;
            if (condition > 0.0) {
              result = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer); SkipBakedCode(code, ref pc);
            } else {
              SkipBakedCode(code, ref pc); result = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            }
            return result;
          }
        case OpCodes.AND: {
            double result = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              if (result <= 0.0) SkipBakedCode(code, ref pc);
              else {
                result = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
              }
            }
            return result <= 0.0 ? -1.0 : 1.0;
          }
        case OpCodes.OR: {
            double result = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            for (int i = 1; i < currentInstr.nArguments; i++) {
              if (result > 0.0) SkipBakedCode(code, ref pc);
              else {
                result = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
              }
            }
            return result > 0.0 ? 1.0 : -1.0;
          }
        case OpCodes.NOT: {
            return -Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
          }
        case OpCodes.GT: {
            double x = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            double y = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            if (x > y) return 1.0;
            else return -1.0;
          }
        case OpCodes.LT: {
            double x = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            double y = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            if (x < y) return 1.0;
            else return -1.0;
          }
        case OpCodes.Call: {
            // evaluate sub-trees
            // push on argStack in reverse order 
            for (int i = 0; i < currentInstr.nArguments; i++) {
              argumentStack[argStackPointer + currentInstr.nArguments - i] = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);
            }
            argStackPointer += currentInstr.nArguments;

            // save the pc
            int nextPc = pc;
            // set pc to start of function  
            pc = currentInstr.iArg0;
            // evaluate the function
            double v = Evaluate(dataset, ref row, code, ref pc, argumentStack, ref argStackPointer);

            // decrease the argument stack pointer by the number of arguments pushed
            // to set the argStackPointer back to the original location
            argStackPointer -= currentInstr.nArguments;

            // restore the pc => evaluation will continue at point after my subtrees  
            pc = nextPc;
            return v;
          }
        case OpCodes.Arg: {
            return argumentStack[argStackPointer - currentInstr.iArg0];
          }
        case OpCodes.Variable: {
            var variableTreeNode = currentInstr.dynamicNode as VariableTreeNode;
            return dataset[row, currentInstr.iArg0] * variableTreeNode.Weight;
          }
        case OpCodes.LagVariable: {
            var laggedVariableTreeNode = currentInstr.dynamicNode as LaggedVariableTreeNode;
            int actualRow = row + laggedVariableTreeNode.Lag;
            if (actualRow < 0 || actualRow >= dataset.Rows) throw new ArgumentException("Out of range access to dataset row: " + row);
            return dataset[actualRow, currentInstr.iArg0] * laggedVariableTreeNode.Weight;
          }
        case OpCodes.Constant: {
            var constTreeNode = currentInstr.dynamicNode as ConstantTreeNode;
            return constTreeNode.Value;
          }
        default: throw new NotSupportedException();
      }
    }

    private byte MapSymbolToOpCode(SymbolicExpressionTreeNode treeNode) {
      if (symbolToOpcode.ContainsKey(treeNode.Symbol.GetType()))
        return symbolToOpcode[treeNode.Symbol.GetType()];
      else
        throw new NotSupportedException("Symbol: " + treeNode.Symbol);
    }

    // skips a whole branch
    private void SkipBakedCode(Instruction[] code, ref int pc) {
      int i = 1;
      while (i > 0) {
        i += code[pc++].nArguments;
        i--;
      }
    }
  }
}
