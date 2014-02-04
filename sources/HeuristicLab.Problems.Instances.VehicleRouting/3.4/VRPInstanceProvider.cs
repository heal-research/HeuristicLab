#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  public abstract class VRPInstanceProvider : ProblemInstanceProvider<VRPData>, IVRPInstanceProvider {
    protected abstract string FileName { get; }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      Dictionary<string, string> solutions = new Dictionary<string, string>();
      var solutionsArchiveName = GetResourceName(FileName + @"\.opt\.zip");
      if (!String.IsNullOrEmpty(solutionsArchiveName)) {
        using (var solutionsZipFile = new ZipInputStream(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName))) {
          foreach (var entry in GetZipContents(solutionsZipFile))
            solutions.Add(entry.Substring(0, entry.Length - ".opt".Length) + "." + FileName, entry);
        }
      }
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      if (String.IsNullOrEmpty(instanceArchiveName)) yield break;

      using (var instanceStream = new ZipInputStream(GetType().Assembly.GetManifestResourceStream(instanceArchiveName))) {
        foreach (var entry in GetZipContents(instanceStream).OrderBy(x => x)) {
          string solutionEntry = entry.Substring(0, entry.Length - ".opt".Length) + "." + FileName;
          yield return new VRPDataDescriptor(Path.GetFileNameWithoutExtension(entry), GetInstanceDescription(), entry, solutions.ContainsKey(solutionEntry) ? solutions[solutionEntry] : String.Empty);
        }
      }
    }

    public override VRPData LoadData(IDataDescriptor id) {
      var descriptor = (VRPDataDescriptor)id;
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      using (var instancesZipFile = new ZipFile(GetType().Assembly.GetManifestResourceStream(instanceArchiveName))) {
        var entry = instancesZipFile.GetEntry(descriptor.InstanceIdentifier);
        var stream = instancesZipFile.GetInputStream(entry);
        var instance = LoadData(stream);
        if (string.IsNullOrEmpty(instance.Name)) {
          instance.Name = Path.GetFileNameWithoutExtension(entry.ToString());
        }

        if (!String.IsNullOrEmpty(descriptor.SolutionIdentifier)) {
          var solutionsArchiveName = GetResourceName(FileName + @"\.opt\.zip");
          using (var solutionsZipFile = new ZipFile(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName))) {
            entry = solutionsZipFile.GetEntry(descriptor.SolutionIdentifier);
            stream = solutionsZipFile.GetInputStream(entry);
            LoadSolution(stream, instance);
          }
        }

        return instance;
      }
    }

    private static void LoadSolution(Stream stream, VRPData instance) {
      List<List<int>> routes = new List<List<int>>();

      using (StreamReader reader = new StreamReader(stream)) {
        String line;
        while ((line = reader.ReadLine()) != null) {
          if (line.StartsWith("Route")) {
            string[] token = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            List<int> route = new List<int>();

            for (int i = 2; i < token.Length; i++) {
              route.Add(int.Parse(token[i]) - 1);
            }

            routes.Add(route);
          }
        }
      }

      instance.BestKnownTour = routes.Select(x => x.ToArray()).ToArray();
    }

    public static void LoadSolution(string path, VRPData instance) {
      using (FileStream stream = new FileStream(path, FileMode.Open)) {
        LoadSolution(stream, instance);
      }
    }

    protected abstract VRPData LoadData(Stream stream);

    public IVRPData Import(string vrpFile, string tourFile) {
      var data = ImportData(vrpFile);
      if (!String.IsNullOrEmpty(tourFile)) {
        LoadSolution(tourFile, data);
      }
      return data;
    }

    public void Export(IVRPData instance, string path) {
      ExportData((VRPData)instance, path);
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
