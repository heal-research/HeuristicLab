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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("SymbolicRegressionScaledMeanAndVarianceSquaredErrorEvaluator", "Calculates the mean and the variance of the squared errors of a linearly scaled symbolic regression solution.")]
  [StorableClass]
  public class SymbolicRegressionScaledMeanAndVarianceSquaredErrorEvaluator : SymbolicRegressionMeanSquaredErrorEvaluator {
    private const string QualityVarianceParameterName = "QualityVariance";
    private const string QualitySamplesParameterName = "QualitySamples";

    #region parameter properties
    public ILookupParameter<DoubleValue> AlphaParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Alpha"]; }
    }
    public ILookupParameter<DoubleValue> BetaParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Beta"]; }
    }
    public ILookupParameter<DoubleValue> QualityVarianceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityVarianceParameterName]; }
    }
    public ILookupParameter<IntValue> QualitySamplesParameter {
      get { return (ILookupParameter<IntValue>)Parameters[QualitySamplesParameterName]; }
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
    public DoubleValue QualityVariance {
      get { return QualityVarianceParameter.ActualValue; }
      set { QualityVarianceParameter.ActualValue = value; }
    }
    public IntValue QualitySamples {
      get { return QualitySamplesParameter.ActualValue; }
      set { QualitySamplesParameter.ActualValue = value; }
    }
    #endregion
    public SymbolicRegressionScaledMeanAndVarianceSquaredErrorEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Alpha", "Alpha parameter for linear scaling of the estimated values."));
      Parameters.Add(new LookupParameter<DoubleValue>("Beta", "Beta parameter for linear scaling of the estimated values."));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityVarianceParameterName, "A parameter which stores the variance of the squared errors."));
      Parameters.Add(new LookupParameter<IntValue>(QualitySamplesParameterName, " The number of evaluated samples."));
    }

    protected override double Evaluate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, Dataset dataset, StringValue targetVariable, IEnumerable<int> rows) {
      double alpha, beta;
      double meanSE, varianceSE;
      int count;
      double mse = Calculate(interpreter, solution, LowerEstimationLimit.Value, UpperEstimationLimit.Value, dataset, targetVariable.Value, rows, out beta, out alpha, out meanSE, out varianceSE, out count);
      Alpha = new DoubleValue(alpha);
      Beta = new DoubleValue(beta);
      QualityVariance = new DoubleValue(varianceSE);
      QualitySamples = new IntValue(count);
      return mse;
    }

    public static double Calculate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows, out double beta, out double alpha, out double meanSE, out double varianceSE, out int count) {
      IEnumerable<double> originalValues = dataset.GetEnumeratedVariableValues(targetVariable, rows);
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, dataset, rows);
      CalculateScalingParameters(originalValues, estimatedValues, out beta, out alpha);

      return CalculateWithScaling(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, rows, beta, alpha, out meanSE, out varianceSE, out count);
    }

    public static double CalculateWithScaling(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows, double beta, double alpha, out double meanSE, out double varianceSE, out int count) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, dataset, rows);
      IEnumerable<double> originalValues = dataset.GetEnumeratedVariableValues(targetVariable, rows);
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlineMeanAndVarianceCalculator seEvaluator = new OnlineMeanAndVarianceCalculator();

      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double estimated = estimatedEnumerator.Current * beta + alpha;
        double original = originalEnumerator.Current;
        if (double.IsNaN(estimated))
          estimated = upperEstimationLimit;
        else
          estimated = Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, estimated));
        double error = estimated - original;
        error *= error;
        seEvaluator.Add(error);
      }

      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else {
        meanSE = seEvaluator.Mean;
        varianceSE = seEvaluator.Variance;
        count = seEvaluator.Count;
        return seEvaluator.Mean;
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
      OnlineMeanAndVarianceCalculator yVarianceCalculator = new OnlineMeanAndVarianceCalculator();
      OnlineMeanAndVarianceCalculator tMeanCalculator = new OnlineMeanAndVarianceCalculator();
      OnlineCovarianceEvaluator ytCovarianceEvaluator = new OnlineCovarianceEvaluator();
      int cnt = 0;

      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double y = estimatedEnumerator.Current;
        double t = originalEnumerator.Current;
        if (IsValidValue(t) && IsValidValue(y)) {
          tMeanCalculator.Add(t);
          yVarianceCalculator.Add(y);
          ytCovarianceEvaluator.Add(y, t);

          cnt++;
        }
      }

      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext())
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      if (cnt < 2) {
        alpha = 0;
        beta = 1;
      } else {
        if (yVarianceCalculator.Variance.IsAlmost(0.0))
          beta = 1;
        else
          beta = ytCovarianceEvaluator.Covariance / yVarianceCalculator.Variance;

        alpha = tMeanCalculator.Mean - beta * yVarianceCalculator.Mean;
      }
    }

    private static bool IsValidValue(double d) {
      return !double.IsInfinity(d) && !double.IsNaN(d) && d > -1.0E07 && d < 1.0E07;  // don't consider very large or very small values for scaling
    }
  }
}
