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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Obsolete("Use ParameterOptimizationEvaluator instead")]
  [Item("Parameter Optimization Evaluator (managed)", "Calculates Pearson R² of a symbolic regression solution and optimizes the parameters used.")]
  [StorableType("24B68851-036D-4446-BD6F-3823E9028FF4")]
  public class SymbolicRegressionParameterOptimizationEvaluator : SymbolicRegressionSingleObjectiveEvaluator {
    private const string ParameterOptimizationIterationsParameterName = "ParameterOptimizationIterations";
    private const string ParameterOptimizationImprovementParameterName = "ParameterOptimizationImprovement";
    private const string ParameterOptimizationProbabilityParameterName = "ParameterOptimizationProbability";
    private const string ParameterOptimizationRowsPercentageParameterName = "ParameterOptimizationRowsPercentage";
    private const string UpdateParametersInTreeParameterName = "UpdateParametersInSymbolicExpressionTree";
    private const string UpdateVariableWeightsParameterName = "Update Variable Weights";

    private const string FunctionEvaluationsResultParameterName = "Parameters Optimization Function Evaluations";
    private const string GradientEvaluationsResultParameterName = "Parameters Optimization Gradient Evaluations";
    private const string CountEvaluationsParameterName = "Count Function and Gradient Evaluations";

    public IFixedValueParameter<IntValue> ParameterOptimizationIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[ParameterOptimizationIterationsParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> ParameterOptimizationImprovementParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[ParameterOptimizationImprovementParameterName]; }
    }
    public IFixedValueParameter<PercentValue> ParameterOptimizationProbabilityParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[ParameterOptimizationProbabilityParameterName]; }
    }
    public IFixedValueParameter<PercentValue> ParameterOptimizationRowsPercentageParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[ParameterOptimizationRowsPercentageParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UpdateParametersInTreeParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateParametersInTreeParameterName]; }
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


    public IntValue ParameterOptimizationIterations {
      get { return ParameterOptimizationIterationsParameter.Value; }
    }
    public DoubleValue ParameterOptimizationImprovement {
      get { return ParameterOptimizationImprovementParameter.Value; }
    }
    public PercentValue ParameterOptimizationProbability {
      get { return ParameterOptimizationProbabilityParameter.Value; }
    }
    public PercentValue ParameterOptimizationRowsPercentage {
      get { return ParameterOptimizationRowsPercentageParameter.Value; }
    }
    public bool UpdateParametersInTree {
      get { return UpdateParametersInTreeParameter.Value.Value; }
      set { UpdateParametersInTreeParameter.Value.Value = value; }
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
      get { return true; }
    }

    [StorableConstructor]
    protected SymbolicRegressionParameterOptimizationEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicRegressionParameterOptimizationEvaluator(SymbolicRegressionParameterOptimizationEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicRegressionParameterOptimizationEvaluator()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(ParameterOptimizationIterationsParameterName, "Determines how many iterations should be calculated while optimizing the parameter of a symbolic expression tree (0 indicates other or default stopping criterion).", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(ParameterOptimizationImprovementParameterName, "Determines the relative improvement which must be achieved in the parameter optimization to continue with it (0 indicates other or default stopping criterion).", new DoubleValue(0)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationProbabilityParameterName, "Determines the probability that the parameters are optimized", new PercentValue(1)));
      Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationRowsPercentageParameterName, "Determines the percentage of the rows which should be used for parameter optimization", new PercentValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateParametersInTreeParameterName, "Determines if the parameters in the tree should be overwritten by the optimized parameters.", new BoolValue(true)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be  optimized.", new BoolValue(true)) { Hidden = true });

      Parameters.Add(new FixedValueParameter<BoolValue>(CountEvaluationsParameterName, "Determines if function and gradient evaluation should be counted.", new BoolValue(false)));
      Parameters.Add(new ResultParameter<IntValue>(FunctionEvaluationsResultParameterName, "The number of function evaluations performed by the parameters optimization evaluator", "Results", new IntValue()));
      Parameters.Add(new ResultParameter<IntValue>(GradientEvaluationsResultParameterName, "The number of gradient evaluations performed by the parameters optimization evaluator", "Results", new IntValue()));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionParameterOptimizationEvaluator(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(UpdateParametersInTreeParameterName)) {
        if (Parameters.ContainsKey("UpdateConstantsInSymbolicExpressionTree")) {
          Parameters.Add(new FixedValueParameter<BoolValue>(UpdateParametersInTreeParameterName, "Determines if the parameters in the tree should be overwritten by the optimized parameters.", (BoolValue)Parameters["UpdateConstantsInSymbolicExpressionTree"].ActualValue));
          Parameters.Remove("UpdateConstantsInSymbolicExpressionTree");
        } else {
          Parameters.Add(new FixedValueParameter<BoolValue>(UpdateParametersInTreeParameterName, "Determines if the parameters in the tree should be overwritten by the optimized parameters.", new BoolValue(true)));
        }
      }

      if (!Parameters.ContainsKey(UpdateVariableWeightsParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be  optimized.", new BoolValue(true)));

      if (!Parameters.ContainsKey(CountEvaluationsParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(CountEvaluationsParameterName, "Determines if function and gradient evaluation should be counted.", new BoolValue(false)));

      if (!Parameters.ContainsKey(FunctionEvaluationsResultParameterName)) {
        if (Parameters.ContainsKey("Constants Optimization Function Evaluations")) {
          Parameters.Remove("Constants Optimization Function Evaluations");
        }
        Parameters.Add(new ResultParameter<IntValue>(FunctionEvaluationsResultParameterName, "The number of function evaluations performed by the parameters optimization evaluator", "Results", new IntValue()));
      }

      if (!Parameters.ContainsKey(GradientEvaluationsResultParameterName)) {
        if (Parameters.ContainsKey("Constants Optimization Gradient Evaluations")) {
          Parameters.Remove("Constants Optimization Gradient Evaluations");
        }
        Parameters.Add(new ResultParameter<IntValue>(GradientEvaluationsResultParameterName, "The number of gradient evaluations performed by the parameters optimization evaluator", "Results", new IntValue()));
      }

      if (!Parameters.ContainsKey(ParameterOptimizationIterationsParameterName)) {
        if (Parameters.ContainsKey("ConstantOptimizationIterations")) {
          Parameters.Add(new FixedValueParameter<IntValue>(ParameterOptimizationIterationsParameterName, "Determines how many iterations should be calculated while optimizing the parameter of a symbolic expression tree (0 indicates other or default stopping criterion).", (IntValue)Parameters["ConstantOptimizationIterations"].ActualValue));
          Parameters.Remove("ConstantOptimizationIterations");
        } else {
          Parameters.Add(new FixedValueParameter<IntValue>(ParameterOptimizationIterationsParameterName, "Determines how many iterations should be calculated while optimizing the parameter of a symbolic expression tree (0 indicates other or default stopping criterion).", new IntValue(10)));
        }
      }

      if (!Parameters.ContainsKey(ParameterOptimizationImprovementParameterName)) {
        if (Parameters.ContainsKey("CosntantOptimizationImprovement")) {
          Parameters.Add(new FixedValueParameter<DoubleValue>(ParameterOptimizationImprovementParameterName, "Determines the relative improvement which must be achieved in the parameter optimization to continue with it (0 indicates other or default stopping criterion).",
            (DoubleValue)Parameters["CosntantOptimizationImprovement"].ActualValue) { Hidden = true });
          Parameters.Remove("CosntantOptimizationImprovement");
        } else {
          Parameters.Add(new FixedValueParameter<DoubleValue>(ParameterOptimizationImprovementParameterName, "Determines the relative improvement which must be achieved in the parameter optimization to continue with it (0 indicates other or default stopping criterion).", new DoubleValue(0)) { Hidden = true });
        }
      }

      if (!Parameters.ContainsKey(ParameterOptimizationProbabilityParameterName)) {
        if (Parameters.ContainsKey("ConstantOptimizationProbability")) {
          Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationProbabilityParameterName, "Determines the probability that the parameters are optimized",
            (PercentValue)Parameters["ConstantOptimizationProbability"].ActualValue));
          Parameters.Remove("ConstantOptimizationProbability");
        } else {
          Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationProbabilityParameterName, "Determines the probability that the parameters are optimized", new PercentValue(1)));
        }
      }

      if (!Parameters.ContainsKey(ParameterOptimizationRowsPercentageParameterName)) {
        if (Parameters.ContainsKey("ConstantOptimizationRowsPercentage")) {
          Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationRowsPercentageParameterName, "Determines the percentage of the rows which should be used for parameter optimization", (PercentValue)Parameters["ConstantOptimizationRowsPercentage"].ActualValue));
          Parameters.Remove("ConstantOptimizationRowsPercentage");
        } else {
          Parameters.Add(new FixedValueParameter<PercentValue>(ParameterOptimizationRowsPercentageParameterName, "Determines the percentage of the rows which should be used for parameter optimization", new PercentValue(1)));
        }

      }
    }

    private static readonly object locker = new object();
    public override IOperation InstrumentedApply() {
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      double quality;
      if (RandomParameter.ActualValue.NextDouble() < ParameterOptimizationProbability.Value) {
        IEnumerable<int> parameterOptimizationRows = GenerateRowsToEvaluate(ParameterOptimizationRowsPercentage.Value);
        var counter = new EvaluationsCounter();
        quality = OptimizeParameters(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, ProblemDataParameter.ActualValue,
           parameterOptimizationRows, ApplyLinearScalingParameter.ActualValue.Value, ParameterOptimizationIterations.Value, updateVariableWeights: UpdateVariableWeights, lowerEstimationLimit: EstimationLimitsParameter.ActualValue.Lower, upperEstimationLimit: EstimationLimitsParameter.ActualValue.Upper, updateParametersInTree: UpdateParametersInTree, counter: counter);

        if (ParameterOptimizationRowsPercentage.Value != RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value) {
          var evaluationRows = GenerateRowsToEvaluate();
          quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
            tree, ProblemDataParameter.ActualValue,
            evaluationRows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
            ApplyLinearScalingParameter.ActualValue.Value,
            EstimationLimitsParameter.ActualValue.Lower,
            EstimationLimitsParameter.ActualValue.Upper);
        }

        if (CountEvaluations) {
          lock (locker) {
            FunctionEvaluationsResultParameter.ActualValue.Value += counter.FunctionEvaluations;
            GradientEvaluationsResultParameter.ActualValue.Value += counter.GradientEvaluations;
          }
        }

      } else {
        var evaluationRows = GenerateRowsToEvaluate();
        quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
          tree, ProblemDataParameter.ActualValue,
          evaluationRows, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
          ApplyLinearScalingParameter.ActualValue.Value,
          EstimationLimitsParameter.ActualValue.Lower,
          EstimationLimitsParameter.ActualValue.Upper);
      }
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.InstrumentedApply();
    }

    public override double Evaluate(
      ISymbolicExpressionTree tree,
      IRegressionProblemData problemData,
      IEnumerable<int> rows,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      bool applyLinearScaling = true,
      double lowerEstimationLimit = double.MinValue,
      double upperEstimationLimit = double.MaxValue) {

      var random = RandomParameter.ActualValue;
      double quality = double.NaN;

      var propability = random.NextDouble();
      if (propability < ParameterOptimizationProbability.Value) {
        quality = OptimizeParameters(
          interpreter, tree,
          problemData, rows,
          applyLinearScaling,
          ParameterOptimizationIterations.Value,
          updateVariableWeights: UpdateVariableWeights,
          lowerEstimationLimit: lowerEstimationLimit,
          upperEstimationLimit: upperEstimationLimit,
          updateParametersInTree: UpdateParametersInTree);
      }
      if (double.IsNaN(quality) || ParameterOptimizationRowsPercentage.Value != RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value) {
        quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
          tree, problemData,
          rows, interpreter,
          applyLinearScaling,
          lowerEstimationLimit,
          upperEstimationLimit);
      }
      return quality;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;
      FunctionEvaluationsResultParameter.ExecutionContext = context;
      GradientEvaluationsResultParameter.ExecutionContext = context;

      // Pearson R² evaluator is used on purpose instead of the const-opt evaluator, 
      // because Evaluate() is used to get the quality of evolved models on 
      // different partitions of the dataset (e.g., best validation model)
      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        tree, problemData, rows,
        SymbolicDataAnalysisTreeInterpreterParameter.ActualValue,
        ApplyLinearScalingParameter.ActualValue.Value,
        EstimationLimitsParameter.ActualValue.Lower,
        EstimationLimitsParameter.ActualValue.Upper);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;
      FunctionEvaluationsResultParameter.ExecutionContext = null;
      GradientEvaluationsResultParameter.ExecutionContext = null;

      return r2;
    }

    public class EvaluationsCounter {
      public int FunctionEvaluations = 0;
      public int GradientEvaluations = 0;
    }

    public static double OptimizeParameters(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling,
      int maxIterations, bool updateVariableWeights = true,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue,
      bool updateParametersInTree = true, Action<double[], double, object> iterationCallback = null, EvaluationsCounter counter = null) {

      // Numeric parameters in the tree become variables for parameter optimization.
      // Variables in the tree become parameters (fixed values) for parameter optimization.
      // For each parameter (variable in the original tree) we store the 
      // variable name, variable value (for factor vars) and lag as a DataForVariable object.
      // A dictionary is used to find parameters
      double[] initialParameters;
      var parameters = new List<TreeToAutoDiffTermConverter.DataForVariable>();

      TreeToAutoDiffTermConverter.ParametricFunction func;
      TreeToAutoDiffTermConverter.ParametricFunctionGradient func_grad;
      if (!TreeToAutoDiffTermConverter.TryConvertToAutoDiff(tree, updateVariableWeights, applyLinearScaling, out parameters, out initialParameters, out func, out func_grad))
        throw new NotSupportedException("Could not optimize parameters of symbolic expression tree due to not supported symbols used in the tree.");
      if (parameters.Count == 0) return 0.0; // constant expressions always have a R² of 0.0 
      var parameterEntries = parameters.ToArray(); // order of entries must be the same for x

      // extract initial parameters
      double[] c;
      if (applyLinearScaling) {
        c = new double[initialParameters.Length + 2];
        c[0] = 0.0;
        c[1] = 1.0;
        Array.Copy(initialParameters, 0, c, 2, initialParameters.Length);
      } else {
        c = (double[])initialParameters.Clone();
      }

      double originalQuality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        tree, problemData, rows,
        interpreter, applyLinearScaling,
        lowerEstimationLimit,
        upperEstimationLimit);

      if (counter == null) counter = new EvaluationsCounter();
      var rowEvaluationsCounter = new EvaluationsCounter();

      alglib.lsfitstate state;
      alglib.lsfitreport rep;
      int retVal;

      IDataset ds = problemData.Dataset;
      double[,] x = new double[rows.Count(), parameters.Count];
      int row = 0;
      foreach (var r in rows) {
        int col = 0;
        foreach (var info in parameterEntries) {
          if (ds.VariableHasType<double>(info.variableName)) {
            x[row, col] = ds.GetDoubleValue(info.variableName, r + info.lag);
          } else if (ds.VariableHasType<string>(info.variableName)) {
            x[row, col] = ds.GetStringValue(info.variableName, r) == info.variableValue ? 1 : 0;
          } else throw new InvalidProgramException("found a variable of unknown type");
          col++;
        }
        row++;
      }
      double[] y = ds.GetDoubleValues(problemData.TargetVariable, rows).ToArray();
      int n = x.GetLength(0);
      int m = x.GetLength(1);
      int k = c.Length;

      alglib.ndimensional_pfunc function_cx_1_func = CreatePFunc(func);
      alglib.ndimensional_pgrad function_cx_1_grad = CreatePGrad(func_grad);
      alglib.ndimensional_rep xrep = (p, f, obj) => iterationCallback(p, f, obj);

      try {
        alglib.lsfitcreatefg(x, y, c, n, m, k, false, out state);
        alglib.lsfitsetcond(state, 0.0, maxIterations);
        alglib.lsfitsetxrep(state, iterationCallback != null);
        alglib.lsfitfit(state, function_cx_1_func, function_cx_1_grad, xrep, rowEvaluationsCounter);
        alglib.lsfitresults(state, out retVal, out c, out rep);
      } catch (ArithmeticException) {
        return originalQuality;
      } catch (alglib.alglibexception) {
        return originalQuality;
      }

      counter.FunctionEvaluations += rowEvaluationsCounter.FunctionEvaluations / n;
      counter.GradientEvaluations += rowEvaluationsCounter.GradientEvaluations / n;

      //retVal == -7  => parameter optimization failed due to wrong gradient
      //          -8  => optimizer detected  NAN / INF  in  the target
      //                 function and/ or gradient
      if (retVal != -7 && retVal != -8) {
        if (applyLinearScaling) {
          var tmp = new double[c.Length - 2];
          Array.Copy(c, 2, tmp, 0, tmp.Length);
          UpdateParameters(tree, tmp, updateVariableWeights);
        } else UpdateParameters(tree, c, updateVariableWeights);
      }
      var quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        tree, problemData, rows,
        interpreter, applyLinearScaling,
        lowerEstimationLimit, upperEstimationLimit);

      if (!updateParametersInTree) UpdateParameters(tree, initialParameters, updateVariableWeights);

      if (originalQuality - quality > 0.001 || double.IsNaN(quality)) {
        UpdateParameters(tree, initialParameters, updateVariableWeights);
        return originalQuality;
      }
      return quality;
    }

    private static void UpdateParameters(ISymbolicExpressionTree tree, double[] parameters, bool updateVariableWeights) {
      int i = 0;
      foreach (var node in tree.Root.IterateNodesPrefix().OfType<SymbolicExpressionTreeTerminalNode>()) {
        NumberTreeNode numberTreeNode = node as NumberTreeNode;
        VariableTreeNodeBase variableTreeNodeBase = node as VariableTreeNodeBase;
        FactorVariableTreeNode factorVarTreeNode = node as FactorVariableTreeNode;
        if (numberTreeNode != null) {
          if (numberTreeNode.Parent.Symbol is Power
              && numberTreeNode.Parent.GetSubtree(1) == numberTreeNode) continue; // exponents in powers are not optimized (see TreeToAutoDiffTermConverter)
          numberTreeNode.Value = parameters[i++];
        } else if (updateVariableWeights && variableTreeNodeBase != null)
          variableTreeNodeBase.Weight = parameters[i++];
        else if (factorVarTreeNode != null) {
          for (int j = 0; j < factorVarTreeNode.Weights.Length; j++)
            factorVarTreeNode.Weights[j] = parameters[i++];
        }
      }
    }

    private static alglib.ndimensional_pfunc CreatePFunc(TreeToAutoDiffTermConverter.ParametricFunction func) {
      return (double[] c, double[] x, ref double fx, object o) => {
        fx = func(c, x);
        var counter = (EvaluationsCounter)o;
        counter.FunctionEvaluations++;
      };
    }

    private static alglib.ndimensional_pgrad CreatePGrad(TreeToAutoDiffTermConverter.ParametricFunctionGradient func_grad) {
      return (double[] c, double[] x, ref double fx, double[] grad, object o) => {
        var tuple = func_grad(c, x);
        fx = tuple.Item2;
        Array.Copy(tuple.Item1, grad, grad.Length);
        var counter = (EvaluationsCounter)o;
        counter.GradientEvaluations++;
      };
    }
    public static bool CanOptimizeParameters(ISymbolicExpressionTree tree) {
      return TreeToAutoDiffTermConverter.IsCompatible(tree);
    }
  }
}
