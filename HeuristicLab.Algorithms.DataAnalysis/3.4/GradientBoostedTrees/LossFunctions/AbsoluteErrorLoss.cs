#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  // loss function for the weighted absolute error
  [StorableClass]
  [Item("Absolute error loss", "")]
  public sealed class AbsoluteErrorLoss : Item, ILossFunction {
    public AbsoluteErrorLoss() { }

    public double GetLoss(IEnumerable<double> target, IEnumerable<double> pred) {
      var targetEnum = target.GetEnumerator();
      var predEnum = pred.GetEnumerator();

      double s = 0;
      while (targetEnum.MoveNext() & predEnum.MoveNext()) {
        double res = targetEnum.Current - predEnum.Current;
        s += Math.Abs(res);  // |res|
      }
      if (targetEnum.MoveNext() | predEnum.MoveNext())
        throw new ArgumentException("target and pred have differing lengths");

      return s;
    }

    public IEnumerable<double> GetLossGradient(IEnumerable<double> target, IEnumerable<double> pred) {
      var targetEnum = target.GetEnumerator();
      var predEnum = pred.GetEnumerator();

      while (targetEnum.MoveNext() & predEnum.MoveNext()) {
        // dL(y, f(x)) / df(x) = sign(res)
        var res = targetEnum.Current - predEnum.Current;
        if (res > 0) yield return 1.0;
        else if (res < 0) yield return -1.0;
        else yield return 0.0;
      }
      if (targetEnum.MoveNext() | predEnum.MoveNext())
        throw new ArgumentException("target and pred have differing lengths");
    }

    // return median of residuals
    // targetArr and predArr are not changed by LineSearch
    public double LineSearch(double[] targetArr, double[] predArr, int[] idx, int startIdx, int endIdx) {
      if (targetArr.Length != predArr.Length)
        throw new ArgumentException("target and pred have differing lengths");

      // Median() is allocating an array anyway
      // It would be possible to pre-allocated an array for the residuals if Median() would allow specification of a sub-range
      int nRows = endIdx - startIdx + 1;
      var res = new double[nRows];
      for (int i = startIdx; i <= endIdx; i++) {
        var row = idx[i];
        res[i - startIdx] = targetArr[row] - predArr[row];
      }
      return res.Median(); // TODO: improve efficiency
    }

    #region item implementation
    [StorableConstructor]
    private AbsoluteErrorLoss(bool deserializing) : base(deserializing) { }

    private AbsoluteErrorLoss(AbsoluteErrorLoss original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AbsoluteErrorLoss(this, cloner);
    }
    #endregion
  }
}
