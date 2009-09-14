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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleTheilInequalityCoefficientEvaluator : SimpleEvaluatorBase {
    public override string Description {
      get {
        return @"Calculates the Theil inequality coefficient (Theil's U2 not U1!) of estimated values vs. real values of 'TargetVariable'.

U2 = Sqrt(1/N * Sum(P_t - A_t)^2 ) / Sqrt(1/N * Sum(A_t)^2 ) 

where P_t is the predicted change of the target variable and A_t is the measured (original) change.
(P_t = y'_t - y_(t-1), A_t = y_t - y_(t-1)).

U2 is 0 for a perfect prediction and 1 for the naive model y'_t = y_(t-1). An U2 > 1 means the
model is worse than the naive model (=> model is useless).";
      }
    }

    public override string OutputVariableName {
      get {
        return "TheilInequalityCoefficient";
      }
    }

    public override double Evaluate(double[,] values) {
      return Calculate(values);
    }

    public static double Calculate(double[,] values) {
      int n = values.GetLength(0);
      double errorsSquaredSum = 0.0;
      double originalSquaredSum = 0.0;
      int nSamples = 0;
      for (int sample = 1; sample < n; sample++) {
        double prevValue = values[sample - 1, 1];
        double estimatedChange = values[sample, 0] - prevValue;
        double originalChange = values[sample, 1] - prevValue;
        if (!double.IsNaN(originalChange) && !double.IsInfinity(originalChange)) {
          double error = estimatedChange - originalChange;
          errorsSquaredSum += error * error;
          originalSquaredSum += originalChange * originalChange;
          nSamples++;
        }
      }
      double quality = Math.Sqrt(errorsSquaredSum / nSamples) / Math.Sqrt(originalSquaredSum / nSamples);
      if (double.IsNaN(quality) || double.IsInfinity(quality))
        quality = double.MaxValue;
      return quality;
    }
  }
}
