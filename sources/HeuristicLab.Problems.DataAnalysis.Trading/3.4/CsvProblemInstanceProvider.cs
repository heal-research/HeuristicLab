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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.DataAnalysis;

namespace HeuristicLab.Problems.DataAnalysis.Trading {
  public class CsvProblemInstanceProvider : ProblemInstanceProvider<IProblemData> {
    public override string Name {
      get { return "CSV File"; }
    }
    public override string Description {
      get {
        return "Comma separated values file importer";
      }
    }
    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com/trac/hl/core/wiki/UsersFAQ#DataAnalysisImportFileFormat"); }
    }
    public override string ReferencePublication {
      get { return ""; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return new List<IDataDescriptor>();
    }
    public override IProblemData LoadData(IDataDescriptor descriptor) {
      throw new NotImplementedException();
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override IProblemData ImportData(string path) {
      TableFileParser csvFileParser = new TableFileParser();
      csvFileParser.Parse(path, csvFileParser.AreColumnNamesInFirstLine(path));

      Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
      string targetVar = (from v in dataset.DoubleVariables
                          where dataset.GetReadOnlyDoubleValues(v).Min() <= 0
                          where dataset.GetReadOnlyDoubleValues(v).Max() >= 0
                          select v).LastOrDefault();
      if (targetVar == null) throw new ArgumentException("The target variable must contain changes (deltas) of the asset price over time.");

      // turn off input variables that are constant in the training partition
      var allowedInputVars = new List<string>();
      var trainingIndizes = Enumerable.Range(0, (csvFileParser.Rows * 2) / 3);
      if (trainingIndizes.Count() >= 2) {
        foreach (var variableName in dataset.DoubleVariables) {
          if (dataset.GetDoubleValues(variableName, trainingIndizes).Range() > 0 &&
            variableName != targetVar)
            allowedInputVars.Add(variableName);
        }
      } else {
        allowedInputVars.AddRange(dataset.DoubleVariables.Where(x => !x.Equals(targetVar)));
      }

      IProblemData problemData = new ProblemData(dataset, allowedInputVars, targetVar);

      var trainingPartEnd = trainingIndizes.Last();
      problemData.TrainingPartition.Start = trainingIndizes.First();
      problemData.TrainingPartition.End = trainingPartEnd;
      problemData.TestPartition.Start = trainingPartEnd;
      problemData.TestPartition.End = csvFileParser.Rows;

      problemData.Name = Path.GetFileName(path);

      return problemData;
    }
  }
}
