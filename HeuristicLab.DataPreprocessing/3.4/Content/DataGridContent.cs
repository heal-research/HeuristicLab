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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Fossil;
using HeuristicLab.Random;

namespace HeuristicLab.DataPreprocessing {
  [Item("Data Grid", "Represents a data grid.")]
  [StorableType("DC6AE5CE-B0FA-4C8C-BDBB-D490C6DE4174")]
  public class DataGridContent : PreprocessingContent, IStringConvertibleMatrix, IViewShortcut {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Table; }
    }

    public int Rows {
      get { return PreprocessingData.Rows; }
      set { }
    }

    public int Columns {
      get { return PreprocessingData.Columns; }
      set { }
    }

    public IEnumerable<string> ColumnNames {
      get { return PreprocessingData.VariableNames; }
      set { }
    }

    public IEnumerable<string> RowNames {
      get { return Enumerable.Range(1, Rows).Select(n => n.ToString()); }
      set { throw new NotSupportedException(); }
    }

    public bool SortableView {
      get { return true; }
      set { throw new NotSupportedException(); }
    }

    public bool ReadOnly {
      get { return false; }
    }

    public IDictionary<int, IList<int>> Selection {
      get { return PreprocessingData.Selection; }
      set { PreprocessingData.Selection = value; }
    }

    #region Constructor, Cloning & Persistence
    public DataGridContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
    }

    public DataGridContent(DataGridContent original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataGridContent(this, cloner);
    }

    [StorableConstructor]
    protected DataGridContent(StorableConstructorFlag _) : base(_) { }
    #endregion

    public void DeleteRows(IEnumerable<int> rows) {
      PreprocessingData.DeleteRowsWithIndices(rows);
    }

    public void DeleteColumn(int column) {
      PreprocessingData.DeleteColumn(column);
    }

    public bool Validate(string value, out string errorMessage, int columnIndex) {
      return PreprocessingData.Validate(value, out errorMessage, columnIndex);
    }

    public string GetValue(int rowIndex, int columnIndex) {
      return PreprocessingData.GetCellAsString(columnIndex, rowIndex);
    }

    public bool SetValue(string value, int rowIndex, int columnIndex) {
      return PreprocessingData.SetValue(value, columnIndex, rowIndex);
    }

    public event DataPreprocessingChangedEventHandler Changed {
      add { PreprocessingData.Changed += value; }
      remove { PreprocessingData.Changed -= value; }
    }

    #region unused stuff/not implemented but necessary due to IStringConvertibleMatrix
#pragma warning disable 0067
    // Is not used since DataGridContentView overrides dataGridView_CellValidating and uses 
    // DataGridLogic#Validate(string value, out string errorMessage, int columnIndex)
    public bool Validate(string value, out string errorMessage) {
      errorMessage = string.Empty;
      return true;
    }

    public event EventHandler ColumnsChanged;
    public event EventHandler RowsChanged;
    public event EventHandler ColumnNamesChanged;
    public event EventHandler RowNamesChanged;
    public event EventHandler SortableViewChanged;
    public event EventHandler<EventArgs<int, int>> ItemChanged;
    public event EventHandler Reset;
