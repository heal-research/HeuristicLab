#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("EC3A5009-EE84-4E1A-A537-20F6F1224842")]
  [Item("CorrelationSplitter", "An experimental split selector that uses correlation coefficients")]
  public sealed class CorrelationSplitter : SplitterBase {
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
    private CorrelationSplitter(StorableConstructorFlag _) { }
    private CorrelationSplitter(CorrelationSplitter original, Cloner cloner) : base(original, cloner) { }
    public CorrelationSplitter() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(OrderParameterName, "The exponent in the split calculation ssrLeft^(1/Order)+ssrRight^(1/Order) (default=1.0).", new DoubleValue(1)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CorrelationSplitter(this, cloner);
    }
    #endregion

    #region ISplitType
    protected override void AttributeSplit(IReadOnlyList<double> attValues, IReadOnlyList<double> targetValues, int minLeafSize, out int leftSize, out double maxImpurity, out double splitValue) {
      leftSize = -1;
      splitValue = double.MinValue;
      maxImpurity = double.NegativeInfinity;
      var splitValues = new List<double>();
      var splitSizes = new List<int>();
      var length = attValues.Count;

      var start = minLeafSize;
      while (start < length && attValues[start - 1].IsAlmost(attValues[start]))
        start++;
      if (start >= length) return;

      var imp = new CorreleationImpurityCalculator(minLeafSize, attValues, targetValues, Order);
      maxImpurity = imp.Impurity;
      splitValues.Add(imp.SplitValue);
      splitSizes.Add(imp.LeftSize);

      while (imp.LeftSize < length - minLeafSize) {
        imp.Increment();
        if (!imp.ValidPosition) continue; //splits can not be made between to equal points

        if (imp.Impurity.IsAlmost(maxImpurity)) {
          splitValues.Add(imp.SplitValue);
          splitSizes.Add(imp.LeftSize);
          continue;
        }

        if (imp.Impurity < maxImpurity) continue;
        splitValues.Clear();
        splitSizes.Clear();
        maxImpurity = imp.Impurity;
        splitValues.Add(imp.SplitValue);
        splitSizes.Add(imp.LeftSize);
      }

      var j = splitSizes.Count / 2;
      if (splitSizes.Count == 0) return;
      splitValue = splitValues[j];
      leftSize = splitSizes[j];
    }
    #endregion
  }
}