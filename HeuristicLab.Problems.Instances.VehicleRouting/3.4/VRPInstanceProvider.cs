#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  public abstract class VRPInstanceProvider<TData> : ProblemInstanceProvider<TData>, IVRPInstanceProvider<TData> where TData : IVRPData {
    protected abstract string FileName { get; }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var solutions = new Dictionary<string, string>();
      var solutionsArchiveName = GetResourceName(FileName + @"\.opt\.zip");
      if (!String.IsNullOrEmpty(solutionsArchiveName)) {
        using (var solutionsZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName), ZipArchiveMode.Read)) {
          foreach (var entry in solutionsZipFile.Entries)
            solutions.Add(Path.GetFileNameWithoutExtension(entry.Name) + "." + FileName, entry.Name);
        }
      }
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      if (String.IsNullOrEmpty(instanceArchiveName)) yield break;

      using (var instanceStream = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        foreach (var entry in instanceStream.Entries.Select(x => x.Name).OrderBy(x => x, new NaturalStringComparer())) {
          string solutionEntry = Path.GetFileNameWithoutExtension(entry) + "." + FileName;
          yield return new VRPDataDescriptor(Path.GetFileNameWithoutExtension(entry), GetInstanceDescription(), entry, solutions.ContainsKey(solutionEntry) ? solutions[solutionEntry] : String.Empty);
        }
      }
    }

    public override TData LoadData(IDataDescriptor id) {
      var descriptor = (VRPDataDescriptor)id;
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      using (var instancesZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName))) {
        var entry = instancesZipFile.GetEntry(descriptor.InstanceIdentifier);
        var stream = entry.Open();
        var instance = LoadData(stream);
        if (string.IsNullOrEmpty(instance.Name)) {
          instance.Name = Path.GetFileNameWithoutExtension(entry.ToString());
        }

        if (!String.IsNullOrEmpty(descriptor.SolutionIdentifier)) {
          var solutionsArchiveName = GetResourceName(FileName + @"\.opt\.zip");
          using (var solutionsZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName))) {
            entry = solutionsZipFile.GetEntry(descriptor.SolutionIdentifier);
            stream = entry.Open();
            LoadSolution(stream, instance);
          }
        }

        return instance;
      }
    }

    #region IVRPInstanceProvider
    public TData Import(string vrpFile, string tourFile) {
      var data = ImportData(vrpFile);
      if (!String.IsNullOrEmpty(tourFile)) {
        LoadSolution(tourFile, data);
      }
      return data;
    }

    public void Export(TData instance, string path) {
      ExportData(instance, path);
    }
    #endregion

    protected virtual void LoadSolution(Stream stream, TData instance) {
      LoadOptFile(stream, instance);
    }

    private void LoadOptFile(Stream stream, TData instance) {
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

          if (line.StartsWith("Solution")) {
            if (routes.Any()) {
              // Skip remaining solutions since only one "best solution" is stored
              break;
            }
          }
        }
      }

      instance.BestKnownTour = routes.Select(x => x.ToArray()).ToArray();
    }

    public void LoadSolution(string path, TData instance) {
      try {
        using (var stream = new FileStream(path, FileMode.Open)) {
          LoadSolution(stream, instance);
        }
      }
      catch (Exception) {
        // new stream necessary because first try already read from stream
        using (var stream = new FileStream(path, FileMode.Open)) {
          LoadOptFile(stream, instance); // Fallback to .opt-Format
        }
      }
    }

    protected abstract TData LoadData(Stream stream);

    #region Helpers
    protected virtual string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
              .Where(x => Regex.Match(x, @".*\.Data\." + fileName).Success).SingleOrDefault();
    }

    protected virtual string GetInstanceDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }
    #endregion
  }
}
