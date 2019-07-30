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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public class SymbolicExpressionTreeBacktransformator : IModelBacktransformator {
    private readonly ITransformationMapper<ISymbolicExpressionTreeNode> transformationMapper;

    public SymbolicExpressionTreeBacktransformator(ITransformationMapper<ISymbolicExpressionTreeNode> transformationMapper) {
      this.transformationMapper = transformationMapper;
    }

    public ISymbolicDataAnalysisModel Backtransform(ISymbolicDataAnalysisModel model, IEnumerable<ITransformation> transformations, string targetVariable) {
      var symbolicModel = (ISymbolicDataAnalysisModel)model.Clone();

      foreach (var transformation in transformations.Reverse()) {
        ApplyBacktransformation(transformation, symbolicModel.SymbolicExpressionTree, targetVariable);
      }

      return symbolicModel;
    }

    private void ApplyBacktransformation(ITransformation transformation, ISymbolicExpressionTree symbolicExpressionTree, string targetVariable) {
      if (transformation.Column != targetVariable) {
        var variableNodes = symbolicExpressionTree.IterateNodesBreadth()
          .OfType<VariableTreeNode>()
          .Where(n => n.VariableName == transformation.Column);
        ApplyRegularBacktransformation(transformation, variableNodes);
      } else if (!(transformation is CopyColumnTransformation)) {
        ApplyInverseBacktransformation(transformation, symbolicExpressionTree);
      }
    }

    private void ApplyRegularBacktransformation(ITransformation transformation, IEnumerable<VariableTreeNode> variableNodes) {
      foreach (var variableNode in variableNodes) {
        // generate new subtrees because same subtree cannot be added more than once
        var transformationTree = transformationMapper.GenerateModel(transformation);
        SwapVariableWithTree(variableNode, transformationTree);
      }
    }

    private void ApplyInverseBacktransformation(ITransformation transformation, ISymbolicExpressionTree symbolicExpressionTree) {
      var startSymbol = symbolicExpressionTree.Root.GetSubtree(0);
      var modelTree = startSymbol.GetSubtree(0);
      startSymbol.RemoveSubtree(0);

      var transformationTree = transformationMapper.GenerateInverseModel(transformation);
      var variableNode = transformationTree.IterateNodesBreadth()
        .OfType<VariableTreeNode>()
        .Single(n => n.VariableName == transformation.Column);

      SwapVariableWithTree(variableNode, modelTree);

      startSymbol.AddSubtree(transformationTree);
    }

    private void SwapVariableWithTree(VariableTreeNode variableNode, ISymbolicExpressionTreeNode treeNode) {
      var parent = variableNode.Parent;
      int index = parent.IndexOfSubtree(variableNode);
      parent.RemoveSubtree(index);

      if (!variableNode.Weight.IsAlmost(1.0))
        treeNode = CreateNodeFromWeight(treeNode, variableNode);

      parent.InsertSubtree(index, treeNode);
    }

    private ISymbolicExpressionTreeNode CreateNodeFromWeight(ISymbolicExpressionTreeNode transformationTree, VariableTreeNode variableNode) {
      var multiplicationNode = new SymbolicExpressionTreeNode(new Multiplication());
      multiplicationNode.AddSubtree(new ConstantTreeNode(new Constant()) { Value = variableNode.Weight });
      multiplicationNode.AddSubtree(transformationTree);
      return multiplicationNode;
    }
  }
}
