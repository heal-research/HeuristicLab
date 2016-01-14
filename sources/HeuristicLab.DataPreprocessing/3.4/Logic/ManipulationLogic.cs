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
using HeuristicLab.Data;

namespace HeuristicLab.DataPreprocessing {
  public class ManipulationLogic {
    private readonly ITransactionalPreprocessingData preprocessingData;
    private readonly StatisticsLogic statisticsLogic;
    private readonly SearchLogic searchLogic;

    public IEnumerable<string> VariableNames {
      get { return preprocessingData.VariableNames; }
    }

    public ITransactionalPreprocessingData PreProcessingData {
      get { return preprocessingData; }
    }

    public ManipulationLogic(ITransactionalPreprocessingData _prepocessingData, SearchLogic theSearchLogic, StatisticsLogic theStatisticsLogic) {
      preprocessingData = _prepocessingData;
      searchLogic = theSearchLogic;
      statisticsLogic = theStatisticsLogic;
    }

    public void ReplaceIndicesByValue<T>(int columnIndex, IEnumerable<int> rowIndices, T value) {
      foreach (int index in rowIndices) {
        preprocessingData.SetCell<T>(columnIndex, index, value);
      }
    }

    public void ReplaceIndicesByAverageValue(IDictionary<int, IList<int>> cells, bool considerSelection = false) {
      preprocessingData.InTransaction(() => {
        foreach (var column in cells) {
          if (preprocessingData.VariableHasType<double>(column.Key)) {
            double average = statisticsLogic.GetAverage(column.Key, considerSelection);
            ReplaceIndicesByValue<double>(column.Key, column.Value, average);
          } else if (preprocessingData.VariableHasType<DateTime>(column.Key)) {
            DateTime average = statisticsLogic.GetAverageDateTime(column.Key, considerSelection);
            ReplaceIndicesByValue<DateTime>(column.Key, column.Value, average);
          }
        }
      });
    }

    public void ReplaceIndicesByMedianValue(IDictionary<int, IList<int>> cells, bool considerSelection = false) {
      preprocessingData.InTransaction(() => {
        foreach (var column in cells) {
          if (preprocessingData.VariableHasType<double>(column.Key)) {
            double median = statisticsLogic.GetMedian(column.Key, considerSelection);
            ReplaceIndicesByValue<double>(column.Key, column.Value, median);
          } else if (preprocessingData.VariableHasType<DateTime>(column.Key)) {
            DateTime median = statisticsLogic.GetMedianDateTime(column.Key, considerSelection);
            ReplaceIndicesByValue<DateTime>(column.Key, column.Value, median);
          }
        }
      });
    }

    public void ReplaceIndicesByRandomValue(IDictionary<int, IList<int>> cells, bool considerSelection = false) {
      preprocessingData.InTransaction(() => {
        Random r = new Random();

        foreach (var column in cells) {
          if (preprocessingData.VariableHasType<double>(column.Key)) {
            double max = statisticsLogic.GetMax<double>(column.Key, considerSelection);
            double min = statisticsLogic.GetMin<double>(column.Key, considerSelection);
            double randMultiplier = (max - min);
            foreach (int index in column.Value) {
              double rand = r.NextDouble() * randMultiplier + min;
              preprocessingData.SetCell<double>(column.Key, index, rand);
            }
          } else if (preprocessingData.VariableHasType<DateTime>(column.Key)) {
            DateTime min = statisticsLogic.GetMin<DateTime>(column.Key, considerSelection);
            DateTime max = statisticsLogic.GetMax<DateTime>(column.Key, considerSelection);
            double randMultiplier = (max - min).TotalSeconds;
            foreach (int index in column.Value) {
              double rand = r.NextDouble() * randMultiplier;
              preprocessingData.SetCell<DateTime>(column.Key, index, min.AddSeconds(rand));
            }
          }
        }
      });
    }

    public void ReplaceIndicesByLinearInterpolationOfNeighbours(IDictionary<int, IList<int>> cells) {
      preprocessingData.InTransaction(() => {
        foreach (var column in cells) {
          int countValues = 0;
          if (preprocessingData.VariableHasType<double>(column.Key)) {
            countValues = preprocessingData.GetValues<double>(column.Key).Count();
          } else if (preprocessingData.VariableHasType<DateTime>(column.Key)) {
            countValues = preprocessingData.GetValues<DateTime>(column.Key).Count();
          }

          IList<Tuple<int, int>> startEndings = GetStartAndEndingsForInterpolation(column);
          foreach (var tuple in startEndings) {
            Interpolate(column, tuple.Item1, tuple.Item2);
          }
        }
      });
    }

    private List<Tuple<int, int>> GetStartAndEndingsForInterpolation(KeyValuePair<int, IList<int>> column) {
      List<Tuple<int, int>> startEndings = new List<Tuple<int, int>>();
      var rowIndices = column.Value;
      rowIndices = rowIndices.OrderBy(x => x).ToList();
      var count = rowIndices.Count;
      int start = int.MinValue;
      for (int i = 0; i < count; ++i) {
        if (start == int.MinValue) {
          start = indexOfPrevPresentValue(column.Key, rowIndices[i]);
        }
        if (i + 1 == count || (i + 1 < count && rowIndices[i + 1] - rowIndices[i] > 1)) {
          int next = indexOfNextPresentValue(column.Key, rowIndices[i]);
          if (start > 0 && next < preprocessingData.Rows) {
            startEndings.Add(new Tuple<int, int>(start, next));
          }
          start = int.MinValue;
        }
      }
      return startEndings;
    }

