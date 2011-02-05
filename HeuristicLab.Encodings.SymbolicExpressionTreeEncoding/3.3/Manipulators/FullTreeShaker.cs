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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Manipulators {
  [StorableClass]
  [Item("FullTreeShaker", "Manipulates all nodes that have local parameters.")]
  public sealed class FullTreeShaker : SymbolicExpressionTreeManipulator {

    [StorableConstructor]
    private FullTreeShaker(bool deserializing) : base(deserializing) { }
    private FullTreeShaker(FullTreeShaker original, Cloner cloner) : base(original, cloner) { }
    public FullTreeShaker() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new FullTreeShaker(this, cloner);
    }

    protected override void Manipulate(IRandom random, SymbolicExpressionTree symbolicExpressionTree, ISymbolicExpressionGrammar grammar, IntValue maxTreeSize, IntValue maxTreeHeight, out bool success) {
      foreach (var node in symbolicExpressionTree.IterateNodesPrefix()) {
        if (node.HasLocalParameters) {
          node.ShakeLocalParameters(random, 1.0);
        }
      }
      success = true;
    }
  }
}
