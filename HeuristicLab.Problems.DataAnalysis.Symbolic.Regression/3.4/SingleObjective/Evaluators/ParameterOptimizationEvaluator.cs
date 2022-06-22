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
  [Item("Parameter Optimization Evaluator (native)", "Optimizes model parameters using nonlinear least squares and returns the mean squared error.")]
  [StorableType("D6443358-1FA3-4F4C-89DB-DCC3D81050B2")]
  public class ParameterOptimizationEvaluator : SymbolicRegressionSingleObjectiveEvaluator {
    private const string IterationsParameterName = "Iterations";
    private const string ProbabilityParameterName = "Probability";
    private const string RowsPercentageParameterName = "Rows percentage";
    private const string UpdateParametersParameterName = "Update parameters";
    private const string UpdateVariableWeightsParameterName = "Update variable weights";
    private const string CountEvaluationsParameterName = "Count function and gradient evaluations";
    private const string FunctionEvaluationsResultParameterName = "Function evaluations";
    private const string GradientEvaluationsResultParameterName = "Gradient evaluations";

    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    public IFixedValueParameter<PercentValue> ProbabilityParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[ProbabilityParameterName]; }
    }
    public IFixedValueParameter<PercentValue> RowsPercentageParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[RowsPercentageParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UpdateParametersParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateParametersParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UpdateVariableWeightsParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateVariableWeightsParameterName]; }
    }
    public IFixedValueParameter<BoolValue> CountEvaluationsParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[CountEvaluationsParameterName]; }
    }
    public IResultParameter<IntValue> FunctionEvaluationsResultParameter {
      get { return (IResultParameter<IntValue>)Parameters[FunctionEvaluationsResultParameterName]; }
    }
    public IResultParameter<IntValue> GradientEvaluationsResultParameter {
      get { return (IResultParameter<IntValue>)Parameters[GradientEvaluationsResultParameterName]; }
    }

    public int Iterations {
      get { return IterationsParameter.Value.Value; }
      set { IterationsParameter.Value.Value = value; }
    }
    public double Probability {
      get { return ProbabilityParameter.Value.Value; }
      set { ProbabilityParameter.Value.Value = value; }
    }
    public double RowsPercentage {
      get { return RowsPercentageParameter.Value.Value; }
      set { RowsPercentageParameter.Value.Value = value; }
    }
    public bool UpdateParameters {
      get { return UpdateParametersParameter.Value.Value; }
      set { UpdateParametersParameter.Value.Value = value; }
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
      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName, "Determines how many iterations should be calculated while optimizing the parameters of a symbolic expression tree (0 indicates other or default stopping criterion).", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<PercentValue>(ProbabilityParameterName, "Determines the probability that the parameters are optimized", new PercentValue(1)));
      Parameters.Add(new FixedValueParameter<PercentValue>(RowsPercentageParameterName, "Determines the percentage of the rows which should be used for parameter optimization", new PercentValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateParametersParameterName, "Determines if the parameters in the tree should be overwritten by the optimized parameters.", new BoolValue(true)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be  optimized.", new BoolValue(true)) { Hidden = true });

      Parameters.Add(new FixedValueParameter<BoolValue>(CountEvaluationsParameterName, "Determines if function and gradient evaluation should be counted.", new BoolValue(false)));
      Parameters.Add(new ResultParameter<IntValue>(FunctionEvaluationsResultParameterName, "The number of function evaluations performed by the evaluator", "Results", new IntValue()));
      Parameters.Add(new ResultParameter<IntValue>(GradientEvaluationsResultParameterName, "The number of gradient evaluations performed by the evaluator", "Results", new IntValue()));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ParameterOptimizationEvaluator(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(UpdateParametersParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateParametersParameterName, "Determines if the parameters in the tree should be overwritten by the optimized parameters.", new BoolValue(true)));
      if (!Parameters.ContainsKey(UpdateVariableWeightsParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be optimized.", new BoolValue(true)));

      if (!Parameters.ContainsKey(CountEvaluationsParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(CountEvaluationsParameterName, "Determines if function and gradient evaluation should be counted.", new BoolValue(false)));

      if (!Parameters.ContainsKey(FunctionEvaluationsResultParameterName))
        Parameters.Add(new ResultParameter<IntValue>(FunctionEvaluationsResultParameterName, "The number of function evaluations performed by the evaluator", "Results", new IntValue()));
      if (!Parameters.ContainsKey(GradientEvaluationsResultParameterName))
        Parameters.Add(new ResultParameter<IntValue>(GradientEvaluationsResultParameterName, "The number of gradient evaluations performed by the evaluator", "Results", new IntValue()));
    }

    private static readonly object locker = new object();
    public override IOperation InstrumentedApply() {
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      double quality;
      if (RandomParameter.ActualValue.NextDouble() < Probability) {
        IEnumerable<int> constantOptimizationRows = GenerateRowsToEvaluate(RowsPercentage);
        var counter = new EvaluationsCounter();
        quality = OptimizeParameters(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, ProblemDataParameter.ActualValue,
           constantOptimizationRows, Enumerable.Empty<double>(), ApplyLinearScalingParameter.ActualValue.Value, Iterations, updateVariableWeights: UpdateVariableWeights, lowerEstimationLimit: EstimationLimitsParameter.ActualValue.Lower, upperEstimationLimit: EstimationLimitsParameter.ActualValue.Upper, updateConstantsInTree: UpdateParameters, counter: counter);

        if (RowsPercentage != RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value) {
          var evaluationRows = GenerateRowsToEvaluate();
          quality = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(tree, ProblemDataParameter.ActualValue, evaluationRows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, ApplyLinearScalingParameter.ActualValue.Value, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
        }

        if (CountEvaluations) {
          lock (locker) {
            FunctionEvaluationsResultParameter.ActualValue.Value += counter.FunctionEvaluations;
            GradientEvaluationsResultParameter.ActualValue.Value += counter.GradientEvaluations;
          }
        }

      } else {
        var evaluationRows = GenerateRowsToEvaluate();
        quality = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(tree, ProblemDataParameter.ActualValue, evaluationRows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, ApplyLinearScalingParameter.ActualValue.Value, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
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

      // Mean Squared Error evaluator is used on purpose instead of the param-opt evaluator, 
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

    public static double OptimizeParameters(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows, IEnumerable<double> weights, bool applyLinearScaling,
      int maxIterations, bool updateVariableWeights = true,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue,
      bool updateConstantsInTree = true, EvaluationsCounter counter = null) {

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
      return SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(tree, problemData, rows, interpreter, applyLinearScaling, lowerEstimationLimit, upperEstimationLimit);
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
