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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Split Group Manipulator", "Performs a maximum of N split operations on the groups. An already split group may be split again.")]
  [StorableType("789781B8-0047-4880-ACAF-077A934AD320")]
  public sealed class SplitGroupManipulator : LinearLinkageManipulator {

    public IValueLookupParameter<IntValue> NParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["N"]; }
    }

    [StorableConstructor]
    private SplitGroupManipulator(StorableConstructorFlag _) : base(_) { }
    private SplitGroupManipulator(SplitGroupManipulator original, Cloner cloner) : base(original, cloner) { }
    public SplitGroupManipulator() {
      Parameters.Add(new ValueLookupParameter<IntValue>("N", "The number of groups to split.", new IntValue(1)));
    }
    public SplitGroupManipulator(int n)
      : this() {
      NParameter.Value = new IntValue(n);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SplitGroupManipulator(this, cloner);
    }

    public static void Apply(IRandom random, LinearLinkage lle, int n) {
      var grouping = lle.GetGroups().ToList();
      var groupsLargerOne = grouping.Select((v, i) => Tuple.Create(i, v))
                                    .Where(x => x.Item2.Count > 1)
                                    .ToDictionary(x => x.Item1, x => x.Item2);
      if (groupsLargerOne.Count == 0) return;
      var toRemove = new List<int>();

      for (var i = 0; i < n; i++) {
        var g = groupsLargerOne.Keys.SampleRandom(random);
        var idx = random.Next(1, groupsLargerOne[g].Count);
        // shuffle here to avoid a potential bias of grouping smaller and larger numbers together
        var tmp = groupsLargerOne[g].Shuffle(random);
        var before = new List<int>();
        var after = new List<int>();
        foreach (var t in tmp) {
          if (idx > 0) before.Add(t);
          else after.Add(t);
          idx--;
        }
        if (before.Count > 1) groupsLargerOne[grouping.Count] = before;
        grouping.Add(before);
        if (after.Count > 1) groupsLargerOne[grouping.Count] = after;
        grouping.Add(after);
        toRemove.Add(g);
        groupsLargerOne.Remove(g);
        if (groupsLargerOne.Count == 0) break;
      }
      foreach (var r in toRemove.OrderByDescending(x => x))
        grouping.RemoveAt(r);

      lle.SetGroups(grouping);
    }

    protected override void Manipulate(IRandom random, LinearLinkage lle) {
      var N = NParameter.ActualValue.Value;
      Apply(random, lle, N);
    }
  }
}
