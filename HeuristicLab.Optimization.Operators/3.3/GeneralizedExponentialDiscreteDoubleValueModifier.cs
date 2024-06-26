﻿#region License Information
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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization.Operators {

  /// <summary>
  /// Modifies a value by exponentially.
  /// </summary>
  [Item("GeneralizedExponentialDiscreteDoubleValueModifier", @"Modifies the value exponentially.
The base is a standardized exponential function with exponent `Base`
that is then transformed to pass through the startValue at the startIndex
and through the endValue at the endIndex.
For a slow change initially use a value > 1 for `Base`.
For a steep change initially use a value < 1 for `Base`.
Negative slopes are automatically generated if the start value is greater than the end value.
If you use `base`=1 you will get a linear interpolation.")]
  [StorableType("349D17F2-44D8-46EB-813F-E6D6E73B007F")]
  public class GeneralizedExponentialDiscreteDoubleValueModifier : DiscreteDoubleValueModifier {

    protected ValueLookupParameter<DoubleValue> BaseParameter {
      get { return (ValueLookupParameter<DoubleValue>) Parameters["Base"]; }
    }
    private double Base { get { return BaseParameter.Value.Value; } }

    [StorableConstructor]
    protected GeneralizedExponentialDiscreteDoubleValueModifier(StorableConstructorFlag _) : base(_) { }
    protected GeneralizedExponentialDiscreteDoubleValueModifier(GeneralizedExponentialDiscreteDoubleValueModifier original, Cloner cloner) : base(original, cloner) { }
    public GeneralizedExponentialDiscreteDoubleValueModifier() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Base", "Base of the exponential function. Must be > 0. If > 1 steep in the end, if < 1 steep at the start, if == 1 linear interpolation.", new DoubleValue(0.00001)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GeneralizedExponentialDiscreteDoubleValueModifier(this, cloner);
    }

    protected override double Modify(double value, double startValue, double endValue, int index, int startIndex, int endIndex) {
      return Apply(value, startValue, endValue, index, startIndex, endIndex, Base);
    }

    /// <summary>
    /// Calculates a new value based on exponential decay or growth.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if Base &lt;= 0.</exception>
    /// <param name="value">The previous value.</param>
    /// <param name="startValue">The initial value.</param>
    /// <param name="endValue">The final value.</param>
    /// <param name="index">The current index.</param>
    /// <param name="startIndex">The initial index.</param>
    /// <param name="endIndex">The final index.</param>
    /// <returns>The new value.</returns>
    public static double Apply(double value, double startValue, double endValue, int index, int startIndex, int endIndex, double @base) {
      if (@base <= 0)
        throw new ArgumentException("Base must be > 0.");
      if (@base == 1.0)
        return startValue + (endValue - startValue) * (index - startIndex) / (endIndex - startIndex);
      return startValue + (endValue - startValue) * (Math.Pow(@base, 1.0 * (index - startIndex) / (endIndex - startIndex)) - 1) / (@base - 1);
    }
  }
}
