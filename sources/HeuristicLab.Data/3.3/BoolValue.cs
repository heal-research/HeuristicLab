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
  [Item("BoolValue", "Represents a boolean value.")]
  [Creatable("Test")]
  [StorableClass]
  public sealed class BoolValue : ValueTypeValue<bool>, IComparable, IStringConvertibleValue {
    public BoolValue() : base() { }
    public BoolValue(bool value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      BoolValue clone = new BoolValue(Value);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public int CompareTo(object obj) {
      BoolValue other = obj as BoolValue;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }

    #region IStringConvertibleValue Members
    bool IStringConvertibleValue.Validate(string value, out string errorMessage) {
      bool val;
      bool valid = bool.TryParse(value, out val);
      errorMessage = string.Empty;
      if (!valid) {
        StringBuilder sb = new StringBuilder();
        sb.Append("Invalid Value (Valid Value Format: \"");
        sb.Append(FormatPatterns.GetBoolFormatPattern());
        sb.Append("\")");
        errorMessage = sb.ToString();
      }
      return valid;
    }
    string IStringConvertibleValue.GetValue() {
      return Value.ToString();
    }
    bool IStringConvertibleValue.SetValue(string value) {
      bool val;
      if (bool.TryParse(value, out val)) {
        Value = val;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}
