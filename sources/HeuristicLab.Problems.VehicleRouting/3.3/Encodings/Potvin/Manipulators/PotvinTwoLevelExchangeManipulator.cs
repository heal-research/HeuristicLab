#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("PotvinTwoLevelExchangeManipulator", "The 2M operator which manipulates a VRP representation.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class PotvinTwoLevelExchangeManipulator : PotvinManipulator {
    [StorableConstructor]
    private PotvinTwoLevelExchangeManipulator(bool deserializing) : base(deserializing) { }
    private PotvinTwoLevelExchangeManipulator(PotvinTwoLevelExchangeManipulator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinTwoLevelExchangeManipulator(this, cloner);
    }
    public PotvinTwoLevelExchangeManipulator() : base() { }

    public static void Apply(IRandom random, PotvinEncoding individual, 
      DoubleArray dueTime, DoubleArray readyTime, DoubleArray serviceTime, DoubleArray demand,
      DoubleValue capacity, DistanceMatrix distMatrix, bool allowInfeasible) {
      int selectedIndex = SelectRandomTourBiasedByLength(random, individual);
      Tour route1 = individual.Tours[selectedIndex];

      bool performed = false;
      int customer1Position = 0;
      while (customer1Position < route1.Cities.Count) {
        performed = false;

        foreach (Tour tour in individual.Tours) {
          if (tour != route1) {
            for (int customer2Position = 0; customer2Position < tour.Cities.Count; customer2Position++) {
              int customer1 = route1.Cities[customer1Position];
              int customer2 = tour.Cities[customer2Position];

              //customer1 can be feasibly inserted at the location of customer2
              tour.Cities[customer2Position] = customer1;
              if (tour.Feasible(dueTime, serviceTime, readyTime, demand, capacity, distMatrix)) {
                int routeIdx, place;
                if (FindInsertionPlace(individual,
                  customer2, selectedIndex,
                  dueTime, serviceTime, readyTime, demand, capacity,
                  distMatrix, allowInfeasible,
                  out routeIdx, out place)) {
                  individual.Tours[routeIdx].Cities.Insert(place, customer2);
                  route1.Cities.RemoveAt(customer1Position);

                  if (route1.Cities.Count == 0)
                    individual.Tours.Remove(route1);

                  //two-level exchange has been performed
                  performed = true;
                  break;
                } else {
                  tour.Cities[customer2Position] = customer2;
                }
              } else {
                tour.Cities[customer2Position] = customer2;
              }
            }
          }

          if (performed)
            break;
        }

        if (!performed)
          customer1Position++;
      }
    }

    protected override void Manipulate(IRandom random, PotvinEncoding individual) {
      BoolValue useDistanceMatrix = UseDistanceMatrixParameter.ActualValue;
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      DistanceMatrix distMatrix = VRPUtilities.GetDistanceMatrix(coordinates, DistanceMatrixParameter, useDistanceMatrix);
      DoubleArray dueTime = DueTimeParameter.ActualValue;
      DoubleArray readyTime = ReadyTimeParameter.ActualValue;
      DoubleArray serviceTime = ServiceTimeParameter.ActualValue;
      DoubleArray demand = DemandParameter.ActualValue;
      DoubleValue capacity = CapacityParameter.ActualValue;

      bool allowInfeasible = AllowInfeasibleSolutions.Value.Value;

      Apply(random, individual, dueTime, readyTime, serviceTime, demand, capacity, distMatrix, allowInfeasible);
    }
  }
}
