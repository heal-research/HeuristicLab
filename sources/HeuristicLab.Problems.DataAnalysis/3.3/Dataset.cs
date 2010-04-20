#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Xml;
using System.Globalization;
using System.Text;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("Dataset", "Represents a dataset containing data that should be analyzed.")]
  [StorableClass]
  public sealed class Dataset : NamedItem, IStringConvertibleMatrix {
    public Dataset()
      : this(new string[1] { "y" }, new double[,] { { 0.0 } }) {
    }

    public Dataset(IEnumerable<string> variableNames, double[,] data)
      : base() {
      Name = "-";
      if (variableNames.Count() != data.GetLength(1)) {
        throw new ArgumentException("Number of variable names doesn't match the number of columns of data");
      }
      this.data = data;
      this.variableNames = variableNames.ToArray();
      this.SortableView = false;
    }

    [Storable]
    private string[] variableNames;
    public IEnumerable<string> VariableNames {
      get { return variableNames; }
      private set {
        if (variableNames != value) {
          variableNames = value.ToArray();
          OnColumnNamesChanged();
        }
      }
    }

    [Storable]
    private double[,] data;
    private double[,] Data {
      get { return data; }
      set {
        if (data != value) {
          if (value == null) throw new ArgumentNullException();
          this.data = value;
          OnReset(EventArgs.Empty);
        }
      }
    }

    // elementwise access
    public double this[int rowIndex, int columnIndex] {
      get { return data[rowIndex, columnIndex]; }
      set {
        if (!value.Equals(data[rowIndex, columnIndex])) {
          data[rowIndex, columnIndex] = value;
          OnDataChanged(new EventArgs<int, int>(rowIndex, columnIndex));
          OnItemChanged(rowIndex, columnIndex);
        }
      }
    }
    // access to full columns
    public double[] this[string variableName] {
      get { return GetVariableValues(GetVariableIndex(variableName), 0, Rows); }
    }

    public double[] GetVariableValues(int variableIndex, int start, int end) {
      if (start < 0 || !(start <= end))
        throw new ArgumentException("Start must be between 0 and end (" + end + ").");
      if (end > Rows || end < start)
        throw new ArgumentException("End must be between start (" + start + ") and dataset rows (" + Rows + ").");

      double[] values = new double[end - start];
      for (int i = 0; i < end - start; i++)
        values[i] = data[i + start, variableIndex];
      return values;
    }

    public double[] GetVariableValues(string variableName, int start, int end) {
      return GetVariableValues(GetVariableIndex(variableName), start, end);
    }

    #region Variable name methods
    public string GetVariableName(int variableIndex) {
      return variableNames[variableIndex];
    }

    public int GetVariableIndex(string variableName) {
      for (int i = 0; i < variableNames.Length; i++) {
        if (variableNames[i].Equals(variableName)) return i;
      }
      throw new ArgumentException("The variable name " + variableName + " was not found.");
    }

    public void SetVariableName(int variableIndex, string name) {
      if (name == null) throw new ArgumentNullException("Cannot set variable name to null for variable at index " + variableIndex + " variableIndex");
      if (variableNames.Contains(name)) throw new ArgumentException("The data set already contains a variable with name " + name + ".");
      if (variableIndex < 0 || variableIndex >= variableNames.Length) throw new ArgumentException(" Cannot set name of not existent variable at index " + variableIndex + ".");
      variableNames[variableIndex] = name;
      OnColumnNamesChanged();
    }

    #endregion

    #region variable statistics
    public double GetMean(string variableName) {
      return GetMean(GetVariableIndex(variableName));
    }

    public double GetMean(string variableName, int start, int end) {
      return GetMean(GetVariableIndex(variableName), start, end);
    }

    public double GetMean(int variableIndex) {
      return GetMean(variableIndex, 0, Rows);
    }

    public double GetMean(int variableIndex, int start, int end) {
      return GetVariableValues(variableIndex, start, end).Average();
    }

    public double GetRange(string variableName) {
      return GetRange(GetVariableIndex(variableName));
    }

    public double GetRange(int variableIndex) {
      return GetRange(variableIndex, 0, Rows);
    }

    public double GetRange(string variableName, int start, int end) {
      return GetRange(GetVariableIndex(variableName), start, end);
    }

    public double GetRange(int variableIndex, int start, int end) {
      var values = GetVariableValues(variableIndex, start, end);
      return values.Max() - values.Min();
    }

    public double GetMax(string variableName) {
      return GetMax(GetVariableIndex(variableName));
    }

    public double GetMax(int variableIndex) {
      return GetMax(variableIndex, 0, Rows);
    }

    public double GetMax(string variableName, int start, int end) {
      return GetMax(GetVariableIndex(variableName), start, end);
    }

    public double GetMax(int variableIndex, int start, int end) {
      return GetVariableValues(variableIndex, start, end).Max();
    }

    public double GetMin(string variableName) {
      return GetMin(GetVariableIndex(variableName));
    }

    public double GetMin(int variableIndex) {
      return GetMin(variableIndex, 0, Rows);
    }

    public double GetMin(string variableName, int start, int end) {
      return GetMin(GetVariableIndex(variableName), start, end);
    }

    public double GetMin(int variableIndex, int start, int end) {
      return GetVariableValues(variableIndex, start, end).Min();
    }

    public int GetMissingValues(string variableName) {
      return GetMissingValues(GetVariableIndex(variableName));
    }
    public int GetMissingValues(int variableIndex) {
      return GetMissingValues(variableIndex, 0, Rows);
    }

    public int GetMissingValues(string variableName, int start, int end) {
      return GetMissingValues(GetVariableIndex(variableName), start, end);
    }

    public int GetMissingValues(int variableIndex, int start, int end) {
      return GetVariableValues(variableIndex, start, end).Count(x => double.IsNaN(x));
    }

    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      Dataset clone = (Dataset)base.Clone(cloner);
      clone.data = (double[,])data.Clone();
      clone.variableNames = (string[])variableNames.Clone();
      return clone;
    }

    #region events
    public event EventHandler<EventArgs<int, int>> DataChanged;
    private void OnDataChanged(EventArgs<int, int> e) {
      var listeners = DataChanged;
      if (listeners != null) listeners(this, e);
    }
    public event EventHandler Reset;
    private void OnReset(EventArgs e) {
      var listeners = Reset;
      if (listeners != null) listeners(this, e);
    }
    #endregion

    #region IStringConvertibleMatrix Members

    public int Rows {
      get {
        return data.GetLength(0);
      }
      set {
        if (value == 0) throw new ArgumentException("Number of rows must be at least one (for variable names)");
        if (value != Rows) {
          var newValues = new double[value, Columns];
          for (int row = 0; row < Math.Min(Rows, value); row++) {
            for (int column = 0; column < Columns; column++) {
              newValues[row, column] = data[row, column];
            }
          }
          Data = newValues;
        }
      }
    }

    public int Columns {
      get {
        return data.GetLength(1);
      }
      set {
        if (value != Columns) {
          var newValues = new double[Rows, value];
          var newVariableNames = new string[value];
          for (int row = 0; row < Rows; row++) {
            for (int column = 0; column < Math.Min(value, Columns); column++) {
              newValues[row, column] = data[row, column];
            }
          }
          string formatString = new StringBuilder().Append('0', (int)Math.Log10(value) + 1).ToString(); // >= 100 variables => ###
          for (int column = 0; column < value; column++) {
            if (column < Columns)
              newVariableNames[column] = variableNames[column];
            else
              newVariableNames[column] = "Var" + column.ToString(formatString);
          }
          VariableNames = newVariableNames;
          Data = newValues;
        }
      }
    }

    [Storable]
    private bool sortableView;
    public bool SortableView {
      get { return sortableView; }
      set {
        if (value != sortableView) {
          sortableView = value;
          OnSortableViewChanged();
        }
      }
    }

    public bool ReadOnly {
      get { return false; }
    }

    IEnumerable<string> IStringConvertibleMatrix.ColumnNames {
      get { return this.VariableNames; }
      set {
        int i = 0;
        foreach (string variableName in value) {
          SetVariableName(i, variableName);
          i++;
        }
        OnColumnNamesChanged();
      }
    }

    IEnumerable<string> IStringConvertibleMatrix.RowNames {
      get { return new List<string>(); }
      set { throw new NotImplementedException(); }
    }

    public bool Validate(string value, out string errorMessage) {
      double val;
      bool valid = double.TryParse(value, out val);
      errorMessage = string.Empty;
      if (!valid) {
        StringBuilder sb = new StringBuilder();
        sb.Append("Invalid Value (Valid Value Format: \"");
        sb.Append(FormatPatterns.GetDoubleFormatPattern());
        sb.Append("\")");
        errorMessage = sb.ToString();
      }
      return valid;
    }

    public string GetValue(int rowIndex, int columnIndex) {
      return data[rowIndex, columnIndex].ToString();
    }

    public bool SetValue(string value, int rowIndex, int columnIndex) {
      double v;
      if (double.TryParse(value, out v)) {
        data[rowIndex, columnIndex] = v;
        OnDataChanged(new EventArgs<int, int>(rowIndex, columnIndex));
        OnItemChanged(rowIndex, columnIndex);
        return true;
      } else return false;
    }

    public event EventHandler ColumnNamesChanged;
    private void OnColumnNamesChanged() {
      EventHandler handler = ColumnNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler RowNamesChanged;
    private void OnRowNamesChanged() {
      EventHandler handler = RowNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler SortableViewChanged;
    private void OnSortableViewChanged() {
      EventHandler handler = SortableViewChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<int, int>> ItemChanged;
    private void OnItemChanged(int rowIndex, int columnIndex) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int, int>(rowIndex, columnIndex));
      OnToStringChanged();
    }
    #endregion
  }
}
