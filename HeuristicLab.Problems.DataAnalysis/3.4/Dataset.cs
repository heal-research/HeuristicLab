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
using System.Collections.ObjectModel;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("Dataset", "Represents a dataset containing data that should be analyzed.")]
  [StorableClass]
  public sealed class Dataset : NamedItem, IStringConvertibleMatrix {
    [StorableConstructor]
    private Dataset(bool deserializing) : base(deserializing) { }
    private Dataset(Dataset original, Cloner cloner)
      : base(original, cloner) {
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

    public Dataset(IEnumerable<string> variableNames, IEnumerable<IList> variableValues)
      : base() {
      Name = "-";
      if (!variableNames.Any()) {
        this.variableNames = Enumerable.Range(0, variableValues.Count()).Select(x => "Column " + x).ToList();
      } else if (variableNames.Count() != variableValues.Count()) {
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

      rows = variableValues.First().Count;
      this.variableNames = new List<string>(variableNames);
      this.variableValues = new Dictionary<string, IList>(this.variableNames.Count);
      for (int i = 0; i < this.variableNames.Count; i++) {
        var values = variableValues.ElementAt(i);
        IList clonedValues = null;
        if (values is IList<double>)
          clonedValues = new List<double>(values.Cast<double>());
        else if (values is IList<string>)
          clonedValues = new List<string>(values.Cast<string>());
        else if (values is IList<DateTime>)
          clonedValues = new List<DateTime>(values.Cast<DateTime>());
        else {
          this.variableNames = new List<string>();
          this.variableValues = new Dictionary<string, IList>();
          throw new ArgumentException("The variable values must be of type IList<double>, IList<string> or IList<DateTime>");
        }
        this.variableValues.Add(this.variableNames[i], clonedValues);
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

    #region Backwards compatible code, remove with 3.5
    private double[,] storableData;
    //name alias used to suppport backwards compatibility
    [Storable(Name = "data", AllowOneWay = true)]
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
    private Dictionary<string, IList> variableValues;

    private List<string> variableNames;
    [Storable]
    public IEnumerable<string> VariableNames {
      get { return variableNames; }
      private set {
        if (variableNames != null) throw new InvalidOperationException();
        variableNames = new List<string>(value);
      }
    }

    public IEnumerable<string> DoubleVariables {
      get { return variableValues.Where(p => p.Value is List<double>).Select(p => p.Key); }
    }

    public IEnumerable<double> GetDoubleValues(string variableName) {
      IList list;
      if (!variableValues.TryGetValue(variableName, out list))
        throw new ArgumentException("The variable " + variableName + " does not exist in the dataset.");
      List<double> values = list as List<double>;
      if (values == null) throw new ArgumentException("The variable " + variableName + " is not a double variable.");

      //mkommend yield return used to enable lazy evaluation
      foreach (double value in values)
        yield return value;
    }

    public IEnumerable<string> GetStringValues(string variableName) {
      IList list;
      if (!variableValues.TryGetValue(variableName, out list))
        throw new ArgumentException("The variable " + variableName + " does not exist in the dataset.");
      List<string> values = list as List<string>;
      if (values == null) throw new ArgumentException("The variable " + variableName + " is not a string variable.");

      //mkommend yield return used to enable lazy evaluation
      foreach (string value in values)
        yield return value;
    }

    public IEnumerable<DateTime> GetDateTimeValues(string variableName) {
      IList list;
      if (!variableValues.TryGetValue(variableName, out list))
        throw new ArgumentException("The variable " + variableName + " does not exist in the dataset.");
      List<DateTime> values = list as List<DateTime>;
      if (values == null) throw new ArgumentException("The variable " + variableName + " is not a datetime variable.");

      //mkommend yield return used to enable lazy evaluation
      foreach (DateTime value in values)
        yield return value;
    }

    public ReadOnlyCollection<double> GetReadOnlyDoubleValues(string variableName) {
      IList list;
      if (!variableValues.TryGetValue(variableName, out list))
        throw new ArgumentException("The variable " + variableName + " does not exist in the dataset.");
      List<double> values = list as List<double>;
      if (values == null) throw new ArgumentException("The variable " + variableName + " is not a double variable.");
      return values.AsReadOnly();
    }
    public double GetDoubleValue(string variableName, int row) {
      IList list;
      if (!variableValues.TryGetValue(variableName, out list))
        throw new ArgumentException("The variable " + variableName + " does not exist in the dataset.");
      List<double> values = list as List<double>;
      if (values == null) throw new ArgumentException("The variable " + variableName + " is not a double variable.");
      return values[row];
    }
    public IEnumerable<double> GetDoubleValues(string variableName, IEnumerable<int> rows) {
      IList list;
      if (!variableValues.TryGetValue(variableName, out list))
        throw new ArgumentException("The variable " + variableName + " does not exist in the dataset.");
      List<double> values = list as List<double>;
      if (values == null) throw new ArgumentException("The variable " + variableName + " is not a double variable.");

      return rows.Select(index => values[index]);
    }

    public bool VariableHasType<T>(string variableName) {
      return variableValues[variableName] is IList<T>;
    }

    #region IStringConvertibleMatrix Members
    [Storable]
    private int rows;
    public int Rows {
      get { return rows; }
      set { throw new NotSupportedException(); }
    }
    public int Columns {
      get { return variableNames.Count; }
      set { throw new NotSupportedException(); }
    }

    public bool SortableView {
      get { return false; }
      set { throw new NotSupportedException(); }
    }
    public bool ReadOnly {
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

    public string GetValue(int rowIndex, int columnIndex) {
      return variableValues[variableNames[columnIndex]][rowIndex].ToString();
    }
    public bool SetValue(string value, int rowIndex, int columnIndex) {
      throw new NotSupportedException();
    }
    public bool Validate(string value, out string errorMessage) {
      throw new NotSupportedException();
    }

    public event EventHandler ColumnsChanged { add { } remove { } }
    public event EventHandler RowsChanged { add { } remove { } }
    public event EventHandler ColumnNamesChanged { add { } remove { } }
    public event EventHandler RowNamesChanged { add { } remove { } }
    public event EventHandler SortableViewChanged { add { } remove { } }
    public event EventHandler<EventArgs<int, int>> ItemChanged { add { } remove { } }
    public event EventHandler Reset { add { } remove { } }
    #endregion
  }
}
