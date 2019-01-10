#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Pearson R² & Average Similarity Evaluator", "Calculates the Pearson R² and the average similarity of a symbolic regression solution candidate.")]
  [StorableType("FE514989-E619-48B8-AC8E-9A2202708F65")]
  public class PearsonRSquaredAverageSimilarityEvaluator : SymbolicRegressionMultiObjectiveEvaluator {
    private const string StrictSimilarityParameterName = "StrictSimilarity";

    private readonly object locker = new object();

    public IFixedValueParameter<BoolValue> StrictSimilarityParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[StrictSimilarityParameterName]; }
    }

    public bool StrictSimilarity {
      get { return StrictSimilarityParameter.Value.Value; }
    }

    [StorableConstructor]
    protected PearsonRSquaredAverageSimilarityEvaluator(StorableConstructorFlag _) : base(_) { }
    protected PearsonRSquaredAverageSimilarityEvaluator(PearsonRSquaredAverageSimilarityEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PearsonRSquaredAverageSimilarityEvaluator(this, cloner);
    }

    public PearsonRSquaredAverageSimilarityEvaluator() : base() {
      Parameters.Add(new FixedValueParameter<BoolValue>(StrictSimilarityParameterName, "Use strict similarity calculation.", new BoolValue(false)));
    }

    public override IEnumerable<bool> Maximization { get { return new bool[2] { true, false }; } } // maximize R² and minimize model complexity 

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
      double[] qualities = Calculate(interpreter, solution, estimationLimits.Lower, estimationLimits.Upper, problemData, rows, applyLinearScaling, DecimalPlaces);
      QualitiesParameter.ActualValue = new DoubleArray(qualities);
      return base.InstrumentedApply();
    }

    public double[] Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, IRegressionProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling, int decimalPlaces) {
      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, problemData, rows, applyLinearScaling);
      if (decimalPlaces >= 0)
        r2 = Math.Round(r2, decimalPlaces);

      var variables = ExecutionContext.Scope.Variables;
      if (!variables.ContainsKey("AverageSimilarity")) {
        lock (locker) {
          CalculateAverageSimilarities(ExecutionContext.Scope.Parent.SubScopes.Where(x => x.Variables.ContainsKey("SymbolicExpressionTree")).ToArray(), StrictSimilarity);

        }
      }

      double avgSim = ((DoubleValue)variables["AverageSimilarity"].Value).Value;
      return new double[2] { r2, avgSim };
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

    private readonly Stopwatch sw = new Stopwatch();
    public void CalculateAverageSimilarities(IScope[] treeScopes, bool strict) {
      var trees = treeScopes.Select(x => (ISymbolicExpressionTree)x.Variables["SymbolicExpressionTree"].Value).ToArray();
      var similarityMatrix = SymbolicExpressionTreeHash.ComputeSimilarityMatrix(trees, simplify: false, strict: strict);

      for (int i = 0; i < treeScopes.Length; ++i) {
        var scope = treeScopes[i];
        var avgSimilarity = 0d;
        for (int j = 0; j < trees.Length; ++j) {
          if (i == j) continue;
          avgSimilarity += similarityMatrix[i, j];
        }
        avgSimilarity /= trees.Length - 1;

        if (scope.Variables.ContainsKey("AverageSimilarity")) {
          ((DoubleValue)scope.Variables["AverageSimilarity"].Value).Value = avgSimilarity;
        } else {
          scope.Variables.Add(new Core.Variable("AverageSimilarity", new DoubleValue(avgSimilarity)));
        }
      }
    }
  }
}
