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
  [Item("MultiDepotVRPProblemInstance", "Represents a multi depot VRP instance.")]
  [StorableType("20788CA4-8AA0-4A57-8118-0D7C3FEF1AA4")]
  public class MultiDepotVRPProblemInstance : VRPProblemInstance, IMultiDepotProblemInstance {
    protected IValueParameter<IntValue> DepotsParameter {
      get { return (IValueParameter<IntValue>)Parameters["Depots"]; }
    }

    protected IValueParameter<IntArray> VehicleDepotAssignmentParameter {
      get { return (IValueParameter<IntArray>)Parameters["VehicleDepotAssignment"]; }
    }

    public IntValue Depots {
      get {
        return DepotsParameter.Value;
      }
    }

    public IntArray VehicleDepotAssignment {
      get {
        return VehicleDepotAssignmentParameter.Value;
      }
      set {
        VehicleDepotAssignmentParameter.Value = value;
      }
    }

    public override IEnumerable<IOperator> FilterOperators(IEnumerable<IOperator> operators) {
      return base.FilterOperators(operators)
        .Where(x => !(x is INotMultiDepotOperator))
        .Union(operators.Where(x => x is IMultiDepotOperator));
    }

    public override IntValue Cities => new IntValue(Demand.Length);


    public override double GetDemand(int city) {
      return base.GetDemand(city - 1);
    }

    public override double[] GetCoordinates(int city) {
      double[] coordinates;

      if (city == 0) {
        //calculate centroid
        coordinates = new double[Coordinates.Columns];

        for (int i = 0; i < Depots.Value; i++) {
          for (int j = 0; j < coordinates.Length; j++) {
            coordinates[j] += Coordinates[i, j];
          }
        }

        for (int j = 0; j < coordinates.Length; j++) {
          coordinates[j] /= (double)Depots.Value;
        }
      } else {
        city += Depots.Value - 1;
        coordinates = base.GetCoordinates(city);
      }

      return coordinates;
    }

    public int GetDepot(int customer, IVRPEncodedSolution solution) {
      int depot = -1;

      Tour tour =
          solution.GetTours().FirstOrDefault(t => t.Stops.Contains(customer));

      if (tour != null) {
        int tourIndex = solution.GetTourIndex(tour);
        int vehicle = solution.GetVehicleAssignment(tourIndex);
        depot = VehicleDepotAssignment[vehicle];
      }

      return depot;
    }

    public override double GetDistance(int start, int end, IVRPEncodedSolution solution) {
      if (start == 0 && end == 0)
        return 0;

      if (start == 0) {
        start = GetDepot(end, solution);
        end += Depots.Value - 1;
      } else if (end == 0) {
        end = GetDepot(start, solution);
        start += Depots.Value - 1;
      } else {
        start += Depots.Value - 1;
        end += Depots.Value - 1;
      }

      return base.GetDistance(start, end, solution);
    }

    public override double GetInsertionDistance(int start, int customer, int end, IVRPEncodedSolution solution,
      out double startDistance, out double endDistance) {
      if (start == 0) {
        start = GetDepot(end, solution);
        end += Depots.Value - 1;
      } else if (end == 0) {
        end = GetDepot(start, solution);
        start += Depots.Value - 1;
      } else {
        start += Depots.Value - 1;
        end += Depots.Value - 1;
      }
      customer += Depots.Value - 1;

      double distance = base.GetDistance(start, end, solution);

      startDistance = base.GetDistance(start, customer, solution);
      endDistance = base.GetDistance(customer, end, solution);

      double newDistance = startDistance + endDistance;

      return newDistance - distance;
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

      eval.Quality += quality;
      tourInfo.Quality = quality;
    }

    protected override double GetTourInsertionCosts(IVRPEncodedSolution solution, TourInsertionInfo tourInsertionInfo, int index, int customer,
      out bool feasible) {
      StopInsertionInfo insertionInfo = tourInsertionInfo.GetStopInsertionInfo(index);

      double costs = 0;
      feasible = true;
      double startDistance, endDistance;

      costs += GetInsertionDistance(insertionInfo.Start, customer, insertionInfo.End, solution, out startDistance, out endDistance);

      return costs;
    }

    [StorableConstructor]
    protected MultiDepotVRPProblemInstance(StorableConstructorFlag _) : base(_) { }
    protected MultiDepotVRPProblemInstance(MultiDepotVRPProblemInstance original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }
    public MultiDepotVRPProblemInstance() {
      Parameters.Add(new ValueParameter<IntValue>("Depots", "The number of depots", new IntValue(0)));
      Parameters.Add(new ValueParameter<IntArray>("VehicleDepotAssignment", "The assignment of vehicles to depots", new IntArray()));
      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiDepotVRPProblemInstance(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      DepotsParameter.ValueChanged += DepotsParameter_ValueChanged;
      Depots.ValueChanged += Depots_ValueChanged;
      VehicleDepotAssignmentParameter.ValueChanged += VehicleDepotAssignmentParameter_ValueChanged;
      VehicleDepotAssignment.Reset += VehicleDepotAssignment_Changed;
      VehicleDepotAssignment.ItemChanged += VehicleDepotAssignment_Changed;
    }

    #region Event handlers
    private void DepotsParameter_ValueChanged(object sender, EventArgs e) {
      Depots.ValueChanged += Depots_ValueChanged;
      EvalBestKnownSolution();
    }
    private void Depots_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    private void VehicleDepotAssignmentParameter_ValueChanged(object sender, EventArgs e) {
      VehicleDepotAssignment.Reset += VehicleDepotAssignment_Changed;
      VehicleDepotAssignment.ItemChanged += VehicleDepotAssignment_Changed;
      EvalBestKnownSolution();
    }
    private void VehicleDepotAssignment_Changed(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    #endregion
  }
}