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
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleMeanAbsolutePercentageOfRangeErrorEvaluator : SimpleEvaluatorBase {
    public override string OutputVariableName {
      get {
        return "MAPRE";
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
      // copy to one-dimensional array for range calculation
      double[] originalValues = new double[values.GetLength(0)];
      for (int i = 0; i < originalValues.Length; i++) originalValues[i] = values[i, ORIGINAL_INDEX];
      double range = Statistics.Range(originalValues);
      if (double.IsInfinity(range)) throw new ArgumentException("Range of elements in values is infinity");
      if (range.IsAlmost(0.0)) throw new ArgumentException("Range of elements in values is zero");

      for (int i = 0; i < values.GetLength(0); i++) {
        double estimated = values[i, ESTIMATION_INDEX];
        double original = values[i, ORIGINAL_INDEX];

        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
          !double.IsNaN(original) && !double.IsInfinity(original) && original != 0.0) {
          double percent_error = Math.Abs((estimated - original) / range);
          errorsSum += percent_error;
          n++;
        }
      }
      if (double.IsInfinity(range) || n == 0) {
        throw new ArgumentException("Mean of absolute percentage of range error is not defined for input vectors of NaN or Inf");
      } else {
        return errorsSum / n;
      }
    }
  }
}
