#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("PercentValue", "Represents a double value in percent.")]
  [StorableClass]
  public class PercentValue : DoubleValue {
    [Storable(DefaultValue = false)]
    private bool restrictToUnitInterval = false;
    public bool RestrictToUnitInterval {
      get { return restrictToUnitInterval; }
    }

    public override double Value {
      get { return base.Value; }
      set {
        if (restrictToUnitInterval && (value < 0 || value > 1))
          throw new ArgumentException("Value must lie in the interval [0,1].");
        base.Value = value;
      }
    }

    [StorableConstructor]
    protected PercentValue(bool deserializing) : base(deserializing) { }
    protected PercentValue(PercentValue original, Cloner cloner)
      : base(original, cloner) {
      restrictToUnitInterval = original.restrictToUnitInterval;
    }
    public PercentValue() : base() { }
    public PercentValue(double value) : base(value) { }

    public PercentValue(double value, bool restrictToUnitInterval)
      : base() {
      this.restrictToUnitInterval = restrictToUnitInterval;
      if (restrictToUnitInterval && (value < 0 || value > 1))
        throw new ArgumentException("Value must lie in the interval [0,1].");
      this.value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PercentValue(this, cloner);
    }

    public override string ToString() {
      return Value.ToString("#0.#################### %");  // percent format
    }

    protected override bool Validate(string value, out string errorMessage) {
      value = value.Replace("%", " ");
      bool valid = base.Validate(value, out errorMessage);
      if (!restrictToUnitInterval || !valid) return valid;

      double val = double.Parse(value);
      if (val < 0 || val > 1) {
        errorMessage = "Value must lie in the interval [0,1].";
        return false;
      }
      return true;
    }

    protected override string GetValue() {
      return Value.ToString("#0.#################### %");  // percent format
    }
    protected override bool SetValue(string value) {
      bool percent = value.Contains("%");
      value = value.Replace("%", " ");
      double val;
      if (double.TryParse(value, out val)) {
        if (percent) {
          if (!(val).IsAlmost(Value * 100.0))
            Value = val == 0 ? 0 : val / 100.0;
        } else {
          Value = val;
        }
        return true;
      } else {
        return false;
      }
    }
  }
}
