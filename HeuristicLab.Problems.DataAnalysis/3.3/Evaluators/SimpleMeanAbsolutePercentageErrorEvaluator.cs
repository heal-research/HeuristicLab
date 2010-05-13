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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public class SimpleMeanAbsolutePercentageErrorEvaluator : SimpleEvaluator {
    public ILookupParameter<PercentValue> AverageRelativeErrorParameter {
      get { return (ILookupParameter<PercentValue>)Parameters["AverageRelativeError"]; }
    }

    public SimpleMeanAbsolutePercentageErrorEvaluator() {
      Parameters.Add(new LookupParameter<PercentValue>("AverageRelativeError", "The average relative error of estimated values."));
    }

    protected override void Apply(DoubleMatrix values) {
      var original = from i in Enumerable.Range(0, values.Rows)
                     select values[i, ORIGINAL_INDEX];
      var estimated = from i in Enumerable.Range(0, values.Rows)
                      select values[i, ESTIMATION_INDEX];
      AverageRelativeErrorParameter.ActualValue = new PercentValue(Calculate(original, estimated));
    }

    public static double Calculate(IEnumerable<double> original, IEnumerable<double> estimated) {
      double sre = 0;
      int cnt = 0;
      var originalEnumerator = original.GetEnumerator();
      var estimatedEnumerator = estimated.GetEnumerator();
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double e = estimatedEnumerator.Current;
        double o = originalEnumerator.Current;
        if (!double.IsNaN(e) && !double.IsInfinity(e) &&
            !double.IsNaN(o) && !double.IsInfinity(o) && !o.IsAlmost(0.0)) {
          double error = Math.Abs((e - o) / o);
          sre += error * error;
          cnt++;
        }
      }
      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else if (cnt == 0) {
        throw new ArgumentException("Average relative error is not defined for input vectors of NaN or Inf");
      } else {
        return sre / cnt;
      }
    }
  }
}
