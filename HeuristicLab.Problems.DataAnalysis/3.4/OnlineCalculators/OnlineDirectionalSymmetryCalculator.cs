#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class OnlineDirectionalSymmetryCalculator : DeepCloneable, IOnlineTimeSeriesCalculator {
    private int n;
    private int nCorrect;

    public double DirectionalSymmetry {
      get {
        if (n < 1) return 0.0;
        return (double)nCorrect / n;
      }
    }

    public OnlineDirectionalSymmetryCalculator() {
      Reset();
    }

    protected OnlineDirectionalSymmetryCalculator(OnlineDirectionalSymmetryCalculator original, Cloner cloner = null)
      : base(original, cloner) {
      n = original.n;
      nCorrect = original.nCorrect;
      errorState = original.errorState;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OnlineDirectionalSymmetryCalculator(this, cloner);
    }

    public double Value {
      get { return DirectionalSymmetry; }
    }

    private OnlineCalculatorError errorState;
    public OnlineCalculatorError ErrorState {
      get { return errorState; }
    }

    public void Add(double startValue, IEnumerable<double> actualContinuation, IEnumerable<double> predictedContinuation) {
      if (double.IsNaN(startValue) || (errorState & OnlineCalculatorError.InvalidValueAdded) > 0) {
        errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
      } else {
        var actualEnumerator = actualContinuation.GetEnumerator();
        var predictedEnumerator = predictedContinuation.GetEnumerator();
        double prevActual = startValue;
        double prevPredicted = startValue;
        while (actualEnumerator.MoveNext() & predictedEnumerator.MoveNext() & errorState != OnlineCalculatorError.InvalidValueAdded) {
          double actual = actualEnumerator.Current;
          double predicted = predictedEnumerator.Current;
          if (double.IsNaN(actual) || double.IsNaN(predicted)) {
            errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
          } else {
            // count a prediction correct if the trend (positive/negative/no change) is predicted correctly
            if ((actual - prevActual) * (predicted - prevPredicted) > 0.0 ||
                (actual - prevActual).IsAlmost(predicted - prevPredicted)
              ) {
              nCorrect++;
            }
            n++;
          }
        }
        // check if both enumerators are at the end to make sure both enumerations have the same length
        if (actualEnumerator.MoveNext() || predictedEnumerator.MoveNext()) {
          errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
        } else {
          errorState = errorState & (~OnlineCalculatorError.InsufficientElementsAdded); // n >= 1
        }
      }
    }

    public void Reset() {
      n = 0;
      nCorrect = 0;
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
    }

    public static double Calculate(double startValue, IEnumerable<double> actualContinuation, IEnumerable<double> predictedContinuation, out OnlineCalculatorError errorState) {
      OnlineDirectionalSymmetryCalculator dsCalculator = new OnlineDirectionalSymmetryCalculator();
      dsCalculator.Add(startValue, actualContinuation, predictedContinuation);
      errorState = dsCalculator.ErrorState;
      return dsCalculator.DirectionalSymmetry;
    }

    public static double Calculate(IEnumerable<double> startValues, IEnumerable<IEnumerable<double>> actualContinuations, IEnumerable<IEnumerable<double>> predictedContinuations, out OnlineCalculatorError errorState) {
      IEnumerator<double> startValueEnumerator = startValues.GetEnumerator();
      IEnumerator<IEnumerable<double>> actualContinuationsEnumerator = actualContinuations.GetEnumerator();
      IEnumerator<IEnumerable<double>> predictedContinuationsEnumerator = predictedContinuations.GetEnumerator();
      OnlineDirectionalSymmetryCalculator dsCalculator = new OnlineDirectionalSymmetryCalculator();

      // always move forward all enumerators (do not use short-circuit evaluation!)
      while (startValueEnumerator.MoveNext() & actualContinuationsEnumerator.MoveNext() & predictedContinuationsEnumerator.MoveNext()) {
        dsCalculator.Add(startValueEnumerator.Current, actualContinuationsEnumerator.Current, predictedContinuationsEnumerator.Current);
        if (dsCalculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if all enumerators are at the end to make sure both enumerations have the same length
      if (dsCalculator.ErrorState == OnlineCalculatorError.None &&
          (startValueEnumerator.MoveNext() || actualContinuationsEnumerator.MoveNext() || predictedContinuationsEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in startValues, actualContinuations and estimatedValues predictedContinuations doesn't match.");
      } else {
        errorState = dsCalculator.ErrorState;
        return dsCalculator.DirectionalSymmetry;
      }
    }
  }
}
