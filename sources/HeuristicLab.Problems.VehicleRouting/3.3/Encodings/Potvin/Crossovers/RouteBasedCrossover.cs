#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("RouteBasedCrossover", "The RBX crossover for the Potvin VRP representations.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class RouteBasedCrossover : PotvinCrossover {
    [StorableConstructor]
    private RouteBasedCrossover(bool deserializing) : base(deserializing) { }

    public RouteBasedCrossover()
      : base() { }
     
    protected override PotvinEncoding Crossover(IRandom random, PotvinEncoding parent1, PotvinEncoding parent2) {
      PotvinEncoding child = parent2.Clone() as PotvinEncoding;

      int tourParent1 = random.Next(parent1.Tours.Count);
      Tour replacing = parent1.Tours[tourParent1].Clone() as Tour;

      int tourParent2 = random.Next(child.Tours.Count);
      Tour replaced = child.Tours[tourParent2];

      child.Tours.Remove(replaced);
      child.Tours.Add(replacing);

      foreach (int city in replaced.Cities)
        if (FindRoute(child, city) == null && !child.Unrouted.Contains(city))
          child.Unrouted.Add(city);

      if (Repair(random, child, replacing))
        return child;
      else {
        if(random.NextDouble() < 0.5)
          return parent1.Clone() as PotvinEncoding;
        else
          return parent2.Clone() as PotvinEncoding;
      }
    }
  }
}
