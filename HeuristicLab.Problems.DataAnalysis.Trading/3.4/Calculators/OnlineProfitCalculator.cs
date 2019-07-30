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
using System.Linq;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis.Trading {
  public class OnlineProfitCalculator : IOnlineCalculator {

    private int position; // currently held position: -1: short, 0: out of market, 1: long
    private readonly double transactionCost;
    private int count; // only necessary to reset position and total profit on the first data point 
    private double totalProfit;
    public double Profit {
      get { return totalProfit; }
    }

    public OnlineProfitCalculator(double transactionCost) {
      this.transactionCost = transactionCost;
      Reset();
    }

    #region IOnlineCalculator Members
    public OnlineCalculatorError ErrorState {
      get { return OnlineCalculatorError.None; }
    }
    public double Value {
      get { return Profit; }
    }
    public void Reset() {
      position = 0;
      count = 0;
      totalProfit = 0.0;
    }

    public void Add(double actualReturn, double signal) {
      double profit = 0.0;
      if (count == 0) {
        position = (int)signal;
        profit = 0;
        count++;
      } else {
        if (position == 0 && signal.IsAlmost(0)) {
        } else if (position == 0 && signal.IsAlmost(1)) {
          position = 1;
          profit = -transactionCost;
        } else if (position == 0 && signal.IsAlmost(-1)) {
          position = -1;
          profit = -transactionCost;
        } else if (position == 1 && signal.IsAlmost(1)) {
          profit = actualReturn;
        } else if (position == 1 && signal.IsAlmost(0)) {
          profit = actualReturn - transactionCost;
          position = 0;
        } else if (position == 1 && signal.IsAlmost(-1)) {
          profit = actualReturn - transactionCost;
          position = -1;
        } else if (position == -1 && signal.IsAlmost(-1)) {
          profit = -actualReturn;
        } else if (position == -1 && signal.IsAlmost(0)) {
          profit = -actualReturn - transactionCost;
          position = 0;
        } else if (position == -1 && signal.IsAlmost(1)) {
          profit = -actualReturn - transactionCost;
          position = 1;
        }
        count++;
      }
      totalProfit += profit;
    }
    #endregion

    public static double Calculate(IEnumerable<double> returns, IEnumerable<double> signals, double transactionCost, out OnlineCalculatorError errorState) {
      errorState = OnlineCalculatorError.None;
      return GetProfits(returns, signals, transactionCost).Sum();
    }

    public static IEnumerable<double> GetProfits(IEnumerable<double> returns, IEnumerable<double> signals, double transactionCost) {
      var calc = new OnlineProfitCalculator(transactionCost);

      IEnumerator<double> returnsEnumerator = returns.GetEnumerator();
      IEnumerator<double> signalsEnumerator = signals.GetEnumerator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (returnsEnumerator.MoveNext() & signalsEnumerator.MoveNext()) {
        double signal = signalsEnumerator.Current;
        double @return = returnsEnumerator.Current;

        double prevTotalProfit = calc.Profit;
        calc.Add(@return, signal);
        double curTotalProfit = calc.Profit;

        yield return curTotalProfit - prevTotalProfit;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (returnsEnumerator.MoveNext() || signalsEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in first and second enumeration doesn't match.");
      }
    }
  }
}
