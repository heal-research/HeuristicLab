#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis {
  public class OnlineAccuracyCalculator : IOnlineCalculator {

    private int correctlyClassified;
    private int n;
    public double Accuracy {
      get {
        return correctlyClassified / (double)n;
      }
    }

    public OnlineAccuracyCalculator() {
      Reset();
    }

    #region IOnlineCalculator Members
    private OnlineCalculatorError errorState;
    public OnlineCalculatorError ErrorState {
      get { return errorState; }
    }
    public double Value {
      get { return Accuracy; }
    }
    public void Reset() {
      n = 0;
      correctlyClassified = 0;
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
    }

    public void Add(double original, double estimated) {
      // ignore cases where original is NaN completly 
      if (!double.IsNaN(original)) {
        // increment number of observed samples
        n++;
        if (original.IsAlmost(estimated)) {
          // original = estimated = +Inf counts as correctly classified
          // original = estimated = -Inf counts as correctly classified
          correctlyClassified++;
        }
        errorState = OnlineCalculatorError.None; // number of (non-NaN) samples >= 1
      }
    }
    #endregion

    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlineAccuracyCalculator accuracyCalculator = new OnlineAccuracyCalculator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double original = originalEnumerator.Current;
        double estimated = estimatedEnumerator.Current;
        accuracyCalculator.Add(original, estimated);
        if (accuracyCalculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (accuracyCalculator.ErrorState == OnlineCalculatorError.None &&
          (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in originalValues and estimatedValues enumerations doesn't match.");
      } else {
        errorState = accuracyCalculator.ErrorState;
        return accuracyCalculator.Accuracy;
      }
    }
  }
}
