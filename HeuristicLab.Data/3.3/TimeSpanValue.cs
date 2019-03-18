#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("TimeSpanValue", "Represents a duration of time.")]
  [StorableClass]
  public class TimeSpanValue : ValueTypeValue<TimeSpan>, IComparable, IStringConvertibleValue {
    [StorableConstructor]
    protected TimeSpanValue(bool deserializing) : base(deserializing) { }
    protected TimeSpanValue(TimeSpanValue original, Cloner cloner)
      : base(original, cloner) {
    }
    public TimeSpanValue() : base() { }
    public TimeSpanValue(TimeSpan value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TimeSpanValue(this, cloner);
    }

    public virtual int CompareTo(object obj) {
      TimeSpanValue other = obj as TimeSpanValue;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }

    protected virtual bool Validate(string value, out string errorMessage) {
      TimeSpan val;
      bool valid = TimeSpan.TryParseExact(value, "c", CultureInfo.CurrentCulture, out val);
      errorMessage = string.Empty;
      if (!valid) {
        StringBuilder sb = new StringBuilder();
        sb.Append("Invalid Value (Valid Value Format: \"");
        sb.Append(FormatPatterns.GetTimeSpanFormatPattern());
        sb.Append("\")");
        errorMessage = sb.ToString();
      }
      return valid;
    }
    protected virtual string GetValue() {
      return Value.ToString("c");
    }
    protected virtual bool SetValue(string value) {
      TimeSpan val;
      if (TimeSpan.TryParseExact(value, "c", CultureInfo.CurrentCulture, out val)) {
        Value = val;
        return true;
      } else {
        return false;
      }
    }
    public override string ToString() {
      return Value.ToString("c");
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
