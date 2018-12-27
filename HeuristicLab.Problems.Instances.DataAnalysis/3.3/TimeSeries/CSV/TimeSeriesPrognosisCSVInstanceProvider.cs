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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class TimeSeriesPrognosisCSVInstanceProvider : TimeSeriesPrognosisInstanceProvider {
    public override string Name {
      get { return "CSV Problem Provider"; }
    }
    public override string Description {
      get {
        return "";
      }
    }
    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com/trac.fcgi/wiki/Documentation/FAQ#DataAnalysisImportFileFormat"); }
    }
    public override string ReferencePublication {
      get { return ""; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return new List<IDataDescriptor>();
    }

    public override ITimeSeriesPrognosisProblemData LoadData(IDataDescriptor descriptor) {
      throw new NotImplementedException();
    }

    public override bool CanImportData { get { return true; } }

    public override ITimeSeriesPrognosisProblemData ImportData(string path) {
      TableFileParser csvFileParser = new TableFileParser();
      csvFileParser.Parse(path, csvFileParser.AreColumnNamesInFirstLine(path));

      Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
      string targetVar = csvFileParser.VariableNames.Last();

      IEnumerable<string> allowedInputVars = dataset.DoubleVariables.Where(x => !x.Equals(targetVar));

      ITimeSeriesPrognosisProblemData timeSeriesPrognosisData = new TimeSeriesPrognosisProblemData(dataset, allowedInputVars, targetVar);

      int trainingPartEnd = csvFileParser.Rows * 2 / 3;
      timeSeriesPrognosisData.TrainingPartition.Start = 0;
      timeSeriesPrognosisData.TrainingPartition.End = trainingPartEnd;
      timeSeriesPrognosisData.TestPartition.Start = trainingPartEnd;
      timeSeriesPrognosisData.TestPartition.End = csvFileParser.Rows;

      int pos = path.LastIndexOf('\\');
      if (pos < 0)
        timeSeriesPrognosisData.Name = path;
      else {
        pos++;
        timeSeriesPrognosisData.Name = path.Substring(pos, path.Length - pos);
      }
      return timeSeriesPrognosisData;
    }

    protected override ITimeSeriesPrognosisProblemData ImportData(string path, TimeSeriesPrognosisImportType type, TableFileParser csvFileParser) {
      Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);

      // turn of input variables that are constant in the training partition
      var allowedInputVars = new List<string>();
      int trainingPartEnd = (csvFileParser.Rows * type.TrainingPercentage) / 100;
      trainingPartEnd = trainingPartEnd > 0 ? trainingPartEnd : 1;
      var trainingIndizes = Enumerable.Range(0, trainingPartEnd);
      if (trainingIndizes.Count() >= 2) {
        foreach (var variableName in dataset.DoubleVariables) {
          if (dataset.GetDoubleValues(variableName, trainingIndizes).Range() > 0 &&
            variableName != type.TargetVariable)
            allowedInputVars.Add(variableName);
        }
      } else {
        allowedInputVars.AddRange(dataset.DoubleVariables.Where(x => !x.Equals(type.TargetVariable)));
      }

      TimeSeriesPrognosisProblemData timeSeriesPrognosisData = new TimeSeriesPrognosisProblemData(dataset, allowedInputVars, type.TargetVariable);

      timeSeriesPrognosisData.TrainingPartition.Start = 0;
      timeSeriesPrognosisData.TrainingPartition.End = trainingPartEnd;
      timeSeriesPrognosisData.TestPartition.Start = trainingPartEnd;
      timeSeriesPrognosisData.TestPartition.End = csvFileParser.Rows;

      timeSeriesPrognosisData.Name = Path.GetFileName(path);

      return timeSeriesPrognosisData;
    }
  }
}
