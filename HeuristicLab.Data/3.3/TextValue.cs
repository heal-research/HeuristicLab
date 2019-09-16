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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  [Item("TextValue", "Represents a multiline string.")]
  [StorableType("136E9CD4-D340-44B1-A7CE-CF1A70D1C032")]
  public class TextValue : StringValue, ITextValue {

    public TextValue() : base() { }
    public TextValue(string value, bool @readonly = false) : base(value ?? string.Empty, @readonly) { }

    [StorableConstructor]
    protected TextValue(StorableConstructorFlag _) : base(_) { }

    protected TextValue(TextValue original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TextValue(this, cloner);
    }
  }
}
