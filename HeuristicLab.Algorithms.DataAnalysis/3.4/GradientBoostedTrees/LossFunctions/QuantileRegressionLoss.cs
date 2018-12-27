#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  // loss function for quantile regression
  // Generalized Boosted Models - A Guide To The gbm Package, Greg Ridgeway, August 2007, page 11 
  [StorableClass]
  [Item("QuantileRegressionloss", "Loss function for quantile regression")]
  public sealed class QuantileRegressionLoss : ParameterizedNamedItem, ILossFunction {
    public IFixedValueParameter<PercentValue> AlphaParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters["Alpha"]; }
    }

    public double Alpha {
      get { return AlphaParameter.Value.Value; }
      set {
        if (value <= 0.0 || value >= 1.0) throw new ArgumentException("Valid values for alpha: 0 < alpha < 1");
        AlphaParameter.Value.Value = value;
      }
    }

    public QuantileRegressionLoss()
      : base("QuantileRegressionLoss", "Loss function for quantile regression") {
      Parameters.Add(new FixedValueParameter<PercentValue>("Alpha", new PercentValue(0.9)));
    }

    public double GetLoss(IEnumerable<double> target, IEnumerable<double> pred) {
      var targetEnum = target.GetEnumerator();
      var predEnum = pred.GetEnumerator();
      var alpha = Alpha;
      double leftSum = 0;
      double rightsum = 0;
      while (targetEnum.MoveNext() & predEnum.MoveNext()) {
        double res = targetEnum.Current - predEnum.Current;
        if (res > 0) leftSum += res;
        else rightsum += -res;
      }
      if (targetEnum.MoveNext() | predEnum.MoveNext())
        throw new ArgumentException("target and pred have differing lengths");

      return alpha * leftSum + (1 - alpha) * rightsum;
    }

    public IEnumerable<double> GetLossGradient(IEnumerable<double> target, IEnumerable<double> pred) {
      var targetEnum = target.GetEnumerator();
      var predEnum = pred.GetEnumerator();
      var alpha = AlphaParameter.Value.Value;

      while (targetEnum.MoveNext() & predEnum.MoveNext()) {
        var res = targetEnum.Current - predEnum.Current;
        if (res > 0) yield return alpha;
        else if (res < 0) yield return -(1.0 - alpha);
        else yield return 0.0;
      }
      if (targetEnum.MoveNext() | predEnum.MoveNext())
        throw new ArgumentException("target and pred have differing lengths");
    }

    // targetArr and predArr are not changed by LineSearch
    public double LineSearch(double[] targetArr, double[] predArr, int[] idx, int startIdx, int endIdx) {
      if (targetArr.Length != predArr.Length)
        throw new ArgumentException("target and pred have differing lengths");

      // Quantile() is allocating an array anyway
      // It would be possible to pre-allocated an array for the residuals if Quantile() would allow specification of a sub-range
      int nRows = endIdx - startIdx + 1;
      var res = new double[nRows];
      for (int i = startIdx; i <= endIdx; i++) {
        var row = idx[i];
        res[i - startIdx] = targetArr[row] - predArr[row];
      }
      return res.Quantile(Alpha);
    }

    #region item implementation
    [StorableConstructor]
    private QuantileRegressionLoss(bool deserializing) : base(deserializing) { }

    private QuantileRegressionLoss(QuantileRegressionLoss original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QuantileRegressionLoss(this, cloner);
    }
    #endregion

    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }
  }
}
