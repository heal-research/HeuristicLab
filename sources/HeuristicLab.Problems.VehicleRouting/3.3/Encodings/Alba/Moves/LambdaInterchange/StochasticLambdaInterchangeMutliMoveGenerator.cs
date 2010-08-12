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
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("StochasticLambdaInterchangeMultiMoveGenerator", "Generates multiple random lambda interchange moves from a given Alba VRP encoding.")]
  [StorableClass]
  public sealed class StochasticLambdaInterchangeMultiMoveGenerator : LambdaInterchangeMoveGenerator, IStochasticOperator, IMultiMoveGenerator, IAlbaLambdaInterchangeMoveOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }
    
    [StorableConstructor]
    private StochasticLambdaInterchangeMultiMoveGenerator(bool deserializing) : base(deserializing) { }

    public StochasticLambdaInterchangeMultiMoveGenerator()
      : base() {
        Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
        Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    protected override LambdaInterchangeMove[] GenerateMoves(AlbaEncoding individual, int lambda) {
      int sampleSize = SampleSizeParameter.ActualValue.Value;

      LambdaInterchangeMove[] moves = new LambdaInterchangeMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        moves[i] = StochasticLambdaInterchangeSingleMoveGenerator.Apply(
          individual, Cities, lambda, RandomParameter.ActualValue);
      }

      return moves;
    }
  }
}
