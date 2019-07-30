#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class OnlineWeightedClassificationMeanSquaredErrorCalculator : IOnlineCalculator {

    private double sse;
    private int n;
    public double WeightedResidualsMeanSquaredError {
      get {
        return n > 0 ? sse / n : 0.0;
      }
    }

    public double PositiveClassValue { get; private set; }
    public double ClassValuesMax { get; private set; }
    public double ClassValuesMin { get; private set; }
    public double DefiniteResidualsWeight { get; private set; }
    public double PositiveClassResidualsWeight { get; private set; }
    public double NegativeClassesResidualsWeight { get; private set; }

    public OnlineWeightedClassificationMeanSquaredErrorCalculator(double positiveClassValue, double classValuesMax, double classValuesMin,
                                                                double definiteResidualsWeight, double positiveClassResidualsWeight, double negativeClassesResidualsWeight) {
      PositiveClassValue = positiveClassValue;
      ClassValuesMax = classValuesMax;
      ClassValuesMin = classValuesMin;
      DefiniteResidualsWeight = definiteResidualsWeight;
      PositiveClassResidualsWeight = positiveClassResidualsWeight;
      NegativeClassesResidualsWeight = negativeClassesResidualsWeight;
      Reset();
    }

    #region IOnlineCalculator Members
    private OnlineCalculatorError errorState;
    public OnlineCalculatorError ErrorState {
      get { return errorState; }
    }
    public double Value {
      get { return WeightedResidualsMeanSquaredError; }
    }
    public void Reset() {
      n = 0;
      sse = 0.0;
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) ||
          double.IsNaN(original) || double.IsInfinity(original) || (errorState & OnlineCalculatorError.InvalidValueAdded) > 0) {
        errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
      } else {
        double error = estimated - original;
        double weight;
        //apply weight
        if (estimated > ClassValuesMax || estimated < ClassValuesMin) {
          weight = DefiniteResidualsWeight;
        } else if (original.IsAlmost(PositiveClassValue)) {
          weight = PositiveClassResidualsWeight;
        } else {
          weight = NegativeClassesResidualsWeight;
        }
        sse += error * error * weight;
        n++;
        errorState = errorState & (~OnlineCalculatorError.InsufficientElementsAdded);        // n >= 1
      }
    }
    #endregion

    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, double positiveClassValue, double classValuesMax, double classValuesMin,
                                                                double definiteResidualsWeight, double positiveClassResidualsWeight, double negativeClassesResidualsWeight, out OnlineCalculatorError errorState) {
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlineWeightedClassificationMeanSquaredErrorCalculator calculator = new OnlineWeightedClassificationMeanSquaredErrorCalculator(positiveClassValue, classValuesMax, classValuesMin, definiteResidualsWeight, positiveClassResidualsWeight, negativeClassesResidualsWeight);

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double original = originalEnumerator.Current;
        double estimated = estimatedEnumerator.Current;
        calculator.Add(original, estimated);
        if (calculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (calculator.ErrorState == OnlineCalculatorError.None &&
         (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in originalValues and estimatedValues enumerations doesn't match.");
      } else {
        errorState = calculator.ErrorState;
        return calculator.WeightedResidualsMeanSquaredError;
      }
    }
  }
}
