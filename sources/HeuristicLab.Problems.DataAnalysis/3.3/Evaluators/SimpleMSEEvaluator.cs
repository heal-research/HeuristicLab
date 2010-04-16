#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public class SimpleMSEEvaluator : SimpleEvaluator {

    public SimpleMSEEvaluator()
      : base() {
      QualityParameter.ActualName = "MeanSquaredError";
    }

    protected override double Apply(DoubleMatrix values) {
      return Calculate(values);
    }

    public static double Calculate(IEnumerable<double> original, IEnumerable<double> estimated) {
      double sse = 0.0;
      int cnt = 0;
      var originalEnumerator = original.GetEnumerator();
      var estimatedEnumerator = estimated.GetEnumerator();
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double e = estimatedEnumerator.Current;
        double o = originalEnumerator.Current;
        if (!double.IsNaN(e) && !double.IsInfinity(e) &&
            !double.IsNaN(o) && !double.IsInfinity(o)) {
          double error = e - o;
          sse += error * error;
          cnt++;
        }
      }
      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else if (cnt == 0) {
        throw new ArgumentException("Mean squared errors is not defined for input vectors of NaN or Inf");
      } else {
        double mse = sse / cnt;
        return mse;
      }
    }

    public static double Calculate(DoubleMatrix values) {
      var original = from row in Enumerable.Range(0, values.Rows)
                     select values[row, ORIGINAL_INDEX];
      var estimated = from row in Enumerable.Range(0, values.Rows)
                      select values[row, ORIGINAL_INDEX];
      return Calculate(original, estimated);
    }
  }
}
