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

namespace HeuristicLab.Problems.DataAnalysis.OnlineCalculators {
  public class MatthewsCorrelationCoefficientCalculator {
    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      var confusionMatrix = ConfusionMatrixCalculator.Calculate(originalValues, estimatedValues, out errorState);
      if (!errorState.Equals(OnlineCalculatorError.None)) {
        return double.NaN;
      }
      return CalculateMCC(confusionMatrix);
    }

    private static double CalculateMCC(double[,] confusionMatrix) {
      if (confusionMatrix.GetLength(0) != confusionMatrix.GetLength(1)) {
        throw new ArgumentException("Confusion matrix is not a square matrix.");
      }

      int classes = confusionMatrix.GetLength(0);
      double numerator = 0;
      for (int k = 0; k < classes; k++) {
        for (int l = 0; l < classes; l++) {
          for (int m = 0; m < classes; m++) {
            numerator += confusionMatrix[k, k] * confusionMatrix[m, l] - confusionMatrix[l, k] * confusionMatrix[k, m];
          }
        }
      }

      double denominator1 = 0;
      double denominator2 = 0;
      for (int k = 0; k < classes; k++) {
        double clk = 0;
        double cgf = 0;
        double ckl = 0;
        double cfg = 0;
        for (int l = 0; l < classes; l++) {
          clk += confusionMatrix[l, k];
          ckl += confusionMatrix[k, l];
        }
        for (int f = 0; f < classes; f++) {
          if (f == k) {
            continue;
          }
          for (int g = 0; g < classes; g++) {
            cgf += confusionMatrix[g, f];
            cfg += confusionMatrix[f, g];
          }
        }
        denominator1 += clk * cgf;
        denominator2 += ckl * cfg;
      }
      denominator1 = Math.Sqrt(denominator1);
      denominator2 = Math.Sqrt(denominator2);

      return numerator / (denominator1 * denominator2);
    }
  }
}
