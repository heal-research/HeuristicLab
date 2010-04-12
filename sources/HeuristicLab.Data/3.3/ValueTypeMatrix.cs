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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("ValueTypeMatrix<T>", "An abstract base class for representing matrices of value types.")]
  [StorableClass]
  public abstract class ValueTypeMatrix<T> : Item, IEnumerable where T : struct {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Class; }
    }

    [Storable]
    protected T[,] matrix;

    [Storable]
    private List<string> columnNames;
    public IEnumerable<string> ColumnNames {
      get { return this.columnNames; }
      set {
        if (value == null || value.Count() == 0)
          columnNames = new List<string>();
        else if (value.Count() != Columns)
          throw new ArgumentException("A column name must be specified for each column.");
        else
          columnNames = new List<string>(value);
      }
    }
    [Storable]
    private List<string> rowNames;
    public IEnumerable<string> RowNames {
      get { return this.rowNames; }
      set {
        if (value == null || value.Count() == 0)
          rowNames = new List<string>();
        else if (value.Count() != Rows)
          throw new ArgumentException("A row name must be specified for each row.");
        else
          rowNames = new List<string>(value);
      }
    }

    public virtual int Rows {
      get { return matrix.GetLength(0); }
      protected set {
        if (value != Rows) {
          T[,] newArray = new T[value, Columns];
          Array.Copy(matrix, newArray, Math.Min(value * Columns, matrix.Length));
          matrix = newArray;
          OnReset();
        }
      }
    }
    public virtual int Columns {
      get { return matrix.GetLength(1); }
      protected set {
        if (value != Columns) {
          T[,] newArray = new T[Rows, value];
          for (int i = 0; i < Rows; i++)
            Array.Copy(matrix, i * Columns, newArray, i * value, Math.Min(value, Columns));
          matrix = newArray;
          OnReset();
        }
      }
    }
    public virtual T this[int rowIndex, int columnIndex] {
      get { return matrix[rowIndex, columnIndex]; }
      set {
        if (!value.Equals(matrix[rowIndex, columnIndex])) {
          matrix[rowIndex, columnIndex] = value;
          OnItemChanged(rowIndex, columnIndex);
        }
      }
    }

    protected ValueTypeMatrix() {
      matrix = new T[0, 0];
      columnNames = new List<string>();
      rowNames = new List<string>();
    }
    protected ValueTypeMatrix(int rows, int columns) {
      matrix = new T[rows, columns];
      columnNames = new List<string>();
      rowNames = new List<string>();
    }
    protected ValueTypeMatrix(int rows, int columns, IEnumerable<string> columnNames)
      : this(rows, columns) {
      ColumnNames = columnNames;
    }
    protected ValueTypeMatrix(int rows, int columns, IEnumerable<string> columnNames,IEnumerable<string> rowNames)
      : this(rows, columns, columnNames) {
      RowNames = rowNames;
    }
    protected ValueTypeMatrix(T[,] elements) {
      if (elements == null) throw new ArgumentNullException();
      matrix = (T[,])elements.Clone();
      columnNames = new List<string>();
      rowNames = new List<string>();
    }
    protected ValueTypeMatrix(T[,] elements, IEnumerable<string> columnNames)
      : this(elements) {
      ColumnNames = columnNames;
    }
    protected ValueTypeMatrix(T[,] elements, IEnumerable<string> columnNames, IEnumerable<string> rowNames)
      : this(elements,columnNames) {
      RowNames = rowNames;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ValueTypeMatrix<T> clone = (ValueTypeMatrix<T>)base.Clone(cloner);
      clone.matrix = (T[,])matrix.Clone();
      clone.columnNames = new List<string>(columnNames);
      clone.rowNames = new List<string>(rowNames);
      return clone;
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      if (matrix.Length > 0) {
        for (int i = 0; i < Rows; i++) {
          sb.Append("[").Append(matrix[i, 0].ToString());
          for (int j = 1; j < Columns; j++)
            sb.Append(";").Append(matrix[i, j].ToString());
          sb.Append("]");
        }
      }
      sb.Append("]");
      return sb.ToString();
    }

    public virtual IEnumerator GetEnumerator() {
      return matrix.GetEnumerator();
    }

    public event EventHandler<EventArgs<int, int>> ItemChanged;
    protected virtual void OnItemChanged(int rowIndex, int columnIndex) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int, int>(rowIndex, columnIndex));
      OnToStringChanged();
    }
    public event EventHandler Reset;
    protected virtual void OnReset() {
      if (Reset != null)
        Reset(this, EventArgs.Empty);
      OnToStringChanged();
    }
  }
}
