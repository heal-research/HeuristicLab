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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVRSwapManipulator", "An operator which manipulates a GVR representation by swapping two customers. It is implemented as described in Pereira, F.B. et al (2002). GVR: a New Genetic Representation for the Vehicle Routing Problem. AICS 2002, LNAI 2464, pp. 95-102.")]
  [StorableType("3EEC2566-633D-4338-853D-AE9930E9F238")]
  public sealed class GVRSwapManipulator : GVRManipulator {
    [StorableConstructor]
    private GVRSwapManipulator(StorableConstructorFlag _) : base(_) { }

    public GVRSwapManipulator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GVRSwapManipulator(this, cloner);
    }

    private GVRSwapManipulator(GVRSwapManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void Manipulate(IRandom random, GVREncoding individual) {
      int customer1 = random.Next(1, individual.Cities + 1);
      int customer2 = random.Next(1, individual.Cities + 1);

      Tour tour1;
      int pos1;
      individual.FindCustomer(customer1, out tour1, out pos1);

      Tour tour2;
      int pos2;
      individual.FindCustomer(customer2, out tour2, out pos2);

      int temp = tour1.Stops[pos1];
      tour1.Stops[pos1] = tour2.Stops[pos2];
      tour2.Stops[pos2] = temp;
    }
  }
}
