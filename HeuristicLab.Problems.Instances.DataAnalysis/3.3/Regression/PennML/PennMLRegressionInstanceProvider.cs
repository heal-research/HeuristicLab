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
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class PennMLRegressionInstanceProvider : ResourceRegressionInstanceProvider {
    public override string Name {
      get { return "PennML Regression Problems"; }
    }

    public override string Description {
      get { return "A set of datasets used for benchmarking symbolic regression algorithms."; }
    }

    public override Uri WebLink {
      get { return new Uri("https://github.com/EpistasisLab/penn-ml-benchmarks"); }
    }

    public override string ReferencePublication {
      get { return "Patryk Orzechowski, William La Cava, Jason H. Moore - Where are we now? A large benchmark study of recent symbolic regression methods"; }
    }

    protected override string FileName {
      get { return "PennML"; }
    }

    // the reference publication uses 75% of the samples in each of the datasets for training and the remaining 25% for testing 
    private const double trainTestSplit = 0.75;

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var instanceArchiveName = GetResourceName(FileName + @"\.zip");
      using (var instancesZipFile = new ZipArchive(GetType().Assembly.GetManifestResourceStream(instanceArchiveName), ZipArchiveMode.Read)) {
        foreach (var entry in instancesZipFile.Entries) {
          NumberFormatInfo numberFormat;
          DateTimeFormatInfo dateFormat;
          char separator;
          using (var stream = entry.Open()) {
            // the method below disposes the stream 
            TableFileParser.DetermineFileFormat(stream, out numberFormat, out dateFormat, out separator);
          }

          using (var stream = entry.Open()) {
            using (var reader = new StreamReader(stream)) {
              var header = reader.ReadLine(); // read the first line

              // by convention each dataset from the PennML collection reserves the last column for the target
              var variableNames = header.Split(separator);
              var allowedInputVariables = variableNames.Take(variableNames.Length - 1);
              var target = variableNames.Last();

              // count lines
              int lines = 0; while (reader.ReadLine() != null) lines++;

              var trainEnd = (int)Math.Round(lines * trainTestSplit);
              var trainRange = new IntRange(0, trainEnd);
              var testRange = new IntRange(trainEnd, lines);

              var descriptor = new PennMLRegressionDataDescriptor(entry.Name, variableNames, allowedInputVariables, target, trainRange, testRange);
              yield return descriptor;
            }
          }
        }
      }
    }
  }
}