#pragma warning restore 0067
    #endregion

    #region Manipulations
    private void ReplaceIndicesByValue(IDictionary<int, IList<int>> cells, Func<int, double> doubleAggregator = null,
      Func<int, DateTime> dateTimeAggregator = null, Func<int, string> stringAggregator = null) {
      PreprocessingData.InTransaction(() => {
        foreach (var column in cells) {
          if (doubleAggregator != null && PreprocessingData.VariableHasType<double>(column.Key)) {
            var value = doubleAggregator(column.Key);
            foreach (int index in column.Value)
              PreprocessingData.SetCell<double>(column.Key, index, value);
          } else if (dateTimeAggregator != null && PreprocessingData.VariableHasType<DateTime>(column.Key)) {
            var value = dateTimeAggregator(column.Key);
            foreach (int index in column.Value)
              PreprocessingData.SetCell<DateTime>(column.Key, index, value);
          } else if (stringAggregator != null && PreprocessingData.VariableHasType<string>(column.Key)) {
            var value = stringAggregator(column.Key);
            foreach (int index in column.Value)
              PreprocessingData.SetCell<string>(column.Key, index, value);
          }
        }
      });
    }

    private void ReplaceIndicesByValues(IDictionary<int, IList<int>> cells, Func<int, IEnumerable<double>> doubleAggregator = null,
      Func<int, IEnumerable<DateTime>> dateTimeAggregator = null, Func<int, IEnumerable<string>> stringAggregator = null) {
      PreprocessingData.InTransaction(() => {
        foreach (var column in cells) {
          if (doubleAggregator != null && PreprocessingData.VariableHasType<double>(column.Key)) {
            var values = doubleAggregator(column.Key);
            foreach (var pair in column.Value.Zip(values, (row, value) => new { row, value }))
              PreprocessingData.SetCell<double>(column.Key, pair.row, pair.value);
          } else if (dateTimeAggregator != null && PreprocessingData.VariableHasType<DateTime>(column.Key)) {
            var values = dateTimeAggregator(column.Key);
            foreach (var pair in column.Value.Zip(values, (row, value) => new { row, value }))
              PreprocessingData.SetCell<DateTime>(column.Key, pair.row, pair.value);
          } else if (stringAggregator != null && PreprocessingData.VariableHasType<string>(column.Key)) {
            var values = stringAggregator(column.Key);
            foreach (var pair in column.Value.Zip(values, (row, value) => new { row, value }))
              PreprocessingData.SetCell<string>(column.Key, pair.row, pair.value);
          }
        }
      });
    }

    public void ReplaceIndicesByMean(IDictionary<int, IList<int>> cells, bool considerSelection = false) {
      ReplaceIndicesByValue(cells,
        col => PreprocessingData.GetMean<double>(col, considerSelection),
        col => PreprocessingData.GetMean<DateTime>(col, considerSelection));
    }

    public void ReplaceIndicesByMedianValue(IDictionary<int, IList<int>> cells, bool considerSelection = false) {
      ReplaceIndicesByValue(cells,
        col => PreprocessingData.GetMedian<double>(col, considerSelection),
        col => PreprocessingData.GetMedian<DateTime>(col, considerSelection));
    }

    public void ReplaceIndicesByMode(IDictionary<int, IList<int>> cells, bool considerSelection = false) {
      ReplaceIndicesByValue(cells,
        col => PreprocessingData.GetMode<double>(col, considerSelection),
        col => PreprocessingData.GetMode<DateTime>(col, considerSelection),
        col => PreprocessingData.GetMode<string>(col, considerSelection));
    }

    public void ReplaceIndicesByRandomValue(IDictionary<int, IList<int>> cells, bool considerSelection = false) {
      var rand = new FastRandom();
      ReplaceIndicesByValues(cells,
        col => {
          double min = PreprocessingData.GetMin<double>(col, considerSelection);
          double max = PreprocessingData.GetMax<double>(col, considerSelection);
          double range = max - min;
          return cells[col].Select(_ => rand.NextDouble() * range + min);
        },
        col => {
          var min = PreprocessingData.GetMin<DateTime>(col, considerSelection);
          var max = PreprocessingData.GetMax<DateTime>(col, considerSelection);
          double range = (max - min).TotalSeconds;
          return cells[col].Select(_ => min + TimeSpan.FromSeconds(rand.NextDouble() * range));
        });
    }

    public void ReplaceIndicesByString(IDictionary<int, IList<int>> cells, string value) {
      PreprocessingData.InTransaction(() => {
        foreach (var column in cells) {
          foreach (var rowIdx in column.Value) {
            PreprocessingData.SetValue(value, column.Key, rowIdx);
          }
        }
      });
    }


    public void ReplaceIndicesByLinearInterpolationOfNeighbours(IDictionary<int, IList<int>> cells) {
      PreprocessingData.InTransaction(() => {
        foreach (var column in cells) {
          IList<Tuple<int, int>> startEndings = GetStartAndEndingsForInterpolation(column);
          foreach (var tuple in startEndings) {
            Interpolate(column, tuple.Item1, tuple.Item2);
          }
        }
      });
    }

    private List<Tuple<int, int>> GetStartAndEndingsForInterpolation(KeyValuePair<int, IList<int>> column) {
      var startEndings = new List<Tuple<int, int>>();
      var rowIndices = column.Value.OrderBy(x => x).ToList();
      var count = rowIndices.Count;
      int start = int.MinValue;
      for (int i = 0; i < count; ++i) {
        if (start == int.MinValue) {
          start = IndexOfPrevPresentValue(column.Key, rowIndices[i]);
        }
        if (i + 1 == count || (i + 1 < count && rowIndices[i + 1] - rowIndices[i] > 1)) {
          int next = IndexOfNextPresentValue(column.Key, rowIndices[i]);
          if (start > 0 && next < PreprocessingData.Rows) {
            startEndings.Add(new Tuple<int, int>(start, next));
          }
          start = int.MinValue;
        }
      }
      return startEndings;
    }

    private void Interpolate(KeyValuePair<int, IList<int>> column, int prevIndex, int nextIndex) {
      int valuesToInterpolate = nextIndex - prevIndex;

      if (PreprocessingData.VariableHasType<double>(column.Key)) {
        double prev = PreprocessingData.GetCell<double>(column.Key, prevIndex);
        double next = PreprocessingData.GetCell<double>(column.Key, nextIndex);
        double interpolationStep = (next - prev) / valuesToInterpolate;

        for (int i = prevIndex; i < nextIndex; ++i) {
          double interpolated = prev + (interpolationStep * (i - prevIndex));
          PreprocessingData.SetCell<double>(column.Key, i, interpolated);
        }
      } else if (PreprocessingData.VariableHasType<DateTime>(column.Key)) {
        DateTime prev = PreprocessingData.GetCell<DateTime>(column.Key, prevIndex);
        DateTime next = PreprocessingData.GetCell<DateTime>(column.Key, nextIndex);
        double interpolationStep = (next - prev).TotalSeconds / valuesToInterpolate;

        for (int i = prevIndex; i < nextIndex; ++i) {
          DateTime interpolated = prev.AddSeconds(interpolationStep * (i - prevIndex));
          PreprocessingData.SetCell<DateTime>(column.Key, i, interpolated);
        }
      }
    }

    private int IndexOfPrevPresentValue(int columnIndex, int start) {
      int offset = start - 1;
      while (offset >= 0 && PreprocessingData.IsCellEmpty(columnIndex, offset)) {
        offset--;
      }

      return offset;
    }

    private int IndexOfNextPresentValue(int columnIndex, int start) {
      int offset = start + 1;
      while (offset < PreprocessingData.Rows && PreprocessingData.IsCellEmpty(columnIndex, offset)) {
        offset++;
      }

      return offset;
    }

    public void Shuffle(bool shuffleRangesSeparately) {
      var random = new FastRandom();

      if (shuffleRangesSeparately) {
        var ranges = new[] { PreprocessingData.TestPartition, PreprocessingData.TrainingPartition };
        PreprocessingData.InTransaction(() => {
          // process all given ranges - e.g. TrainingPartition, TestPartition
          foreach (IntRange range in ranges) {
            var indices = Enumerable.Range(0, PreprocessingData.Rows).ToArray();
            var shuffledIndices = Enumerable.Range(range.Start, range.Size).Shuffle(random).ToArray();
            for (int i = range.Start, j = 0; i < range.End; i++, j++)
              indices[i] = shuffledIndices[j];

            ReOrderToIndices(indices);
          }
        });

      } else {
        PreprocessingData.InTransaction(() => {
          var indices = Enumerable.Range(0, PreprocessingData.Rows).ToArray();
          indices.ShuffleInPlace(random);
          ReOrderToIndices(indices);
        });
      }
    }

    public void ReOrderToIndices(int[] indices) {
      PreprocessingData.InTransaction(() => {
        for (int i = 0; i < PreprocessingData.Columns; ++i) {
          if (PreprocessingData.VariableHasType<double>(i))
            ReOrderToIndices<double>(i, indices);
          else if (PreprocessingData.VariableHasType<string>(i))
            ReOrderToIndices<string>(i, indices);
          else if (PreprocessingData.VariableHasType<DateTime>(i))
            ReOrderToIndices<DateTime>(i, indices);
        }
      });
    }

    private void ReOrderToIndices<T>(int columnIndex, int[] indices) {
      var originalData = new List<T>(PreprocessingData.GetValues<T>(columnIndex));
      if (indices.Length != originalData.Count) throw new InvalidOperationException("The number of provided indices does not match the values.");

      for (int i = 0; i < indices.Length; i++) {
        T newValue = originalData[indices[i]];
        PreprocessingData.SetCell<T>(columnIndex, i, newValue);
      }
    }
    #endregion
  }
}
