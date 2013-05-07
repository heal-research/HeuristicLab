#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;

namespace HeuristicLab.Problems.DataAnalysis {
  public class SpearmansRankCorrelationCoefficientCalculator : IDependencyCalculator {

    public double Maximum { get { return 1.0; } }

    public double Minimum { get { return -1.0; } }

    public string Name { get { return "Spearmans Rank"; } }

    public double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      return SpearmansRankCorrelationCoefficientCalculator.CalculateSpearmansRank(originalValues, estimatedValues, out errorState);
    }

    public static double CalculateSpearmansRank(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      double rs = double.NaN;
      try {
        var original = originalValues.ToArray();
        var estimated = estimatedValues.ToArray();
        rs = alglib.basestat.spearmancorr2(original, estimated, original.Length);
        errorState = OnlineCalculatorError.None;
      }
      catch (alglib.alglibexception) {
        errorState = OnlineCalculatorError.InvalidValueAdded;
      }

      return rs;
    }
  }
}
