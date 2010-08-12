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
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("StochasticLambdaInterchangeSingleMoveGenerator", "Generates one random lambda interchange move from a given Alba VRP encoding.")]
  [StorableClass]
  public sealed class StochasticLambdaInterchangeSingleMoveGenerator : LambdaInterchangeMoveGenerator,
    IStochasticOperator, ISingleMoveGenerator, IAlbaLambdaInterchangeMoveOperator, IMultiVRPMoveGenerator {
    #region IMultiVRPMoveOperator Members

    public ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["LambdaInterchangeMove"]; }
    }

    #endregion
    
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    
    [StorableConstructor]
    private StochasticLambdaInterchangeSingleMoveGenerator(bool deserializing) : base(deserializing) { }

    public StochasticLambdaInterchangeSingleMoveGenerator()
      : base() {
        Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public static LambdaInterchangeMove Apply(AlbaEncoding individual, int cities, int lambda, IRandom rand) {
      int route1Index = rand.Next(individual.Tours.Count);
      Tour route1 = individual.Tours[route1Index];

      int route2Index = rand.Next(individual.Tours.Count - 1);
      if (route2Index >= route1Index)
        route2Index += 1;
      Tour route2 = individual.Tours[route2Index];

      int length1 = rand.Next(Math.Min(lambda + 1, route1.Cities.Count + 1));
      int index1 = rand.Next(route1.Cities.Count - length1 + 1);

      int l2Min = 0;
      if (length1 == 0)
        l2Min = 1;
      int length2 = rand.Next(l2Min, Math.Min(lambda + 1, route2.Cities.Count + 1));
      int index2 = rand.Next(route2.Cities.Count - length2 + 1);

      return new LambdaInterchangeMove(route1Index, index1, length1, route2Index, index2, length2);
    }

    protected override LambdaInterchangeMove[] GenerateMoves(AlbaEncoding individual, int lambda) {
      List<LambdaInterchangeMove> moves = new List<LambdaInterchangeMove>();

      LambdaInterchangeMove move = Apply(individual, Cities, lambda, RandomParameter.ActualValue);
      if(move != null)
        moves.Add(move);

      return moves.ToArray();
    }
  }
}
