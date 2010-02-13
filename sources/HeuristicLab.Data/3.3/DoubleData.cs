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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [EmptyStorableClass]
  [Item("DoubleData", "Represents a double value.")]
  [Creatable("Test")]
  public sealed class DoubleData : ValueTypeData<double>, IComparable, IStringConvertibleData {
    public DoubleData() : base() { }
    public DoubleData(double value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      DoubleData clone = new DoubleData(Value);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public override string ToString() {
      return Value.ToString("r");  // round-trip format
    }

    public int CompareTo(object obj) {
      DoubleData other = obj as DoubleData;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }

    #region IStringConvertibleData Members
    bool IStringConvertibleData.Validate(string value, out string errorMessage) {
      double val;
      bool valid = double.TryParse(value, out val);
      errorMessage = string.Empty;
      if (!valid) {
        StringBuilder sb = new StringBuilder();
        sb.Append("Invalid Value (Valid Value Format: \"");
        sb.Append(FormatPatterns.GetDoubleFormatPattern());
        sb.Append("\")");
        errorMessage = sb.ToString();
      }
      return valid;
    }
    string IStringConvertibleData.GetValue() {
      return Value.ToString("r");  // round-trip format
    }
    bool IStringConvertibleData.SetValue(string value) {
      double val;
      if (double.TryParse(value, out val)) {
        Value = val;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}
