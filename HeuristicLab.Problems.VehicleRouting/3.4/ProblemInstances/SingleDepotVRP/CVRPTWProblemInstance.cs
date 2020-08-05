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

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("CVRPTWProblemInstance", "Represents a single depot CVRPTW instance.")]
  [StorableType("2138BE6C-058E-4003-8578-55E8DE3706C7")]
  public class CVRPTWProblemInstance : CVRPProblemInstance, ITimeWindowedProblemInstance {
    protected IValueParameter<DoubleArray> ReadyTimeParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["ReadyTime"]; }
    }
    protected IValueParameter<DoubleArray> DueTimeParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["DueTime"]; }
    }
    protected IValueParameter<DoubleArray> ServiceTimeParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["ServiceTime"]; }
    }

    protected IValueParameter<DoubleValue> TimeFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalTimeFactor"]; }
    }
    protected IValueParameter<DoubleValue> TardinessPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalTardinessPenalty"]; }
    }

    public DoubleArray ReadyTime {
      get { return ReadyTimeParameter.Value; }
      set { ReadyTimeParameter.Value = value; }
    }
    public DoubleArray DueTime {
      get { return DueTimeParameter.Value; }
      set { DueTimeParameter.Value = value; }
    }
    public DoubleArray ServiceTime {
      get { return ServiceTimeParameter.Value; }
      set { ServiceTimeParameter.Value = value; }
    }
    public DoubleValue TimeFactor {
      get { return TimeFactorParameter.Value; }
      set { TimeFactorParameter.Value = value; }
    }

    protected IValueParameter<DoubleValue> CurrentTardinessPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["CurrentTardinessPenalty"]; }
    }

    public DoubleValue TardinessPenalty {
      get {
        DoubleValue currentTardinessPenalty = CurrentTardinessPenaltyParameter.Value;
        if (currentTardinessPenalty != null)
          return currentTardinessPenalty;
        else
          return TardinessPenaltyParameter.Value;
      }
    }
    DoubleValue ITimeWindowedProblemInstance.CurrentTardinessPenalty {
      get { return CurrentTardinessPenaltyParameter.Value; }
      set { CurrentTardinessPenaltyParameter.Value = value; }
    }

    public override IEnumerable<IOperator> FilterOperators(IEnumerable<IOperator> operators) {
      return base.FilterOperators(operators)
        .Where(x => !(x is INotTimeWindowedOperator))
        .Union(operators.Where(x => x is ITimeWindowedOperator));
    }

    protected override VRPEvaluation CreateTourEvaluation() {
      return new CVRPTWEvaluation();
    }

    protected override void EvaluateTour(VRPEvaluation eval, Tour tour, IVRPEncodedSolution solution) {
      TourInsertionInfo tourInfo = new TourInsertionInfo(solution.GetVehicleAssignment(solution.GetTourIndex(tour)));
      eval.InsertionInfo.AddTourInsertionInfo(tourInfo);
      double originalQuality = eval.Quality;

      double time = 0.0;
      double waitingTime = 0.0;
      double serviceTime = 0.0;
      double tardiness = 0.0;
      double delivered = 0.0;
      double overweight = 0.0;
      double distance = 0.0;

      double capacity = Capacity.Value;
      for (int i = 0; i <= tour.Stops.Count; i++) {
        int end = 0;
        if (i < tour.Stops.Count)
          end = tour.Stops[i];

        delivered += Demand[end];
      }

      double spareCapacity = capacity - delivered;

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
        if (time > DueTime[end]) {
          tardiness += time - DueTime[end];
        }

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

        CVRPTWInsertionInfo stopInfo = new CVRPTWInsertionInfo(start, end, spareCapacity, tourStartTime, arrivalTime, time, spareTime, waitTime);
        tourInfo.AddStopInsertionInfo(stopInfo);
      }

      eval.Quality += FleetUsageFactor.Value;
      eval.Quality += DistanceFactor.Value * distance;
      eval.Distance += distance;
      eval.VehicleUtilization += 1;

      if (delivered > capacity) {
        overweight = delivered - capacity;
      }

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
      eval.Quality += time * TimeFactor.Value;
      tourInfo.Penalty = tourPenalty;
      tourInfo.Quality = eval.Quality - originalQuality;

      eval.IsFeasible = overweight == 0 && tardiness == 0;
    }

    protected override double GetTourInsertionCosts(IVRPEncodedSolution solution, TourInsertionInfo tourInsertionInfo, int index, int customer,
      out bool feasible) {
      CVRPTWInsertionInfo insertionInfo = tourInsertionInfo.GetStopInsertionInfo(index) as CVRPTWInsertionInfo;

      double costs = 0;
      feasible = tourInsertionInfo.Penalty < double.Epsilon;

      double overloadPenalty = OverloadPenalty.Value;
      double tardinessPenalty = TardinessPenalty.Value;

      double distance = GetDistance(insertionInfo.Start, insertionInfo.End, solution);
      double newDistance =
        GetDistance(insertionInfo.Start, customer, solution) +
        GetDistance(customer, insertionInfo.End, solution);
      costs += DistanceFactor.Value * (newDistance - distance);

      double demand = Demand[customer];
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
        feasible = false;
      }

      costs += tardiness * tardinessPenalty;

      return costs;
    }

    [StorableConstructor]
    protected CVRPTWProblemInstance(StorableConstructorFlag _) : base(_) { }

    public CVRPTWProblemInstance() {
      Parameters.Add(new ValueParameter<DoubleArray>("ReadyTime", "The ready time of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("DueTime", "The due time of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("ServiceTime", "The service time of each customer.", new DoubleArray()));

      Parameters.Add(new ValueParameter<DoubleValue>("EvalTimeFactor", "The time factor considered in the evaluation.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalTardinessPenalty", "The tardiness penalty considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("CurrentTardinessPenalty", "The current tardiness penalty considered in the evaluation.") { Hidden = true });

      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPTWProblemInstance(this, cloner);
    }

    protected CVRPTWProblemInstance(CVRPTWProblemInstance original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      ReadyTimeParameter.ValueChanged += ReadyTimeParameter_ValueChanged;
      ReadyTime.Reset += ReadyTime_Changed;
      ReadyTime.ItemChanged += ReadyTime_Changed;
      DueTimeParameter.ValueChanged += DueTimeParameter_ValueChanged;
      DueTime.Reset += DueTime_Changed;
      DueTime.ItemChanged += DueTime_Changed;
      ServiceTimeParameter.ValueChanged += ServiceTimeParameter_ValueChanged;
      ServiceTime.Reset += ServiceTime_Changed;
      ServiceTime.ItemChanged += ServiceTime_Changed;
      TardinessPenaltyParameter.ValueChanged += TardinessPenaltyParameter_ValueChanged;
      TardinessPenalty.ValueChanged += TardinessPenalty_ValueChanged;
      TimeFactorParameter.ValueChanged += TimeFactorParameter_ValueChanged;
      TimeFactor.ValueChanged += TimeFactor_ValueChanged;
    }

    public override void InitializeState() {
      base.InitializeState();

      CurrentTardinessPenaltyParameter.Value = null;
    }

    #region Event handlers
    private void ReadyTimeParameter_ValueChanged(object sender, EventArgs e) {
      ReadyTime.Reset += ReadyTime_Changed;
      ReadyTime.ItemChanged += ReadyTime_Changed;
      EvalBestKnownSolution();
    }
    private void ReadyTime_Changed(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    private void DueTimeParameter_ValueChanged(object sender, EventArgs e) {
      DueTime.Reset += DueTime_Changed;
      DueTime.ItemChanged += DueTime_Changed;
      EvalBestKnownSolution();
    }
    private void DueTime_Changed(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    private void ServiceTimeParameter_ValueChanged(object sender, EventArgs e) {
      ServiceTime.Reset += ServiceTime_Changed;
      ServiceTime.ItemChanged += ServiceTime_Changed;
      EvalBestKnownSolution();
    }
    private void ServiceTime_Changed(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    private void TardinessPenaltyParameter_ValueChanged(object sender, EventArgs e) {
      TardinessPenaltyParameter.Value.ValueChanged += TardinessPenalty_ValueChanged;
      EvalBestKnownSolution();
    }
    private void TardinessPenalty_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    private void TimeFactorParameter_ValueChanged(object sender, EventArgs e) {
      TimeFactorParameter.Value.ValueChanged += TimeFactor_ValueChanged;
      EvalBestKnownSolution();
    }
    private void TimeFactor_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    #endregion
  }
}