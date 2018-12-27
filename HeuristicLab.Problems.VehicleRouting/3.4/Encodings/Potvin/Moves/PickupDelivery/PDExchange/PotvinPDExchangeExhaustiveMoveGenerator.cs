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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDExchangeExhaustiveMoveGenerator", "Generates exchange moves from a given PDP encoding.")]
  [StorableClass]
  public sealed class PotvinPDExchangeExhaustiveMoveGenerator : PotvinPDExchangeMoveGenerator, IExhaustiveMoveGenerator {
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDExchangeExhaustiveMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinPDExchangeExhaustiveMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinPDExchangeExhaustiveMoveGenerator()
      : base() {
    }

    private PotvinPDExchangeExhaustiveMoveGenerator(PotvinPDExchangeExhaustiveMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override PotvinPDExchangeMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinPDExchangeMove> result = new List<PotvinPDExchangeMove>();
      IPickupAndDeliveryProblemInstance pdp = problemInstance as IPickupAndDeliveryProblemInstance;

      int max = individual.Tours.Count;
      if (individual.Tours.Count < problemInstance.Vehicles.Value)
        max = max - 1;

      for (int i = 0; i < individual.Tours.Count; i++) {
        for (int j = 0; j < individual.Tours[i].Stops.Count; j++) {
          for (int k = 0; k <= max; k++) {
            if (k != i) {
              int city1 = individual.Tours[i].Stops[j];
              if (pdp == null || pdp.GetDemand(city1) >= 0) {
                for (int l = 0; l < individual.Tours[k].Stops.Count; l++) {
                  int city2 = individual.Tours[k].Stops[l];
                  if (pdp == null || pdp.GetDemand(city2) >= 0) {
                    bool valid = pdp == null ||
                      (pdp.GetPickupDeliveryLocation(city2) != pdp.GetPickupDeliveryLocation(city1) &&
                       pdp.GetPickupDeliveryLocation(city2) != city1 &&
                       pdp.GetPickupDeliveryLocation(city1) != city2);

                    if (valid) {
                      PotvinPDExchangeMove move = new PotvinPDExchangeMove(
                        city1, i, k, city2, individual);

                      result.Add(move);
                    }
                  }
                }
              }
            }
          }
        }
      }

      return result.ToArray();
    }
  }
}
