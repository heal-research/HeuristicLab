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
    [StorableConstructor]
    protected Dataset(bool deserializing)
      : base(deserializing) {
    }

    public Dataset(IEnumerable<string> variableNames, double[,] data)
      : base() {
      Name = "-";
      if (variableNames.Count() != data.GetLength(1)) {
        throw new ArgumentException("Number of variable names doesn't match the number of columns of data");
      }
      this.data = (double[,])data.Clone();
      this.variableNames = variableNames.ToArray();
    }

    [Storable]
    private string[] variableNames;
    public IEnumerable<string> VariableNames {
      get { return variableNames; }
    }

    [Storable]
    private double[,] data;
    private double[,] Data {
      get { return data; }
    }

    // elementwise access
    public double this[int rowIndex, int columnIndex] {
      get { return data[rowIndex, columnIndex]; }
    }

    public double this[string variableName, int rowIndex] {
      get {
        int columnIndex = GetVariableIndex(variableName);
        return data[rowIndex, columnIndex];
      }
    }

    public double[] GetVariableValues(int variableIndex) {
      return GetVariableValues(variableIndex, 0, Rows);
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

    public double[] GetVariableValues(string variableName) {
      return GetVariableValues(GetVariableIndex(variableName), 0, Rows);
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
    #endregion

    public double[,] GetClonedData() {
      return (double[,])data.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return this;
    }

    public event EventHandler Reset;
    private void OnReset(EventArgs e) {
      var listeners = Reset;
      if (listeners != null) listeners(this, e);
    }

    #region IStringConvertibleMatrix Members

    public int Rows {
      get { return data.GetLength(0); }
      set { throw new NotSupportedException(); }
    }

    public int Columns {
      get { return data.GetLength(1); }
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
      get { return new List<string>(); }
      set { throw new NotSupportedException(); }
    }

    public bool Validate(string value, out string errorMessage) {
      throw new NotSupportedException();
    }

    public string GetValue(int rowIndex, int columnIndex) {
      return data[rowIndex, columnIndex].ToString();
    }

    public bool SetValue(string value, int rowIndex, int columnIndex) {
      throw new NotSupportedException();
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
