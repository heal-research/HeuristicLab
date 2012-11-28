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

using System.Collections.Generic;

namespace HeuristicLab.Problems.DataAnalysis {
  public class OnlineMeanAndVarianceCalculator {

    private double m_oldM, m_newM, m_oldS, m_newS;
    private int n;

    private OnlineCalculatorError varianceErrorState;
    public OnlineCalculatorError VarianceErrorState {
      get { return varianceErrorState; }
    }

    public double Variance {
      get {
        return (n > 1) ? m_newS / (n - 1) : 0.0;
      }
    }

    private OnlineCalculatorError errorState;
    public OnlineCalculatorError PopulationVarianceErrorState {
      get { return errorState; }
    }
    public double PopulationVariance {
      get {
        return (n > 0) ? m_newS / n : 0.0;
      }
    }

    public OnlineCalculatorError MeanErrorState {
      get { return errorState; }
    }
    public double Mean {
      get {
        return (n > 0) ? m_newM : 0.0;
      }
    }

    public int Count {
      get { return n; }
    }

    public OnlineMeanAndVarianceCalculator() {
      Reset();
    }

    public void Reset() {
      n = 0;
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
      varianceErrorState = OnlineCalculatorError.InsufficientElementsAdded;
    }

    public void Add(double x) {
      if (double.IsNaN(x) || double.IsInfinity(x) || (errorState & OnlineCalculatorError.InvalidValueAdded) > 0) {
        errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
        varianceErrorState = errorState | OnlineCalculatorError.InvalidValueAdded;
      } else {
        n++;
        // See Knuth TAOCP vol 2, 3rd edition, page 232
        if (n == 1) {
          m_oldM = m_newM = x;
          m_oldS = 0.0;
          errorState = errorState & (~OnlineCalculatorError.InsufficientElementsAdded);        // n >= 1
        } else {

          varianceErrorState = varianceErrorState & (~OnlineCalculatorError.InsufficientElementsAdded);        // n >= 2
          m_newM = m_oldM + (x - m_oldM) / n;
          m_newS = m_oldS + (x - m_oldM) * (x - m_newM);

          // set up for next iteration
          m_oldM = m_newM;
          m_oldS = m_newS;
        }
      }
    }

    public static void Calculate(IEnumerable<double> x, out double mean, out double variance, out OnlineCalculatorError meanErrorState, out OnlineCalculatorError varianceErrorState) {
      OnlineMeanAndVarianceCalculator meanAndVarianceCalculator = new OnlineMeanAndVarianceCalculator();
      foreach (double xi in x) {
        meanAndVarianceCalculator.Add(xi);
      }
      mean = meanAndVarianceCalculator.Mean;
      variance = meanAndVarianceCalculator.Variance;
      meanErrorState = meanAndVarianceCalculator.MeanErrorState;
      varianceErrorState = meanAndVarianceCalculator.VarianceErrorState;
    }
  }
}
