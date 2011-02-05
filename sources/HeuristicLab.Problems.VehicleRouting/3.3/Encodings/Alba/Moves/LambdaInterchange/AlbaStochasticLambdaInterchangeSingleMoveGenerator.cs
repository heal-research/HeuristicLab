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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaStochasticLambdaInterchangeSingleMoveGenerator", "Generates one random lambda interchange move from a given VRP encoding.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaStochasticLambdaInterchangeSingleMoveGenerator : AlbaLambdaInterchangeMoveGenerator,
    IStochasticOperator, ISingleMoveGenerator, IAlbaLambdaInterchangeMoveOperator, IMultiVRPMoveGenerator {
    #region IMultiVRPMoveOperator Members

    public ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["AlbaLambdaInterchangeMove"]; }
    }

    #endregion

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    private AlbaStochasticLambdaInterchangeSingleMoveGenerator(bool deserializing) : base(deserializing) { }
    private AlbaStochasticLambdaInterchangeSingleMoveGenerator(AlbaStochasticLambdaInterchangeSingleMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }
    public AlbaStochasticLambdaInterchangeSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaStochasticLambdaInterchangeSingleMoveGenerator(this, cloner);
    }

    public static AlbaLambdaInterchangeMove Apply(AlbaEncoding individual, int cities, int lambda, IRandom rand) {
      List<Tour> tours = individual.GetTours();

      if (tours.Count > 1) {
        int route1Index = rand.Next(tours.Count);
        Tour route1 = tours[route1Index];

        int route2Index = rand.Next(tours.Count - 1);
        if (route2Index >= route1Index)
          route2Index += 1;
        Tour route2 = tours[route2Index];

        int length1 = rand.Next(Math.Min(lambda + 1, route1.Cities.Count + 1));
        int index1 = rand.Next(route1.Cities.Count - length1 + 1);

        int l2Min = 0;
        if (length1 == 0)
          l2Min = 1;
        int length2 = rand.Next(l2Min, Math.Min(lambda + 1, route2.Cities.Count + 1));
        int index2 = rand.Next(route2.Cities.Count - length2 + 1);

        return new AlbaLambdaInterchangeMove(route1Index, index1, length1, route2Index, index2, length2, individual);
      } else {
        return new AlbaLambdaInterchangeMove(0, 0, 0, 0, 0, 0, individual);
      }
    }

    protected override AlbaLambdaInterchangeMove[] GenerateMoves(AlbaEncoding individual, int lambda) {
      List<AlbaLambdaInterchangeMove> moves = new List<AlbaLambdaInterchangeMove>();

      AlbaLambdaInterchangeMove move = Apply(individual, Cities, lambda, RandomParameter.ActualValue);
      if (move != null)
        moves.Add(move);

      return moves.ToArray();
    }
  }
}
