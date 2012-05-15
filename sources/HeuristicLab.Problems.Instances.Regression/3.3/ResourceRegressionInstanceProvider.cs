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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HeuristicLab.Problems.DataAnalysis;
using ICSharpCode.SharpZipLib.Zip;

namespace HeuristicLab.Problems.Instances.Regression {
  public abstract class ResourceRegressionInstanceProvider : RegressionInstanceProvider {

    protected abstract string FileName { get; }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var solutionsArchiveName = GetResourceName(FileName + @"\.zip");
      if (!String.IsNullOrEmpty(solutionsArchiveName)) {
        using (var solutionsZipFile = new ZipInputStream(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName))) {
          IList<string> entries = new List<string>();
          ZipEntry curEntry;
          while ((curEntry = solutionsZipFile.GetNextEntry()) != null) {
            entries.Add(curEntry.Name);
          }
          foreach (var entry in entries.OrderBy(x => x)) {
            yield return new ResourceRegressionDataDescriptor(Path.GetFileNameWithoutExtension(entry), Description, entry);
          }
        }
      }
    }

    public override IRegressionProblemData LoadData(IDataDescriptor id) {
      var descriptor = (ResourceRegressionDataDescriptor)id;

      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      using (var instancesZipFile = new ZipFile(GetType().Assembly.GetManifestResourceStream(instanceArchiveName))) {
        var entry = instancesZipFile.GetEntry(descriptor.ResourceName);
        NumberFormatInfo numberFormat;
        DateTimeFormatInfo dateFormat;
        char separator;
        using (Stream stream = instancesZipFile.GetInputStream(entry)) {
          TableFileParser.DetermineFileFormat(stream, out numberFormat, out dateFormat, out separator);
        }

        IRegressionProblemData regData;
        using (Stream stream = instancesZipFile.GetInputStream(entry)) {
          regData = LoadData(stream, numberFormat, dateFormat, separator);
        }

        regData.Name = descriptor.Name;
        regData.Description = descriptor.Description;
        return regData;
      }
    }

    protected virtual string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
              .Where(x => Regex.Match(x, @".*\.Data\." + fileName).Success).SingleOrDefault();
    }
  }
}
