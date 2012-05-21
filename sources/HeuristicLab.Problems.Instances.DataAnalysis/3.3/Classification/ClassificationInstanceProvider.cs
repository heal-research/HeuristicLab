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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public abstract class ClassificationInstanceProvider : IProblemInstanceProvider<IClassificationProblemData> {
    public IClassificationProblemData LoadData(string path) {
      NumberFormatInfo numberFormat;
      DateTimeFormatInfo dateFormat;
      char separator;
      TableFileParser.DetermineFileFormat(new FileStream(path, FileMode.Open), out numberFormat, out dateFormat, out separator);

      IClassificationProblemData claData = LoadData(new FileStream(path, FileMode.Open), numberFormat, dateFormat, separator);
      int pos = path.LastIndexOf('\\');
      if (pos < 0)
        claData.Name = path;
      else {
        pos++;
        claData.Name = path.Substring(pos, path.Length - pos);
      }

      return claData;
    }

    protected IClassificationProblemData LoadData(Stream stream, NumberFormatInfo numberFormat, DateTimeFormatInfo dateFormat, char separator) {
      TableFileParser csvFileParser = new TableFileParser();

      csvFileParser.Parse(stream, numberFormat, dateFormat, separator);

      Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
      string targetVar = csvFileParser.VariableNames.Last();
      IEnumerable<string> allowedInputVars = csvFileParser.VariableNames.Where(x => !x.Equals(targetVar));

      ClassificationProblemData claData = new ClassificationProblemData(dataset, allowedInputVars, targetVar);

      int trainingPartEnd = csvFileParser.Rows * 2 / 3;
      claData.TrainingPartition.Start = 0;
      claData.TrainingPartition.End = trainingPartEnd;
      claData.TestPartition.Start = trainingPartEnd;
      claData.TestPartition.End = csvFileParser.Rows;
      return claData;
    }

    public void SaveData(IClassificationProblemData instance, string path) {
      StringBuilder strBuilder = new StringBuilder();

      foreach (var variable in instance.InputVariables) {
        strBuilder.Append(variable + ";");
      }
      strBuilder.Remove(strBuilder.Length - 1, 1);
      strBuilder.AppendLine();

      Dataset dataset = instance.Dataset;

      for (int i = 0; i < dataset.Rows; i++) {
        for (int j = 0; j < dataset.Columns; j++) {
          strBuilder.Append(dataset.GetValue(i, j) + ";");
        }
        strBuilder.Remove(strBuilder.Length - 1, 1);
        strBuilder.AppendLine();
      }

      using (StreamWriter writer = new StreamWriter(path)) {
        writer.Write(strBuilder);
      }
    }

    public abstract IEnumerable<IDataDescriptor> GetDataDescriptors();
    public abstract IClassificationProblemData LoadData(IDataDescriptor descriptor);

    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract Uri WebLink { get; }
    public abstract string ReferencePublication { get; }
  }
}
