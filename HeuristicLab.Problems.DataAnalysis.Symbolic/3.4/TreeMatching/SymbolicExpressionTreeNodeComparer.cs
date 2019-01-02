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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("eabf848e-d10c-499e-8c48-c771232ede0e")]
  // this comparer considers that a < b if the type of a is "greater" than the type of b, for example:
  // - A function node is "greater" than a terminal node
  // - A variable terminal is "greater" than a constant terminal
  // - used for bringing subtrees to a "canonical" form when the operation allows reordering of arguments
  public class SymbolicExpressionTreeNodeComparer : ISymbolicExpressionTreeNodeComparer {
    public static int CompareNodes(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      var ta = a as SymbolicExpressionTreeTerminalNode;
      var tb = b as SymbolicExpressionTreeTerminalNode;

      if (ta == null)
        return tb == null ? String.CompareOrdinal(a.Symbol.Name, b.Symbol.Name) : -1;

      if (tb == null)
        return 1;

      // at this point we know a and b are both terminals
      var va = a as VariableTreeNode;
      var vb = b as VariableTreeNode;

      if (va != null)
        return vb == null ? -1 : CompareVariables(va, vb);

      if (vb != null)
        return 1;

      // at this point we know a and b are not variables
      var ca = a as ConstantTreeNode;
      var cb = b as ConstantTreeNode;

      if (ca != null && cb != null)
        return ca.Value.CompareTo(cb.Value);

      // for other unknown terminal types, compare strings
      return string.CompareOrdinal(a.ToString(), b.ToString());
    }

    private static int CompareVariables(VariableTreeNode a, VariableTreeNode b) {
      int result = string.CompareOrdinal(a.VariableName, b.VariableName);
      return result == 0 ? a.Weight.CompareTo(b.Weight) : result;
    }

    public int Compare(ISymbolicExpressionTreeNode x, ISymbolicExpressionTreeNode y) {
      return CompareNodes(x, y);
    }
  }

}
