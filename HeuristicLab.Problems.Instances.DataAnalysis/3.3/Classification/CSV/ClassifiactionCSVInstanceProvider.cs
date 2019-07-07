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
  public class ClassificationCSVInstanceProvider : ClassificationInstanceProvider {
    public override string Name {
      get { return "CSV File"; }
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

    public override IClassificationProblemData LoadData(IDataDescriptor descriptor) {
      throw new NotImplementedException();
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override IClassificationProblemData ImportData(string path) {
      TableFileParser csvFileParser = new TableFileParser();

      csvFileParser.Parse(path, csvFileParser.AreColumnNamesInFirstLine(path));

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

      ClassificationProblemData classificationData = new ClassificationProblemData(dataset, allowedInputVars, targetVar);

      int trainingPartEnd = trainingIndizes.Last();
      classificationData.TrainingPartition.Start = trainingIndizes.First();
      classificationData.TrainingPartition.End = trainingPartEnd;
      classificationData.TestPartition.Start = trainingPartEnd;
      classificationData.TestPartition.End = csvFileParser.Rows;

      classificationData.Name = Path.GetFileName(path);

      return classificationData;
    }

    protected override IClassificationProblemData ImportData(string path, ClassificationImportType type, TableFileParser csvFileParser) {
      int trainingPartEnd = (csvFileParser.Rows * type.TrainingPercentage) / 100;
      List<IList> values = csvFileParser.Values;
      if (type.Shuffle) {
        values = Shuffle(values);
        if (type.UniformlyDistributeClasses) {
          values = Shuffle(values, csvFileParser.VariableNames.ToList().FindIndex(x => x.Equals(type.TargetVariable)),
                           type.TrainingPercentage, out trainingPartEnd);
        }
      }

      Dataset dataset = new Dataset(csvFileParser.VariableNames, values);

      // turn of input variables that are constant in the training partition
      var allowedInputVars = new List<string>();
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

      ClassificationProblemData classificationData = new ClassificationProblemData(dataset, allowedInputVars, type.TargetVariable);

      classificationData.TrainingPartition.Start = 0;
      classificationData.TrainingPartition.End = trainingPartEnd;
      classificationData.TestPartition.Start = trainingPartEnd;
      classificationData.TestPartition.End = csvFileParser.Rows;

      classificationData.Name = Path.GetFileName(path);

      return classificationData;
    }

    protected List<IList> Shuffle(List<IList> values, int target, int trainingPercentage, out int trainingPartEnd) {
      IList targetValues = values[target];
      var group = targetValues.Cast<double>().GroupBy(x => x).Select(g => new { Key = g.Key, Count = g.Count() }).ToList();
      Dictionary<double, double> taken = new Dictionary<double, double>();
      foreach (var classCount in group) {
        taken[classCount.Key] = (classCount.Count * trainingPercentage) / 100.0;
      }

      List<IList> training = GetListOfIListCopy(values);
      List<IList> test = GetListOfIListCopy(values);

      for (int i = 0; i < targetValues.Count; i++) {
        if (taken[(double)targetValues[i]] > 0) {
          AddRow(training, values, i);
          taken[(double)targetValues[i]]--;
        } else {
          AddRow(test, values, i);
        }
      }

      trainingPartEnd = training.First().Count;

      for (int i = 0; i < training.Count; i++) {
        for (int j = 0; j < test[i].Count; j++) {
          training[i].Add(test[i][j]);
        }
      }

      return training;
    }

    private void AddRow(List<IList> destination, List<IList> source, int index) {
      for (int i = 0; i < source.Count; i++) {
        destination[i].Add(source[i][index]);
      }
    }

    private List<IList> GetListOfIListCopy(List<IList> values) {
      List<IList> newList = new List<IList>(values.Count);
      foreach (IList t in values) {
        if (t is List<double>)
          newList.Add(new List<double>());
        else if (t is List<DateTime>)
          newList.Add(new List<DateTime>());
        else if (t is List<string>)
          newList.Add(new List<string>());
        else
          throw new InvalidOperationException();
      }
      return newList;
    }
  }
}
