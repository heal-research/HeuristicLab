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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaStochasticLambdaInterchangeMultiMoveGenerator", "Generates multiple random lambda interchange moves from a given VRP encoding.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableType("30C4BF89-1C6F-4A9D-B179-E04FFAD75A8B")]
  public sealed class AlbaStochasticLambdaInterchangeMultiMoveGenerator : AlbaLambdaInterchangeMoveGenerator, IStochasticOperator,
    IMultiMoveGenerator, IAlbaLambdaInterchangeMoveOperator, IMultiVRPMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    [StorableConstructor]
    private AlbaStochasticLambdaInterchangeMultiMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public AlbaStochasticLambdaInterchangeMultiMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaStochasticLambdaInterchangeMultiMoveGenerator(this, cloner);
    }

    private AlbaStochasticLambdaInterchangeMultiMoveGenerator(AlbaStochasticLambdaInterchangeMultiMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public static AlbaLambdaInterchangeMove[] GenerateAllMoves(AlbaEncoding individual, IVRPProblemInstance problemInstance, int lambda, int sampleSize, IRandom rand) {
      AlbaLambdaInterchangeMove[] moves = new AlbaLambdaInterchangeMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        moves[i] = AlbaStochasticLambdaInterchangeSingleMoveGenerator.Apply(
          individual, problemInstance.Cities.Value, lambda, rand);
      }

      return moves;
    }

    protected override AlbaLambdaInterchangeMove[] GenerateMoves(AlbaEncoding individual, IVRPProblemInstance problemInstance, int lambda) {
      return GenerateAllMoves(individual, problemInstance, lambda, SampleSizeParameter.ActualValue.Value, RandomParameter.ActualValue);
    }
  }
}
