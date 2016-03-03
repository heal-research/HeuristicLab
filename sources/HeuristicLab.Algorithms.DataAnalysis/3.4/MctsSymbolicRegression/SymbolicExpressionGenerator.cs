#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics.Contracts;
using System.Linq;

using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression {

  // translates byte code to a symbolic expression tree
  internal class SymbolicExpressionTreeGenerator {
    const int MaxStackSize = 100;
    private readonly ISymbolicExpressionTreeNode[] stack;
    private readonly ConstantTreeNode const0;
    private readonly ConstantTreeNode const1;
    private readonly Addition addSy;
    private readonly Multiplication mulSy;
    private readonly Exponential expSy;
    private readonly Logarithm logSy;
    private readonly Division divSy;
    private readonly VariableTreeNode varNode;
    private readonly string[] variableNames;
    private readonly StartSymbol startSy;
    private readonly ProgramRootSymbol progRootSy;

    public SymbolicExpressionTreeGenerator(string[] variableNames) {
      stack = new ISymbolicExpressionTreeNode[MaxStackSize];
      var grammar = new TypeCoherentExpressionGrammar();
      this.variableNames = variableNames;

      grammar.ConfigureAsDefaultRegressionGrammar();
      const0 = (ConstantTreeNode)grammar.Symbols.OfType<Constant>().First().CreateTreeNode();
      const0.Value = 0;
      const1 = (ConstantTreeNode)grammar.Symbols.OfType<Constant>().First().CreateTreeNode();
      const1.Value = 1;
      varNode = (VariableTreeNode)grammar.Symbols.OfType<Variable>().First().CreateTreeNode();

      addSy = grammar.AllowedSymbols.OfType<Addition>().First();
      mulSy = grammar.AllowedSymbols.OfType<Multiplication>().First();
      logSy = grammar.AllowedSymbols.OfType<Logarithm>().First();
      expSy = grammar.AllowedSymbols.OfType<Exponential>().First();
      divSy = grammar.AllowedSymbols.OfType<Division>().First();

      progRootSy = grammar.AllowedSymbols.OfType<ProgramRootSymbol>().First();
      startSy = grammar.AllowedSymbols.OfType<StartSymbol>().First();
    }

    public ISymbolicExpressionTreeNode Exec(byte[] code, double[] consts, int nParams, double[] scalingFactor, double[] scalingOffset) {
      int topOfStack = -1;
      int pc = 0;
      int nextParamIdx = -1;
      OpCodes op;
      short arg;
      while (true) {
        ReadNext(code, ref pc, out op, out arg);
        switch (op) {
          case OpCodes.Nop: break;
          case OpCodes.LoadConst0: {
              ++topOfStack;
              stack[topOfStack] = (ISymbolicExpressionTreeNode)const0.Clone();
              break;
            }
          case OpCodes.LoadConst1: {
              ++topOfStack;
              stack[topOfStack] = (ISymbolicExpressionTreeNode)const1.Clone();
              break;
            }
          case OpCodes.LoadParamN: {
              ++topOfStack;
              var p = (ConstantTreeNode)const1.Clone(); // value will be tuned later (evaluator and tree generator both use 1 as initial values)
              p.Value = consts[++nextParamIdx];
              stack[topOfStack] = p;
              break;
            }
          case OpCodes.LoadVar:
            ++topOfStack;
            if (scalingOffset != null) {
              var sumNode = addSy.CreateTreeNode();
              var varNode = (VariableTreeNode)this.varNode.Clone();
              var constNode = (ConstantTreeNode)const0.Clone();
              varNode.Weight = scalingFactor[arg];
              varNode.VariableName = variableNames[arg];
              constNode.Value = scalingOffset[arg];
              sumNode.AddSubtree(varNode);
              sumNode.AddSubtree(constNode);
              stack[topOfStack] = sumNode;
            } else {
              var varNode = (VariableTreeNode)this.varNode.Clone();
              varNode.Weight = 1.0;
              varNode.VariableName = variableNames[arg];
              stack[topOfStack] = varNode;
            }
            break;
          case OpCodes.Add: {
              var t1 = stack[topOfStack];
              var t2 = stack[topOfStack - 1];
              topOfStack--;
              if (t2.Symbol is Addition) {
                t2.AddSubtree(t1);
              } else {
                var addNode = addSy.CreateTreeNode();
                addNode.AddSubtree(t1);
                addNode.AddSubtree(t2);
                stack[topOfStack] = addNode;
              }
              break;
            }
          case OpCodes.Mul: {
              var t1 = stack[topOfStack];
              var t2 = stack[topOfStack - 1];
              topOfStack--;
              if (t2.Symbol is Multiplication) {
                t2.AddSubtree(t1);
              } else {
                var mulNode = mulSy.CreateTreeNode();
                mulNode.AddSubtree(t1);
                mulNode.AddSubtree(t2);
                stack[topOfStack] = mulNode;
              }
              break;
            }
          case OpCodes.Log: {
              var v1 = stack[topOfStack];
              var logNode = logSy.CreateTreeNode();
              logNode.AddSubtree(v1);
              stack[topOfStack] = logNode;
              break;
            }
          case OpCodes.Exp: {
              var v1 = stack[topOfStack];
              var expNode = expSy.CreateTreeNode();
              expNode.AddSubtree(v1);
              stack[topOfStack] = expNode;
              break;
            }
          case OpCodes.Inv: {
              var v1 = stack[topOfStack];
              var divNode = divSy.CreateTreeNode();
              divNode.AddSubtree(v1);
              stack[topOfStack] = divNode;
              break;
            }
          case OpCodes.Exit:
            Contract.Assert(topOfStack == 0);
            var rootNode = progRootSy.CreateTreeNode();
            var startNode = startSy.CreateTreeNode();
            startNode.AddSubtree(stack[topOfStack]);
            rootNode.AddSubtree(startNode);
            return rootNode;
        }
      }
    }

    private void ReadNext(byte[] code, ref int pc, out OpCodes op, out short s) {
      op = (OpCodes)Enum.ToObject(typeof(OpCodes), code[pc++]);
      s = 0;
      if (op == OpCodes.LoadVar) {
        s = (short)(((short)code[pc] << 8) | (short)code[pc + 1]);
        pc += 2;
      }
    }
  }
}
