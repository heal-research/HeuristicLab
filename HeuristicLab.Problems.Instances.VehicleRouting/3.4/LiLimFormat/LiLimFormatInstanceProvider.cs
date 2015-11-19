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

using System.IO;

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  public abstract class LiLimFormatInstanceProvider : VRPInstanceProvider<PDPTWData> {
    protected override PDPTWData LoadData(Stream stream) {
      return LoadInstance(new LiLimParser(stream));
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override PDPTWData ImportData(string path) {
      return LoadInstance(new LiLimParser(path));
    }

    private PDPTWData LoadInstance(LiLimParser parser) {
      parser.Parse();

      var instance = new PDPTWData();

      instance.Dimension = parser.Cities + 1;
      instance.Coordinates = parser.Coordinates;
      instance.Capacity = parser.Capacity;
      instance.Demands = parser.Demands;
      instance.DistanceMeasure = DistanceMeasure.Euclidean;
      instance.ReadyTimes = parser.Readytimes;
      instance.ServiceTimes = parser.Servicetimes;
      instance.DueTimes = parser.Duetimes;
      instance.MaximumVehicles = parser.Vehicles;
      instance.PickupDeliveryLocations = parser.PickupDeliveryLocations;

      instance.Name = parser.ProblemName;

      return instance;
    }
  }
}
