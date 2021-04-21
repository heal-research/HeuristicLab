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
  [Item("CVRPProblemInstance", "Represents a single depot CVRP instance.")]
  [StorableType("CBE1D39B-9BBE-4119-801B-32739D1E8DEE")]
  public class CVRPProblemInstance : SingleDepotVRPProblemInstance, IHomogenousCapacitatedProblemInstance {
    protected IValueParameter<DoubleValue> CapacityParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Capacity"]; }
    }
    protected IValueParameter<DoubleValue> OverloadPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalOverloadPenalty"]; }
    }

    public DoubleValue Capacity {
      get { return CapacityParameter.Value; }
      set { CapacityParameter.Value = value; }
    }

    protected IValueParameter<DoubleValue> CurrentOverloadPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["CurrentOverloadPenalty"]; }
    }

    public DoubleValue OverloadPenalty {
      get {
        DoubleValue currentOverloadPenalty = CurrentOverloadPenaltyParameter.Value;
        if (currentOverloadPenalty != null)
          return currentOverloadPenalty;
        else
          return OverloadPenaltyParameter.Value;
      }
    }
    DoubleValue ICapacitatedProblemInstance.CurrentOverloadPenalty {
      get { return CurrentOverloadPenaltyParameter.Value; }
      set { CurrentOverloadPenaltyParameter.Value = value; }
    }

    public override IEnumerable<IOperator> FilterOperators(IEnumerable<IOperator> operators) {
      return base.FilterOperators(operators)
        .Where(x => !(x is INotCapacitatedOperator))
        .Union(operators.Where(x => x is IHomogenousCapacitatedOperator
                                 || x is ICapacitatedOperator && !(x is IHeterogenousCapacitatedOperator)));
    }
    protected override VRPEvaluation CreateTourEvaluation() {
      return new CVRPEvaluation();
    }

    protected override void EvaluateTour(VRPEvaluation eval, Tour tour, IVRPEncodedSolution solution) {
      TourInsertionInfo tourInfo = new TourInsertionInfo(solution.GetVehicleAssignment(solution.GetTourIndex(tour))); ;
      eval.InsertionInfo.AddTourInsertionInfo(tourInfo);
      double originalQuality = eval.Quality;

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
        distance += currentDistace;

        CVRPInsertionInfo stopInfo = new CVRPInsertionInfo(start, end, spareCapacity);
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
      double penalty = overweight * OverloadPenalty.Value;
      eval.Penalty += penalty;
      eval.Quality += penalty;
      tourInfo.Penalty = penalty;
      tourInfo.Quality = eval.Quality - originalQuality;

      eval.IsFeasible = overweight == 0;
    }

    protected override double GetTourInsertionCosts(IVRPEncodedSolution solution, TourInsertionInfo tourInsertionInfo, int index, int customer,
      out bool feasible) {
      CVRPInsertionInfo insertionInfo = tourInsertionInfo.GetStopInsertionInfo(index) as CVRPInsertionInfo;

      double costs = 0;
      feasible = tourInsertionInfo.Penalty < double.Epsilon;

      double overloadPenalty = OverloadPenalty.Value;

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

      return costs;
    }

    [StorableConstructor]
    protected CVRPProblemInstance(StorableConstructorFlag _) : base(_) { }

    public CVRPProblemInstance() {
      Parameters.Add(new ValueParameter<DoubleValue>("Capacity", "The capacity of each vehicle.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalOverloadPenalty", "The overload penalty considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("CurrentOverloadPenalty", "The current overload penalty considered in the evaluation.") { Hidden = true });

      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPProblemInstance(this, cloner);
    }

    protected CVRPProblemInstance(CVRPProblemInstance original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      CapacityParameter.ValueChanged += new EventHandler(CapacityParameter_ValueChanged);
      CapacityParameter.Value.ValueChanged += new EventHandler(Capacity_ValueChanged);
      OverloadPenaltyParameter.ValueChanged += new EventHandler(OverloadPenaltyParameter_ValueChanged);
      OverloadPenaltyParameter.Value.ValueChanged += new EventHandler(OverloadPenalty_ValueChanged);
    }

    public override void InitializeState() {
      base.InitializeState();

      CurrentOverloadPenaltyParameter.Value = null;
    }

    #region Event handlers
    void CapacityParameter_ValueChanged(object sender, EventArgs e) {
      CapacityParameter.Value.ValueChanged += new EventHandler(Capacity_ValueChanged);
      EvalBestKnownSolution();
    }
    void Capacity_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void OverloadPenaltyParameter_ValueChanged(object sender, EventArgs e) {
      OverloadPenaltyParameter.Value.ValueChanged += new EventHandler(OverloadPenalty_ValueChanged);
      EvalBestKnownSolution();
    }
    void OverloadPenalty_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    #endregion
  }
}