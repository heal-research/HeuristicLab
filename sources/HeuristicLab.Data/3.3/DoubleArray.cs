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
  [Item("DoubleArray", "Represents an array of double values.")]
  [Creatable("Test")]
  [StorableClass]
  public sealed class DoubleArray : ValueTypeArray<double>, IStringConvertibleArray {
    public DoubleArray() : base() { }
    public DoubleArray(int length) : base(length) { }
    public DoubleArray(double[] elements) : base(elements) { }
    private DoubleArray(DoubleArray elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      DoubleArray clone = new DoubleArray(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    #region IStringConvertibleArray Members
    int IStringConvertibleArray.Length {
      get { return Length; }
      set { Length = value; }
    }

    bool IStringConvertibleArray.Validate(string value, out string errorMessage) {
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
    string IStringConvertibleArray.GetValue(int index) {
      return this[index].ToString();
    }
    bool IStringConvertibleArray.SetValue(string value, int index) {
      double val;
      if (double.TryParse(value, out val)) {
        this[index] = val;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}
