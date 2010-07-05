#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Operators;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("SymbolicRegressionScaledMeanSquaredErrorEvaluator", "Calculates the mean squared error of a linearly scaled symbolic regression solution.")]
  [StorableClass]
  public class SymbolicRegressionScaledMeanSquaredErrorEvaluator : SymbolicRegressionMeanSquaredErrorEvaluator {

    #region parameter properties
    public ILookupParameter<DoubleValue> AlphaParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Alpha"]; }
    }
    public ILookupParameter<DoubleValue> BetaParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Beta"]; }
    }
    #endregion
    #region properties
    public DoubleValue Alpha {
      get { return AlphaParameter.ActualValue; }
      set { AlphaParameter.ActualValue = value; }
    }
    public DoubleValue Beta {
      get { return BetaParameter.ActualValue; }
      set { BetaParameter.ActualValue = value; }
    }
    #endregion
    public SymbolicRegressionScaledMeanSquaredErrorEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Alpha", "Alpha parameter for linear scaling of the estimated values."));
      Parameters.Add(new LookupParameter<DoubleValue>("Beta", "Beta parameter for linear scaling of the estimated values."));
    }

    protected override double Evaluate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, Dataset dataset, StringValue targetVariable, IntValue samplesStart, IntValue samplesEnd) {
      double alpha, beta;
      double mse = Calculate(interpreter, solution, LowerEstimationLimit.Value, UpperEstimationLimit.Value, dataset, targetVariable.Value, samplesStart.Value, samplesEnd.Value, out beta, out alpha);
      AlphaParameter.ActualValue = new DoubleValue(alpha);
      BetaParameter.ActualValue = new DoubleValue(beta);
      return mse;
    }

    public static double Calculate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, int start, int end, out double beta, out double alpha) {
      IEnumerable<double> originalValues = dataset.GetEnumeratedVariableValues(targetVariable, start, end);
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, dataset, Enumerable.Range(start, end - start));
      CalculateScalingParameters(originalValues, estimatedValues, out beta, out alpha);

      return CalculateWithScaling(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, start, end, beta, alpha);
    }

    public static double CalculateWithScaling(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, int start, int end, double beta, double alpha) {
      //IEnumerable<double> estimatedValues = from x in interpreter.GetSymbolicExpressionTreeValues(solution, dataset, Enumerable.Range(start, end - start))
      //                                      let boundedX = Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, x * beta + alpha))
      //                                      select double.IsNaN(boundedX) ? upperEstimationLimit : boundedX;
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, dataset, Enumerable.Range(start, end - start));
      IEnumerable<double> originalValues = dataset.GetEnumeratedVariableValues(targetVariable, start, end);
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      double cnt = 0;
      double sse = 0;

      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double estimated = estimatedEnumerator.Current * beta + alpha;
        double original = originalEnumerator.Current;
        estimated = Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, estimated));
        if (double.IsNaN(estimated))
          estimated = upperEstimationLimit;
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
            !double.IsNaN(original) && !double.IsInfinity(original)) {
          double error = estimated - original;
          sse += error * error;
          cnt++;
        }
      }

      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else if (cnt == 0) {
        throw new ArgumentException("Mean squared errors is not defined for input vectors of NaN or Inf");
      } else {
        double mse = sse / cnt;
        return mse;
      }
    }

    /// <summary>
    /// Calculates linear scaling parameters in one pass.
    /// The formulas to calculate the scaling parameters were taken from Scaled Symblic Regression by Maarten Keijzer.
    /// http://www.springerlink.com/content/x035121165125175/
    /// </summary>
    public static void CalculateScalingParameters(IEnumerable<double> original, IEnumerable<double> estimated, out double beta, out double alpha) {
      IEnumerator<double> originalEnumerator = original.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimated.GetEnumerator();

      int cnt = 0;
      double tSum = 0;
      double ySum = 0;
      double yySum = 0;
      double ytSum = 0;

      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double y = estimatedEnumerator.Current;
        double t = originalEnumerator.Current;
        if (IsValidValue(t) && IsValidValue(y)) {
          cnt++;
          tSum += t;
          ySum += y;
          yySum += y * y;
          ytSum += t * y;
        }
      }

      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext())
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      if (cnt < 2) {
        alpha = 0;
        beta = 1;
      } else {
        double tMean = tSum / cnt;
        double yMean = ySum / cnt;
        //division by cnt is omited because the variance and covariance are divided afterwards.
        double yVariance = yySum - 2 * yMean * ySum + cnt * yMean * yMean;
        double ytCovariance = ytSum - tMean * ySum - yMean * tSum + cnt * yMean * tMean;

        if (yVariance.IsAlmost(0.0))
          beta = 1;
        else
          beta = ytCovariance / yVariance;

        alpha = tMean - beta * yMean;
      }
    }

    private static bool IsValidValue(double d) {
      return !double.IsInfinity(d) && !double.IsNaN(d) && d > -1.0E07 && d < 1.0E07;  // don't consider very large or very small values for scaling
    }
  }
}
