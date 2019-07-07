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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.ALPS {
  [Item("WeightingReducer", "An operator that combines two values based on the weight of the lower (0.0) and higher (1.0) value.")]
  [StorableType("A2B65E34-61EC-414A-827C-AAA3F1A1E46C")]
  public sealed class WeightingReducer : SingleSuccessorOperator {
    public IScopeTreeLookupParameter<DoubleValue> ParameterToReduce {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["ParameterToReduce"]; }
    }
    public ILookupParameter<DoubleValue> TargetParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["TargetParameter"]; }
    }
    public IValueLookupParameter<DoubleValue> WeightParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Weight"]; }
    }

    [StorableConstructor]
    private WeightingReducer(StorableConstructorFlag _) : base(_) { }
    private WeightingReducer(WeightingReducer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new WeightingReducer(this, cloner);
    }
    public WeightingReducer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("ParameterToReduce", "The parameter on which the weighting should be applied."));
      Parameters.Add(new LookupParameter<DoubleValue>("TargetParameter", "The target variable in which the weighted value should be stored."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Weight", "Weight of the two values (0.0 for the smaller, 1.0 for the larger)."));
    }

    public override IOperation Apply() {
      var values = ParameterToReduce.ActualValue;
      if (values.Length != 2)
        throw new InvalidOperationException("Weighting between values can only done for two values.");

      double weight = WeightParameter.ActualValue.Value;

      double max = values.Max(v => v.Value);
      double min = values.Min(v => v.Value);

      double result = max * weight + min * (1 - weight);

      if (TargetParameter.ActualValue == null)
        TargetParameter.ActualValue = new DoubleValue(result);
      else TargetParameter.ActualValue.Value = result;

      return base.Apply();
    }
  }
}