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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Maxgroups Linear Linkage Creator", "Creates a random linear linkage LLE encoded solution with a given maximum number of groups.")]
  [StorableClass]
  public sealed class MaxGroupsLinearLinkageCreator : LinearLinkageCreator {

    public IValueLookupParameter<IntValue> MaxGroupsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaxGroups"]; }
    }

    public IntValue MaxGroups {
      get { return MaxGroupsParameter.Value; }
      set { MaxGroupsParameter.Value = value; }
    }

    [StorableConstructor]
    private MaxGroupsLinearLinkageCreator(bool deserializing) : base(deserializing) { }
    private MaxGroupsLinearLinkageCreator(MaxGroupsLinearLinkageCreator original, Cloner cloner) : base(original, cloner) { }

    public MaxGroupsLinearLinkageCreator() {
      Parameters.Add(new ValueLookupParameter<IntValue>("MaxGroups", "The maximum number of groups to create.", new IntValue(3)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MaxGroupsLinearLinkageCreator(this, cloner);
    }

    public static LinearLinkage Apply(IRandom random, int length, int maxGroups) {
      var solution = new LinearLinkage(length);
      var groups = Enumerable.Range(0, length).Select(x => Tuple.Create(x, random.Next(maxGroups)))
                                              .GroupBy(x => x.Item2)
                                              .Select(x => x.Select(y => y.Item1).ToList());
      solution.SetGroups(groups);
      return solution;
    }

    protected override LinearLinkage Create(IRandom random, int length) {
      var maxGroups = MaxGroupsParameter.ActualValue.Value;
      return Apply(random, length, maxGroups);
    }
  }
}
