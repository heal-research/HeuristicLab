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
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Pearson R² & Number of Variables Evaluator", "Calculates the Pearson R² and the number of used variables of a symbolic regression solution.")]
  [StorableType("DF68BE26-D76C-4CB7-BB38-CBBB4391FE86")]
  public class PearsonRSquaredNumberOfVariablesEvaluator : SymbolicRegressionMultiObjectiveEvaluator {
    [StorableConstructor]
    protected PearsonRSquaredNumberOfVariablesEvaluator(StorableConstructorFlag _) : base(_) { }
    protected PearsonRSquaredNumberOfVariablesEvaluator(PearsonRSquaredNumberOfVariablesEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PearsonRSquaredNumberOfVariablesEvaluator(this, cloner);
    }

    public PearsonRSquaredNumberOfVariablesEvaluator() : base() { }

    public override IEnumerable<bool> Maximization { get { return new bool[2] { true, false }; } } // maximize R² and minimize the number of variables

    public override IOperation InstrumentedApply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      var interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
      var estimationLimits = EstimationLimitsParameter.ActualValue;
      var applyLinearScaling = ApplyLinearScalingParameter.ActualValue.Value;

      if (UseConstantOptimization) {
        SymbolicRegressionConstantOptimizationEvaluator.OptimizeConstants(interpreter, solution, problemData, rows, applyLinearScaling, ConstantOptimizationIterations, updateVariableWeights: ConstantOptimizationUpdateVariableWeights, lowerEstimationLimit: estimationLimits.Lower, upperEstimationLimit: estimationLimits.Upper);
      }
      double[] qualities = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, rows, ApplyLinearScalingParameter.ActualValue.Value, DecimalPlaces);
      QualitiesParameter.ActualValue = new DoubleArray(qualities);
      return base.InstrumentedApply();
    }

    public static double[] Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, IRegressionProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling, int decimalPlaces) {
      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, problemData, rows, applyLinearScaling);
      if (decimalPlaces >= 0)
        r2 = Math.Round(r2, decimalPlaces);
      return new double[2] { r2, solution.IterateNodesPostfix().OfType<IVariableTreeNode>().Count() }; // count the number of variables
    }

    public override double[] Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;
      // DecimalPlaces parameter is a FixedValueParameter and doesn't need the context.

      double[] quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows, ApplyLinearScalingParameter.ActualValue.Value, DecimalPlaces);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return quality;
    }
  }
}
