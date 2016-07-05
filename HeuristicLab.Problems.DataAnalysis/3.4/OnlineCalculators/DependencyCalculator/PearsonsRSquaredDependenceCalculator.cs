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

namespace HeuristicLab.Problems.DataAnalysis {
  public class PearsonsRSquaredDependenceCalculator : IDependencyCalculator {

    public double Maximum { get { return 1.0; } }

    public double Minimum { get { return 0.0; } }

    public string Name { get { return "Pearsons R Squared"; } }

    public double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      var r = OnlinePearsonsRCalculator.Calculate(originalValues, estimatedValues, out errorState);
      return r * r;
    }

    public double Calculate(IEnumerable<Tuple<double, double>> values, out OnlineCalculatorError errorState) {
      var calculator = new OnlinePearsonsRCalculator();
      foreach (var tuple in values) {
        calculator.Add(tuple.Item1, tuple.Item2);
        if (calculator.ErrorState != OnlineCalculatorError.None) break;
      }
      errorState = calculator.ErrorState;
      var r = calculator.R;
      return r * r;
    }
  }
}
