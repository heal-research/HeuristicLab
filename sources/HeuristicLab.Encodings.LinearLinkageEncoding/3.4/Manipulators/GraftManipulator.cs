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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Graft Manipulator", "Performs graft mutation as described in Du, J., Korkmaz, E.E., Alhajj, R., and Barker, K. 2004. Novel Clustering Approach that employs Genetic Algorithm with New Representation Scheme and Multiple Objectives. Data Warehousing and Knowledge Discovery, pp. 219-228. Springer Berlin Heidelberg.")]
  [StorableClass]
  public sealed class GraftManipulator : LinearLinkageManipulator {

    [StorableConstructor]
    private GraftManipulator(bool deserializing) : base(deserializing) { }
    private GraftManipulator(GraftManipulator original, Cloner cloner) : base(original, cloner) { }
    public GraftManipulator() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GraftManipulator(this, cloner);
    }

    public static void Apply(IRandom random, LinearLinkage lle) {
      int tries = lle.Length;
      var index = random.Next(lle.Length);
      while (tries > 0 && lle[index] == index) {
        index = random.Next(lle.Length);
        tries--;
      }
      if (lle[index] != index) Apply(random, lle, index);
    }

    public static void Apply(IRandom random, LinearLinkage lle, int index) {
      var groups = lle.Select((val, idx) => Tuple.Create(idx, val))
                      .Where(x => x.Item1 == x.Item2)
                      .Select(x => x.Item2).ToList();
      var z = groups.Count;

      if (random.NextDouble() < 0.5)
        lle[index] = index; // divide the cluster into two
      else {
        var c = random.Next(z);
        if (groups[c] > index)
          lle[index] = groups[c]; // combine the portion with another class
        else {
          // combine the other class here
          lle[groups[c]] = lle[index];
          lle[index] = index;
        }
        lle.LinearizeTreeStructures();
      }
    }

    protected override void Manipulate(IRandom random, LinearLinkage lle) {
      Apply(random, lle);
    }
  }
}
