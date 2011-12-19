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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("AlbaLambdaInterchangeMoveEvaluator", "Evaluates a lamnbda interchange move for a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaLambdaInterchangeMoveEvaluator : VRPMoveEvaluator, IAlbaLambdaInterchangeMoveOperator {
    public ILookupParameter<AlbaLambdaInterchangeMove> LambdaInterchangeMoveParameter {
      get { return (ILookupParameter<AlbaLambdaInterchangeMove>)Parameters["AlbaLambdaInterchangeMove"]; }
    }

    [StorableConstructor]
    private AlbaLambdaInterchangeMoveEvaluator(bool deserializing) : base(deserializing) { }
    private AlbaLambdaInterchangeMoveEvaluator(AlbaLambdaInterchangeMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public AlbaLambdaInterchangeMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<AlbaLambdaInterchangeMove>("AlbaLambdaInterchangeMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaLambdaInterchangeMoveEvaluator(this, cloner);
    }
    public static TourEvaluation GetMoveQuality(AlbaEncoding individual, AlbaLambdaInterchangeMove move, 
      IntValue vehicles, 
      DoubleArray dueTimeArray, DoubleArray serviceTimeArray, DoubleArray readyTimeArray, 
      DoubleArray demandArray, DoubleValue capacity, DoubleMatrix coordinates, 
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor, 
      DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      ILookupParameter<DoubleMatrix> distanceMatrix, Data.BoolValue useDistanceMatrix) {
      AlbaEncoding newSolution = individual.Clone() as AlbaEncoding;
      AlbaLambdaInterchangeMoveMaker.Apply(newSolution, move);

      return VRPEvaluator.Evaluate(
        newSolution, vehicles, dueTimeArray, serviceTimeArray, readyTimeArray, 
        demandArray, capacity, fleetUsageFactor, timeFactor, distanceFactor, 
        overloadPenalty, tardinessPenalty, coordinates, distanceMatrix, useDistanceMatrix);
    }

    protected override TourEvaluation GetMoveQuality() {
      return GetMoveQuality(
        LambdaInterchangeMoveParameter.ActualValue.Individual as AlbaEncoding, LambdaInterchangeMoveParameter.ActualValue, 
        VehiclesParameter.ActualValue,
        DueTimeParameter.ActualValue, ServiceTimeParameter.ActualValue, ReadyTimeParameter.ActualValue,
        DemandParameter.ActualValue, CapacityParameter.ActualValue, CoordinatesParameter.ActualValue, 
        FleetUsageFactor.ActualValue, TimeFactor.ActualValue, DistanceFactor.ActualValue, OverloadPenalty.ActualValue, 
        TardinessPenalty.ActualValue, DistanceMatrixParameter, UseDistanceMatrixParameter.ActualValue);
    }
  }
}
