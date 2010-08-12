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

using System;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;
using HeuristicLab.Parameters;
using System.Collections.Generic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("ExhaustiveSimpleLocalSearchMoveGenerator", "Generates all possible simple local search moves from a given Alba VRP encoding.")]
  [StorableClass]
  public sealed class ExhaustiveSimpleLocalSearchMoveGenerator : SimpleLocalSearchMoveGenerator, IExhaustiveMoveGenerator, IAlbaSimpleLocalSearchMoveOperator {
    [StorableConstructor]
    private ExhaustiveSimpleLocalSearchMoveGenerator(bool deserializing) : base(deserializing) { }

    public ExhaustiveSimpleLocalSearchMoveGenerator()
      : base() {
    }

    protected override SimpleLocalSearchMove[] GenerateMoves(AlbaEncoding individual) {
      List<SimpleLocalSearchMove> moves = new List<SimpleLocalSearchMove>();

      int currentTourStart = 0;
      int currentTourEnd = 0;
      while(currentTourEnd != individual.Length) {
        currentTourEnd = currentTourStart;
        while (individual[currentTourEnd] < individual.Cities && 
          currentTourEnd < individual.Length) {
          currentTourEnd++;
        }

        int tourLength = currentTourEnd - currentTourStart;
        if (tourLength >= 4) {
          for (int i = 0; i <= tourLength - 4; i++ ) {
            for (int j = i + 2; j <= tourLength - 2; j++) {
              SimpleLocalSearchMove move = new SimpleLocalSearchMove(
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
