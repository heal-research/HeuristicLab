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
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public class OnlineMeanAbsolutePercentageErrorEvaluator : IOnlineEvaluator {

    private double sre;
    private int n;
    public double MeanAbsolutePercentageError {
      get {
        if (n < 1)
          throw new InvalidOperationException("No elements");
        else
          return sre / n;
      }
    }

    public OnlineMeanAbsolutePercentageErrorEvaluator() {
      Reset();
    }

    #region IOnlineEvaluator Members
    public double Value {
      get { return MeanAbsolutePercentageError; }
    }
    public void Reset() {
      n = 0;
      sre = 0.0;
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) ||
          double.IsNaN(original) || double.IsInfinity(original)) {
        throw new ArgumentException("Relative error is not defined for variables with NaN or infinity values.");
      } else {
        if (!original.IsAlmost(0.0)) {
          sre += Math.Abs((estimated - original) / original);
          n++;
        }
      }
    }

    #endregion
  }
}
