#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaCustomerSwapManipulator", "An operator which manipulates a VRP representation by swapping two customers.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableType("9040B9C9-B7BB-4EC9-8049-4A488F8A6976")]
  public sealed class AlbaCustomerSwapManipulator : AlbaManipulator {
    [StorableConstructor]
    private AlbaCustomerSwapManipulator(StorableConstructorFlag _) : base(_) { }

    public AlbaCustomerSwapManipulator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaCustomerSwapManipulator(this, cloner);
    }

    private AlbaCustomerSwapManipulator(AlbaCustomerSwapManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void Manipulate(IRandom random, AlbaEncoding individual) {
      int index1, index2, temp;

      int customer1 = random.Next(ProblemInstance.Cities.Value);
      index1 = FindCustomerLocation(customer1, individual);

      int customer2 = random.Next(ProblemInstance.Cities.Value);
      index2 = FindCustomerLocation(customer2, individual);

      temp = individual[index1];
      individual[index1] = individual[index2];
      individual[index2] = temp;
    }
  }
}
