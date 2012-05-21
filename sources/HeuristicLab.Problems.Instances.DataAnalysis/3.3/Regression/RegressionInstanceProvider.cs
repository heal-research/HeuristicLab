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
  public abstract class RegressionInstanceProvider : IProblemInstanceProvider<IRegressionProblemData> {

    public IRegressionProblemData LoadData(string path) {
      NumberFormatInfo numberFormat;
      DateTimeFormatInfo dateFormat;
      char separator;
      TableFileParser.DetermineFileFormat(path, out numberFormat, out dateFormat, out separator);

      IRegressionProblemData regData = LoadData(new FileStream(path, FileMode.Open), numberFormat, dateFormat, separator);

      int pos = path.LastIndexOf('\\');
      if (pos < 0)
        regData.Name = path;
      else {
        pos++;
        regData.Name = path.Substring(pos, path.Length - pos);
      }
      return regData;
    }

    protected IRegressionProblemData LoadData(Stream stream, NumberFormatInfo numberFormat, DateTimeFormatInfo dateFormat, char separator) {
      TableFileParser csvFileParser = new TableFileParser();

      csvFileParser.Parse(stream, numberFormat, dateFormat, separator);

      Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
      string targetVar = csvFileParser.VariableNames.Last();
      IEnumerable<string> allowedInputVars = csvFileParser.VariableNames.Where(x => !x.Equals(targetVar));

      RegressionProblemData regData = new RegressionProblemData(dataset, allowedInputVars, targetVar);

      int trainingPartEnd = csvFileParser.Rows * 2 / 3;
      regData.TrainingPartition.Start = 0;
      regData.TrainingPartition.End = trainingPartEnd;
      regData.TestPartition.Start = trainingPartEnd;
      regData.TestPartition.End = csvFileParser.Rows;

      return regData;
    }

    public void SaveData(IRegressionProblemData instance, string path) {
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
    public abstract IRegressionProblemData LoadData(IDataDescriptor descriptor);

    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract Uri WebLink { get; }
    public abstract string ReferencePublication { get; }
  }
}
