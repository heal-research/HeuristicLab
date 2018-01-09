#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.Instances.Types;

namespace HeuristicLab.Problems.Instances.Orienteering {
  public class SchildeInstanceProvider : ProblemInstanceProvider<OPData> {
    public override string Name {
      get { return "Schilde (OP)"; }
    }
    public override string Description {
      get { return "Modified Tsiligirdes (1984) and Chao (1996) instances by Schilde."; }
    }
    public override string ReferencePublication {
      get {
        return @"Michael Schilde, Karl F. Doerner, Richard F. Hartl, Guenter Kiechle. 2009.
Metaheuristics for the bi-objective orienteering problem.
Swarm Intelligence, Volume 3, Issue 3, pp 179-201.";
      }
    }
    public override Uri WebLink {
      get { return new Uri("http://prolog.univie.ac.at/research/OP/"); }
    }

    private const string FileName = "Schilde";

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      if (String.IsNullOrEmpty(instanceArchiveName)) yield break;

      using (var instanceStream = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName))) {
        foreach (var entry in instanceStream.Entries.Select(x => x.Name).OrderBy(x => x)) {
          yield return new OPDataDescriptor(Path.GetFileNameWithoutExtension(entry), GetInstanceDescription(), entry);
        }
      }
    }
    public override OPData LoadData(IDataDescriptor id) {
      var descriptor = (OPDataDescriptor)id;
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      var parser = new SchildeParser();
      using (var instancesZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName))) {
        var entry = instancesZipFile.GetEntry(descriptor.InstanceIdentifier);
        using (var stream = entry.Open()) {
          parser.Parse(stream);
          var instance = LoadInstance(parser);

          instance.Name = id.Name;
          instance.Description = id.Description;

          return instance;
        }
      }
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override OPData ImportData(string path) {
      var parser = new SchildeParser();
      parser.Parse(path);
      var instance = LoadInstance(parser);

      instance.Name = Path.GetFileName(path);
      instance.Description = "Loaded from file \"" + path + "\" on " + DateTime.Now.ToString();

      return instance;
    }

    private OPData LoadInstance(SchildeParser parser) {
      return new OPData {
        Dimension = parser.Coordinates.GetLength(0),
        Coordinates = parser.Coordinates,
        Distances = parser.Distances,
        DistanceMeasure = parser.Distances != null ? DistanceMeasure.Direct : DistanceMeasure.Euclidean,
        MaximumDistance = parser.UpperBoundConstraint,
        StartingPoint = parser.StartingPoint,
        TerminalPoint = parser.TerminalPoint,
        Scores = Enumerable.Range(0, parser.Scores.GetLength(0)).Select(i => parser.Scores[i, 0]).ToArray()
      };
    }

    protected virtual string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
        .SingleOrDefault(x => Regex.Match(x, @".*\.Data\." + fileName).Success);
    }

    protected virtual string GetInstanceDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }
  }
}