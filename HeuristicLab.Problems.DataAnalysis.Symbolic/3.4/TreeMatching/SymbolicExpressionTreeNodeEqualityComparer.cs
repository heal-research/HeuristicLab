﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("SymbolicExpressionTreeNodeEqualityComparer", "An operator that checks node equality based on different similarity measures.")]
  [StorableType("F5BC06AA-3F08-4692-93E8-E44CE8205A46")]
  public class SymbolicExpressionTreeNodeEqualityComparer : Item, ISymbolicExpressionTreeNodeSimilarityComparer {
    [StorableConstructor]
    protected SymbolicExpressionTreeNodeEqualityComparer(StorableConstructorFlag _) : base(_) { }
    protected SymbolicExpressionTreeNodeEqualityComparer(SymbolicExpressionTreeNodeEqualityComparer original, Cloner cloner)
      : base(original, cloner) {
      matchNumericValues = original.matchNumericValues;
      matchVariableNames = original.matchVariableNames;
      matchVariableWeights = original.matchVariableWeights;
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicExpressionTreeNodeEqualityComparer(this, cloner); }

    // more flexible matching criteria 
    [Storable]
    private bool matchNumericValues;
    public bool MatchNumericValues {
      get { return matchNumericValues; }
      set { matchNumericValues = value; }
    }

    [Storable]
    private bool matchVariableNames;
    public bool MatchVariableNames {
      get { return matchVariableNames; }
      set { matchVariableNames = value; }
    }

    [Storable]
    private bool matchVariableWeights;
    public bool MatchVariableWeights {
      get { return matchVariableWeights; }
      set { matchVariableWeights = value; }
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    public SymbolicExpressionTreeNodeEqualityComparer() {
      matchNumericValues = true;
      matchVariableNames = true;
      matchVariableWeights = true;
    }

    public int GetHashCode(ISymbolicExpressionTreeNode n) {
      return n.ToString().ToLower().GetHashCode();
    }

    public bool Equals(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (!(a is SymbolicExpressionTreeTerminalNode))
        // if a and b are non terminal nodes, check equality of symbol names
        return !(b is SymbolicExpressionTreeTerminalNode) && a.Symbol.Name.Equals(b.Symbol.Name);
      var va = a as VariableTreeNode;
      if (va != null) {
        var vb = b as VariableTreeNode;
        if (vb == null) return false;

        return (!MatchVariableNames || va.VariableName.Equals(vb.VariableName)) && (!MatchVariableWeights || va.Weight.Equals(vb.Weight));
      }

      if (a is INumericTreeNode ca) {
        var cb = b as INumericTreeNode;
        if (cb == null) return false;
        return (!MatchNumericValues || ca.Value.Equals(cb.Value));
      }
      return false;
    }
  }
}
