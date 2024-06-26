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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Log Residual Evaluator", "Evaluator for symbolic regression models that calculates the mean of logarithmic absolute residuals avg(log( 1 + abs(y' - y)))" + 
                                  "This evaluator does not perform linear scaling!" +
                                  "This evaluator can be useful if the modeled function contains discontinuities (e.g. 1/x). " +
                                  "For some data sets (e.g. Korns benchmark instances containing inverses of near zero values) the squared error or absolute " +
                                  "error put too much emphasis on modeling the outlier values. Using log-residuals instead has the " +
                                  "effect that smaller residuals have a stronger impact on the total quality compared to the large residuals." +
                                  "This effects GP convergence because functional fragments which are necessary to explain small variations are also more likely" +
                                  " to stay in the population. This is useful even when the actual objective function is mean of squared errors.")]
  [StorableType("8CEA1A56-167D-481B-9167-C1DED8E06680")]
  public class SymbolicRegressionLogResidualEvaluator : SymbolicRegressionSingleObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicRegressionLogResidualEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicRegressionLogResidualEvaluator(SymbolicRegressionLogResidualEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionLogResidualEvaluator(this, cloner);
    }

    public SymbolicRegressionLogResidualEvaluator() : base() { }

    public override bool Maximization { get { return false; } }

    public override IOperation InstrumentedApply() {
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      IEnumerable<int> rows = GenerateRowsToEvaluate();

      double quality = Calculate(
        tree, ProblemDataParameter.ActualValue, 
        rows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
        EstimationLimitsParameter.ActualValue.Lower, 
        EstimationLimitsParameter.ActualValue.Upper);
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.InstrumentedApply();
    }

    public static double Calculate(
      ISymbolicExpressionTree tree,
      IRegressionProblemData problemData,
      IEnumerable<int> rows,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      double lowerEstimationLimit, 
      double upperEstimationLimit) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, rows);
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      IEnumerable<double> boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);

      var logRes = boundedEstimatedValues.Zip(targetValues, (e, t) => Math.Log(1.0 + Math.Abs(e - t)));

      OnlineCalculatorError errorState;
      OnlineCalculatorError varErrorState;
      double mlr;
      double variance;
      OnlineMeanAndVarianceCalculator.Calculate(logRes, out mlr, out variance, out errorState, out varErrorState);
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return mlr;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;

      double mlr = Calculate(
        tree, problemData, rows, 
        SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
        EstimationLimitsParameter.ActualValue.Lower, 
        EstimationLimitsParameter.ActualValue.Upper);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;

      return mlr;
    }

    public override double Evaluate(
      ISymbolicExpressionTree tree,
      IRegressionProblemData problemData,
      IEnumerable<int> rows,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      bool applyLinearScaling = true,
      double lowerEstimationLimit = double.MinValue,
      double upperEstimationLimit = double.MaxValue) {
      return Calculate(tree, problemData, rows, interpreter, lowerEstimationLimit, upperEstimationLimit);
    }
  }
}
