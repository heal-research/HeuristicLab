#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("MDCVRPTWEvaluator", "Represents a multi depot CVRPTW evaluator.")]
  [StorableClass]
  public class MDCVRPTWEvaluator : MDCVRPEvaluator {
    public ILookupParameter<DoubleValue> TardinessParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }

    public ILookupParameter<DoubleValue> TravelTimeParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["TravelTime"]; }
    }

    protected override VRPEvaluation CreateTourEvaluation() {
      return new CVRPTWEvaluation();
    }

    protected override void EvaluateTour(VRPEvaluation eval, IVRPProblemInstance instance, Tour tour, IVRPEncoding solution) {
      TourInsertionInfo tourInfo = new TourInsertionInfo(solution.GetVehicleAssignment(solution.GetTourIndex(tour)));
      eval.InsertionInfo.AddTourInsertionInfo(tourInfo);
      double originalQuality = eval.Quality;

      IHeterogenousCapacitatedProblemInstance cvrpInstance = instance as IHeterogenousCapacitatedProblemInstance;
      DoubleArray demand = instance.Demand;

      ITimeWindowedProblemInstance vrptw = instance as ITimeWindowedProblemInstance;
      DoubleArray dueTime = vrptw.DueTime;
      DoubleArray readyTime = vrptw.ReadyTime;
      DoubleArray serviceTimes = vrptw.ServiceTime;

      IMultiDepotProblemInstance mdp = instance as IMultiDepotProblemInstance;
      IntArray vehicleAssignment = mdp.VehicleDepotAssignment;
      int depots = mdp.Depots.Value;

      double time = 0.0;
      double waitingTime = 0.0;
      double serviceTime = 0.0;
      double tardiness = 0.0;
      double delivered = 0.0;
      double overweight = 0.0;
      double distance = 0.0;

      int tourIndex = solution.GetTourIndex(tour);
      int vehicle = solution.GetVehicleAssignment(tourIndex);
      int depot = vehicleAssignment[vehicle];

      double capacity = cvrpInstance.Capacity[vehicle];
      for (int i = 0; i < tour.Stops.Count; i++) {
        delivered += instance.GetDemand(tour.Stops[i]);
      }

      double spareCapacity = capacity - delivered;

      double tourStartTime = vrptw.ReadyTime[depot];
      time = tourStartTime;

      //simulate a tour, start and end at depot
      for (int i = 0; i <= tour.Stops.Count; i++) {
        int start = 0;
        if (i > 0)
          start = tour.Stops[i - 1];
        int end = 0;
        if (i < tour.Stops.Count)
          end = tour.Stops[i];

        //drive there
        double currentDistace = vrptw.GetDistance(start, end, solution);
        time += currentDistace;
        distance += currentDistace;

        double arrivalTime = time;

        int endIndex;
        if (end == 0)
          endIndex = depot;
        else
          endIndex = end + depots - 1;

        //check if it was serviced on time
        if (time > dueTime[endIndex])
          tardiness += time - dueTime[endIndex];

        //wait
        double currentWaitingTime = 0.0;
        if (time < readyTime[endIndex])
          currentWaitingTime = readyTime[endIndex] - time;

        double waitTime = readyTime[endIndex] - time;

        waitingTime += currentWaitingTime;
        time += currentWaitingTime;

        double spareTime = dueTime[endIndex] - time;

        //service
        double currentServiceTime = 0;
        if (end > 0)
          currentServiceTime = serviceTimes[end - 1];
        serviceTime += currentServiceTime;
        time += currentServiceTime;

        CVRPTWInsertionInfo stopInfo = new CVRPTWInsertionInfo(start, end, spareCapacity, tourStartTime, arrivalTime, time, spareTime, waitTime);
        tourInfo.AddStopInsertionInfo(stopInfo);
      }

      eval.Quality += instance.FleetUsageFactor.Value;
      eval.Quality += instance.DistanceFactor.Value * distance;
      eval.Distance += distance;
      eval.VehicleUtilization += 1;

      if (delivered > capacity) {
        overweight = delivered - capacity;
      }

      (eval as CVRPEvaluation).Overload += overweight;
      double tourPenalty = 0;
      double penalty = overweight * cvrpInstance.OverloadPenalty.Value;
      eval.Penalty += penalty;
      eval.Quality += penalty;
      tourPenalty += penalty;

      (eval as CVRPTWEvaluation).Tardiness += tardiness;
      (eval as CVRPTWEvaluation).TravelTime += time;

      penalty = tardiness * vrptw.TardinessPenalty.Value;
      eval.Penalty += penalty;
      eval.Quality += penalty;
      tourPenalty += penalty;
      eval.Quality += time * vrptw.TimeFactor.Value;
      tourInfo.Penalty = tourPenalty;
      tourInfo.Quality = eval.Quality - originalQuality;
    }

    protected override double GetTourInsertionCosts(IVRPProblemInstance instance, IVRPEncoding solution, TourInsertionInfo tourInsertionInfo, int index, int customer,
      out bool feasible) {
      CVRPTWInsertionInfo insertionInfo = tourInsertionInfo.GetStopInsertionInfo(index) as CVRPTWInsertionInfo;

      double costs = 0;
      feasible = tourInsertionInfo.Penalty < double.Epsilon;

      ICapacitatedProblemInstance cvrp = instance as ICapacitatedProblemInstance;
      double overloadPenalty = cvrp.OverloadPenalty.Value;

      ITimeWindowedProblemInstance vrptw = instance as ITimeWindowedProblemInstance;
      DoubleArray dueTime = vrptw.DueTime;
      DoubleArray readyTime = vrptw.ReadyTime;
      DoubleArray serviceTimes = vrptw.ServiceTime;
      double tardinessPenalty = vrptw.TardinessPenalty.Value;

      IMultiDepotProblemInstance mdp = instance as IMultiDepotProblemInstance;
      int depots = mdp.Depots.Value;

      double startDistance, endDistance;
      costs += instance.GetInsertionDistance(insertionInfo.Start, customer, insertionInfo.End, solution, out startDistance, out endDistance);

      double demand = instance.GetDemand(customer);
      if (demand > insertionInfo.SpareCapacity) {
        feasible = false;

        if (insertionInfo.SpareCapacity >= 0)
          costs += (demand - insertionInfo.SpareCapacity) * overloadPenalty;
        else
          costs += demand * overloadPenalty;
      }

      double time = 0;
      double tardiness = 0;

      if (index > 0)
        time = (tourInsertionInfo.GetStopInsertionInfo(index - 1) as CVRPTWInsertionInfo).LeaveTime;
      else
        time = insertionInfo.TourStartTime;

      time += startDistance;

      int customerIndex = customer + depots - 1;

      if (time > dueTime[customerIndex]) {
        tardiness += time - dueTime[customerIndex];
      }
      if (time < readyTime[customerIndex])
        time += readyTime[customerIndex] - time;
      time += serviceTimes[customer - 1];
      time += endDistance;

      double additionalTime = time - (tourInsertionInfo.GetStopInsertionInfo(index) as CVRPTWInsertionInfo).ArrivalTime;
      for (int i = index; i < tourInsertionInfo.GetStopCount(); i++) {
        CVRPTWInsertionInfo nextStop = tourInsertionInfo.GetStopInsertionInfo(i) as CVRPTWInsertionInfo;

        if (additionalTime < 0) {
          //arrive earlier than before
          //wait probably
          if (nextStop.WaitingTime < 0) {
            double wait = nextStop.WaitingTime - additionalTime;
            if (wait > 0)
              additionalTime += wait;
          } else {
            additionalTime = 0;
          }

          //check due date, decrease tardiness
          if (nextStop.SpareTime < 0) {
            costs += Math.Max(nextStop.SpareTime, additionalTime) * tardinessPenalty;
          }
        } else {
          //arrive later than before, probably don't have to wait
          if (nextStop.WaitingTime > 0) {
            additionalTime -= Math.Min(additionalTime, nextStop.WaitingTime);
          }

          //check due date
          if (nextStop.SpareTime > 0) {
            double spare = nextStop.SpareTime - additionalTime;
            if (spare < 0)
              tardiness += -spare;
          } else {
            tardiness += additionalTime;
          }
        }
      }

      costs += additionalTime * vrptw.TimeFactor.Value;

      if (tardiness > 0) {
        feasible = false;
      }

      costs += tardiness * tardinessPenalty;

      return costs;
    }

    protected override void InitResultParameters() {
      base.InitResultParameters();

      TardinessParameter.ActualValue = new DoubleValue(0);
      TravelTimeParameter.ActualValue = new DoubleValue(0);
    }

    protected override void SetResultParameters(VRPEvaluation tourEvaluation) {
      base.SetResultParameters(tourEvaluation);

      TardinessParameter.ActualValue.Value = (tourEvaluation as CVRPTWEvaluation).Tardiness;
      TravelTimeParameter.ActualValue.Value = (tourEvaluation as CVRPTWEvaluation).TravelTime;
    }

    [StorableConstructor]
    protected MDCVRPTWEvaluator(bool deserializing) : base(deserializing) { }

    public MDCVRPTWEvaluator() {
      Parameters.Add(new LookupParameter<DoubleValue>("Tardiness", "The tardiness."));
      Parameters.Add(new LookupParameter<DoubleValue>("TravelTime", "The travel time."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MDCVRPTWEvaluator(this, cloner);
    }

    protected MDCVRPTWEvaluator(MDCVRPTWEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}