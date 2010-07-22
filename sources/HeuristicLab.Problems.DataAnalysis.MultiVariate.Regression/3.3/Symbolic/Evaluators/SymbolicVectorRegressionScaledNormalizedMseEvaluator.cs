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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Interfaces;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Evaluators {
  [Item("SymbolicVectorRegressionScaledNormalizedMseEvaluator", "Represents an operator that calculates the sum of the normalized mean squared error over all components.")]
  [StorableClass]
  public class SymbolicVectorRegressionScaledNormalizedMseEvaluator : SymbolicVectorRegressionEvaluator, ISingleObjectiveSymbolicVectorRegressionEvaluator {
    private const string QualityParameterName = "ScaledNormalizedMeanSquaredError";
    private const string AlphaParameterName = "Alpha";
    private const string BetaParameterName = "Beta";

    #region parameter properties
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<DoubleArray> AlphaParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[AlphaParameterName]; }
    }
    public ILookupParameter<DoubleArray> BetaParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[BetaParameterName]; }
    }

    #endregion

    public SymbolicVectorRegressionScaledNormalizedMseEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The sum of the normalized mean squared error over all components of the symbolic vector regression solution encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<DoubleArray>(AlphaParameterName, "The alpha parameter for linear scaling."));
      Parameters.Add(new LookupParameter<DoubleArray>(BetaParameterName, "The beta parameter for linear scaling."));
    }

    public override void Evaluate(SymbolicExpressionTree tree, ISymbolicExpressionTreeInterpreter interpreter, MultiVariateDataAnalysisProblemData problemData, IEnumerable<string> targetVariables, IEnumerable<int> rows, DoubleArray lowerEstimationBound, DoubleArray upperEstimationBound) {
      List<string> targetVariablesList = targetVariables.ToList();
      double nmseSum = 0.0;

      DoubleArray alpha = new DoubleArray(targetVariablesList.Count);
      DoubleArray beta = new DoubleArray(targetVariablesList.Count);

      // use only the i-th vector component
      List<SymbolicExpressionTreeNode> componentBranches = new List<SymbolicExpressionTreeNode>(tree.Root.SubTrees[0].SubTrees);
      while (tree.Root.SubTrees[0].SubTrees.Count > 0) tree.Root.SubTrees[0].RemoveSubTree(0);
      for (int i = 0; i < targetVariablesList.Count; i++) {
        tree.Root.SubTrees[0].AddSubTree(componentBranches[i]);
        double compAlpha;
        double compBeta;
        double nmse = SymbolicRegressionScaledNormalizedMeanSquaredErrorEvaluator.Calculate(interpreter, tree,
          lowerEstimationBound[i], upperEstimationBound[i],
          problemData.Dataset, targetVariablesList[i], rows, out compBeta, out compAlpha);
        alpha[i] = compAlpha;
        beta[i] = compBeta;
        nmseSum += nmse;
        tree.Root.SubTrees[0].RemoveSubTree(0);
      }
      // restore tree
      foreach (var treeNode in componentBranches) {
        tree.Root.SubTrees[0].AddSubTree(treeNode);
      }
      AlphaParameter.ActualValue = alpha;
      BetaParameter.ActualValue = beta;
      QualityParameter.ActualValue = new DoubleValue(nmseSum / targetVariables.Count());
    }
  }
}
