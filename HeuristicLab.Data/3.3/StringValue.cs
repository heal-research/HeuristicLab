#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("StringValue", "Represents a string.")]
  [StorableClass]
  public class StringValue : Item, IComparable, IStringConvertibleValue {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Field; }
    }

    [Storable]
    protected string value;
    public virtual string Value {
      get { return value; }
      set {
        if (ReadOnly) throw new NotSupportedException("Value cannot be set. StringValue is read-only.");
        if (value != this.value) {
          if ((value != null) || (this.value != string.Empty)) {
            this.value = value != null ? value : string.Empty;
            OnValueChanged();
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
    protected StringValue(bool deserializing) : base(deserializing) { }
    protected StringValue(StringValue original, Cloner cloner)
      : base(original, cloner) {
      this.value = original.value != null ? original.value : string.Empty;
      this.readOnly = original.readOnly;
    }
    public StringValue() {
      this.value = string.Empty;
      this.readOnly = false;
    }
    public StringValue(string value) {
      this.value = value != null ? value : string.Empty;
      this.readOnly = false;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StringValue(this, cloner);
    }

    public virtual StringValue AsReadOnly() {
      StringValue readOnlyStringValue = (StringValue)this.Clone();
      readOnlyStringValue.readOnly = true;
      return readOnlyStringValue;
    }

    public override string ToString() {
      return value;
    }

    public virtual int CompareTo(object obj) {
      StringValue other = obj as StringValue;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }

    public event EventHandler ValueChanged;
    protected virtual void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, EventArgs.Empty);
      OnToStringChanged();
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
    protected virtual string GetValue() {
      return Value;
    }
    protected virtual bool SetValue(string value) {
      if (value != null) {
        Value = value;
        return true;
      } else {
        return false;
      }
    }

    #region IStringConvertibleValue Members
    bool IStringConvertibleValue.Validate(string value, out string errorMessage) {
      return Validate(value, out errorMessage);
    }
    string IStringConvertibleValue.GetValue() {
      return GetValue();
    }
    bool IStringConvertibleValue.SetValue(string value) {
      return SetValue(value);
    }
    #endregion
  }
}
