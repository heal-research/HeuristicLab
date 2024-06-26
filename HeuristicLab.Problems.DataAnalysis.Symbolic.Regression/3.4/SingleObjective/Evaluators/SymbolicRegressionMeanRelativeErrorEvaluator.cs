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
  [Item("Mean relative error Evaluator", "Evaluator for symbolic regression models that calculates the mean relative error avg( |y' - y| / (|y| + 1))." +
                                         "The +1 is necessary to handle data with the value of 0.0 correctly. " +
                                         "Notice: Linear scaling is ignored for this evaluator.")]
  [StorableType("8A5AAF93-5338-4E11-B3B2-3D9274329E5F")]
  public class SymbolicRegressionMeanRelativeErrorEvaluator : SymbolicRegressionSingleObjectiveEvaluator {
    public override bool Maximization { get { return false; } }
    [StorableConstructor]
    protected SymbolicRegressionMeanRelativeErrorEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicRegressionMeanRelativeErrorEvaluator(SymbolicRegressionMeanRelativeErrorEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionMeanRelativeErrorEvaluator(this, cloner);
    }
    public SymbolicRegressionMeanRelativeErrorEvaluator() : base() { }

    public override IOperation InstrumentedApply() {
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      IEnumerable<int> rows = GenerateRowsToEvaluate();

      double quality = Calculate(
        tree, ProblemDataParameter.ActualValue, rows,
        SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
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
      double lowerEstimationLimit, double upperEstimationLimit) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, rows);
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      IEnumerable<double> boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);

      var relResiduals = boundedEstimatedValues.Zip(targetValues, (e, t) => Math.Abs(t - e) / (Math.Abs(t) + 1.0));

      OnlineCalculatorError errorState;
      OnlineCalculatorError varErrorState;
      double mre;
      double variance;
      OnlineMeanAndVarianceCalculator.Calculate(relResiduals, out mre, out variance, out errorState, out varErrorState);
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return mre;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;

      double mre = Calculate(
        tree, problemData, rows,
        SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
        EstimationLimitsParameter.ActualValue.Lower, 
        EstimationLimitsParameter.ActualValue.Upper);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;

      return mre;
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