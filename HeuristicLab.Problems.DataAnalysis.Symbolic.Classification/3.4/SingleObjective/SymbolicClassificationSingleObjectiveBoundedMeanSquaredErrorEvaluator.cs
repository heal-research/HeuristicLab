#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification.SingleObjective {
  [Item("Bounded Mean squared error Evaluator", "Calculates the bounded mean squared error of a symbolic classification solution (estimations above or below the class values are only penaltilized linearly.")]
  [StorableClass]
  public class SymbolicClassificationSingleObjectiveBoundedMeanSquaredErrorEvaluator : SymbolicClassificationSingleObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicClassificationSingleObjectiveBoundedMeanSquaredErrorEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicClassificationSingleObjectiveBoundedMeanSquaredErrorEvaluator(SymbolicClassificationSingleObjectiveBoundedMeanSquaredErrorEvaluator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSingleObjectiveBoundedMeanSquaredErrorEvaluator(this, cloner);
    }

    public SymbolicClassificationSingleObjectiveBoundedMeanSquaredErrorEvaluator() : base() { }

    public override bool Maximization { get { return false; } }

    public override IOperation Apply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      double quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, rows, ApplyLinearScalingParameter.ActualValue.Value);
      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.Apply();
    }

    public static double Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, IClassificationProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows);
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      OnlineCalculatorError errorState;

      double lowestClassValue = problemData.ClassValues.OrderBy(x => x).First();
      double upmostClassValue = problemData.ClassValues.OrderByDescending(x => x).First();

      double boundedMse;
      if (applyLinearScaling) {
        var boundedMseCalculator = new OnlineBoundedMeanSquaredErrorCalculator(lowestClassValue, upmostClassValue);
        CalculateWithScaling(targetValues, estimatedValues, lowerEstimationLimit, upperEstimationLimit, boundedMseCalculator, problemData.Dataset.Rows);
        errorState = boundedMseCalculator.ErrorState;
        boundedMse = boundedMseCalculator.BoundedMeanSquaredError;
      } else {
        IEnumerable<double> boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
        boundedMse = OnlineBoundedMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, lowestClassValue, upmostClassValue, out errorState);
      }
      if (errorState != OnlineCalculatorError.None) return Double.NaN;
      return boundedMse;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IClassificationProblemData problemData, IEnumerable<int> rows) {
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
