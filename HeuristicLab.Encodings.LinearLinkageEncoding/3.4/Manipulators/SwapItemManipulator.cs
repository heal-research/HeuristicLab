#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Swap Item Manipulator", "Performs N swaps operations of two items each. The same items may be swapped multiple times, at least two groups need to be present.")]
  [StorableType("21F18233-0752-40A2-8B9A-6E6A9E2CE456")]
  public sealed class SwapItemManipulator : LinearLinkageManipulator {

    public IValueLookupParameter<IntValue> NParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["N"]; }
    }

    [StorableConstructor]
    private SwapItemManipulator(StorableConstructorFlag _) : base(_) { }
    private SwapItemManipulator(SwapItemManipulator original, Cloner cloner) : base(original, cloner) { }
    public SwapItemManipulator() {
      Parameters.Add(new ValueLookupParameter<IntValue>("N", "The number of swaps to perform.", new IntValue(1)));
    }
    public SwapItemManipulator(int n)
      : this() {
      NParameter.Value = new IntValue(n);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SwapItemManipulator(this, cloner);
    }

    public static void Apply(IRandom random, LinearLinkage lle, int n) {
      var grouping = lle.GetGroups().Select(x => x.ToList()).ToList();
      if (grouping.Count == 1) return; // nothing can be changed

      var prevGroup = random.Next(grouping.Count);
      var prevItem = random.Next(grouping[prevGroup].Count);
      for (var i = 0; i < n; i++) {
        int nextGroup, nextItem;
        do {
          nextGroup = random.Next(grouping.Count);
          nextItem = random.Next(grouping[nextGroup].Count);
        } while (nextGroup == prevGroup);
        var h = grouping[nextGroup][nextItem];
        grouping[nextGroup][nextItem] = grouping[prevGroup][prevItem];
        grouping[prevGroup][prevItem] = h;
        prevGroup = nextGroup;
        prevItem = nextItem;
      }

      lle.SetGroups(grouping);
    }

    protected override void Manipulate(IRandom random, LinearLinkage lle) {
      var N = NParameter.ActualValue.Value;
      Apply(random, lle, N);
    }
  }
}
