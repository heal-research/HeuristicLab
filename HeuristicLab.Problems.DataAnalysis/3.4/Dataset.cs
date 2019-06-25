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
using System.Collections.ObjectModel;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("Dataset", "Represents a dataset containing data that should be analyzed.")]
  [StorableType("49F4D145-50D7-4497-8D8A-D190CD556CC8")]
  public class Dataset : NamedItem, IDataset {
    [StorableConstructor]
    protected Dataset(StorableConstructorFlag _) : base(_) { }
    protected Dataset(Dataset original, Cloner cloner)
      : base(original, cloner) {
      // no need to clone the variable values because these can't be modified
      variableValues = new Dictionary<string, IList>(original.variableValues);
      variableNames = new List<string>(original.variableNames);
      rows = original.rows;
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new Dataset(this, cloner); }

    public Dataset()
      : base() {
      Name = "-";
      VariableNames = Enumerable.Empty<string>();
      variableValues = new Dictionary<string, IList>();
      rows = 0;
    }

    /// <summary>
    /// Creates a new dataset. The variableValues are not cloned.
    /// </summary>
    /// <param name="variableNames">The names of the variables in the dataset</param>
    /// <param name="variableValues">The values for the variables (column-oriented storage). Values are not cloned!</param>
    public Dataset(IEnumerable<string> variableNames, IEnumerable<IList> variableValues)
      : this(variableNames, variableValues, cloneValues: true) {
    }

    protected Dataset(IEnumerable<string> variableNames, IEnumerable<IList> variableValues, bool cloneValues = false) {
      Name = "-";

      if (variableNames.Any()) {
        this.variableNames = new List<string>(variableNames);
      } else {
        this.variableNames = Enumerable.Range(0, variableValues.Count()).Select(x => "Column " + x).ToList();
      }
      // check if the arguments are consistent (no duplicate variables, same number of rows, correct data types, ...)
      CheckArguments(this.variableNames, variableValues);

      rows = variableValues.First().Count;

      if (cloneValues) {
        this.variableValues = CloneValues(this.variableNames, variableValues);
      } else {
        this.variableValues = new Dictionary<string, IList>(this.variableNames.Count);
        for (int i = 0; i < this.variableNames.Count; i++) {
          var variableName = this.variableNames[i];
          var values = variableValues.ElementAt(i);
          this.variableValues.Add(variableName, values);
        }
      }
    }

    public Dataset(IEnumerable<string> variableNames, double[,] variableValues) {
      Name = "-";
      if (variableNames.Count() != variableValues.GetLength(1)) {
        throw new ArgumentException("Number of variable names doesn't match the number of columns of variableValues");
      }
      if (variableNames.Distinct().Count() != variableNames.Count()) {
        var duplicateVariableNames = variableNames.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        string message = "The dataset cannot contain duplicate variables names: " + Environment.NewLine;
        foreach (var duplicateVariableName in duplicateVariableNames)
          message += duplicateVariableName + Environment.NewLine;
        throw new ArgumentException(message);
      }

      rows = variableValues.GetLength(0);
      this.variableNames = new List<string>(variableNames);

      this.variableValues = new Dictionary<string, IList>(variableValues.GetLength(1));
      for (int col = 0; col < variableValues.GetLength(1); col++) {
        string columName = this.variableNames[col];
        var values = new List<double>(variableValues.GetLength(0));
        for (int row = 0; row < variableValues.GetLength(0); row++) {
          values.Add(variableValues[row, col]);
        }
        this.variableValues.Add(columName, values);
      }
    }

    public static Dataset FromRowData(IEnumerable<string> variableNames, double[,] data) {
      var colWise = new List<IList>(data.GetLength(1));
      for (var col = 0; col < data.GetLength(1); col++) {
        var column = new List<double>(data.GetLength(0));
        for (var row = 0; row < data.GetLength(0); row++) {
          column.Add(data[row, col]);
        }
        colWise.Add(column);
      }
      return new Dataset(variableNames, colWise);
    }

    public static Dataset FromRowData(IEnumerable<string> variableNames, IEnumerable<IList> data) {
      var vnames = variableNames.ToList();
      var transposed = new List<IList>();
      var iter = data.GetEnumerator();
      if (!iter.MoveNext()) throw new ArgumentException("Data does not contain any rows", nameof(data));
      for (var i = 0; i < iter.Current.Count; i++) {
        if (i >= vnames.Count) throw new ArgumentException("There are more variables in data, than variable names.", nameof(variableNames));
        if (iter.Current[i] == null) throw new ArgumentException("Null values are not supported.", nameof(data));
        if (!IsAllowedType(iter.Current[i].GetType())) throw new ArgumentException("Data contains types that are not allowed.", nameof(data));
        if (iter.Current[i] is double d)
          transposed.Add(new List<double>() { d });
        else if (iter.Current[i] is DateTime dt)
          transposed.Add(new List<DateTime>() { dt });
        else if (iter.Current[i] is string s)
          transposed.Add(new List<string>() { s });
        else throw new NotSupportedException(string.Format("Variable {0} has type {1}. This is not supported when converting from row-wise data.", vnames[i], iter.Current[i].GetType()));
      }
      if (transposed.Count < vnames.Count) throw new ArgumentException("There are less variables in data, than variable names.", nameof(variableNames));
      while (iter.MoveNext()) {
        for (var i = 0; i < iter.Current.Count; i++)
          if (transposed[i].Add(iter.Current[i]) < 0)
            throw new ArgumentException(string.Format("Variable {0} has invalid value ({1})", vnames[i], iter.Current[i]), nameof(data));
      }
      return new Dataset(vnames, transposed);
    }

    public ModifiableDataset ToModifiable() {
      return new ModifiableDataset(variableNames, variableNames.Select(v => variableValues[v]), true);
    }

    /// <summary>
    /// Shuffle a dataset's rows
    /// </summary>
    /// <param name="random">Random number generator used for shuffling.</param>
    /// <returns>A shuffled copy of the current dataset.</returns>
    public Dataset Shuffle(IRandom random) {
      var values = variableNames.Select(x => variableValues[x]).ToList();
      return new Dataset(variableNames, values.ShuffleLists(random));
    }



    #region Backwards compatible code, remove with 3.5
    private double[,] storableData;
    //name alias used to support backwards compatibility
    [Storable(OldName = "data")]
    private double[,] StorableData { set { storableData = value; } }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (variableValues == null) {
        rows = storableData.GetLength(0);
        variableValues = new Dictionary<string, IList>();
        for (int col = 0; col < storableData.GetLength(1); col++) {
          string columName = variableNames[col];
          var values = new List<double>(rows);
          for (int row = 0; row < rows; row++) {
            values.Add(storableData[row, col]);
          }
          variableValues.Add(columName, values);
        }
        storableData = null;
      }
    }
    #endregion

    [Storable(Name = "VariableValues")]
    protected Dictionary<string, IList> variableValues;

    protected List<string> variableNames;
    [Storable]
    public IEnumerable<string> VariableNames {
      get { return variableNames; }
      protected set {
        if (variableNames != null) throw new InvalidOperationException();
        variableNames = new List<string>(value);
      }
    }

    public bool ContainsVariable(string variableName) {
      return variableValues.ContainsKey(variableName);
    }
    public IEnumerable<string> DoubleVariables {
      get { return variableValues.Where(p => p.Value is IList<double>).Select(p => p.Key); }
    }

    public IEnumerable<string> StringVariables {
      get { return variableValues.Where(p => p.Value is IList<string>).Select(p => p.Key); }
    }

    public IEnumerable<string> DateTimeVariables {
      get { return variableValues.Where(p => p.Value is IList<DateTime>).Select(p => p.Key); }
    }

    public IEnumerable<double> GetDoubleValues(string variableName) {
      return GetValues<double>(variableName);
    }
    public IEnumerable<string> GetStringValues(string variableName) {
      return GetValues<string>(variableName);
    }
    public IEnumerable<DateTime> GetDateTimeValues(string variableName) {
      return GetValues<DateTime>(variableName);
    }

    public ReadOnlyCollection<double> GetReadOnlyDoubleValues(string variableName) {
      var values = GetValues<double>(variableName);
      return new ReadOnlyCollection<double>(values);
    }
    public double GetDoubleValue(string variableName, int row) {
      var values = GetValues<double>(variableName);
      return values[row];
    }
    public IEnumerable<double> GetDoubleValues(string variableName, IEnumerable<int> rows) {
      return GetValues<double>(variableName, rows);
    }

    public string GetStringValue(string variableName, int row) {
      var values = GetValues<string>(variableName);
      return values[row];
    }

    public IEnumerable<string> GetStringValues(string variableName, IEnumerable<int> rows) {
      return GetValues<string>(variableName, rows);
    }
    public ReadOnlyCollection<string> GetReadOnlyStringValues(string variableName) {
      var values = GetValues<string>(variableName);
      return new ReadOnlyCollection<string>(values);
    }

    public DateTime GetDateTimeValue(string variableName, int row) {
      var values = GetValues<DateTime>(variableName);
      return values[row];
    }
    public IEnumerable<DateTime> GetDateTimeValues(string variableName, IEnumerable<int> rows) {
      return GetValues<DateTime>(variableName, rows);
    }
    public ReadOnlyCollection<DateTime> GetReadOnlyDateTimeValues(string variableName) {
      var values = GetValues<DateTime>(variableName);
      return new ReadOnlyCollection<DateTime>(values);
    }
    private IEnumerable<T> GetValues<T>(string variableName, IEnumerable<int> rows) {
      var values = GetValues<T>(variableName);
      return rows.Select(x => values[x]);
    }
    private IList<T> GetValues<T>(string variableName) {
      IList list;
      if (!variableValues.TryGetValue(variableName, out list))
        throw new ArgumentException("The variable " + variableName + " does not exist in the dataset.");
      IList<T> values = list as IList<T>;
      if (values == null) throw new ArgumentException("The variable " + variableName + " is not a " + typeof(T) + " variable.");
      return values;
    }
    public bool VariableHasType<T>(string variableName) {
      return variableValues[variableName] is IList<T>;
    }
    protected Type GetVariableType(string variableName) {
      IList list;
      variableValues.TryGetValue(variableName, out list);
      if (list == null)
        throw new ArgumentException("The variable " + variableName + " does not exist in the dataset.");
      return GetElementType(list);
    }
    protected static Type GetElementType(IList list) {
      var type = list.GetType();
      return type.IsGenericType ? type.GetGenericArguments()[0] : type.GetElementType();
    }
    protected static bool IsAllowedType(IList list) {
      var type = GetElementType(list);
      return IsAllowedType(type);
    }
    protected static bool IsAllowedType(Type type) {
      return type == typeof(double) || type == typeof(string) || type == typeof(DateTime);
    }

    protected static void CheckArguments(IEnumerable<string> variableNames, IEnumerable<IList> variableValues) {
      if (variableNames.Count() != variableValues.Count()) {
        throw new ArgumentException("Number of variable names doesn't match the number of columns of variableValues");
      } else if (!variableValues.All(list => list.Count == variableValues.First().Count)) {
        throw new ArgumentException("The number of values must be equal for every variable");
      } else if (variableNames.Distinct().Count() != variableNames.Count()) {
        var duplicateVariableNames =
          variableNames.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        string message = "The dataset cannot contain duplicate variables names: " + Environment.NewLine;
        foreach (var duplicateVariableName in duplicateVariableNames)
          message += duplicateVariableName + Environment.NewLine;
        throw new ArgumentException(message);
      }
      // check if all the variables are supported
      foreach (var t in variableNames.Zip(variableValues, Tuple.Create)) {
        var variableName = t.Item1;
        var values = t.Item2;

        if (!IsAllowedType(values)) {
          throw new ArgumentException(string.Format("Unsupported type {0} for variable {1}.", GetElementType(values), variableName));
        }
      }
    }

    protected static Dictionary<string, IList> CloneValues(Dictionary<string, IList> variableValues) {
      return variableValues.ToDictionary(x => x.Key, x => CloneValues(x.Value));
    }

    protected static Dictionary<string, IList> CloneValues(IEnumerable<string> variableNames, IEnumerable<IList> variableValues) {
      return variableNames.Zip(variableValues, Tuple.Create).ToDictionary(x => x.Item1, x => CloneValues(x.Item2));
    }

    protected static IList CloneValues(IList values) {
      var doubleValues = values as IList<double>;
      if (doubleValues != null) return new List<double>(doubleValues);

      var stringValues = values as IList<string>;
      if (stringValues != null) return new List<string>(stringValues);

      var dateTimeValues = values as IList<DateTime>;
      if (dateTimeValues != null) return new List<DateTime>(dateTimeValues);

      throw new ArgumentException(string.Format("Unsupported variable type {0}.", GetElementType(values)));
    }

    #region IStringConvertibleMatrix Members
    [Storable]
    private int rows;
    public int Rows {
      get { return rows; }
      protected set { rows = value; }
    }
    int IStringConvertibleMatrix.Rows {
      get { return Rows; }
      set { throw new NotSupportedException(); }
    }

    public int Columns {
      get { return variableNames.Count; }
    }
    int IStringConvertibleMatrix.Columns {
      get { return Columns; }
      set { throw new NotSupportedException(); }
    }
    bool IStringConvertibleMatrix.SortableView {
      get { return false; }
      set { throw new NotSupportedException(); }
    }
    bool IStringConvertibleMatrix.ReadOnly {
      get { return true; }
    }
    IEnumerable<string> IStringConvertibleMatrix.ColumnNames {
      get { return this.VariableNames; }
      set { throw new NotSupportedException(); }
    }
    IEnumerable<string> IStringConvertibleMatrix.RowNames {
      get { return Enumerable.Empty<string>(); }
      set { throw new NotSupportedException(); }
    }
    string IStringConvertibleMatrix.GetValue(int rowIndex, int columnIndex) {
      return variableValues[variableNames[columnIndex]][rowIndex].ToString();
    }
    bool IStringConvertibleMatrix.SetValue(string value, int rowIndex, int columnIndex) {
      throw new NotSupportedException();
    }
    bool IStringConvertibleMatrix.Validate(string value, out string errorMessage) {
      throw new NotSupportedException();
    }

    public virtual event EventHandler ColumnsChanged { add { } remove { } }
    public virtual event EventHandler RowsChanged { add { } remove { } }
    public virtual event EventHandler ColumnNamesChanged { add { } remove { } }
    public virtual event EventHandler RowNamesChanged { add { } remove { } }
    public virtual event EventHandler SortableViewChanged { add { } remove { } }
    public virtual event EventHandler<EventArgs<int, int>> ItemChanged { add { } remove { } }
    public virtual event EventHandler Reset { add { } remove { } }
    #endregion
  }
}
