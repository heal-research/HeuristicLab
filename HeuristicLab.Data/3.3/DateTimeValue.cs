#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("DateTimeValue", "Represents a date and time value.")]
  [StorableClass]
  public class DateTimeValue : ValueTypeValue<DateTime>, IComparable, IStringConvertibleValue {
    [StorableConstructor]
    protected DateTimeValue(bool deserializing) : base(deserializing) { }
    protected DateTimeValue(DateTimeValue original, Cloner cloner)
      : base(original, cloner) {
    }
    public DateTimeValue() : base() { }
    public DateTimeValue(DateTime value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DateTimeValue(this, cloner);
    }

    public override string ToString() {
      return Value.ToString("o");  // round-trip format
    }

    public virtual int CompareTo(object obj) {
      DateTimeValue other = obj as DateTimeValue;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }

    protected virtual bool Validate(string value, out string errorMessage) {
      DateTime val;
      bool valid = DateTime.TryParse(value, out val);
      errorMessage = valid ? string.Empty : "Invalid Value (values must be formatted according to the current culture settings)";
      return valid;
    }
    protected virtual string GetValue() {
      return Value.ToString("o");  // round-trip format
    }
    protected virtual bool SetValue(string value) {
      DateTime val;
      if (DateTime.TryParse(value, out val)) {
        Value = val;
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
