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

using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("CVRPPDTWProblemInstance", "Represents a single depot CVRPPDTW instance.")]
  [StorableType("6DC3F907-9CDC-4CDA-8C84-AC9ED248DB3B")]
  public class CVRPPDTWProblemInstance : CVRPTWProblemInstance, IPickupAndDeliveryProblemInstance {
    protected IValueParameter<IntArray> PickupDeliveryLocationParameter {
      get { return (IValueParameter<IntArray>)Parameters["PickupDeliveryLocation"]; }
    }
    protected IValueParameter<DoubleValue> PickupViolationPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalPickupViolationPenalty"]; }
    }

    public IntArray PickupDeliveryLocation {
      get { return PickupDeliveryLocationParameter.Value; }
      set { PickupDeliveryLocationParameter.Value = value; }
    }

    protected IValueParameter<DoubleValue> CurrentPickupViolationPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["CurrentPickupViolationPenalty"]; }
    }

    public DoubleValue PickupViolationPenalty {
      get {
        DoubleValue currentPickupViolationPenalty = CurrentPickupViolationPenaltyParameter.Value;
        if (currentPickupViolationPenalty != null)
          return currentPickupViolationPenalty;
        else
          return PickupViolationPenaltyParameter.Value;
      }
    }
    DoubleValue IPickupAndDeliveryProblemInstance.CurrentPickupViolationPenalty {
      get { return CurrentOverloadPenaltyParameter.Value; }
      set { CurrentPickupViolationPenaltyParameter.Value = value; }
    }

    public int GetPickupDeliveryLocation(int city) {
      return PickupDeliveryLocation[city];
    }

    public override IEnumerable<IOperator> FilterOperators(IEnumerable<IOperator> operators) {
      return base.FilterOperators(operators).Where(x => x is IPickupAndDeliveryOperator);
    }

    protected override VRPEvaluation CreateTourEvaluation() {
      return new CVRPPDTWEvaluation();
    }

    protected override void EvaluateTour(VRPEvaluation eval, Tour tour, IVRPEncodedSolution solution) {
      TourInsertionInfo tourInfo = new TourInsertionInfo(solution.GetVehicleAssignment(solution.GetTourIndex(tour)));
      eval.InsertionInfo.AddTourInsertionInfo(tourInfo);
      double originalQuality = eval.Quality;

      double capacity = Capacity.Value;

      double time = 0.0;
      double waitingTime = 0.0;
      double serviceTime = 0.0;
      double tardiness = 0.0;
      double overweight = 0.0;
      double distance = 0.0;

      double currentLoad = 0.0;
      Dictionary<int, bool> stops = new Dictionary<int, bool>();
      int pickupViolations = 0;

      double tourStartTime = ReadyTime[0];
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
        double currentDistace = GetDistance(start, end, solution);
        time += currentDistace;
        distance += currentDistace;

        double arrivalTime = time;

        //check if it was serviced on time
        if (time > DueTime[end])
          tardiness += time - DueTime[end];

        //wait
        double currentWaitingTime = 0.0;
        if (time < ReadyTime[end])
          currentWaitingTime = ReadyTime[end] - time;

        double waitTime = ReadyTime[end] - time;

        waitingTime += currentWaitingTime;
        time += currentWaitingTime;

        double spareTime = DueTime[end] - time;

        //service
        double currentServiceTime = ServiceTime[end];
        serviceTime += currentServiceTime;
        time += currentServiceTime;

        //Pickup / deliver
        double arrivalSpareCapacity = capacity - currentLoad;

        bool validPickupDelivery =
          validPickupDelivery =
          ((Demand[end] >= 0) ||
           (stops.ContainsKey(PickupDeliveryLocation[end])));

        if (validPickupDelivery) {
          currentLoad += Demand[end];
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

      eval.Quality += FleetUsageFactor.Value;
      eval.Quality += DistanceFactor.Value * distance;
      eval.Distance += distance;
      eval.VehicleUtilization += 1;

      (eval as CVRPEvaluation).Overload += overweight;
      double tourPenalty = 0;
      double penalty = overweight * OverloadPenalty.Value;
      eval.Penalty += penalty;
      eval.Quality += penalty;
      tourPenalty += penalty;

      (eval as CVRPTWEvaluation).Tardiness += tardiness;
      (eval as CVRPTWEvaluation).TravelTime += time;

      penalty = tardiness * TardinessPenalty.Value;
      eval.Penalty += penalty;
      eval.Quality += penalty;
      tourPenalty += penalty;

      (eval as CVRPPDTWEvaluation).PickupViolations += pickupViolations;
      penalty = pickupViolations * PickupViolationPenalty.Value;
      eval.Penalty += penalty;
      tourPenalty += penalty;

      eval.Quality += penalty;
      eval.Quality += time * TimeFactor.Value;
      tourInfo.Penalty = tourPenalty;
      tourInfo.Quality = eval.Quality - originalQuality;

      eval.IsFeasible = overweight == 0 && tardiness == 0 && pickupViolations == 0;
    }

    protected override double GetTourInsertionCosts(IVRPEncodedSolution solution, TourInsertionInfo tourInsertionInfo, int index, int customer,
      out bool feasible) {
      CVRPPDTWInsertionInfo insertionInfo = tourInsertionInfo.GetStopInsertionInfo(index) as CVRPPDTWInsertionInfo;

      double costs = 0;
      feasible = tourInsertionInfo.Penalty < double.Epsilon;
      bool tourFeasible = true;

      double overloadPenalty = OverloadPenalty.Value;
      double tardinessPenalty = TardinessPenalty.Value;
      double pickupPenalty = PickupViolationPenalty.Value;

      double distance = GetDistance(insertionInfo.Start, insertionInfo.End, solution);
      double newDistance =
        GetDistance(insertionInfo.Start, customer, solution) +
        GetDistance(customer, insertionInfo.End, solution);
      costs += DistanceFactor.Value * (newDistance - distance);

      double demand = Demand[customer];
      if (demand > insertionInfo.ArrivalSpareCapacity) {
        tourFeasible = feasible = false;
        if (insertionInfo.ArrivalSpareCapacity >= 0)
          costs += (demand - insertionInfo.ArrivalSpareCapacity) * overloadPenalty;
        else
          costs += demand * overloadPenalty;
      }
      int destination = PickupDeliveryLocation[customer];

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

      time += GetDistance(insertionInfo.Start, customer, solution);
      if (time > DueTime[customer]) {
        tardiness += time - DueTime[customer];
      }
      if (time < ReadyTime[customer])
        time += ReadyTime[customer] - time;
      time += ServiceTime[customer];
      time += GetDistance(customer, insertionInfo.End, solution);

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

      costs += additionalTime * TimeFactor.Value;

      if (tardiness > 0) {
        tourFeasible = feasible = false;
      }

      costs += tardiness * tardinessPenalty;

      return costs;
    }

    [StorableConstructor]
    protected CVRPPDTWProblemInstance(StorableConstructorFlag _) : base(_) { }

    public CVRPPDTWProblemInstance() {
      Parameters.Add(new ValueParameter<IntArray>("PickupDeliveryLocation", "The pickup and delivery location for each customer.", new IntArray()));

      Parameters.Add(new ValueParameter<DoubleValue>("EvalPickupViolationPenalty", "The pickup violation penalty considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("CurrentPickupViolationPenalty", "The current pickup violation penalty considered in the evaluation.") { Hidden = true });

      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPPDTWProblemInstance(this, cloner);
    }

    protected CVRPPDTWProblemInstance(CVRPPDTWProblemInstance original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      PickupDeliveryLocationParameter.ValueChanged += PickupDeliveryLocationParameter_ValueChanged;
      PickupDeliveryLocation.Reset += PickupDeliveryLocation_Changed;
      PickupDeliveryLocation.ItemChanged += PickupDeliveryLocation_Changed;
    }

    public override void InitializeState() {
      base.InitializeState();

      CurrentPickupViolationPenaltyParameter.Value = null;
    }

    #region Event handlers
    void PickupDeliveryLocationParameter_ValueChanged(object sender, EventArgs e) {
      PickupDeliveryLocation.Reset += PickupDeliveryLocation_Changed;
      PickupDeliveryLocation.ItemChanged += PickupDeliveryLocation_Changed;
      EvalBestKnownSolution();
    }
    private void PickupDeliveryLocation_Changed(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    #endregion
  }
}