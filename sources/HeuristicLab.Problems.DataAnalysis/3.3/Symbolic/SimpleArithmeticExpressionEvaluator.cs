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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SimpleArithmeticExpressionEvaluator", "Default evaluator for arithmetic symbolic expression trees.")]
  public class SimpleArithmeticExpressionEvaluator {
    private Dataset dataset;
    private int row;
    private Instruction[] code;
    private int pc;
    public IEnumerable<double> EstimatedValues(SymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows) {
      this.dataset = dataset;
      var compiler = new SymbolicExpressionTreeCompiler();
      code = compiler.Compile(tree);
      foreach (var row in rows) {
        this.row = row;
        pc = 0;
        arguments.Clear();
        var estimatedValue = Evaluate();
        if (double.IsNaN(estimatedValue) || double.IsInfinity(estimatedValue)) yield return 0.0;
        else yield return estimatedValue;
      }
    }

    private List<double> arguments = new List<double>();
    public double Evaluate() {
      var currentInstr = code[pc++];
      switch (currentInstr.symbol) {
        case CodeSymbol.Add: {
            double s = 0.0;
            for (int i = 0; i < currentInstr.nArguments; i++) {
              s += Evaluate();
            }
            return s;
          }
        case CodeSymbol.Sub: {
            double s = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              s -= Evaluate();
            }
            return s;
          }
        case CodeSymbol.Mul: {
            double p = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              p *= Evaluate();
            }
            return p;
          }
        case CodeSymbol.Div: {
            double p = Evaluate();
            for (int i = 1; i < currentInstr.nArguments; i++) {
              p /= Evaluate();
            }
            return p;
          }
        case CodeSymbol.Call: {
            // save current arguments
            var oldArgs = new List<double>(arguments);
            arguments.Clear();
            // evaluate sub-trees
            for (int i = 0; i < currentInstr.nArguments; i++) {
              arguments.Add(Evaluate());
            }
            // save the pc
            int nextPc = pc;
            // set pc to start of function  
            pc = currentInstr.iArg0;
            // evaluate the function
            double v = Evaluate();
            // restore the pc => evaluation will continue at point after my subtrees  
            pc = nextPc;
            // restore arguments
            arguments = oldArgs;
            return v;
          }
        case CodeSymbol.Arg: {
            return arguments[currentInstr.iArg0];
          }
        case CodeSymbol.Dynamic: {
            var variableTreeNode = currentInstr.dynamicNode as VariableTreeNode;
            if (variableTreeNode != null) {
              return dataset[row, dataset.GetVariableIndex(variableTreeNode.VariableName)] * variableTreeNode.Weight;
            }
            var constTreeNode = currentInstr.dynamicNode as ConstantTreeNode;
            if (constTreeNode != null) {
              return constTreeNode.Value;
            } else throw new NotSupportedException();
          }
        default: throw new NotSupportedException();
      }
    }
  }
}
