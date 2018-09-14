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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// Modifies the value by quadratic fall (slow fall initially, fast fall to the end) or rise (slow rise initally, fast rise to the end).
  /// </summary>
  [Item("QuadraticDiscreteDoubleValueModifier", "Modifies the value by quadratic fall (slow fall initially, fast fall to the end) or rise (slow rise initally, fast rise to the end).")]
  [StorableClass]
  public class QuadraticDiscreteDoubleValueModifier : DiscreteDoubleValueModifier {
    [StorableConstructor]
    protected QuadraticDiscreteDoubleValueModifier(bool deserializing) : base(deserializing) { }
    protected QuadraticDiscreteDoubleValueModifier(QuadraticDiscreteDoubleValueModifier original, Cloner cloner) : base(original, cloner) { }
    public QuadraticDiscreteDoubleValueModifier() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QuadraticDiscreteDoubleValueModifier(this, cloner);
    }

    protected override double Modify(double value, double startValue, double endValue, int index, int startIndex, int endIndex) {
      double a = (endValue - startValue) / ((endIndex - startIndex) * (endIndex - startIndex));
      return a * (index - startIndex) * (index - startIndex) + startValue;
    }
  }
}
