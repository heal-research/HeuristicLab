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

namespace HeuristicLab.Problems.Instances.Scheduling {
  public class JSSPORLIBInstanceProvider : ProblemInstanceProvider<JSSPData> {

    public override string Name {
      get { return "ORLIB JSSP"; }
    }

    public override string Description {
      get { return "Job shop scheduling problems from the Operations Research Library."; }
    }

    public override Uri WebLink {
      get { return new Uri("http://people.brunel.ac.uk/~mastjjb/jeb/orlib/jobshopinfo.html"); }
    }

    public override string ReferencePublication {
      get { return String.Empty; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var instanceArchiveName = GetResourceName("JSSPORLIB.zip");
      if (String.IsNullOrEmpty(instanceArchiveName)) yield break;

      using (var instanceStream = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        foreach (var entry in instanceStream.Entries.Select(x => x.Name).OrderBy(x => x)) {
          yield return new JSSPORLIBDataDescriptor(Path.GetFileNameWithoutExtension(entry), GetDescription(), entry, null);
        }
      }
    }

    public override JSSPData LoadData(IDataDescriptor id) {
      var descriptor = (JSSPORLIBDataDescriptor)id;
      var instanceArchiveName = GetResourceName("JSSPORLIB.zip");
      using (var instancesZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        var entry = instancesZipFile.GetEntry(descriptor.InstanceIdentifier);

        using (var stream = entry.Open()) {
          var parser = new JSSPORLIBParser();
          parser.Parse(stream);
          var instance = Load(parser);
          instance.Name = id.Name;
          instance.Description = id.Description;

          return instance;
        }
      }
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override JSSPData ImportData(string path) {
      var parser = new JSSPORLIBParser();
      parser.Parse(path);
      var instance = Load(parser);
      instance.Name = Path.GetFileName(path);
      instance.Description = "Loaded from file \"" + path + "\" on " + DateTime.Now.ToString();
      return instance;
    }

    private JSSPData Load(JSSPORLIBParser parser) {
      var instance = new JSSPData {
        Jobs = parser.Jobs,
        Resources = parser.Resources,
        ProcessingTimes = parser.ProcessingTimes,
        Demands = parser.Demands,
        DueDates = parser.DueDates
      };
      return instance;
    }

    public override bool CanExportData {
      get { return true; }
    }

    public override void ExportData(JSSPData instance, string path) {
      var parser = new JSSPORLIBParser {
        Name = instance.Name,
        Description = instance.Description,
        Jobs = instance.Jobs,
        Resources = instance.Resources,
        ProcessingTimes = instance.ProcessingTimes,
        Demands = instance.Demands,
        DueDates = instance.DueDates
      };
      parser.Export(path);
    }

    private string GetDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }

    protected virtual string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
        .SingleOrDefault(x => Regex.Match(x, @".*\.Data\." + fileName).Success);
    }
  }
}
