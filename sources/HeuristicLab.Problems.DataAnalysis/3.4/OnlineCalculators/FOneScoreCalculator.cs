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
using System.Linq;

namespace HeuristicLab.Problems.DataAnalysis {
  public class FOneScoreCalculator {
    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      if (originalValues.Distinct().Skip(2).Any()) {
        throw new ArgumentException("F1 score can only be calculated for binary classification.");
      }

      var confusionMatrix = ConfusionMatrixCalculator.Calculate(originalValues, estimatedValues, out errorState);
      if (!errorState.Equals(OnlineCalculatorError.None)) {
        return double.NaN;
      }
      return CalculateFOne(confusionMatrix);
    }

    private static double CalculateFOne(double[,] confusionMatrix) {
      double precision = confusionMatrix[0, 0] / (confusionMatrix[0, 0] + confusionMatrix[0, 1]);
      double recall = confusionMatrix[0, 0] / (confusionMatrix[0, 0] + confusionMatrix[1, 0]);

      return 2 * ((precision * recall) / (precision + recall));
    }
  }
}
