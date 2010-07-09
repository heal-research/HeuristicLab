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
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public class OnlinePearsonsRSquaredEvaluator : IOnlineEvaluator {

    private double sum_sq_x;
    private double sum_sq_y;
    private double sum_coproduct;
    private double mean_x;
    private double mean_y;
    private int n;

    public double RSquared {
      get {
        if (n < 1)
          throw new InvalidOperationException("No elements");
        else {
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
    }

    public OnlinePearsonsRSquaredEvaluator() { }

    #region IOnlineEvaluator Members
    public double Value {
      get { return RSquared; }
    }
    public void Reset() {
      sum_sq_x = 0.0;
      sum_sq_y = 0.0;
      sum_coproduct = 0.0;
      mean_x = 0.0;
      mean_y = 0.0;
      n = 0;
    }

    public void Add(double original, double estimated) {
      // stable and iterative calculation of R²
      if (IsInvalidValue(original) || IsInvalidValue(estimated)) {
        throw new ArgumentException("R² is not defined for variables with NaN or infinity values.");
      }
      if (n == 0) {
        mean_x = original;
        mean_y = estimated;
        n = 1;
      } else {
        double sweep = (n - 1.0) / n;
        double delta_x = original - mean_x;
        double delta_y = estimated - mean_y;
        sum_sq_x += delta_x * delta_x * sweep;
        sum_sq_y += delta_y * delta_y * sweep;
        sum_coproduct += delta_x * delta_y * sweep;
        mean_x += delta_x / n;
        mean_y += delta_y / n;
        n++;
      }
    }

    #endregion

    private bool IsInvalidValue(double x) {
      return double.IsNaN(x) || double.IsInfinity(x);
    }
  }
}
