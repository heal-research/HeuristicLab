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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Lowest Index Max Crossover", "The Lowest Index Max Crossover (LIMX) is implemented as described in Ülker, Ö., Özcan, E., Korkmaz, E. E. 2007. Linear linkage encoding in grouping problems: applications on graph coloring and timetabling. In Practice and Theory of Automated Timetabling VI, pp. 347-363. Springer Berlin Heidelberg.")]
  [StorableClass]
  public sealed class LowestIndexMaxCrossover : LinearLinkageCrossover {

    [StorableConstructor]
    private LowestIndexMaxCrossover(bool deserializing) : base(deserializing) { }
    private LowestIndexMaxCrossover(LowestIndexMaxCrossover original, Cloner cloner) : base(original, cloner) { }
    public LowestIndexMaxCrossover() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LowestIndexMaxCrossover(this, cloner);
    }

    public static LinearLinkage Apply(IRandom random, ItemArray<LinearLinkage> parents) {
      var len = parents[0].Length;
      var child = LinearLinkage.SingleElementGroups(len);
      var remaining = new SortedSet<int>(Enumerable.Range(0, len));
      do {
        var groups = parents.Select(x => x.GetGroupForward(remaining.Min).Where(y => remaining.Contains(y)).ToList()).ToList();
        var max = groups.Select((v, idx) => Tuple.Create(idx, v.Count)).MaxItems(x => x.Item2).SampleRandom(random).Item1;
        var i = groups[max][0];
        for (var k = 1; k < groups[max].Count; k++) {
          child[i] = groups[max][k];
          remaining.Remove(i);
          i = child[i];
        }
        child[i] = i;
        remaining.Remove(i);
      } while (remaining.Count > 0);

      return child;
    }

    protected override LinearLinkage Cross(IRandom random, ItemArray<LinearLinkage> parents) {
      return Apply(random, parents);
    }
  }
}
