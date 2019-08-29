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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinCustomerRelocationExhaustiveMoveGenerator", "Generates customer relocation moves from a given VRP encoding.")]
  [StorableType("713A5EF1-7CA7-4992-AA8B-5BCF430FA4B4")]
  public sealed class PotvinCustomerRelocationExhaustiveMoveGenerator : PotvinCustomerRelocationMoveGenerator, IExhaustiveMoveGenerator {
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinCustomerRelocationExhaustiveMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinCustomerRelocationExhaustiveMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public PotvinCustomerRelocationExhaustiveMoveGenerator()
      : base() {
    }

    private PotvinCustomerRelocationExhaustiveMoveGenerator(PotvinCustomerRelocationExhaustiveMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override PotvinCustomerRelocationMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinCustomerRelocationMove> result = new List<PotvinCustomerRelocationMove>();

      int max = individual.Tours.Count;
      if (individual.Tours.Count >= problemInstance.Vehicles.Value)
        max = max - 1;

      for (int i = 0; i < individual.Tours.Count; i++) {
        for (int j = 0; j < individual.Tours[i].Stops.Count; j++) {
          for (int k = 0; k <= max; k++) {
            if (k != i) {
              PotvinCustomerRelocationMove move = new PotvinCustomerRelocationMove(
                individual.Tours[i].Stops[j], i, k, individual);

              result.Add(move);
            }
          }
        }
      }

      return result.ToArray();
    }
  }
}
