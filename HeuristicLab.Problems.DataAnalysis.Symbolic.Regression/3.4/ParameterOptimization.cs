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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  public static class ParameterOptimization {
    public static double OptimizeTreeParameters(IRegressionProblemData problemData, ISymbolicExpressionTree tree, int maxIterations = 10,
      bool updateParametersInTree = true, bool updateVariableWeights = true, IEnumerable<ISymbolicExpressionTreeNode> excludeNodes = null,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue,
      IEnumerable<int> rows = null, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter = null,
      Action<double[], double, object> iterationCallback = null) {

      if (rows == null) rows = problemData.TrainingIndices;
      if (interpreter == null) interpreter = new SymbolicDataAnalysisExpressionTreeBatchInterpreter();
      if (excludeNodes == null) excludeNodes = Enumerable.Empty<ISymbolicExpressionTreeNode>();

      // Numeric parameters in the tree become variables for parameter optimization.
      // Variables in the tree become parameters (fixed values) for parameter optimization.
      // For each parameter (variable in the original tree) we store the 
      // variable name, variable value (for factor vars) and lag as a DataForVariable object.
      // A dictionary is used to find parameters
      double[] initialParameters;
      var parameters = new List<TreeToAutoDiffTermConverter.DataForVariable>();

      TreeToAutoDiffTermConverter.ParametricFunction func;
      TreeToAutoDiffTermConverter.ParametricFunctionGradient func_grad;
      if (!TreeToAutoDiffTermConverter.TryConvertToAutoDiff(tree,
        updateVariableWeights, addLinearScalingTerms: false, excludeNodes,
        out parameters, out initialParameters, out func, out func_grad))
        throw new NotSupportedException("Could not optimize parameters of symbolic expression tree due to not supported symbols used in the tree.");
      var parameterEntries = parameters.ToArray(); // order of entries must be the same for x

      // extract initial parameters
      double[] c = (double[])initialParameters.Clone();
      alglib.minlmreport rep;

      double originalQuality = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(
        tree, problemData, rows,
        interpreter, applyLinearScaling: false,
        lowerEstimationLimit, upperEstimationLimit);


      IDataset ds = problemData.Dataset;
      int n = rows.Count();
      int k = parameters.Count;

      double[,] x = new double[n, k];
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

      alglib.ndimensional_rep xrep = (p, f, obj) => iterationCallback(p, f, obj);

      try {
        alglib.minlmcreatevj(y.Length, c, out var lmstate);
        alglib.minlmsetcond(lmstate, 0.0, maxIterations);
        alglib.minlmsetxrep(lmstate, iterationCallback != null);
        // alglib.minlmoptguardgradient(lmstate, 1e-5); // for debugging gradient calculation
        alglib.minlmoptimize(lmstate, CreateFunc(func, x, y), CreateJac(func_grad, x, y), xrep, null);
        alglib.minlmresults(lmstate, out c, out rep);
        // alglib.minlmoptguardresults(lmstate, out var optGuardReport);
      } catch (ArithmeticException) {
        return originalQuality;
      } catch (alglib.alglibexception) {
        return originalQuality;
      }


      // * TerminationType, completion code:
      //     * -8    optimizer detected NAN/INF values either in the function itself,
      //             or in its Jacobian
      //     * -5    inappropriate solver was used:
      //             * solver created with minlmcreatefgh() used  on  problem  with
      //               general linear constraints (set with minlmsetlc() call).
      //     * -3    constraints are inconsistent
      //     *  2    relative step is no more than EpsX.
      //     *  5    MaxIts steps was taken
      //     *  7    stopping conditions are too stringent,
      //             further improvement is impossible
      //     *  8    terminated   by  user  who  called  MinLMRequestTermination().
      //             X contains point which was "current accepted" when termination
      //             request was submitted.
      if (rep.terminationtype > 0) {
        UpdateParameters(tree, c, updateVariableWeights, excludeNodes);
      }
      var quality = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(
        tree, problemData, rows,
        interpreter, applyLinearScaling: false,
        lowerEstimationLimit, upperEstimationLimit);

      if (!updateParametersInTree) UpdateParameters(tree, initialParameters, updateVariableWeights, excludeNodes);

      if (originalQuality < quality || double.IsNaN(quality)) {
        UpdateParameters(tree, initialParameters, updateVariableWeights, excludeNodes);
        return originalQuality;
      }
      return quality;
    }

    private static void UpdateParameters(ISymbolicExpressionTree tree, double[] parameters,
      bool updateVariableWeights, IEnumerable<ISymbolicExpressionTreeNode> excludedNodes) {
      int i = 0;
      foreach (var node in tree.Root.IterateNodesPrefix().OfType<SymbolicExpressionTreeTerminalNode>().Except(excludedNodes)) {
        NumberTreeNode numberTreeNode = node as NumberTreeNode;
        VariableTreeNodeBase variableTreeNodeBase = node as VariableTreeNodeBase;
        FactorVariableTreeNode factorVarTreeNode = node as FactorVariableTreeNode;
        if (numberTreeNode != null) {
          if (numberTreeNode.Parent.Symbol is Power
              && numberTreeNode.Parent.GetSubtree(1) == numberTreeNode) continue; // exponents in powers are not optimized (see TreeToAutoDiffTermConverter)
          numberTreeNode.Value = parameters[i++];
        } else if (updateVariableWeights && variableTreeNodeBase != null)
          variableTreeNodeBase.Weight = parameters[i++];
        else if (updateVariableWeights && factorVarTreeNode != null) {
          for (int j = 0; j < factorVarTreeNode.Weights.Length; j++)
            factorVarTreeNode.Weights[j] = parameters[i++];
        }
      }
    }

    private static alglib.ndimensional_fvec CreateFunc(TreeToAutoDiffTermConverter.ParametricFunction func, double[,] x, double[] y) {
      int d = x.GetLength(1);
      // row buffer
      var xi = new double[d];
      // function must return residuals, alglib optimizes resid²
      return (double[] c, double[] resid, object o) => {
        for (int i = 0; i < y.Length; i++) {
          Buffer.BlockCopy(x, i * d * sizeof(double), xi, 0, d * sizeof(double)); // copy row. We are using BlockCopy instead of Array.Copy because x has rank 2
          resid[i] = func(c, xi) - y[i];
        }
      };
    }

    private static alglib.ndimensional_jac CreateJac(TreeToAutoDiffTermConverter.ParametricFunctionGradient func_grad, double[,] x, double[] y) {
      int numParams = x.GetLength(1);
      // row buffer
      var xi = new double[numParams];
      return (double[] c, double[] resid, double[,] jac, object o) => {
        int numVars = c.Length;
        for (int i = 0; i < y.Length; i++) {
          Buffer.BlockCopy(x, i * numParams * sizeof(double), xi, 0, numParams * sizeof(double)); // copy row
          var tuple = func_grad(c, xi);
          resid[i] = tuple.Item2 - y[i];
          Buffer.BlockCopy(tuple.Item1, 0, jac, i * numVars * sizeof(double), numVars * sizeof(double)); // copy the gradient to jac. BlockCopy because jac has rank 2.
        }
      };
    }

  }
}
