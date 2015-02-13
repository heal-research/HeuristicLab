#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.IO;
using HeuristicLab.Data;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using System;

namespace HeuristicLab.Problems.VehicleRouting.Interpreters {
  public abstract class VRPInterpreter : IVRPDataInterpreter<IVRPData> {
    protected abstract void Interpret(IVRPData data, IVRPProblemInstance problemInstance);

    protected virtual IVRPEncoding GetBestKnowTour(IVRPData data, IVRPProblemInstance problemInstance) {
      if (data.BestKnownTour != null) {
        PotvinEncoding solution = new PotvinEncoding(problemInstance);

        for (int i = 0; i < data.BestKnownTour.GetLength(0); i++) {
          Tour tour = new Tour();
          solution.Tours.Add(tour);

          foreach (int stop in data.BestKnownTour[i]) {
            tour.Stops.Add(stop + 1);
          }
        }

        if (data.BestKnownTourVehicleAssignment != null) {
          if (data.BestKnownTourVehicleAssignment.Length != solution.VehicleAssignment.Length)
            throw new InvalidDataException("Number of vehicles of the best known tour does not match the number vehicles of the instance.");
          for (int i = 0; i < data.BestKnownTourVehicleAssignment.Length; i++)
            solution.VehicleAssignment[i] = data.BestKnownTourVehicleAssignment[i];
        }

        return solution;
      } else {
        return null;
      }
    }

    protected abstract IVRPProblemInstance CreateProblemInstance();

    public VRPInstanceDescription Interpret(IVRPData data) {
      VRPInstanceDescription result = new VRPInstanceDescription();
      result.Name = data.Name;
      result.Description = data.Description;

      IVRPProblemInstance problem = CreateProblemInstance();
      Interpret(data, problem);
      result.ProblemInstance = problem;

      result.BestKnownQuality = data.BestKnownQuality;
      result.BestKnownSolution = GetBestKnowTour(data, problem);

      return result;
    }
  }
}
