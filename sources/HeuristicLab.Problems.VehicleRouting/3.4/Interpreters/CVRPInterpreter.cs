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
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Data;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;

namespace HeuristicLab.Problems.VehicleRouting.Interpreters {
  public class CVRPInterpreter: IVRPDataInterpreter<CVRPData> {
    public VRPInstanceDescription Interpret(IVRPData data) {
      CVRPData cvrpData = data as CVRPData;

      VRPInstanceDescription result = new VRPInstanceDescription();
      result.Name = cvrpData.Name;
      result.Description = cvrpData.Description;

      CVRPProblemInstance problem = new CVRPProblemInstance();
      if(cvrpData.Coordinates != null)
        problem.Coordinates = new DoubleMatrix(cvrpData.Coordinates);
      if (cvrpData.MaximumVehicles != null)
        problem.Vehicles.Value = (int)cvrpData.MaximumVehicles;
      else
        problem.Vehicles.Value = cvrpData.Dimension - 1;
      problem.Capacity.Value = cvrpData.Capacity;
      problem.Demand = new DoubleArray(cvrpData.Demands);
      if (cvrpData.DistanceMeasure != DistanceMeasure.Euclidean) {
        problem.UseDistanceMatrix.Value = true;
        problem.DistanceMatrix = new DoubleMatrix(cvrpData.GetDistanceMatrix());
      }
      result.ProblemInstance = problem;

      result.BestKnownQuality = cvrpData.BestKnownQuality;
      if (cvrpData.BestKnownTour != null) {
        PotvinEncoding solution = new PotvinEncoding(problem);

        for(int i = 0; i < cvrpData.BestKnownTour.GetLength(0); i++) {
          Tour tour = new Tour();
          solution.Tours.Add(tour);

          foreach (int stop in cvrpData.BestKnownTour[i]) {
            tour.Stops.Add(stop + 1);
          }
        }
        
        result.BestKnownSolution = solution;
      }

      return result;
    }
  }
}
