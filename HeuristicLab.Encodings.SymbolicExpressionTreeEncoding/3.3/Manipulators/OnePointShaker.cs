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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Manipulators {
  [StorableClass]
  [Item("OnePointShaker", "Selects a random node with local parameters and manipulates the selected node.")]
  public sealed class OnePointShaker : SymbolicExpressionTreeManipulator {
    [StorableConstructor]
    private OnePointShaker(bool deserializing) : base(deserializing) { }
    private OnePointShaker(OnePointShaker original, Cloner cloner) : base(original, cloner) { }
    public OnePointShaker() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OnePointShaker(this, cloner);
    }

    protected override void Manipulate(IRandom random, SymbolicExpressionTree symbolicExpressionTree, ISymbolicExpressionGrammar grammar, IntValue maxTreeSize, IntValue maxTreeHeight, out bool success) {
      var parametricNodes = from node in symbolicExpressionTree.IterateNodesPrefix()
                            where node.HasLocalParameters
                            select node;
      if (parametricNodes.Count() > 0) {
        SymbolicExpressionTreeNode selectedPoint = parametricNodes.SelectRandom(random);

        selectedPoint.ShakeLocalParameters(random, 1.0);
        success = true;
      } else {
        success = false;
      }
    }
  }
}
