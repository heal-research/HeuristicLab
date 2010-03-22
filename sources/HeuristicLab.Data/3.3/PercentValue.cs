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
  [Item("PercentValue", "Represents a double value in percent.")]
  [StorableClass]
  public class PercentValue : DoubleValue {
    public PercentValue() : base() { }
    public PercentValue(double value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      PercentValue clone = new PercentValue(value);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public override string ToString() {
      return (Value * 100).ToString("r") + " %";  // round-trip format
    }

    protected override bool Validate(string value, out string errorMessage) {
      value = value.Replace("%", string.Empty);
      return base.Validate(value, out errorMessage);
    }
    protected override string GetValue() {
      return (Value * 100).ToString("r") + " %";  // round-trip format
    }
    protected override bool SetValue(string value) {
      value = value.Replace("%", string.Empty);
      double val;
      if (double.TryParse(value, out val)) {
        Value = val == 0 ? 0 : val / 100;
        return true;
      } else {
        return false;
      }
    }
  }
}
