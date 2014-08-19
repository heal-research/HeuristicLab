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

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  public abstract class SolomonFormatInstanceProvider : VRPInstanceProvider<CVRPTWData> {
    protected override CVRPTWData LoadData(Stream stream) {
      return LoadInstance(new SolomonParser(stream));
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override CVRPTWData ImportData(string path) {
      return LoadInstance(new SolomonParser(path));
    }

    private CVRPTWData LoadInstance(SolomonParser parser) {
      parser.Parse();

      var instance = new CVRPTWData();

      instance.Dimension = parser.Cities + 1;
      instance.Coordinates = parser.Coordinates;
      instance.Capacity = parser.Capacity;
      instance.Demands = parser.Demands;
      instance.DistanceMeasure = DistanceMeasure.Euclidean;
      instance.ReadyTimes = parser.Readytimes;
      instance.ServiceTimes = parser.Servicetimes;
      instance.DueTimes = parser.Duetimes;
      instance.MaximumVehicles = parser.Vehicles;

      instance.Name = parser.ProblemName;

      return instance;
    }
  }
}
