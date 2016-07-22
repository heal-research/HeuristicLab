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
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public abstract class ResourceClassificationInstanceProvider : ClassificationInstanceProvider {

    protected abstract string FileName { get; }

    public override IClassificationProblemData LoadData(IDataDescriptor id) {
      var descriptor = (ResourceClassificationDataDescriptor)id;

      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      using (var instancesZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        var entry = instancesZipFile.GetEntry(descriptor.ResourceName);
        NumberFormatInfo numberFormat;
        DateTimeFormatInfo dateFormat;
        char separator;
        using (Stream stream = entry.Open()) {
          TableFileParser.DetermineFileFormat(stream, out numberFormat, out dateFormat, out separator);
        }

        TableFileParser csvFileParser = new TableFileParser();
        using (Stream stream = entry.Open()) {
          csvFileParser.Parse(stream, numberFormat, dateFormat, separator, true);
        }

        Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
        if (!descriptor.CheckVariableNames(csvFileParser.VariableNames)) {
          throw new ArgumentException("Parsed file contains variables which are not in the descriptor.");
        }

        return descriptor.GenerateClassificationData(dataset);
      }
    }

    protected virtual string GetResourceName(string fileName) {
      return Assembly.GetExecutingAssembly().GetManifestResourceNames()
              .Where(x => Regex.Match(x, @".*\.Data\." + fileName).Success).SingleOrDefault();
    }
  }
}
