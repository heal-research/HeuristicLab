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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic {
  /// <summary>
  /// An operator that creates a linearly transformed symbolic regression solution (given alpha and beta).
  /// </summary>
  [Item("SymbolicVectorRegressionSolutionLinearScaler", "An operator that creates a linearly transformed symbolic regression solution (given alpha and beta).")]
  [StorableClass]
  public sealed class SymbolicVectorRegressionSolutionLinearScaler : SingleSuccessorOperator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ScaledSymbolicExpressionTreeParameterName = "ScaledSymbolicExpressionTree";
    private const string AlphaParameterName = "Alpha";
    private const string BetaParameterName = "Beta";

    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<SymbolicExpressionTree> ScaledSymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[ScaledSymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<DoubleArray> AlphaParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[AlphaParameterName]; }
    }
    public ILookupParameter<DoubleArray> BetaParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[BetaParameterName]; }
    }

    public SymbolicVectorRegressionSolutionLinearScaler()
      : base() {
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to transform."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(ScaledSymbolicExpressionTreeParameterName, "The resulting symbolic expression trees after transformation."));
      Parameters.Add(new LookupParameter<DoubleArray>(AlphaParameterName, "Alpha parameter for linear transformation."));
      Parameters.Add(new LookupParameter<DoubleArray>(BetaParameterName, "Beta parameter for linear transformation."));
    }

    public override IOperation Apply() {
      SymbolicExpressionTree tree = SymbolicExpressionTreeParameter.ActualValue;
      DoubleArray alpha = AlphaParameter.ActualValue;
      DoubleArray beta = BetaParameter.ActualValue;
      if (alpha != null && beta != null) {
        ScaledSymbolicExpressionTreeParameter.ActualValue = Scale(tree, alpha.ToArray(), beta.ToArray());
      } else {
        // alpha or beta parameter not available => do not scale tree
        ScaledSymbolicExpressionTreeParameter.ActualValue = tree;
      }

      return base.Apply();
    }

    public static SymbolicExpressionTree Scale(SymbolicExpressionTree original, double[] alpha, double[] beta) {
      List<SymbolicExpressionTreeNode> resultProducingBranches = new List<SymbolicExpressionTreeNode>(original.Root.SubTrees[0].SubTrees);
      // remove the main branch before cloning to prevent cloning of sub-trees
      while (original.Root.SubTrees[0].SubTrees.Count > 0)
        original.Root.SubTrees[0].RemoveSubTree(0);
      var scaledTree = (SymbolicExpressionTree)original.Clone();
      int i = 0;
      foreach (var resultProducingBranch in resultProducingBranches) {
        var scaledMainBranch = MakeSum(MakeProduct(beta[i], resultProducingBranch), alpha[i]);

        // insert main branch into the original tree again 
        original.Root.SubTrees[0].AddSubTree(resultProducingBranch);
        // insert the scaled main branch into the cloned tree
        scaledTree.Root.SubTrees[0].AddSubTree(scaledMainBranch);
        i++;
      }
      return scaledTree;
    }

    private static SymbolicExpressionTreeNode MakeSum(SymbolicExpressionTreeNode treeNode, double alpha) {
      var node = (new Addition()).CreateTreeNode();
      var alphaConst = MakeConstant(alpha);
      node.AddSubTree(treeNode);
      node.AddSubTree(alphaConst);
      return node;
    }

    private static SymbolicExpressionTreeNode MakeProduct(double beta, SymbolicExpressionTreeNode treeNode) {
      var node = (new Multiplication()).CreateTreeNode();
      var betaConst = MakeConstant(beta);
      node.AddSubTree(treeNode);
      node.AddSubTree(betaConst);
      return node;
    }

    private static SymbolicExpressionTreeNode MakeConstant(double c) {
      var node = (ConstantTreeNode)(new Constant()).CreateTreeNode();
      node.Value = c;
      return node;
    }
  }
}
