#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.TSPLIB {
  public abstract class TSPLIBInstanceProvider<T> : ProblemInstanceProvider<T> {

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

    protected abstract string FileExtension { get; }

    protected abstract T LoadInstance(TSPLIBParser parser, IDataDescriptor descriptor = null);
    protected abstract void LoadSolution(TSPLIBParser parser, T instance);
    protected abstract void LoadQuality(double? bestQuality, T instance);

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      Dictionary<string, string> solutions = new Dictionary<string, string>();
      var solutionsArchiveName = GetResourceName(FileExtension + @"\.opt\.tour\.zip");
      if (!String.IsNullOrEmpty(solutionsArchiveName)) {
        using (var solutionsZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName), ZipArchiveMode.Read)) {
          foreach (var entry in solutionsZipFile.Entries)
            solutions.Add(entry.Name.Substring(0, entry.Name.Length - ".opt.tour".Length) + "." + FileExtension, entry.Name);
        }
      }

      Dictionary<string, double> qualities = new Dictionary<string, double>();
      var qualitiesArchiveName = GetResourceName(FileExtension + @"\.qual");
      if (!String.IsNullOrEmpty(qualitiesArchiveName)) {
        using (var qualitiesReader = new StreamReader(GetType().Assembly.GetManifestResourceStream(qualitiesArchiveName))) {
          while (!qualitiesReader.EndOfStream) {
            var line = qualitiesReader.ReadLine().Split(';');
            qualities.Add(line[0] + "." + FileExtension, double.Parse(line[1]));
          }
        }
      }

      var instanceArchiveName = GetResourceName(FileExtension + @"\.zip");
      if (String.IsNullOrEmpty(instanceArchiveName)) yield break;

      using (var instanceStream = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        foreach (var entry in instanceStream.Entries.Select(x => x.Name).OrderBy(x => x)) {
          yield return new TSPLIBDataDescriptor(Path.GetFileNameWithoutExtension(entry),
                                                GetInstanceDescription(),
                                                entry,
                                                solutions.ContainsKey(entry) ? solutions[entry] : String.Empty,
                                                qualities.ContainsKey(entry) ? (double?)qualities[entry] : null);
        }
      }
    }

    public override T LoadData(IDataDescriptor id) {
      var descriptor = (TSPLIBDataDescriptor)id;
      var instanceArchiveName = GetResourceName(FileExtension + @"\.zip");
      using (var instancesZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        var entry = instancesZipFile.GetEntry(descriptor.InstanceIdentifier);
        var stream = entry.Open();
        var parser = new TSPLIBParser(stream);
        var instance = LoadInstance(parser, id);
        LoadQuality(descriptor.BestQuality, instance);

        if (!String.IsNullOrEmpty(descriptor.SolutionIdentifier)) {
          var solutionsArchiveName = GetResourceName(FileExtension + @"\.opt\.tour\.zip");
          using (var solutionsZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName), ZipArchiveMode.Read)) {
            entry = solutionsZipFile.GetEntry(descriptor.SolutionIdentifier);
            stream = entry.Open();
            parser = new TSPLIBParser(stream);
            LoadSolution(parser, instance);
          }
        }

        return instance;
      }
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override T ImportData(string path) {
      return LoadInstance(new TSPLIBParser(path));
    }

    protected virtual string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
              .Where(x => Regex.Match(x, @".*\.Data\." + fileName).Success).SingleOrDefault();
    }

    protected virtual string GetInstanceDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }
  }
}
