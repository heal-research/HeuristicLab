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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [Item("Pearson R² evaluator", "Calculates the square of the pearson correlation coefficient (also known as coefficient of determination) of a symbolic classification solution.")]
  [StorableType("F6F480F9-21DC-4D22-9D5C-5951906BEB79")]
  public class SymbolicClassificationSingleObjectivePearsonRSquaredEvaluator : SymbolicClassificationSingleObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicClassificationSingleObjectivePearsonRSquaredEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicClassificationSingleObjectivePearsonRSquaredEvaluator(SymbolicClassificationSingleObjectivePearsonRSquaredEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSingleObjectivePearsonRSquaredEvaluator(this, cloner);
    }

    public SymbolicClassificationSingleObjectivePearsonRSquaredEvaluator() : base() { }

    public override bool Maximization { get { return true; } }

    public override IOperation InstrumentedApply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      double quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, rows, ApplyLinearScalingParameter.ActualValue.Value);
      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.InstrumentedApply();
    }

    public static double Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, IClassificationProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows);
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      OnlineCalculatorError errorState;

      double r;
      if (applyLinearScaling) {
        var rCalculator = new OnlinePearsonsRCalculator();
        CalculateWithScaling(targetValues, estimatedValues, lowerEstimationLimit, upperEstimationLimit, rCalculator, problemData.Dataset.Rows);
        errorState = rCalculator.ErrorState;
        r = rCalculator.R;
      } else {
        IEnumerable<double> boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
        r = OnlinePearsonsRCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      }
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return r*r;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IClassificationProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      double r2 = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows, ApplyLinearScalingParameter.ActualValue.Value);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return r2;
    }
    
    public override double Evaluate(
      ISymbolicExpressionTree tree, 
      IClassificationProblemData problemData, 
      IEnumerable<int> rows, 
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      bool applyLinearScaling = true, 
      double lowerEstimationLimit = double.MinValue, 
      double upperEstimationLimit = double.MaxValue) {
      return Calculate(interpreter, tree, lowerEstimationLimit, upperEstimationLimit, problemData, rows, applyLinearScaling);
    }
  }
}
