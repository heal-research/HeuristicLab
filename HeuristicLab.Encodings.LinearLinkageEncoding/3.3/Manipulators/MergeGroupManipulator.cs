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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Merge Group Manipulator", "Performs a maximum of N merge operations on the groups. An already merged group may be merged again.")]
  [StorableClass]
  public sealed class MergeGroupManipulator : LinearLinkageManipulator {

    public IValueLookupParameter<IntValue> NParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["N"]; }
    }

    [StorableConstructor]
    private MergeGroupManipulator(bool deserializing) : base(deserializing) { }
    private MergeGroupManipulator(MergeGroupManipulator original, Cloner cloner) : base(original, cloner) { }
    public MergeGroupManipulator() {
      Parameters.Add(new ValueLookupParameter<IntValue>("N", "The number of groups to merge.", new IntValue(1)));
    }
    public MergeGroupManipulator(int n)
      : this() {
      NParameter.Value = new IntValue(n);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MergeGroupManipulator(this, cloner);
    }

    public static void Apply(IRandom random, LinearLinkage lle, int n) {
      var grouping = lle.GetGroups().ToList();
      if (grouping.Count == 1) return; // nothing to merge

      for (var i = 0; i < n; i++) {
        var g1 = random.Next(grouping.Count);
        var g2 = random.Next(grouping.Count);
        while (g1 == g2) g2 = random.Next(grouping.Count);
        grouping[g1].AddRange(grouping[g2]);
        grouping.RemoveAt(g2);
        if (grouping.Count == 1) break;
      }

      lle.SetGroups(grouping);
    }

    protected override void Manipulate(IRandom random, LinearLinkage lle) {
      var N = NParameter.ActualValue.Value;
      Apply(random, lle, N);
    }
  }
}
