#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;

namespace HeuristicLab.DataAnalysis {
  public class LinearStatistics {
    /// <summary>
    /// Calculates linear regression for the given data. The result is given as regression values, which are returned,
    /// as well as the characteristic coefficients <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    /// <param name="Data">The data that is given; linear regression is calculated for these data samples.</param>
    /// <param name="a">Regression coefficient 'a' (output parameter).</param>
    /// <param name="b">Regression coefficient 'b' (output parameter).</param>
    /// <returns>Calculated linear regression values.</returns>
    public static double[] LinearRegression(double[] data, out double a, out double b) {
      int n = data.Length;
      double xMean = n / 2.0;
      double yMean = Statistics.Mean(data);
      double[] xMinusMean = new double[n];
      double[] yMinusMean = new double[n];
      double[] xMinusMeanSquared = new double[n];
      double[] xMinusMeanTimesYMinusMean = new double[n];

      double ssxx = 0;
      double ssxy = 0;
      for(int i = 0; i < n; i++) {
        xMinusMean[i] = i - xMean;
        yMinusMean[i] = data[i] - yMean;
        xMinusMeanSquared[i] = xMinusMean[i] * xMinusMean[i];
        xMinusMeanTimesYMinusMean[i] = xMinusMean[i] * yMinusMean[i];
        ssxx += xMinusMeanSquared[i];
        ssxy += xMinusMeanTimesYMinusMean[i];
      }
      b = ssxy / ssxx;
      a = yMean - b * xMean;
      double[] result = new double[n];
      for(int x = 0; x < n; x++)
        result[x] = a + x * b;
      return result;
    }

    /// <summary>
    /// Calculates linear regression for the given data. The result is given as regression values, which are returned,
    /// as well as the characteristic coefficients <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    /// <param name="Data">The data that is given; linear regression is calculated for these data samples.</param>
    /// <returns>Calculated linear regression values.</returns>
    public static double[] LinearRegression(double[] data) {
      double a, b;
      return LinearRegression(data, out a, out b);
    }

    public static double CorrelationCoefficient(double[] xValues, double[] yValues) {
      if(xValues.Length != yValues.Length)
        throw new Exception("ERROR in CorrelationCoefficient: The given variables have to be equally long!");
      int n = xValues.Length;
      double[] x = new double[n];
      double[] y = new double[n];
      for(int i = 0; i < n; i++) {
        if(double.IsNaN(xValues[i]))
          throw new NotFiniteNumberException();
        else
          x[i] = xValues[i];
        if(double.IsNaN(yValues[i]))
          throw new NotFiniteNumberException();
        else
          y[i] = yValues[i];
      }
      double OneOverN = 1.0 / (n + 1);
      double xMean = Statistics.Mean(x);
      double yMean = Statistics.Mean(y);
      double[] xMinusMean = new double[n];
      double[] yMinusMean = new double[n];
      double[] xMinusMeanSquared = new double[n];
      double xMinusMeanSquaredSum = 0.0;
      double[] yMinusMeanSquared = new double[n];
      double yMinusMeanSquaredSum = 0.0;
      double[] xMinusMeanTimesYMinusMean = new double[n];
      double xMinusMeanTimesYMinusMeanSum = 0.0;
      for(int i = 0; i < n; i++) {
        xMinusMean[i] = x[i] - xMean;
        yMinusMean[i] = y[i] - yMean;
        xMinusMeanSquared[i] = xMinusMean[i] * xMinusMean[i];
        xMinusMeanSquaredSum += xMinusMeanSquared[i];
        yMinusMeanSquared[i] = yMinusMean[i] * yMinusMean[i];
        yMinusMeanSquaredSum += yMinusMeanSquared[i];
        xMinusMeanTimesYMinusMean[i] = xMinusMean[i] * yMinusMean[i];
        xMinusMeanTimesYMinusMeanSum += xMinusMeanTimesYMinusMean[i];
      }
      return (OneOverN * xMinusMeanTimesYMinusMeanSum) /
        (Math.Sqrt(OneOverN * xMinusMeanSquaredSum) * Math.Sqrt(OneOverN * yMinusMeanSquaredSum));
    }

    #region Coefficient of Determination (R-squared)
    /// <summary>
    /// In statistics, the coefficient of determination (R-squared) is the proportion of a sample variance
    /// of a response variable that is "explained" by the predictor (explanatory) variables when regression is done.
    /// </summary>
    /// <param name="originalValues">The original values for which a model shall be created.</param>
    /// <param name="residuals">The errors between original and predicted values.</param>
    /// <returns></returns>
    public static double CoefficientOfDetermination(double[] originalValues, double[] residuals) {
      int n = originalValues.Length;

      double originalValuesMean = Statistics.Mean(originalValues);

      double[] originalValuesMinusMeanSquared = new double[n];
      originalValuesMinusMeanSquared = Array.ConvertAll<double, double>(originalValues, delegate(double v) {
        double t = v - originalValuesMean;
        return t * t;
      });

      double totalSumOfSquares = Statistics.Sum(originalValuesMinusMeanSquared);

      double[] residualsSquared = new double[residuals.Length];
      residualsSquared = Array.ConvertAll<double, double>(residuals, delegate(double r) {
        return r * r;
      });

      double sumOfSquaredResiduals = Statistics.Sum(residualsSquared);

      return (1.0 - sumOfSquaredResiduals / totalSumOfSquares);
    }
    #endregion

    #region Adjusted Coefficient of Determination (Adjusted R-squared)
    public static double AdjustedCoefficientOfDetermination(double[] originalValues, double[] residuals, int numberOfExplanatoryTerms) {
      double rSquared = CoefficientOfDetermination(originalValues, residuals);
      double n = originalValues.Length;
      return (1 - (1 - rSquared) * (n - 1) / (n - numberOfExplanatoryTerms - 1));
    }
    #endregion
  }
}
