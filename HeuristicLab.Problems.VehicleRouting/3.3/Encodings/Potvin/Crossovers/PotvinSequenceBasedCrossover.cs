#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinSequenceBasedCrossover", "The SBX crossover for a VRP representations.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class PotvinSequenceBasedCrossover : PotvinCrossover {
    [StorableConstructor]
    private PotvinSequenceBasedCrossover(bool deserializing) : base(deserializing) { }
    private PotvinSequenceBasedCrossover(PotvinSequenceBasedCrossover original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinSequenceBasedCrossover(this, cloner);
    }

    public PotvinSequenceBasedCrossover()
      : base() { }

    protected override PotvinEncoding Crossover(IRandom random, PotvinEncoding parent1, PotvinEncoding parent2) {
      BoolValue useDistanceMatrix = UseDistanceMatrixParameter.ActualValue;
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      DistanceMatrix distMatrix = VRPUtilities.GetDistanceMatrix(coordinates, DistanceMatrixParameter, useDistanceMatrix);
      DoubleArray dueTime = DueTimeParameter.ActualValue;
      DoubleArray readyTime = ReadyTimeParameter.ActualValue;
      DoubleArray serviceTime = ServiceTimeParameter.ActualValue;
      DoubleArray demand = DemandParameter.ActualValue;
      DoubleValue capacity = CapacityParameter.ActualValue;

      bool allowInfeasible = AllowInfeasibleSolutions.Value.Value;

      PotvinEncoding child = parent1.Clone() as PotvinEncoding;
      Tour newTour = new Tour();

      int breakPoint1 = random.Next(1, Cities + 1);
      Tour tour1 = FindRoute(child, breakPoint1);
      breakPoint1 = tour1.Cities.IndexOf(breakPoint1);

      for (int i = 0; i < breakPoint1; i++)
        newTour.Cities.Add(tour1.Cities[i]);

      int breakPoint2 = random.Next(1, Cities + 1);
      Tour tour2 = FindRoute(parent2, breakPoint2);
      breakPoint2 = tour2.Cities.IndexOf(breakPoint2);

      for (int i = breakPoint2; i < tour2.Cities.Count; i++)
        newTour.Cities.Add(tour2.Cities[i]);

      child.Tours.Remove(tour1);
      child.Tours.Add(newTour);

      foreach (int city in tour1.Cities)
        if (FindRoute(child, city) == null && !child.Unrouted.Contains(city))
          child.Unrouted.Add(city);

      foreach (int city in tour2.Cities)
        if (FindRoute(child, city) == null && !child.Unrouted.Contains(city))
          child.Unrouted.Add(city);

      if (Repair(random, child, newTour, distMatrix, dueTime, readyTime, serviceTime, demand, capacity, allowInfeasible) || allowInfeasible) {
        return child;
      } else {
         if (random.NextDouble() < 0.5)
          return parent1.Clone() as PotvinEncoding;
        else
          return parent2.Clone() as PotvinEncoding;   
      }
    }
  }
}
