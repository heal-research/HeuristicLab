#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;

namespace HeuristicLab.DataPreprocessing {

  public class StatisticsLogic : IStatisticsLogic {

    private readonly ITransactionalPreprocessingData preprocessingData;
    private readonly ISearchLogic searchLogic;

    public StatisticsLogic(ITransactionalPreprocessingData thePreprocessingData, ISearchLogic theSearchLogic) {
      preprocessingData = thePreprocessingData;
      searchLogic = theSearchLogic;
    }

    public int GetColumnCount() {
      return searchLogic.Columns;
    }

    public int GetRowCount() {
      return searchLogic.Rows;
    }

    public int GetNumericColumnCount() {
      int count = 0;

      for (int i = 0; i < searchLogic.Columns; ++i) {
        if (preprocessingData.VariableHasType<double>(i)) {
          ++count;
        }
      }
      return count;
    }

    public int GetNominalColumnCount() {
      return searchLogic.Columns - GetNumericColumnCount();
    }

    public int GetMissingValueCount() {
      int count = 0;
      for (int i = 0; i < searchLogic.Columns; ++i) {
        count += GetMissingValueCount(i);
      }
      return count;
    }

    public int GetMissingValueCount(int columnIndex) {
      return searchLogic.GetMissingValueIndices(columnIndex).Count();
    }

    public T GetMin<T>(int columnIndex, bool considerSelection) where T : IComparable<T> {
      var min = default(T);
      if (preprocessingData.VariableHasType<T>(columnIndex)) {
        var values = GetValuesWithoutNaN<T>(columnIndex, considerSelection);
        if (values.Any()) {
          min = values.Min();
        }
      }
      return min;
    }

    public T GetMax<T>(int columnIndex, bool considerSelection) where T : IComparable<T> {
      var max = default(T);
      if (preprocessingData.VariableHasType<T>(columnIndex)) {
        var values = GetValuesWithoutNaN<T>(columnIndex, considerSelection);
        if (values.Any()) {
          max = values.Max();
        }
      }
      return max;
    }

    public double GetMedian(int columnIndex, bool considerSelection) {
      double median = double.NaN;
      if (preprocessingData.VariableHasType<double>(columnIndex)) {
        var values = GetValuesWithoutNaN<double>(columnIndex, considerSelection);
        if (values.Any()) {
          median = values.Median();
        }
      }
      return median;
    }

    public double GetAverage(int columnIndex, bool considerSelection) {
      double avg = double.NaN;
      if (preprocessingData.VariableHasType<double>(columnIndex)) {
        var values = GetValuesWithoutNaN<double>(columnIndex, considerSelection);
        if (values.Any()) {
          avg = values.Average();
        }
      }
      return avg;
    }

    public DateTime GetMedianDateTime(int columnIndex, bool considerSelection) {
      DateTime median = new DateTime();
      if (preprocessingData.VariableHasType<DateTime>(columnIndex)) {
        median = GetSecondsAsDateTime(GetDateTimeAsSeconds(columnIndex, considerSelection).Median());
      }
      return median;
    }

    public DateTime GetAverageDateTime(int columnIndex, bool considerSelection) {
      DateTime avg = new DateTime();
      if (preprocessingData.VariableHasType<DateTime>(columnIndex)) {
        avg = GetSecondsAsDateTime(GetDateTimeAsSeconds(columnIndex, considerSelection).Average());
      }
      return avg;
    }

    public T GetMostCommonValue<T>(int columnIndex, bool considerSelection) {
      var values = GetValuesWithoutNaN<T>(columnIndex, considerSelection);
      if (!values.Any())
        return default(T);
      return values.GroupBy(x => x)
                              .OrderByDescending(g => g.Count())
                              .Select(g => g.Key)
                              .First();
    }


    public double GetStandardDeviation(int columnIndex) {
      double stdDev = double.NaN;
      if (preprocessingData.VariableHasType<double>(columnIndex)) {
        stdDev = GetValuesWithoutNaN<double>(columnIndex).StandardDeviation();
      } else if (preprocessingData.VariableHasType<DateTime>(columnIndex)) {
        stdDev = GetDateTimeAsSeconds(columnIndex).StandardDeviation();
      }
      return stdDev;
    }

    public double GetVariance(int columnIndex) {
      double variance = double.NaN;
      if (preprocessingData.VariableHasType<double>(columnIndex)) {
        variance = GetValuesWithoutNaN<double>(columnIndex).Variance();
      } else if (preprocessingData.VariableHasType<DateTime>(columnIndex)) {
        variance = GetDateTimeAsSeconds(columnIndex).Variance();
      }
      return variance;
    }

    public int GetDifferentValuesCount<T>(int columnIndex) {
      return preprocessingData.GetValues<T>(columnIndex).GroupBy(x => x).Count();
    }

    public int GetRowMissingValueCount(int rowIndex) {
      int count = 0;
      for (int i = 0; i < preprocessingData.Columns; ++i) {
        if (searchLogic.IsMissingValue(i, rowIndex)) {
          ++count;
        }
      }
      return count;
    }

    public string GetVariableName(int columnIndex) {
      return preprocessingData.GetVariableName(columnIndex);
    }

    public bool VariableHasType<T>(int columnIndex) {
      return preprocessingData.VariableHasType<T>(columnIndex);
    }

    public string GetColumnTypeAsString(int columnIndex) {
      if (preprocessingData.VariableHasType<double>(columnIndex)) {
        return "double";
      } else if (preprocessingData.VariableHasType<string>(columnIndex)) {
        return "string";
      } else if (preprocessingData.VariableHasType<DateTime>(columnIndex)) {
        return "DateTime";
      }
      return "Unknown Type";
    }

    private IEnumerable<double> GetDateTimeAsSeconds(int columnIndex, bool considerSelection = false) {
      return GetValuesWithoutNaN<DateTime>(columnIndex, considerSelection).Select(x => (double)x.Ticks / TimeSpan.TicksPerSecond);
    }

    private IEnumerable<T> GetValuesWithoutNaN<T>(int columnIndex, bool considerSelection = false) {
      return searchLogic.GetValuesWithoutNaN<T>(columnIndex, considerSelection);
    }

    private DateTime GetSecondsAsDateTime(double seconds) {
      DateTime dateTime = new DateTime();
      return dateTime.AddSeconds(seconds);
    }

    public event DataPreprocessingChangedEventHandler Changed {
      add { preprocessingData.Changed += value; }
      remove { preprocessingData.Changed -= value; }
    }
  }
}
