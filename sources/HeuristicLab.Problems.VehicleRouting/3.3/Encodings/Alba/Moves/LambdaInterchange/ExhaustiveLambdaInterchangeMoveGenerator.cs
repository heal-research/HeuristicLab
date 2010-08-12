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
  [Item("ExhaustiveLambdaInterchangeMoveGenerator", "Generates all possible lambda interchange moves from a given Alba VRP encoding.")]
  [StorableClass]
  public sealed class ExhaustiveLambdaInterchangeMoveGenerator : LambdaInterchangeMoveGenerator, IExhaustiveMoveGenerator, IAlbaLambdaInterchangeMoveOperator {
    [StorableConstructor]
    private ExhaustiveLambdaInterchangeMoveGenerator(bool deserializing) : base(deserializing) { }

    public ExhaustiveLambdaInterchangeMoveGenerator()
      : base() {
    }

    protected override LambdaInterchangeMove[] GenerateMoves(AlbaEncoding individual, int lambda) {
      List<LambdaInterchangeMove> moves = new List<LambdaInterchangeMove>();

      for (int tour1Index = 0; tour1Index < individual.Tours.Count; tour1Index++) {
        Tour tour1 = individual.Tours[tour1Index];
        for (int tour2Index = tour1Index + 1; tour2Index < individual.Tours.Count; tour2Index++) {
          Tour tour2 = individual.Tours[tour2Index];

          for (int length1 = 0; length1 <= Math.Min(lambda, tour1.Cities.Count); length1++) {
            for(int length2 = 0; length2 <= Math.Min(lambda, tour2.Cities.Count); length2++) {
              if(length1 != 0 || length2 != 0) {
                for(int index1 = 0; index1 < tour1.Cities.Count - length1 + 1; index1++) {
                  for(int index2 = 0; index2 < tour2.Cities.Count - length2 + 1; index2++) {
                    moves.Add(new LambdaInterchangeMove(tour1Index, index1, length1, 
                      tour2Index, index2, length2));
                  }
                }
              }
            }
          }
        }
      }

      return moves.ToArray();
    }
  }
}
