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
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.BinPacking3D.Instances;

namespace HeuristicLab.Problems.BinPacking3D {
  public sealed class RealWorldContainerPackingInstanceProvider : ProblemInstanceProvider<BPPData> {
    private string FileName { get { return "ContainerPackingInstances"; } }
    
    public override string Name {
      get { return "Real-world Container Packing"; }
    }

    public override string Description {
      get { return "Problem instances derived from real-world container packing data."; }
    }

    public override Uri WebLink {
      get { return null; }
    }

    public override string ReferencePublication {
      get { return null; }
    }

    public RealWorldContainerPackingInstanceProvider() : base() { }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      if (String.IsNullOrEmpty(instanceArchiveName)) yield break;

      using (var instanceStream = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        foreach (var entry in instanceStream.Entries.Select(x => x.Name).OrderBy(x => x)) {
          yield return new ThreeDInstanceDescriptor(Path.GetFileNameWithoutExtension(entry), GetDescription(), entry);
        }
      }
    }

    private string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
              .Where(x => Regex.Match(x, @".*\.Instances\." + fileName).Success).SingleOrDefault();
    }

    private string GetDescription() {
      return "Embedded instance of plugin version " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().First().Version + ".";
    }

    public override BPPData LoadData(IDataDescriptor dd) {
      var desc = dd as ThreeDInstanceDescriptor;
      if (desc == null) throw new NotSupportedException("Cannot load data descriptor " + dd);
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      using (
        var instancesZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName),
          ZipArchiveMode.Read)) {
        var entry = instancesZipFile.GetEntry(desc.InstanceIdentifier);

        using (var stream = entry.Open()) {
          var parser = new ThreeDInstanceParser();
          parser.Parse(stream);

          return new BPPData() {
            Name = desc.Name,
            Description = desc.Description,
            BinShape = parser.Bin,
            Items = parser.Items.ToArray()
          };
        }
      }
    }

    public override bool CanImportData {
      get { return true; }
    }

    public override BPPData ImportData(string path) {
      using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
        var parser = new ThreeDInstanceParser();
        parser.Parse(stream);
        return new BPPData() {
          Name = Path.GetFileNameWithoutExtension(path),
          Description = "Imported instance from " + path,
          BinShape = parser.Bin,
          Items = parser.Items.ToArray()
        };
      }
    }

    public override bool CanExportData {
      get { return true; }
    }

    public override void ExportData(BPPData instance, string file) {
      using (Stream stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write)) {
        Export(instance, stream);
      }
    }
    public static void Export(BPPData instance, Stream stream) {

      using (var writer = new StreamWriter(stream)) {
        writer.WriteLine(String.Format("{0,-5} {1,-5} {2,-5}   WBIN,HBIN,DBIN", instance.BinShape.Width, instance.BinShape.Height, instance.BinShape.Depth));
        for (int i = 0; i < instance.NumItems; i++) {
          if (i == 0)
            writer.WriteLine("{0,-5} {1,-5} {2,-5}   W(I),H(I),D(I),I=1,...,N", instance.Items[i].Width, instance.Items[i].Height, instance.Items[i].Depth);
          else
            writer.WriteLine("{0,-5} {1,-5} {2,-5}", instance.Items[i].Width, instance.Items[i].Height, instance.Items[i].Depth);

        }
        writer.Flush();
      }
    }

  }
}
