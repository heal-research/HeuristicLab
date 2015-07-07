#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("Greedy Partition Crossover", "The Greedy Partition Crossover (GPX) is implemented as described in Ülker, Ö., Özcan, E., Korkmaz, E. E. 2007. Linear linkage encoding in grouping problems: applications on graph coloring and timetabling. In Practice and Theory of Automated Timetabling VI, pp. 347-363. Springer Berlin Heidelberg.")]
  [StorableClass]
  public sealed class GreedyPartitionCrossover : LinearLinkageCrossover {

    [StorableConstructor]
    private GreedyPartitionCrossover(bool deserializing) : base(deserializing) { }
    private GreedyPartitionCrossover(GreedyPartitionCrossover original, Cloner cloner) : base(original, cloner) { }
    public GreedyPartitionCrossover() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GreedyPartitionCrossover(this, cloner);
    }

    public static LinearLinkage Apply(IRandom random, ItemArray<LinearLinkage> parents) {
      var len = parents[0].Length;

      var child = new LinearLinkage(len);
      var childGroup = new List<HashSet<int>>();
      var currentParent = random.Next(parents.Length);
      var groups = parents.Select(x => x.GetGroups().Select(y => new HashSet<int>(y)).ToList()).ToList();
      bool remaining;
      do {
        var maxGroup = groups[currentParent].Select((v, i) => Tuple.Create(i, v))
          .MaxItems(x => x.Item2.Count)
          .SampleRandom(random).Item1;
        var group = groups[currentParent][maxGroup];
        groups[currentParent].RemoveAt(maxGroup);
        childGroup.Add(group);

        remaining = false;
        for (var p = 0; p < groups.Count; p++) {
          for (var j = 0; j < groups[p].Count; j++) {
            foreach (var elem in group) groups[p][j].Remove(elem);
            if (!remaining && groups[p][j].Count > 0) remaining = true;
          }
        }

        currentParent = (currentParent + 1) % parents.Length;
      } while (remaining);

      child.SetGroups(childGroup);
      return child;
    }

    protected override LinearLinkage Cross(IRandom random, ItemArray<LinearLinkage> parents) {
      return Apply(random, parents);
    }
  }
}
