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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;


namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("SingleDepotVRPEvaluator", "Represents a single depot VRP evaluator.")]
  [StorableType("AFC33483-2E8A-4469-8216-1F4FEE0AEB51")]
  public class MultiDepotVRPEvaluator : VRPEvaluator {
    protected override void EvaluateTour(VRPEvaluation eval, IVRPProblemInstance instance, Tour tour, IVRPEncoding solution) {
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
        double currentDistace = instance.GetDistance(start, end, solution);
        distance += currentDistace;

        StopInsertionInfo stopInfo = new StopInsertionInfo(start, end);
        tourInfo.AddStopInsertionInfo(stopInfo);
      }

      //Fleet usage
      quality += instance.FleetUsageFactor.Value;
      //Distance
      quality += instance.DistanceFactor.Value * distance;

      eval.Distance += distance;
      eval.VehicleUtilization += 1;

      eval.Quality += quality;
      tourInfo.Quality = quality;
    }

    protected override double GetTourInsertionCosts(IVRPProblemInstance instance, IVRPEncoding solution, TourInsertionInfo tourInsertionInfo, int index, int customer,
      out bool feasible) {
      StopInsertionInfo insertionInfo = tourInsertionInfo.GetStopInsertionInfo(index);

      double costs = 0;
      feasible = true;
      double startDistance, endDistance;

      costs += instance.GetInsertionDistance(insertionInfo.Start, customer, insertionInfo.End, solution, out startDistance, out endDistance);

      return costs;
    }

    [StorableConstructor]
    protected MultiDepotVRPEvaluator(StorableConstructorFlag _) : base(_) { }

    public MultiDepotVRPEvaluator() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiDepotVRPEvaluator(this, cloner);
    }

    protected MultiDepotVRPEvaluator(MultiDepotVRPEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}