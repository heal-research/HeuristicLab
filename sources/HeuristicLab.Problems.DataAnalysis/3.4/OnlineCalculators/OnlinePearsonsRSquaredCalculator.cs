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
  [Obsolete("Use OnlinePearsonsRCalculator directly")]
  public class OnlinePearsonsRSquaredCalculator : DeepCloneable, IOnlineCalculator {
    private readonly OnlinePearsonsRCalculator rCalculator = new OnlinePearsonsRCalculator();

    public double RSquared {
      get {
        if (rCalculator.ErrorState != OnlineCalculatorError.None) return 0.0;
        else return rCalculator.R * rCalculator.R;
      }
    }

    public OnlinePearsonsRSquaredCalculator() { }

    protected OnlinePearsonsRSquaredCalculator(OnlinePearsonsRSquaredCalculator other, Cloner cloner) {
      this.rCalculator = (OnlinePearsonsRCalculator)other.rCalculator.Clone(cloner);
    }

    #region IOnlineCalculator Members
    public OnlineCalculatorError ErrorState {
      get { return rCalculator.ErrorState; }
    }
    public double Value {
      get { return RSquared; }
    }
    public void Reset() {
      rCalculator.Reset();
    }

    public void Add(double x, double y) {
      rCalculator.Add(x, y);
    }

    #endregion

    public static double Calculate(IEnumerable<double> first, IEnumerable<double> second, out OnlineCalculatorError errorState) {
      var r = OnlinePearsonsRCalculator.Calculate(first, second, out errorState);
      return r * r;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OnlinePearsonsRSquaredCalculator(this, cloner);
    }
  }
}
