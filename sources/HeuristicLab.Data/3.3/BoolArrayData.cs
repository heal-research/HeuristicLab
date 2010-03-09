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
  [EmptyStorableClass]
  [Item("BoolArrayData", "Represents an array of boolean values.")]
  [Creatable("Test")]
  public sealed class BoolArrayData : ValueTypeArrayData<bool>, IStringConvertibleArrayData {
    public BoolArrayData() : base() { }
    public BoolArrayData(int length) : base(length) { }
    public BoolArrayData(bool[] elements) : base(elements) { }
    private BoolArrayData(BoolArrayData elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      BoolArrayData clone = new BoolArrayData(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    #region IStringConvertibleArrayData Members
    int IStringConvertibleArrayData.Rows {
      get { return Length; }
      set { Length = value; }
    }

    bool IStringConvertibleArrayData.Validate(string value, out string errorMessage) {
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
    string IStringConvertibleArrayData.GetValue(int index) {
      return this[index].ToString();
    }
    bool IStringConvertibleArrayData.SetValue(string value, int index) {
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
