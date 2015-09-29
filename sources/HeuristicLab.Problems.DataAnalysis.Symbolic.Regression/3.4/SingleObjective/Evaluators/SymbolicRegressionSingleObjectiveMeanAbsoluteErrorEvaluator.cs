#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Mean absolute error Evaluator", "Calculates the mean absolute error of a symbolic regression solution.")]
  [StorableClass]
  public class SymbolicRegressionSingleObjectiveMeanAbsoluteErrorEvaluator : SymbolicRegressionSingleObjectiveEvaluator {
    public override bool Maximization { get { return false; } }
    [StorableConstructor]
    protected SymbolicRegressionSingleObjectiveMeanAbsoluteErrorEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicRegressionSingleObjectiveMeanAbsoluteErrorEvaluator(SymbolicRegressionSingleObjectiveMeanAbsoluteErrorEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSingleObjectiveMeanAbsoluteErrorEvaluator(this, cloner);
    }
    public SymbolicRegressionSingleObjectiveMeanAbsoluteErrorEvaluator() : base() { }

    public override IOperation InstrumentedApply() {
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      IEnumerable<int> rows = GenerateRowsToEvaluate();

      var problemData = ProblemDataParameter.ActualValue;
      var interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows).ToArray();
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var estimationLimits = EstimationLimitsParameter.ActualValue;

      if (SaveEstimatedValuesToScope) {
        var boundedValues = estimatedValues.LimitToRange(estimationLimits.Lower, estimationLimits.Upper).ToArray();
        var scope = ExecutionContext.Scope;
        if (scope.Variables.ContainsKey("EstimatedValues"))
          scope.Variables["EstimatedValues"].Value = new DoubleArray(boundedValues);
        else
          scope.Variables.Add(new Core.Variable("EstimatedValues", new DoubleArray(boundedValues)));
      }

      double quality = Calculate(targetValues, estimatedValues, estimationLimits.Lower, estimationLimits.Upper, problemData, ApplyLinearScalingParameter.ActualValue.Value);
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.InstrumentedApply();
    }

    public static double Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, IRegressionProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows);
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      return Calculate(targetValues, estimatedValues, lowerEstimationLimit, upperEstimationLimit, problemData, applyLinearScaling);
    }

    private static double Calculate(IEnumerable<double> targetValues, IEnumerable<double> estimatedValues, double lowerEstimationLimit, double upperEstimationLimit, IRegressionProblemData problemData, bool applyLinearScaling) {
      OnlineCalculatorError errorState;

      double mae;
      if (applyLinearScaling) {
        var maeCalculator = new OnlineMeanAbsoluteErrorCalculator();
        CalculateWithScaling(targetValues, estimatedValues, lowerEstimationLimit, upperEstimationLimit, maeCalculator, problemData.Dataset.Rows);
        errorState = maeCalculator.ErrorState;
        mae = maeCalculator.MeanAbsoluteError;
      } else {
        IEnumerable<double> boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
        mae = OnlineMeanAbsoluteErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      }
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return mae;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      double mse = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows, ApplyLinearScalingParameter.ActualValue.Value);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return mse;
    }
  }
}
