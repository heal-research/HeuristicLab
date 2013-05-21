#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [Item("Pearson R² & Tree size Evaluator", "Calculates the Pearson R² and the tree size of a symbolic classification solution.")]
  [StorableClass]
  public class SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator : SymbolicClassificationMultiObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator(SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator(this, cloner);
    }

    public SymbolicClassificationMultiObjectivePearsonRSquaredTreeSizeEvaluator() : base() { }

    public override IEnumerable<bool> Maximization { get { return new bool[2] { true, false }; } }

    public override IOperation Apply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      double[] qualities = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, rows, ApplyLinearScalingParameter.ActualValue.Value);
      QualitiesParameter.ActualValue = new DoubleArray(qualities);
      return base.Apply();
    }

    public static double[] Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, IClassificationProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows);
      IEnumerable<double> targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      OnlineCalculatorError errorState;

      double r2;
      if (applyLinearScaling) {
        var r2Calculator = new OnlinePearsonsRSquaredCalculator();
        CalculateWithScaling(targetValues, estimatedValues, lowerEstimationLimit, upperEstimationLimit, r2Calculator, problemData.Dataset.Rows);
        errorState = r2Calculator.ErrorState;
        r2 = r2Calculator.RSquared;
      } else {
        IEnumerable<double> boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
        r2 = OnlinePearsonsRSquaredCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      }

      if (errorState != OnlineCalculatorError.None) r2 = double.NaN;
      return new double[2] { r2, solution.Length };

    }

    public override double[] Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IClassificationProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;

      double[] quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows, ApplyLinearScalingParameter.ActualValue.Value);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;

      return quality;
    }
  }
}
