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
using HEAL.Fossil;

namespace HeuristicLab.Algorithms.DataAnalysis {
  // relative error loss is a special case of weighted absolute error loss with weights = (1/target)
  [StorableType("1334143A-7F77-46C1-8080-0CC4F4DA8995")]
  [Item("Relative error loss", "")]
  public sealed class RelativeErrorLoss : Item, ILossFunction {
    public RelativeErrorLoss() { }

    public double GetLoss(IEnumerable<double> target, IEnumerable<double> pred) {
      var targetEnum = target.GetEnumerator();
      var predEnum = pred.GetEnumerator();

      double s = 0;
      while (targetEnum.MoveNext() & predEnum.MoveNext()) {
        double res = targetEnum.Current - predEnum.Current;
        s += Math.Abs(res) * Math.Abs(1.0 / targetEnum.Current);
      }
      if (targetEnum.MoveNext() | predEnum.MoveNext())
        throw new ArgumentException("target and pred have different lengths");

      return s;
    }

    public IEnumerable<double> GetLossGradient(IEnumerable<double> target, IEnumerable<double> pred) {
      var targetEnum = target.GetEnumerator();
      var predEnum = pred.GetEnumerator();

      while (targetEnum.MoveNext() & predEnum.MoveNext()) {
        // sign(res) * abs(1 / target)
        var res = targetEnum.Current - predEnum.Current;
        if (res > 0) yield return 1.0 / Math.Abs(targetEnum.Current);
        else if (res < 0) yield return -1.0 / Math.Abs(targetEnum.Current);
        else yield return 0.0;
      }
      if (targetEnum.MoveNext() | predEnum.MoveNext())
        throw new ArgumentException("target and pred have different lengths");
    }

    // targetArr and predArr are not changed by LineSearch
    public double LineSearch(double[] targetArr, double[] predArr, int[] idx, int startIdx, int endIdx) {
      if (targetArr.Length != predArr.Length)
        throw new ArgumentException("target and pred have different lengths");

      // line search for relative error
      // weighted median (weight = 1/target)
      int nRows = endIdx - startIdx + 1; // startIdx and endIdx are inclusive
      if (nRows == 1) return targetArr[idx[startIdx]] - predArr[idx[startIdx]]; // res 
      else if (nRows == 2) {
        // weighted average of two residuals
        var w0 = Math.Abs(1.0 / targetArr[idx[startIdx]]);
        var w1 = Math.Abs(1.0 / targetArr[idx[endIdx]]);
        if (w0 > w1) {
          return targetArr[idx[startIdx]] - predArr[idx[startIdx]];
        } else if (w0 < w1) {
          return targetArr[idx[endIdx]] - predArr[idx[endIdx]];
        } else {
          // same weight -> return average of both residuals
          return ((targetArr[idx[startIdx]] - predArr[idx[startIdx]]) + (targetArr[idx[endIdx]] - predArr[idx[endIdx]])) / 2;
        }
      } else {
        // create an array of key-value pairs to be sorted (instead of using Array.Sort(res, weights))
        var res_w = new KeyValuePair<double, double>[nRows];
        var totalWeight = 0.0;
        for (int i = startIdx; i <= endIdx; i++) {
          int row = idx[i];
          var res = targetArr[row] - predArr[row];
          var w = Math.Abs(1.0 / targetArr[row]);
          res_w[i - startIdx] = new KeyValuePair<double, double>(res, w);
          totalWeight += w;
        }
        // TODO: improve efficiency (find median without sort)
        res_w.StableSort((a, b) => Math.Sign(a.Key - b.Key));

        int k = 0;
        double sum = totalWeight - res_w[k].Value; // total - first weight
        while (sum > totalWeight / 2) {
          k++;
          sum -= res_w[k].Value;
        }
        return res_w[k].Key;
      }
    }

    #region item implementation
    [StorableConstructor]
    private RelativeErrorLoss(StorableConstructorFlag _) : base(_) { }

    private RelativeErrorLoss(RelativeErrorLoss original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RelativeErrorLoss(this, cloner);
    }
    #endregion
  }
}
