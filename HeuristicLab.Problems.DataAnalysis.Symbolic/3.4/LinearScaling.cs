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
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class LinearScaling {

    public static ISymbolicExpressionTree AddLinearScalingTerms(ISymbolicExpressionTree tree, double offset = 0.0, double scale = 1.0) {
      var startNode = tree.Root.Subtrees.First();
      var template = startNode.Subtrees.First();

      var addNode = new Addition().CreateTreeNode();
      var mulNode = new Multiplication().CreateTreeNode();
      var offsetNode = new NumberTreeNode(offset);
      var scaleNode = new NumberTreeNode(scale);

      addNode.AddSubtree(offsetNode);
      addNode.AddSubtree(mulNode);
      mulNode.AddSubtree(scaleNode);

      startNode.RemoveSubtree(0);
      startNode.AddSubtree(addNode);
      mulNode.AddSubtree(template);
      return tree;
    }

    public static void RemoveLinearScalingTerms(ISymbolicExpressionTree tree) {
      var startNode = tree.Root.GetSubtree(0);
      ExtractScalingTerms(tree, out NumberTreeNode offsetNode, out NumberTreeNode scaleNode);

      var evaluationNode = scaleNode.Parent.GetSubtree(1); //move up to multiplication and take second child
      startNode.RemoveSubtree(0);
      startNode.AddSubtree(evaluationNode);
    }

    public static void ExtractScalingTerms(ISymbolicExpressionTree tree,
      out NumberTreeNode offset, out NumberTreeNode scale) {
      var startNode = tree.Root.Subtrees.First();

      //check for scaling terms
      var addNode = startNode.GetSubtree(0);
      var offsetNode = addNode.GetSubtree(0);
      var mulNode = addNode.GetSubtree(1);
      var scaleNode = mulNode.GetSubtree(0);


      var error = false;
      if (addNode.Symbol is not Addition) error = true;
      if (mulNode.Symbol is not Multiplication) error = true;
      if (offsetNode is not NumberTreeNode) error = true;
      if (scaleNode is not NumberTreeNode) error = true;
      if (error) throw new ArgumentException("Scaling terms cannot be found.");

      offset = (NumberTreeNode)offsetNode;
      scale = (NumberTreeNode)scaleNode;
    }

    public static void AdjustLinearScalingParams(IRegressionProblemData problemData, ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter) {
      ExtractScalingTerms(tree, out NumberTreeNode offsetNode, out NumberTreeNode scaleNode);

      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, problemData.TrainingIndices);
      var targetValues = problemData.TargetVariableTrainingValues;

      OnlineLinearScalingParameterCalculator.Calculate(estimatedValues, targetValues, out double a, out double b, out OnlineCalculatorError error);
      if (error == OnlineCalculatorError.None) {
        offsetNode.Value = a;
        scaleNode.Value = b;
      }
    }
  }
}
