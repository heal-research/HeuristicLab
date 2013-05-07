#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("ValueTypeArray", "An abstract base class for representing arrays of value types.")]
  [StorableClass]
  public abstract class ValueTypeArray<T> : Item, IEnumerable<T> where T : struct {
    private const int maximumToStringLength = 100;

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Class; }
    }

    [Storable]
    protected T[] array;

    public virtual int Length {
      get { return array.Length; }
      #region Mono Compatibility
      // this setter should be protected, but the Mono compiler couldn't handle it
      set {
        if (ReadOnly) throw new NotSupportedException("Length cannot be set. ValueTypeArray is read-only.");
        if (value != Length) {
          Array.Resize<T>(ref array, value);
          OnReset();
        }
      }
      #endregion
    }
    public virtual T this[int index] {
      get { return array[index]; }
      set {
        if (ReadOnly) throw new NotSupportedException("Item cannot be set. ValueTypeArray is read-only.");
        if (!value.Equals(array[index])) {
          array[index] = value;
          OnItemChanged(index);
        }
      }
    }

    [Storable]
    protected bool readOnly;
    public virtual bool ReadOnly {
      get { return readOnly; }
    }

    [StorableConstructor]
    protected ValueTypeArray(bool deserializing) : base(deserializing) { }
    protected ValueTypeArray(ValueTypeArray<T> original, Cloner cloner)
      : base(original, cloner) {
      this.array = (T[])original.array.Clone();
      this.readOnly = original.readOnly;
    }
    protected ValueTypeArray() {
      array = new T[0];
      readOnly = false;
    }
    protected ValueTypeArray(int length) {
      array = new T[length];
      readOnly = false;
    }
    protected ValueTypeArray(T[] elements) {
      if (elements == null) throw new ArgumentNullException();
      array = (T[])elements.Clone();
      readOnly = false;
    }

    public virtual ValueTypeArray<T> AsReadOnly() {
      ValueTypeArray<T> readOnlyValueTypeArray = (ValueTypeArray<T>)this.Clone();
      readOnlyValueTypeArray.readOnly = true;
      return readOnlyValueTypeArray;
    }

    public override string ToString() {
      if (array.Length == 0) return "[]";

      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      sb.Append(array[0].ToString());
      for (int i = 1; i < array.Length; i++) {
        sb.Append(";").Append(array[i].ToString());
        if (sb.Length > maximumToStringLength) {
          sb.Append("...");
          break;
        }
      }
      sb.Append("]");
      return sb.ToString();
    }

    public virtual IEnumerator<T> GetEnumerator() {
      return array.Cast<T>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public event EventHandler<EventArgs<int>> ItemChanged;
    protected virtual void OnItemChanged(int index) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int>(index));
      if (index < maximumToStringLength)
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
