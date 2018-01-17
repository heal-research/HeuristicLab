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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Zhu {
  [Item("ZhuMergeCrossover2", "The Zhu Merge Crossover (Version 2).  It is implemented as described in Zhu, K.Q. (2000). A New Genetic Algorithm For VRPTW. Proceedings of the International Conference on Artificial Intelligence.")]
  [StorableClass]
  public sealed class ZhuMergeCrossover2 : ZhuCrossover {
    [StorableConstructor]
    private ZhuMergeCrossover2(bool deserializing) : base(deserializing) { }

    public ZhuMergeCrossover2()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZhuMergeCrossover2(this, cloner);
    }

    private ZhuMergeCrossover2(ZhuMergeCrossover2 original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override ZhuEncoding Crossover(IRandom random, ZhuEncoding parent1, ZhuEncoding parent2) {
      List<int> p1 = new List<int>(parent1);
      List<int> p2 = new List<int>(parent2);

      ZhuEncoding child = parent2.Clone() as ZhuEncoding;

      if (parent1.Length != parent2.Length)
        return child;

      int breakPoint = random.Next(child.Length);
      int i = breakPoint;

      int parent1Index = i;
      int parent2Index = i;

      DoubleArray dueTime = null;
      if (ProblemInstance is ITimeWindowedProblemInstance) {
        dueTime = (ProblemInstance as ITimeWindowedProblemInstance).DueTime;
      }

      do {
        if (i == breakPoint) {
          child[i] = p1[parent1Index];
        } else {
          if (dueTime != null &&
            (dueTime[p1[parent1Index] + 1] <
            dueTime[p2[parent2Index] + 1])) {
            child[i] = p1[parent1Index];
          } else {
            child[i] = p2[parent2Index];
          }
        }

        p1.Remove(child[i]);
        if (parent1Index >= p1.Count)
          parent1Index = 0;

        p2.Remove(child[i]);
        if (parent2Index >= p2.Count)
          parent2Index = 0;

        i = (i + 1) % child.Length;
      } while (i != breakPoint);

      return child;
    }
  }
}
