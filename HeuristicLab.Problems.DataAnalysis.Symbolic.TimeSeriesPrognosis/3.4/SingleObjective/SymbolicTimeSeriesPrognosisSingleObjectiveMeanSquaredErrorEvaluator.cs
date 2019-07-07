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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis {
  [Item("Mean squared error Evaluator", "Calculates the mean squared error of a symbolic time-series prognosis solution.")]
  [StorableType("B1B4F084-C978-4A90-B678-0C00D4CAED59")]
  public class SymbolicTimeSeriesPrognosisSingleObjectiveMeanSquaredErrorEvaluator : SymbolicTimeSeriesPrognosisSingleObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicTimeSeriesPrognosisSingleObjectiveMeanSquaredErrorEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicTimeSeriesPrognosisSingleObjectiveMeanSquaredErrorEvaluator(SymbolicTimeSeriesPrognosisSingleObjectiveMeanSquaredErrorEvaluator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicTimeSeriesPrognosisSingleObjectiveMeanSquaredErrorEvaluator(this, cloner);
    }

    public SymbolicTimeSeriesPrognosisSingleObjectiveMeanSquaredErrorEvaluator() : base() { }

    public override bool Maximization { get { return false; } }

    public override IOperation InstrumentedApply() {
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      IEnumerable<int> rows = GenerateRowsToEvaluate();

      var interpreter = (ISymbolicTimeSeriesPrognosisExpressionTreeInterpreter)SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;

      double quality = Calculate(interpreter, solution,
        EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper,
        ProblemDataParameter.ActualValue, rows, EvaluationPartitionParameter.ActualValue,
        HorizonParameter.ActualValue.Value, ApplyLinearScalingParameter.ActualValue.Value);
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.InstrumentedApply();
    }

    public static double Calculate(ISymbolicTimeSeriesPrognosisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, ITimeSeriesPrognosisProblemData problemData, IEnumerable<int> rows, IntRange evaluationPartition, int horizon, bool applyLinearScaling) {
      var horizions = rows.Select(r => Math.Min(horizon, evaluationPartition.End - r));
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows.Zip(horizions, Enumerable.Range).SelectMany(r => r));
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows, horizions).SelectMany(x => x);
      OnlineCalculatorError errorState;

      double mse;
      if (applyLinearScaling && horizon == 1) { //perform normal evaluation and afterwards scale the solution and calculate the fitness value        
        var mseCalculator = new OnlineMeanSquaredErrorCalculator();
        CalculateWithScaling(targetValues, estimatedValues, lowerEstimationLimit, upperEstimationLimit, mseCalculator, problemData.Dataset.Rows * horizon);
        errorState = mseCalculator.ErrorState;
        mse = mseCalculator.MeanSquaredError;
      } else if (applyLinearScaling) { //first create model to perform linear scaling and afterwards calculate fitness for the scaled model
        var model = new SymbolicTimeSeriesPrognosisModel(problemData.TargetVariable, (ISymbolicExpressionTree)solution.Clone(), interpreter, lowerEstimationLimit, upperEstimationLimit);
        model.Scale(problemData);
        var scaledSolution = model.SymbolicExpressionTree;
        estimatedValues = interpreter.GetSymbolicExpressionTreeValues(scaledSolution, problemData.Dataset, rows, horizions).SelectMany(x => x);
        var boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
        mse = OnlineMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      } else {
        var boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
        mse = OnlineMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      }

      if (errorState != OnlineCalculatorError.None) return Double.NaN;
      else return mse;
    }


    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, ITimeSeriesPrognosisProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      HorizonParameter.ExecutionContext = context;
      EvaluationPartitionParameter.ExecutionContext = context;

      double mse = Calculate((ISymbolicTimeSeriesPrognosisExpressionTreeInterpreter)SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows, EvaluationPartitionParameter.ActualValue, HorizonParameter.ActualValue.Value, ApplyLinearScalingParameter.ActualValue.Value);

      HorizonParameter.ExecutionContext = null;
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      EvaluationPartitionParameter.ExecutionContext = null;

      return mse;
    }
  }
}
