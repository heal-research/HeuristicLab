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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaCustomerInversionManipulator", "An operator which manipulates a VRP representation by inverting the order the customers are visited.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaCustomerInversionManipulator : AlbaManipulator {
    [StorableConstructor]
    private AlbaCustomerInversionManipulator(bool deserializing) : base(deserializing) { }
    private AlbaCustomerInversionManipulator(AlbaCustomerInversionManipulator original, Cloner cloner)
      : base(original, cloner) {
    }
    public AlbaCustomerInversionManipulator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaCustomerInversionManipulator(this, cloner);
    }
    protected override void Manipulate(IRandom random, AlbaEncoding individual) {
      int breakPoint1, breakPoint2;

      int customer1 = random.Next(Cities);
      breakPoint1 = FindCustomerLocation(customer1, individual);

      int customer2 = random.Next(Cities);
      breakPoint2 = FindCustomerLocation(customer1, individual);

      List<int> visitingOrder = new List<int>();
      for (int i = breakPoint1; i <= breakPoint2; i++) {
        if (individual[i] != 0)
          visitingOrder.Add(individual[i]);
      }
      visitingOrder.Reverse();
      for (int i = breakPoint1; i <= breakPoint2; i++) {
        if (individual[i] != 0) {
          individual[i] = visitingOrder[0];
          visitingOrder.RemoveAt(0);
        }
      }
    }
  }
}
