#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item("Squared error loss", "")]
  public sealed class SquaredErrorLoss : Item, ILossFunction {
    public SquaredErrorLoss() { }

    public double GetLoss(IEnumerable<double> target, IEnumerable<double> pred) {
      var targetEnum = target.GetEnumerator();
      var predEnum = pred.GetEnumerator();

      double s = 0;
      while (targetEnum.MoveNext() & predEnum.MoveNext()) {
        double res = targetEnum.Current - predEnum.Current;
        s += res * res; // (res)^2
      }
      if (targetEnum.MoveNext() | predEnum.MoveNext())
        throw new ArgumentException("target and pred have different lengths");

      return s;
    }

    public IEnumerable<double> GetLossGradient(IEnumerable<double> target, IEnumerable<double> pred) {
      var targetEnum = target.GetEnumerator();
      var predEnum = pred.GetEnumerator();

      while (targetEnum.MoveNext() & predEnum.MoveNext()) {
        yield return 2.0 * (targetEnum.Current - predEnum.Current); // dL(y, f(x)) / df(x)  = 2 * res 
      }
      if (targetEnum.MoveNext() | predEnum.MoveNext())
        throw new ArgumentException("target and pred have different lengths");
    }

    // targetArr and predArr are not changed by LineSearch
    public double LineSearch(double[] targetArr, double[] predArr, int[] idx, int startIdx, int endIdx) {
      if (targetArr.Length != predArr.Length)
        throw new ArgumentException("target and pred have different lengths");

      // line search for squared error loss 
      // for a given partition of rows the optimal constant that should be added to the current prediction values is the average of the residuals
      double s = 0.0;
      int n = 0;
      for (int i = startIdx; i <= endIdx; i++) {
        int row = idx[i];
        s += (targetArr[row] - predArr[row]);
        n++;
      }
      return s / n;
    }

    #region item implementation
    [StorableConstructor]
    private SquaredErrorLoss(bool deserializing) : base(deserializing) { }

    private SquaredErrorLoss(SquaredErrorLoss original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SquaredErrorLoss(this, cloner);
    }
    #endregion
  }
}
