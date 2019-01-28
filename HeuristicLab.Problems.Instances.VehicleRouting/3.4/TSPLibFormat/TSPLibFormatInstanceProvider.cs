#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.Instances.TSPLIB;

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  public abstract class TSPLibFormatInstanceProvider : VRPInstanceProvider<CVRPData> {
    protected override CVRPData LoadData(Stream stream) {
      return LoadInstance(new TSPLIBParser(stream));
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override CVRPData ImportData(string path) {
      return LoadInstance(new TSPLIBParser(path));
    }

    private CVRPData LoadInstance(TSPLIBParser parser) {
      parser.Parse();

      var instance = new CVRPData();
      instance.Dimension = parser.Dimension;
      instance.Coordinates = parser.Vertices != null ? parser.Vertices : parser.DisplayVertices;
      instance.Distances = parser.Distances;
      instance.Capacity = parser.Capacity.Value;
      instance.Demands = parser.Demands;
      switch (parser.EdgeWeightType) {
        case TSPLIBEdgeWeightTypes.ATT:
          instance.DistanceMeasure = DistanceMeasure.Att; break;
        case TSPLIBEdgeWeightTypes.CEIL_2D:
          instance.DistanceMeasure = DistanceMeasure.UpperEuclidean; break;
        case TSPLIBEdgeWeightTypes.EUC_2D:
        case TSPLIBEdgeWeightTypes.EUC_3D:
          instance.DistanceMeasure = DistanceMeasure.RoundedEuclidean; break;
        case TSPLIBEdgeWeightTypes.EXPLICIT:
          instance.DistanceMeasure = DistanceMeasure.Direct; break;
        case TSPLIBEdgeWeightTypes.GEO:
          instance.DistanceMeasure = DistanceMeasure.Geo; break;
        case TSPLIBEdgeWeightTypes.MAN_2D:
        case TSPLIBEdgeWeightTypes.MAN_3D:
          instance.DistanceMeasure = DistanceMeasure.Manhattan; break;
        case TSPLIBEdgeWeightTypes.MAX_2D:
        case TSPLIBEdgeWeightTypes.MAX_3D:
          instance.DistanceMeasure = DistanceMeasure.Maximum; break;
        default:
          throw new InvalidDataException("The given edge weight is not supported by HeuristicLab.");
      }

      instance.Name = parser.Name;
      instance.Description = parser.Comment
        + Environment.NewLine + Environment.NewLine
        + GetInstanceDescription();

      return instance;
    }
  }
}
