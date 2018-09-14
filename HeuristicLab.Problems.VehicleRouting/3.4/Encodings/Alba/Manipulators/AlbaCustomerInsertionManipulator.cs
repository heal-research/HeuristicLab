#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaCustomerInsertionManipulator", "An operator which manipulates a VRP representation by inserting a customer in another place.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaCustomerInsertionManipulator : AlbaManipulator {
    [StorableConstructor]
    private AlbaCustomerInsertionManipulator(bool deserializing) : base(deserializing) { }

    public AlbaCustomerInsertionManipulator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaCustomerInsertionManipulator(this, cloner);
    }

    private AlbaCustomerInsertionManipulator(AlbaCustomerInsertionManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void Manipulate(IRandom random, AlbaEncoding individual) {
      AlbaEncoding original = (AlbaEncoding)individual.Clone();
      int cutIndex, insertIndex, number;

      int customer = random.Next(ProblemInstance.Cities.Value);
      cutIndex = FindCustomerLocation(customer, individual);

      insertIndex = random.Next(original.Length);
      number = original[cutIndex];

      int i = 0;  // index in new permutation
      int j = 0;  // index in old permutation
      while (i < original.Length) {
        if (j == cutIndex) {
          j++;
        }
        if (i == insertIndex) {
          individual[i] = number;
          i++;
        }
        if ((i < original.Length) && (j < original.Length)) {
          individual[i] = original[j];
          i++;
          j++;
        }
      }
    }
  }
}
