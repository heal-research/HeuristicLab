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
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Compiler {
  public class SymbolicExpressionTreeCompiler {
    private Dictionary<string, ushort> entryPoint = new Dictionary<string, ushort>();
    private List<Func<Instruction, Instruction>> postInstructionCompiledHooks = new List<Func<Instruction, Instruction>>();

    public Instruction[] Compile(SymbolicExpressionTree tree, Func<SymbolicExpressionTreeNode, byte> opCodeMapper) {
      List<Instruction> code = new List<Instruction>();
      entryPoint.Clear();
      // compile main body branches
      foreach (var branch in tree.Root.SubTrees[0].SubTrees) {
        code.AddRange(Compile(branch, opCodeMapper));
      }
      // compile function branches
      var functionBranches = from node in tree.IterateNodesPrefix()
                             where node.Symbol is Defun
                             select node;
      foreach (DefunTreeNode branch in functionBranches) {
        if (code.Count > ushort.MaxValue) throw new ArgumentException("Code for the tree is too long (> ushort.MaxValue).");
        entryPoint[branch.FunctionName] = (ushort)code.Count;
        code.AddRange(Compile(branch.SubTrees[0], opCodeMapper));
      }
      // address of all functions is fixed now
      // iterate through code again and fill in the jump locations
      for (int i = 0; i < code.Count; i++) {
        Instruction instr = code[i];
        if (instr.dynamicNode.Symbol is InvokeFunction) {
          var invokeNode = (InvokeFunctionTreeNode)instr.dynamicNode;
          instr.iArg0 = entryPoint[invokeNode.Symbol.FunctionName];
          code[i] = instr;
        }
      }

      return code.ToArray();
    }

    private IEnumerable<Instruction> Compile(SymbolicExpressionTreeNode branch, Func<SymbolicExpressionTreeNode, byte> opCodeMapper) {
      foreach (var node in branch.IterateNodesPrefix()) {
        Instruction instr = new Instruction();
        if (node.SubTrees.Count > 255) throw new ArgumentException("Number of subtrees is too big (>255)");
        instr.nArguments = (byte)node.SubTrees.Count;
        instr.opCode = opCodeMapper(node);
        if (node.Symbol is Argument) {
          var argNode = (ArgumentTreeNode)node;
          instr.iArg0 = (ushort)argNode.Symbol.ArgumentIndex;
        }
        instr.dynamicNode = node;
        foreach (var hook in postInstructionCompiledHooks) {
          instr = hook(instr);
        }
        yield return instr;
      }
    }

    /// <summary>
    /// Adds a function that will be called every time an instruction is compiled.
    /// The compiled will insert the instruction returned by the hook into the code.
    /// </summary>
    /// <param name="hook">The hook that should be called for each compiled instruction.</param>
    public void AddInstructionPostProcessingHook(Func<Instruction, Instruction> hook) {
      postInstructionCompiledHooks.Add(hook);
    }
  }
}
