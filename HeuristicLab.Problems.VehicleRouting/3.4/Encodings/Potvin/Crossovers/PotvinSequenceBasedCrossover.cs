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
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinSequenceBasedCrossover", "The SBX crossover for a VRP representations.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableType("EFC16985-0308-462C-8D6C-0F1177F53CD2")]
  public sealed class PotvinSequenceBasedCrossover : PotvinCrossover {
    [StorableConstructor]
    private PotvinSequenceBasedCrossover(StorableConstructorFlag _) : base(_) { }

    public PotvinSequenceBasedCrossover()
      : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinSequenceBasedCrossover(this, cloner);
    }

    private PotvinSequenceBasedCrossover(PotvinSequenceBasedCrossover original, Cloner cloner)
      : base(original, cloner) {
    }

    public static PotvinEncoding Apply(IRandom random, PotvinEncoding parent1, PotvinEncoding parent2, IVRPProblemInstance problemInstance, bool allowInfeasible) {
      PotvinEncoding child = parent1.Clone() as PotvinEncoding;
      Tour newTour = new Tour();

      int cities = problemInstance.Cities.Value;

      if (cities > 0) {
        int breakPoint1 = random.Next(1, cities + 1);
        Tour tour1 = FindRoute(child, breakPoint1);
        breakPoint1 = tour1.Stops.IndexOf(breakPoint1);

        for (int i = 0; i < breakPoint1; i++)
          newTour.Stops.Add(tour1.Stops[i]);

        int breakPoint2 = random.Next(1, cities + 1);
        Tour tour2 = FindRoute(parent2, breakPoint2);
        breakPoint2 = tour2.Stops.IndexOf(breakPoint2);

        for (int i = breakPoint2; i < tour2.Stops.Count; i++)
          newTour.Stops.Add(tour2.Stops[i]);

        int tour1Index = child.Tours.IndexOf(tour1);
        child.Tours.Remove(tour1);
        child.Tours.Insert(tour1Index, newTour);

        foreach (int city in tour1.Stops)
          if (FindRoute(child, city) == null && !child.Unrouted.Contains(city))
            child.Unrouted.Add(city);

        foreach (int city in tour2.Stops)
          if (FindRoute(child, city) == null && !child.Unrouted.Contains(city))
            child.Unrouted.Add(city);

        if (Repair(random, child, newTour, problemInstance, allowInfeasible) || allowInfeasible) {
          return child;
        } else {
          if (random.NextDouble() < 0.5)
            return parent1.Clone() as PotvinEncoding;
          else
            return parent2.Clone() as PotvinEncoding;
        }
      } else {
        return child;
      }
    }

    protected override PotvinEncoding Crossover(IRandom random, PotvinEncoding parent1, PotvinEncoding parent2) {
      return Apply(random, parent1, parent2, ProblemInstance, AllowInfeasibleSolutions.Value.Value);
    }
  }
}
