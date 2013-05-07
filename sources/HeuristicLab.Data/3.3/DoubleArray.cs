#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("DoubleArray", "Represents an array of double values.")]
  [StorableClass]
  public class DoubleArray : ValueTypeArray<double>, IStringConvertibleArray {
    [StorableConstructor]
    protected DoubleArray(bool deserializing) : base(deserializing) { }
    protected DoubleArray(DoubleArray original, Cloner cloner)
      : base(original, cloner) {
    }
    public DoubleArray() : base() { }
    public DoubleArray(int length) : base(length) { }
    public DoubleArray(double[] elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DoubleArray(this, cloner);
    }

    protected virtual bool Validate(string value, out string errorMessage) {
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
    protected virtual string GetValue(int index) {
      return this[index].ToString();
    }
    protected virtual bool SetValue(string value, int index) {
      double val;
      if (double.TryParse(value, out val)) {
        this[index] = val;
        return true;
      } else {
        return false;
      }
    }

    #region IStringConvertibleArray Members
    int IStringConvertibleArray.Length {
      get { return Length; }
      set { Length = value; }
    }
    bool IStringConvertibleArray.Validate(string value, out string errorMessage) {
      return Validate(value, out errorMessage);
    }
    string IStringConvertibleArray.GetValue(int index) {
      return GetValue(index);
    }
    bool IStringConvertibleArray.SetValue(string value, int index) {
      return SetValue(value, index);
    }
    #endregion
  }
}
