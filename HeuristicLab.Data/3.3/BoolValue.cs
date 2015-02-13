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
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("BoolValue", "Represents a boolean value.")]
  [StorableClass]
  public class BoolValue : ValueTypeValue<bool>, IComparable, IStringConvertibleValue {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Field; }
    }

    protected BoolValue(BoolValue original, Cloner cloner)
      : base(original, cloner) {
    }
    public BoolValue() : base() { }
    [StorableConstructor]
    public BoolValue(bool value)
      : base(value) {
      //mkommend: Be aware that the base call refers to the storable ctor => the value is set explicitly in the ctor body.
      //          This should not affect the persistence, because first the ctor is called and afterwards the values are set by reflection.
      this.value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BoolValue(this, cloner);
    }

    public virtual int CompareTo(object obj) {
      BoolValue other = obj as BoolValue;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }

    protected virtual bool Validate(string value, out string errorMessage) {
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
    protected virtual string GetValue() {
      return Value.ToString();
    }
    protected virtual bool SetValue(string value) {
      bool val;
      if (bool.TryParse(value, out val)) {
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
