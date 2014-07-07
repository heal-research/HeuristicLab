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
using ICSharpCode.SharpZipLib.Zip;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class UCIInstanceProvider : ResourceClassificationInstanceProvider {
    public override string Name {
      get { return "UCI Problems"; }
    }
    public override string Description {
      get {
        return "";
      }
    }
    public override Uri WebLink {
      get { return new Uri("http://archive.ics.uci.edu/ml/datasets.html"); }
    }
    public override string ReferencePublication {
      get { return ""; }
    }

    protected override string FileName { get { return "UCI"; } }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      List<UCIDataDescriptor> descriptorList = new List<UCIDataDescriptor>();
      descriptorList.Add(new Iris());
      descriptorList.Add(new Mammography());
      descriptorList.Add(new Parkinsons());
      descriptorList.Add(new Thyroid());
      descriptorList.Add(new Vertebral_3C());
      descriptorList.Add(new Wine());
      descriptorList.Add(new WisconsinDiagnosticBreastCancer());
      var solutionsArchiveName = GetResourceName(FileName + @"\.zip");
      if (!String.IsNullOrEmpty(solutionsArchiveName)) {
        using (var solutionsZipFile = new ZipInputStream(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName))) {
          IList<string> entries = new List<string>();
          ZipEntry curEntry;
          while ((curEntry = solutionsZipFile.GetNextEntry()) != null) {
            entries.Add(curEntry.Name);
          }
          foreach (var entry in entries.OrderBy(x => x)) {
            string filename = Path.GetFileNameWithoutExtension(entry);
            UCIDataDescriptor desc = descriptorList.Where(x => x.Filename.Equals(filename)).FirstOrDefault();
            if (desc != null) {
              desc.ResourceName = entry;
              yield return desc;
            } else
              throw new ArgumentNullException("No Descriptor could be found for this entry.");
          }
        }
      }
    }
  }
}
