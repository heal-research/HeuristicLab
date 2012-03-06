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

namespace HeuristicLab.Problems.Instances.CordeauGQAP {
  public class CordeauGQAPInstanceProvider : ProblemInstanceProvider<GQAPData> {
    public override string Name {
      get { return "Cordeau et al. GQAP instances"; }
    }

    public override string Description {
      get { return "GQAP instances published by Cordeau, Gaudioso, Laporte, and Moccia."; }
    }

    public override Uri WebLink {
      get { return null; }
    }

    public override string ReferencePublication {
      get {
        return @"Cordeau, J.-F., Gaudioso, M., Laporte, G., Moccia, L. 2006.
A memetic heuristic for the generalized quadratic assignment problem.
INFORMS Journal on Computing, 18, pp. 433–443.";
      }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return Assembly.GetExecutingAssembly()
          .GetManifestResourceNames()
          .Where(x => x.EndsWith(".txt"))
          .OrderBy(x => x)
          .Select(x => new CordeauGQAPDataDescriptor(GetPrettyName(x), GetDescription(), x));
    }

    public override GQAPData LoadData(IDataDescriptor id) {
      var descriptor = (CordeauGQAPDataDescriptor)id;
      using (var stream = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream(descriptor.InstanceIdentifier)) {
        var parser = new CordeauGQAPParser();
        parser.Parse(stream);
        var instance = Load(parser);

        instance.Name = id.Name;
        instance.Description = id.Description;

        return instance;
      }
    }

    public override GQAPData LoadData(string path) {
      var parser = new CordeauGQAPParser();
      parser.Parse(path);
      var instance = Load(parser);

      instance.Name = Path.GetFileName(path);
      instance.Description = "Loaded from file \"" + path + "\" on " + DateTime.Now.ToString();

      return instance;
    }

    public override void SaveData(GQAPData instance, string path) {
      throw new NotSupportedException();
    }

    private GQAPData Load(CordeauGQAPParser parser) {
      var instance = new GQAPData();
      instance.Equipments = parser.Equipments;
      instance.Locations = parser.Locations;
      instance.Demands = parser.Demands;
      instance.Capacities = parser.Capacities;
      instance.Weights = parser.Weights;
      instance.Distances = parser.Distances;
      instance.InstallationCosts = parser.InstallationCosts;
      instance.TransportationCosts = parser.TransportationCosts;
      return instance;
    }

    private string GetPrettyName(string instanceIdentifier) {
      return Regex.Match(instanceIdentifier, GetType().Namespace + @"\.Data\.(.*)\.txt").Groups[1].Captures[0].Value;
    }

    private string GetDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }
  }
}
