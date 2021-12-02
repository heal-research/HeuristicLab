#region License Information
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Mean squared error & Tree size Evaluator", "Calculates the mean squared error and the tree size of a symbolic regression solution.")]
  [StorableType("B1EFB303-9C37-4CBB-8269-BDBC223D9086")]
  public class SymbolicRegressionMultiObjectiveMeanSquaredErrorSolutionSizeEvaluator : SymbolicRegressionMultiObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicRegressionMultiObjectiveMeanSquaredErrorSolutionSizeEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicRegressionMultiObjectiveMeanSquaredErrorSolutionSizeEvaluator(SymbolicRegressionMultiObjectiveMeanSquaredErrorSolutionSizeEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionMultiObjectiveMeanSquaredErrorSolutionSizeEvaluator(this, cloner);
    }

    public SymbolicRegressionMultiObjectiveMeanSquaredErrorSolutionSizeEvaluator() : base() { }

    public override IEnumerable<bool> Maximization { get { return new bool[2] { false, false }; } }

    public override IOperation InstrumentedApply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      var interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
      var estimationLimits = EstimationLimitsParameter.ActualValue;
      var applyLinearScaling = ApplyLinearScalingParameter.ActualValue.Value;

      if (UseConstantOptimization) {
        SymbolicRegressionConstantOptimizationEvaluator.OptimizeConstants(interpreter, tree, problemData, rows, applyLinearScaling, ConstantOptimizationIterations, updateVariableWeights: ConstantOptimizationUpdateVariableWeights, lowerEstimationLimit: estimationLimits.Lower, upperEstimationLimit: estimationLimits.Upper);
      }

      double[] qualities = Calculate(
        tree, ProblemDataParameter.ActualValue, 
        rows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, 
        ApplyLinearScalingParameter.ActualValue.Value, 
        EstimationLimitsParameter.ActualValue.Lower, 
        EstimationLimitsParameter.ActualValue.Upper, 
        DecimalPlaces);
      QualitiesParameter.ActualValue = new DoubleArray(qualities);
      return base.InstrumentedApply();
    }

    public static double[] Calculate(
      ISymbolicExpressionTree tree,
      IRegressionProblemData problemData, 
      IEnumerable<int> rows,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      bool applyLinearScaling,
      double lowerEstimationLimit, double upperEstimationLimit,
      int decimalPlaces) {
      var mse = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(
        tree, problemData, rows, 
        interpreter, applyLinearScaling, 
        lowerEstimationLimit,
        upperEstimationLimit);

      if (decimalPlaces >= 0)
        mse = Math.Round(mse, decimalPlaces);

      return new double[2] { mse, tree.Length };
    }

    public override double[] Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      double[] quality = Calculate(
        tree, problemData, rows, 
        SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, 
        ApplyLinearScalingParameter.ActualValue.Value, 
        EstimationLimitsParameter.ActualValue.Lower, 
        EstimationLimitsParameter.ActualValue.Upper, DecimalPlaces);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return quality;
    }
  }
}
