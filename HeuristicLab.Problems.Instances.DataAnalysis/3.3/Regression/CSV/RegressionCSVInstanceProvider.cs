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
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class RegressionCSVInstanceProvider : RegressionInstanceProvider {
    public override string Name {
      get { return "CSV File"; }
    }
    public override string Description {
      get {
        return "";
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
    public override IRegressionProblemData LoadData(IDataDescriptor descriptor) {
      throw new NotImplementedException();
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override IRegressionProblemData ImportData(string path) {
      TableFileParser csvFileParser = new TableFileParser();
      csvFileParser.Parse(path);

      Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
      string targetVar = dataset.DoubleVariables.Last();

      // turn of input variables that are constant in the training partition
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

      IRegressionProblemData regressionData = new RegressionProblemData(dataset, allowedInputVars, targetVar);

      var trainingPartEnd = trainingIndizes.Last();
      regressionData.TrainingPartition.Start = trainingIndizes.First();
      regressionData.TrainingPartition.End = trainingPartEnd;
      regressionData.TestPartition.Start = trainingPartEnd;
      regressionData.TestPartition.End = csvFileParser.Rows;

      regressionData.Name = Path.GetFileName(path);

      return regressionData;
    }

    protected override IRegressionProblemData ImportData(string path, RegressionImportType type, TableFileParser csvFileParser) {
      List<IList> values = csvFileParser.Values;
      if (type.Shuffle) {
        values = Shuffle(values);
      }
      Dataset dataset = new Dataset(csvFileParser.VariableNames, values);

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

      RegressionProblemData regressionData = new RegressionProblemData(dataset, allowedInputVars, type.TargetVariable);

      regressionData.TrainingPartition.Start = 0;
      regressionData.TrainingPartition.End = trainingPartEnd;
      regressionData.TestPartition.Start = trainingPartEnd;
      regressionData.TestPartition.End = csvFileParser.Rows;

      regressionData.Name = Path.GetFileName(path);

      return regressionData;
    }
  }
}
