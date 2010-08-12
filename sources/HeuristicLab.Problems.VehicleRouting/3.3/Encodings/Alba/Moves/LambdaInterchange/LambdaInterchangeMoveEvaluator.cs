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

using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("LambdaInterchangeMoveEvaluator", "Evaluates a lamnbda interchange move for the Alba representation.")]
  [StorableClass]
  public sealed class LambdaInterchangeMoveEvaluator : VRPMoveEvaluator, IAlbaLambdaInterchangeMoveOperator {
    public ILookupParameter<LambdaInterchangeMove> LambdaInterchangeMoveParameter {
      get { return (ILookupParameter<LambdaInterchangeMove>)Parameters["LambdaInterchangeMove"]; }
    }

    [StorableConstructor]
    private LambdaInterchangeMoveEvaluator(bool deserializing) : base(deserializing) { }

    public LambdaInterchangeMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<LambdaInterchangeMove>("LambdaInterchangeMove", "The move to evaluate."));
    }

    protected override TourEvaluation GetMoveQuality() {
      LambdaInterchangeMove move = LambdaInterchangeMoveParameter.ActualValue;
      //perform move
      AlbaEncoding newSolution = VRPToursParameter.ActualValue.Clone() as AlbaEncoding;
      LambdaInterchangeMoveMaker.Apply(newSolution, move);

      return VRPEvaluator.Evaluate(
        newSolution,
        DueTimeParameter.ActualValue, ServiceTimeParameter.ActualValue, ReadyTimeParameter.ActualValue,
        DemandParameter.ActualValue, CapacityParameter.ActualValue,
        FleetUsageFactor.ActualValue, TimeFactor.ActualValue, DistanceFactor.ActualValue, OverloadPenalty.ActualValue, TardinessPenalty.ActualValue,
        CoordinatesParameter.ActualValue, DistanceMatrixParameter, UseDistanceMatrixParameter.ActualValue);
    }
  }
}
