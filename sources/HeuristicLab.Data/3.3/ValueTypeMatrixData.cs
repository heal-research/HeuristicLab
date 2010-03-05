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
using System.Text;
using System.Xml;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("ValueTypeMatrixData<T>", "A base class for representing matrices of value types.")]
  public class ValueTypeMatrixData<T> : Item, IEnumerable where T : struct {
    [Storable]
    private T[,] array;

    public int Rows {
      get { return array.GetLength(0); }
      protected set {
        if (value != Rows) {
          T[,] newArray = new T[value, Columns];
          Array.Copy(array, newArray, Math.Min(value * Columns, array.Length));
          array = newArray;
          OnReset();
        }
      }
    }
    public int Columns {
      get { return array.GetLength(1); }
      protected set {
        if (value != Columns) {
          T[,] newArray = new T[Rows, value];
          for (int i = 0; i < Rows; i++)
            Array.Copy(array, i * Columns, newArray, i * value, Math.Min(value, Columns));
          array = newArray;
          OnReset();
        }
      }
    }
    public T this[int rowIndex, int columnIndex] {
      get { return array[rowIndex, columnIndex]; }
      set {
        if (!value.Equals(array[rowIndex, columnIndex])) {
          array[rowIndex, columnIndex] = value;
          OnItemChanged(rowIndex, columnIndex);
        }
      }
    }

    public ValueTypeMatrixData() {
      array = new T[0, 0];
    }
    public ValueTypeMatrixData(int rows, int columns) {
      array = new T[rows, columns];
    }
    public ValueTypeMatrixData(T[,] elements) {
      if (elements == null) throw new ArgumentNullException();
      array = (T[,])elements.Clone();
    }
    protected ValueTypeMatrixData(ValueTypeMatrixData<T> elements) {
      if (elements == null) throw new ArgumentNullException();
      array = (T[,])elements.array.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ValueTypeMatrixData<T> clone = (ValueTypeMatrixData<T>)base.Clone(cloner);
      clone.array = (T[,])array.Clone();
      return clone;
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      if (array.Length > 0) {
        for (int i = 0; i < Rows; i++) {
          sb.Append("[").Append(array[i, 0].ToString());
          for (int j = 1; j < Columns; j++)
            sb.Append(";").Append(array[i, j].ToString());
          sb.Append("]");
        }
      }
      sb.Append("]");
      return sb.ToString();
    }

    public IEnumerator GetEnumerator() {
      return array.GetEnumerator();
    }

    protected event EventHandler<EventArgs<int, int>> ItemChanged;
    private void OnItemChanged(int rowIndex, int columnIndex) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int, int>(rowIndex, columnIndex));
      OnToStringChanged();
    }
    protected event EventHandler Reset;
    private void OnReset() {
      if (Reset != null)
        Reset(this, EventArgs.Empty);
      OnToStringChanged();
    }
  }
}
