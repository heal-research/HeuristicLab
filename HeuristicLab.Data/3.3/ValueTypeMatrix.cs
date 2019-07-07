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
using System.Drawing;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Data {
  [Item("ValueTypeMatrix", "An abstract base class for representing matrices of value types.")]
  [StorableType("4EDE6858-4628-4F21-8F27-A45C59C2DE36")]
  public abstract class ValueTypeMatrix<T> : Item, IEnumerable<T> where T : struct {
    private const int maximumToStringLength = 100;

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Class; }
    }

    [Storable]
    protected T[,] matrix;

    [Storable]
    protected List<string> columnNames;
    public virtual IEnumerable<string> ColumnNames {
      get { return this.columnNames; }
      set {
        if (ReadOnly) throw new NotSupportedException("ColumnNames cannot be set. ValueTypeMatrix is read-only.");
        if (value == null || value.Count() == 0)
          columnNames = new List<string>();
        else if (value.Count() != Columns)
          throw new ArgumentException("A column name must be specified for each column.");
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
        if (ReadOnly) throw new NotSupportedException("RowNames cannot be set. ValueTypeMatrix is read-only.");
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
        if (ReadOnly) throw new NotSupportedException("SortableView cannot be set. ValueTypeMatrix is read-only.");
        if (value != sortableView) {
          sortableView = value;
          OnSortableViewChanged();
        }
      }
    }

    public virtual int Rows {
      get { return matrix.GetLength(0); }
      protected set {
        if (ReadOnly) throw new NotSupportedException("Rows cannot be set. ValueTypeMatrix is read-only.");
        if (value != Rows) {
          T[,] newArray = new T[value, Columns];
          Array.Copy(matrix, newArray, Math.Min(value * Columns, matrix.Length));
          matrix = newArray;
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
        if (ReadOnly) throw new NotSupportedException("Columns cannot be set. ValueTypeMatrix is read-only.");
        if (value != Columns) {
          T[,] newArray = new T[Rows, value];
          for (int i = 0; i < Rows; i++)
            Array.Copy(matrix, i * Columns, newArray, i * value, Math.Min(value, Columns));
          matrix = newArray;
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
    public virtual T this[int rowIndex, int columnIndex] {
      get { return matrix[rowIndex, columnIndex]; }
      set {
        if (ReadOnly) throw new NotSupportedException("Item cannot be set. ValueTypeMatrix is read-only.");
        if (!value.Equals(matrix[rowIndex, columnIndex])) {
          matrix[rowIndex, columnIndex] = value;
          OnItemChanged(rowIndex, columnIndex);
        }
      }
    }

    [Storable]
    protected bool readOnly;
    public virtual bool ReadOnly {
      get { return readOnly; }
    }

    [StorableConstructor]
    protected ValueTypeMatrix(StorableConstructorFlag _) : base(_) { }
    protected ValueTypeMatrix(ValueTypeMatrix<T> original, Cloner cloner)
      : base(original, cloner) {
      this.matrix = (T[,])original.matrix.Clone();
      this.columnNames = new List<string>(original.columnNames);
      this.rowNames = new List<string>(original.rowNames);
      this.sortableView = original.sortableView;
      this.readOnly = original.readOnly;
    }
    protected ValueTypeMatrix() {
      matrix = new T[0, 0];
      columnNames = new List<string>();
      rowNames = new List<string>();
      sortableView = false;
      readOnly = false;
    }
    protected ValueTypeMatrix(int rows, int columns) {
      matrix = new T[rows, columns];
      columnNames = new List<string>();
      rowNames = new List<string>();
      sortableView = false;
      readOnly = false;
    }
    protected ValueTypeMatrix(int rows, int columns, IEnumerable<string> columnNames)
      : this(rows, columns) {
      ColumnNames = columnNames;
    }
    protected ValueTypeMatrix(int rows, int columns, IEnumerable<string> columnNames, IEnumerable<string> rowNames)
      : this(rows, columns, columnNames) {
      RowNames = rowNames;
    }
    protected ValueTypeMatrix(T[,] elements) {
      if (elements == null) throw new ArgumentNullException();
      matrix = (T[,])elements.Clone();
      columnNames = new List<string>();
      rowNames = new List<string>();
      sortableView = false;
      readOnly = false;
    }
    protected ValueTypeMatrix(T[,] elements, IEnumerable<string> columnNames)
      : this(elements) {
      ColumnNames = columnNames;
    }
    protected ValueTypeMatrix(T[,] elements, IEnumerable<string> columnNames, IEnumerable<string> rowNames)
      : this(elements, columnNames) {
      RowNames = rowNames;
    }

    public virtual ValueTypeMatrix<T> AsReadOnly() {
      ValueTypeMatrix<T> readOnlyValueTypeMatrix = (ValueTypeMatrix<T>)this.Clone();
      readOnlyValueTypeMatrix.readOnly = true;
      return readOnlyValueTypeMatrix;
    }

   
    public T[,] CloneAsMatrix() {
      //mkommend: this works because T must be a value type (struct constraint);
      return (T[,])matrix.Clone();
    }

    public virtual IEnumerable<T> GetRow(int row) {
      for (var col = 0; col < Columns; col++) {
        yield return matrix[row, col];
      }
    }

    public virtual IEnumerable<T> GetColumn(int col) {
      for (var row = 0; row < Rows; row++) {
        yield return matrix[row, col];
      }
    }

    public override string ToString() {
      if (matrix.Length == 0) return "[]";

      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      for (int i = 0; i < Rows; i++) {
        sb.Append("[").Append(matrix[i, 0].ToString());
        for (int j = 1; j < Columns; j++)
          sb.Append(";").Append(matrix[i, j].ToString());
        sb.Append("]");

        if (sb.Length > maximumToStringLength) {
          sb.Append("[...]");
          break;
        }
      }
      sb.Append("]");

      return sb.ToString();
    }

    public virtual IEnumerator<T> GetEnumerator() {
      return matrix.Cast<T>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
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
  }
}
