#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Constant Optimization Evaluator", "Calculates Pearson R² of a symbolic regression solution and optimizes the constant used.")]
  [StorableClass]
  public class SymbolicRegressionConstantOptimizationEvaluator : SymbolicRegressionSingleObjectiveEvaluator {
    private const string ConstantOptimizationIterationsParameterName = "ConstantOptimizationIterations";
    private const string ConstantOptimizationImprovementParameterName = "ConstantOptimizationImprovement";
    private const string ConstantOptimizationProbabilityParameterName = "ConstantOptimizationProbability";
    private const string ConstantOptimizationRowsPercentageParameterName = "ConstantOptimizationRowsPercentage";
    private const string UpdateConstantsInTreeParameterName = "UpdateConstantsInSymbolicExpressionTree";
    private const string UpdateVariableWeightsParameterName = "Update Variable Weights";

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

    public override bool Maximization {
      get { return true; }
    }

    [StorableConstructor]
    protected SymbolicRegressionConstantOptimizationEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicRegressionConstantOptimizationEvaluator(SymbolicRegressionConstantOptimizationEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicRegressionConstantOptimizationEvaluator()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(ConstantOptimizationIterationsParameterName, "Determines how many iterations should be calculated while optimizing the constant of a symbolic expression tree (0 indicates other or default stopping criterion).", new IntValue(10), true));
      Parameters.Add(new FixedValueParameter<DoubleValue>(ConstantOptimizationImprovementParameterName, "Determines the relative improvement which must be achieved in the constant optimization to continue with it (0 indicates other or default stopping criterion).", new DoubleValue(0), true) { Hidden = true });
      Parameters.Add(new FixedValueParameter<PercentValue>(ConstantOptimizationProbabilityParameterName, "Determines the probability that the constants are optimized", new PercentValue(1), true));
      Parameters.Add(new FixedValueParameter<PercentValue>(ConstantOptimizationRowsPercentageParameterName, "Determines the percentage of the rows which should be used for constant optimization", new PercentValue(1), true));
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateConstantsInTreeParameterName, "Determines if the constants in the tree should be overwritten by the optimized constants.", new BoolValue(true)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be  optimized.", new BoolValue(true)) { Hidden = true });
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionConstantOptimizationEvaluator(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(UpdateConstantsInTreeParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateConstantsInTreeParameterName, "Determines if the constants in the tree should be overwritten by the optimized constants.", new BoolValue(true)));
      if (!Parameters.ContainsKey(UpdateVariableWeightsParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be  optimized.", new BoolValue(true)));
    }

    public override IOperation InstrumentedApply() {
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      double quality;
      if (RandomParameter.ActualValue.NextDouble() < ConstantOptimizationProbability.Value) {
        IEnumerable<int> constantOptimizationRows = GenerateRowsToEvaluate(ConstantOptimizationRowsPercentage.Value);
        quality = OptimizeConstants(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, ProblemDataParameter.ActualValue,
           constantOptimizationRows, ApplyLinearScalingParameter.ActualValue.Value, ConstantOptimizationIterations.Value, updateVariableWeights: UpdateVariableWeights, lowerEstimationLimit: EstimationLimitsParameter.ActualValue.Lower, upperEstimationLimit: EstimationLimitsParameter.ActualValue.Upper, updateConstantsInTree: UpdateConstantsInTree);

        if (ConstantOptimizationRowsPercentage.Value != RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value) {
          var evaluationRows = GenerateRowsToEvaluate();
          quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, evaluationRows, ApplyLinearScalingParameter.ActualValue.Value);
        }
      } else {
        var evaluationRows = GenerateRowsToEvaluate();
        quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, evaluationRows, ApplyLinearScalingParameter.ActualValue.Value);
      }
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.InstrumentedApply();
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      // Pearson R² evaluator is used on purpose instead of the const-opt evaluator, 
      // because Evaluate() is used to get the quality of evolved models on 
      // different partitions of the dataset (e.g., best validation model)
      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows, ApplyLinearScalingParameter.ActualValue.Value);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return r2;
    }

    public static double OptimizeConstants(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling,
      int maxIterations, bool updateVariableWeights = true,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue,
      bool updateConstantsInTree = true) {

      // numeric constants in the tree become variables for constant opt
      // variables in the tree become parameters (fixed values) for constant opt
      // for each parameter (variable in the original tree) we store the 
      // variable name, variable value (for factor vars) and lag as a DataForVariable object.
      // A dictionary is used to find parameters
      double[] initialConstants;
      var parameters = new List<TreeToAutoDiffTermConverter.DataForVariable>();

      TreeToAutoDiffTermConverter.ParametricFunction func;
      TreeToAutoDiffTermConverter.ParametricFunctionGradient func_grad;
      if (!TreeToAutoDiffTermConverter.TryConvertToAutoDiff(tree, updateVariableWeights, out parameters, out initialConstants, out func, out func_grad))
        throw new NotSupportedException("Could not optimize constants of symbolic expression tree due to not supported symbols used in the tree.");
      if (parameters.Count == 0) return 0.0; // gkronber: constant expressions always have a R² of 0.0 

      var parameterEntries = parameters.ToArray(); // order of entries must be the same for x

      //extract inital constants
      double[] c = new double[initialConstants.Length + 2];
      {
        c[0] = 0.0;
        c[1] = 1.0;
        Array.Copy(initialConstants, 0, c, 2, initialConstants.Length);
      }
      double[] originalConstants = (double[])c.Clone();
      double originalQuality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(interpreter, tree, lowerEstimationLimit, upperEstimationLimit, problemData, rows, applyLinearScaling);

      alglib.lsfitstate state;
      alglib.lsfitreport rep;
      int retVal;

      IDataset ds = problemData.Dataset;
      double[,] x = new double[rows.Count(), parameters.Count];
      int row = 0;
      foreach (var r in rows) {
        int col = 0;
        foreach (var info in parameterEntries) {
          int lag = info.lag;
          if (ds.VariableHasType<double>(info.variableName)) {
            x[row, col] = ds.GetDoubleValue(info.variableName, r + lag);
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

      try {
        alglib.lsfitcreatefg(x, y, c, n, m, k, false, out state);
        alglib.lsfitsetcond(state, 0.0, 0.0, maxIterations);
        //alglib.lsfitsetgradientcheck(state, 0.001);
        alglib.lsfitfit(state, function_cx_1_func, function_cx_1_grad, null, null);
        alglib.lsfitresults(state, out retVal, out c, out rep);
      } catch (ArithmeticException) {
        return originalQuality;
      } catch (alglib.alglibexception) {
        return originalQuality;
      }

      //retVal == -7  => constant optimization failed due to wrong gradient
      if (retVal != -7) UpdateConstants(tree, c.Skip(2).ToArray(), updateVariableWeights);
      var quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(interpreter, tree, lowerEstimationLimit, upperEstimationLimit, problemData, rows, applyLinearScaling);

      if (!updateConstantsInTree) UpdateConstants(tree, originalConstants.Skip(2).ToArray(), updateVariableWeights);
      if (originalQuality - quality > 0.001 || double.IsNaN(quality)) {
        UpdateConstants(tree, originalConstants.Skip(2).ToArray(), updateVariableWeights);
        return originalQuality;
      }
      return quality;
    }

    private static void UpdateConstants(ISymbolicExpressionTree tree, double[] constants, bool updateVariableWeights) {
      int i = 0;
      foreach (var node in tree.Root.IterateNodesPrefix().OfType<SymbolicExpressionTreeTerminalNode>()) {
        ConstantTreeNode constantTreeNode = node as ConstantTreeNode;
        VariableTreeNode variableTreeNode = node as VariableTreeNode;
        BinaryFactorVariableTreeNode binFactorVarTreeNode = node as BinaryFactorVariableTreeNode;
        FactorVariableTreeNode factorVarTreeNode = node as FactorVariableTreeNode;
        if (constantTreeNode != null)
          constantTreeNode.Value = constants[i++];
        else if (updateVariableWeights && variableTreeNode != null)
          variableTreeNode.Weight = constants[i++];
        else if (updateVariableWeights && binFactorVarTreeNode != null)
          binFactorVarTreeNode.Weight = constants[i++];
        else if (factorVarTreeNode != null) {
          for (int j = 0; j < factorVarTreeNode.Weights.Length; j++)
            factorVarTreeNode.Weights[j] = constants[i++];
        }
      }
    }

    private static alglib.ndimensional_pfunc CreatePFunc(TreeToAutoDiffTermConverter.ParametricFunction func) {
      return (double[] c, double[] x, ref double fx, object o) => {
        fx = func(c, x);
      };
    }

    private static alglib.ndimensional_pgrad CreatePGrad(TreeToAutoDiffTermConverter.ParametricFunctionGradient func_grad) {
      return (double[] c, double[] x, ref double fx, double[] grad, object o) => {
        var tupel = func_grad(c, x);
        fx = tupel.Item2;
        Array.Copy(tupel.Item1, grad, grad.Length);
      };
    }
    public static bool CanOptimizeConstants(ISymbolicExpressionTree tree) {
      return TreeToAutoDiffTermConverter.IsCompatible(tree);
    }
  }
}
