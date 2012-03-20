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
using ICSharpCode.SharpZipLib.Zip;

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

    protected abstract T LoadInstance(TSPLIBParser parser);
    protected abstract void LoadSolution(TSPLIBParser parser, T instance);

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      Dictionary<string, string> solutions = new Dictionary<string, string>();
      var solutionsArchiveName = GetResourceName(FileExtension + @"\.opt\.tour\.zip");
      if (!String.IsNullOrEmpty(solutionsArchiveName)) {
        using (var solutionsZipFile = new ZipInputStream(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName))) {
          foreach (var entry in GetZipContents(solutionsZipFile))
            solutions.Add(entry.Substring(0, entry.Length - ".opt.tour".Length) + "." + FileExtension, entry);
        }
      }
      var instanceArchiveName = GetResourceName(FileExtension + @"\.zip");
      if (String.IsNullOrEmpty(instanceArchiveName)) yield break;

      using (var instanceStream = new ZipInputStream(GetType().Assembly.GetManifestResourceStream(instanceArchiveName))) {
        foreach (var entry in GetZipContents(instanceStream).OrderBy(x => x)) {
          yield return new TSPLIBDataDescriptor(Path.GetFileNameWithoutExtension(entry), GetInstanceDescription(), entry, solutions.ContainsKey(entry) ? solutions[entry] : String.Empty);
        }
      }
    }

    public override T LoadData(IDataDescriptor id) {
      var descriptor = (TSPLIBDataDescriptor)id;
      var instanceArchiveName = GetResourceName(FileExtension + @"\.zip");
      using (var instancesZipFile = new ZipFile(GetType().Assembly.GetManifestResourceStream(instanceArchiveName))) {
        var entry = instancesZipFile.GetEntry(descriptor.InstanceIdentifier);
        var stream = instancesZipFile.GetInputStream(entry);
        var parser = new TSPLIBParser(stream);
        var instance = LoadInstance(parser);

        if (!String.IsNullOrEmpty(descriptor.SolutionIdentifier)) {
          var solutionsArchiveName = GetResourceName(FileExtension + @"\.opt\.tour\.zip");
          using (var solutionsZipFile = new ZipFile(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName))) {
            entry = solutionsZipFile.GetEntry(descriptor.SolutionIdentifier);
            stream = solutionsZipFile.GetInputStream(entry);
            parser = new TSPLIBParser(stream);
            LoadSolution(parser, instance);
          }
        }

        return instance;
      }
    }

    public override T LoadData(string path) {
      return LoadInstance(new TSPLIBParser(path));
    }

    public override void SaveData(T instance, string path) {
      throw new NotSupportedException();
    }

    protected virtual string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
              .Where(x => Regex.Match(x, @".*\.Data\." + fileName).Success).SingleOrDefault();
    }

    protected virtual string GetInstanceDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }

    protected IEnumerable<string> GetZipContents(ZipInputStream zipFile) {
      ZipEntry entry;
      while ((entry = zipFile.GetNextEntry()) != null) {
        yield return entry.Name;
      }
    }
  }
}
