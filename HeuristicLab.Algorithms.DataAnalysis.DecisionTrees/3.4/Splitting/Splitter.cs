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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("502B1429-7A28-45C1-A60A-93E72CB3AF4A")]
  [Item("Splitter", "A split selector that uses the ratio between Variances^(1/Order) to determine good splits.")]
  public sealed class Splitter : SplitterBase {
    public const string OrderParameterName = "Order";
    public IFixedValueParameter<DoubleValue> OrderParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[OrderParameterName]; }
    }
    public double Order {
      get { return OrderParameter.Value.Value; }
      set { OrderParameter.Value.Value = value; }
    }

    #region Constructors & Cloning
    [StorableConstructor]
    private Splitter(StorableConstructorFlag _) { }
    private Splitter(Splitter original, Cloner cloner) : base(original, cloner) { }
    public Splitter() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(OrderParameterName, "The exponent in the split calculation sum (x_i - x_avg)^Order (default=5).", new DoubleValue(5)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Splitter(this, cloner);
    }
    #endregion

    protected override void AttributeSplit(IReadOnlyList<double> attValues, IReadOnlyList<double> targetValues, int minLeafSize, out int position, out double maxImpurity, out double splitValue) {
      position = 0;
      maxImpurity = double.NegativeInfinity;
      splitValue = 0.0;
      var length = targetValues.Count;

      // weka code
      var low = 0;
      var high = length - 1;
      if (high - low + 1 < 4) return;
      var len = Math.Max(minLeafSize - 1, high - low + 1 < 5 ? 1 : (high - low + 1) / 5);
      position = low;
      var part = low + len - 1;
      var imp = new OrderImpurityCalculator(part + 1, targetValues, Order);

      for (var i = low + len; i < high - len; i++) {
        imp.Increment(targetValues[i], OrderImpurityCalculator.IncrementType.Left);
        if (attValues[i].IsAlmost(attValues[i + 1])) continue; //splits can not be made between to equal points
        if (imp.Impurity < maxImpurity) continue;
        maxImpurity = imp.Impurity;
        splitValue = (attValues[i] + attValues[i + 1]) / 2;
        position = i;
      }
    }
  }
}