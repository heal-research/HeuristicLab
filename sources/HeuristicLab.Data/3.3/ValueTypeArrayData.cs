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
  [Item("ValueTypeArrayData<T>", "A base class for representing arrays of value types.")]
  public class ValueTypeArrayData<T> : Item, IEnumerable where T : struct {
    [Storable]
    private T[] array;

    public int Length {
      get { return array.Length; }
      protected set {
        if (value != Length) {
          Array.Resize<T>(ref array, value);
          OnReset();
        }
      }
    }
    public T this[int index] {
      get { return array[index]; }
      set {
        if (!value.Equals(array[index])) {
          array[index] = value;
          OnItemChanged(index);
        }
      }
    }

    public ValueTypeArrayData() {
      array = new T[0];
    }
    public ValueTypeArrayData(int length) {
      array = new T[length];
    }
    public ValueTypeArrayData(T[] elements) {
      if (elements == null) throw new ArgumentNullException();
      array = (T[])elements.Clone();
    }
    protected ValueTypeArrayData(ValueTypeArrayData<T> elements) {
      if (elements == null) throw new ArgumentNullException();
      array = (T[])elements.array.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ValueTypeArrayData<T> clone = (ValueTypeArrayData<T>)base.Clone(cloner);
      clone.array = (T[])array.Clone();
      return clone;
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      if (array.Length > 0) {
        sb.Append(array[0].ToString());
        for (int i = 1; i < array.Length; i++)
          sb.Append(";").Append(array[i].ToString());
      }
      sb.Append("]");
      return sb.ToString();
    }

    public IEnumerator GetEnumerator() {
      return array.GetEnumerator();
    }

    protected event EventHandler<EventArgs<int, int>> ItemChanged;
    private void OnItemChanged(int index) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int, int>(index, 0));
      OnChanged();
    }
    protected event EventHandler Reset;
    private void OnReset() {
      if (Reset != null)
        Reset(this, EventArgs.Empty);
      OnChanged();
    }
  }
}
