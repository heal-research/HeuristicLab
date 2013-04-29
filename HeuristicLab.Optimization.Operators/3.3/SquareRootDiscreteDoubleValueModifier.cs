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
  /// Modifies the value by square rooted fall (fast fall initially, slow fall to the end) or rise (fast rise initially, slow rise to the end).
  /// </summary>
  [Item("SquareRootDiscreteDoubleValueModifier", "Modifies the value by square rooted fall (fast fall initially, slow fall to the end) or rise (fast rise initially, slow rise to the end).")]
  [StorableClass]
  public class SquareRootDiscreteDoubleValueModifier : DiscreteDoubleValueModifier {
    [StorableConstructor]
    protected SquareRootDiscreteDoubleValueModifier(bool deserializing) : base(deserializing) { }
    protected SquareRootDiscreteDoubleValueModifier(SquareRootDiscreteDoubleValueModifier original, Cloner cloner) : base(original, cloner) { }
    public SquareRootDiscreteDoubleValueModifier() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SquareRootDiscreteDoubleValueModifier(this, cloner);
    }

    protected override double Modify(double value, double startValue, double endValue, int index, int startIndex, int endIndex) {
      double a = (endValue - startValue) / Math.Sqrt(endIndex - startIndex);
      return a * Math.Sqrt(index - startIndex) + startValue;
    }
  }
}
