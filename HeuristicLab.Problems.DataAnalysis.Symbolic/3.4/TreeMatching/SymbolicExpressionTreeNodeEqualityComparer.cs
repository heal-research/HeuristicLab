#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("SymbolicExpressionTreeNodeEqualityComparer", "An operator that checks node equality based on different similarity measures.")]
  [StorableClass]
  public class SymbolicExpressionTreeNodeEqualityComparer : Item, ISymbolicExpressionTreeNodeSimilarityComparer {
    [StorableConstructor]
    protected SymbolicExpressionTreeNodeEqualityComparer(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeNodeEqualityComparer(SymbolicExpressionTreeNodeEqualityComparer original, Cloner cloner)
      : base(original, cloner) {
      matchConstantValues = original.matchConstantValues;
      matchVariableNames = original.matchVariableNames;
      matchVariableWeights = original.matchVariableWeights;
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicExpressionTreeNodeEqualityComparer(this, cloner); }

    // more flexible matching criteria 
    [Storable]
    private bool matchConstantValues;
    public bool MatchConstantValues {
      get { return matchConstantValues; }
      set { matchConstantValues = value; }
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
      matchConstantValues = true;
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
      var ca = a as ConstantTreeNode;
      if (ca != null) {
        var cb = b as ConstantTreeNode;
        if (cb == null) return false;
        return (!MatchConstantValues || ca.Value.Equals(cb.Value));
      }
      return false;
    }
  }
}
