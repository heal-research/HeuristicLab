#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis {
  [Item("Pearson R² Evaluator", "Calculates the square of the pearson correlation coefficient (also known as coefficient of determination) of a symbolic time-series prognosis solution.")]
  [StorableClass]
  public class SymbolicTimeSeriesPrognosisSingleObjectivePearsonRSquaredEvaluator : SymbolicTimeSeriesPrognosisSingleObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicTimeSeriesPrognosisSingleObjectivePearsonRSquaredEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicTimeSeriesPrognosisSingleObjectivePearsonRSquaredEvaluator(SymbolicTimeSeriesPrognosisSingleObjectivePearsonRSquaredEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicTimeSeriesPrognosisSingleObjectivePearsonRSquaredEvaluator(this, cloner);
    }

    public SymbolicTimeSeriesPrognosisSingleObjectivePearsonRSquaredEvaluator() : base() { }

    public override bool Maximization { get { return true; } }

    public override IOperation Apply() {
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      IEnumerable<int> rows = GenerateRowsToEvaluate();

      double quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution,
        EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper,
        ProblemDataParameter.ActualValue,
        rows, HorizonParameter.ActualValue.Value);
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.Apply();
    }

    public static double Calculate(ISymbolicTimeSeriesPrognosisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, ITimeSeriesPrognosisProblemData problemData, IEnumerable<int> rows, int horizon) {
      var allPredictedContinuations =
        interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, problemData.TargetVariables.ToArray(),
                                                    rows, horizon).ToArray();

      var meanCalculator = new OnlineMeanAndVarianceCalculator();
      int i = 0;
      foreach (var targetVariable in problemData.TargetVariables) {
        var actualContinuations = from r in rows
                                  select problemData.Dataset.GetDoubleValues(targetVariable, Enumerable.Range(r, horizon));
        var startValues = problemData.Dataset.GetDoubleValues(targetVariable, rows.Select(r => r - 1));
        OnlineCalculatorError errorState;
        meanCalculator.Add(OnlineTheilsUStatisticCalculator.Calculate(
          startValues,
          allPredictedContinuations.Select(v => v.ElementAt(i)),
          actualContinuations, out errorState));
        if (errorState != OnlineCalculatorError.None) return double.NaN;
        i++;
      }
      return meanCalculator.Mean;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, ITimeSeriesPrognosisProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      HorizonParameter.ExecutionContext = context;

      double r2 = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows, HorizonParameter.ActualValue.Value);

      HorizonParameter.ExecutionContext = null;
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;

      return r2;
    }
  }
}
