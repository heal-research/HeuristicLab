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
using System.Collections;
using System.Collections.Generic;

namespace HeuristicLab.DataPreprocessing {
  public class SearchLogic {
    private readonly ITransactionalPreprocessingData preprocessingData;
    private readonly FilterLogic filterLogic;

    private Dictionary<int, IList<int>> MissingValueIndicies { get; set; }
    private Dictionary<int, IList> ValuesWithoutNaN { get; set; }

    public IEnumerable<string> VariableNames {
      get { return preprocessingData.VariableNames; }
    }

    public int Columns {
      get { return preprocessingData.Columns; }
    }

    public int Rows {
      get { return preprocessingData.Rows; }
    }

    public SearchLogic(ITransactionalPreprocessingData thePreprocessingData, FilterLogic theFilterLogic) {
      preprocessingData = thePreprocessingData;
      filterLogic = theFilterLogic;

      MissingValueIndicies = new Dictionary<int, IList<int>>();
      ValuesWithoutNaN = new Dictionary<int, IList>();

      preprocessingData.Changed += PreprocessingData_Changed;
      filterLogic.FilterChanged += FilterLogic_FilterChanged;
    }

    void FilterLogic_FilterChanged(object sender, EventArgs e) {
      //recalculate
      for (int i = 0; i < Columns; i++) {
        MissingValueIndicies.Remove(i);
        ValuesWithoutNaN.Remove(i);
      }
    }

    void PreprocessingData_Changed(object sender, DataPreprocessingChangedEventArgs e) {
      switch (e.Type) {
        case DataPreprocessingChangedEventType.DeleteColumn:
        case DataPreprocessingChangedEventType.ChangeColumn:
          MissingValueIndicies.Remove(e.Column);
          ValuesWithoutNaN.Remove(e.Column);
          break;
        case DataPreprocessingChangedEventType.AddColumn:
          //cache does not need to be updated, will be calculated the first time it is requested
          break;
        case DataPreprocessingChangedEventType.DeleteRow:
        case DataPreprocessingChangedEventType.AddRow:
        case DataPreprocessingChangedEventType.ChangeItem:
        case DataPreprocessingChangedEventType.Any:
        case DataPreprocessingChangedEventType.Transformation:
        default:
          MissingValueIndicies = new Dictionary<int, IList<int>>();
          ValuesWithoutNaN = new Dictionary<int, IList>();
          break;
      }
    }

    public IDictionary<int, IList<int>> GetMissingValueIndices() {
      var dic = new Dictionary<int, IList<int>>();
      for (int i = 0; i < preprocessingData.Columns; ++i) {
        dic.Add(i, GetMissingValueIndices(i));
      }
      return dic;
    }

    public bool IsMissingValue(int columnIndex, int rowIndex) {
      if (preprocessingData.VariableHasType<double>(columnIndex)) {
        return double.IsNaN(preprocessingData.GetCell<double>(columnIndex, rowIndex));
      } else if (preprocessingData.VariableHasType<string>(columnIndex)) {
        return string.IsNullOrEmpty(preprocessingData.GetCell<string>(columnIndex, rowIndex));
      } else if (preprocessingData.VariableHasType<DateTime>(columnIndex)) {
        return preprocessingData.GetCell<DateTime>(columnIndex, rowIndex).Equals(DateTime.MinValue);
      } else {
        throw new ArgumentException("cell in column " + columnIndex + " and row index " + rowIndex + " contains a non supported type.");
      }
    }

    public IList<int> GetMissingValueIndices(int columnIndex) {
      if (!MissingValueIndicies.ContainsKey(columnIndex)) {
        if (preprocessingData.VariableHasType<double>(columnIndex)) {
          MissingValueIndicies[columnIndex] = GetMissingValueIndices<double>(columnIndex);
        } else if (preprocessingData.VariableHasType<string>(columnIndex)) {
          MissingValueIndicies[columnIndex] = GetMissingValueIndices<string>(columnIndex);
        } else if (preprocessingData.VariableHasType<DateTime>(columnIndex)) {
          MissingValueIndicies[columnIndex] = GetMissingValueIndices<DateTime>(columnIndex);
        } else {
          throw new ArgumentException("column " + columnIndex + " contains a non supported type.");
        }
      }
      return MissingValueIndicies[columnIndex];
    }

    private IList<int> GetMissingValueIndices<T>(int columnIndex) {
      List<int> missingIndices = new List<int>();

      for (int row = 0; row < preprocessingData.Rows; ++row) {
        if (IsMissingValue(columnIndex, row)) {
          missingIndices.Add(row);
        }
      }

      return missingIndices;
    }

    public IEnumerable<T> GetValuesWithoutNaN<T>(int columnIndex, bool considerSelection) {
      if (considerSelection) {
        var selectedRows = preprocessingData.Selection[columnIndex];

        List<T> values = new List<T>();
        foreach (var rowIdx in selectedRows) {
          if (!IsMissingValue(columnIndex, rowIdx)) {
            values.Add(preprocessingData.GetCell<T>(columnIndex, rowIdx));
          }
        }
        return values;
      } else {
        if (!ValuesWithoutNaN.ContainsKey(columnIndex)) {
          List<T> values = new List<T>();

          for (int row = 0; row < preprocessingData.Rows; ++row) {
            if (!IsMissingValue(columnIndex, row)) {
              values.Add(preprocessingData.GetCell<T>(columnIndex, row));
            }
          }

          ValuesWithoutNaN[columnIndex] = values;
        }
        return (IEnumerable<T>)ValuesWithoutNaN[columnIndex];
      }
    }
  }
}
