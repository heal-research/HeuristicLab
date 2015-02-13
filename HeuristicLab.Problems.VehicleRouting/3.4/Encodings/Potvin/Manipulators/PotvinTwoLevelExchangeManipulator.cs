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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinTwoLevelExchangeManipulator", "The 2M operator which manipulates a VRP representation.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class PotvinTwoLevelExchangeManipulator : PotvinManipulator {
    [StorableConstructor]
    private PotvinTwoLevelExchangeManipulator(bool deserializing) : base(deserializing) { }

    public PotvinTwoLevelExchangeManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinTwoLevelExchangeManipulator(this, cloner);
    }

    private PotvinTwoLevelExchangeManipulator(PotvinTwoLevelExchangeManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void ApplyManipulation(IRandom random, PotvinEncoding individual, IVRPProblemInstance instance, bool allowInfeasible) {
      int selectedIndex = SelectRandomTourBiasedByLength(random, individual, instance);
      if (selectedIndex >= 0) {
        Tour route1 = individual.Tours[selectedIndex];

        bool performed = false;
        int customer1Position = 0;
        while (customer1Position < route1.Stops.Count) {
          performed = false;

          foreach (Tour tour in individual.Tours) {
            if (tour != route1) {
              for (int customer2Position = 0; customer2Position < tour.Stops.Count; customer2Position++) {
                int customer1 = route1.Stops[customer1Position];
                int customer2 = tour.Stops[customer2Position];

                //customer1 can be feasibly inserted at the location of customer2
                tour.Stops[customer2Position] = customer1;
                route1.Stops.RemoveAt(customer1Position);

                if (instance.TourFeasible(tour, individual)) {
                  int routeIdx, place;
                  if (FindInsertionPlace(individual,
                    customer2, selectedIndex, allowInfeasible, out routeIdx, out place)) {
                    individual.Tours[routeIdx].Stops.Insert(place, customer2);

                    //two-level exchange has been performed
                    performed = true;
                    break;
                  } else {
                    tour.Stops[customer2Position] = customer2;
                    route1.Stops.Insert(customer1Position, customer1);
                  }
                } else {
                  tour.Stops[customer2Position] = customer2;
                  route1.Stops.Insert(customer1Position, customer1);
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
    }
      

    protected override void Manipulate(IRandom random, PotvinEncoding individual) {
      bool allowInfeasible = AllowInfeasibleSolutions.Value.Value;

      ApplyManipulation(random, individual, ProblemInstance, allowInfeasible);
    }
  }
}
