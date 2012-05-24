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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.VehicleRouting.Variants;
using HeuristicLab.Problems.VehicleRouting.Encodings;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("CVRPEvaluator", "Represents a single depot CVRP evaluator.")]
  [StorableClass]
  public class CVRPEvaluator: VRPEvaluator {
    public ILookupParameter<DoubleValue> OverloadParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Overload"]; }
    }

    protected override VRPEvaluation CreateTourEvaluation() {
      return new CVRPEvaluation();
    }

    protected override void EvaluateTour(VRPEvaluation eval, IVRPProblemInstance instance, Tour tour, IVRPEncoding solution) {
      TourInsertionInfo tourInfo = new TourInsertionInfo(solution.GetVehicleAssignment(solution.GetTourIndex(tour))); ;
      eval.InsertionInfo.AddTourInsertionInfo(tourInfo);
      double originalQuality = eval.Quality;
      
      IHomogenousCapacitatedProblemInstance cvrpInstance = instance as IHomogenousCapacitatedProblemInstance;
      DoubleArray demand = instance.Demand;

      double delivered = 0.0;
      double overweight = 0.0;
      double distance = 0.0;

      double capacity = cvrpInstance.Capacity.Value;
      for (int i = 0; i <= tour.Stops.Count; i++) {
        int end = 0;
        if (i < tour.Stops.Count)
          end = tour.Stops[i];

        delivered += demand[end];
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

      ICapacitatedProblemInstance cvrp = instance as ICapacitatedProblemInstance;
      double overloadPenalty = cvrp.OverloadPenalty.Value;

      double distance = instance.GetDistance(insertionInfo.Start, insertionInfo.End, solution);
      double newDistance =
        instance.GetDistance(insertionInfo.Start, customer, solution) +
        instance.GetDistance(customer, insertionInfo.End, solution);
      costs += instance.DistanceFactor.Value * (newDistance - distance);

      double demand = instance.Demand[customer];
      if (demand > insertionInfo.SpareCapacity) {
        feasible = false;

        if(insertionInfo.SpareCapacity >= 0)
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
    protected CVRPEvaluator(bool deserializing) : base(deserializing) { }

    public CVRPEvaluator() {
      Parameters.Add(new LookupParameter<DoubleValue>("Overload", "The overload."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPEvaluator(this, cloner);
    }

    protected CVRPEvaluator(CVRPEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}