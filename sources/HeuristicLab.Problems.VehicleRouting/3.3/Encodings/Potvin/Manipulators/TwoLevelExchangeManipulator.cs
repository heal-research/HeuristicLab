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
using System.Collections.Generic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("TwoLevelExchangeOperator", "The 2M operator which manipulates a Potvin VRP representation.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class TwoLevelExchangeOperator : PotvinManipulator {
    [StorableConstructor]
    private TwoLevelExchangeOperator(bool deserializing) : base(deserializing) { }

    public TwoLevelExchangeOperator(): base() { }
    
    protected override void Manipulate(IRandom random, PotvinEncoding individual) {
      int selectedIndex = SelectRandomTourBiasedByLength(random, individual);
      Tour route1 =
        individual.Tours[selectedIndex];

      for (int customer1Position = 0; customer1Position < route1.Cities.Count; customer1Position++) {
        foreach (Tour tour in individual.Tours) {
          if (tour != route1) {
            for (int customer2Position = 0; customer2Position < tour.Cities.Count; customer2Position++) {
              int customer1 = route1.Cities[customer1Position];
              int customer2 = tour.Cities[customer2Position];

              //customer1 can be feasibly inserted at the location of customer2
              tour.Cities[customer2Position] = customer1;
              if (Feasible(tour)) {
                int route, place;
                if (FindInsertionPlace(individual,
                  customer2, selectedIndex, out route, out place)) {
                    individual.Tours[route].Cities.Insert(place, customer2);
                  route1.Cities.RemoveAt(customer1Position);

                  if (route1.Cities.Count == 0)
                    individual.Tours.Remove(route1);

                  //two-level exchange has been performed
                  return;
                } else {
                  tour.Cities[customer2Position] = customer2;
                }
              } else {
                tour.Cities[customer2Position] = customer2;
              }
            }
          }
        }
      }
    }
  }
}
