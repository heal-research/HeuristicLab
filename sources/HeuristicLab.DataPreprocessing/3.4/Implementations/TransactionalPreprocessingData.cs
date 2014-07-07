#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  [Item("PreprocessingData", "Represents data used for preprocessing.")]
  public class TransactionalPreprocessingData : PreprocessingData, ITransactionalPreprocessingData {

    private class Snapshot {
      public IList<IList> VariableValues { get; set; }
      public IList<string> VariableNames { get; set; }

      public IntRange TrainingPartition { get; set; }
      public IntRange TestPartition { get; set; }
      public IList<ITransformation> Transformations { get; set; }
      public DataPreprocessingChangedEventType ChangedType { get; set; }

      public int ChangedColumn { get; set; }
      public int ChangedRow { get; set; }
    }

    private const int MAX_UNDO_DEPTH = 5;

    private readonly IList<Snapshot> undoHistory = new List<Snapshot>();
    private readonly Stack<DataPreprocessingChangedEventType> eventStack = new Stack<DataPreprocessingChangedEventType>();

    public bool IsInTransaction { get { return eventStack.Count > 0; } }

    public TransactionalPreprocessingData(IDataAnalysisProblemData problemData)
      : base(problemData) {
    }

    protected TransactionalPreprocessingData(TransactionalPreprocessingData original, Cloner cloner)
      : base(original, cloner) {
    }

    private void SaveSnapshot(DataPreprocessingChangedEventType changedType, int column, int row) {
      if (IsInTransaction) return;

      var currentSnapshot = new Snapshot {
        VariableValues = CopyVariableValues(variableValues),
        VariableNames = new List<string>(variableNames),
        TrainingPartition = new IntRange(TrainingPartition.Start, TrainingPartition.End),
        TestPartition = new IntRange(TestPartition.Start, TestPartition.End),
        Transformations = new List<ITransformation>(transformations),
        ChangedType = changedType,
        ChangedColumn = column,
        ChangedRow = row
      };

      if (undoHistory.Count >= MAX_UNDO_DEPTH)
        undoHistory.RemoveAt(0);

      undoHistory.Add(currentSnapshot);
    }

    #region NamedItem abstract Member Implementations

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TransactionalPreprocessingData(this, cloner);
    }

    #endregion

    #region Overridden IPreprocessingData Members

    public override T GetCell<T>(int columnIndex, int rowIndex) {
      return (T)variableValues[columnIndex][rowIndex];
    }

    public override void SetCell<T>(int columnIndex, int rowIndex, T value) {
      SaveSnapshot(DataPreprocessingChangedEventType.ChangeItem, columnIndex, rowIndex);
      variableValues[columnIndex][rowIndex] = value;
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.ChangeItem, columnIndex, rowIndex);
    }

    public override string GetCellAsString(int columnIndex, int rowIndex) {
      return variableValues[columnIndex][rowIndex].ToString();
    }

    public override string GetVariableName(int columnIndex) {
      return variableNames[columnIndex];
    }

    public override int GetColumnIndex(string variableName) {
      return variableNames.IndexOf(variableName);
    }

    public override bool IsType<T>(int columnIndex) {
      return variableValues[columnIndex] is List<T>;
    }

    [Obsolete("use the index based variant, is faster")]
    public override IList<T> GetValues<T>(string variableName, bool considerSelection) {
      return GetValues<T>(GetColumnIndex(variableName), considerSelection);
    }

    public override IList<T> GetValues<T>(int columnIndex, bool considerSelection) {
      if (considerSelection) {
        var list = new List<T>();
        foreach (var rowIdx in selection[columnIndex]) {
          list.Add((T)variableValues[columnIndex][rowIdx]);
        }
        return list;
      } else {
        return (IList<T>)variableValues[columnIndex];
      }
    }

    public override void SetValues<T>(int columnIndex, IList<T> values) {
      SaveSnapshot(DataPreprocessingChangedEventType.ChangeColumn, columnIndex, -1);
      if (IsType<T>(columnIndex)) {
        variableValues[columnIndex] = (IList)values;
      } else {
        throw new ArgumentException("The datatype of column " + columnIndex + " must be of type " + variableValues[columnIndex].GetType().Name + " but was " + typeof(T).Name);
      }
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.ChangeColumn, columnIndex, -1);
    }

    public override bool SetValue(string value, int columnIndex, int rowIndex) {
      bool valid = false;
      if (IsType<double>(columnIndex)) {
        double val;
        valid = double.TryParse(value, out val);
        SetValueIfValid(columnIndex, rowIndex, valid, val);
      } else if (IsType<string>(columnIndex)) {
        valid = value != null;
        SetValueIfValid(columnIndex, rowIndex, valid, value);
      } else if (IsType<DateTime>(columnIndex)) {
        DateTime date;
        valid = DateTime.TryParse(value, out date);
        SetValueIfValid(columnIndex, rowIndex, valid, date);
      } else {
        throw new ArgumentException("column " + columnIndex + " contains a non supported type.");
      }

      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.ChangeColumn, columnIndex, -1);

      return valid;
    }

    public override bool Validate(string value, out string errorMessage, int columnIndex) {
      if (columnIndex < 0 || columnIndex > VariableNames.Count()) {
        throw new ArgumentOutOfRangeException("column index is out of range");
      }

      bool valid = false;
      errorMessage = string.Empty;
      if (IsType<double>(columnIndex)) {
        double val;
        valid = double.TryParse(value, out val);
        if (!valid) {
          errorMessage = "Invalid Value (Valid Value Format: \"" + FormatPatterns.GetDoubleFormatPattern() + "\")";
        }
      } else if (IsType<string>(columnIndex)) {
        valid = value != null;
        if (!valid) {
          errorMessage = "Invalid Value (string must not be null)";
        }
      } else if (IsType<DateTime>(columnIndex)) {
        DateTime date;
        valid = DateTime.TryParse(value, out date);
        if (!valid) {
          errorMessage = "Invalid Value (Valid Value Format: \"" + CultureInfo.CurrentCulture.DateTimeFormat + "\"";
        }
      } else {
        throw new ArgumentException("column " + columnIndex + " contains a non supported type.");
      }

      return valid;
    }

    private void SetValueIfValid<T>(int columnIndex, int rowIndex, bool valid, T value) {
      if (valid)
        SetCell<T>(columnIndex, rowIndex, value);
    }

    public override bool AreAllStringColumns(IEnumerable<int> columnIndices) {
      return columnIndices.All(x => IsType<string>(x));
    }

    public override void DeleteRowsWithIndices(IEnumerable<int> rows) {
      SaveSnapshot(DataPreprocessingChangedEventType.AddRow, -1, -1);
      foreach (int rowIndex in rows.OrderByDescending(x => x)) {
        foreach (IList column in variableValues) {
          column.RemoveAt(rowIndex);
        }
      }
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.DeleteRow, -1, -1);
    }

    public override void InsertRow(int rowIndex) {
      SaveSnapshot(DataPreprocessingChangedEventType.DeleteRow, -1, rowIndex);
      foreach (IList column in variableValues) {
        Type type = column.GetType().GetGenericArguments()[0];
        column.Insert(rowIndex, type.IsValueType ? Activator.CreateInstance(type) : null);
      }
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.AddRow, -1, rowIndex);
    }

    public override void DeleteRow(int rowIndex) {
      SaveSnapshot(DataPreprocessingChangedEventType.AddRow, -1, rowIndex);
      foreach (IList column in variableValues) {
        column.RemoveAt(rowIndex);
      }
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.DeleteRow, -1, rowIndex);
    }

    public override void InsertColumn<T>(string variableName, int columnIndex) {
      SaveSnapshot(DataPreprocessingChangedEventType.DeleteColumn, columnIndex, -1);
      variableValues.Insert(columnIndex, new List<T>(Rows));
      variableNames.Insert(columnIndex, variableName);
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.AddColumn, columnIndex, -1);
    }

    public override void DeleteColumn(int columnIndex) {
      SaveSnapshot(DataPreprocessingChangedEventType.AddColumn, columnIndex, -1);
      variableValues.RemoveAt(columnIndex);
      variableNames.RemoveAt(columnIndex);
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.DeleteColumn, columnIndex, -1);
    }

    public override Dataset ExportToDataset() {
      IList<IList> values = new List<IList>();

      for (int i = 0; i < Columns; ++i) {
        values.Add(variableValues[i]);
      }

      var dataset = new Dataset(variableNames, values);
      return dataset;
    }

    public override void ClearSelection() {
      Selection = new Dictionary<int, IList<int>>();
    }

    public override event EventHandler SelectionChanged;

    protected override void OnSelectionChanged() {
      var listeners = SelectionChanged;
      if (listeners != null) listeners(this, EventArgs.Empty);
    }


    #endregion

    #region TransactionalPreprocessingData members

    public bool IsUndoAvailable {
      get { return undoHistory.Count > 0; }
    }

    public void Undo() {
      if (IsUndoAvailable) {
        Snapshot previousSnapshot = undoHistory[undoHistory.Count - 1];
        variableValues = previousSnapshot.VariableValues;
        variableNames = previousSnapshot.VariableNames;
        TrainingPartition = previousSnapshot.TrainingPartition;
        TestPartition = previousSnapshot.TestPartition;
        transformations = previousSnapshot.Transformations;
        undoHistory.Remove(previousSnapshot);
        OnChanged(previousSnapshot.ChangedType,
          previousSnapshot.ChangedColumn,
          previousSnapshot.ChangedRow);
      }
    }

    public void InTransaction(Action action, DataPreprocessingChangedEventType type = DataPreprocessingChangedEventType.Any) {
      BeginTransaction(type);
      action();
      EndTransaction();
    }

    public void BeginTransaction(DataPreprocessingChangedEventType type) {
      SaveSnapshot(type, -1, -1);
      eventStack.Push(type);
    }

    public void EndTransaction() {
      if (eventStack.Count == 0)
        throw new InvalidOperationException("There is no open transaction that can be ended.");

      var @event = eventStack.Pop();
      OnChanged(@event, -1, -1);
    }

    #endregion
  }
}
