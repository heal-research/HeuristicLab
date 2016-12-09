#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.DIMACS {
  public class GcolInstanceProvider : ProblemInstanceProvider<GCPData> {

    public override string Name {
      get { return "DIMACS Graph Coloring"; }
    }

    public override string Description {
      get { return "Graph Coloring problem instance library"; }
    }

    public override Uri WebLink {
      get { return new Uri("https://turing.cs.hbg.psu.edu/txn131/graphcoloring.html"); }
    }

    public override string ReferencePublication {
      get {
        return string.Empty;
      }
    }

    protected virtual string FileName { get { return "col"; } }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      if (String.IsNullOrEmpty(instanceArchiveName)) yield break;

      using (var instanceStream = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        foreach (var entry in instanceStream.Entries.Select(x => x.Name).OrderBy(x => x)) {
          yield return new GcolDataDescriptor(Path.GetFileNameWithoutExtension(entry), GetDescription(), entry);
        }
      }
    }

    public override GCPData LoadData(IDataDescriptor id) {
      var descriptor = (GcolDataDescriptor)id;
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      using (var instancesZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        var entry = instancesZipFile.GetEntry(descriptor.InstanceIdentifier);

        using (var stream = entry.Open()) {
          var parser = new Parser();
          parser.Parse(stream);
          var instance = Load(parser);
          instance.Name = id.Name;
          instance.Description = id.Description;
          int bestknown;
          if (bkq.TryGetValue(instance.Name, out bestknown))
            instance.BestKnownColors = bestknown;
          return instance;
        }
      }
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override GCPData ImportData(string path) {
      var parser = new Parser();
      parser.Parse(path);
      var instance = Load(parser);
      instance.Name = Path.GetFileName(path);
      instance.Description = "Loaded from file \"" + path + "\" on " + DateTime.Now.ToString();
      return instance;
    }

    private GCPData Load(Parser parser) {
      var instance = new GCPData();
      instance.Nodes = parser.Nodes;
      var adjacencies = new int[parser.Edges, 2];
      var i = 0;
      foreach (var a in parser.AdjacencyList) {
        adjacencies[i, 0] = a.Item1 - 1;
        adjacencies[i, 1] = a.Item2 - 1;
        i++;
      }
      instance.Adjacencies = adjacencies;
      return instance;
    }

    private string GetDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }

    protected virtual string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
              .Where(x => Regex.Match(x, @".*\.Data\." + fileName).Success).SingleOrDefault();
    }

    private Dictionary<string, int> bkq = new Dictionary<string, int> {
      { "fpsol2.i.1", 65 },
      { "fpsol2.i.2", 30 },
      { "fpsol2.i.3", 30 },
      { "inithx.i.1", 54 },
      { "inithx.i.2", 31 },
      { "inithx.i.3", 31 },
      { "le450_5a", 5 },
      { "le450_5b", 5 },
      { "le450_5c", 5 },
      { "le450_5d", 5 },
      { "le450_15a", 15 },
      { "le450_15b", 15 },
      { "le450_15c", 15 },
      { "le450_15d", 15 },
      { "le450_25a", 25 },
      { "le450_25b", 25 },
      { "le450_25c", 25 },
      { "le450_25d", 25 },
      { "mulsol.i.1", 49 },
      { "mulsol.i.2", 31 },
      { "mulsol.i.3", 31 },
      { "mulsol.i.4", 31 },
      { "mulsol.i.5", 31 },
      { "zeroin.i.1", 49 },
      { "zeroin.i.2", 30 },
      { "zeroin.i.3", 30 },
      { "anna", 11 },
      { "david", 11 },
      { "homer", 13 },
      { "huck", 11 },
      { "jean", 10 },
      { "games120", 9 },
      { "miles250", 8 },
      { "miles500", 20 },
      { "miles750", 31 },
      { "miles1000", 42 },
      { "miles1500", 73 },
      { "queen5_5", 5 },
      { "queen6_6", 7 },
      { "queen7_7", 7 },
      { "queen8_8", 9 },
      { "queen8_12", 12 },
      { "queen9_9", 10 },
      { "queen11_11", 11 },
      { "queen13_13", 13 },
      { "myciel3", 4 },
      { "myciel4", 5 },
      { "myciel5", 6 },
      { "myciel6", 7 },
      { "myciel7", 8 },
      { "mugg88_1", 4 },
      { "mugg88_25", 4 },
      { "mugg100_1", 4 },
      { "mugg100_25", 4 },
      { "1-Insertions_4", 4 },
      { "2-Insertions_3", 4 },
      { "2-Insertions_4", 4 },
      { "3-Insertions_3", 4 },
      { "4-Insertions_3", 3 },
      { "qg.order30", 30 },
      { "qg.order40", 40 },
      { "qg.order60", 60 },
      { "qg.order100", 100 }
    };
  }
}
