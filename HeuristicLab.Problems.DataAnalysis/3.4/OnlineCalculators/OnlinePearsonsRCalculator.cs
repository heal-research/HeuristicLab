#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class OnlinePearsonsRCalculator : DeepCloneable, IOnlineCalculator {
    private double sumX;
    private double sumY;
    private double sumWe;

    private double sumXX;
    private double sumYY;
    private double sumXY;

    private OnlineCalculatorError errorState;

    public double R {
      get {
        if (!(sumXX > 0.0 && sumYY > 0.0)) {
          return (sumXX == sumYY) ? 1.0 : 0.0;
        }
        return sumXY / Math.Sqrt(sumXX * sumYY);
      }
    }

    public double MeanX { get { return sumX / sumWe; } }

    public double MeanY { get { return sumY / sumWe; } }

    public double NaiveCovariance { get { return sumXY / sumWe; } }

    public double SampleCovariance {
      get {
        if (sumWe > 1.0) {
          errorState = OnlineCalculatorError.None;
          return sumXY / (sumWe - 1);
        }
        errorState = OnlineCalculatorError.InsufficientElementsAdded;
        return double.NaN;
      }
    }

    public double NaiveVarianceX { get { return sumXX / sumWe; } }

    public double SampleVarianceX {
      get {
        if (sumWe > 1.0) {
          errorState = OnlineCalculatorError.None;
          return sumXX / (sumWe - 1);
        }
        errorState = OnlineCalculatorError.InsufficientElementsAdded;
        return double.NaN;
      }
    }

    public double NaiveStdevX { get { return Math.Sqrt(NaiveVarianceY); } }

    public double SampleStdevX { get { return Math.Sqrt(SampleVarianceX); } }

    public double NaiveVarianceY { get { return sumYY / sumWe; } }

    public double SampleVarianceY {
      get {
        if (sumWe > 1.0) {
          errorState = OnlineCalculatorError.None;
          return sumYY / (sumWe - 1);
        }
        errorState = OnlineCalculatorError.InsufficientElementsAdded;
        return double.NaN;
      }
    }

    public double NaiveStdevY { get { return Math.Sqrt(NaiveVarianceY); } }

    public double SampleStdevY { get { return Math.Sqrt(SampleVarianceX); } }

    public OnlinePearsonsRCalculator() { }

    protected OnlinePearsonsRCalculator(OnlinePearsonsRCalculator original, Cloner cloner)
      : base(original, cloner) {
      sumX = original.sumX;
      sumY = original.sumY;
      sumXX = original.sumXX;
      sumYY = original.sumYY;
      sumXY = original.sumXY;
      sumWe = original.sumWe;
      errorState = original.ErrorState;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OnlinePearsonsRCalculator(this, cloner);
    }

    #region IOnlineCalculator Members
    public OnlineCalculatorError ErrorState {
      get { return errorState; }
    }
    public double Value {
      get { return R; }
    }
    public void Reset() {
      sumXX = sumYY = sumXY = sumX = sumY = sumWe = 0.0;
      errorState = OnlineCalculatorError.None;
    }

    public void Add(double x, double y) {
      if (sumWe <= 0.0) {
        sumX = x;
        sumY = y;
        sumWe = 1;
        return;
      }
      // Delta to previous mean
      double deltaX = x * sumWe - sumX, deltaY = y * sumWe - sumY;
      double oldWe = sumWe;
      // Incremental update
      sumWe += 1;
      double f = 1.0 / (sumWe * oldWe);
      // Update
      sumXX += f * deltaX * deltaX;
      sumYY += f * deltaY * deltaY;
      // should equal weight * deltaY * neltaX!
      sumXY += f * deltaX * deltaY;
      // Update means
      sumX += x;
      sumY += y;
    }

    #endregion

    public static double Calculate(IEnumerable<double> first, IEnumerable<double> second, out OnlineCalculatorError errorState) {
      var x = first.GetEnumerator(); x.MoveNext();
      var y = second.GetEnumerator(); y.MoveNext();
      double sumXX = 0.0, sumYY = 0.0, sumXY = 0.0;
      double sumX = x.Current, sumY = y.Current;
      int i = 1;

      // Inlined computation of Pearson correlation, to avoid allocating objects
      // This is a numerically stabilized version, avoiding sum-of-squares.
      while (x.MoveNext() & y.MoveNext()) {
        double xv = x.Current, yv = y.Current;
        // Delta to previous mean
        double deltaX = xv * i - sumX, deltaY = yv * i - sumY;
        // Increment count first
        double oldi = i; // Convert to double!
        ++i;
        double f = 1.0 / (i * oldi);
        // Update
        sumXX += f * deltaX * deltaX;
        sumYY += f * deltaY * deltaY;
        // should equal deltaY * deltaX!
        sumXY += f * deltaX * deltaY;
        // Update sums
        sumX += xv;
        sumY += yv;
      }

      errorState = OnlineCalculatorError.None;
      // One or both series were constant:
      return !(sumXX > 0.0 && sumYY > 0.0) ? sumXX == sumYY ? 1.0 : 0.0 : //
          sumXY / Math.Sqrt(sumXX * sumYY);
    }
  }
}
