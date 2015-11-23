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

namespace HeuristicLab.DataPreprocessing {
  public interface IStatisticsLogic {

    int GetColumnCount();
    int GetRowCount();
    int GetNumericColumnCount();
    int GetNominalColumnCount();
    int GetMissingValueCount();
    int GetMissingValueCount(int columnIndex);
    int GetRowMissingValueCount(int rowIndex);

    T GetMin<T>(int columnIndex, bool considerSelection = false) where T : IComparable<T>;
    T GetMax<T>(int columnIndex, bool considerSelection = false) where T : IComparable<T>;

    double GetMedian(int columnIndex, bool considerSelection = false);
    double GetAverage(int columnIndex, bool considerSelection = false);
    DateTime GetMedianDateTime(int columnIndex, bool considerSelection = false);
    DateTime GetAverageDateTime(int columnIndex, bool considerSelection = false);

    double GetOneQuarterPercentile(int columnIndex);
    double GetThreeQuarterPercentile(int columnIndex);
    double GetStandardDeviation(int columnIndex);
    double GetVariance(int columnIndex);
    T GetMostCommonValue<T>(int columnIndex, bool considerSelection = false);
    int GetDifferentValuesCount<T>(int columnIndex);

    bool VariableHasType<T>(int columnIndex);
    string GetColumnTypeAsString(int columnIndex);
    string GetVariableName(int columnIndex);

    event DataPreprocessingChangedEventHandler Changed;
  }
}
