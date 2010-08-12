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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("CustomerSwapManipualtor", "An operator which manipulates an Alba VRP representation by swapping two customers.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class CustomerSwapManipualtor : AlbaManipulator {
    [StorableConstructor]
    private CustomerSwapManipualtor(bool deserializing) : base(deserializing) { }

    public CustomerSwapManipualtor()
      : base() {
    }

    protected override void Manipulate(IRandom random, AlbaEncoding individual) {
      int index1, index2, temp;

      int customer1 = random.Next(Cities);
      index1 = FindCustomerLocation(customer1, individual);

      int customer2 = random.Next(Cities);
      index2 = FindCustomerLocation(customer2, individual);

      temp = individual[index1];
      individual[index1] = individual[index2];
      individual[index2] = temp;
    }
  }
}
