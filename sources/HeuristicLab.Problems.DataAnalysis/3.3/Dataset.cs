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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("Dataset", "Represents a dataset containing data that should be analyzed.")]
  [StorableClass]
  public sealed class Dataset : NamedItem, IStringConvertibleMatrix {
    public Dataset()
      : this(new string[] { "x" }, new double[,] { { 0.0 } }) {
    }

    public Dataset(IEnumerable<string> variableNames, double[,] data)
      : base() {
      Name = "-";
      if (variableNames.Count() != data.GetLength(1)) {
        throw new ArgumentException("Number of variable names doesn't match the number of columns of data");
      }
      Data = new DoubleMatrix(data);
      this.variableNames = new StringArray(variableNames.ToArray());
    }

    [Storable]
    private StringArray variableNames;
    public IEnumerable<string> VariableNames {
      get { return variableNames; }
    }

    [Storable]
    private DoubleMatrix data;
    private DoubleMatrix Data {
      get { return data; }
      set {
        if (data != value) {
          if (value == null) throw new ArgumentNullException();
          if (data != null) DeregisterDataEvents();
          this.data = value;
          RegisterDataEvents();
          OnReset(EventArgs.Empty);
        }
      }
    }

    private void RegisterDataEvents() {
      data.Reset += new EventHandler(data_Reset);
      data.ItemChanged += new EventHandler<EventArgs<int, int>>(data_ItemChanged);
    }

    private void DeregisterDataEvents() {
      data.Reset -= new EventHandler(data_Reset);
      data.ItemChanged -= new EventHandler<EventArgs<int, int>>(data_ItemChanged);
    }
    // elementwise access
    public double this[int rowIndex, int columnIndex] {
      get { return data[rowIndex, columnIndex]; }
      set {
        if (!value.Equals(data[rowIndex, columnIndex])) {
          data[rowIndex, columnIndex] = value;
          OnDataChanged(new EventArgs<int, int>(rowIndex, columnIndex));
        }
      }
    }
    // access to full columns
    public double[] this[string variableName] {
      get { return GetVariableValues(GetVariableIndex(variableName), 0, data.Rows); }
    }

    public double[] GetVariableValues(int variableIndex, int start, int end) {
      if (start < 0 || !(start <= end))
        throw new ArgumentException("Start must be between 0 and end (" + end + ").");
      if (end > data.Rows || end < start)
        throw new ArgumentException("End must be between start (" + start + ") and dataset rows (" + data.Rows + ").");

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
      if (variableNames.Contains(name)) throw new ArgumentException("The data set already contains a variable with name " + name + ".");
      variableNames[variableIndex] = name;
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
      return GetMean(variableIndex, 0, data.Rows);
    }

    public double GetMean(int variableIndex, int start, int end) {
      return GetVariableValues(variableIndex, start, end).Average();
    }

    public double GetRange(string variableName) {
      return GetRange(GetVariableIndex(variableName));
    }

    public double GetRange(int variableIndex) {
      return GetRange(variableIndex, 0, data.Rows);
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
      return GetMax(variableIndex, 0, data.Rows);
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
      return GetMin(variableIndex, 0, data.Rows);
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
      return GetMissingValues(variableIndex, 0, data.Rows);
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
      clone.data = (DoubleMatrix)data.Clone(cloner);
      clone.variableNames = (StringArray)variableNames.Clone(cloner);
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

    private void data_ItemChanged(object sender, EventArgs<int, int> e) {
      OnDataChanged(e);
    }

    private void data_Reset(object sender, EventArgs e) {
      OnReset(e);
    }
    #endregion

    #region IStringConvertibleMatrix Members

    public int Rows {
      get {
        return data.Rows + 1;
      }
      set {
        if (value == 0) throw new ArgumentException("Number of rows must be at least one (for variable names)");
        if (value - 1 != data.Rows) {
          var newValues = new double[value - 1, data.Columns];
          for (int row = 0; row < Math.Min(data.Rows, value - 1); row++) {
            for (int column = 0; column < data.Columns; column++) {
              newValues[row, column] = data[row, column];
            }
          }
          Data = new DoubleMatrix(newValues);
        }
      }
    }

    public int Columns {
      get {
        return data.Columns;
      }
      set {
        if (value != data.Columns) {
          var newValues = new double[data.Rows, value];
          var newVariableNames = new string[value];
          for (int row = 0; row < data.Rows; row++) {
            for (int column = 0; column < Math.Min(value, data.Columns); column++) {
              newValues[row, column] = data[row, column];
            }
          }
          string formatString = new StringBuilder().Append('0', (int)Math.Log10(value) + 1).ToString(); // >= 100 variables => ###
          for (int column = 0; column < value; column++) {
            if (column < data.Columns)
              newVariableNames[column] = variableNames[column];
            else
              newVariableNames[column] = "Var" + column.ToString(formatString);
          }
          variableNames = new StringArray(newVariableNames);
          Data = new DoubleMatrix(newValues);
        }
      }
    }

    public bool Validate(string value, out string errorMessage) {
      errorMessage = string.Empty;
      return true;
    }

    public string GetValue(int rowIndex, int columnIndex) {
      if (rowIndex == 0) {
        // return variable name
        return variableNames[columnIndex];
      } else {
        return data[rowIndex - 1, columnIndex].ToString();
      }
    }

    public bool SetValue(string value, int rowIndex, int columnIndex) {
      if (rowIndex == 0) {
        // check if the variable name is already used
        if (variableNames.Contains(value)) {
          return false;
        } else {
          variableNames[columnIndex] = value;
          return true;
        }
      } else {
        double v;
        if (double.TryParse(value, out v)) {
          data[rowIndex - 1, columnIndex] = v;
          return true;
        } else return false;
      }
    }

    public event EventHandler<EventArgs<int, int>> ItemChanged;

    #endregion
  }
}
