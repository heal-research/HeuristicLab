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
  [Item("OneLevelExchangeMainpulator", "The 1M operator which manipulates a VRP representation.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class OneLevelExchangeMainpulator : PotvinManipulator {
    [StorableConstructor]
    private OneLevelExchangeMainpulator(bool deserializing) : base(deserializing) { }

    public OneLevelExchangeMainpulator() : base() { }
    
    protected override void Manipulate(IRandom random, PotvinEncoding individual) {
      int selectedIndex = SelectRandomTourBiasedByLength(random, individual);
      Tour route1 =
        individual.Tours[selectedIndex];

      List<int> replaced = new List<int>();
      for (int i = 0; i < route1.Cities.Count; i++) {
        int insertedRoute, insertedPlace;

        if (FindInsertionPlace(individual, route1.Cities[i], selectedIndex, out insertedRoute, out insertedPlace)) {
          individual.Tours[insertedRoute].Cities.Insert(insertedPlace, route1.Cities[i]);
          replaced.Add(route1.Cities[i]);
        }
      }

      route1.Cities.RemoveAll(
        new System.Predicate<int>( 
          delegate(int val) {
            return (replaced.Contains(val)); 
          }
        )
      );

      if (route1.Cities.Count == 0)
        individual.Tours.Remove(route1);
    }
  }
}
