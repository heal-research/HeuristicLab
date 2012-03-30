#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

    private const string EvaluatedTreesResultName = "EvaluatedTrees";
    private const string EvaluatedTreeNodesResultName = "EvaluatedTreeNodes";

    public ILookupParameter<IntValue> EvaluatedTreesParameter {
      get { return (ILookupParameter<IntValue>)Parameters[EvaluatedTreesResultName]; }
    }
    public ILookupParameter<IntValue> EvaluatedTreeNodesParameter {
      get { return (ILookupParameter<IntValue>)Parameters[EvaluatedTreeNodesResultName]; }
    }

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
      Parameters.Add(new FixedValueParameter<IntValue>(ConstantOptimizationIterationsParameterName, "Determines how many iterations should be calculated while optimizing the constant of a symbolic expression tree (0 indicates other or default stopping criterion).", new IntValue(3), true));
      Parameters.Add(new FixedValueParameter<DoubleValue>(ConstantOptimizationImprovementParameterName, "Determines the relative improvement which must be achieved in the constant optimization to continue with it (0 indicates other or default stopping criterion).", new DoubleValue(0), true));
      Parameters.Add(new FixedValueParameter<PercentValue>(ConstantOptimizationProbabilityParameterName, "Determines the probability that the constants are optimized", new PercentValue(1), true));
      Parameters.Add(new FixedValueParameter<PercentValue>(ConstantOptimizationRowsPercentageParameterName, "Determines the percentage of the rows which should be used for constant optimization", new PercentValue(1), true));

      Parameters.Add(new LookupParameter<IntValue>(EvaluatedTreesResultName));
      Parameters.Add(new LookupParameter<IntValue>(EvaluatedTreeNodesResultName));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionConstantOptimizationEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      AddResults();
      int seed = RandomParameter.ActualValue.Next();
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      double quality;
      if (RandomParameter.ActualValue.NextDouble() < ConstantOptimizationProbability.Value) {
        IEnumerable<int> constantOptimizationRows = GenerateRowsToEvaluate(ConstantOptimizationRowsPercentage.Value);
        quality = OptimizeConstants(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, ProblemDataParameter.ActualValue,
           constantOptimizationRows, ConstantOptimizationImprovement.Value, ConstantOptimizationIterations.Value, 0.001,
           EstimationLimitsParameter.ActualValue.Upper, EstimationLimitsParameter.ActualValue.Lower,
          EvaluatedTreesParameter.ActualValue, EvaluatedTreeNodesParameter.ActualValue);
        if (ConstantOptimizationRowsPercentage.Value != RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value) {
          var evaluationRows = GenerateRowsToEvaluate();
          quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, evaluationRows);
        }
      } else {
        var evaluationRows = GenerateRowsToEvaluate();
        quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, ProblemDataParameter.ActualValue, evaluationRows);
      }
      QualityParameter.ActualValue = new DoubleValue(quality);
      EvaluatedTreesParameter.ActualValue.Value += 1;
      EvaluatedTreeNodesParameter.ActualValue.Value += solution.Length;

      if (Successor != null)
        return ExecutionContext.CreateOperation(Successor);
      else
        return null;
    }

    private void AddResults() {
      if (EvaluatedTreesParameter.ActualValue == null) {
        var scope = ExecutionContext.Scope;
        while (scope.Parent != null)
          scope = scope.Parent;
        scope.Variables.Add(new Core.Variable(EvaluatedTreesResultName, new IntValue()));
      }
      if (EvaluatedTreeNodesParameter.ActualValue == null) {
        var scope = ExecutionContext.Scope;
        while (scope.Parent != null)
          scope = scope.Parent;
        scope.Variables.Add(new Core.Variable(EvaluatedTreeNodesResultName, new IntValue()));
      }
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;

      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper, problemData, rows);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;

      return r2;
    }

    public static double OptimizeConstants(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree tree, IRegressionProblemData problemData,
      IEnumerable<int> rows, double improvement, int iterations, double differentialStep, double upperEstimationLimit = double.MaxValue, double lowerEstimationLimit = double.MinValue, IntValue evaluatedTrees = null, IntValue evaluatedTreeNodes = null) {
      List<SymbolicExpressionTreeTerminalNode> terminalNodes = tree.Root.IterateNodesPrefix().OfType<SymbolicExpressionTreeTerminalNode>().ToList();
      double[] c = new double[terminalNodes.Count];
      int treeLength = tree.Length;

      //extract inital constants
      for (int i = 0; i < terminalNodes.Count; i++) {
        ConstantTreeNode constantTreeNode = terminalNodes[i] as ConstantTreeNode;
        if (constantTreeNode != null) c[i] = constantTreeNode.Value;
        VariableTreeNode variableTreeNode = terminalNodes[i] as VariableTreeNode;
        if (variableTreeNode != null) c[i] = variableTreeNode.Weight;
      }

      double epsg = 0;
      double epsf = improvement;
      double epsx = 0;
      int maxits = iterations;
      double diffstep = differentialStep;

      alglib.minlmstate state;
      alglib.minlmreport report;

      alglib.minlmcreatev(1, c, diffstep, out state);
      alglib.minlmsetcond(state, epsg, epsf, epsx, maxits);
      alglib.minlmoptimize(state, CreateCallBack(interpreter, tree, problemData, rows, upperEstimationLimit, lowerEstimationLimit, treeLength, evaluatedTrees, evaluatedTreeNodes), null, terminalNodes);
      alglib.minlmresults(state, out c, out report);

      for (int i = 0; i < c.Length; i++) {
        ConstantTreeNode constantTreeNode = terminalNodes[i] as ConstantTreeNode;
        if (constantTreeNode != null) constantTreeNode.Value = c[i];
        VariableTreeNode variableTreeNode = terminalNodes[i] as VariableTreeNode;
        if (variableTreeNode != null) variableTreeNode.Weight = c[i];
      }

      return (state.fi[0] - 1) * -1;
    }

    private static alglib.ndimensional_fvec CreateCallBack(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree tree, IRegressionProblemData problemData, IEnumerable<int> rows, double upperEstimationLimit, double lowerEstimationLimit, int treeLength, IntValue evaluatedTrees = null, IntValue evaluatedTreeNodes = null) {
      return (double[] arg, double[] fi, object obj) => {
        // update constants of tree
        List<SymbolicExpressionTreeTerminalNode> terminalNodes = (List<SymbolicExpressionTreeTerminalNode>)obj;
        for (int i = 0; i < terminalNodes.Count; i++) {
          ConstantTreeNode constantTreeNode = terminalNodes[i] as ConstantTreeNode;
          if (constantTreeNode != null) constantTreeNode.Value = arg[i];
          VariableTreeNode variableTreeNode = terminalNodes[i] as VariableTreeNode;
          if (variableTreeNode != null) variableTreeNode.Weight = arg[i];
        }

        double quality = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(interpreter, tree, lowerEstimationLimit, upperEstimationLimit, problemData, rows);

        fi[0] = 1 - quality;
        if (evaluatedTrees != null) evaluatedTrees.Value++;
        if (evaluatedTreeNodes != null) evaluatedTreeNodes.Value += treeLength;
      };
    }

  }
}
