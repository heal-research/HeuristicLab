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

using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("BoolArray", "Represents an array of boolean values.")]
  [Creatable("Test")]
  [StorableClass]
  public sealed class BoolArray : ValueTypeArray<bool>, IStringConvertibleArray {
    public BoolArray() : base() { }
    public BoolArray(int length) : base(length) { }
    public BoolArray(bool[] elements) : base(elements) { }
    private BoolArray(BoolArray elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      BoolArray clone = new BoolArray(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    #region IStringConvertibleArray Members
    int IStringConvertibleArray.Length {
      get { return Length; }
      set { Length = value; }
    }

    bool IStringConvertibleArray.Validate(string value, out string errorMessage) {
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
    string IStringConvertibleArray.GetValue(int index) {
      return this[index].ToString();
    }
    bool IStringConvertibleArray.SetValue(string value, int index) {
      bool val;
      if (bool.TryParse(value, out val)) {
        this[index] = val;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}
