#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("StringMatrix", "Represents a matrix of strings.")]
  [StorableClass]
  public class StringMatrix : Item, IEnumerable<string>, IStringConvertibleMatrix {
    private const int maximumToStringLength = 100;

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Class; }
    }

    [Storable]
    protected string[,] matrix;

    [Storable]
    protected List<string> columnNames;
    public virtual IEnumerable<string> ColumnNames {
      get { return this.columnNames; }
      set {
        if (ReadOnly) throw new NotSupportedException("ColumnNames cannot be set. StringMatrix is read-only.");
        if (value == null || value.Count() == 0)
          columnNames = new List<string>();
        else if (value.Count() != Columns)
          throw new ArgumentException("A column name must be specified for each column .");
        else
          columnNames = new List<string>(value);
        OnColumnNamesChanged();
      }
    }
    [Storable]
    protected List<string> rowNames;
    public virtual IEnumerable<string> RowNames {
      get { return this.rowNames; }
      set {
        if (ReadOnly) throw new NotSupportedException("RowNames cannot be set. StringMatrix is read-only.");
        if (value == null || value.Count() == 0)
          rowNames = new List<string>();
        else if (value.Count() != Rows)
          throw new ArgumentException("A row name must be specified for each row.");
        else
          rowNames = new List<string>(value);
        OnRowNamesChanged();
      }
    }
    [Storable]
    protected bool sortableView;
    public virtual bool SortableView {
      get { return sortableView; }
      set {
        if (ReadOnly) throw new NotSupportedException("SortableView cannot be set. StringMatrix is read-only.");
        if (value != sortableView) {
          sortableView = value;
          OnSortableViewChanged();
        }
      }
    }

    public virtual int Rows {
      get { return matrix.GetLength(0); }
      protected set {
        if (ReadOnly) throw new NotSupportedException("Rows cannot be set. StringMatrix is read-only.");
        if (value != Rows) {
          string[,] newMatrix = new string[value, Columns];
          Array.Copy(matrix, newMatrix, Math.Min(value * Columns, matrix.Length));
          matrix = newMatrix;
          while (rowNames.Count > value)
            rowNames.RemoveAt(rowNames.Count - 1);
          while (rowNames.Count < value)
            rowNames.Add("Row " + rowNames.Count);
          OnRowsChanged();
          OnRowNamesChanged();
          OnReset();
        }
      }
    }
    public virtual int Columns {
      get { return matrix.GetLength(1); }
      protected set {
        if (ReadOnly) throw new NotSupportedException("Columns cannot be set. StringMatrix is read-only.");
        if (value != Columns) {
          string[,] newMatrix = new string[Rows, value];
          for (int i = 0; i < Rows; i++)
            Array.Copy(matrix, i * Columns, newMatrix, i * value, Math.Min(value, Columns));
          matrix = newMatrix;
          while (columnNames.Count > value)
            columnNames.RemoveAt(columnNames.Count - 1);
          while (columnNames.Count < value)
            columnNames.Add("Column " + columnNames.Count);
          OnColumnsChanged();
          OnColumnNamesChanged();
          OnReset();
        }
      }
    }
    public virtual string this[int rowIndex, int columnIndex] {
      get { return matrix[rowIndex, columnIndex]; }
      set {
        if (ReadOnly) throw new NotSupportedException("Item cannot be set. StringMatrix is read-only.");
        if (value != matrix[rowIndex, columnIndex]) {
          if ((value != null) || (matrix[rowIndex, columnIndex] != string.Empty)) {
            matrix[rowIndex, columnIndex] = value != null ? value : string.Empty;
            OnItemChanged(rowIndex, columnIndex);
          }
        }
      }
    }

    [Storable]
    protected bool readOnly;
    public virtual bool ReadOnly {
      get { return readOnly; }
    }

    [StorableConstructor]
    protected StringMatrix(bool deserializing) : base(deserializing) { }
    protected StringMatrix(StringMatrix original, Cloner cloner)
      : base(original, cloner) {
      this.matrix = (string[,])original.matrix.Clone();
      this.columnNames = new List<string>(original.columnNames);
      this.rowNames = new List<string>(original.rowNames);
      this.sortableView = original.sortableView;
      this.readOnly = original.readOnly;
    }
    public StringMatrix() {
      matrix = new string[0, 0];
      columnNames = new List<string>();
      rowNames = new List<string>();
      sortableView = false;
      readOnly = false;
    }
    public StringMatrix(int rows, int columns) {
      matrix = new string[rows, columns];
      for (int i = 0; i < matrix.GetLength(0); i++) {
        for (int j = 0; j < matrix.GetLength(1); j++)
          matrix[i, j] = string.Empty;
      }
      columnNames = new List<string>();
      rowNames = new List<string>();
      sortableView = false;
      readOnly = false;
    }
    protected StringMatrix(int rows, int columns, IEnumerable<string> columnNames)
      : this(rows, columns) {
      ColumnNames = columnNames;
    }
    protected StringMatrix(int rows, int columns, IEnumerable<string> columnNames, IEnumerable<string> rowNames)
      : this(rows, columns, columnNames) {
      RowNames = rowNames;
    }
    public StringMatrix(string[,] elements) {
      if (elements == null) throw new ArgumentNullException();
      matrix = new string[elements.GetLength(0), elements.GetLength(1)];
      for (int i = 0; i < matrix.GetLength(0); i++) {
        for (int j = 0; j < matrix.GetLength(1); j++)
          matrix[i, j] = elements[i, j] == null ? string.Empty : elements[i, j];
      }
      columnNames = new List<string>();
      rowNames = new List<string>();
      sortableView = false;
      readOnly = false;
    }
    protected StringMatrix(string[,] elements, IEnumerable<string> columnNames)
      : this(elements) {
      ColumnNames = columnNames;
    }
    protected StringMatrix(string[,] elements, IEnumerable<string> columnNames, IEnumerable<string> rowNames)
      : this(elements, columnNames) {
      RowNames = rowNames;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StringMatrix(this, cloner);
    }

    public virtual StringMatrix AsReadOnly() {
      StringMatrix readOnlyStringMatrix = (StringMatrix)this.Clone();
      readOnlyStringMatrix.readOnly = true;
      return readOnlyStringMatrix;
    }

    public override string ToString() {
      if (matrix.Length == 0) return "[]";

      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      for (int i = 0; i < Rows; i++) {
        sb.Append("[").Append(matrix[i, 0]);
        for (int j = 1; j < Columns; j++)
          sb.Append(";").Append(matrix[i, j]);
        sb.Append("]");

        if (sb.Length > maximumToStringLength) {
          sb.Append("[...]");
          break;
        }
      }
      sb.Append("]");

      return sb.ToString();
    }

    public virtual IEnumerator<string> GetEnumerator() {
      return matrix.Cast<string>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    protected virtual bool Validate(string value, out string errorMessage) {
      if (value == null) {
        errorMessage = "Invalid Value (string must not be null)";
        return false;
      } else {
        errorMessage = string.Empty;
        return true;
      }
    }
    protected virtual string GetValue(int rowIndex, int columIndex) {
      return this[rowIndex, columIndex];
    }
    protected virtual bool SetValue(string value, int rowIndex, int columnIndex) {
      if (value != null) {
        this[rowIndex, columnIndex] = value;
        return true;
      } else {
        return false;
      }
    }

    #region events
    public event EventHandler ColumnsChanged;
    protected virtual void OnColumnsChanged() {
      EventHandler handler = ColumnsChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler RowsChanged;
    protected virtual void OnRowsChanged() {
      EventHandler handler = RowsChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler ColumnNamesChanged;
    protected virtual void OnColumnNamesChanged() {
      EventHandler handler = ColumnNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler RowNamesChanged;
    protected virtual void OnRowNamesChanged() {
      EventHandler handler = RowNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler SortableViewChanged;
    protected virtual void OnSortableViewChanged() {
      EventHandler handler = SortableViewChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<int, int>> ItemChanged;
    protected virtual void OnItemChanged(int rowIndex, int columnIndex) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int, int>(rowIndex, columnIndex));

      //approximation to avoid firing of unnecessary ToStringChangedEvents
      //columnIndex is not used, because always full rows are returned in the ToString method
      if (rowIndex * Columns < maximumToStringLength)
        OnToStringChanged();
    }
    public event EventHandler Reset;
    protected virtual void OnReset() {
      if (Reset != null)
        Reset(this, EventArgs.Empty);
      OnToStringChanged();
    }
    #endregion

    #region IStringConvertibleMatrix Members
    int IStringConvertibleMatrix.Rows {
      get { return Rows; }
      set { Rows = value; }
    }
    int IStringConvertibleMatrix.Columns {
      get { return Columns; }
      set { Columns = value; }
    }
    bool IStringConvertibleMatrix.Validate(string value, out string errorMessage) {
      return Validate(value, out errorMessage);
    }
    string IStringConvertibleMatrix.GetValue(int rowIndex, int columIndex) {
      return GetValue(rowIndex, columIndex);
    }
    bool IStringConvertibleMatrix.SetValue(string value, int rowIndex, int columnIndex) {
      return SetValue(value, rowIndex, columnIndex);
    }
    #endregion
  }
}
