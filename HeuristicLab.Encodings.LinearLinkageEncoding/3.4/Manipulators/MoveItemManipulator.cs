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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Move Item Manipulator", "Performs a maximum of N move operations between groups or to new groups. An already moved item may be moved again.")]
  [StorableType("26B371F4-8E37-406A-AC5A-3E7734087890")]
  public sealed class MoveItemManipulator : LinearLinkageManipulator {

    public IValueLookupParameter<IntValue> NParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["N"]; }
    }

    [StorableConstructor]
    private MoveItemManipulator(StorableConstructorFlag _) : base(_) { }
    private MoveItemManipulator(MoveItemManipulator original, Cloner cloner) : base(original, cloner) { }
    public MoveItemManipulator() {
      Parameters.Add(new ValueLookupParameter<IntValue>("N", "The number of items to move.", new IntValue(1)));
    }
    public MoveItemManipulator(int n)
      : this() {
      NParameter.Value = new IntValue(n);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MoveItemManipulator(this, cloner);
    }

    public static void Apply(IRandom random, LinearLinkage lle, int n) {
      var grouping = lle.GetGroups().ToList();
      grouping.Add(new List<int>()); // add an empty group

      for (var i = 0; i < n; i++) {
        var src = random.Next(grouping.Count - 1); // only select from non-empty groups
        var dest = random.Next(grouping.Count);
        while (src == dest || grouping[src].Count == 1 && dest == grouping.Count - 1) {
          src = random.Next(grouping.Count - 1);
          dest = random.Next(grouping.Count);
        }
        if (dest == grouping.Count - 1) grouping.Add(new List<int>());

        var e = random.Next(grouping[src].Count);
        grouping[dest].Add(grouping[src][e]);
        grouping[src].RemoveAt(e);
        if (grouping[src].Count == 0)
          grouping.RemoveAt(src);
      }

      lle.SetGroups(grouping.Where(x => x.Count > 0));
    }

    protected override void Manipulate(IRandom random, LinearLinkage lle) {
      var N = NParameter.ActualValue.Value;
      Apply(random, lle, N);
    }
  }
}
