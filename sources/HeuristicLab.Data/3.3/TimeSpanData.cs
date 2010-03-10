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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [StorableClass(StorableClassType.Empty)]
  [Item("TimeSpanData", "Represents a duration of time.")]
  [Creatable("Test")]
  public sealed class TimeSpanData : ValueTypeData<TimeSpan>, IComparable, IStringConvertibleData {
    public TimeSpanData() : base() { }
    public TimeSpanData(TimeSpan value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      TimeSpanData clone = new TimeSpanData(Value);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public int CompareTo(object obj) {
      TimeSpanData other = obj as TimeSpanData;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }

    #region IStringConvertibleData Members
    bool IStringConvertibleData.Validate(string value, out string errorMessage) {
      TimeSpan val;
      bool valid = TimeSpan.TryParse(value, out val);
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
    string IStringConvertibleData.GetValue() {
      return Value.ToString();
    }
    bool IStringConvertibleData.SetValue(string value) {
      TimeSpan val;
      if (TimeSpan.TryParse(value, out val)) {
        Value = val;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}
