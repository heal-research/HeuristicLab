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
  [Item("ValueTypeArray", "An abstract base class for representing arrays of value types.")]
  [StorableType("6A05E421-E015-44A8-959F-5711CF9400A9")]
  public abstract class ValueTypeArray<T> : Item, IValueTypeArray<T> where T : struct {
    private const int maximumToStringLength = 100;

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Class; }
    }

    [Storable]
    protected T[] array;

    [Storable]
    protected List<string> elementNames;
    public virtual IEnumerable<string> ElementNames {
      get { return this.elementNames; }
      set {
        if (ReadOnly) throw new NotSupportedException("ElementNames cannot be set. ValueTypeArray is read-only.");
        if (value == null || !value.Any())
          elementNames = new List<string>();
        else if (value.Count() > Length)
          throw new ArgumentException("The number of element names must not exceed the array length.");
        else
          elementNames = new List<string>(value);
        OnElementNamesChanged();
      }
    }

    public virtual int Length {
      get { return array.Length; }
      #region Mono Compatibility
      // this setter should be protected, but the Mono compiler couldn't handle it
      set {
        if (ReadOnly) throw new NotSupportedException("Length cannot be set. ValueTypeArray is read-only.");
        if (value != Length) {
          Array.Resize<T>(ref array, value);
          while (elementNames.Count > value)
            elementNames.RemoveAt(elementNames.Count - 1);
          OnElementNamesChanged();
          OnReset();
        }
      }
      #endregion
    }

    public int Count
    {
      get { return Length; }
    }

    [Storable]
    protected bool resizable = true;
    public bool Resizable {
      get { return resizable; }
      set {
        if (resizable != value) {
          resizable = value;
          OnResizableChanged();
        }
      }
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

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (elementNames == null) { elementNames = new List<string>(); }
    }

    [StorableConstructor]
    protected ValueTypeArray(StorableConstructorFlag _) : base(_) { }
    protected ValueTypeArray(ValueTypeArray<T> original, Cloner cloner)
      : base(original, cloner) {
      this.array = (T[])original.array.Clone();
      this.readOnly = original.readOnly;
      this.resizable = original.resizable;
      this.elementNames = new List<string>(original.elementNames);
    }
    protected ValueTypeArray() {
      array = new T[0];
      readOnly = false;
      resizable = true;
      elementNames = new List<string>();
    }
    protected ValueTypeArray(int length) {
      array = new T[length];
      readOnly = false;
      resizable = true;
      elementNames = new List<string>();
    }
    protected ValueTypeArray(T[] elements) {
      if (elements == null) throw new ArgumentNullException();
      array = (T[])elements.Clone();
      readOnly = false;
      resizable = true;
      elementNames = new List<string>();
    }

    public virtual IValueTypeArray AsReadOnly() {
      ValueTypeArray<T> readOnlyValueTypeArray = (ValueTypeArray<T>)this.Clone();
      readOnlyValueTypeArray.readOnly = true;
      return readOnlyValueTypeArray;
    }

    public T[] CloneAsArray() {
      //mkommend: this works because T must be a value type (struct constraint);
      return (T[])array.Clone();
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

    public event EventHandler ResizableChanged;
    protected virtual void OnResizableChanged() {
      EventHandler handler = ResizableChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler ElementNamesChanged;
    protected virtual void OnElementNamesChanged() {
      EventHandler handler = ElementNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
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
