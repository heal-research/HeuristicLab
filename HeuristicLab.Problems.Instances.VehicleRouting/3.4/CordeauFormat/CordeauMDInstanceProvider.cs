#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  public class CordeauMDInstanceProvider : CordeauFormatInstanceProvider<MDCVRPData> {
    public override string Name {
      get { return "Cordeau (MDCVRP)"; }
    }

    public override string Description {
      get { return "Cordeau test set"; }
    }

    public override Uri WebLink {
      get { return new Uri(@"http://neo.lcc.uma.es/vrp/vrp-instances/multiple-depot-vrp-instances/"); }
    }

    public override string ReferencePublication {
      get {
        return @"J.-F. Cordeau, M. Gendreau, G. Laporte. 1997.
A tabu search heuristic for periodic and multi-depot vehicle routing problems.
Networks, 30, pp. 105–119.";
      }
    }

    protected override string FileName {
      get { return "CordeauMD"; }
    }

    internal override MDCVRPData LoadInstance(CordeauParser parser) {
      parser.Parse();

      var instance = new MDCVRPData();
      instance.Dimension = parser.Cities + 1;
      instance.Depots = parser.Depots;
      instance.Coordinates = parser.Coordinates;
      instance.Capacity = parser.Capacity;
      instance.Demands = parser.Demands;
      instance.DistanceMeasure = DistanceMeasure.Euclidean;
      instance.MaximumVehicles = parser.Vehicles;

      int depots = parser.Depots;
      int vehicles = parser.Vehicles / parser.Depots;
      instance.VehicleDepotAssignment = new int[depots * vehicles];
      int index = 0;

      for (int i = 0; i < depots; i++)
        for (int j = 0; j < vehicles; j++) {
          instance.VehicleDepotAssignment[index] = i;
          index++;
        }

      instance.Name = parser.ProblemName;

      return instance;
    }
  }
}
