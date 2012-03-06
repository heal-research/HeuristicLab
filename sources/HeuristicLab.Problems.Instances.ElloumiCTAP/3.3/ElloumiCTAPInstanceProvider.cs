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

namespace HeuristicLab.Problems.Instances.ElloumiCTAP {
  public class ElloumiCTAPInstanceProvider : ProblemInstanceProvider<CTAPData> {
    public override string Name {
      get { return "Elloumi's CTAP instances"; }
    }

    public override string Description {
      get { return "CTAP instances published by Sourour Elloumi"; }
    }

    public override Uri WebLink {
      get { return new Uri("http://cedric.cnam.fr/oc/TAP/TAP.html"); }
    }

    public override string ReferencePublication {
      get {
        return @"Elloumi, S. 1991.
Contribution for solving non linear programs with o-1 variables, application to task assignment problems in distributed systems.
PhD Thesis. Conservatoire National des Arts et Métiers, Paris.";
      }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var solutions = Assembly.GetExecutingAssembly()
        .GetManifestResourceNames()
        .Where(x => x.EndsWith(".sol"))
        .ToDictionary(x => Path.GetFileNameWithoutExtension(x) + ".dat", x => x);

      return Assembly.GetExecutingAssembly()
          .GetManifestResourceNames()
          .Where(x => x.EndsWith(".dat"))
          .OrderBy(x => x)
          .Select(x => new ElloumiCTAPDataDescriptor(GetPrettyName(x), GetDescription(), x, solutions.ContainsKey(x) ? solutions[x] : String.Empty));
    }

    public override CTAPData LoadData(IDataDescriptor id) {
      var descriptor = (ElloumiCTAPDataDescriptor)id;
      using (var stream = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream(descriptor.InstanceIdentifier)) {
        var parser = new ElloumiCTAPParser();
        parser.Parse(stream);
        var instance = Load(parser);

        instance.Name = id.Name;
        instance.Description = id.Description;

        if (!String.IsNullOrEmpty(descriptor.SolutionIdentifier)) {
          using (Stream solStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(descriptor.SolutionIdentifier)) {
            ElloumiCTAPSolutionParser slnParser = new ElloumiCTAPSolutionParser();
            slnParser.Parse(solStream, instance.MemoryRequirements.Length);
            if (slnParser.Error != null) throw slnParser.Error;

            instance.BestKnownAssignment = slnParser.Assignment;
            instance.BestKnownQuality = slnParser.Quality;
          }
        }
        return instance;
      }
    }

    public override CTAPData LoadData(string path) {
      var parser = new ElloumiCTAPParser();
      parser.Parse(path);
      var instance = Load(parser);
      instance.Name = Path.GetFileName(path);
      instance.Description = "Loaded from file \"" + path + "\" on " + DateTime.Now.ToString();
      return instance;
    }

    public override void SaveData(CTAPData instance, string path) {
      throw new NotSupportedException();
    }

    private CTAPData Load(ElloumiCTAPParser parser) {
      var instance = new CTAPData();
      instance.Processors = parser.Processors;
      instance.Tasks = parser.Tasks;
      instance.ExecutionCosts = parser.ExecutionCosts;
      instance.CommunicationCosts = parser.CommunicationCosts;
      instance.MemoryRequirements = parser.MemoryRequirements;
      instance.MemoryCapacities = parser.MemoryCapacities;
      return instance;
    }

    private string GetPrettyName(string instanceIdentifier) {
      return Regex.Match(instanceIdentifier, GetType().Namespace + @"\.Data\.(.*)\.dat").Groups[1].Captures[0].Value;
    }

    private string GetDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }
  }
}
