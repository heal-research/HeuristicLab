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
  public class OnlineNormalizedMeanSquaredErrorCalculator : IOnlineCalculator, IDeepCloneable {
    private OnlineMeanAndVarianceCalculator meanSquaredErrorCalculator;
    private OnlineMeanAndVarianceCalculator originalVarianceCalculator;

    public double NormalizedMeanSquaredError {
      get {
        double var = originalVarianceCalculator.PopulationVariance;
        double m = meanSquaredErrorCalculator.Mean;
        return var > 0 ? m / var : 0.0;
      }
    }

    public OnlineNormalizedMeanSquaredErrorCalculator() {
      meanSquaredErrorCalculator = new OnlineMeanAndVarianceCalculator();
      originalVarianceCalculator = new OnlineMeanAndVarianceCalculator();
      Reset();
    }

    protected OnlineNormalizedMeanSquaredErrorCalculator(OnlineNormalizedMeanSquaredErrorCalculator other, Cloner cloner) {
      meanSquaredErrorCalculator = (OnlineMeanAndVarianceCalculator)other.meanSquaredErrorCalculator.Clone(cloner);
      originalVarianceCalculator = (OnlineMeanAndVarianceCalculator)originalVarianceCalculator.Clone(cloner);
    }

    #region IOnlineCalculator Members
    public OnlineCalculatorError ErrorState {
      get { return meanSquaredErrorCalculator.MeanErrorState | originalVarianceCalculator.PopulationVarianceErrorState; }
    }
    public double Value {
      get { return NormalizedMeanSquaredError; }
    }

    public void Reset() {
      meanSquaredErrorCalculator.Reset();
      originalVarianceCalculator.Reset();
    }

    public void Add(double original, double estimated) {
      // no need to check for validity of values explicitly as it is checked in the meanAndVariance calculator anyway
      double error = estimated - original;
      meanSquaredErrorCalculator.Add(error * error);
      originalVarianceCalculator.Add(original);
    }
    #endregion

    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlineNormalizedMeanSquaredErrorCalculator normalizedMSECalculator = new OnlineNormalizedMeanSquaredErrorCalculator();

      //needed because otherwise the normalizedMSECalculator is in ErrorState.InsufficientValuesAdded
      if (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double original = originalEnumerator.Current;
        double estimated = estimatedEnumerator.Current;
        normalizedMSECalculator.Add(original, estimated);
      }

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double original = originalEnumerator.Current;
        double estimated = estimatedEnumerator.Current;
        normalizedMSECalculator.Add(original, estimated);
        if (normalizedMSECalculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (normalizedMSECalculator.ErrorState == OnlineCalculatorError.None &&
           (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in originalValues and estimatedValues enumeration doesn't match.");
      } else {
        errorState = normalizedMSECalculator.ErrorState;
        return normalizedMSECalculator.NormalizedMeanSquaredError;
      }
    }

    // IDeepCloneable members
    public object Clone() {
      var cloner = new Cloner();
      return new OnlineNormalizedMeanSquaredErrorCalculator(this, cloner);
    }

    public IDeepCloneable Clone(Cloner cloner) {
      var clone = cloner.GetClone(this);
      if (clone == null) {
        clone = new OnlineNormalizedMeanSquaredErrorCalculator(this, cloner);
        cloner.RegisterClonedObject(this, clone);
      }
      return clone;
    }
  }
}
