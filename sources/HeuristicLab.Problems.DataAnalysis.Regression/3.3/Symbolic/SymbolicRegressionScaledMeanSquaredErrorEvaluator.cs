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
      var estimatedValues = CalculateScaledEstimatedValues(interpreter, solution, dataset, targetVariable, start, end, out beta, out alpha);
      estimatedValues = from x in estimatedValues
                        let boundedX = Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, x))
                        select double.IsNaN(boundedX) ? upperEstimationLimit : boundedX;
      var originalValues = dataset.GetVariableValues(targetVariable, start, end);
      return SimpleMSEEvaluator.Calculate(originalValues, estimatedValues);
    }

    public static double CalculateWithScaling(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, int start, int end, double beta, double alpha) {
      var estimatedValues = from x in interpreter.GetSymbolicExpressionTreeValues(solution, dataset, Enumerable.Range(start, end - start))
                            let boundedX = Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, x * beta + alpha))
                            select double.IsNaN(boundedX) ? upperEstimationLimit : boundedX;
      var originalValues = dataset.GetVariableValues(targetVariable, start, end);
      return SimpleMSEEvaluator.Calculate(originalValues, estimatedValues);
    }

    private static IEnumerable<double> CalculateScaledEstimatedValues(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, Dataset dataset, string targetVariable, int start, int end, out double beta, out double alpha) {
      int targetVariableIndex = dataset.GetVariableIndex(targetVariable);
      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, dataset, Enumerable.Range(start, end - start)).ToList();
      var originalValues = dataset.GetVariableValues(targetVariable, start, end);
      CalculateScalingParameters(originalValues, estimatedValues, out beta, out alpha);
      for (int i = 0; i < estimatedValues.Count; i++)
        estimatedValues[i] = estimatedValues[i] * beta + alpha;
      return estimatedValues;
    }


    public static void CalculateScalingParameters(IEnumerable<double> original, IEnumerable<double> estimated, out double beta, out double alpha) {
      var originalEnumerator = original.GetEnumerator();
      var estimatedEnumerator = estimated.GetEnumerator();

      double tMean = original.Average();
      double xMean = estimated.Average();
      double sumXT = 0;
      double sumXX = 0;
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        // calculate alpha and beta on the subset of rows with valid values 
        if (IsValidValue(originalEnumerator.Current) && IsValidValue(estimatedEnumerator.Current)) {
          double x = estimatedEnumerator.Current;
          double t = originalEnumerator.Current;
          sumXT += (x - xMean) * (t - tMean);
          sumXX += (x - xMean) * (x - xMean);
        }
      }
      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in estimated and original doesn't match.");
      }
      if (sumXX != 0) {
        beta = sumXT / sumXX;
      } else {
        beta = 1;
      }
      alpha = tMean - beta * xMean;
    }

    private static bool IsValidValue(double d) {
      return !double.IsInfinity(d) && !double.IsNaN(d);
    }
  }
}
