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

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public class SimpleMSEEvaluator : SimpleEvaluator {

    public SimpleMSEEvaluator()
      : base() {
      QualityParameter.ActualName = "MeanSquaredError";
    }

    protected override double Apply(DoubleMatrix values) {
      return Calculate(values);
    }

    public static double Calculate(DoubleMatrix values) {
      double sse = 0;
      double cnt = 0;
      for (int i = 0; i < values.Rows; i++) {
        double estimated = values[i, ESTIMATION_INDEX];
        double target = values[i, ORIGINAL_INDEX];
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
            !double.IsNaN(target) && !double.IsInfinity(target)) {
          double error = estimated - target;
          sse += error * error;
          cnt++;
        }
      }
      if (cnt > 0) {
        double mse = sse / cnt;
        return mse;
      } else {
        throw new ArgumentException("Mean squared errors is not defined for input vectors of NaN or Inf");
      }
    }
  }
}
