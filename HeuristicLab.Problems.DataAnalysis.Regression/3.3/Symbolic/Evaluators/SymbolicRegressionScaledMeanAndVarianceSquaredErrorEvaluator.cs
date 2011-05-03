#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public sealed class SymbolicRegressionScaledMeanAndVarianceSquaredErrorEvaluator : SymbolicRegressionMeanSquaredErrorEvaluator {
    private const string QualityVarianceParameterName = "QualityVariance";
    private const string QualitySamplesParameterName = "QualitySamples";
    private const string DecompositionBiasParameterName = "QualityDecompositionBias";
    private const string DecompositionVarianceParameterName = "QualityDecompositionVariance";
    private const string DecompositionCovarianceParameterName = "QualityDecompositionCovariance";
    private const string ApplyScalingParameterName = "ApplyScaling";

    #region parameter properties
    public IValueLookupParameter<BoolValue> ApplyScalingParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters[ApplyScalingParameterName]; }
    }
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
    public ILookupParameter<DoubleValue> DecompositionBiasParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[DecompositionBiasParameterName]; }
    }
    public ILookupParameter<DoubleValue> DecompositionVarianceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[DecompositionVarianceParameterName]; }
    }
    public ILookupParameter<DoubleValue> DecompositionCovarianceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[DecompositionCovarianceParameterName]; }
    }

    #endregion
    #region properties
    public BoolValue ApplyScaling {
      get { return ApplyScalingParameter.ActualValue; }
    }
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
    [StorableConstructor]
    private SymbolicRegressionScaledMeanAndVarianceSquaredErrorEvaluator(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionScaledMeanAndVarianceSquaredErrorEvaluator(SymbolicRegressionScaledMeanAndVarianceSquaredErrorEvaluator original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionScaledMeanAndVarianceSquaredErrorEvaluator()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>(ApplyScalingParameterName, "Determines if the estimated values should be scaled.", new BoolValue(true)));
      Parameters.Add(new LookupParameter<DoubleValue>("Alpha", "Alpha parameter for linear scaling of the estimated values."));
      Parameters.Add(new LookupParameter<DoubleValue>("Beta", "Beta parameter for linear scaling of the estimated values."));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityVarianceParameterName, "A parameter which stores the variance of the squared errors."));
      Parameters.Add(new LookupParameter<IntValue>(QualitySamplesParameterName, " The number of evaluated samples."));
      Parameters.Add(new LookupParameter<DoubleValue>(DecompositionBiasParameterName, "A parameter which stores the relativ bias of the MSE."));
      Parameters.Add(new LookupParameter<DoubleValue>(DecompositionVarianceParameterName, "A parameter which stores the relativ bias of the MSE."));
      Parameters.Add(new LookupParameter<DoubleValue>(DecompositionCovarianceParameterName, "A parameter which stores the relativ bias of the MSE."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionScaledMeanAndVarianceSquaredErrorEvaluator(this, cloner);
    }

    public override double Evaluate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows) {
      double alpha, beta;
      double meanSE, varianceSE;
      int count;
      double bias, variance, covariance;
      double mse;
      if (ExecutionContext != null) {
        if (ApplyScaling.Value) {
          mse = Calculate(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, rows, out beta, out alpha, out meanSE, out varianceSE, out count, out bias, out variance, out covariance);
          Alpha = new DoubleValue(alpha);
          Beta = new DoubleValue(beta);
        } else {
          mse = CalculateWithScaling(interpreter, solution,lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, rows, 1, 0, out meanSE, out varianceSE, out count, out bias, out variance, out covariance);
        }
        QualityVariance = new DoubleValue(varianceSE);
        QualitySamples = new IntValue(count);
        DecompositionBiasParameter.ActualValue = new DoubleValue(bias / meanSE);
        DecompositionVarianceParameter.ActualValue = new DoubleValue(variance / meanSE);
        DecompositionCovarianceParameter.ActualValue = new DoubleValue(covariance / meanSE);
      } else {
        if (ApplyScalingParameter.Value != null && ApplyScalingParameter.Value.Value)
          mse = Calculate(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, rows, out beta, out alpha, out meanSE, out varianceSE, out count, out bias, out variance, out covariance);
        else
          mse = CalculateWithScaling(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, rows, 1, 0, out meanSE, out varianceSE, out count, out bias, out variance, out covariance);
      }

      return mse;
    }

    public static double Calculate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows, out double beta, out double alpha, out double meanSE, out double varianceSE, out int count, out double bias, out double variance, out double covariance) {
      IEnumerable<double> originalValues = dataset.GetEnumeratedVariableValues(targetVariable, rows);
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, dataset, rows);
      CalculateScalingParameters(originalValues, estimatedValues, out beta, out alpha);

      return CalculateWithScaling(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, rows, beta, alpha, out meanSE, out varianceSE, out count, out bias, out variance, out covariance);
    }

    public static double CalculateWithScaling(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows, double beta, double alpha, out double meanSE, out double varianceSE, out int count, out double bias, out double variance, out double covariance) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, dataset, rows);
      IEnumerable<double> originalValues = dataset.GetEnumeratedVariableValues(targetVariable, rows);
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlineMeanAndVarianceCalculator seEvaluator = new OnlineMeanAndVarianceCalculator();
      OnlineMeanAndVarianceCalculator originalMeanEvaluator = new OnlineMeanAndVarianceCalculator();
      OnlineMeanAndVarianceCalculator estimatedMeanEvaluator = new OnlineMeanAndVarianceCalculator();
      OnlinePearsonsRSquaredEvaluator r2Evaluator = new OnlinePearsonsRSquaredEvaluator();

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
        originalMeanEvaluator.Add(original);
        estimatedMeanEvaluator.Add(estimated);
        r2Evaluator.Add(original, estimated);
      }

      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else {
        meanSE = seEvaluator.Mean;
        varianceSE = seEvaluator.Variance;
        count = seEvaluator.Count;
        bias = (originalMeanEvaluator.Mean - estimatedMeanEvaluator.Mean);
        bias *= bias;

        double sO = Math.Sqrt(originalMeanEvaluator.Variance);
        double sE = Math.Sqrt(estimatedMeanEvaluator.Variance);
        variance = sO - sE;
        variance *= variance;
        double r = Math.Sqrt(r2Evaluator.RSquared);
        covariance = 2 * sO * sE * (1 - r);
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
