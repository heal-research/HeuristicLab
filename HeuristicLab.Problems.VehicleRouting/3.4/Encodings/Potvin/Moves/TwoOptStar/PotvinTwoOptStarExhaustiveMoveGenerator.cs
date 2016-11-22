#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinTwoOptStarExhaustiveMoveGenerator", "Generates two opt star moves from a given VRP encoding.")]
  [StorableClass]
  public sealed class PotvinTwoOptStarExhaustiveMoveGenerator : PotvinTwoOptStarMoveGenerator, IExhaustiveMoveGenerator {
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinTwoOptStarExhaustiveMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinTwoOptStarExhaustiveMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinTwoOptStarExhaustiveMoveGenerator()
      : base() {
    }

    private PotvinTwoOptStarExhaustiveMoveGenerator(PotvinTwoOptStarExhaustiveMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override PotvinTwoOptStarMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinTwoOptStarMove> result = new List<PotvinTwoOptStarMove>();

      for (int tour1 = 0; tour1 < individual.Tours.Count; tour1++) {
        for (int tour2 = tour1 + 1; tour2 < individual.Tours.Count; tour2++) {
          for (int index1 = 0; index1 <= individual.Tours[tour1].Stops.Count; index1++) {
            for (int index2 = 0; index2 <= individual.Tours[tour2].Stops.Count; index2++) {
              if ((index1 != individual.Tours[tour1].Stops.Count || index2 != individual.Tours[tour2].Stops.Count) &&
                  (index1 != 0 || index2 != 0))
                result.Add(new PotvinTwoOptStarMove(tour1, index1, tour2, index2, individual));
            }
          }
        }
      }

      return result.ToArray();
    }
  }
}
