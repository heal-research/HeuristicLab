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

using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("IntArray", "Represents an array of integer values.")]
  [StorableClass]
  public class IntArray : StringConvertibleArray<int> {
    [StorableConstructor]
    protected IntArray(bool deserializing) : base(deserializing) { }
    protected IntArray(IntArray original, Cloner cloner)
      : base(original, cloner) {
    }
    public IntArray() : base() { }
    public IntArray(int length) : base(length) { }
    public IntArray(int[] elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntArray(this, cloner);
    }

    protected override bool Validate(string value, out string errorMessage) {
      int val;
      bool valid = int.TryParse(value, out val);
      errorMessage = string.Empty;
      if (!valid) {
        StringBuilder sb = new StringBuilder();
        sb.Append("Invalid Value (Valid Value Format: \"");
        sb.Append(FormatPatterns.GetIntFormatPattern());
        sb.Append("\")");
        errorMessage = sb.ToString();
      }
      return valid;
    }
    protected override string GetValue(int index) {
      return this[index].ToString();
    }
    protected override bool SetValue(string value, int index) {
      int val;
      if (int.TryParse(value, out val)) {
        this[index] = val;
        return true;
      } else {
        return false;
      }
    }
  }
}
