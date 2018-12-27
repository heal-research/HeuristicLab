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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  [Item("FilteredPreprocessingData", "Represents filtered data used for preprocessing.")]
  [StorableClass]
  public sealed class FilteredPreprocessingData : NamedItem, IFilteredPreprocessingData {

    [Storable]
    private readonly IPreprocessingData originalData;
    [Storable]
    private IPreprocessingData filteredData;

    public IPreprocessingData ActiveData {
      get { return IsFiltered ? filteredData : originalData; }
    }

    #region Constructor, Cloning & Persistence
    public FilteredPreprocessingData(IPreprocessingData preprocessingData)
      : base() {
      originalData = preprocessingData;
      filteredData = null;
    }

    private FilteredPreprocessingData(FilteredPreprocessingData original, Cloner cloner)
      : base(original, cloner) {
      originalData = original.originalData;
      filteredData = original.filteredData;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new FilteredPreprocessingData(this, cloner);
    }

    [StorableConstructor]
    private FilteredPreprocessingData(bool deserializing)
      : base(deserializing) { }
    #endregion

    #region Cells
    public bool IsCellEmpty(int columnIndex, int rowIndex) {
      return ActiveData.IsCellEmpty(columnIndex, rowIndex);
    }

    public T GetCell<T>(int columnIndex, int rowIndex) {
      return ActiveData.GetCell<T>(columnIndex, rowIndex);
    }

    public void SetCell<T>(int columnIndex, int rowIndex, T value) {
      if (IsFiltered)
        throw new InvalidOperationException("SetValues not possible while data is filtered");
      originalData.SetCell<T>(columnIndex, rowIndex, value);
    }

    public string GetCellAsString(int columnIndex, int rowIndex) {
      return ActiveData.GetCellAsString(columnIndex, rowIndex);
    }

    public IList<T> GetValues<T>(int columnIndex, bool considerSelection) {
      return ActiveData.GetValues<T>(columnIndex, considerSelection);
    }

    public void SetValues<T>(int columnIndex, IList<T> values) {
      if (IsFiltered)
        throw new InvalidOperationException("SetValues not possible while data is filtered");

      originalData.SetValues<T>(columnIndex, values);
    }

    public bool SetValue(string value, int columnIndex, int rowIndex) {
      if (IsFiltered)
        throw new InvalidOperationException("SetValue not possible while data is filtered");
      return originalData.SetValue(value, columnIndex, rowIndex);
    }

    public int Columns {
      get { return ActiveData.Columns; }
    }

    public int Rows {
      get { return ActiveData.Rows; }
    }
    #endregion

    #region Rows
    public void InsertRow(int rowIndex) {
      if (IsFiltered)
        throw new InvalidOperationException("InsertRow not possible while data is filtered");

      originalData.InsertRow(rowIndex);
    }

    public void DeleteRow(int rowIndex) {
      if (IsFiltered)
        throw new InvalidOperationException("DeleteRow not possible while data is filtered");

      originalData.DeleteRow(rowIndex);
    }

    public void DeleteRowsWithIndices(IEnumerable<int> rows) {
      if (IsFiltered)
        throw new InvalidOperationException("DeleteRowsWithIndices not possible while data is filtered");

      originalData.DeleteRowsWithIndices(rows);
    }

    public void InsertColumn<T>(string variableName, int columnIndex) {
      if (IsFiltered)
        throw new InvalidOperationException("InsertColumn not possible while data is filtered");

      originalData.InsertColumn<T>(variableName, columnIndex);
    }

    public void DeleteColumn(int columnIndex) {
      if (IsFiltered)
        throw new InvalidOperationException("DeleteColumn not possible while data is filtered");
      originalData.DeleteColumn(columnIndex);
    }

    public void RenameColumn(int columnIndex, string name) {
      if (IsFiltered)
        throw new InvalidOperationException("RenameColumn not possible while data is filtered");
      originalData.RenameColumn(columnIndex, name);
    }

    public void RenameColumns(IList<string> names) {
      if (IsFiltered)
        throw new InvalidOperationException("RenameColumns not possible while data is filtered");
      originalData.RenameColumns(names);
    }

    public bool AreAllStringColumns(IEnumerable<int> columnIndices) {
      return originalData.AreAllStringColumns(columnIndices);
    }
    #endregion

    #region Variables
    public IEnumerable<string> VariableNames {
      get { return ActiveData.VariableNames; }
    }
    public IEnumerable<string> GetDoubleVariableNames() {
      return originalData.GetDoubleVariableNames();
    }
    public string GetVariableName(int columnIndex) {
      return ActiveData.GetVariableName(columnIndex);
    }

    public int GetColumnIndex(string variableName) {
      return ActiveData.GetColumnIndex(variableName);
    }

    public bool VariableHasType<T>(int columnIndex) {
      return originalData.VariableHasType<T>(columnIndex);
    }

    public Type GetVariableType(int columnIndex) {
      return ActiveData.GetVariableType(columnIndex);
    }

    public IList<string> InputVariables {
      get { return ActiveData.InputVariables; }
    }

    public string TargetVariable {
      get { return ActiveData.TargetVariable; }
    } // optional
    #endregion

    #region Partitions
    public IntRange TrainingPartition {
      get { return originalData.TrainingPartition; }
    }

    public IntRange TestPartition {
      get { return originalData.TestPartition; }
    }
    #endregion

    #region Transformations
    public IList<ITransformation> Transformations {
      get { return originalData.Transformations; }
    }
    #endregion

    #region Validation
    public bool Validate(string value, out string errorMessage, int columnIndex) {
      return originalData.Validate(value, out errorMessage, columnIndex);
    }
    #endregion

    #region Import & Export
    public void Import(IDataAnalysisProblemData problemData) {
      if (IsFiltered)
        throw new InvalidOperationException("Import not possible while data is filtered");
      originalData.Import(problemData);
    }

    public Dataset ExportToDataset() {
      return originalData.ExportToDataset();
    }
    #endregion

    #region Selection
    public IDictionary<int, IList<int>> Selection {
      get { return originalData.Selection; }
      set { originalData.Selection = value; }
    }

    public void ClearSelection() {
      originalData.ClearSelection();
    }

    public event EventHandler SelectionChanged {
      add { originalData.SelectionChanged += value; }
      remove { originalData.SelectionChanged -= value; }
    }
    #endregion

    #region Transactions
    public event DataPreprocessingChangedEventHandler Changed {
      add { originalData.Changed += value; }
      remove { originalData.Changed -= value; }
    }

    public bool IsUndoAvailable {
      get { return IsFiltered ? false : originalData.IsUndoAvailable; }
    }

    public void Undo() {
      if (IsFiltered)
        throw new InvalidOperationException("Undo not possible while data is filtered");

      originalData.Undo();
    }

    public void InTransaction(Action action, DataPreprocessingChangedEventType type = DataPreprocessingChangedEventType.Any) {
      if (IsFiltered)
        throw new InvalidOperationException("Transaction not possible while data is filtered");
      originalData.InTransaction(action, type);
    }

    public void BeginTransaction(DataPreprocessingChangedEventType type) {
      if (IsFiltered)
        throw new InvalidOperationException("Transaction not possible while data is filtered");
      originalData.BeginTransaction(type);
    }

    public void EndTransaction() {
      originalData.EndTransaction();
    }
    #endregion

    #region Statistics
    public T GetMin<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) {
      return ActiveData.GetMin<T>(columnIndex, considerSelection, emptyValue);
    }
    public T GetMax<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) {
      return ActiveData.GetMax<T>(columnIndex, considerSelection, emptyValue);
    }
    public T GetMean<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) {
      return ActiveData.GetMean<T>(columnIndex, considerSelection, emptyValue);
    }
    public T GetMedian<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) where T : IComparable<T> {
      return ActiveData.GetMedian<T>(columnIndex, considerSelection, emptyValue);
    }
    public T GetMode<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) where T : IEquatable<T> {
      return ActiveData.GetMode<T>(columnIndex, considerSelection, emptyValue);
    }
    public T GetStandardDeviation<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) {
      return ActiveData.GetStandardDeviation<T>(columnIndex, considerSelection, emptyValue);
    }
    public T GetVariance<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) {
      return ActiveData.GetVariance<T>(columnIndex, considerSelection, emptyValue);
    }
    public T GetQuantile<T>(double alpha, int columnIndex, bool considerSelection = false, T emptyValue = default(T)) where T : IComparable<T> {
      return ActiveData.GetQuantile<T>(alpha, columnIndex, considerSelection, emptyValue);
    }
    public int GetDistinctValues<T>(int columnIndex, bool considerSelection = false) {
      return ActiveData.GetDistinctValues<T>(columnIndex, considerSelection);
    }

    public int GetMissingValueCount() {
      return ActiveData.GetMissingValueCount();
    }
    public int GetMissingValueCount(int columnIndex) {
      return ActiveData.GetMissingValueCount(columnIndex);
    }
    public int GetRowMissingValueCount(int rowIndex) {
      return ActiveData.GetRowMissingValueCount(rowIndex);
    }
    #endregion

    #region Filters
    public void SetFilter(bool[] remainingRows) {
      filteredData = (IPreprocessingData)originalData.Clone();
      filteredData.InTransaction(() => {
        var remainingIndices = Enumerable.Range(0, remainingRows.Length).Where(x => remainingRows[x]);

        foreach (var v in filteredData.VariableNames) {
          var ci = filteredData.GetColumnIndex(v);
          if (filteredData.VariableHasType<double>(ci)) {
            var values = filteredData.GetValues<double>(ci);
            var filteredValues = remainingIndices.Select(x => values[x]).ToList();
            filteredData.SetValues(ci, filteredValues);
          } else if (filteredData.VariableHasType<DateTime>(ci)) {
            var values = filteredData.GetValues<DateTime>(ci);
            var filteredValues = remainingIndices.Select(x => values[x]).ToList();
            filteredData.SetValues(ci, filteredValues);
          } else if (filteredData.VariableHasType<string>(ci)) {
            var values = filteredData.GetValues<string>(ci);
            var filteredValues = remainingIndices.Select(x => values[x]).ToList();
            filteredData.SetValues(ci, filteredValues);
          }
        }
      });
      OnFilterChanged();
    }

    public void PersistFilter() {
      originalData.InTransaction(() => {
        for (int i = 0; i < filteredData.Columns; ++i) {
          if (filteredData.VariableHasType<double>(i)) {
            originalData.SetValues<double>(i, filteredData.GetValues<double>(i));
          } else if (filteredData.VariableHasType<string>(i)) {
            originalData.SetValues<string>(i, filteredData.GetValues<string>(i));
          } else if (filteredData.VariableHasType<DateTime>(i)) {
            originalData.SetValues<DateTime>(i, filteredData.GetValues<DateTime>(i));
          } else {
            throw new ArgumentException("Data types of columns do not match");
          }
        }
      });
      ResetFilter();
    }

    public void ResetFilter() {
      filteredData = null;
      OnFilterChanged();
    }

    public bool IsFiltered {
      get { return filteredData != null; }
    }

    public event EventHandler FilterChanged;

    private void OnFilterChanged() {
      if (FilterChanged != null) {
        FilterChanged(this, new EventArgs());
      }
    }
    #endregion
  }
}
