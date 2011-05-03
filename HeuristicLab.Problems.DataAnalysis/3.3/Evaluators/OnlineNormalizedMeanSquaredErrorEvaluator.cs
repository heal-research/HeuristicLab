#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public class OnlineNormalizedMeanSquaredErrorEvaluator : IOnlineEvaluator {
    private OnlineMeanAndVarianceCalculator meanSquaredErrorCalculator;
    private OnlineMeanAndVarianceCalculator originalVarianceCalculator;

    public double NormalizedMeanSquaredError {
      get {
        return meanSquaredErrorCalculator.Mean / originalVarianceCalculator.Variance;
      }
    }

    public OnlineNormalizedMeanSquaredErrorEvaluator() {
      meanSquaredErrorCalculator = new OnlineMeanAndVarianceCalculator();
      originalVarianceCalculator = new OnlineMeanAndVarianceCalculator();
      Reset();
    }

    #region IOnlineEvaluator Members
    public double Value {
      get { return NormalizedMeanSquaredError; }
    }

    public void Reset() {
      meanSquaredErrorCalculator.Reset();
      originalVarianceCalculator.Reset();
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) ||
          double.IsNaN(original) || double.IsInfinity(original)) {
        throw new ArgumentException("Mean squared error is not defined for NaN or infinity elements");
      } else {
        double error = estimated - original;
        meanSquaredErrorCalculator.Add(error * error);
        originalVarianceCalculator.Add(original);
      }
    }
    #endregion
  }
}
