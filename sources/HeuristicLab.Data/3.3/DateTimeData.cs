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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [StorableClass(StorableClassType.Empty)]
  [Item("DateTimeData", "Represents a date and time value.")]
  [Creatable("Test")]
  public sealed class DateTimeData : ValueTypeData<DateTime>, IComparable, IStringConvertibleData {
    public DateTimeData() : base() { }
    public DateTimeData(DateTime value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      DateTimeData clone = new DateTimeData(Value);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public override string ToString() {
      return Value.ToString("o");  // round-trip format
    }

    public int CompareTo(object obj) {
      DateTimeData other = obj as DateTimeData;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }

    #region IStringConvertibleData Members
    bool IStringConvertibleData.Validate(string value, out string errorMessage) {
      DateTime val;
      bool valid = DateTime.TryParse(value, out val);
      errorMessage = valid ? string.Empty : "Invalid Value (values must be formatted according to the current culture settings)";
      return valid;
    }
    string IStringConvertibleData.GetValue() {
      return Value.ToString("o");  // round-trip format
    }
    bool IStringConvertibleData.SetValue(string value) {
      DateTime val;
      if (DateTime.TryParse(value, out val)) {
        Value = val;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}
