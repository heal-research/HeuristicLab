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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("MDCVRPEvaluator", "Represents a multi depot CVRP evaluator.")]
  [StorableClass]
  public class MDCVRPEvaluator : VRPEvaluator {
    public ILookupParameter<DoubleValue> OverloadParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Overload"]; }
    }

    protected override VRPEvaluation CreateTourEvaluation() {
      return new CVRPEvaluation();
    }

    protected override void EvaluateTour(VRPEvaluation eval, IVRPProblemInstance instance, Tour tour, IVRPEncoding solution) {
      TourInsertionInfo tourInfo = new TourInsertionInfo(solution.GetVehicleAssignment(solution.GetTourIndex(tour)));
      eval.InsertionInfo.AddTourInsertionInfo(tourInfo);
      double originalQuality = eval.Quality;

      IHeterogenousCapacitatedProblemInstance cvrpInstance = instance as IHeterogenousCapacitatedProblemInstance;

      double delivered = 0.0;
      double overweight = 0.0;
      double distance = 0.0;

      int tourIndex = solution.GetTourIndex(tour);
      int vehicle = solution.GetVehicleAssignment(tourIndex);

      double capacity = cvrpInstance.Capacity[vehicle];
      for (int i = 0; i < tour.Stops.Count; i++) {
        delivered += instance.GetDemand(tour.Stops[i]);
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
        double currentDistace = instance.GetDistance(start, end, solution);
        distance += currentDistace;

        CVRPInsertionInfo stopInfo = new CVRPInsertionInfo(start, end, spareCapacity);
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
      double penalty = overweight * cvrpInstance.OverloadPenalty.Value;
      eval.Penalty += penalty;
      eval.Quality += penalty;
      tourInfo.Penalty = penalty;
      tourInfo.Quality = eval.Quality - originalQuality;
    }

    protected override double GetTourInsertionCosts(IVRPProblemInstance instance, IVRPEncoding solution, TourInsertionInfo tourInsertionInfo, int index, int customer,
      out bool feasible) {
      CVRPInsertionInfo insertionInfo = tourInsertionInfo.GetStopInsertionInfo(index) as CVRPInsertionInfo;

      double costs = 0;
      feasible = tourInsertionInfo.Penalty < double.Epsilon;

      IHeterogenousCapacitatedProblemInstance cvrp = instance as IHeterogenousCapacitatedProblemInstance;
      double overloadPenalty = cvrp.OverloadPenalty.Value;

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

      return costs;
    }

    protected override void InitResultParameters() {
      base.InitResultParameters();

      OverloadParameter.ActualValue = new DoubleValue(0);
    }

    protected override void SetResultParameters(VRPEvaluation tourEvaluation) {
      base.SetResultParameters(tourEvaluation);

      OverloadParameter.ActualValue.Value = (tourEvaluation as CVRPEvaluation).Overload;
    }

    [StorableConstructor]
    protected MDCVRPEvaluator(bool deserializing) : base(deserializing) { }

    public MDCVRPEvaluator() {
      Parameters.Add(new LookupParameter<DoubleValue>("Overload", "The overload."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MDCVRPEvaluator(this, cloner);
    }

    protected MDCVRPEvaluator(MDCVRPEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}