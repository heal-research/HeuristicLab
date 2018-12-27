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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Group Crossover", "The Group Crossover is implemented as described in Korkmaz, E.E. 2010. Multi-objective Genetic Algorithms for grouping problems. Applied Intelligence 33(2), pp. 179-192.")]
  [StorableClass]
  public sealed class GroupCrossover : LinearLinkageCrossover {

    [StorableConstructor]
    private GroupCrossover(bool deserializing) : base(deserializing) { }
    private GroupCrossover(GroupCrossover original, Cloner cloner) : base(original, cloner) { }
    public GroupCrossover() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GroupCrossover(this, cloner);
    }
    
    public static LinearLinkage Apply(IRandom random, LinearLinkage p1, LinearLinkage p2) {
      var length = p1.Length;
      var lleeP1 = p1.ToEndLinks();
      var lleeP2 = p2.ToEndLinks();
      var lleeChild = new int[length];
      var isTransfered = new bool[length];

      for (var i = p1.Length - 1; i >= 0; i--) {
        lleeChild[i] = i;

        // Step 1
        var isEndP1 = p1[i] == i;
        var isEndP2 = p2[i] == i;
        if (isEndP1 & isEndP2 || (isEndP1 | isEndP2 && random.NextDouble() < 0.5)) {
          isTransfered[i] = true;
          continue;
        }

        // Step 2
        var end1 = lleeP1[i];
        var end2 = lleeP2[i];

        if (isTransfered[end1] & isTransfered[end2]) {
          var end = random.NextDouble() < 0.5 ? end1 : end2;
          lleeChild[i] = end;
        } else if (isTransfered[end1]) {
          lleeChild[i] = end1;
        } else if (isTransfered[end2]) {
          lleeChild[i] = end2;
        } else {
          var next = random.NextDouble() < 0.5 ? p1[i] : p2[i];
          var end = lleeChild[next];
          lleeChild[i] = end;
        }
      }
      return LinearLinkage.FromEndLinks(lleeChild);
    }

    protected override LinearLinkage Cross(IRandom random, ItemArray<LinearLinkage> parents) {
      if (parents.Length != 2) throw new InvalidOperationException(Name + ": Can only cross exactly two parents.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
