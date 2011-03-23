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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Optimization;
using System;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  /// <summary>
  /// Represents a symbolic classification solution (model + data) and attributes of the solution like accuracy and complexity
  /// </summary>
  [StorableClass]
  [Item(Name = "SymbolicDiscriminantFunctionClassificationSolution", Description = "Represents a symbolic classification solution (model + data) and attributes of the solution like accuracy and complexity.")]
  public sealed class SymbolicDiscriminantFunctionClassificationSolution : DiscriminantFunctionClassificationSolution, ISymbolicClassificationSolution {
    private const string ModelLengthResultName = "ModelLength";
    private const string ModelDepthResultName = "ModelDepth";

    public new ISymbolicDiscriminantFunctionClassificationModel Model {
      get { return (ISymbolicDiscriminantFunctionClassificationModel)base.Model; }
      set { base.Model = value; }
    }

    ISymbolicClassificationModel ISymbolicClassificationSolution.Model {
      get { return Model; }
    }

    ISymbolicDataAnalysisModel ISymbolicDataAnalysisSolution.Model {
      get { return Model; }
    }
    public int ModelLength {
      get { return ((IntValue)this[ModelLengthResultName].Value).Value; }
      private set { ((IntValue)this[ModelLengthResultName].Value).Value = value; }
    }

    public int ModelDepth {
      get { return ((IntValue)this[ModelDepthResultName].Value).Value; }
      private set { ((IntValue)this[ModelDepthResultName].Value).Value = value; }
    }
    [StorableConstructor]
    private SymbolicDiscriminantFunctionClassificationSolution(bool deserializing) : base(deserializing) { }
    private SymbolicDiscriminantFunctionClassificationSolution(SymbolicDiscriminantFunctionClassificationSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDiscriminantFunctionClassificationSolution(ISymbolicDiscriminantFunctionClassificationModel model, IClassificationProblemData problemData)
      : base(model, problemData) {
      Add(new Result(ModelLengthResultName, "Length of the symbolic classification model.", new IntValue()));
      Add(new Result(ModelDepthResultName, "Depth of the symbolic classification model.", new IntValue()));
      RecalculateResults();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDiscriminantFunctionClassificationSolution(this, cloner);
    }

    protected override void OnModelChanged(EventArgs e) {
      base.OnModelChanged(e);
      RecalculateResults();
    }

    private new void RecalculateResults() {
      ModelLength = Model.SymbolicExpressionTree.Length;
      ModelDepth = Model.SymbolicExpressionTree.Depth;
    }

    public void ScaleModel() {
      var dataset = ProblemData.Dataset;
      var targetVariable = ProblemData.TargetVariable;
      var rows = ProblemData.TrainingIndizes;
      var estimatedValues = GetEstimatedValues(rows);
      var targetValues = dataset.GetEnumeratedVariableValues(targetVariable, rows);
      double alpha;
      double beta;
      OnlineLinearScalingParameterCalculator.Calculate(estimatedValues, targetValues, out alpha, out beta);

      ConstantTreeNode alphaTreeNode = null;
      ConstantTreeNode betaTreeNode = null;
      // check if model has been scaled previously by analyzing the structure of the tree
      var startNode = Model.SymbolicExpressionTree.Root.GetSubtree(0);
      if (startNode.GetSubtree(0).Symbol is Addition) {
        var addNode = startNode.GetSubtree(0);
        if (addNode.SubtreesCount == 2 && addNode.GetSubtree(0).Symbol is Multiplication && addNode.GetSubtree(1).Symbol is Constant) {
          alphaTreeNode = addNode.GetSubtree(1) as ConstantTreeNode;
          var mulNode = addNode.GetSubtree(0);
          if (mulNode.SubtreesCount == 2 && mulNode.GetSubtree(1).Symbol is Constant) {
            betaTreeNode = mulNode.GetSubtree(1) as ConstantTreeNode;
          }
        }
      }
      // if tree structure matches the structure necessary for linear scaling then reuse the existing tree nodes
      if (alphaTreeNode != null && betaTreeNode != null) {
        betaTreeNode.Value *= beta;
        alphaTreeNode.Value *= beta;
        alphaTreeNode.Value += alpha;
      } else {
        var mainBranch = startNode.GetSubtree(0);
        startNode.RemoveSubtree(0);
        var scaledMainBranch = MakeSum(MakeProduct(beta, mainBranch), alpha);
        startNode.AddSubtree(scaledMainBranch);
      }

      OnModelChanged(EventArgs.Empty);
    }

    private static ISymbolicExpressionTreeNode MakeSum(ISymbolicExpressionTreeNode treeNode, double alpha) {
      if (alpha.IsAlmost(0.0)) {
        return treeNode;
      } else {
        var node = (new Addition()).CreateTreeNode();
        var alphaConst = MakeConstant(alpha);
        node.AddSubtree(treeNode);
        node.AddSubtree(alphaConst);
        return node;
      }
    }

    private static ISymbolicExpressionTreeNode MakeProduct(double beta, ISymbolicExpressionTreeNode treeNode) {
      if (beta.IsAlmost(1.0)) {
        return treeNode;
      } else {
        var node = (new Multiplication()).CreateTreeNode();
        var betaConst = MakeConstant(beta);
        node.AddSubtree(treeNode);
        node.AddSubtree(betaConst);
        return node;
      }
    }

    private static ISymbolicExpressionTreeNode MakeConstant(double c) {
      var node = (ConstantTreeNode)(new Constant()).CreateTreeNode();
      node.Value = c;
      return node;
    }
  }
}
