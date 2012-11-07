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
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public abstract class DataAnalysisInstanceProvider<TData, ImportType> : ProblemInstanceProvider<TData>
    where TData : class, IDataAnalysisProblemData
    where ImportType : DataAnalysisImportType {


    public TData ImportData(string path, ImportType type, DataAnalysisCSVFormat csvFormat) {
      TableFileParser csvFileParser = new TableFileParser();
      csvFileParser.Parse(path, csvFormat.NumberFormatInfo, csvFormat.DateTimeFormatInfo, csvFormat.Separator);
      return ImportData(path, type, csvFileParser);
    }

    protected virtual TData ImportData(string path, ImportType type, TableFileParser csvFileParser) {
      throw new NotSupportedException();
    }

    protected List<IList> Shuffle(List<IList> values) {
      int count = values.First().Count;
      int[] indices = Enumerable.Range(0, count).Shuffle(new FastRandom()).ToArray();
      List<IList> shuffeledValues = new List<IList>(values.Count);
      for (int col = 0; col < values.Count; col++) {

        if (values[col] is List<double>)
          shuffeledValues.Add(new List<double>());
        else if (values[col] is List<DateTime>)
          shuffeledValues.Add(new List<DateTime>());
        else if (values[col] is List<string>)
          shuffeledValues.Add(new List<string>());
        else
          throw new InvalidOperationException();

        for (int i = 0; i < count; i++) {
          shuffeledValues[col].Add(values[col][indices[i]]);
        }
      }
      return shuffeledValues;
    }

    public override bool CanExportData {
      get { return true; }
    }
    public override void ExportData(TData instance, string path) {
      var strBuilder = new StringBuilder();
      var colSep = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
      foreach (var variable in instance.Dataset.VariableNames) {
        strBuilder.Append(variable.Replace(colSep, String.Empty) + colSep);
      }
      strBuilder.Remove(strBuilder.Length - colSep.Length, colSep.Length);
      strBuilder.AppendLine();

      var dataset = instance.Dataset;

      for (int i = 0; i < dataset.Rows; i++) {
        for (int j = 0; j < dataset.Columns; j++) {
          if (j > 0) strBuilder.Append(colSep);
          strBuilder.Append(dataset.GetValue(i, j));
        }
        strBuilder.AppendLine();
      }

      using (var writer = new StreamWriter(path)) {
        writer.Write(strBuilder);
      }
    }
  }
}
