#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      var child = new LinearLinkage(length);
      var endNodes = new HashSet<int>();
      for (var i = 0; i < length; i++) {
        if ((p1[i] == i && p2[i] == i)
          || ((p1[i] == i || p2[i] == i) && random.NextDouble() < 0.5)) {
          child[i] = i;
          endNodes.Add(i);
        }
      }
      for (var i = 0; i < length; i++) {
        if (endNodes.Contains(i)) continue;
        var p1End = endNodes.Contains(p1[i]);
        var p2End = endNodes.Contains(p2[i]);
        if ((p1End && p2End) || (!p1End && !p2End)) {
          child[i] = random.NextDouble() < 0.5 ? p1[i] : p2[i];
        } else if (p1End) {
          child[i] = p1[i];
        } else {
          child[i] = p2[i];
        }
      }
      child.LinearizeTreeStructures();
      return child;
    }

    protected override LinearLinkage Cross(IRandom random, ItemArray<LinearLinkage> parents) {
      if (parents.Length != 2) throw new InvalidOperationException(Name + ": Can only cross exactly two parents.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
