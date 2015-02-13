#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaExhaustiveLambdaInterchangeMoveGenerator", "Generates all possible lambda interchange moves from a given VRP encoding.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaExhaustiveLambdaInterchangeMoveGenerator : AlbaLambdaInterchangeMoveGenerator, IExhaustiveMoveGenerator, IAlbaLambdaInterchangeMoveOperator {
    [StorableConstructor]
    private AlbaExhaustiveLambdaInterchangeMoveGenerator(bool deserializing) : base(deserializing) { }

    public AlbaExhaustiveLambdaInterchangeMoveGenerator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaExhaustiveLambdaInterchangeMoveGenerator(this, cloner);
    }

    private AlbaExhaustiveLambdaInterchangeMoveGenerator(AlbaExhaustiveLambdaInterchangeMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override AlbaLambdaInterchangeMove[] GenerateMoves(AlbaEncoding individual, IVRPProblemInstance problemInstance, int lambda) {
      List<AlbaLambdaInterchangeMove> moves = new List<AlbaLambdaInterchangeMove>();

      List<Tour> tours = individual.GetTours();

      for (int tour1Index = 0; tour1Index < tours.Count; tour1Index++) {
        Tour tour1 = tours[tour1Index];
        for (int tour2Index = tour1Index + 1; tour2Index < tours.Count; tour2Index++) {
          Tour tour2 = tours[tour2Index];

          for (int length1 = 0; length1 <= Math.Min(lambda, tour1.Stops.Count); length1++) {
            for (int length2 = 0; length2 <= Math.Min(lambda, tour2.Stops.Count); length2++) {
              if (length1 != 0 || length2 != 0) {
                for (int index1 = 0; index1 < tour1.Stops.Count - length1 + 1; index1++) {
                  for (int index2 = 0; index2 < tour2.Stops.Count - length2 + 1; index2++) {
                    moves.Add(new AlbaLambdaInterchangeMove(tour1Index, index1, length1,
                      tour2Index, index2, length2, individual));
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
