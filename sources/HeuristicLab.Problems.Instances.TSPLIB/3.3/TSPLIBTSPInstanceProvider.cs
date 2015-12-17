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
using System.IO;
using System.Linq;

namespace HeuristicLab.Problems.Instances.TSPLIB {
  public class TSPLIBTSPInstanceProvider : TSPLIBInstanceProvider<TSPData> {

    public override string Name {
      get { return "TSPLIB (symmetric TSP)"; }
    }

    public override string Description {
      get { return "Traveling Salesman Problem Library"; }
    }

    protected override string FileExtension { get { return "tsp"; } }

    protected override TSPData LoadInstance(TSPLIBParser parser, IDataDescriptor descriptor = null) {
      parser.Parse();
      if (parser.FixedEdges != null) throw new InvalidDataException("TSP instance " + parser.Name + " contains fixed edges which are not supported by HeuristicLab.");
      var instance = new TSPData();
      instance.Dimension = parser.Dimension;
      instance.Coordinates = parser.Vertices != null ? parser.Vertices : parser.DisplayVertices;
      instance.Distances = parser.Distances;
      switch (parser.EdgeWeightType) {
        case TSPLIBEdgeWeightTypes.ATT:
          instance.DistanceMeasure = DistanceMeasure.Att; break;
        case TSPLIBEdgeWeightTypes.CEIL_2D:
          instance.DistanceMeasure = DistanceMeasure.UpperEuclidean; break;
        case TSPLIBEdgeWeightTypes.EUC_2D:
          instance.DistanceMeasure = DistanceMeasure.RoundedEuclidean; break;
        case TSPLIBEdgeWeightTypes.EUC_3D:
          throw new InvalidDataException("3D coordinates are not supported.");
        case TSPLIBEdgeWeightTypes.EXPLICIT:
          instance.DistanceMeasure = DistanceMeasure.Direct; break;
        case TSPLIBEdgeWeightTypes.GEO:
          instance.DistanceMeasure = DistanceMeasure.Geo; break;
        case TSPLIBEdgeWeightTypes.MAN_2D:
          instance.DistanceMeasure = DistanceMeasure.Manhattan; break;
        case TSPLIBEdgeWeightTypes.MAN_3D:
          throw new InvalidDataException("3D coordinates are not supported.");
        case TSPLIBEdgeWeightTypes.MAX_2D:
          instance.DistanceMeasure = DistanceMeasure.Maximum; break;
        case TSPLIBEdgeWeightTypes.MAX_3D:
          throw new InvalidDataException("3D coordinates are not supported.");
        default:
          throw new InvalidDataException("The given edge weight is not supported by HeuristicLab.");
      }

      instance.Name = parser.Name;
      instance.Description = parser.Comment
        + Environment.NewLine + Environment.NewLine
        + GetInstanceDescription();
      return instance;
    }

    protected override void LoadSolution(TSPLIBParser parser, TSPData instance) {
      parser.Parse();
      instance.BestKnownTour = parser.Tour.FirstOrDefault();
    }

    public TSPData LoadData(string tspFile, string tourFile, double? bestQuality) {
      var data = LoadInstance(new TSPLIBParser(tspFile));
      if (!String.IsNullOrEmpty(tourFile)) {
        var tourParser = new TSPLIBParser(tourFile);
        LoadSolution(tourParser, data);
      }
      if (bestQuality.HasValue)
        data.BestKnownQuality = bestQuality.Value;
      return data;
    }

  }
}
