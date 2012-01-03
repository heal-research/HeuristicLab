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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaStochasticLambdaInterchangeMultiMoveGenerator", "Generates multiple random lambda interchange moves from a given VRP encoding.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaStochasticLambdaInterchangeMultiMoveGenerator : AlbaLambdaInterchangeMoveGenerator, IStochasticOperator, IMultiMoveGenerator, IAlbaLambdaInterchangeMoveOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    [StorableConstructor]
    private AlbaStochasticLambdaInterchangeMultiMoveGenerator(bool deserializing) : base(deserializing) { }
    private AlbaStochasticLambdaInterchangeMultiMoveGenerator(AlbaStochasticLambdaInterchangeMultiMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public AlbaStochasticLambdaInterchangeMultiMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaStochasticLambdaInterchangeMultiMoveGenerator(this, cloner);
    }

    public static AlbaLambdaInterchangeMove[] GenerateAllMoves(AlbaEncoding individual, int lambda, int sampleSize, IRandom random) {
      AlbaLambdaInterchangeMove[] moves = new AlbaLambdaInterchangeMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        moves[i] = AlbaStochasticLambdaInterchangeSingleMoveGenerator.Apply(
          individual, individual.Cities, lambda, random);
      }

      return moves;
    }

    protected override AlbaLambdaInterchangeMove[] GenerateMoves(AlbaEncoding individual, int lambda) {
      int sampleSize = SampleSizeParameter.ActualValue.Value;

      return GenerateAllMoves(individual, lambda, sampleSize, RandomParameter.ActualValue);
    }
  }
}
