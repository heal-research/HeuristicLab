#region License Information

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

using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("NMSE Evaluator with shape constraints (multi-objective)",
    "Calculates the NMSE and constraint violations for a symbolic regression model.")]
  [StorableType("8E9D76B7-ED9C-43E7-9898-01FBD3633880")]
  public class NMSEMultiObjectiveConstraintsEvaluator : SymbolicRegressionMultiObjectiveEvaluator, IMultiObjectiveConstraintsEvaluator {
    private const string NumConstraintsParameterName = "NumConstraints";
    private const string BoundsEstimatorParameterName = "BoundsEstimator";

    public IFixedValueParameter<IntValue> NumConstraintsParameter =>
      (IFixedValueParameter<IntValue>)Parameters[NumConstraintsParameterName];

    public IValueParameter<IBoundsEstimator> BoundsEstimatorParameter =>
      (IValueParameter<IBoundsEstimator>)Parameters[BoundsEstimatorParameterName];

    public int NumConstraints {
      get => NumConstraintsParameter.Value.Value;
      set {
        NumConstraintsParameter.Value.Value = value;
      }
    }

    public IBoundsEstimator BoundsEstimator {
      get => BoundsEstimatorParameter.Value;
      set => BoundsEstimatorParameter.Value = value;
    }

    public override IEnumerable<bool> Maximization => new bool[1 + NumConstraints]; // minimize all objectives

    #region Constructors

    public NMSEMultiObjectiveConstraintsEvaluator() {
      Parameters.Add(new FixedValueParameter<IntValue>(NumConstraintsParameterName, new IntValue(0)));
      Parameters.Add(new ValueParameter<IBoundsEstimator>(BoundsEstimatorParameterName, new IntervalArithBoundsEstimator()));
    }

    [StorableConstructor]
    protected NMSEMultiObjectiveConstraintsEvaluator(StorableConstructorFlag _) : base(_) { }

    protected NMSEMultiObjectiveConstraintsEvaluator(NMSEMultiObjectiveConstraintsEvaluator original, Cloner cloner) : base(original, cloner) { }

    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NMSEMultiObjectiveConstraintsEvaluator(this, cloner);
    }


    public override IOperation InstrumentedApply() {
      var rows = GenerateRowsToEvaluate();
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      var interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
      var estimationLimits = EstimationLimitsParameter.ActualValue;
      var applyLinearScaling = ApplyLinearScalingParameter.ActualValue.Value;

      if (UseParameterOptimization) {
        SymbolicRegressionParameterOptimizationEvaluator.OptimizeParameters(interpreter, tree, problemData, rows,
          false,
          ParameterOptimizationIterations,
          ParameterOptimizationUpdateVariableWeights,
          estimationLimits.Lower,
          estimationLimits.Upper);
      } else {
        if (applyLinearScaling) {
          var rootNode = new ProgramRootSymbol().CreateTreeNode();
          var startNode = new StartSymbol().CreateTreeNode();
          var offset = tree.Root.GetSubtree(0) //Start
                                .GetSubtree(0); //Offset
          var scaling = offset.GetSubtree(0);

          // Check if tree contains offset and scaling nodes
          if (!(offset.Symbol is Addition) || !(scaling.Symbol is Multiplication))
            throw new ArgumentException($"{ItemName} can only be used with LinearScalingGrammar.");


          var t = (ISymbolicExpressionTreeNode)scaling.GetSubtree(0).Clone();
          rootNode.AddSubtree(startNode);
          startNode.AddSubtree(t);
          var newTree = new SymbolicExpressionTree(rootNode);

          // calculate alpha and beta for scaling
          var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(newTree, problemData.Dataset, rows);

          var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
          OnlineLinearScalingParameterCalculator.Calculate(estimatedValues, targetValues, out var alpha, out var beta,
            out var errorState);
          if (errorState == OnlineCalculatorError.None) {
            // Set alpha and beta to the scaling nodes from linear scaling grammar
            var offsetParameter = offset.GetSubtree(1) as NumberTreeNode;
            offsetParameter.Value = alpha;
            var scalingParameter = scaling.GetSubtree(1) as NumberTreeNode;
            scalingParameter.Value = beta;
          }
        } // else alpha and beta are evolved
      }

      var qualities = Calculate(interpreter, tree, estimationLimits.Lower, estimationLimits.Upper, problemData,
        rows, BoundsEstimator, DecimalPlaces);
      QualitiesParameter.ActualValue = new DoubleArray(qualities);
      return base.InstrumentedApply();
    }

    public override double[] Evaluate(
      IExecutionContext context, ISymbolicExpressionTree tree,
      IRegressionProblemData problemData,
      IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      var quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree,
        EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper,
        problemData, rows, BoundsEstimator, DecimalPlaces);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return quality;
    }


    public static double[] Calculate(
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      ISymbolicExpressionTree solution, double lowerEstimationLimit,
      double upperEstimationLimit,
      IRegressionProblemData problemData, IEnumerable<int> rows, IBoundsEstimator estimator, int decimalPlaces) {
      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows);
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var constraints = Enumerable.Empty<ShapeConstraint>();
      if (problemData is ShapeConstrainedRegressionProblemData scProbData) {
        constraints = scProbData.ShapeConstraints.EnabledConstraints;
      }
      var intervalCollection = problemData.VariableRanges;

      double nmse;

      var boundedEstimatedValues = estimatedValues.LimitToRange(lowerEstimationLimit, upperEstimationLimit);
      nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out OnlineCalculatorError errorState);

      if (errorState != OnlineCalculatorError.None) nmse = 1.0;

      if (decimalPlaces >= 0)
        nmse = Math.Round(nmse, decimalPlaces);

      if (nmse > 1)
        nmse = 1.0;

      var objectives = new List<double> { nmse };
      var violations = IntervalUtil.GetConstraintViolations(constraints, estimator, intervalCollection, solution);
      foreach (var violation in violations) {
        if (double.IsNaN(violation) || double.IsInfinity(violation)) {
          objectives.Add(double.MaxValue);
        } else {
          objectives.Add(Math.Round(violation, decimalPlaces));
        }
      }

      return objectives.ToArray();
    }
  }
}