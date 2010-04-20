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
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public class SimpleRSquaredEvaluator : SimpleEvaluator {
    public ILookupParameter<DoubleValue> RSquaredParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["RSquared"]; }
    }

    public SimpleRSquaredEvaluator() {
      Parameters.Add(new LookupParameter<DoubleValue>("RSquared", "The squared Pearson's Product Moment Correlation (R²) of estimated values and original values."));
    }

    protected override void Apply(DoubleMatrix values) {
      var original = from i in Enumerable.Range(0, values.Rows)
                     select values[i, ORIGINAL_INDEX];
      var estimated = from i in Enumerable.Range(0, values.Rows)
                      select values[i, ESTIMATION_INDEX];
      RSquaredParameter.ActualValue = new DoubleValue(Calculate(original, estimated));
    }


    public static double Calculate(IEnumerable<double> original, IEnumerable<double> estimated) {
      var originalEnumerator = original.GetEnumerator();
      var estimatedEnumerator = estimated.GetEnumerator();
      originalEnumerator.MoveNext();
      estimatedEnumerator.MoveNext();
      double e = estimatedEnumerator.Current;
      double o = originalEnumerator.Current;

      // stable and iterative calculation of R² in one pass over original and estimated
      double sum_sq_x = 0.0;
      double sum_sq_y = 0.0;
      double sum_coproduct = 0.0;
      if (IsInvalidValue(o) || IsInvalidValue(e)) {
        throw new ArgumentException("R² is not defined for variables with NaN or infinity values.");
      }
      double mean_x = o;
      double mean_y = e;
      int n = 1;
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        e = estimatedEnumerator.Current;
        o = originalEnumerator.Current;
        double sweep = (n - 1.0) / n;
        if (IsInvalidValue(o) || IsInvalidValue(e)) {
          throw new ArgumentException("Correlation coefficient is not defined for variables with NaN or infinity values.");
        }
        double delta_x = o - mean_x;
        double delta_y = e - mean_y;
        sum_sq_x += delta_x * delta_x * sweep;
        sum_sq_y += delta_y * delta_y * sweep;
        sum_coproduct += delta_x * delta_y * sweep;
        mean_x += delta_x / n;
        mean_y += delta_y / n;
        n++;
      }
      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else {
        double pop_sd_x = Math.Sqrt(sum_sq_x / n);
        double pop_sd_y = Math.Sqrt(sum_sq_y / n);
        double cov_x_y = sum_coproduct / n;

        if (pop_sd_x.IsAlmost(0.0) || pop_sd_y.IsAlmost(0.0))
          return 0.0;
        else {
          double r = cov_x_y / (pop_sd_x * pop_sd_y);
          return r * r;
        }
      }
    }

    private static bool IsInvalidValue(double d) {
      return double.IsNaN(d) || double.IsInfinity(d);
    }
  }
}
