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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Pearson R² & Average Similarity Evaluator", "Calculates the Pearson R² and the average similarity of a symbolic regression solution candidate.")]
  [StorableType("FE514989-E619-48B8-AC8E-9A2202708F65")]
  public class PearsonRSquaredAverageSimilarityEvaluator : SymbolicRegressionMultiObjectiveEvaluator {
    private const string StrictSimilarityParameterName = "StrictSimilarity";
    private const string AverageSimilarityParameterName = "AverageSimilarity";

    private readonly object locker = new object();

    private readonly SymbolicDataAnalysisExpressionTreeAverageSimilarityCalculator SimilarityCalculator = new SymbolicDataAnalysisExpressionTreeAverageSimilarityCalculator();

    public IFixedValueParameter<BoolValue> StrictSimilarityParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[StrictSimilarityParameterName]; }
    }

    public ILookupParameter<DoubleValue> AverageSimilarityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[AverageSimilarityParameterName]; }
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
      Parameters.Add(new LookupParameter<DoubleValue>(AverageSimilarityParameterName));
    }

    public override IEnumerable<bool> Maximization { get { return new bool[2] { true, false }; } } // maximize R² and minimize average similarity 

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

      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(interpreter, solution, estimationLimits.Lower, estimationLimits.Upper, problemData, rows, applyLinearScaling);

      if (DecimalPlaces >= 0)
        r2 = Math.Round(r2, DecimalPlaces);

      lock (locker) {
        if (AverageSimilarityParameter.ActualValue == null) {
          var context = new ExecutionContext(null, SimilarityCalculator, ExecutionContext.Scope.Parent);
          SimilarityCalculator.StrictSimilarity = StrictSimilarity;
          SimilarityCalculator.Execute(context, CancellationToken);
        }
      }
      var avgSimilarity = AverageSimilarityParameter.ActualValue.Value;

      QualitiesParameter.ActualValue = new DoubleArray(new[] { r2, avgSimilarity });
      return base.InstrumentedApply();
    }

    public override double[] Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      AverageSimilarityParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      var estimationLimits = EstimationLimitsParameter.ActualValue;
      var applyLinearScaling = ApplyLinearScalingParameter.ActualValue.Value;

      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, estimationLimits.Lower, estimationLimits.Upper, problemData, rows, applyLinearScaling);

      lock (locker) {
        if (AverageSimilarityParameter.ActualValue == null) {
          var ctx = new ExecutionContext(null, SimilarityCalculator, context.Scope.Parent);
          SimilarityCalculator.StrictSimilarity = StrictSimilarity;
          SimilarityCalculator.Execute(context, CancellationToken);
        }
      }
      var avgSimilarity = AverageSimilarityParameter.ActualValue.Value;

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return new[] { r2, avgSimilarity };
    }
  }
}
