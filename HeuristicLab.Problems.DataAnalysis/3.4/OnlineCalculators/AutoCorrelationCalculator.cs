﻿#region License Information
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

using System.Linq;

namespace HeuristicLab.Problems.DataAnalysis {
  public class AutoCorrelationCalculator {
    public static double[] Calculate(double[] values, out OnlineCalculatorError error) {
      error = OnlineCalculatorError.None;
      if (values.Any(x => double.IsNaN(x) || double.IsInfinity(x))) {
        error = OnlineCalculatorError.InvalidValueAdded;
        return new double[0];
      }

      alglib.corrr1dcircular(values, values.Length, values, values.Length, out var correlations);
      return correlations;
    }
  }
}
