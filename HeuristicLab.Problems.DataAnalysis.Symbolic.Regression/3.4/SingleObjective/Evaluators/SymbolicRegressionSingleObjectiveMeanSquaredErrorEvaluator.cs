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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Mean squared error Evaluator", "Calculates the mean squared error of a symbolic regression solution.")]
  [StorableType("8D4B5243-1635-46A6-AEF9-18C9BCB725DD")]
  public class SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator : SymbolicRegressionSingleObjectiveEvaluator {
    public override bool Maximization { get { return false; } }
    [StorableConstructor]
    protected SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator(SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator(this, cloner);
    }
    public SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator() : base() { }

    public override IOperation InstrumentedApply() {
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      IEnumerable<int> rows = GenerateRowsToEvaluate();

      double quality = Calculate(
        tree, ProblemDataParameter.ActualValue, 
        rows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, 
        ApplyLinearScalingParameter.ActualValue.Value,         
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
      bool applyLinearScaling,
      double lowerEstimationLimit, double upperEstimationLimit) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, rows);
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      OnlineCalculatorError errorState;

      double mse;
      if (applyLinearScaling) {
        var mseCalculator = new OnlineMeanSquaredErrorCalculator();
        CalculateWithScaling(targetValues, estimatedValues, lowerEstimationLimit, upperEstimationLimit, mseCalculator, problemData.Dataset.Rows);
        errorState = mseCalculator.ErrorState;
        mse = mseCalculator.MeanSquaredError;
      } else {
        IEnumerable<double> boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
        mse = OnlineMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      }
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return mse;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      double mse = Calculate(        
        tree, problemData, rows, 
        SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, 
        ApplyLinearScalingParameter.ActualValue.Value,
        EstimationLimitsParameter.ActualValue.Lower, 
        EstimationLimitsParameter.ActualValue.Upper);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return mse;
    }

    public override double Evaluate(
      ISymbolicExpressionTree tree,
      IRegressionProblemData problemData,
      IEnumerable<int> rows,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      bool applyLinearScaling = true,
      double lowerEstimationLimit = double.MinValue,
      double upperEstimationLimit = double.MaxValue) {
      return Calculate(tree, problemData, rows, interpreter, applyLinearScaling, lowerEstimationLimit, upperEstimationLimit);
    }
  }
}
