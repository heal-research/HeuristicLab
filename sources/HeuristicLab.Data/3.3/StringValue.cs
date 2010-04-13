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
using System.Drawing;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("StringValue", "Represents a string.")]
  [StorableClass]
  public class StringValue : Item, IComparable, IStringConvertibleValue {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Field; }
    }

    [Storable]
    protected string value;
    public virtual string Value {
      get { return value; }
      set {
        if (value != this.value) {
          if ((value != null) || (this.value != string.Empty)) {
            this.value = value != null ? value : string.Empty;
            OnValueChanged();
          }
        }
      }
    }

    public StringValue() {
      this.value = string.Empty;
    }
    public StringValue(string value) {
      this.value = value != null ? value : string.Empty;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      StringValue clone = new StringValue(value);
      cloner.RegisterClonedObject(this, clone);
      clone.ReadOnlyView = ReadOnlyView;
      return clone;
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
