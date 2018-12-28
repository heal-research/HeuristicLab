#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("CVRPPDTWEvaluator", "Represents a single depot CVRPPDTW evaluator.")]
  [StorableType("6324F32C-727E-4593-BB21-4625755F844A")]
  public class CVRPPDTWEvaluator : CVRPTWEvaluator {
    public ILookupParameter<IntValue> PickupViolationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["PickupViolations"]; }
    }

    protected override VRPEvaluation CreateTourEvaluation() {
      return new CVRPPDTWEvaluation();
    }

    protected override void EvaluateTour(VRPEvaluation eval, IVRPProblemInstance instance, Tour tour, IVRPEncoding solution) {
      TourInsertionInfo tourInfo = new TourInsertionInfo(solution.GetVehicleAssignment(solution.GetTourIndex(tour)));
      eval.InsertionInfo.AddTourInsertionInfo(tourInfo);
      double originalQuality = eval.Quality;

      IHomogenousCapacitatedProblemInstance cvrpInstance = instance as IHomogenousCapacitatedProblemInstance;
      DoubleArray demand = instance.Demand;

      ITimeWindowedProblemInstance vrptw = instance as ITimeWindowedProblemInstance;
      DoubleArray dueTime = vrptw.DueTime;
      DoubleArray readyTime = vrptw.ReadyTime;
      DoubleArray serviceTimes = vrptw.ServiceTime;

      IPickupAndDeliveryProblemInstance pdp = instance as IPickupAndDeliveryProblemInstance;
      IntArray pickupDeliveryLocation = pdp.PickupDeliveryLocation;

      double capacity = cvrpInstance.Capacity.Value;

      double time = 0.0;
      double waitingTime = 0.0;
      double serviceTime = 0.0;
      double tardiness = 0.0;
      double overweight = 0.0;
      double distance = 0.0;

      double currentLoad = 0.0;
      Dictionary<int, bool> stops = new Dictionary<int, bool>();
      int pickupViolations = 0;

      double tourStartTime = readyTime[0];
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

        //check if it was serviced on time
        if (time > dueTime[end])
          tardiness += time - dueTime[end];

        //wait
        double currentWaitingTime = 0.0;
        if (time < readyTime[end])
          currentWaitingTime = readyTime[end] - time;

        double waitTime = readyTime[end] - time;

        waitingTime += currentWaitingTime;
        time += currentWaitingTime;

        double spareTime = dueTime[end] - time;

        //service
        double currentServiceTime = serviceTimes[end];
        serviceTime += currentServiceTime;
        time += currentServiceTime;

        //Pickup / deliver
        double arrivalSpareCapacity = capacity - currentLoad;

        bool validPickupDelivery =
          validPickupDelivery =
          ((demand[end] >= 0) ||
           (stops.ContainsKey(pickupDeliveryLocation[end])));

        if (validPickupDelivery) {
          currentLoad += demand[end];
        } else {
          pickupViolations++;
        }

        if (currentLoad > capacity)
          overweight += currentLoad - capacity;

        double spareCapacity = capacity - currentLoad;
        CVRPPDTWInsertionInfo stopInfo = new CVRPPDTWInsertionInfo(start, end, spareCapacity, tourStartTime,
          arrivalTime, time, spareTime, waitTime, new List<int>(stops.Keys), arrivalSpareCapacity);
        tourInfo.AddStopInsertionInfo(stopInfo);

        stops.Add(end, true);
      }

      eval.Quality += instance.FleetUsageFactor.Value;
      eval.Quality += instance.DistanceFactor.Value * distance;
      eval.Distance += distance;
      eval.VehicleUtilization += 1;

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

      (eval as CVRPPDTWEvaluation).PickupViolations += pickupViolations;
      penalty = pickupViolations * pdp.PickupViolationPenalty.Value;
      eval.Penalty += penalty;
      tourPenalty += penalty;

      eval.Quality += penalty;
      eval.Quality += time * vrptw.TimeFactor.Value;
      tourInfo.Penalty = tourPenalty;
      tourInfo.Quality = eval.Quality - originalQuality;
    }

    protected override double GetTourInsertionCosts(IVRPProblemInstance instance, IVRPEncoding solution, TourInsertionInfo tourInsertionInfo, int index, int customer,
      out bool feasible) {
      CVRPPDTWInsertionInfo insertionInfo = tourInsertionInfo.GetStopInsertionInfo(index) as CVRPPDTWInsertionInfo;

      double costs = 0;
      feasible = tourInsertionInfo.Penalty < double.Epsilon;
      bool tourFeasible = true;

      ICapacitatedProblemInstance cvrp = instance as ICapacitatedProblemInstance;
      double overloadPenalty = cvrp.OverloadPenalty.Value;

      ITimeWindowedProblemInstance vrptw = instance as ITimeWindowedProblemInstance;
      DoubleArray dueTime = vrptw.DueTime;
      DoubleArray readyTime = vrptw.ReadyTime;
      DoubleArray serviceTimes = vrptw.ServiceTime;
      double tardinessPenalty = vrptw.TardinessPenalty.Value;

      IPickupAndDeliveryProblemInstance pdp = instance as IPickupAndDeliveryProblemInstance;
      IntArray pickupDeliveryLocation = pdp.PickupDeliveryLocation;
      double pickupPenalty = pdp.PickupViolationPenalty.Value;

      double distance = instance.GetDistance(insertionInfo.Start, insertionInfo.End, solution);
      double newDistance =
        instance.GetDistance(insertionInfo.Start, customer, solution) +
        instance.GetDistance(customer, insertionInfo.End, solution);
      costs += instance.DistanceFactor.Value * (newDistance - distance);

      double demand = instance.Demand[customer];
      if (demand > insertionInfo.ArrivalSpareCapacity) {
        tourFeasible = feasible = false;
        if (insertionInfo.ArrivalSpareCapacity >= 0)
          costs += (demand - insertionInfo.ArrivalSpareCapacity) * overloadPenalty;
        else
          costs += demand * overloadPenalty;
      }
      int destination = pickupDeliveryLocation[customer];

      bool validPickup = true;
      if (demand < 0 && !insertionInfo.Visited.Contains(destination)) {
        tourFeasible = feasible = false;
        validPickup = false;
        costs += pickupPenalty;
      }

      double time = 0;
      double tardiness = 0;

      if (index > 0)
        time = (tourInsertionInfo.GetStopInsertionInfo(index - 1) as CVRPTWInsertionInfo).LeaveTime;
      else
        time = insertionInfo.TourStartTime;

      time += instance.GetDistance(insertionInfo.Start, customer, solution);
      if (time > dueTime[customer]) {
        tardiness += time - dueTime[customer];
      }
      if (time < readyTime[customer])
        time += readyTime[customer] - time;
      time += serviceTimes[customer];
      time += instance.GetDistance(customer, insertionInfo.End, solution);

      double additionalTime = time - (tourInsertionInfo.GetStopInsertionInfo(index) as CVRPTWInsertionInfo).ArrivalTime;
      for (int i = index; i < tourInsertionInfo.GetStopCount(); i++) {
        CVRPTWInsertionInfo nextStop = tourInsertionInfo.GetStopInsertionInfo(i) as CVRPTWInsertionInfo;

        if (demand >= 0) {
          if (nextStop.End == destination) {
            demand = 0;
            costs -= pickupPenalty;
            if (tourInsertionInfo.Penalty == pickupPenalty && tourFeasible)
              feasible = true;
          } else if (nextStop.SpareCapacity < 0) {
            costs += demand * overloadPenalty;
          } else if (nextStop.SpareCapacity < demand) {
            tourFeasible = feasible = false;
            costs += (demand - nextStop.SpareCapacity) * overloadPenalty;
          }
        } else if (validPickup) {
          if (nextStop.SpareCapacity < 0) {
            costs += Math.Max(demand, nextStop.SpareCapacity) * overloadPenalty;
          }
        }

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
        tourFeasible = feasible = false;
      }

      costs += tardiness * tardinessPenalty;

      return costs;
    }

    protected override void InitResultParameters() {
      base.InitResultParameters();

      PickupViolationsParameter.ActualValue = new IntValue(0);
    }

    protected override void SetResultParameters(VRPEvaluation tourEvaluation) {
      base.SetResultParameters(tourEvaluation);

      PickupViolationsParameter.ActualValue.Value = (tourEvaluation as CVRPPDTWEvaluation).PickupViolations;
    }

    [StorableConstructor]
    protected CVRPPDTWEvaluator(StorableConstructorFlag _) : base(_) { }

    public CVRPPDTWEvaluator() {
      Parameters.Add(new LookupParameter<IntValue>("PickupViolations", "The number of pickup violations."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPPDTWEvaluator(this, cloner);
    }

    protected CVRPPDTWEvaluator(CVRPPDTWEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}