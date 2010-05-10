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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Core;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Compiler;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SimpleArithmeticExpressionInterpreter", "Interpreter for arithmetic symbolic expression trees including function calls.")]
  // not thread safe!
  public class SimpleArithmeticExpressionInterpreter : NamedItem, ISymbolicExpressionTreeInterpreter {
    private class OpCodes {
      public const byte Add = 1;
      public const byte Sub = 2;
      public const byte Mul = 3;
      public const byte Div = 4;
      public const byte Variable = 5;
      public const byte Constant = 6;
      public const byte Call = 100;
      public const byte Arg = 101;
    }

    private const int ARGUMENT_STACK_SIZE = 1024;

    private Dataset dataset;
    private int row;
    private Instruction[] code;
    private int pc;

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    public SimpleArithmeticExpressionInterpreter()
      : base() {
    }

    public IEnumerable<double> GetSymbolicExpressionTreeValues(SymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows) {
      this.dataset = dataset;
      var compiler = new SymbolicExpressionTreeCompiler();
      compiler.AddInstructionPostProcessingHook(PostProcessInstruction);
      code = compiler.Compile(tree, MapSymbolToOpCode);
      foreach (var row in rows) {
        this.row = row;
        pc = 0;
        argStackPointer = 0;
        yield return Evaluate();
      }
    }

    private Instruction PostProcessInstruction(Instruction instr) {
      if (instr.opCode == OpCodes.Variable) {
        var variableTreeNode = instr.dynamicNode as VariableTreeNode;
        instr.iArg0 = (ushort)dataset.GetVariableIndex(variableTreeNode.VariableName);
      } 
      return instr;
    }

    private byte MapSymbolToOpCode(SymbolicExpressionTreeNode treeNode) {
      if (treeNode.Symbol is Addition) return OpCodes.Add;
      if (treeNode.Symbol is Subtraction) return OpCodes.Sub;
      if (treeNode.Symbol is Multiplication) return OpCodes.Mul;
      if (treeNode.Symbol is Division) return OpCodes.Div;
      if (treeNode.Symbol is HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable) return OpCodes.Variable;
      if (treeNode.Symbol is Constant) return OpCodes.Constant;
      if (treeNode.Symbol is InvokeFunction) return OpCodes.Call;
      if (treeNode.Symbol is Argument) return OpCodes.Arg;
      throw new NotSupportedException("Symbol: " + treeNode.Symbol);
    }

    private double[] argumentStack = new double[ARGUMENT_STACK_SIZE];
    private int argStackPointer;

    public double Evaluate() {
      var currentInstr = code[pc++];
      switch (currentInstr.opCode) {
        case OpCodes.Add: {
            double s = 0.0;
            for (int i = 0; i < currentInstr.nArguments; i++) {
              s += Evaluate();
            }
            return s;
          }
        case OpCodes.Sub: {
            double s = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              s -= Evaluate();
            }
            if (currentInstr.nArguments == 1) s = -s;
            return s;
          }
        case OpCodes.Mul: {
            double p = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              p *= Evaluate();
            }
            return p;
          }
        case OpCodes.Div: {
            double p = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              p /= Evaluate();
            }
            if (currentInstr.nArguments == 1) p = 1.0 / p;
            return p;
          }
        case OpCodes.Call: {
            // evaluate sub-trees
            // push on argStack in reverse order 
            for (int i = 0; i < currentInstr.nArguments; i++) {
              argumentStack[argStackPointer + currentInstr.nArguments - i] = Evaluate();
            }
            argStackPointer += currentInstr.nArguments;

            // save the pc
            int nextPc = pc;
            // set pc to start of function  
            pc = currentInstr.iArg0;
            // evaluate the function
            double v = Evaluate();

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
        case OpCodes.Constant: {
            var constTreeNode = currentInstr.dynamicNode as ConstantTreeNode;
            return constTreeNode.Value;
          }
        default: throw new NotSupportedException();
      }
    }
  }
}
