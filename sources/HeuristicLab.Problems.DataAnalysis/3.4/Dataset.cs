#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("Dataset", "Represents a dataset containing data that should be analyzed.")]
  [StorableClass]
  public sealed class Dataset : NamedItem, IStringConvertibleMatrix {
    [StorableConstructor]
    private Dataset(bool deserializing) : base(deserializing) { }
    private Dataset(Dataset original, Cloner cloner)
      : base(original, cloner) {
      variableNameToVariableIndexMapping = original.variableNameToVariableIndexMapping;
      data = original.data;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Dataset(this, cloner);
    }

    public Dataset()
      : base() {
      Name = "-";
      VariableNames = Enumerable.Empty<string>();
      data = new double[0, 0];
    }

    public Dataset(IEnumerable<string> variableNames, double[,] data)
      : base() {
      Name = "-";
      if (variableNames.Count() != data.GetLength(1)) {
        throw new ArgumentException("Number of variable names doesn't match the number of columns of data");
      }
      this.data = (double[,])data.Clone();
      VariableNames = variableNames;
    }


    private Dictionary<string, int> variableNameToVariableIndexMapping;
    private Dictionary<int, string> variableIndexToVariableNameMapping;
    [Storable]
    public IEnumerable<string> VariableNames {
      get {
        // convert KeyCollection to an array first for persistence
        return variableNameToVariableIndexMapping.Keys.ToArray();
      }
      private set {
        if (variableNameToVariableIndexMapping != null) throw new InvalidOperationException("VariableNames can only be set once.");
        this.variableNameToVariableIndexMapping = new Dictionary<string, int>();
        this.variableIndexToVariableNameMapping = new Dictionary<int, string>();
        int i = 0;
        foreach (string variableName in value) {
          this.variableNameToVariableIndexMapping.Add(variableName, i);
          this.variableIndexToVariableNameMapping.Add(i, variableName);
          i++;
        }
      }
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
    public double[] GetVariableValues(string variableName) {
      return GetVariableValues(GetVariableIndex(variableName), 0, Rows);
    }
    public double[] GetVariableValues(int variableIndex, int start, int end) {
      return GetEnumeratedVariableValues(variableIndex, start, end).ToArray();
    }
    public double[] GetVariableValues(string variableName, int start, int end) {
      return GetVariableValues(GetVariableIndex(variableName), start, end);
    }

    public IEnumerable<double> GetEnumeratedVariableValues(int variableIndex) {
      return GetEnumeratedVariableValues(variableIndex, 0, Rows);
    }
    public IEnumerable<double> GetEnumeratedVariableValues(int variableIndex, int start, int end) {
      if (start < 0 || !(start <= end))
        throw new ArgumentException("Start must be between 0 and end (" + end + ").");
      if (end > Rows || end < start)
        throw new ArgumentException("End must be between start (" + start + ") and dataset rows (" + Rows + ").");

      for (int i = start; i < end; i++)
        yield return data[i, variableIndex];
    }
    public IEnumerable<double> GetEnumeratedVariableValues(int variableIndex, IEnumerable<int> rows) {
      foreach (int row in rows)
        yield return data[row, variableIndex];
    }

    public IEnumerable<double> GetEnumeratedVariableValues(string variableName) {
      return GetEnumeratedVariableValues(GetVariableIndex(variableName), 0, Rows);
    }
    public IEnumerable<double> GetEnumeratedVariableValues(string variableName, int start, int end) {
      return GetEnumeratedVariableValues(GetVariableIndex(variableName), start, end);
    }
    public IEnumerable<double> GetEnumeratedVariableValues(string variableName, IEnumerable<int> rows) {
      return GetEnumeratedVariableValues(GetVariableIndex(variableName), rows);
    }

    public string GetVariableName(int variableIndex) {
      try {
        return variableIndexToVariableNameMapping[variableIndex];
      }
      catch (KeyNotFoundException ex) {
        throw new ArgumentException("The variable index " + variableIndex + " was not found.", ex);
      }
    }
    public int GetVariableIndex(string variableName) {
      try {
        return variableNameToVariableIndexMapping[variableName];
      }
      catch (KeyNotFoundException ex) {
        throw new ArgumentException("The variable name " + variableName + " was not found.", ex);
      }
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
      get { return Enumerable.Empty<string>(); }
      set { throw new NotSupportedException(); }
    }

    public string GetValue(int rowIndex, int columnIndex) {
      return data[rowIndex, columnIndex].ToString();
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
