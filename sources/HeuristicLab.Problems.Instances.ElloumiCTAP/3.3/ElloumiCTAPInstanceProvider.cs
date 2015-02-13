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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

    private const string FileName = "ElloumiCTAP";

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      Dictionary<string, string> solutions = new Dictionary<string, string>();
      var solutionsArchiveName = GetResourceName(FileName + @"\.sol\.zip");
      if (!String.IsNullOrEmpty(solutionsArchiveName)) {
        using (var solutionsZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName), ZipArchiveMode.Read)) {
          foreach (var entry in solutionsZipFile.Entries)
            solutions.Add(Path.GetFileNameWithoutExtension(entry.Name) + ".dat", entry.Name);
        }
      }
      var instanceArchiveName = GetResourceName(FileName + @"\.dat\.zip");
      if (String.IsNullOrEmpty(instanceArchiveName)) yield break;

      using (var instanceStream = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        foreach (var entry in instanceStream.Entries.Select(x => x.Name).OrderBy(x => x)) {
          yield return new ElloumiCTAPDataDescriptor(Path.GetFileNameWithoutExtension(entry), GetDescription(), entry, solutions.ContainsKey(entry) ? solutions[entry] : String.Empty);
        }
      }
    }

    public override CTAPData LoadData(IDataDescriptor id) {
      var descriptor = (ElloumiCTAPDataDescriptor)id;
      var instanceArchiveName = GetResourceName(FileName + @"\.dat\.zip");
      using (var instancesZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        var entry = instancesZipFile.GetEntry(descriptor.InstanceIdentifier);
        using (var stream = entry.Open()) {
          var parser = new ElloumiCTAPParser();
          parser.Parse(stream);
          var instance = Load(parser);

          instance.Name = id.Name;
          instance.Description = id.Description;

          if (!String.IsNullOrEmpty(descriptor.SolutionIdentifier)) {
            var solutionsArchiveName = GetResourceName(FileName + @"\.sol\.zip");
            using (var solutionsZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName), ZipArchiveMode.Read)) {
              entry = solutionsZipFile.GetEntry(descriptor.SolutionIdentifier);
              using (var solStream = entry.Open()) {
                ElloumiCTAPSolutionParser slnParser = new ElloumiCTAPSolutionParser();
                slnParser.Parse(solStream, instance.MemoryRequirements.Length);
                if (slnParser.Error != null) throw slnParser.Error;

                instance.BestKnownAssignment = slnParser.Assignment;
                instance.BestKnownQuality = slnParser.Quality;
              }
            }
          }
          return instance;
        }
      }
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override CTAPData ImportData(string path) {
      var parser = new ElloumiCTAPParser();
      parser.Parse(path);
      var instance = Load(parser);
      instance.Name = Path.GetFileName(path);
      instance.Description = "Loaded from file \"" + path + "\" on " + DateTime.Now.ToString();
      return instance;
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

    protected virtual string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
              .Where(x => Regex.Match(x, @".*\.Data\." + fileName).Success).SingleOrDefault();
    }
  }
}
