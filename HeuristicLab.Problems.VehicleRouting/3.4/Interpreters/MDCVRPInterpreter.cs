#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Data;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using System;

namespace HeuristicLab.Problems.VehicleRouting.Interpreters {
  public class MDCVRPInterpreter : VRPInterpreter, IVRPDataInterpreter<MDCVRPData> {   
    protected override IVRPProblemInstance CreateProblemInstance() {
      return new MDCVRPProblemInstance();
    }

    protected override void Interpret(IVRPData data, IVRPProblemInstance problemInstance) {
      MDCVRPData cvrpData = (MDCVRPData)data;
      MDCVRPProblemInstance problem = (MDCVRPProblemInstance)problemInstance;

      if (cvrpData.Coordinates != null)
        problem.Coordinates = new DoubleMatrix(cvrpData.Coordinates);
      if (cvrpData.MaximumVehicles != null)
        problem.Vehicles.Value = (int)cvrpData.MaximumVehicles;
      else
        problem.Vehicles.Value = cvrpData.Dimension - 1;
      problem.Capacity = new DoubleArray(cvrpData.Capacity);
      problem.Demand = new DoubleArray(cvrpData.Demands);
      if (cvrpData.DistanceMeasure != DistanceMeasure.Euclidean) {
        problem.UseDistanceMatrix.Value = true;
        problem.DistanceMatrix = new DoubleMatrix(cvrpData.GetDistanceMatrix());
      }

      problem.Depots.Value = cvrpData.Depots;
      problem.VehicleDepotAssignment = new IntArray(cvrpData.VehicleDepotAssignment);
    }
  }
}
