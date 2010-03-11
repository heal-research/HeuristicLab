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
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("StringMatrixData", "Represents a matrix of strings.")]
  [Creatable("Test")]
  [StorableClass(StorableClassType.MarkedOnly)]
  public sealed class StringMatrixData : Item, IEnumerable, IStringConvertibleMatrixData {
    [Storable]
    private string[,] array;

    public int Rows {
      get { return array.GetLength(0); }
      private set {
        if (value != Rows) {
          string[,] newArray = new string[value, Columns];
          Array.Copy(array, newArray, Math.Min(value * Columns, array.Length));
          array = newArray;
          OnReset();
        }
      }
    }
    public int Columns {
      get { return array.GetLength(1); }
      private set {
        if (value != Columns) {
          string[,] newArray = new string[Rows, value];
          for (int i = 0; i < Rows; i++)
            Array.Copy(array, i * Columns, newArray, i * value, Math.Min(value, Columns));
          array = newArray;
          OnReset();
        }
      }
    }
    public string this[int rowIndex, int columnIndex] {
      get { return array[rowIndex, columnIndex]; }
      set {
        if (value != array[rowIndex, columnIndex]) {
          if ((value != null) || (array[rowIndex, columnIndex] != string.Empty)) {
            array[rowIndex, columnIndex] = value != null ? value : string.Empty;
            OnItemChanged(rowIndex, columnIndex);
          }
        }
      }
    }

    public StringMatrixData() {
      array = new string[0, 0];
    }
    public StringMatrixData(int rows, int columns) {
      array = new string[rows, columns];
      for (int i = 0; i < array.GetLength(0); i++) {
        for (int j = 0; j < array.GetLength(1); j++)
          array[i, j] = string.Empty;
      }
    }
    public StringMatrixData(string[,] elements) {
      if (elements == null) throw new ArgumentNullException();
      array = new string[elements.GetLength(0), elements.GetLength(1)];
      for (int i = 0; i < array.GetLength(0); i++) {
        for (int j = 0; j < array.GetLength(1); j++)
          array[i, j] = elements[i, j] == null ? string.Empty : elements[i, j];
      }
    }
    private StringMatrixData(StringMatrixData elements) {
      if (elements == null) throw new ArgumentNullException();
      array = (string[,])elements.array.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      StringMatrixData clone = new StringMatrixData(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      if (array.Length > 0) {
        for (int i = 0; i < Rows; i++) {
          sb.Append("[").Append(array[i, 0]);
          for (int j = 1; j < Columns; j++)
            sb.Append(";").Append(array[i, j]);
          sb.Append("]");
        }
      }
      sb.Append("]");
      return sb.ToString();
    }

    public IEnumerator GetEnumerator() {
      return array.GetEnumerator();
    }

    #region IStringConvertibleMatrixData Members
    int IStringConvertibleMatrixData.Rows {
      get { return Rows; }
      set { Rows = value; }
    }
    int IStringConvertibleMatrixData.Columns {
      get { return Columns; }
      set { Columns = value; }
    }

    bool IStringConvertibleMatrixData.Validate(string value, out string errorMessage) {
      if (value == null) {
        errorMessage = "Invalid Value (string must not be null)";
        return false;
      } else {
        errorMessage = string.Empty;
        return true;
      }
    }
    string IStringConvertibleMatrixData.GetValue(int rowIndex, int columIndex) {
      return this[rowIndex, columIndex];
    }
    bool IStringConvertibleMatrixData.SetValue(string value, int rowIndex, int columnIndex) {
      if (value != null) {
        this[rowIndex, columnIndex] = value;
        return true;
      } else {
        return false;
      }
    }
    public event EventHandler<EventArgs<int, int>> ItemChanged;
    private void OnItemChanged(int rowIndex, int columnIndex) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int, int>(rowIndex, columnIndex));
      OnToStringChanged();
    }
    public event EventHandler Reset;
    private void OnReset() {
      if (Reset != null)
        Reset(this, EventArgs.Empty);
      OnToStringChanged();
    }
    #endregion
  }
}
