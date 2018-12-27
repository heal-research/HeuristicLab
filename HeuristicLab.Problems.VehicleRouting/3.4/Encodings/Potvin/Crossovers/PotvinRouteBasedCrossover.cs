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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinRouteBasedCrossover", "The RBX crossover for a VRP representations.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class PotvinRouteBasedCrossover : PotvinCrossover {
    [StorableConstructor]
    private PotvinRouteBasedCrossover(bool deserializing) : base(deserializing) { }

    public PotvinRouteBasedCrossover()
      : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinRouteBasedCrossover(this, cloner);
    }

    private PotvinRouteBasedCrossover(PotvinRouteBasedCrossover original, Cloner cloner)
      : base(original, cloner) {
    }

    public static PotvinEncoding Apply(IRandom random, PotvinEncoding parent1, PotvinEncoding parent2, IVRPProblemInstance problemInstance, bool allowInfeasible) {
      PotvinEncoding child = parent2.Clone() as PotvinEncoding;

      if (parent1.Tours.Count > 0 && child.Tours.Count > 0) {
        int tourParent1 = random.Next(parent1.Tours.Count);
        Tour replacing = parent1.Tours[tourParent1].Clone() as Tour;

        int tourParent2 = random.Next(child.Tours.Count);
        Tour replaced = child.Tours[tourParent2];

        child.Tours.Remove(replaced);
        child.Tours.Insert(tourParent2, replacing);

        Permutation vehicleAssignment = child.VehicleAssignment;

        int vehicle = vehicleAssignment[tourParent2];
        int vehicle2 = parent1.VehicleAssignment[tourParent1];
        vehicleAssignment[tourParent2] = vehicle2;

        for (int i = 0; i < vehicleAssignment.Length; i++) {
          if (vehicleAssignment[i] == vehicle2 && i != tourParent2) {
            vehicleAssignment[i] = vehicle;
            break;
          }
        }

        foreach (int city in replaced.Stops)
          if (FindRoute(child, city) == null && !child.Unrouted.Contains(city))
            child.Unrouted.Add(city);

        if (Repair(random, child, replacing, problemInstance, allowInfeasible) || allowInfeasible)
          return child;
        else {
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
