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
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Evaluates FunctionTrees recursively by interpretation of the function symbols in each node.
  /// Simple unoptimized code, arithmetic expressions only.
  /// Not thread-safe!
  /// </summary>
  [StorableClass]
  [Item("SimpleArithmeticExpressionEvaluator", "Default evaluator for arithmetic symbolic expression trees.")]
  public class SimpleArithmeticExpressionEvaluator : Item {
    public IEnumerable<double> EstimatedValues(SymbolicExpressionTree tree, Dataset dataset, IEnumerable<int> rows) {
      foreach (var row in rows) {
        var estimatedValue = Evaluate(tree.Root.SubTrees[0], dataset, row);
        if (double.IsNaN(estimatedValue) || double.IsInfinity(estimatedValue)) yield return 0.0;
        else yield return estimatedValue;
      }
    }

    private double Evaluate(SymbolicExpressionTreeNode node, Dataset dataset, int row) {
      if (node.Symbol is HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable) {
        var variableTreeNode = node as VariableTreeNode;
        return dataset[row, 1 /*dataset.VariableIndex(variableTreeNode.VariableName)*/] * 1.0; //variableTreeNode.Weight;
      } else if (node.Symbol is Constant) {
        return ((ConstantTreeNode)node).Value;
      } else if (node.Symbol is Addition) {
        return Evaluate(node.SubTrees[0], dataset, row) + Evaluate(node.SubTrees[1], dataset, row);
      } else if (node.Symbol is Subtraction) {
        return Evaluate(node.SubTrees[0], dataset, row) - Evaluate(node.SubTrees[1], dataset, row);
      } else if (node.Symbol is Multiplication) {
        return Evaluate(node.SubTrees[0], dataset, row) * Evaluate(node.SubTrees[1], dataset, row);
      } else if (node.Symbol is Division) {
        return Evaluate(node.SubTrees[0], dataset, row) / Evaluate(node.SubTrees[1], dataset, row);
      } else {
        throw new NotSupportedException("Tree contains unknown symbol: " + node.Symbol.Name);
      }
    }
  }
}
