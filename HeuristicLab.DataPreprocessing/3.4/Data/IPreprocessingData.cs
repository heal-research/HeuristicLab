#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  public interface IPreprocessingData : INamedItem {
    #region Cells
    bool IsCellEmpty(int columnIndex, int rowIndex);
    T GetCell<T>(int columnIndex, int rowIndex);

    void SetCell<T>(int columnIndex, int rowIndex, T value);

    string GetCellAsString(int columnIndex, int rowIndex);

    IList<T> GetValues<T>(int columnIndex, bool considerSelection = false);

    void SetValues<T>(int columnIndex, IList<T> values);
    bool SetValue(string value, int columnIndex, int rowIndex);

    int Columns { get; }
    int Rows { get; }
    #endregion

    #region Rows
    void InsertRow(int rowIndex);
    void DeleteRow(int rowIndex);
    void DeleteRowsWithIndices(IEnumerable<int> rows);
    void InsertColumn<T>(string variableName, int columnIndex);

    void DeleteColumn(int columnIndex);

    void RenameColumn(int columnIndex, string name);
    void RenameColumns(IList<string> names);

    bool AreAllStringColumns(IEnumerable<int> columnIndices);
    #endregion

    #region Variables
    IEnumerable<string> VariableNames { get; }
    IEnumerable<string> GetDoubleVariableNames();
    string GetVariableName(int columnIndex);
    int GetColumnIndex(string variableName);

    bool VariableHasType<T>(int columnIndex);
    Type GetVariableType(int columnIndex);

    IList<string> InputVariables { get; }
    string TargetVariable { get; } // optional
    #endregion

    #region Partitions
    IntRange TrainingPartition { get; }
    IntRange TestPartition { get; }
    #endregion

    #region Transformations
    IList<ITransformation> Transformations { get; }
    #endregion

    #region Validation
    bool Validate(string value, out string errorMessage, int columnIndex);
    #endregion

    #region Import & Export
    void Import(IDataAnalysisProblemData problemData);
    Dataset ExportToDataset();
    #endregion

    #region Selection
    IDictionary<int, IList<int>> Selection { get; set; }
    void ClearSelection();

    event EventHandler SelectionChanged;
    #endregion

    #region Transactions
    event DataPreprocessingChangedEventHandler Changed;

    bool IsUndoAvailable { get; }
    void Undo();
    void InTransaction(Action action, DataPreprocessingChangedEventType type = DataPreprocessingChangedEventType.Any);
    void BeginTransaction(DataPreprocessingChangedEventType type);
    void EndTransaction();
    #endregion

    #region Statistics
    T GetMin<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T));
    T GetMax<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T));
    T GetMean<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T));
    T GetMedian<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) where T : IComparable<T>;
    T GetMode<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) where T : IEquatable<T>;
    T GetStandardDeviation<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T));
    T GetVariance<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T));
    T GetQuantile<T>(double alpha, int columnIndex, bool considerSelection = false, T emptyValue = default(T)) where T : IComparable<T>;
    int GetDistinctValues<T>(int columnIndex, bool considerSelection = false);

    int GetMissingValueCount();
    int GetMissingValueCount(int columnIndex);
    int GetRowMissingValueCount(int rowIndex);
    #endregion
  }
}
