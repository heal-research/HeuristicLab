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
using System.Linq;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public class SymbolicExpressionTreeCompiler {
    private Dictionary<Type, CodeSymbol> codeSymbol = new Dictionary<Type, CodeSymbol>() {
      {typeof(Addition), CodeSymbol.Add},
      {typeof(Subtraction), CodeSymbol.Sub},
      {typeof(Multiplication), CodeSymbol.Mul},
      {typeof(Division), CodeSymbol.Div},
      {typeof(InvokeFunction), CodeSymbol.Call},
      //{typeof(Values), CodeSymbol.Values}
    };
    private Dictionary<string, short> entryPoint = new Dictionary<string, short>();

    public Instruction[] Compile(SymbolicExpressionTree tree) {
      List<Instruction> code = new List<Instruction>();
      entryPoint.Clear();
      // compile main body
      code.AddRange(Compile(tree.ResultProducingExpression));
      // compile branches
      var functionBranches = from node in tree.IterateNodesPrefix()
                             where node.Symbol is Defun
                             select node;
      foreach (DefunTreeNode branch in functionBranches) {
        entryPoint[branch.FunctionName] = (short)code.Count;
        code.AddRange(Compile(branch));
      }
      return code.ToArray();
    }

    private IEnumerable<Instruction> Compile(SymbolicExpressionTreeNode branch) {
      foreach (var node in IteratePrefix(branch)) {
        Instruction instr = new Instruction();
        if (node.SubTrees.Count > 255) throw new ArgumentException();
        instr.nArguments = (byte)node.SubTrees.Count;
        if (codeSymbol.ContainsKey(node.Symbol.GetType())) {
          instr.symbol = codeSymbol[node.Symbol.GetType()];
          if (instr.symbol == CodeSymbol.Call) {
            var invokeNode = (InvokeFunctionTreeNode)node;
            instr.iArg0 = entryPoint[invokeNode.Symbol.FunctionName];
          }
        } else {
          instr.symbol = CodeSymbol.Dynamic;
          instr.dynamicNode = node;
        }
        yield return instr;
      }
    }

    private IEnumerable<SymbolicExpressionTreeNode> IteratePrefix(SymbolicExpressionTreeNode branch) {
      yield return branch;
      foreach (var subtree in branch.SubTrees) {
        foreach (var node in IteratePrefix(subtree))
          yield return node;
      }
    }
  }
}
