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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Pearson R² & Nested Tree size Evaluator", "Calculates the Pearson R² and the nested tree size of a symbolic regression solution.")]
  [StorableClass]
  public class PearsonRSquaredNestedTreeSizeEvaluator : SymbolicRegressionMultiObjectiveEvaluator {
    [StorableConstructor]
    protected PearsonRSquaredNestedTreeSizeEvaluator(bool deserializing) : base(deserializing) { }
    protected PearsonRSquaredNestedTreeSizeEvaluator(PearsonRSquaredNestedTreeSizeEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PearsonRSquaredNestedTreeSizeEvaluator(this, cloner);
    }

    public PearsonRSquaredNestedTreeSizeEvaluator() : base() { }

    public override IEnumerable<bool> Maximization { get { return new bool[2] { true, false }; } } // maximize R² & minimize nested tree size

    public override IOperation InstrumentedApply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      var interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
      var estimationLimits = EstimationLimitsParameter.ActualValue;
      var applyLinearScaling = ApplyLinearScalingParameter.ActualValue.Value;

      if (UseConstantOptimization) {
        SymbolicRegressionConstantOptimizationEvaluator.OptimizeConstants(interpreter, solution, problemData, rows, applyLinearScaling, ConstantOptimizationIterations, updateVariableWeights: ConstantOptimizationUpdateVariableWeights,lowerEstimationLimit: estimationLimits.Lower, upperEstimationLimit: estimationLimits.Upper);
      }

      double[] qualities = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, rows, ApplyLinearScalingParameter.ActualValue.Value, DecimalPlaces);
      QualitiesParameter.ActualValue = new DoubleArray(qualities);
      return base.InstrumentedApply();
    }

    public static double[] Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, IRegressionProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling, int decimalPlaces) {
      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, problemData, rows, applyLinearScaling);
      if (decimalPlaces >= 0)
        r2 = Math.Round(r2, decimalPlaces);
      return new double[2] { r2, solution.IterateNodesPostfix().Sum(n => n.GetLength()) }; // sum of the length of the whole sub-tree for each node 
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
