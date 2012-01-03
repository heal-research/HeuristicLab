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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings;

namespace HeuristicLab.Problems.VehicleRouting {
  public struct TourEvaluation {
    public double Quality { get; set; }
    public double VehcilesUtilized { get; set; }
    public double TravelTime { get; set; }
    public double Distance { get; set; }
    public double Overload { get; set; }
    public double Tardiness { get; set; }
  }

  [Item("VRPEvaluator", "Evaluates solutions for the VRP problem.")]
  [StorableClass]
  public sealed class VRPEvaluator : VRPOperator, IVRPEvaluator {
    #region ISingleObjectiveEvaluator Members
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    #endregion

    public ILookupParameter<DoubleValue> VehcilesUtilizedParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["VehiclesUtilized"]; }
    }
    public ILookupParameter<DoubleValue> TravelTimeParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["TravelTime"]; }
    }
    public ILookupParameter<DoubleValue> DistanceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ILookupParameter<DoubleValue> OverloadParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Overload"]; }
    }
    public ILookupParameter<DoubleValue> TardinessParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }

    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
   
    public ILookupParameter<DoubleValue> FleetUsageFactor {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvalFleetUsageFactor"]; }
    }
    public ILookupParameter<DoubleValue> TimeFactor {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvalTimeFactor"]; }
    }
    public ILookupParameter<DoubleValue> DistanceFactor {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvalDistanceFactor"]; }
    }
    public ILookupParameter<DoubleValue> OverloadPenalty {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvalOverloadPenalty"]; }
    }
    public ILookupParameter<DoubleValue> TardinessPenalty {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvalTardinessPenalty"]; }
    }

    [StorableConstructor]
    private VRPEvaluator(bool deserializing) : base(deserializing) { }
    private VRPEvaluator(VRPEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public VRPEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The evaluated quality of the VRP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("VehiclesUtilized", "The number of vehicles utilized."));
      Parameters.Add(new LookupParameter<DoubleValue>("TravelTime", "The total travel time."));
      Parameters.Add(new LookupParameter<DoubleValue>("Distance", "The distance."));
      Parameters.Add(new LookupParameter<DoubleValue>("Overload", "The overload."));
      Parameters.Add(new LookupParameter<DoubleValue>("Tardiness", "The tardiness."));
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours which should be evaluated."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalFleetUsageFactor", "The fleet usage factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalTimeFactor", "The time factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalDistanceFactor", "The distance factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalOverloadPenalty", "The overload penalty considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalTardinessPenalty", "The tardiness penalty considered in the evaluation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VRPEvaluator(this, cloner);
    }

    private double CalculateFleetUsage() {
      IVRPEncoding vrpSolution = VRPToursParameter.ActualValue;

      return vrpSolution.GetTours(DistanceMatrixParameter, VehiclesParameter.ActualValue.Value).Count;
    }

    internal static TourEvaluation EvaluateTour(Tour tour, DoubleArray dueTimeArray,
      DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor, DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      DistanceMatrix distMatrix) {
      TourEvaluation eval = new TourEvaluation();

      double quality = 0.0;
      double time = 0.0;

      double distance = 0.0;
      double waitingTime = 0.0;
      double serviceTime = 0.0;
      double delivered = 0.0;

      double overweight = 0.0;
      double tardiness = 0.0;

      //simulate a tour, start and end at depot
      for (int i = 0; i <= tour.Cities.Count; i++) {
        int start = 0;
        if(i > 0)
          start = tour.Cities[i - 1];
        int end = 0;
        if (i < tour.Cities.Count)
          end = tour.Cities[i];

        //drive there
        double currentDistace = VRPUtilities.GetDistance(start, end, distMatrix);
        distance += currentDistace;
        time += currentDistace;

        //check if it was serviced on time
        if (time > dueTimeArray[end])
          tardiness += time - dueTimeArray[end];

        //wait
        double currentWaitingTime = 0.0;
        if (time < readyTimeArray[end])
          currentWaitingTime = readyTimeArray[end] - time;
        waitingTime += currentWaitingTime;
        time += currentWaitingTime;

        //service
        delivered += demandArray[end];
        double currentServiceTime = serviceTimeArray[end];
        serviceTime += currentServiceTime;
        time += currentServiceTime;
      }

      if (delivered > capacity.Value) {
        overweight = delivered - capacity.Value;
      }

      //Fleet usage
      quality += fleetUsageFactor.Value;
      //Travel time
      quality += timeFactor.Value * time;
      //Distance
      quality += distanceFactor.Value * distance;

      //Penalties
      quality += overloadPenalty.Value * overweight;
      quality += tardinessPenalty.Value * tardiness;

      eval.Distance = distance;
      eval.TravelTime = time;
      eval.VehcilesUtilized = 1;
      eval.Overload = overweight;
      eval.Tardiness = tardiness;
      eval.Quality = quality;

      return eval;
    }

    public static TourEvaluation Evaluate(IVRPEncoding solution, IntValue vehicles, DoubleArray dueTimeArray, 
      DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor, DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      DoubleMatrix coordinates, IParameter distanceMatrix, BoolValue useDistanceMatrix) {
      TourEvaluation sumEval = new TourEvaluation();
      sumEval.Distance = 0;
      sumEval.Quality = 0;
      sumEval.TravelTime = 0;
      sumEval.VehcilesUtilized = 0;
      sumEval.Overload = 0;
      sumEval.Tardiness = 0;

      DistanceMatrix distMatrix = VRPUtilities.GetDistanceMatrix(coordinates, distanceMatrix, useDistanceMatrix);

      foreach (Tour tour in solution.GetTours(distanceMatrix as ILookupParameter<DoubleMatrix>)) {
        TourEvaluation eval = EvaluateTour(tour, dueTimeArray, serviceTimeArray, readyTimeArray, demandArray, capacity,
          fleetUsageFactor, timeFactor, distanceFactor, overloadPenalty, tardinessPenalty,
          distMatrix);
        sumEval.Quality += eval.Quality;
        sumEval.Distance += eval.Distance;
        sumEval.TravelTime += eval.TravelTime;
        sumEval.VehcilesUtilized += eval.VehcilesUtilized;
        sumEval.Overload += eval.Overload;
        sumEval.Tardiness += eval.Tardiness;
      }

      return sumEval;
    }

    public sealed override IOperation Apply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;

      TourEvaluation sumEval = Evaluate(solution, VehiclesParameter.ActualValue, DueTimeParameter.ActualValue, ServiceTimeParameter.ActualValue, ReadyTimeParameter.ActualValue,
        DemandParameter.ActualValue, CapacityParameter.ActualValue,
        FleetUsageFactor.ActualValue, TimeFactor.ActualValue, DistanceFactor.ActualValue, OverloadPenalty.ActualValue, TardinessPenalty.ActualValue,
        CoordinatesParameter.ActualValue, DistanceMatrixParameter, UseDistanceMatrixParameter.ActualValue);

      QualityParameter.ActualValue = new DoubleValue(sumEval.Quality);
      VehcilesUtilizedParameter.ActualValue = new DoubleValue(sumEval.VehcilesUtilized);
      TravelTimeParameter.ActualValue = new DoubleValue(sumEval.TravelTime);
      DistanceParameter.ActualValue = new DoubleValue(sumEval.Distance);
      OverloadParameter.ActualValue = new DoubleValue(sumEval.Overload);
      TardinessParameter.ActualValue = new DoubleValue(sumEval.Tardiness);

      return base.Apply();
    }
  }
}
