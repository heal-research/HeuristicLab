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

using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("SingleDepotVRPProblemInstance", "Represents a single depot VRP instance.")]
  [StorableType("A45435DD-F615-45C6-8456-5A49EE5D3C8E")]
  public class SingleDepotVRPProblemInstance : VRPProblemInstance, ISingleDepotProblemInstance {

    public override IEnumerable<IOperator> FilterOperators(IEnumerable<IOperator> operators) {
      return base.FilterOperators(operators).Where(x => x is ISingleDepotOperator);
    }

    public override IntValue Cities {
      get {
        return new IntValue(Demand.Length - 1);
      }
    }
    protected override void EvaluateTour(VRPEvaluation eval, Tour tour, IVRPEncodedSolution solution) {
      TourInsertionInfo tourInfo = new TourInsertionInfo(solution.GetVehicleAssignment(solution.GetTourIndex(tour)));
      eval.InsertionInfo.AddTourInsertionInfo(tourInfo);

      double distance = 0.0;
      double quality = 0.0;

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

        StopInsertionInfo stopInfo = new StopInsertionInfo(start, end);
        tourInfo.AddStopInsertionInfo(stopInfo);
      }

      //Fleet usage
      quality += FleetUsageFactor.Value;
      //Distance
      quality += DistanceFactor.Value * distance;

      eval.Distance += distance;
      eval.VehicleUtilization += 1;

      tourInfo.Quality = quality;
      eval.Quality += quality;
    }

    protected override double GetTourInsertionCosts(IVRPEncodedSolution solution, TourInsertionInfo tourInsertionInfo, int index, int customer,
      out bool feasible) {
      StopInsertionInfo insertionInfo = tourInsertionInfo.GetStopInsertionInfo(index);

      double costs = 0;
      feasible = true;

      double distance = GetDistance(insertionInfo.Start, insertionInfo.End, solution);
      double newDistance =
        GetDistance(insertionInfo.Start, customer, solution) +
        GetDistance(customer, insertionInfo.End, solution);

      costs += DistanceFactor.Value * (newDistance - distance);

      return costs;
    }


    [StorableConstructor]
    protected SingleDepotVRPProblemInstance(StorableConstructorFlag _) : base(_) { }

    public SingleDepotVRPProblemInstance() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleDepotVRPProblemInstance(this, cloner);
    }

    protected SingleDepotVRPProblemInstance(SingleDepotVRPProblemInstance original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}