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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HeuristicLab.Problems.Instances.TSPLIB {
  public class TSPLIBCVRPInstanceProvider : ProblemInstanceProvider<CVRPData> {

    public override string Name {
      get { return "TSPLIB (CVRP)"; }
    }

    public override string Description {
      get { return "Traveling Salesman Problem Library"; }
    }

    public override Uri WebLink {
      get { return new Uri("http://comopt.ifi.uni-heidelberg.de/software/TSPLIB95/"); }
    }

    public override string ReferencePublication {
      get {
        return @"G. Reinelt. 1991.
TSPLIB - A Traveling Salesman Problem Library.
ORSA Journal on Computing, 3, pp. 376-384.";
      }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var solutions = Assembly.GetExecutingAssembly()
        .GetManifestResourceNames()
        .Where(x => Regex.Match(x, @".*\.Data\.CVRP\..*").Success)
        .Where(x => x.EndsWith(".opt.tour"))
        .ToDictionary(x => x.Substring(0, x.Length - ".opt.tour".Length) + ".vrp", x => x);

      return Assembly.GetExecutingAssembly()
          .GetManifestResourceNames()
          .Where(x => Regex.Match(x, @".*\.Data\.CVRP\..*").Success)
          .OrderBy(x => x)
          .Select(x => new TSPLIBDataDescriptor(GetPrettyName(x), GetDescription(), x, solutions.ContainsKey(x) ? solutions[x] : String.Empty));
    }

    public override CVRPData LoadData(IDataDescriptor id) {
      var descriptor = (TSPLIBDataDescriptor)id;
      using (var stream = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream(descriptor.InstanceIdentifier)) {
        var instance = Load(new TSPLIBParser(stream));

        if (!String.IsNullOrEmpty(descriptor.SolutionIdentifier)) {
          using (Stream solStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(descriptor.SolutionIdentifier)) {
            var slnParser = new TSPLIBParser(solStream);
            slnParser.Parse();
            instance.BestKnownTour = slnParser.Tour;
          }
        }
        return instance;
      }
    }

    public override CVRPData LoadData(string path) {
      return Load(new TSPLIBParser(path));
    }

    public override void SaveData(CVRPData instance, string path) {
      throw new NotSupportedException();
    }

    private CVRPData Load(TSPLIBParser parser) {
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
        + GetDescription();

      return instance;
    }

    private string GetPrettyName(string instanceIdentifier) {
      return Regex.Match(instanceIdentifier, GetType().Namespace + @"\.Data\.CVRP\.(.*)\.vrp").Groups[1].Captures[0].Value;
    }

    private string GetDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }
  }
}
