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
  [Item("StringArrayData", "Represents an array of strings.")]
  [Creatable("Test")]
  public sealed class StringArrayData : Item, IEnumerable, IStringConvertibleMatrixData {
    [Storable]
    private string[] array;

    public int Length {
      get { return array.Length; }
      private set {
        if (value != Length) {
          Array.Resize<string>(ref array, value);
          OnReset();
        }
      }
    }
    public string this[int index] {
      get { return array[index]; }
      set {
        if (value != array[index]) {
          if ((value != null) || (array[index] != string.Empty)) {
            array[index] = value != null ? value : string.Empty;
            OnItemChanged(index);
          }
        }
      }
    }

    public StringArrayData() {
      array = new string[0];
    }
    public StringArrayData(int length) {
      array = new string[length];
      for (int i = 0; i < array.Length; i++)
        array[i] = string.Empty;
    }
    public StringArrayData(string[] elements) {
      if (elements == null) throw new ArgumentNullException();
      array = new string[elements.Length];
      for (int i = 0; i < array.Length; i++)
        array[i] = elements[i] == null ? string.Empty : elements[i];
    }
    private StringArrayData(StringArrayData elements) {
      if (elements == null) throw new ArgumentNullException();
      array = (string[])elements.array.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      StringArrayData clone = new StringArrayData(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      if (array.Length > 0) {
        sb.Append(array[0]);
        for (int i = 1; i < array.Length; i++)
          sb.Append(";").Append(array[i]);
      }
      sb.Append("]");
      return sb.ToString();
    }

    public IEnumerator GetEnumerator() {
      return array.GetEnumerator();
    }

    #region IStringConvertibleMatrixData Members
    StringConvertibleArrayDataDimensions IStringConvertibleMatrixData.Dimensions {
      get { return StringConvertibleArrayDataDimensions.Rows; }
    }
    int IStringConvertibleMatrixData.Rows {
      get { return Length; }
      set { Length = value; }
    }
    int IStringConvertibleMatrixData.Columns {
      get { return 1; }
      set { throw new NotSupportedException("Columns cannot be changed."); }
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
      return this[rowIndex];
    }
    bool IStringConvertibleMatrixData.SetValue(string value, int rowIndex, int columnIndex) {
      if (value != null) {
        this[rowIndex] = value;
        return true;
      } else {
        return false;
      }
    }
    private event EventHandler<EventArgs<int, int>> ItemChanged;
    event EventHandler<EventArgs<int, int>> IStringConvertibleMatrixData.ItemChanged {
      add { ItemChanged += value; }
      remove { ItemChanged -= value; }
    }
    private void OnItemChanged(int index) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int, int>(index, 0));
      OnChanged();
    }
    private event EventHandler Reset;
    event EventHandler IStringConvertibleMatrixData.Reset {
      add { Reset += value; }
      remove { Reset -= value; }
    }
    private void OnReset() {
      if (Reset != null)
        Reset(this, EventArgs.Empty);
      OnChanged();
    }
    #endregion
  }
}
