#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDShiftExhaustiveMoveGenerator", "Generates shift moves from a given PDP encoding.")]
  [StorableType("CD03F921-FC4D-49AD-BEA3-156C60AAD5BD")]
  public sealed class PotvinPDShiftExhaustiveMoveGenerator : PotvinPDShiftMoveGenerator, IExhaustiveMoveGenerator {
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDShiftExhaustiveMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinPDShiftExhaustiveMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public PotvinPDShiftExhaustiveMoveGenerator()
      : base() {
    }

    private PotvinPDShiftExhaustiveMoveGenerator(PotvinPDShiftExhaustiveMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override PotvinPDShiftMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinPDShiftMove> result = new List<PotvinPDShiftMove>();
      IPickupAndDeliveryProblemInstance pdp = problemInstance as IPickupAndDeliveryProblemInstance;

      int max = individual.Tours.Count;
      if (individual.Tours.Count >= problemInstance.Vehicles.Value)
        max = max - 1;

      for (int i = 0; i < individual.Tours.Count; i++) {
        for (int j = 0; j < individual.Tours[i].Stops.Count; j++) {
          for (int k = 0; k <= max; k++) {
            if (k != i) {
              int city = individual.Tours[i].Stops[j];
              if (pdp == null || pdp.GetDemand(city) >= 0) {
                PotvinPDShiftMove move = new PotvinPDShiftMove(
                  city, i, k, individual);

                result.Add(move);
              }
            }
          }
        }
      }

      return result.ToArray();
    }
  }
}
