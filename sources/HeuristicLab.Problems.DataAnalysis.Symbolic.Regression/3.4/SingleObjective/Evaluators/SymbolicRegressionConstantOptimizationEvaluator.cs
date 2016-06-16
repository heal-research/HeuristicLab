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
using AutoDiff;
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
      Parameters.Add(new FixedValueParameter<DoubleValue>(ConstantOptimizationImprovementParameterName, "Determines the relative improvement which must be achieved in the constant optimization to continue with it (0 indicates other or default stopping criterion).", new DoubleValue(0), true));
      Parameters.Add(new FixedValueParameter<PercentValue>(ConstantOptimizationProbabilityParameterName, "Determines the probability that the constants are optimized", new PercentValue(1), true));
      Parameters.Add(new FixedValueParameter<PercentValue>(ConstantOptimizationRowsPercentageParameterName, "Determines the percentage of the rows which should be used for constant optimization", new PercentValue(1), true));
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateConstantsInTreeParameterName, "Determines if the constants in the tree should be overwritten by the optimized constants.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be  optimized.", new BoolValue(true)));
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

    #region derivations of functions
    // create function factory for arctangent
    private readonly Func<Term, UnaryFunc> arctan = UnaryFunc.Factory(
      eval: Math.Atan,
      diff: x => 1 / (1 + x * x));
    private static readonly Func<Term, UnaryFunc> sin = UnaryFunc.Factory(
      eval: Math.Sin,
      diff: Math.Cos);
    private static readonly Func<Term, UnaryFunc> cos = UnaryFunc.Factory(
       eval: Math.Cos,
       diff: x => -Math.Sin(x));
    private static readonly Func<Term, UnaryFunc> tan = UnaryFunc.Factory(
      eval: Math.Tan,
      diff: x => 1 + Math.Tan(x) * Math.Tan(x));
    private static readonly Func<Term, UnaryFunc> erf = UnaryFunc.Factory(
      eval: alglib.errorfunction,
      diff: x => 2.0 * Math.Exp(-(x * x)) / Math.Sqrt(Math.PI));
    private static readonly Func<Term, UnaryFunc> norm = UnaryFunc.Factory(
      eval: alglib.normaldistribution,
      diff: x => -(Math.Exp(-(x * x)) * Math.Sqrt(Math.Exp(x * x)) * x) / Math.Sqrt(2 * Math.PI));
    #endregion


    public static double OptimizeConstants(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows, bool applyLinearScaling, int maxIterations, bool updateVariableWeights = true, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue, bool updateConstantsInTree = true) {

      List<AutoDiff.Variable> variables = new List<AutoDiff.Variable>();
      List<AutoDiff.Variable> parameters = new List<AutoDiff.Variable>();
      List<string> variableNames = new List<string>();

      AutoDiff.Term func;
      if (!TryTransformToAutoDiff(tree.Root.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out func))
        throw new NotSupportedException("Could not optimize constants of symbolic expression tree due to not supported symbols used in the tree.");
      if (variableNames.Count == 0) return 0.0;

      AutoDiff.IParametricCompiledTerm compiledFunc = func.Compile(variables.ToArray(), parameters.ToArray());

      List<SymbolicExpressionTreeTerminalNode> terminalNodes = null;
      if (updateVariableWeights)
        terminalNodes = tree.Root.IterateNodesPrefix().OfType<SymbolicExpressionTreeTerminalNode>().ToList();
      else
        terminalNodes = new List<SymbolicExpressionTreeTerminalNode>(tree.Root.IterateNodesPrefix().OfType<ConstantTreeNode>());

      //extract inital constants
      double[] c = new double[variables.Count];
      {
        c[0] = 0.0;
        c[1] = 1.0;
        int i = 2;
        foreach (var node in terminalNodes) {
          ConstantTreeNode constantTreeNode = node as ConstantTreeNode;
          VariableTreeNode variableTreeNode = node as VariableTreeNode;
          if (constantTreeNode != null)
            c[i++] = constantTreeNode.Value;
          else if (updateVariableWeights && variableTreeNode != null)
            c[i++] = variableTreeNode.Weight;
        }
      }
      double[] originalConstants = (double[])c.Clone();
      double originalQuality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(interpreter, tree, lowerEstimationLimit, upperEstimationLimit, problemData, rows, applyLinearScaling);

      alglib.lsfitstate state;
      alglib.lsfitreport rep;
      int info;

      IDataset ds = problemData.Dataset;
      double[,] x = new double[rows.Count(), variableNames.Count];
      int row = 0;
      foreach (var r in rows) {
        for (int col = 0; col < variableNames.Count; col++) {
          x[row, col] = ds.GetDoubleValue(variableNames[col], r);
        }
        row++;
      }
      double[] y = ds.GetDoubleValues(problemData.TargetVariable, rows).ToArray();
      int n = x.GetLength(0);
      int m = x.GetLength(1);
      int k = c.Length;

      alglib.ndimensional_pfunc function_cx_1_func = CreatePFunc(compiledFunc);
      alglib.ndimensional_pgrad function_cx_1_grad = CreatePGrad(compiledFunc);

      try {
        alglib.lsfitcreatefg(x, y, c, n, m, k, false, out state);
        alglib.lsfitsetcond(state, 0.0, 0.0, maxIterations);
        //alglib.lsfitsetgradientcheck(state, 0.001);
        alglib.lsfitfit(state, function_cx_1_func, function_cx_1_grad, null, null);
        alglib.lsfitresults(state, out info, out c, out rep);
      }
      catch (ArithmeticException) {
        return originalQuality;
      }
      catch (alglib.alglibexception) {
        return originalQuality;
      }

      //info == -7  => constant optimization failed due to wrong gradient
      if (info != -7) UpdateConstants(tree, c.Skip(2).ToArray(), updateVariableWeights);
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
        if (constantTreeNode != null)
          constantTreeNode.Value = constants[i++];
        else if (updateVariableWeights && variableTreeNode != null)
          variableTreeNode.Weight = constants[i++];
      }
    }

    private static alglib.ndimensional_pfunc CreatePFunc(AutoDiff.IParametricCompiledTerm compiledFunc) {
      return (double[] c, double[] x, ref double func, object o) => {
        func = compiledFunc.Evaluate(c, x);
      };
    }

    private static alglib.ndimensional_pgrad CreatePGrad(AutoDiff.IParametricCompiledTerm compiledFunc) {
      return (double[] c, double[] x, ref double func, double[] grad, object o) => {
        var tupel = compiledFunc.Differentiate(c, x);
        func = tupel.Item2;
        Array.Copy(tupel.Item1, grad, grad.Length);
      };
    }

    private static bool TryTransformToAutoDiff(ISymbolicExpressionTreeNode node, List<AutoDiff.Variable> variables, List<AutoDiff.Variable> parameters, List<string> variableNames, bool updateVariableWeights, out AutoDiff.Term term) {
      if (node.Symbol is Constant) {
        var var = new AutoDiff.Variable();
        variables.Add(var);
        term = var;
        return true;
      }
      if (node.Symbol is Variable) {
        var varNode = node as VariableTreeNode;
        var par = new AutoDiff.Variable();
        parameters.Add(par);
        variableNames.Add(varNode.VariableName);

        if (updateVariableWeights) {
          var w = new AutoDiff.Variable();
          variables.Add(w);
          term = AutoDiff.TermBuilder.Product(w, par);
        } else {
          term = par;
        }
        return true;
      }
      if (node.Symbol is Addition) {
        List<AutoDiff.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          AutoDiff.Term t;
          if (!TryTransformToAutoDiff(subTree, variables, parameters, variableNames, updateVariableWeights, out t)) {
            term = null;
            return false;
          }
          terms.Add(t);
        }
        term = AutoDiff.TermBuilder.Sum(terms);
        return true;
      }
      if (node.Symbol is Subtraction) {
        List<AutoDiff.Term> terms = new List<Term>();
        for (int i = 0; i < node.SubtreeCount; i++) {
          AutoDiff.Term t;
          if (!TryTransformToAutoDiff(node.GetSubtree(i), variables, parameters, variableNames, updateVariableWeights, out t)) {
            term = null;
            return false;
          }
          if (i > 0) t = -t;
          terms.Add(t);
        }
        term = AutoDiff.TermBuilder.Sum(terms);
        return true;
      }
      if (node.Symbol is Multiplication) {
        List<AutoDiff.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          AutoDiff.Term t;
          if (!TryTransformToAutoDiff(subTree, variables, parameters, variableNames, updateVariableWeights, out t)) {
            term = null;
            return false;
          }
          terms.Add(t);
        }
        if (terms.Count == 1) term = terms[0];
        else term = terms.Aggregate((a, b) => new AutoDiff.Product(a, b));
        return true;

      }
      if (node.Symbol is Division) {
        List<AutoDiff.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          AutoDiff.Term t;
          if (!TryTransformToAutoDiff(subTree, variables, parameters, variableNames, updateVariableWeights, out t)) {
            term = null;
            return false;
          }
          terms.Add(t);
        }
        if (terms.Count == 1) term = 1.0 / terms[0];
        else term = terms.Aggregate((a, b) => new AutoDiff.Product(a, 1.0 / b));
        return true;
      }
      if (node.Symbol is Logarithm) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out t)) {
          term = null;
          return false;
        } else {
          term = AutoDiff.TermBuilder.Log(t);
          return true;
        }
      }
      if (node.Symbol is Exponential) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out t)) {
          term = null;
          return false;
        } else {
          term = AutoDiff.TermBuilder.Exp(t);
          return true;
        }
      }
      if (node.Symbol is Square) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out t)) {
          term = null;
          return false;
        } else {
          term = AutoDiff.TermBuilder.Power(t, 2.0);
          return true;
        }
      }
      if (node.Symbol is SquareRoot) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out t)) {
          term = null;
          return false;
        } else {
          term = AutoDiff.TermBuilder.Power(t, 0.5);
          return true;
        }
      }
      if (node.Symbol is Sine) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out t)) {
          term = null;
          return false;
        } else {
          term = sin(t);
          return true;
        }
      }
      if (node.Symbol is Cosine) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out t)) {
          term = null;
          return false;
        } else {
          term = cos(t);
          return true;
        }
      }
      if (node.Symbol is Tangent) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out t)) {
          term = null;
          return false;
        } else {
          term = tan(t);
          return true;
        }
      }
      if (node.Symbol is Erf) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out t)) {
          term = null;
          return false;
        } else {
          term = erf(t);
          return true;
        }
      }
      if (node.Symbol is Norm) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out t)) {
          term = null;
          return false;
        } else {
          term = norm(t);
          return true;
        }
      }
      if (node.Symbol is StartSymbol) {
        var alpha = new AutoDiff.Variable();
        var beta = new AutoDiff.Variable();
        variables.Add(beta);
        variables.Add(alpha);
        AutoDiff.Term branchTerm;
        if (TryTransformToAutoDiff(node.GetSubtree(0), variables, parameters, variableNames, updateVariableWeights, out branchTerm)) {
          term = branchTerm * alpha + beta;
          return true;
        } else {
          term = null;
          return false;
        }
      }
      term = null;
      return false;
    }

    public static bool CanOptimizeConstants(ISymbolicExpressionTree tree) {
      var containsUnknownSymbol = (
        from n in tree.Root.GetSubtree(0).IterateNodesPrefix()
        where
         !(n.Symbol is Variable) &&
         !(n.Symbol is Constant) &&
         !(n.Symbol is Addition) &&
         !(n.Symbol is Subtraction) &&
         !(n.Symbol is Multiplication) &&
         !(n.Symbol is Division) &&
         !(n.Symbol is Logarithm) &&
         !(n.Symbol is Exponential) &&
         !(n.Symbol is SquareRoot) &&
         !(n.Symbol is Square) &&
         !(n.Symbol is Sine) &&
         !(n.Symbol is Cosine) &&
         !(n.Symbol is Tangent) &&
         !(n.Symbol is Erf) &&
         !(n.Symbol is Norm) &&
         !(n.Symbol is StartSymbol)
        select n).
      Any();
      return !containsUnknownSymbol;
    }
  }
}
