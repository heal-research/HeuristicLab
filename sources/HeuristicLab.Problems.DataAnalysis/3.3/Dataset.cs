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
    private Dictionary<int, Dictionary<int, double>>[] cachedMeans;
    private Dictionary<int, Dictionary<int, double>>[] cachedRanges;
    private bool cachedValuesInvalidated = true;

    public Dataset()
      : this(new double[,] { { 0.0 } }) {
    }

    public Dataset(double[,] data)
      : base() {
      Name = "-";
      Data = new DoubleMatrix(data);
      string formatString = new StringBuilder().Append('#', (int)Math.Log10(this.data.Columns) + 1).ToString(); // >= 100 variables => ###
      this.variableNames = new StringArray((from col in Enumerable.Range(1, this.data.Columns)
                                            select "Var" + col.ToString(formatString)).ToArray());
    }

    private StringArray variableNames;
    public IEnumerable<string> VariableNames {
      get { return variableNames; }
    }

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
      get { return VariableValues(VariableIndex(variableName), 0, data.Rows); }
    }

    public double[] VariableValues(int variableIndex, int start, int end) {
      if (start < 0 || !(start <= end))
        throw new ArgumentException("Start must be between 0 and end (" + end + ").");
      if (end > data.Rows || end < start)
        throw new ArgumentException("End must be between start (" + start + ") and dataset rows (" + data.Rows + ").");

      double[] values = new double[end - start];
      for (int i = 0; i < end - start; i++)
        values[i] = data[i + start, variableIndex];
      return values;
    }

    public double[] VariableValues(string variableName, int start, int end) {
      return VariableValues(VariableIndex(variableName), start, end);
    }

    #region Variable name methods
    public string VariableName(int variableIndex) {
      return variableNames[variableIndex];
    }

    public int VariableIndex(string variableName) {
      for (int i = 0; i < variableNames.Length; i++) {
        if (variableNames[i].Equals(variableName)) return i;
      }
      throw new ArgumentException("The variable name " + variableName + " was not found.");
    }

    public void SetVariableName(int variableIndex, string name) {
      variableNames[variableIndex] = name;
    }

    #endregion

    #region variable statistics
    public double Mean(string variableName) {
      return Mean(VariableIndex(variableName));
    }

    public double Mean(string variableName, int start, int end) {
      return Mean(VariableIndex(variableName), start, end);
    }

    public double Mean(int variableIndex) {
      return Mean(variableIndex, 0, data.Rows);
    }

    public double Mean(int variableIndex, int start, int end) {
      if (cachedValuesInvalidated) CreateDictionaries();
      if (!cachedMeans[variableIndex].ContainsKey(start) || !cachedMeans[variableIndex][start].ContainsKey(end)) {
        double mean = VariableValues(variableIndex, start, end).Average();
        if (!cachedMeans[variableIndex].ContainsKey(start)) cachedMeans[variableIndex][start] = new Dictionary<int, double>();
        cachedMeans[variableIndex][start][end] = mean;
        return mean;
      } else {
        return cachedMeans[variableIndex][start][end];
      }
    }

    public double Range(string variableName) {
      return Range(VariableIndex(variableName));
    }

    public double Range(int variableIndex) {
      return Range(variableIndex, 0, data.Rows);
    }

    public double Range(string variableName, int start, int end) {
      return Range(VariableIndex(variableName), start, end);
    }

    public double Range(int variableIndex, int start, int end) {
      if (cachedValuesInvalidated) CreateDictionaries();
      if (!cachedRanges[variableIndex].ContainsKey(start) || !cachedRanges[variableIndex][start].ContainsKey(end)) {
        var values = VariableValues(variableIndex, start, end);
        double range = values.Max() - values.Min();
        if (!cachedRanges[variableIndex].ContainsKey(start)) cachedRanges[variableIndex][start] = new Dictionary<int, double>();
        cachedRanges[variableIndex][start][end] = range;
        return range;
      } else {
        return cachedRanges[variableIndex][start][end];
      }
    }

    public double Max(string variableName) {
      return Max(VariableIndex(variableName));
    }

    public double Max(int variableIndex) {
      return Max(variableIndex, 0, data.Rows);
    }

    public double Max(string variableName, int start, int end) {
      return Max(VariableIndex(variableName), start, end);
    }

    public double Max(int variableIndex, int start, int end) {
      return VariableValues(variableIndex, start, end).Max();
    }

    public double Min(string variableName) {
      return Min(VariableIndex(variableName));
    }

    public double Min(int variableIndex) {
      return Min(variableIndex, 0, data.Rows);
    }

    public double Min(string variableName, int start, int end) {
      return Min(VariableIndex(variableName), start, end);
    }

    public double Min(int variableIndex, int start, int end) {
      return VariableValues(variableIndex, start, end).Min();
    }

    public int MissingValues(string variableName) {
      return MissingValues(VariableIndex(variableName));
    }
    public int MissingValues(int variableIndex) {
      return MissingValues(variableIndex, 0, data.Rows);
    }

    public int MissingValues(string variableName, int start, int end) {
      return MissingValues(VariableIndex(variableName), start, end);
    }

    public int MissingValues(int variableIndex, int start, int end) {
      return VariableValues(variableIndex, start, end).Count(x => double.IsNaN(x));
    }

    #endregion

    private void CreateDictionaries() {
      // keep a means and ranges dictionary for each column (possible target variable) of the dataset.
      cachedMeans = new Dictionary<int, Dictionary<int, double>>[data.Columns];
      cachedRanges = new Dictionary<int, Dictionary<int, double>>[data.Columns];
      for (int i = 0; i < data.Columns; i++) {
        cachedMeans[i] = new Dictionary<int, Dictionary<int, double>>();
        cachedRanges[i] = new Dictionary<int, Dictionary<int, double>>();
      }
      cachedValuesInvalidated = false;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Dataset clone = (Dataset)base.Clone(cloner);
      clone.data = (DoubleMatrix)data.Clone(cloner);
      clone.variableNames = (StringArray)variableNames.Clone(cloner);
      return clone;
    }

    #region events
    public event EventHandler<EventArgs<int, int>> DataChanged;
    private void OnDataChanged(EventArgs<int, int> e) {
      cachedValuesInvalidated = true;

      var listeners = DataChanged;
      if (listeners != null) listeners(this, e);
    }
    public event EventHandler Reset;
    private void OnReset(EventArgs e) {
      cachedValuesInvalidated = true;

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
          string formatString = new StringBuilder().Append('#', (int)Math.Log10(value) + 1).ToString(); // >= 100 variables => ###
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
        // set variable name
        variableNames[columnIndex] = value;
        return true;
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
