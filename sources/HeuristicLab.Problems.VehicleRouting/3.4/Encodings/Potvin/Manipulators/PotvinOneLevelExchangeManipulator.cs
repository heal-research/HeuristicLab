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
using HeuristicLab.Common;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinOneLevelExchangeMainpulator", "The 1M operator which manipulates a VRP representation.  It is implemented as described in Potvin, J.-Y. and Bengio, S. (1996). The Vehicle Routing Problem with Time Windows - Part II: Genetic Search. INFORMS Journal of Computing, 8:165–172.")]
  [StorableClass]
  public sealed class PotvinOneLevelExchangeMainpulator : PotvinManipulator {
    [StorableConstructor]
    private PotvinOneLevelExchangeMainpulator(bool deserializing) : base(deserializing) { }

    public PotvinOneLevelExchangeMainpulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinOneLevelExchangeMainpulator(this, cloner);
    }

    private PotvinOneLevelExchangeMainpulator(PotvinOneLevelExchangeMainpulator original, Cloner cloner)
      : base(original, cloner) {
    }
    
    protected override void Manipulate(IRandom random, PotvinEncoding individual) {
      bool allowInfeasible = AllowInfeasibleSolutions.Value.Value;
      
      int selectedIndex = SelectRandomTourBiasedByLength(random, individual);
      if (selectedIndex >= 0) {
        Tour route1 =
          individual.Tours[selectedIndex];

        int count = route1.Stops.Count;
        int i = 0;
        while (i < count) {
          int insertedRoute, insertedPlace;

          int city = route1.Stops[i];
          route1.Stops.Remove(city);

          if (FindInsertionPlace(individual, city, selectedIndex, allowInfeasible, out insertedRoute, out insertedPlace)) {
            individual.Tours[insertedRoute].Stops.Insert(insertedPlace, city);
          } else {
            route1.Stops.Insert(i, city);
            i++;
          }

          count = route1.Stops.Count;
        }
      }
    }
  }
}
