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
using System.Globalization;
using System.IO;
using System.Linq;

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

    protected override void LoadSolution(Stream stream, CVRPTWData instance) {
      using (var reader = new StreamReader(stream)) {
        string instanceName = ExtractValue(reader.ReadLine());
        string authors = ExtractValue(reader.ReadLine());
        string date = ExtractValue(reader.ReadLine());
        string reference = ExtractValue(reader.ReadLine());
        switch (reader.ReadLine().Trim()) { // "Solution" or "Distance"
          case "Solution":
            var routesQuery = from line in reader.ReadAllLines()
                              where !string.IsNullOrEmpty(line)
                              let tokens = ExtractValue(line).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              let stops = tokens.Select(int.Parse).Select(s => s - 1)
                              select stops;
            var routes = routesQuery.Select(s => s.ToArray()).ToArray();
            instance.BestKnownTour = routes;
            break;
          case "Distance":
            double quality = double.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
            instance.BestKnownQuality = quality;
            break;
        }
      }
    }

    private static string ExtractValue(string line) {
      return line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
    }
  }
}