    public void ReplaceIndicesBySmoothing(IDictionary<int, IList<int>> cells) {
      preprocessingData.InTransaction(() => {
        foreach (var column in cells) {
          int countValues = preprocessingData.Rows;

          foreach (int index in column.Value) {
            // dont replace first or last values
            if (index > 0 && index < countValues) {
              int prevIndex = indexOfPrevPresentValue(column.Key, index);
              int nextIndex = indexOfNextPresentValue(column.Key, index);

              // no neighbours found
              if (prevIndex < 0 || nextIndex >= countValues) {
                continue;
              }

              Interpolate(column, prevIndex, nextIndex);
            }
          }
        }
      });
    }

    private void Interpolate(KeyValuePair<int, IList<int>> column, int prevIndex, int nextIndex) {
      int valuesToInterpolate = nextIndex - prevIndex;

      if (preprocessingData.VariableHasType<double>(column.Key)) {
        double prev = preprocessingData.GetCell<double>(column.Key, prevIndex);
        double next = preprocessingData.GetCell<double>(column.Key, nextIndex);
        double interpolationStep = (next - prev) / valuesToInterpolate;

        for (int i = prevIndex; i < nextIndex; ++i) {
          double interpolated = prev + (interpolationStep * (i - prevIndex));
          preprocessingData.SetCell<double>(column.Key, i, interpolated);
        }
      } else if (preprocessingData.VariableHasType<DateTime>(column.Key)) {
        DateTime prev = preprocessingData.GetCell<DateTime>(column.Key, prevIndex);
        DateTime next = preprocessingData.GetCell<DateTime>(column.Key, nextIndex);
        double interpolationStep = (next - prev).TotalSeconds / valuesToInterpolate;

        for (int i = prevIndex; i < nextIndex; ++i) {
          DateTime interpolated = prev.AddSeconds(interpolationStep * (i - prevIndex));
          preprocessingData.SetCell<DateTime>(column.Key, i, interpolated);
        }
      }
    }

    private int indexOfPrevPresentValue(int columnIndex, int start) {
      int offset = start - 1;
      while (offset >= 0 && searchLogic.IsMissingValue(columnIndex, offset)) {
        offset--;
      }

      return offset;
    }

    private int indexOfNextPresentValue(int columnIndex, int start) {
      int offset = start + 1;
      while (offset < preprocessingData.Rows && searchLogic.IsMissingValue(columnIndex, offset)) {
        offset++;
      }

      return offset;
    }

    public void ReplaceIndicesByMostCommonValue(IDictionary<int, IList<int>> cells, bool considerSelection = false) {
      preprocessingData.InTransaction(() => {
        foreach (var column in cells) {
          if (preprocessingData.VariableHasType<double>(column.Key)) {
            ReplaceIndicesByValue<double>(column.Key, column.Value, statisticsLogic.GetMostCommonValue<double>(column.Key, considerSelection));
          } else if (preprocessingData.VariableHasType<string>(column.Key)) {
            ReplaceIndicesByValue<string>(column.Key, column.Value, statisticsLogic.GetMostCommonValue<string>(column.Key, considerSelection));
          } else if (preprocessingData.VariableHasType<DateTime>(column.Key)) {
            ReplaceIndicesByValue<DateTime>(column.Key, column.Value, statisticsLogic.GetMostCommonValue<DateTime>(column.Key, considerSelection));
          } else {
            throw new ArgumentException("column with index: " + column.Key + " contains a non supported type.");
          }
        }
      });
    }

    public void Shuffle(bool shuffleRangesSeparately) {
      Random random = new Random();
      var ranges = new[] { preprocessingData.TestPartition, preprocessingData.TrainingPartition };
      if (shuffleRangesSeparately) {
        preprocessingData.InTransaction(() => {
          // process all given ranges - e.g. TrainingPartition, TestPartition
          foreach (IntRange range in ranges) {
            List<Tuple<int, int>> shuffledIndices = new List<Tuple<int, int>>();

            // generate random indices used for shuffeling each column
            for (int i = range.End - 1; i >= range.Start; --i) {
              int rand = random.Next(range.Start, i);
              shuffledIndices.Add(new Tuple<int, int>(i, rand));
            }

            ShuffleToIndices(shuffledIndices);
          }
        });
      } else {
        preprocessingData.InTransaction(() => {
          var indices = ranges.SelectMany(x => Enumerable.Range(x.Start, x.Size)).ToList();
          var shuffledIndices = indices.OrderBy(x => random.Next());
          ShuffleToIndices(indices.Zip(shuffledIndices, (i, j) => new Tuple<int, int>(i, j)).ToList());
        });
      }
    }

