#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ClassificationPerformanceMeasuresCalculator {

    public ClassificationPerformanceMeasuresCalculator(string positiveClassName, double positiveClassValue) {
      this.positiveClassName = positiveClassName;
      this.positiveClassValue = positiveClassValue;
      Reset();
    }

    #region Properties
    private int truePositiveCount, falsePositiveCount, trueNegativeCount, falseNegativeCount;

    private readonly string positiveClassName;
    public string PositiveClassName {
      get { return positiveClassName; }
    }

    private readonly double positiveClassValue;
    public double PositiveClassValue {
      get { return positiveClassValue; }
    }
    public double TruePositiveRate {
      get {
        double divisor = truePositiveCount + falseNegativeCount;
        return divisor.IsAlmost(0) ? double.NaN : truePositiveCount / divisor;
      }
    }
    public double TrueNegativeRate {
      get {
        double divisor = falsePositiveCount + trueNegativeCount;
        return divisor.IsAlmost(0) ? double.NaN : trueNegativeCount / divisor;
      }
    }
    public double PositivePredictiveValue {
      get {
        double divisor = truePositiveCount + falsePositiveCount;
        return divisor.IsAlmost(0) ? double.NaN : truePositiveCount / divisor;
      }
    }
    public double NegativePredictiveValue {
      get {
        double divisor = trueNegativeCount + falseNegativeCount;
        return divisor.IsAlmost(0) ? double.NaN : trueNegativeCount / divisor;
      }
    }
    public double FalsePositiveRate {
      get {
        double divisor = falsePositiveCount + trueNegativeCount;
        return divisor.IsAlmost(0) ? double.NaN : falsePositiveCount / divisor;
      }
    }
    public double FalseDiscoveryRate {
      get {
        double divisor = falsePositiveCount + truePositiveCount;
        return divisor.IsAlmost(0) ? double.NaN : falsePositiveCount / divisor;
      }
    }

    private OnlineCalculatorError errorState;
    public OnlineCalculatorError ErrorState {
      get { return errorState; }
    }
    #endregion

    public void Reset() {
      truePositiveCount = 0;
      falseNegativeCount = 0;
      trueNegativeCount = 0;
      falseNegativeCount = 0;
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
    }

    public void Add(double originalClassValue, double estimatedClassValue) {
      // ignore cases where original is NaN completely 
      if (double.IsNaN(originalClassValue)) return;

      if (originalClassValue.IsAlmost(positiveClassValue)
            || estimatedClassValue.IsAlmost(positiveClassValue)) { //positive class/positive class estimation
        if (estimatedClassValue.IsAlmost(originalClassValue)) {
          truePositiveCount++;
        } else {
          if (estimatedClassValue.IsAlmost(positiveClassValue)) //misclassification of the negative class
            falsePositiveCount++;
          else //misclassification of the positive class
            falseNegativeCount++;
        }
      } else { //negative class/negative class estimation
        //In a multiclass classification all misclassifications of the negative class
        //will be treated as true negatives except on positive class estimations
        trueNegativeCount++;
      }

      errorState = OnlineCalculatorError.None; // number of (non-NaN) samples >= 1
    }

    public void Calculate(IEnumerable<double> originalClassValues, IEnumerable<double> estimatedClassValues) {
      IEnumerator<double> originalEnumerator = originalClassValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedClassValues.GetEnumerator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double original = originalEnumerator.Current;
        double estimated = estimatedEnumerator.Current;
        Add(original, estimated);
        if (ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (ErrorState == OnlineCalculatorError.None && (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in originalValues and estimatedValues enumerations doesn't match.");
      }
      errorState = ErrorState;
    }
  }
}
