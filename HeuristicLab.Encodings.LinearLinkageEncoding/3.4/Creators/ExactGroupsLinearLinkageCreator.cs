#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Exactgroups Linear Linkage Creator", "Creates a random linear linkage LLE encoded solution with a given number of equal-sized groups.")]
  [StorableClass]
  public sealed class ExactGroupsLinearLinkageCreator : LinearLinkageCreator {

    public IValueLookupParameter<IntValue> ExactGroupsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["ExactGroups"]; }
    }

    public IntValue ExactGroups {
      get { return ExactGroupsParameter.Value; }
      set { ExactGroupsParameter.Value = value; }
    }

    [StorableConstructor]
    private ExactGroupsLinearLinkageCreator(bool deserializing) : base(deserializing) { }
    private ExactGroupsLinearLinkageCreator(ExactGroupsLinearLinkageCreator original, Cloner cloner) : base(original, cloner) { }

    public ExactGroupsLinearLinkageCreator() {
      Parameters.Add(new ValueLookupParameter<IntValue>("ExactGroups", "The exact number of equal-sized groups to create.", new IntValue(3)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExactGroupsLinearLinkageCreator(this, cloner);
    }

    public static LinearLinkage Apply(IRandom random, int length, int groups) {
      var groupNumbers = Enumerable.Range(0, length).Select(x => x % groups).Shuffle(random);
      var grouping = Enumerable.Range(0, groups).Select(x => new List<int>()).ToList();
      var idx = 0;
      foreach (var g in groupNumbers)
        grouping[g].Add(idx++);

      return LinearLinkage.FromGroups(length, grouping);
    }

    protected override LinearLinkage Create(IRandom random, int length) {
      var groups = ExactGroupsParameter.ActualValue.Value;
      return Apply(random, length, groups);
    }
  }
}
