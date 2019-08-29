#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Data {
  [Item("BoolArray", "Represents an array of boolean values.")]
  [StorableType("CBDDC242-3F94-4CA0-B2A8-E3B10F07DFB8")]
  public class BoolArray : StringConvertibleArray<bool> {
    [StorableConstructor]
    protected BoolArray(StorableConstructorFlag _) : base(_) { }
    protected BoolArray(BoolArray original, Cloner cloner)
      : base(original, cloner) {
    }
    public BoolArray() : base() { }
    public BoolArray(int length) : base(length) { }
    public BoolArray(bool[] elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BoolArray(this, cloner);
    }

    protected override bool Validate(string value, out string errorMessage) {
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
    protected override string GetValue(int index) {
      return this[index].ToString();
    }
    protected override bool SetValue(string value, int index) {
      bool val;
      if (bool.TryParse(value, out val)) {
        this[index] = val;
        return true;
      } else {
        return false;
      }
    }
  }
}
