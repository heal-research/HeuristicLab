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
  public sealed class VRPEvaluator : SingleSuccessorOperator, IVRPEvaluator {
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

    public ILookupParameter<IVRPEncoding> VRPSolutionParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPSolution"]; }
    }
    public ILookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ILookupParameter<DoubleMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ILookupParameter<BoolValue> UseDistanceMatrixParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["UseDistanceMatrix"]; }
    }
    public ILookupParameter<IntValue> VehiclesParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Vehicles"]; }
    }
    public ILookupParameter<DoubleValue> CapacityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Capacity"]; }
    }
    public ILookupParameter<DoubleArray> DemandParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Demand"]; }
    }
    public ILookupParameter<DoubleArray> ReadyTimeParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["ReadyTime"]; }
    }
    public ILookupParameter<DoubleArray> DueTimeParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["DueTime"]; }
    }
    public ILookupParameter<DoubleArray> ServiceTimeParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["ServiceTime"]; }
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

    public VRPEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The evaluated quality of the VRP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("VehiclesUtilized", "The number of vehicles utilized."));
      Parameters.Add(new LookupParameter<DoubleValue>("TravelTime", "The total travel time."));
      Parameters.Add(new LookupParameter<DoubleValue>("Distance", "The distance."));
      Parameters.Add(new LookupParameter<DoubleValue>("Overload", "The overload."));
      Parameters.Add(new LookupParameter<DoubleValue>("Tardiness", "The tardiness."));
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPSolution", "The VRP solution which should be evaluated."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The coordinates of the cities."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new LookupParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated and used for evaluation, otherwise false."));
      Parameters.Add(new LookupParameter<IntValue>("Vehicles", "The number of vehicles."));
      Parameters.Add(new LookupParameter<DoubleValue>("Capacity", "The capacity of each vehicle."));
      Parameters.Add(new LookupParameter<DoubleArray>("Demand", "The demand of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("ReadyTime", "The ready time of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("DueTime", "The due time of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("ServiceTime", "The service time of each customer."));
      Parameters.Add(new LookupParameter<DoubleValue>("FleetUsageFactor", "The fleet usage factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("TimeFactor", "The time factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("DistanceFactor", "The distance factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("OverloadPenalty", "The overload penalty considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("TardinessPenalty", "The tardiness penalty considered in the evaluation."));
    }

    private double CalculateFleetUsage() {
      IVRPEncoding vrpSolution = VRPSolutionParameter.ActualValue;

      return vrpSolution.Tours.Count;
    }

    private static TourEvaluation EvaluateTour(Tour tour, DoubleArray dueTimeArray,
      DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor, DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      DoubleMatrix coordinates, ILookupParameter<DoubleMatrix> distanceMatrix, BoolValue useDistanceMatrix) {
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
      for (int i = 1; i < tour.Count; i++) {
        int start = tour[i - 1].Value;
        int end = tour[i].Value;

        //drive there
        double currentDistace = VehicleRoutingProblem.GetDistance(start, end, coordinates, distanceMatrix, useDistanceMatrix);
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

    public static TourEvaluation Evaluate(IVRPEncoding solution, DoubleArray dueTimeArray,
      DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleValue fleetUsageFactor, DoubleValue timeFactor, DoubleValue distanceFactor, DoubleValue overloadPenalty, DoubleValue tardinessPenalty,
      DoubleMatrix coordinates, ILookupParameter<DoubleMatrix> distanceMatrix, BoolValue useDistanceMatrix) {
      TourEvaluation sumEval = new TourEvaluation();
      sumEval.Distance = 0;
      sumEval.Quality = 0;
      sumEval.TravelTime = 0;
      sumEval.VehcilesUtilized = 0;
      sumEval.Overload = 0;
      sumEval.Tardiness = 0;

      foreach (Tour tour in solution.Tours) {
        TourEvaluation eval = EvaluateTour(tour, dueTimeArray, serviceTimeArray, readyTimeArray, demandArray, capacity,
          fleetUsageFactor, timeFactor, distanceFactor, overloadPenalty, tardinessPenalty,
          coordinates, distanceMatrix, useDistanceMatrix);
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
      IVRPEncoding solution = VRPSolutionParameter.ActualValue;

      TourEvaluation sumEval = Evaluate(solution, DueTimeParameter.ActualValue, ServiceTimeParameter.ActualValue, ReadyTimeParameter.ActualValue,
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