    public void ReOrderToIndices(IEnumerable<int> indices) {
      List<Tuple<int, int>> indicesTuple = new List<Tuple<int, int>>();

      for (int i = 0; i < indices.Count(); ++i) {
        indicesTuple.Add(new Tuple<int, int>(i, indices.ElementAt(i)));
      }

      ReOrderToIndices(indicesTuple);
    }

    public void ReOrderToIndices(IList<System.Tuple<int, int>> indices) {
      preprocessingData.InTransaction(() => {
        for (int i = 0; i < preprocessingData.Columns; ++i) {
          if (preprocessingData.VariableHasType<double>(i)) {
            reOrderToIndices<double>(i, indices);
          } else if (preprocessingData.VariableHasType<string>(i)) {
            reOrderToIndices<string>(i, indices);
          } else if (preprocessingData.VariableHasType<DateTime>(i)) {
            reOrderToIndices<DateTime>(i, indices);
          }
        }
      });
    }

    public void ShuffleToIndices(IList<System.Tuple<int, int>> indices) {
      preprocessingData.InTransaction(() => {
        for (int i = 0; i < preprocessingData.Columns; ++i) {
          if (preprocessingData.VariableHasType<double>(i)) {
            ShuffleToIndices<double>(i, indices);
          } else if (preprocessingData.VariableHasType<string>(i)) {
            ShuffleToIndices<string>(i, indices);
          } else if (preprocessingData.VariableHasType<DateTime>(i)) {
            ShuffleToIndices<DateTime>(i, indices);
          }
        }
      });
    }

    private void reOrderToIndices<T>(int columnIndex, IList<Tuple<int, int>> indices) {

      List<T> originalData = new List<T>(preprocessingData.GetValues<T>(columnIndex));

      // process all columns equally
      foreach (Tuple<int, int> index in indices) {
        int originalIndex = index.Item1;
        int replaceIndex = index.Item2;

        T replaceValue = originalData.ElementAt<T>(replaceIndex);
        preprocessingData.SetCell<T>(columnIndex, originalIndex, replaceValue);
      }
    }

    private void ShuffleToIndices<T>(int columnIndex, IList<Tuple<int, int>> indices) {
      // process all columns equally
      foreach (Tuple<int, int> index in indices) {
        int originalIndex = index.Item1;
        int replaceIndex = index.Item2;

        T tmp = preprocessingData.GetCell<T>(columnIndex, originalIndex);
        T replaceValue = preprocessingData.GetCell<T>(columnIndex, replaceIndex);

        preprocessingData.SetCell<T>(columnIndex, originalIndex, replaceValue);
        preprocessingData.SetCell<T>(columnIndex, replaceIndex, tmp);
      }
    }

    public void ReplaceIndicesByValue(IDictionary<int, IList<int>> cells, string value) {
      preprocessingData.InTransaction(() => {
        foreach (var column in cells) {
          foreach (var rowIdx in column.Value) {
            preprocessingData.SetValue(value, column.Key, rowIdx);
          }
        }
      });
    }


    public List<int> RowsWithMissingValuesGreater(double percent) {

      List<int> rows = new List<int>();

      for (int i = 0; i < preprocessingData.Rows; ++i) {
        int missingCount = statisticsLogic.GetRowMissingValueCount(i);
        if (100f / preprocessingData.Columns * missingCount > percent) {
          rows.Add(i);
        }
      }

      return rows;
    }

    public List<int> ColumnsWithMissingValuesGreater(double percent) {

      List<int> columns = new List<int>();
      for (int i = 0; i < preprocessingData.Columns; ++i) {
        int missingCount = statisticsLogic.GetMissingValueCount(i);
        if (100f / preprocessingData.Rows * missingCount > percent) {
          columns.Add(i);
        }
      }

      return columns;
    }

    public List<int> ColumnsWithVarianceSmaller(double variance) {

      List<int> columns = new List<int>();
      for (int i = 0; i < preprocessingData.Columns; ++i) {
        if (preprocessingData.VariableHasType<double>(i) || preprocessingData.VariableHasType<DateTime>(i)) {
          double columnVariance = statisticsLogic.GetVariance(i);
          if (columnVariance < variance) {
            columns.Add(i);
          }
        }
      }
      return columns;
    }

    public void DeleteRowsWithMissingValuesGreater(double percent) {
      DeleteRows(RowsWithMissingValuesGreater(percent));
    }

    public void DeleteColumnsWithMissingValuesGreater(double percent) {
      DeleteColumns(ColumnsWithMissingValuesGreater(percent));
    }

    public void DeleteColumnsWithVarianceSmaller(double variance) {
      DeleteColumns(ColumnsWithVarianceSmaller(variance));
    }

    private void DeleteRows(List<int> rows) {
      rows.Sort();
      rows.Reverse();
      preprocessingData.InTransaction(() => {
        foreach (int row in rows) {
          preprocessingData.DeleteRow(row);
        }
      });
    }

    private void DeleteColumns(List<int> columns) {
      columns.Sort();
      columns.Reverse();
      preprocessingData.InTransaction(() => {
        foreach (int column in columns) {
          preprocessingData.DeleteColumn(column);
        }
      });
    }

  }
}
