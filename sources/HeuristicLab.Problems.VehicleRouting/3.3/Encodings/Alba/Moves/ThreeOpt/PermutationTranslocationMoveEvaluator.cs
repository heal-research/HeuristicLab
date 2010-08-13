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
  [Item("PermutationTranslocationMoveEvaluator", "Evaluates a translocation or insertion move (3-opt) for a VRP representation.")]
  [StorableClass]
  public sealed class PermutationTranslocationMoveEvaluator : VRPMoveEvaluator, IAlbaTranslocationMoveOperator {
    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return (ILookupParameter<TranslocationMove>)Parameters["TranslocationMove"]; }
      set { Parameters["TranslocationMove"].ActualValue = value.ActualValue; }
    }

    [StorableConstructor]
    private PermutationTranslocationMoveEvaluator(bool deserializing) : base(deserializing) { }

    public PermutationTranslocationMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<TranslocationMove>("TranslocationMove", "The move to evaluate."));
    }

    protected override TourEvaluation GetMoveQuality() {
      TranslocationMove move = TranslocationMoveParameter.ActualValue;
      //perform move
      AlbaEncoding newSolution = VRPToursParameter.ActualValue.Clone() as AlbaEncoding;
      TranslocationManipulator.Apply(newSolution, move.Index1, move.Index2, move.Index3);

      return VRPEvaluator.Evaluate(
        newSolution,
        DueTimeParameter.ActualValue, ServiceTimeParameter.ActualValue, ReadyTimeParameter.ActualValue,
        DemandParameter.ActualValue, CapacityParameter.ActualValue,
        FleetUsageFactor.ActualValue, TimeFactor.ActualValue, DistanceFactor.ActualValue, OverloadPenalty.ActualValue, TardinessPenalty.ActualValue,
        CoordinatesParameter.ActualValue, DistanceMatrixParameter, UseDistanceMatrixParameter.ActualValue);
    }
  }
}
