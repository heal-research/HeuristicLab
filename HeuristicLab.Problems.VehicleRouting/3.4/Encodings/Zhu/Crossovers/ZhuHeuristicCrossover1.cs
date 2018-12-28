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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Zhu {
  [Item("ZhuHeuristicCrossover1", "The Zhu Heuristic Crossover (Version 1). It is implemented as described in Zhu, K.Q. (2000). A New Genetic Algorithm For VRPTW. Proceedings of the International Conference on Artificial Intelligence.")]
  [StorableType("C3E48066-FD84-428E-89E7-DD0A8322535E")]
  public sealed class ZhuHeuristicCrossover1 : ZhuCrossover {
    [StorableConstructor]
    private ZhuHeuristicCrossover1(StorableConstructorFlag _) : base(_) { }

    public ZhuHeuristicCrossover1()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZhuHeuristicCrossover1(this, cloner);
    }

    private ZhuHeuristicCrossover1(ZhuHeuristicCrossover1 original, Cloner cloner)
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
      int predecessor = breakPoint - 1;
      if (predecessor < 0)
        predecessor = predecessor + child.Length;

      while (i != predecessor) {
        if (i == breakPoint) {
          child[i] = parent1[i];
          Swap(parent2, parent2[i], parent1[i]);
        }

        if (ProblemInstance.GetDistance(
          child[i] + 1, parent1[(i + 1) % child.Length] + 1, child)
          <
          ProblemInstance.GetDistance(
          child[i] + 1, parent2[(i + 1) % child.Length] + 1, child)) {
          child[(i + 1) % child.Length] = parent1[(i + 1) % child.Length];
          Swap(parent2, parent2[(i + 1) % child.Length], parent1[(i + 1) % child.Length]);
        } else {
          child[(i + 1) % child.Length] = parent2[(i + 1) % child.Length];
          Swap(parent1, parent1[(i + 1) % child.Length], parent2[(i + 1) % child.Length]);
        }

        i = (i + 1) % child.Length;
      }

      return child;
    }
  }
}
