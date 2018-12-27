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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Lowest Index First Crossover", "The Lowest Index First Crossover (LIFX) is implemented as described in Ülker, Ö., Özcan, E., Korkmaz, E. E. 2007. Linear linkage encoding in grouping problems: applications on graph coloring and timetabling. In Practice and Theory of Automated Timetabling VI, pp. 347-363. Springer Berlin Heidelberg.")]
  [StorableClass]
  public sealed class LowestIndexFirstCrossover : LinearLinkageCrossover {

    [StorableConstructor]
    private LowestIndexFirstCrossover(bool deserializing) : base(deserializing) { }
    private LowestIndexFirstCrossover(LowestIndexFirstCrossover original, Cloner cloner) : base(original, cloner) { }
    public LowestIndexFirstCrossover() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LowestIndexFirstCrossover(this, cloner);
    }

    public static LinearLinkage Apply(IRandom random, ItemArray<LinearLinkage> parents) {
      var len = parents[0].Length;
      var p = random.Next(parents.Length);
      var child = LinearLinkage.SingleElementGroups(len);
      var remaining = new SortedSet<int>(Enumerable.Range(0, len));
      do {
        var i = remaining.Min;
        foreach (var g in parents[p].GetGroupForward(i)) {
          if (!remaining.Contains(g)) continue;
          child[i] = g;
          i = g;
          remaining.Remove(g);
        }
        child[i] = i;
        remaining.Remove(i);

        p = (p + 1) % parents.Length;
      } while (remaining.Count > 0);

      return child;
    }

    protected override LinearLinkage Cross(IRandom random, ItemArray<LinearLinkage> parents) {
      return Apply(random, parents);
    }
  }
}
