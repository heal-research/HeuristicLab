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
  [Item("SymbolicRegressionScaledNormalizedMeanSquaredErrorEvaluator", "Calculates the normalized mean squared error of a linearly scaled symbolic regression solution.")]
  [StorableClass]
  public class SymbolicRegressionScaledNormalizedMeanSquaredErrorEvaluator : SymbolicRegressionNormalizedMeanSquaredErrorEvaluator {

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
    public SymbolicRegressionScaledNormalizedMeanSquaredErrorEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Alpha", "Alpha parameter for linear scaling of the estimated values."));
      Parameters.Add(new LookupParameter<DoubleValue>("Beta", "Beta parameter for linear scaling of the estimated values."));
    }

    protected override double Evaluate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, Dataset dataset, StringValue targetVariable, IEnumerable<int> rows) {
      double alpha, beta;
      double nmse = Calculate(interpreter, solution, LowerEstimationLimit.Value, UpperEstimationLimit.Value, dataset, targetVariable.Value, rows, out beta, out alpha);
      AlphaParameter.ActualValue = new DoubleValue(alpha);
      BetaParameter.ActualValue = new DoubleValue(beta);
      return nmse;
    }

    public static double Calculate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows, out double beta, out double alpha) {
      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, dataset, rows);
      var originalValues = dataset.GetEnumeratedVariableValues(targetVariable, rows);

      SymbolicRegressionScaledMeanSquaredErrorEvaluator.CalculateScalingParameters(originalValues, estimatedValues, out beta, out alpha);
      return CalculateWithScaling(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, rows, beta, alpha);
    }

    public static double CalculateWithScaling(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows, double beta, double alpha) {
      int targetVariableIndex = dataset.GetVariableIndex(targetVariable);
      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, dataset, rows);
      var originalValues = dataset.GetEnumeratedVariableValues(targetVariableIndex, rows);
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlineNormalizedMeanSquaredErrorEvaluator mseEvaluator = new OnlineNormalizedMeanSquaredErrorEvaluator();

      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double estimated = estimatedEnumerator.Current * beta + alpha;
        double original = originalEnumerator.Current;
        if (double.IsNaN(estimated))
          estimated = upperEstimationLimit;
        else
          estimated = Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, estimated));
        mseEvaluator.Add(original, estimated);
      }

      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else {
        return mseEvaluator.NormalizedMeanSquaredError;
      }
    }
  }
}
