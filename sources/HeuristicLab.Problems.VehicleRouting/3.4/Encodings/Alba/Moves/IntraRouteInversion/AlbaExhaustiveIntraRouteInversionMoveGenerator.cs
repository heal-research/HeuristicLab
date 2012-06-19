#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaExhaustiveIntraRouteInversionGenerator", "Generates all possible intra route inversion moves from a given VRP encoding.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaExhaustiveIntraRouteInversionGenerator : AlbaIntraRouteInversionMoveGenerator, IExhaustiveMoveGenerator, IAlbaIntraRouteInversionMoveOperator {
    [StorableConstructor]
    private AlbaExhaustiveIntraRouteInversionGenerator(bool deserializing) : base(deserializing) { }

    public AlbaExhaustiveIntraRouteInversionGenerator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaExhaustiveIntraRouteInversionGenerator(this, cloner);
    }

    private AlbaExhaustiveIntraRouteInversionGenerator(AlbaExhaustiveIntraRouteInversionGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override AlbaIntraRouteInversionMove[] GenerateMoves(AlbaEncoding individual, IVRPProblemInstance problemInstance) {
      List<AlbaIntraRouteInversionMove> moves = new List<AlbaIntraRouteInversionMove>();

      int currentTourStart = 0;
      int currentTourEnd = 0;
      while (currentTourEnd != individual.Length) {
        currentTourEnd = currentTourStart;
        while (individual[currentTourEnd] < problemInstance.Cities.Value &&
          currentTourEnd < individual.Length) {
          currentTourEnd++;
        }

        int tourLength = currentTourEnd - currentTourStart;
        if (tourLength >= 4) {
          for (int i = 0; i <= tourLength - 4; i++) {
            for (int j = i + 2; j <= tourLength - 2; j++) {
              AlbaIntraRouteInversionMove move = new AlbaIntraRouteInversionMove(
                currentTourStart + i,
                currentTourStart + j,
                individual);

              moves.Add(move);
            }
          }
        }

        currentTourStart = currentTourEnd;
      }

      return moves.ToArray();
    }
  }
}
