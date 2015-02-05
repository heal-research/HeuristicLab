#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Analysis.Statistics {
  public static class BonferroniHolm {
    /// <summary>
    /// Based on David Groppe's MATLAB implementation 
    /// (BSD licensed, see 
    /// http://www.mathworks.com/matlabcentral/fileexchange/28303-bonferroni-holm-correction-for-multiple-comparisons)
    /// </summary>
    public static double[] Calculate(double globalAlpha, double[] pValues, out bool[] h) {
      int k = pValues.Length;
      double[] alphaNiveau = new double[k];
      double[] adjustedPValues = new double[k];
      bool[] decision = new bool[k];
      Dictionary<int, double> pValuesIndizes = new Dictionary<int, double>();

      for (int i = 0; i < k; i++) {
        pValuesIndizes.Add(i, pValues[i]);
      }
      var sortedPValues = pValuesIndizes.OrderBy(x => x.Value).ToArray();

      for (int i = 1; i < k + 1; i++) {
        alphaNiveau[i - 1] = globalAlpha / (k - i + 1);
        int idx = sortedPValues[i - 1].Key;

        if (i == 1) {
          //true means reject
          decision[idx] = sortedPValues[i - 1].Value < alphaNiveau[i - 1];
          adjustedPValues[idx] = sortedPValues[i - 1].Value * (k - i + 1);
        } else {
          decision[idx] = decision[sortedPValues[i - 2].Key] && (sortedPValues[i - 1].Value < alphaNiveau[i - 1]);
          adjustedPValues[idx] = Math.Max(adjustedPValues[sortedPValues[i - 2].Key], sortedPValues[i - 1].Value * (k - i + 1));
        }
        if (adjustedPValues[idx] > 1.0) {
          adjustedPValues[idx] = 1.0;
        }
      }

      h = decision;
      return adjustedPValues;
    }
  }
}
