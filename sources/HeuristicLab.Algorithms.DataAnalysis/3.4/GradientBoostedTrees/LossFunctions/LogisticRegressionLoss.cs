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
using System.Diagnostics;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  // Greedy Function Approximation: A Gradient Boosting Machine (page 9) 
  [StorableClass]
  [Item("Logistic regression loss", "")]
  public class LogisticRegressionLoss : Item, ILossFunction {
    public LogisticRegressionLoss() { }

    public double GetLoss(IEnumerable<double> target, IEnumerable<double> pred) {
      var targetEnum = target.GetEnumerator();
      var predEnum = pred.GetEnumerator();

      double s = 0;
      while (targetEnum.MoveNext() & predEnum.MoveNext()) {
        Debug.Assert(targetEnum.Current.IsAlmost(0.0) || targetEnum.Current.IsAlmost(1.0), "labels must be 0 or 1 for logistic regression loss");

        var y = targetEnum.Current * 2 - 1; // y in {-1,1}
        s += Math.Log(1 + Math.Exp(-2 * y * predEnum.Current));
      }
      if (targetEnum.MoveNext() | predEnum.MoveNext())
        throw new ArgumentException("target and pred have different lengths");

      return s;
    }

    public IEnumerable<double> GetLossGradient(IEnumerable<double> target, IEnumerable<double> pred) {
      var targetEnum = target.GetEnumerator();
      var predEnum = pred.GetEnumerator();

      while (targetEnum.MoveNext() & predEnum.MoveNext()) {
        Debug.Assert(targetEnum.Current.IsAlmost(0.0) || targetEnum.Current.IsAlmost(1.0), "labels must be 0 or 1 for logistic regression loss");
        var y = targetEnum.Current * 2 - 1; // y in {-1,1}

        yield return 2 * y / (1 + Math.Exp(2 * y * predEnum.Current));

      }
      if (targetEnum.MoveNext() | predEnum.MoveNext())
        throw new ArgumentException("target and pred have different lengths");
    }

    // targetArr and predArr are not changed by LineSearch
    public double LineSearch(double[] targetArr, double[] predArr, int[] idx, int startIdx, int endIdx) {
      if (targetArr.Length != predArr.Length)
        throw new ArgumentException("target and pred have different lengths");

      // "Simple Newton-Raphson step" of eqn. 23 
      double sumY = 0.0;
      double sumDiff = 0.0;
      for (int i = startIdx; i <= endIdx; i++) {
        var row = idx[i];
        var y = targetArr[row] * 2 - 1; // y in {-1,1}
        var pseudoResponse = 2 * y / (1 + Math.Exp(2 * y * predArr[row]));

        sumY += pseudoResponse;
        sumDiff += Math.Abs(pseudoResponse) * (2 - Math.Abs(pseudoResponse));
      }
      // prevent divByZero
      sumDiff = Math.Max(1E-12, sumDiff);
      return sumY / sumDiff;
    }

    #region item implementation
    private LogisticRegressionLoss(LogisticRegressionLoss original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LogisticRegressionLoss(this, cloner);
    }
    #endregion

  }
}
