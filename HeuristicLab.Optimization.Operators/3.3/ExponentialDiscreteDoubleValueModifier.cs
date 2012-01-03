#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// Modifies the value by exponential fall (steep fall initially, slow fall to the end) or rise (slow rise initially, fast rise to the end).
  /// </summary>
  [Item("ExponentialDiscreteDoubleValueModifier", "Modifies the value by exponential fall (steep fall initially, slow fall to the end) or rise (slow rise initially, fast rise to the end).")]
  [StorableClass]
  public class ExponentialDiscreteDoubleValueModifier : DiscreteDoubleValueModifier {
    [StorableConstructor]
    protected ExponentialDiscreteDoubleValueModifier(bool deserializing) : base(deserializing) { }
    protected ExponentialDiscreteDoubleValueModifier(ExponentialDiscreteDoubleValueModifier original, Cloner cloner) : base(original, cloner) { }
    public ExponentialDiscreteDoubleValueModifier() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExponentialDiscreteDoubleValueModifier(this, cloner);
    }

    /// <summary>
    /// Calculates a new value based on exponential decay or growth.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when endValue or startValue or both are 0.</exception>
    /// <param name="value">The last value.</param>
    /// <param name="startValue">The start value.</param>
    /// <param name="endValue">The end value.</param>
    /// <param name="index">The current index.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="endIndex">The end index.</param>
    /// <returns>The new value.</returns>
    protected override double Modify(double value, double startValue, double endValue, int index, int startIndex, int endIndex) {
      if (endValue <= 0 || startValue <= 0) throw new ArgumentException("ExponentialDiscreteDoubleValueModifier: startValue and endValue must be greater than 0.");
      double b = Math.Pow(endValue / startValue, 1.0 / (endIndex - startIndex));
      return startValue * Math.Pow(b, index - startIndex);
    }
  }
}
