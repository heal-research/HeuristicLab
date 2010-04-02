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
using HeuristicLab.Core;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Evaluates FunctionTrees recursively by interpretation of the function symbols in each node.
  /// Simple unoptimized code, general symbolic expressions only.
  /// Not thread-safe!
  /// </summary>
  [StorableClass]
  [Item("GeneralSymbolicExpressionTreeEvaluator", "Default evaluator for symbolic expression trees.")]
  public class GeneralSymbolicExpressionTreeEvaluator : Item {
    public virtual double Evaluate(SymbolicExpressionTreeNode node) {
      if (node.Symbol is Addition) {
        return Evaluate(node.SubTrees[0]) + Evaluate(node.SubTrees[1]);
      } else if (node.Symbol is Subtraction) {
        return Evaluate(node.SubTrees[0]) - Evaluate(node.SubTrees[1]);
      } else if (node.Symbol is Multiplication) {
        return Evaluate(node.SubTrees[0]) * Evaluate(node.SubTrees[1]);
      } else if (node.Symbol is Division) {
        return Evaluate(node.SubTrees[0]) / Evaluate(node.SubTrees[1]);
      } else {
        throw new NotSupportedException("Tree contains unknown symbol: " + node.Symbol.Name);
      }
    }
  }
}
