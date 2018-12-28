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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator which calculates the minimum, average and maximum of a value in the scope tree.
  /// </summary>
  [Item("MinAverageMaxValueCalculator", "An operator which calculates the minimum, average and maximum of a value in the scope tree.")]
  [StorableType("C10C55BB-737F-4A0A-BAB0-F3DAC28FBCB8")]
  public sealed class MinAverageMaxValueCalculator : SingleSuccessorOperator {
    public ScopeTreeLookupParameter<DoubleValue> ValueParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Value"]; }
    }
    public ValueLookupParameter<DoubleValue> MinValueParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MinValue"]; }
    }
    public ValueLookupParameter<DoubleValue> AverageValueParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["AverageValue"]; }
    }
    public ValueLookupParameter<DoubleValue> MaxValueParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaxValue"]; }
    }

    #region Storing & Cloning
    [StorableConstructor]
    private MinAverageMaxValueCalculator(StorableConstructorFlag _) : base(_) { }
    private MinAverageMaxValueCalculator(MinAverageMaxValueCalculator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MinAverageMaxValueCalculator(this, cloner);
    }
    #endregion
    public MinAverageMaxValueCalculator()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Value", "The value contained in the scope tree for which the minimum, average and maximum should be calculated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MinValue", "The minimum of the value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AverageValue", "The average of the value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaxValue", "The maximum of the value."));
    }

    public override IOperation Apply() {
      ItemArray<DoubleValue> values = ValueParameter.ActualValue;

      if (values.Length > 0) {
        double min = double.MaxValue, max = double.MinValue, sum = 0.0;
        for (int i = 0; i < values.Length; i++) {
          if (values[i].Value < min) min = values[i].Value;
          if (values[i].Value > max) max = values[i].Value;
          sum += values[i].Value;
        }

        DoubleValue minValue = MinValueParameter.ActualValue;
        if (minValue == null) MinValueParameter.ActualValue = new DoubleValue(min);
        else minValue.Value = min;
        DoubleValue averageValue = AverageValueParameter.ActualValue;
        if (averageValue == null) AverageValueParameter.ActualValue = new DoubleValue(sum / values.Length);
        else averageValue.Value = sum / values.Length;
        DoubleValue maxValue = MaxValueParameter.ActualValue;
        if (maxValue == null) MaxValueParameter.ActualValue = new DoubleValue(max);
        else maxValue.Value = max;
      }
      return base.Apply();
    }
  }
}
