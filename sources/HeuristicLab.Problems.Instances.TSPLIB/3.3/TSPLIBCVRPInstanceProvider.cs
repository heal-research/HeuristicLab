#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.TSPLIB {
  public class TSPLIBCVRPInstanceProvider : TSPLIBInstanceProvider<CVRPData> {

    public override string Name {
      get { return "TSPLIB (CVRP)"; }
    }

    public override string Description {
      get { return "Traveling Salesman Problem Library"; }
    }

    protected override string FileExtension { get { return "vrp"; } }

    protected override CVRPData LoadInstance(TSPLIBParser parser) {
      parser.Parse();
      var instance = new CVRPData();
      instance.Dimension = parser.Dimension;
      instance.Coordinates = parser.Vertices != null ? parser.Vertices : parser.DisplayVertices;
      instance.Distances = parser.Distances;
      instance.Capacity = parser.Capacity.Value;
      instance.Demands = parser.Demands;
      switch (parser.EdgeWeightType) {
        case TSPLIBEdgeWeightTypes.ATT:
          instance.DistanceMeasure = CVRPDistanceMeasure.Att; break;
        case TSPLIBEdgeWeightTypes.CEIL_2D:
          instance.DistanceMeasure = CVRPDistanceMeasure.UpperEuclidean; break;
        case TSPLIBEdgeWeightTypes.EUC_2D:
        case TSPLIBEdgeWeightTypes.EUC_3D:
          instance.DistanceMeasure = CVRPDistanceMeasure.RoundedEuclidean; break;
        case TSPLIBEdgeWeightTypes.EXPLICIT:
          instance.DistanceMeasure = CVRPDistanceMeasure.Direct; break;
        case TSPLIBEdgeWeightTypes.GEO:
          instance.DistanceMeasure = CVRPDistanceMeasure.Geo; break;
        case TSPLIBEdgeWeightTypes.MAN_2D:
        case TSPLIBEdgeWeightTypes.MAN_3D:
          instance.DistanceMeasure = CVRPDistanceMeasure.Manhattan; break;
        case TSPLIBEdgeWeightTypes.MAX_2D:
        case TSPLIBEdgeWeightTypes.MAX_3D:
          instance.DistanceMeasure = CVRPDistanceMeasure.Maximum; break;
        default:
          throw new InvalidDataException("The given edge weight is not supported by HeuristicLab.");
      }

      instance.Name = parser.Name;
      instance.Description = parser.Comment
        + Environment.NewLine + Environment.NewLine
        + GetInstanceDescription();

      return instance;
    }

    protected override void LoadSolution(TSPLIBParser parser, CVRPData instance) {
      parser.Parse();
      instance.BestKnownTour = parser.Tour;
    }
  }
}
