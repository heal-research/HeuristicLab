#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public abstract class TaillardFormatInstanceProvider : VRPInstanceProvider<CVRPData> {
    protected override CVRPData LoadData(Stream stream) {
      return LoadInstance(new TaillardParser(stream));
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override CVRPData ImportData(string path) {
      return LoadInstance(new TaillardParser(path));
    }

    private CVRPData LoadInstance(TaillardParser parser) {
      parser.Parse();

      var instance = new CVRPData();
      instance.Dimension = parser.Cities + 1;
      instance.Coordinates = parser.Coordinates;
      instance.Capacity = parser.Capacity;
      instance.Demands = parser.Demands;
      instance.DistanceMeasure = DistanceMeasure.Euclidean;

      instance.Name = parser.ProblemName;

      return instance;
    }
  }
}
