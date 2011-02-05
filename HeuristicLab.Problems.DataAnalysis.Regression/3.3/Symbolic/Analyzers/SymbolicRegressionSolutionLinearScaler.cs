#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// An operator that creates a linearly transformed symbolic regression solution (given alpha and beta).
  /// </summary>
  [Item("SymbolicRegressionSolutionLinearScaler", "An operator that creates a linearly transformed symbolic regression solution (given alpha and beta).")]
  [StorableClass]
  public sealed class SymbolicRegressionSolutionLinearScaler : SingleSuccessorOperator {
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
    public ILookupParameter<DoubleValue> AlphaParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[AlphaParameterName]; }
    }
    public ILookupParameter<DoubleValue> BetaParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BetaParameterName]; }
    }

    [StorableConstructor]
    private SymbolicRegressionSolutionLinearScaler(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionSolutionLinearScaler(SymbolicRegressionSolutionLinearScaler original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionSolutionLinearScaler()
      : base() {
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to transform."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(ScaledSymbolicExpressionTreeParameterName, "The resulting symbolic expression trees after transformation."));
      Parameters.Add(new LookupParameter<DoubleValue>(AlphaParameterName, "Alpha parameter for linear transformation."));
      Parameters.Add(new LookupParameter<DoubleValue>(BetaParameterName, "Beta parameter for linear transformation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSolutionLinearScaler(this, cloner);
    }

    public override IOperation Apply() {
      SymbolicExpressionTree tree = SymbolicExpressionTreeParameter.ActualValue;
      DoubleValue alpha = AlphaParameter.ActualValue;
      DoubleValue beta = BetaParameter.ActualValue;
      if (alpha != null && beta != null) {
        ScaledSymbolicExpressionTreeParameter.ActualValue = Scale(tree, alpha.Value, beta.Value);
      } else {
        // alpha or beta parameter not available => do not scale tree
        ScaledSymbolicExpressionTreeParameter.ActualValue = tree;
      }

      return base.Apply();
    }

    public static SymbolicExpressionTree Scale(SymbolicExpressionTree original, double alpha, double beta) {
      var mainBranch = original.Root.SubTrees[0].SubTrees[0];
      var scaledMainBranch = MakeSum(MakeProduct(beta, mainBranch), alpha);

      // remove the main branch before cloning to prevent cloning of sub-trees
      original.Root.SubTrees[0].RemoveSubTree(0);
      var scaledTree = (SymbolicExpressionTree)original.Clone();
      // insert main branch into the original tree again 
      original.Root.SubTrees[0].InsertSubTree(0, mainBranch);
      // insert the scaled main branch into the cloned tree
      scaledTree.Root.SubTrees[0].InsertSubTree(0, scaledMainBranch);
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
