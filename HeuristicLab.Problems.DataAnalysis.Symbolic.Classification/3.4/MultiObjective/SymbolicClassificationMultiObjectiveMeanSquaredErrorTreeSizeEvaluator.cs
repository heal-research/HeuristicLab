#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [Item("Mean squared error & Tree size Evaluator", "Calculates the mean squared error and the tree size of a symbolic classification solution.")]
  [StorableType("1D1693A0-6479-45D1-990F-125FFCEC430E")]
  public class SymbolicClassificationMultiObjectiveMeanSquaredErrorTreeSizeEvaluator : SymbolicClassificationMultiObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicClassificationMultiObjectiveMeanSquaredErrorTreeSizeEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicClassificationMultiObjectiveMeanSquaredErrorTreeSizeEvaluator(SymbolicClassificationMultiObjectiveMeanSquaredErrorTreeSizeEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationMultiObjectiveMeanSquaredErrorTreeSizeEvaluator(this, cloner);
    }

    public SymbolicClassificationMultiObjectiveMeanSquaredErrorTreeSizeEvaluator() : base() { }

    public override IEnumerable<bool> Maximization { get { return new bool[2] { false, false }; } }

    public override IOperation InstrumentedApply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      double[] qualities = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, rows);
      QualitiesParameter.ActualValue = new DoubleArray(qualities);
      return base.InstrumentedApply();
    }

    public static double[] Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, IClassificationProblemData problemData, IEnumerable<int> rows) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows);
      IEnumerable<double> originalValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      IEnumerable<double> boundedEstimationValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
      OnlineCalculatorError errorState;
      double mse = OnlineMeanSquaredErrorCalculator.Calculate(originalValues, boundedEstimationValues, out errorState);
      if (errorState != OnlineCalculatorError.None) mse = double.NaN;
      return new double[2] { mse, solution.Length };
    }

    public override double[] Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IClassificationProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      double[] quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return quality;
    }
  }
}
