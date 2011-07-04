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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("MultiVRPMoveEvaluator", "Evaluates a move for the VRP representation.")]
  [StorableClass]
  public sealed class MultiVRPMoveEvaluator : VRPMoveEvaluator, IMultiVRPMoveOperator {
    public ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["VRPMove"]; }
    }

    [StorableConstructor]
    private MultiVRPMoveEvaluator(bool deserializing) : base(deserializing) { }
    private MultiVRPMoveEvaluator(MultiVRPMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPMoveEvaluator(this, cloner);
    }

    public MultiVRPMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPMove>("VRPMove", "The generated moves."));
    }

    protected override TourEvaluation GetMoveQuality() {
      IVRPMove move = VRPMoveParameter.ActualValue as IVRPMove;

      return move.GetMoveQuality(
        VehiclesParameter.ActualValue,
        DueTimeParameter.ActualValue,
        ServiceTimeParameter.ActualValue,
        ReadyTimeParameter.ActualValue,
        DemandParameter.ActualValue,
        CapacityParameter.ActualValue,
        CoordinatesParameter.ActualValue,
        FleetUsageFactor.ActualValue,
        TimeFactor.ActualValue,
        DistanceFactor.ActualValue,
        OverloadPenalty.ActualValue,
        TardinessPenalty.ActualValue,
        DistanceMatrixParameter,
        UseDistanceMatrixParameter.ActualValue);
    }
  }
}
