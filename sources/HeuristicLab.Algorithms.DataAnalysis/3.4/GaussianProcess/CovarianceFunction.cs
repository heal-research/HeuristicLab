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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  // base class for covariance functions
  public abstract class CovarianceFunction : ParameterizedNamedItem, ICovarianceFunction {
    [StorableConstructor]
    protected CovarianceFunction(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceFunction(CovarianceFunction original, Cloner cloner)
      : base(original, cloner) {
    }

    protected CovarianceFunction()
      : base() {
      Name = ItemName;
      Description = ItemDescription;
    }

    protected void SetNullableDoubleParameter(IValueParameter<DoubleValue> parameter, double? value) {
      if (value.HasValue) {
        parameter.Value = new DoubleValue(value.Value);
      } else {
        parameter.Value = null;
      }
    }

    protected double? GetNullableDoubleParameter(IValueParameter<DoubleValue> parameter) {
      return parameter.Value == null ? (double?)null : parameter.Value.Value;
    }

    protected void AttachValueChangeHandler<T, U>(IValueParameter<T> parameter, Action action)
      where T : ValueTypeValue<U>
      where U : struct {
      parameter.ValueChanged += (sender, args) => {
        if (parameter.Value != null) {
          parameter.Value.ValueChanged += (s, a) => action();
          action();
        }
      };
      if (parameter.Value != null) {
        parameter.Value.ValueChanged += (s, a) => action();
      }
    }

    protected void AttachArrayChangeHandler<T, U>(IValueParameter<T> parameter, Action action)
      where T : ValueTypeArray<U>
      where U : struct {
      parameter.ValueChanged += (sender, args) => {
        if (parameter.Value != null) {
          parameter.Value.ItemChanged += (s, a) => action();
          parameter.Value.Reset += (s, a) => action();
          action();
        }
      };
      if (parameter.Value != null) {
        parameter.Value.ItemChanged += (s, a) => action();
        parameter.Value.Reset += (s, a) => action();
      }

    }

    public abstract int GetNumberOfParameters(int numberOfVariables);
    public abstract void SetParameter(double[] hyp);
    public abstract double GetCovariance(double[,] x, int i, int j);
    public abstract IEnumerable<double> GetGradient(double[,] x, int i, int j);
    public abstract double GetCrossCovariance(double[,] x, double[,] xt, int i, int j);

  }
}
