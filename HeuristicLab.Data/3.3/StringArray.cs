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
  [Item("StringArray", "Represents an array of strings.")]
  [StorableClass]
  public class StringArray : Item, IEnumerable<string>, IStringConvertibleArray {
    private const int maximumToStringLength = 100;

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Class; }
    }

    [Storable]
    protected string[] array;

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
      protected set {
        if (ReadOnly) throw new NotSupportedException("Length cannot be set. StringArray is read-only.");
        if (value != Length) {
          Array.Resize<string>(ref array, value);
          while (elementNames.Count > value)
            elementNames.RemoveAt(elementNames.Count - 1);
          OnElementNamesChanged();
          OnReset();
        }
      }
    }
    public virtual string this[int index] {
      get { return array[index]; }
      set {
        if (ReadOnly) throw new NotSupportedException("Item cannot be set. StringArray is read-only.");
        if (value != array[index]) {
          if ((value != null) || (array[index] != string.Empty)) {
            array[index] = value != null ? value : string.Empty;
            OnItemChanged(index);
          }
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
    protected StringArray(bool deserializing) : base(deserializing) { }
    protected StringArray(StringArray original, Cloner cloner)
      : base(original, cloner) {
      this.array = (string[])original.array.Clone();
      this.readOnly = original.readOnly;
      this.elementNames = new List<string>(original.elementNames);
    }
    public StringArray() {
      array = new string[0];
      readOnly = false;
      elementNames = new List<string>();
    }
    public StringArray(int length) {
      array = new string[length];
      for (int i = 0; i < array.Length; i++)
        array[i] = string.Empty;
      readOnly = false;
      elementNames = new List<string>();
    }
    public StringArray(string[] elements) {
      if (elements == null) throw new ArgumentNullException();
      array = new string[elements.Length];
      for (int i = 0; i < array.Length; i++)
        array[i] = elements[i] == null ? string.Empty : elements[i];
      readOnly = false;
      elementNames = new List<string>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StringArray(this, cloner);
    }

    public virtual StringArray AsReadOnly() {
      StringArray readOnlyStringArray = (StringArray)this.Clone();
      readOnlyStringArray.readOnly = true;
      return readOnlyStringArray;
    }

    public override string ToString() {
      if (array.Length == 0) return "[]";

      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      sb.Append(array[0]);
      for (int i = 1; i < array.Length; i++) {
        sb.Append(";").Append(array[i]);
        if (sb.Length > maximumToStringLength) {
          sb.Append("...");
          break;
        }
      }
      sb.Append("]");
      return sb.ToString();
    }

    public virtual IEnumerator<string> GetEnumerator() {
      return array.Cast<string>().GetEnumerator();
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
    protected virtual string GetValue(int index) {
      return this[index];
    }
    protected virtual bool SetValue(string value, int index) {
      if (value != null) {
        this[index] = value;
        return true;
      } else {
        return false;
      }
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

    #region IStringConvertibleArray Members
    int IStringConvertibleArray.Length {
      get { return Length; }
      set { Length = value; }
    }
    bool IStringConvertibleArray.Validate(string value, out string errorMessage) {
      return Validate(value, out errorMessage);
    }
    string IStringConvertibleArray.GetValue(int index) {
      return GetValue(index);
    }
    bool IStringConvertibleArray.SetValue(string value, int index) {
      return SetValue(value, index);
    }
    #endregion
  }
}
