#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class OnlineWeightedDirectionalSymmetryCalculator : IOnlineTimeSeriesCalculator, IDeepCloneable {
    private int n;
    private double correctSum;
    private double incorrectSum;

    public double WeightedDirectionalSymmetry {
      get {
        if (n <= 1) return 0.0;
        return incorrectSum / correctSum;
      }
    }

    public OnlineWeightedDirectionalSymmetryCalculator() {
      Reset();
    }

    // private constructor used internally by the Clone() method
    private OnlineWeightedDirectionalSymmetryCalculator(OnlineWeightedDirectionalSymmetryCalculator other) {
      n = other.n;
      correctSum = other.correctSum;
      incorrectSum = other.incorrectSum;
      errorState = other.errorState;
    }

    public double Value {
      get { return WeightedDirectionalSymmetry; }
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
        while (actualEnumerator.MoveNext() & predictedEnumerator.MoveNext() & errorState != OnlineCalculatorError.InvalidValueAdded) {
          double actual = actualEnumerator.Current;
          double predicted = predictedEnumerator.Current;
          if (double.IsNaN(actual) || double.IsNaN(predicted)) {
            errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
          } else {
            double err = Math.Abs(actual - predicted);
            // count as correct only if the trend (positive/negative/no change) is predicted correctly
            if ((actual - startValue) * (predicted - startValue) > 0.0 ||
              (actual - startValue).IsAlmost(predicted - startValue)) {
              correctSum += err;
            } else {
              incorrectSum += err;
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
      correctSum = 0;
      incorrectSum = 0;
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
    }

    public static double Calculate(double startValue, IEnumerable<double> actualContinuation, IEnumerable<double> predictedContinuation, out OnlineCalculatorError errorState) {
      OnlineWeightedDirectionalSymmetryCalculator calculator = new OnlineWeightedDirectionalSymmetryCalculator();
      calculator.Add(startValue, actualContinuation, predictedContinuation);
      errorState = calculator.ErrorState;
      return calculator.WeightedDirectionalSymmetry;
    }

    public static double Calculate(IEnumerable<double> startValues, IEnumerable<IEnumerable<double>> actualContinuations, IEnumerable<IEnumerable<double>> predictedContinuations, out OnlineCalculatorError errorState) {
      IEnumerator<double> startValueEnumerator = startValues.GetEnumerator();
      IEnumerator<IEnumerable<double>> actualContinuationsEnumerator = actualContinuations.GetEnumerator();
      IEnumerator<IEnumerable<double>> predictedContinuationsEnumerator = predictedContinuations.GetEnumerator();
      OnlineWeightedDirectionalSymmetryCalculator calculator = new OnlineWeightedDirectionalSymmetryCalculator();

      // always move forward all enumerators (do not use short-circuit evaluation!)
      while (startValueEnumerator.MoveNext() & actualContinuationsEnumerator.MoveNext() & predictedContinuationsEnumerator.MoveNext()) {
        calculator.Add(startValueEnumerator.Current, actualContinuationsEnumerator.Current, predictedContinuationsEnumerator.Current);
        if (calculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if all enumerators are at the end to make sure both enumerations have the same length
      if (calculator.ErrorState == OnlineCalculatorError.None &&
          (startValueEnumerator.MoveNext() || actualContinuationsEnumerator.MoveNext() || predictedContinuationsEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in startValues, actualContinuations and estimatedValues predictedContinuations doesn't match.");
      } else {
        errorState = calculator.ErrorState;
        return calculator.WeightedDirectionalSymmetry;
      }
    }

    // IDeepCloneable members
    public object Clone() {
      return new OnlineWeightedDirectionalSymmetryCalculator(this);
    }

    public IDeepCloneable Clone(Cloner cloner) {
      var clone = cloner.GetClone(this);
      if (clone == null) {
        clone = (IDeepCloneable)this.Clone();
        cloner.RegisterClonedObject(this, clone);
      }
      return clone;
    }
  }
}
