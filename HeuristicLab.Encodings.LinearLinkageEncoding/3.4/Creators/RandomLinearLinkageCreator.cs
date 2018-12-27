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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Random Linear Linkage Creator", "Creates a random linear linkage LLE encoded solution (similar to MaxGroups set to N).")]
  [StorableClass]
  public sealed class RandomLinearLinkageCreator : LinearLinkageCreator {

    [StorableConstructor]
    private RandomLinearLinkageCreator(bool deserializing) : base(deserializing) { }
    private RandomLinearLinkageCreator(RandomLinearLinkageCreator original, Cloner cloner) : base(original, cloner) { }
    public RandomLinearLinkageCreator() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomLinearLinkageCreator(this, cloner);
    }

    public static LinearLinkage Apply(IRandom random, int length) {
      var groups = Enumerable.Range(0, length).Select(x => Tuple.Create(x, random.Next(length)))
                                              .GroupBy(x => x.Item2)
                                              .Select(x => x.Select(y => y.Item1).ToList());
      return LinearLinkage.FromGroups(length, groups);
    }

    protected override LinearLinkage Create(IRandom random, int length) {
      return Apply(random, length);
    }
  }
}
