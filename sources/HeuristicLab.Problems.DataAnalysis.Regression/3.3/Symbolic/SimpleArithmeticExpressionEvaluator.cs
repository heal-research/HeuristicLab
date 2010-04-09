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
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
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
        var estimatedValue = Evaluate();
        if (double.IsNaN(estimatedValue) || double.IsInfinity(estimatedValue)) yield return 0.0;
        else yield return estimatedValue;
      }
    }

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
        case CodeSymbol.Dynamic: {
            if (currentInstr.dynamicNode is VariableTreeNode) {
              var variableTreeNode = currentInstr.dynamicNode as VariableTreeNode;
              return dataset[row, dataset.GetVariableIndex(variableTreeNode.VariableName)] * variableTreeNode.Weight;
            } else if (currentInstr.dynamicNode is ConstantTreeNode) {
              return ((ConstantTreeNode)currentInstr.dynamicNode).Value;
            } else throw new NotSupportedException();
          }
        default: throw new NotSupportedException();
      }
    }
  }
}
