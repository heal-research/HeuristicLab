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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Zhu {
  [Item("ZhuMergeCrossover1", "The Zhu Merge Crossover (Version 1). It is implemented as described in Zhu, K.Q. (2000). A New Genetic Algorithm For VRPTW. Proceedings of the International Conference on Artificial Intelligence.")]
  [StorableClass]
  public sealed class ZhuMergeCrossover1 : ZhuCrossover {
    [StorableConstructor]
    private ZhuMergeCrossover1(bool deserializing) : base(deserializing) { }

    public ZhuMergeCrossover1()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZhuMergeCrossover1(this, cloner);
    }

    private ZhuMergeCrossover1(ZhuMergeCrossover1 original, Cloner cloner)
      : base(original, cloner) {
    }

    private void Swap(ZhuEncoding individual, int city1, int city2) {
      int index1 = individual.IndexOf(city1);
      int index2 = individual.IndexOf(city2);

      int temp = individual[index1];
      individual[index1] = individual[index2];
      individual[index2] = temp;
    }

    protected override ZhuEncoding Crossover(IRandom random, ZhuEncoding parent1, ZhuEncoding parent2) {
      parent1 = parent1.Clone() as ZhuEncoding;
      parent2 = parent2.Clone() as ZhuEncoding;

      ZhuEncoding child = parent2.Clone() as ZhuEncoding;

      if (parent1.Length != parent2.Length)
        return child;

      int breakPoint = random.Next(child.Length);
      int i = breakPoint;

      DoubleArray dueTime = null;
      if (ProblemInstance is ITimeWindowedProblemInstance) {
        dueTime = (ProblemInstance as ITimeWindowedProblemInstance).DueTime;
      }

      do {
        if (i == breakPoint) {
          child[i] = parent1[i];
          Swap(parent2, parent2[i], parent1[i]);
        } else {
          if (
            (dueTime != null) &&
            (dueTime[parent1[i] + 1] <
             dueTime[parent2[i] + 1])) {
            child[i] = parent1[i];
            Swap(parent2, parent2[i], parent1[i]);
          } else {
            child[i] = parent2[i];
            Swap(parent1, parent1[i], parent2[i]);
          }
        }

        i = (i + 1) % child.Length;
      } while (i != breakPoint);

      return child;
    }
  }
}
