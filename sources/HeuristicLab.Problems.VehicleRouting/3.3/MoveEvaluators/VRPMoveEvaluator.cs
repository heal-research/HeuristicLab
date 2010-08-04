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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("VRPMoveEvaluator", "A base class for operators which evaluate VRP moves.")]
  [StorableClass]
  public abstract class VRPMoveEvaluator : VRPOperator, IVRPMoveEvaluator, IMoveOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IVRPEncoding> VRPSolutionParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPSolution"]; }
    }
   
    public ILookupParameter<DoubleValue> FleetUsageFactor {
      get { return (ILookupParameter<DoubleValue>)Parameters["FleetUsageFactor"]; }
    }
    public ILookupParameter<DoubleValue> TimeFactor {
      get { return (ILookupParameter<DoubleValue>)Parameters["TimeFactor"]; }
    }
    public ILookupParameter<DoubleValue> DistanceFactor {
      get { return (ILookupParameter<DoubleValue>)Parameters["DistanceFactor"]; }
    }
    public ILookupParameter<DoubleValue> OverloadPenalty {
      get { return (ILookupParameter<DoubleValue>)Parameters["OverloadPenalty"]; }
    }
    public ILookupParameter<DoubleValue> TardinessPenalty {
      get { return (ILookupParameter<DoubleValue>)Parameters["TardinessPenalty"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<DoubleValue> MoveVehcilesUtilizedParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveVehiclesUtilized"]; }
    }
    public ILookupParameter<DoubleValue> MoveTravelTimeParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveTravelTime"]; }
    }
    public ILookupParameter<DoubleValue> MoveDistanceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveDistance"]; }
    }
    public ILookupParameter<DoubleValue> MoveOverloadParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveOverload"]; }
    }
    public ILookupParameter<DoubleValue> MoveTardinessParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveTardiness"]; }
    }

    protected VRPMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPSolution", "The VRP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a VRP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveVehiclesUtilized", "The number of vehicles utilized."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveTravelTime", "The total travel time."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveDistance", "The distance."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveOverload", "The overload."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveTardiness", "The tardiness."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The evaluated quality of a move on a VRP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("FleetUsageFactor", "The fleet usage factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("TimeFactor", "The time factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("DistanceFactor", "The distance factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("OverloadPenalty", "The overload penalty considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("TardinessPenalty", "The tardiness penalty considered in the evaluation."));
    }

    protected abstract TourEvaluation GetMoveQuality();

    public override IOperation Apply() {
      TourEvaluation tourEval = GetMoveQuality();

      MoveQualityParameter.ActualValue = new DoubleValue(tourEval.Quality);
      MoveDistanceParameter.ActualValue = new DoubleValue(tourEval.Distance);
      MoveVehcilesUtilizedParameter.ActualValue = new DoubleValue(tourEval.VehcilesUtilized);
      MoveOverloadParameter.ActualValue = new DoubleValue(tourEval.Overload);
      MoveTardinessParameter.ActualValue = new DoubleValue(tourEval.Tardiness);
      MoveTravelTimeParameter.ActualValue = new DoubleValue(tourEval.TravelTime);

      return base.Apply();
    }
  }
}
