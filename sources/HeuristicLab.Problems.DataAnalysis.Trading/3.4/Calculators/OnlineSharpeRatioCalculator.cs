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
using System.Collections.Generic;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis {
  public class OnlineSharpeRatioCalculator : IOnlineCalculator {

    private int p;
    private double transactionCost;
    private OnlineMeanAndVarianceCalculator meanAndVarianceCalculator;
    public double SharpeRatio {
      get {
        if (meanAndVarianceCalculator.Variance > 0)
          return meanAndVarianceCalculator.Mean / Math.Sqrt(meanAndVarianceCalculator.Variance);
        else return 0.0;
      }
    }

    public OnlineSharpeRatioCalculator(double transactionCost) {
      this.transactionCost = transactionCost;
      this.meanAndVarianceCalculator = new OnlineMeanAndVarianceCalculator();
      Reset();
    }

    #region IOnlineCalculator Members
    public OnlineCalculatorError ErrorState {
      get {
        return meanAndVarianceCalculator.MeanErrorState | meanAndVarianceCalculator.VarianceErrorState;
      }
    }
    public double Value {
      get { return SharpeRatio; }
    }
    public void Reset() {
      p = 0;
      meanAndVarianceCalculator.Reset();
    }

    public void Add(double actualReturn, double signal) {
      double iterationReturn = 0.0;
      if (p == 0 && signal.IsAlmost(0)) {
      } else if (p == 0 && signal.IsAlmost(1)) {
        p = 1;
      } else if (p == 0 && signal.IsAlmost(-1)) {
        p = -1;
      } else if (p == 1 && signal.IsAlmost(1)) {
        iterationReturn = actualReturn;
      } else if (p == 1 && signal.IsAlmost(0)) {
        iterationReturn = actualReturn - transactionCost;
        p = 0;
      } else if (p == 1 && signal.IsAlmost(-1)) {
        iterationReturn = actualReturn - transactionCost;
        p = -1;
      } else if (p == -1 && signal.IsAlmost(-1)) {
        iterationReturn = -actualReturn;
      } else if (p == -1 && signal.IsAlmost(0)) {
        iterationReturn = -actualReturn - transactionCost;
        p = 0;
      } else if (p == -1 && signal.IsAlmost(1)) {
        iterationReturn = -actualReturn - transactionCost;
        p = -1;
      }
      meanAndVarianceCalculator.Add(iterationReturn);
    }
    #endregion

    public static double Calculate(IEnumerable<double> returns, IEnumerable<double> signals, double transactionCost, out OnlineCalculatorError errorState) {
      IEnumerator<double> returnsEnumerator = returns.GetEnumerator();
      IEnumerator<double> signalsEnumerator = signals.GetEnumerator();
      OnlineSharpeRatioCalculator calculator = new OnlineSharpeRatioCalculator(transactionCost);

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (returnsEnumerator.MoveNext() & signalsEnumerator.MoveNext()) {
        double signal = signalsEnumerator.Current;
        double @return = returnsEnumerator.Current;
        calculator.Add(@return, signal);
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (returnsEnumerator.MoveNext() || signalsEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in first and second enumeration doesn't match.");
      } else {
        errorState = calculator.ErrorState;
        return calculator.SharpeRatio;
      }
    }
  }
}
