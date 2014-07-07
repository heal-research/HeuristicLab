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

using System;
using System.IO;

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  public abstract class GoldenFormatInstanceProvider : VRPInstanceProvider {
    protected override VRPData LoadData(Stream stream) {
      return LoadInstance(new GoldenParser(stream));
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override VRPData ImportData(string path) {
      return LoadInstance(new GoldenParser(path));
    }

    private CVRPTWData LoadInstance(GoldenParser parser) {
      parser.Parse();

      var instance = new CVRPTWData();
      instance.Dimension = parser.Vertices.GetLength(0) + 1;
      instance.Coordinates = parser.Vertices;
      instance.Capacity = parser.Capacity;
      instance.Demands = parser.Demands;
      switch (parser.WeightType) {
        case GoldenParser.GoldenEdgeWeightType.EUC_2D:
          instance.DistanceMeasure = DistanceMeasure.Euclidean; break;
        case GoldenParser.GoldenEdgeWeightType.GEO:
          instance.DistanceMeasure = DistanceMeasure.Geo; break;
        default:
          throw new InvalidDataException("The given edge weight is not supported by HeuristicLab.");
      }

      instance.ReadyTimes = new double[instance.Dimension];
      instance.ServiceTimes = new double[instance.Dimension];
      instance.DueTimes = new double[instance.Dimension];

      for (int i = 0; i < instance.Dimension; i++) {
        instance.ReadyTimes[i] = 0;
        instance.ServiceTimes[i] = 0;
        instance.DueTimes[i] = double.MaxValue;
      }

      if (parser.Distance > 0)
        instance.DueTimes[0] = parser.Distance;

      if (parser.Vehicles > 0)
        instance.MaximumVehicles = parser.Vehicles;

      instance.Name = parser.ProblemName;
      instance.Description = parser.Comment
        + Environment.NewLine + Environment.NewLine
        + GetInstanceDescription();

      return instance;
    }
  }
}
