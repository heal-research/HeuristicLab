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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Fossil;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {

  [Item("PreprocessingData", "Represents data used for preprocessing.")]
  [StorableType("DDF0FC89-E180-47EB-B96E-CBD9E15D697E")]
  public class PreprocessingData : NamedItem, IPreprocessingData {

    [Storable]
    protected IList<IList> variableValues;
    [Storable]
    protected IList<string> variableNames;

    #region Constructor, Cloning & Persistence
    public PreprocessingData(IDataAnalysisProblemData problemData)
      : base() {
      Name = "Preprocessing Data";

      Transformations = new List<ITransformation>();
      selection = new Dictionary<int, IList<int>>();

      Import(problemData);

      RegisterEventHandler();
    }

    protected PreprocessingData(PreprocessingData original, Cloner cloner)
      : base(original, cloner) {
      variableValues = CopyVariableValues(original.variableValues);
      variableNames = new List<string>(original.variableNames);
      TrainingPartition = (IntRange)original.TrainingPartition.Clone(cloner);
      TestPartition = (IntRange)original.TestPartition.Clone(cloner);
      Transformations = new List<ITransformation>(original.Transformations.Select(cloner.Clone));

      InputVariables = new List<string>(original.InputVariables);
      TargetVariable = original.TargetVariable;

      RegisterEventHandler();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PreprocessingData(this, cloner);
    }

    [StorableConstructor]
    protected PreprocessingData(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandler();
    }

    private void RegisterEventHandler() {
      Changed += (s, e) => {
        switch (e.Type) {
          case DataPreprocessingChangedEventType.DeleteRow:
          case DataPreprocessingChangedEventType.Any:
          case DataPreprocessingChangedEventType.Transformation:
            int maxRowIndex = Math.Max(0, Rows);
            TrainingPartition.Start = Math.Min(TrainingPartition.Start, maxRowIndex);
            TrainingPartition.End = Math.Min(TrainingPartition.End, maxRowIndex);
            TestPartition.Start = Math.Min(TestPartition.Start, maxRowIndex);
            TestPartition.End = Math.Min(TestPartition.End, maxRowIndex);
            break;
        }
      };
    }
    #endregion

    #region Cells
    public bool IsCellEmpty(int columnIndex, int rowIndex) {
      var value = variableValues[columnIndex][rowIndex];
      return IsMissingValue(value);
    }

    public T GetCell<T>(int columnIndex, int rowIndex) {
      return (T)variableValues[columnIndex][rowIndex];
    }

    public void SetCell<T>(int columnIndex, int rowIndex, T value) {
      SaveSnapshot(DataPreprocessingChangedEventType.ChangeItem, columnIndex, rowIndex);

      for (int i = Rows; i <= rowIndex; i++)
        InsertRow(i);
      for (int i = Columns; i <= columnIndex; i++)
        InsertColumn<T>(i.ToString(), i);

      variableValues[columnIndex][rowIndex] = value;
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.ChangeItem, columnIndex, rowIndex);
    }

    public string GetCellAsString(int columnIndex, int rowIndex) {
      return variableValues[columnIndex][rowIndex].ToString();
    }

    public IList<T> GetValues<T>(int columnIndex, bool considerSelection) {
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

    public void SetValues<T>(int columnIndex, IList<T> values) {
      SaveSnapshot(DataPreprocessingChangedEventType.ChangeColumn, columnIndex, -1);
      if (VariableHasType<T>(columnIndex)) {
        variableValues[columnIndex] = (IList)values;
      } else {
        throw new ArgumentException("The datatype of column " + columnIndex + " must be of type " + variableValues[columnIndex].GetType().Name + " but was " + typeof(T).Name);
      }
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.ChangeColumn, columnIndex, -1);
    }

    public bool SetValue(string value, int columnIndex, int rowIndex) {
      bool valid = false;
      if (VariableHasType<double>(columnIndex)) {
        double val;
        if (string.IsNullOrWhiteSpace(value)) {
          val = double.NaN;
          valid = true;
        } else {
          valid = double.TryParse(value, out val);
        }
        if (valid)
          SetCell(columnIndex, rowIndex, val);
      } else if (VariableHasType<string>(columnIndex)) {
        valid = value != null;
        if (valid)
          SetCell(columnIndex, rowIndex, value);
      } else if (VariableHasType<DateTime>(columnIndex)) {
        DateTime date;
        valid = DateTime.TryParse(value, out date);
        if (valid)
          SetCell(columnIndex, rowIndex, date);
      } else {
        throw new ArgumentException("column " + columnIndex + " contains a non supported type.");
      }

      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.ChangeColumn, columnIndex, -1);

      return valid;
    }

    public int Columns {
      get { return variableNames.Count; }
    }

    public int Rows {
      get { return variableValues.Count > 0 ? variableValues[0].Count : 0; }
    }

    public static bool IsMissingValue(object value) {
      if (value is double) return double.IsNaN((double)value);
      if (value is string) return string.IsNullOrEmpty((string)value);
      if (value is DateTime) return ((DateTime)value).Equals(DateTime.MinValue);
      throw new ArgumentException();
    }
    #endregion

    #region Rows
    public void InsertRow(int rowIndex) {
      SaveSnapshot(DataPreprocessingChangedEventType.DeleteRow, -1, rowIndex);
      foreach (IList column in variableValues) {
        Type type = column.GetType().GetGenericArguments()[0];
        column.Insert(rowIndex, type.IsValueType ? Activator.CreateInstance(type) : null);
      }
      if (TrainingPartition.Start <= rowIndex && rowIndex <= TrainingPartition.End) {
        TrainingPartition.End++;
        if (TrainingPartition.End <= TestPartition.Start) {
          TestPartition.Start++;
          TestPartition.End++;
        }
      } else if (TestPartition.Start <= rowIndex && rowIndex <= TestPartition.End) {
        TestPartition.End++;
        if (TestPartition.End <= TrainingPartition.Start) {
          TestPartition.Start++;
          TestPartition.End++;
        }
      }
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.AddRow, -1, rowIndex);
    }
    public void DeleteRow(int rowIndex) {
      SaveSnapshot(DataPreprocessingChangedEventType.AddRow, -1, rowIndex);
      foreach (IList column in variableValues) {
        column.RemoveAt(rowIndex);
      }
      if (TrainingPartition.Start <= rowIndex && rowIndex <= TrainingPartition.End) {
        TrainingPartition.End--;
        if (TrainingPartition.End <= TestPartition.Start) {
          TestPartition.Start--;
          TestPartition.End--;
        }
      } else if (TestPartition.Start <= rowIndex && rowIndex <= TestPartition.End) {
        TestPartition.End--;
        if (TestPartition.End <= TrainingPartition.Start) {
          TestPartition.Start--;
          TestPartition.End--;
        }
      }
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.DeleteRow, -1, rowIndex);
    }
    public void DeleteRowsWithIndices(IEnumerable<int> rows) {
      SaveSnapshot(DataPreprocessingChangedEventType.AddRow, -1, -1);
      foreach (int rowIndex in rows.OrderByDescending(x => x)) {
        foreach (IList column in variableValues) {
          column.RemoveAt(rowIndex);
        }
        if (TrainingPartition.Start <= rowIndex && rowIndex <= TrainingPartition.End) {
          TrainingPartition.End--;
          if (TrainingPartition.End <= TestPartition.Start) {
            TestPartition.Start--;
            TestPartition.End--;
          }
        } else if (TestPartition.Start <= rowIndex && rowIndex <= TestPartition.End) {
          TestPartition.End--;
          if (TestPartition.End <= TrainingPartition.Start) {
            TestPartition.Start--;
            TestPartition.End--;
          }
        }
      }
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.DeleteRow, -1, -1);
    }

    public void InsertColumn<T>(string variableName, int columnIndex) {
      SaveSnapshot(DataPreprocessingChangedEventType.DeleteColumn, columnIndex, -1);
      variableValues.Insert(columnIndex, new List<T>(Enumerable.Repeat(default(T), Rows)));
      variableNames.Insert(columnIndex, variableName);
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.AddColumn, columnIndex, -1);
    }

    public void DeleteColumn(int columnIndex) {
      SaveSnapshot(DataPreprocessingChangedEventType.AddColumn, columnIndex, -1);
      variableValues.RemoveAt(columnIndex);
      variableNames.RemoveAt(columnIndex);
      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.DeleteColumn, columnIndex, -1);
    }

    public void RenameColumn(int columnIndex, string name) {
      SaveSnapshot(DataPreprocessingChangedEventType.ChangeColumn, columnIndex, -1);
      if (columnIndex < 0 || columnIndex > variableNames.Count)
        throw new ArgumentOutOfRangeException("columnIndex");
      variableNames[columnIndex] = name;

      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.ChangeColumn, -1, -1);
    }

    public void RenameColumns(IList<string> names) {
      if (names == null) throw new ArgumentNullException("names");
      if (names.Count != variableNames.Count) throw new ArgumentException("number of names must match the number of columns.", "names");

      SaveSnapshot(DataPreprocessingChangedEventType.ChangeColumn, -1, -1);
      for (int i = 0; i < names.Count; i++)
        variableNames[i] = names[i];

      if (!IsInTransaction)
        OnChanged(DataPreprocessingChangedEventType.ChangeColumn, -1, -1);
    }

    public bool AreAllStringColumns(IEnumerable<int> columnIndices) {
      return columnIndices.All(x => VariableHasType<string>(x));
    }
    #endregion

    #region Variables
    public IEnumerable<string> VariableNames {
      get { return variableNames; }
    }

    public IEnumerable<string> GetDoubleVariableNames() {
      var doubleVariableNames = new List<string>();
      for (int i = 0; i < Columns; ++i) {
        if (VariableHasType<double>(i)) {
          doubleVariableNames.Add(variableNames[i]);
        }
      }
      return doubleVariableNames;
    }

    public string GetVariableName(int columnIndex) {
      return variableNames[columnIndex];
    }

    public int GetColumnIndex(string variableName) {
      return variableNames.IndexOf(variableName);
    }

    public bool VariableHasType<T>(int columnIndex) {
      return columnIndex >= variableValues.Count || variableValues[columnIndex] is List<T>;
    }

    public Type GetVariableType(int columnIndex) {
      var listType = variableValues[columnIndex].GetType();
      return listType.GenericTypeArguments.Single();
    }

    public IList<string> InputVariables { get; private set; }
    public string TargetVariable { get; private set; } // optional
    #endregion

    #region Partitions
    [Storable]
    public IntRange TrainingPartition { get; set; }
    [Storable]
    public IntRange TestPartition { get; set; }
    #endregion

    #region Transformations
    [Storable]
    public IList<ITransformation> Transformations { get; protected set; }
    #endregion

    #region Validation
    public bool Validate(string value, out string errorMessage, int columnIndex) {
      if (columnIndex < 0 || columnIndex > VariableNames.Count()) {
        throw new ArgumentOutOfRangeException("column index is out of range");
      }

      bool valid = false;
      errorMessage = string.Empty;
      if (VariableHasType<double>(columnIndex)) {
        if (string.IsNullOrWhiteSpace(value)) {
          valid = true;
        } else {
          double val;
          valid = double.TryParse(value, out val);
          if (!valid) {
            errorMessage = "Invalid Value (Valid Value Format: \"" + FormatPatterns.GetDoubleFormatPattern() + "\")";
          }
        }
      } else if (VariableHasType<string>(columnIndex)) {
        valid = value != null;
        if (!valid) {
          errorMessage = "Invalid Value (string must not be null)";
        }
      } else if (VariableHasType<DateTime>(columnIndex)) {
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
    #endregion

    #region Import & Export
    public void Import(IDataAnalysisProblemData problemData) {
      Dataset dataset = (Dataset)problemData.Dataset;
      variableNames = new List<string>(problemData.Dataset.VariableNames);
      InputVariables = new List<string>(problemData.AllowedInputVariables);
      TargetVariable = (problemData is IRegressionProblemData) ? ((IRegressionProblemData)problemData).TargetVariable
        : (problemData is IClassificationProblemData) ? ((IClassificationProblemData)problemData).TargetVariable
          : null;

      int columnIndex = 0;
      variableValues = new List<IList>();
      foreach (var variableName in problemData.Dataset.VariableNames) {
        if (dataset.VariableHasType<double>(variableName)) {
          variableValues.Insert(columnIndex, dataset.GetDoubleValues(variableName).ToList());
        } else if (dataset.VariableHasType<string>(variableName)) {
          variableValues.Insert(columnIndex, dataset.GetStringValues(variableName).ToList());
        } else if (dataset.VariableHasType<DateTime>(variableName)) {
          variableValues.Insert(columnIndex, dataset.GetDateTimeValues(variableName).ToList());
        } else {
          throw new ArgumentException("The datatype of column " + variableName + " must be of type double, string or DateTime");
        }
        ++columnIndex;
      }

      TrainingPartition = new IntRange(problemData.TrainingPartition.Start, problemData.TrainingPartition.End);
      TestPartition = new IntRange(problemData.TestPartition.Start, problemData.TestPartition.End);
    }

    public Dataset ExportToDataset() {
      IList<IList> values = new List<IList>();

      for (int i = 0; i < Columns; ++i) {
        values.Add(variableValues[i]);
      }

      var dataset = new Dataset(variableNames, values);
      return dataset;
    }
    #endregion

    #region Selection
    [Storable]
    protected IDictionary<int, IList<int>> selection;
    public IDictionary<int, IList<int>> Selection {
      get { return selection; }
      set {
        selection = value;
        OnSelectionChanged();
      }
    }
    public void ClearSelection() {
      Selection = new Dictionary<int, IList<int>>();
    }

    public event EventHandler SelectionChanged;
    protected void OnSelectionChanged() {
      var listeners = SelectionChanged;
      if (listeners != null) listeners(this, EventArgs.Empty);
    }
    #endregion

    #region Transactions
    // Stapshot/History are nost storable/cloneable on purpose
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

    public event DataPreprocessingChangedEventHandler Changed;
    protected virtual void OnChanged(DataPreprocessingChangedEventType type, int column, int row) {
      var listeners = Changed;
      if (listeners != null) listeners(this, new DataPreprocessingChangedEventArgs(type, column, row));
    }

    private const int MAX_UNDO_DEPTH = 5;

    private readonly IList<Snapshot> undoHistory = new List<Snapshot>();
    private readonly Stack<DataPreprocessingChangedEventType> eventStack = new Stack<DataPreprocessingChangedEventType>();

    public bool IsInTransaction { get { return eventStack.Count > 0; } }

    private void SaveSnapshot(DataPreprocessingChangedEventType changedType, int column, int row) {
      if (IsInTransaction) return;

      var currentSnapshot = new Snapshot {
        VariableValues = CopyVariableValues(variableValues),
        VariableNames = new List<string>(variableNames),
        TrainingPartition = new IntRange(TrainingPartition.Start, TrainingPartition.End),
        TestPartition = new IntRange(TestPartition.Start, TestPartition.End),
        Transformations = new List<ITransformation>(Transformations),
        ChangedType = changedType,
        ChangedColumn = column,
        ChangedRow = row
      };

      if (undoHistory.Count >= MAX_UNDO_DEPTH)
        undoHistory.RemoveAt(0);

      undoHistory.Add(currentSnapshot);
    }

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
        Transformations = previousSnapshot.Transformations;
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

    #region Statistics
    public T GetMin<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) {
      var values = GetValuesWithoutMissingValues<T>(columnIndex, considerSelection);
      return values.Any() ? values.Min() : emptyValue;
    }

    public T GetMax<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) {
      var values = GetValuesWithoutMissingValues<T>(columnIndex, considerSelection);
      return values.Any() ? values.Max() : emptyValue;
    }

    public T GetMean<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) {
      if (typeof(T) == typeof(double)) {
        var values = GetValuesWithoutMissingValues<double>(columnIndex, considerSelection);
        return values.Any() ? Convert<T>(values.Average()) : emptyValue;
      }
      if (typeof(T) == typeof(string)) {
        return Convert<T>(string.Empty);
      }
      if (typeof(T) == typeof(DateTime)) {
        var values = GetValuesWithoutMissingValues<DateTime>(columnIndex, considerSelection);
        return values.Any() ? Convert<T>(AggregateAsDouble(values, Enumerable.Average)) : emptyValue;
      }

      throw new InvalidOperationException(typeof(T) + " not supported");
    }

    public T GetMedian<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) where T : IComparable<T> {
      if (typeof(T) == typeof(double)) {// IEnumerable<double> is faster  
        var doubleValues = GetValuesWithoutMissingValues<double>(columnIndex, considerSelection);
        return doubleValues.Any() ? Convert<T>(doubleValues.Median()) : emptyValue;
      }
      var values = GetValuesWithoutMissingValues<T>(columnIndex, considerSelection);
      return values.Any() ? values.Quantile(0.5) : emptyValue;
    }

    public T GetMode<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) where T : IEquatable<T> {
      var values = GetValuesWithoutMissingValues<T>(columnIndex, considerSelection);
      return values.Any() ? values.GroupBy(x => x).OrderByDescending(g => g.Count()).Select(g => g.Key).First() : emptyValue;
    }

    public T GetStandardDeviation<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) {
      if (typeof(T) == typeof(double)) {
        var values = GetValuesWithoutMissingValues<double>(columnIndex, considerSelection);
        return values.Any() ? Convert<T>(values.StandardDeviation()) : emptyValue;
      }
      // For DateTime, std.dev / variance would have to be TimeSpan
      //if (typeof(T) == typeof(DateTime)) {
      //  var values = GetValuesWithoutMissingValues<DateTime>(columnIndex, considerSelection);
      //  return values.Any() ? Convert<T>(AggregateAsDouble(values, EnumerableStatisticExtensions.StandardDeviation)) : emptyValue;
      //}
      return default(T);
    }

    public T GetVariance<T>(int columnIndex, bool considerSelection = false, T emptyValue = default(T)) {
      if (typeof(T) == typeof(double)) {
        var values = GetValuesWithoutMissingValues<double>(columnIndex, considerSelection);
        return values.Any() ? Convert<T>(values.Variance()) : emptyValue;
      }
      // DateTime variance often overflows long, thus the corresponding DateTime is invalid
      //if (typeof(T) == typeof(DateTime)) {
      //  var values = GetValuesWithoutMissingValues<DateTime>(columnIndex, considerSelection);
      //  return values.Any() ? Convert<T>(AggregateAsDouble(values, EnumerableStatisticExtensions.Variance)) : emptyValue;
      //}
      return default(T);
    }

    public T GetQuantile<T>(double alpha, int columnIndex, bool considerSelection = false, T emptyValue = default(T)) where T : IComparable<T> {
      if (typeof(T) == typeof(double)) {// IEnumerable<double> is faster  
        var doubleValues = GetValuesWithoutMissingValues<double>(columnIndex, considerSelection);
        return doubleValues.Any() ? Convert<T>(doubleValues.Quantile(alpha)) : emptyValue;
      }
      var values = GetValuesWithoutMissingValues<T>(columnIndex, considerSelection);
      return values.Any() ? values.Quantile(alpha) : emptyValue;
    }

    public int GetDistinctValues<T>(int columnIndex, bool considerSelection = false) {
      var values = GetValuesWithoutMissingValues<T>(columnIndex, considerSelection);
      return values.GroupBy(x => x).Count();
    }

    private IEnumerable<T> GetValuesWithoutMissingValues<T>(int columnIndex, bool considerSelection) {
      return GetValues<T>(columnIndex, considerSelection).Where(x => !IsMissingValue(x));
    }

    private static DateTime AggregateAsDouble(IEnumerable<DateTime> values, Func<IEnumerable<double>, double> func) {
      return new DateTime((long)(func(values.Select(x => (double)x.Ticks / TimeSpan.TicksPerSecond)) * TimeSpan.TicksPerSecond));
    }
    private static T Convert<T>(object obj) { return (T)obj; }

    public int GetMissingValueCount() {
      int count = 0;
      for (int i = 0; i < Columns; ++i) {
        count += GetMissingValueCount(i);
      }
      return count;
    }
    public int GetMissingValueCount(int columnIndex) {
      int sum = 0;
      for (int i = 0; i < Rows; i++) {
        if (IsCellEmpty(columnIndex, i))
          sum++;
      }
      return sum;
    }
    public int GetRowMissingValueCount(int rowIndex) {
      int sum = 0;
      for (int i = 0; i < Columns; i++) {
        if (IsCellEmpty(i, rowIndex))
          sum++;
      }
      return sum;
    }
    #endregion

    #region Helpers
    private static IList<IList> CopyVariableValues(IList<IList> original) {
      var copy = new List<IList>(original);
      for (int i = 0; i < original.Count; ++i) {
        copy[i] = (IList)Activator.CreateInstance(original[i].GetType(), original[i]);
      }
      return copy;
    }
    #endregion
  }

  // Adapted from HeuristicLab.Common.EnumerableStatisticExtensions
  internal static class EnumerableExtensions {
    public static T Quantile<T>(this IEnumerable<T> values, double alpha) where T : IComparable<T> {
      T[] valuesArr = values.ToArray();
      int n = valuesArr.Length;
      if (n == 0) throw new InvalidOperationException("Enumeration contains no elements.");

      var pos = n * alpha;

      return Select((int)Math.Ceiling(pos) - 1, valuesArr);

    }

    private static T Select<T>(int k, T[] arr) where T : IComparable<T> {
      int i, ir, j, l, mid, n = arr.Length;
      T a;
      l = 0;
      ir = n - 1;
      for (;;) {
        if (ir <= l + 1) {
          // Active partition contains 1 or 2 elements.
          if (ir == l + 1 && arr[ir].CompareTo(arr[l]) < 0) {
            // Case of 2 elements.
            Swap(arr, l, ir);
          }
          return arr[k];
        } else {
          mid = (l + ir) >> 1; // Choose median of left, center, and right elements
          Swap(arr, mid, l + 1); // as partitioning element a. Also

          if (arr[l].CompareTo(arr[ir]) > 0) {  // rearrange so that arr[l] arr[ir] <= arr[l+1],
            Swap(arr, l, ir); // . arr[ir] >= arr[l+1]
          }

          if (arr[l + 1].CompareTo(arr[ir]) > 0) {
            Swap(arr, l + 1, ir);
          }
          if (arr[l].CompareTo(arr[l + 1]) > 0) {
            Swap(arr, l, l + 1);
          }
          i = l + 1; // Initialize pointers for partitioning.
          j = ir;
          a = arr[l + 1]; // Partitioning element.
          for (;;) { // Beginning of innermost loop.
            do i++; while (arr[i].CompareTo(a) < 0); // Scan up to find element > a.
            do j--; while (arr[j].CompareTo(a) > 0); // Scan down to find element < a.
            if (j < i) break; // Pointers crossed. Partitioning complete.
            Swap(arr, i, j);
          } // End of innermost loop.
          arr[l + 1] = arr[j]; // Insert partitioning element.
          arr[j] = a;
          if (j >= k) ir = j - 1; // Keep active the partition that contains the
          if (j <= k) l = i; // kth element.
        }
      }
    }

    private static void Swap<T>(T[] arr, int i, int j) {
      T temp = arr[i];
      arr[i] = arr[j];
      arr[j] = temp;
    }
  }
}
