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
using HeuristicLab.NativeInterpreter;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Parameter Optimization Evaluator", "Optimizes model parameters using nonlinear least squares and returns the mean squared error.")]
  [StorableType("D6443358-1FA3-4F4C-89DB-DCC3D81050B2")]
  public class ParameterOptimizationEvaluator : SymbolicRegressionSingleObjectiveEvaluator {
    private const string ConstantOptimizationIterationsParameterName = "ConstantOptimizationIterations";
    private const string ConstantOptimizationImprovementParameterName = "ConstantOptimizationImprovement";
    private const string ConstantOptimizationProbabilityParameterName = "ConstantOptimizationProbability";
    private const string ConstantOptimizationRowsPercentageParameterName = "ConstantOptimizationRowsPercentage";
    private const string UpdateConstantsInTreeParameterName = "UpdateConstantsInSymbolicExpressionTree";
    private const string UpdateVariableWeightsParameterName = "Update Variable Weights";
    private const string FunctionEvaluationsResultParameterName = "Constants Optimization Function Evaluations";
    private const string GradientEvaluationsResultParameterName = "Constants Optimization Gradient Evaluations";
    private const string CountEvaluationsParameterName = "Count Function and Gradient Evaluations";

    public IFixedValueParameter<IntValue> ConstantOptimizationIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[ConstantOptimizationIterationsParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> ConstantOptimizationImprovementParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[ConstantOptimizationImprovementParameterName]; }
    }
    public IFixedValueParameter<PercentValue> ConstantOptimizationProbabilityParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[ConstantOptimizationProbabilityParameterName]; }
    }
    public IFixedValueParameter<PercentValue> ConstantOptimizationRowsPercentageParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[ConstantOptimizationRowsPercentageParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UpdateConstantsInTreeParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateConstantsInTreeParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UpdateVariableWeightsParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateVariableWeightsParameterName]; }
    }

    public IResultParameter<IntValue> FunctionEvaluationsResultParameter {
      get { return (IResultParameter<IntValue>)Parameters[FunctionEvaluationsResultParameterName]; }
    }
    public IResultParameter<IntValue> GradientEvaluationsResultParameter {
      get { return (IResultParameter<IntValue>)Parameters[GradientEvaluationsResultParameterName]; }
    }
    public IFixedValueParameter<BoolValue> CountEvaluationsParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[CountEvaluationsParameterName]; }
    }

    public IntValue ConstantOptimizationIterations {
      get { return ConstantOptimizationIterationsParameter.Value; }
    }
    public DoubleValue ConstantOptimizationImprovement {
      get { return ConstantOptimizationImprovementParameter.Value; }
    }
    public PercentValue ConstantOptimizationProbability {
      get { return ConstantOptimizationProbabilityParameter.Value; }
    }
    public PercentValue ConstantOptimizationRowsPercentage {
      get { return ConstantOptimizationRowsPercentageParameter.Value; }
    }
    public bool UpdateConstantsInTree {
      get { return UpdateConstantsInTreeParameter.Value.Value; }
      set { UpdateConstantsInTreeParameter.Value.Value = value; }
    }

    public bool UpdateVariableWeights {
      get { return UpdateVariableWeightsParameter.Value.Value; }
      set { UpdateVariableWeightsParameter.Value.Value = value; }
    }

    public bool CountEvaluations {
      get { return CountEvaluationsParameter.Value.Value; }
      set { CountEvaluationsParameter.Value.Value = value; }
    }

    public override bool Maximization {
      get { return false; }
    }

    [StorableConstructor]
    protected ParameterOptimizationEvaluator(StorableConstructorFlag _) : base(_) { }
    protected ParameterOptimizationEvaluator(ParameterOptimizationEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public ParameterOptimizationEvaluator()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(ConstantOptimizationIterationsParameterName, "Determines how many iterations should be calculated while optimizing the constant of a symbolic expression tree (0 indicates other or default stopping criterion).", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(ConstantOptimizationImprovementParameterName, "Determines the relative improvement which must be achieved in the constant optimization to continue with it (0 indicates other or default stopping criterion).", new DoubleValue(0)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<PercentValue>(ConstantOptimizationProbabilityParameterName, "Determines the probability that the constants are optimized", new PercentValue(1)));
      Parameters.Add(new FixedValueParameter<PercentValue>(ConstantOptimizationRowsPercentageParameterName, "Determines the percentage of the rows which should be used for constant optimization", new PercentValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateConstantsInTreeParameterName, "Determines if the constants in the tree should be overwritten by the optimized constants.", new BoolValue(true)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be  optimized.", new BoolValue(true)) { Hidden = true });

      Parameters.Add(new FixedValueParameter<BoolValue>(CountEvaluationsParameterName, "Determines if function and gradient evaluation should be counted.", new BoolValue(false)));
      Parameters.Add(new ResultParameter<IntValue>(FunctionEvaluationsResultParameterName, "The number of function evaluations performed by the constants optimization evaluator", "Results", new IntValue()));
      Parameters.Add(new ResultParameter<IntValue>(GradientEvaluationsResultParameterName, "The number of gradient evaluations performed by the constants optimization evaluator", "Results", new IntValue()));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ParameterOptimizationEvaluator(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(UpdateConstantsInTreeParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateConstantsInTreeParameterName, "Determines if the constants in the tree should be overwritten by the optimized constants.", new BoolValue(true)));
      if (!Parameters.ContainsKey(UpdateVariableWeightsParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be  optimized.", new BoolValue(true)));

      if (!Parameters.ContainsKey(CountEvaluationsParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(CountEvaluationsParameterName, "Determines if function and gradient evaluation should be counted.", new BoolValue(false)));

      if (!Parameters.ContainsKey(FunctionEvaluationsResultParameterName))
        Parameters.Add(new ResultParameter<IntValue>(FunctionEvaluationsResultParameterName, "The number of function evaluations performed by the constants optimization evaluator", "Results", new IntValue()));
      if (!Parameters.ContainsKey(GradientEvaluationsResultParameterName))
        Parameters.Add(new ResultParameter<IntValue>(GradientEvaluationsResultParameterName, "The number of gradient evaluations performed by the constants optimization evaluator", "Results", new IntValue()));
    }

    private static readonly object locker = new object();
    public override IOperation InstrumentedApply() {
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      double quality;
      if (RandomParameter.ActualValue.NextDouble() < ConstantOptimizationProbability.Value) {
        IEnumerable<int> constantOptimizationRows = GenerateRowsToEvaluate(ConstantOptimizationRowsPercentage.Value);
        var counter = new EvaluationsCounter();
        quality = OptimizeConstants(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, ProblemDataParameter.ActualValue,
           constantOptimizationRows, Enumerable.Empty<double>(), ApplyLinearScalingParameter.ActualValue.Value, ConstantOptimizationIterations.Value, updateVariableWeights: UpdateVariableWeights, lowerEstimationLimit: EstimationLimitsParameter.ActualValue.Lower, upperEstimationLimit: EstimationLimitsParameter.ActualValue.Upper, updateConstantsInTree: UpdateConstantsInTree, counter: counter);

        if (ConstantOptimizationRowsPercentage.Value != RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value) {
          var evaluationRows = GenerateRowsToEvaluate();
          //quality = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, evaluationRows, ApplyLinearScalingParameter.ActualValue.Value);
          quality = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(solution, ProblemDataParameter.ActualValue, evaluationRows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, ApplyLinearScalingParameter.ActualValue.Value, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
        }

        if (CountEvaluations) {
          lock (locker) {
            FunctionEvaluationsResultParameter.ActualValue.Value += counter.FunctionEvaluations;
            GradientEvaluationsResultParameter.ActualValue.Value += counter.GradientEvaluations;
          }
        }

      } else {
        var evaluationRows = GenerateRowsToEvaluate();
        quality = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(solution, ProblemDataParameter.ActualValue, evaluationRows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, ApplyLinearScalingParameter.ActualValue.Value, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      }
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.InstrumentedApply();
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;
      FunctionEvaluationsResultParameter.ExecutionContext = context;
      GradientEvaluationsResultParameter.ExecutionContext = context;

      // Mean Squared Error evaluator is used on purpose instead of the const-opt evaluator, 
      // because Evaluate() is used to get the quality of evolved models on 
      // different partitions of the dataset (e.g., best validation model)
      var mse = Evaluate(tree, problemData, rows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, ApplyLinearScalingParameter.ActualValue.Value, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;
      FunctionEvaluationsResultParameter.ExecutionContext = null;
      GradientEvaluationsResultParameter.ExecutionContext = null;

      return mse;
    }

    public class EvaluationsCounter {
      public int FunctionEvaluations = 0;
      public int GradientEvaluations = 0;
    }

    public static double OptimizeConstants(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows, IEnumerable<double> weights, bool applyLinearScaling,
      int maxIterations, bool updateVariableWeights = true,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue,
      bool updateConstantsInTree = true, Action<double[], double, object> iterationCallback = null, EvaluationsCounter counter = null) {

      var nodesToOptimize = new HashSet<ISymbolicExpressionTreeNode>();
      var originalNodeValues = new Dictionary<ISymbolicExpressionTreeNode, double>();

      foreach (var node in tree.IterateNodesPrefix().OfType<SymbolicExpressionTreeTerminalNode>()) {
        if (node is VariableTreeNode && !updateVariableWeights) {
          continue;
        }
        if (node is ConstantTreeNode) {
          // do not optimize constants
          continue;
        }
        if (node is NumberTreeNode && node.Parent.Symbol is Power && node.Parent.GetSubtree(1) == node) {
          // do not optimize exponents
          continue;
        }
        nodesToOptimize.Add(node);
        if (node is NumberTreeNode number) {
          originalNodeValues[node] = number.Value;
        } else if (node is VariableTreeNode variable) {
          originalNodeValues[node] = variable.Weight;
        }
      }

      var options = new SolverOptions {
        Iterations = maxIterations
      };
      var summary = new SolverSummary();
      var optimizedNodeValues = ParameterOptimizer.OptimizeTree(tree, problemData.Dataset, problemData.TrainingIndices, problemData.TargetVariable, weights, nodesToOptimize, options, ref summary);

      counter.FunctionEvaluations += summary.ResidualEvaluations;
      counter.GradientEvaluations += summary.JacobianEvaluations;

      if (summary.Success != 0 && updateConstantsInTree) {
        UpdateNodeValues(optimizedNodeValues);
      }
      var mse = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(tree, problemData, rows, interpreter ,applyLinearScaling, lowerEstimationLimit, upperEstimationLimit);
      return mse;
    }

    private static void UpdateNodeValues(IDictionary<ISymbolicExpressionTreeNode, double> values) {
      foreach (var item in values) {
        var node = item.Key;
        if (node is NumberTreeNode number) {
          number.Value = item.Value;
        } else if (node is VariableTreeNode variable) {
          variable.Weight = item.Value;
        }
      }
    }

    public static bool CanOptimizeConstants(ISymbolicExpressionTree tree) {
      return TreeToAutoDiffTermConverter.IsCompatible(tree);
    }

    public override double Evaluate(ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, bool applyLinearScaling = true, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue) {
      return SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(tree, problemData, rows, interpreter, applyLinearScaling, lowerEstimationLimit, upperEstimationLimit);
    }
  }
}
