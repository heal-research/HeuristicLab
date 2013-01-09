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
using HeuristicLab.Problems.DataAnalysis;
using ICSharpCode.SharpZipLib.Zip;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class RegressionRealWorldInstanceProvider : ResourceRegressionInstanceProvider {
    public override string Name {
      get { return "Real World Benchmark Problems"; }
    }
    public override string Description {
      get {
        return "";
      }
    }
    public override Uri WebLink {
      get { return null; }
    }
    public override string ReferencePublication {
      get { return ""; }
    }

    protected override string FileName { get { return "RegressionRealWorld"; } }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      List<IRealWorldRegressionDataDescriptor> descriptorList = new List<IRealWorldRegressionDataDescriptor>();
      descriptorList.Add(new Tower());
      var solutionsArchiveName = GetResourceName(FileName + @"\.zip");
      if (!String.IsNullOrEmpty(solutionsArchiveName)) {
        using (var solutionsZipFile = new ZipInputStream(GetType().Assembly.GetManifestResourceStream(solutionsArchiveName))) {
          IList<string> entries = new List<string>();
          ZipEntry curEntry;
          while ((curEntry = solutionsZipFile.GetNextEntry()) != null) {
            entries.Add(curEntry.Name);
          }
          foreach (var entry in entries.OrderBy(x => x)) {
            string prettyName = Path.GetFileNameWithoutExtension(entry);
            IRealWorldRegressionDataDescriptor desc = descriptorList.Where(x => x.Name.Equals(prettyName)).FirstOrDefault();
            if (desc != null) {
              yield return new RealWorldResourceRegressionDataDescriptor(prettyName, desc.Description, entry, desc.Training, desc.Test);
            } else
              yield return new ResourceRegressionDataDescriptor(prettyName, Description, entry);
          }
        }
      }
    }

    public override IRegressionProblemData LoadData(IDataDescriptor id) {
      var problem = base.LoadData(id);

      var descriptor = id as RealWorldResourceRegressionDataDescriptor;

      if (descriptor == null) {
        return problem;
      }

      problem.TrainingPartition.Start = descriptor.Training.Start;
      problem.TrainingPartition.End = descriptor.Training.End;
      problem.TestPartition.Start = descriptor.Test.Start;
      problem.TestPartition.End = descriptor.Test.End;

      return problem;
    }
  }
}
