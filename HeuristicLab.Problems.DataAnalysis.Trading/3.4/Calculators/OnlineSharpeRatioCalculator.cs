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

namespace HeuristicLab.Problems.DataAnalysis.Trading {
  public class OnlineSharpeRatioCalculator : IOnlineCalculator {

    private OnlineMeanAndVarianceCalculator meanAndVarianceCalculator;
    private OnlineProfitCalculator profitCalculator;

    public double SharpeRatio {
      get {
        if (meanAndVarianceCalculator.PopulationVariance > 0)
          return meanAndVarianceCalculator.Mean / Math.Sqrt(meanAndVarianceCalculator.PopulationVariance);
        else return 0.0;
      }
    }

    public OnlineSharpeRatioCalculator(double transactionCost) {
      this.meanAndVarianceCalculator = new OnlineMeanAndVarianceCalculator();
      this.profitCalculator = new OnlineProfitCalculator(transactionCost);
      Reset();
    }

    #region IOnlineCalculator Members
    public OnlineCalculatorError ErrorState {
      get {
        return meanAndVarianceCalculator.MeanErrorState | meanAndVarianceCalculator.PopulationVarianceErrorState | profitCalculator.ErrorState;
      }
    }
    public double Value {
      get { return SharpeRatio; }
    }
    public void Reset() {
      profitCalculator.Reset();
      meanAndVarianceCalculator.Reset();
    }

    public void Add(double actualReturn, double signal) {
      double prevTotalProfit = profitCalculator.Profit;
      profitCalculator.Add(actualReturn, signal);
      double curTotalProfit = profitCalculator.Profit;

      meanAndVarianceCalculator.Add(curTotalProfit - prevTotalProfit);
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
