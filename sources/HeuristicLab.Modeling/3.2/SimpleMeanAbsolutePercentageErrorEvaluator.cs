#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleMeanAbsolutePercentageErrorEvaluator : SimpleEvaluatorBase {

    public override string OutputVariableName {
      get {
        return "MAPE";
      }
    }

    public override double Evaluate(double[,] values) {
      try {
        return Calculate(values);
      }
      catch (ArgumentException) {
        return double.PositiveInfinity;
      }
    }

    public static double Calculate(double[,] values) {
      double errorsSum = 0.0;
      int n = 0;
      for (int i = 0; i < values.GetLength(0); i++) {
        double estimated = values[i, ESTIMATION_INDEX];
        double original = values[i, ORIGINAL_INDEX];

        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
          !double.IsNaN(original) && !double.IsInfinity(original) && original != 0.0) {
          double percent_error = Math.Abs((estimated - original) / original);
          errorsSum += percent_error;
          n++;
        }
      }
      if (n > 0) {
        return errorsSum / n;
      } else throw new ArgumentException("Mean of absolute percentage error is not defined for input vectors of NaN or Inf");
    }
  }
}
