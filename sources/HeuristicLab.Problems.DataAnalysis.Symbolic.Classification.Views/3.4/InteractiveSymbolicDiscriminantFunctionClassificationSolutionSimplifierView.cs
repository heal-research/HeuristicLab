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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Views;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification.Views {
  public partial class InteractiveSymbolicDiscriminantFunctionClassificationSolutionSimplifierView : InteractiveSymbolicDataAnalysisSolutionSimplifierView {
    private readonly ConstantTreeNode constantNode;
    private readonly SymbolicExpressionTree tempTree;

    public new SymbolicDiscriminantFunctionClassificationSolution Content {
      get { return (SymbolicDiscriminantFunctionClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    public InteractiveSymbolicDiscriminantFunctionClassificationSolutionSimplifierView()
      : base() {
      InitializeComponent();
      this.Caption = "Interactive Classification Solution Simplifier";

      constantNode = ((ConstantTreeNode)new Constant().CreateTreeNode());
      ISymbolicExpressionTreeNode root = new ProgramRootSymbol().CreateTreeNode();
      ISymbolicExpressionTreeNode start = new StartSymbol().CreateTreeNode();
      root.AddSubtree(start);
      tempTree = new SymbolicExpressionTree(root);
    }

    protected override void UpdateModel(ISymbolicExpressionTree tree) {
      Content.Model = new SymbolicDiscriminantFunctionClassificationModel(tree, Content.Model.Interpreter);
      Content.SetClassDistibutionCutPointThresholds();
    }

    protected override Dictionary<ISymbolicExpressionTreeNode, double> CalculateReplacementValues(ISymbolicExpressionTree tree) {
      Dictionary<ISymbolicExpressionTreeNode, double> replacementValues = new Dictionary<ISymbolicExpressionTreeNode, double>();
      foreach (ISymbolicExpressionTreeNode node in tree.IterateNodesPrefix()) {
        if (!(node.Symbol is ProgramRootSymbol || node.Symbol is StartSymbol)) {
          replacementValues[node] = CalculateReplacementValue(node);
        }
      }
      return replacementValues;
    }

    protected override Dictionary<ISymbolicExpressionTreeNode, double> CalculateImpactValues(ISymbolicExpressionTree tree) {
      var interpreter = Content.Model.Interpreter;
      var dataset = Content.ProblemData.Dataset;
      var rows = Content.ProblemData.TrainingIndizes;
      string targetVariable = Content.ProblemData.TargetVariable;
      Dictionary<ISymbolicExpressionTreeNode, double> impactValues = new Dictionary<ISymbolicExpressionTreeNode, double>();
      List<ISymbolicExpressionTreeNode> nodes = tree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPostfix().ToList();

      var targetClassValues = dataset.GetEnumeratedVariableValues(targetVariable, rows);
      var originalOutput = interpreter.GetSymbolicExpressionTreeValues(tree, dataset, rows)
        .LimitToRange(Content.Model.LowerEstimationLimit, Content.Model.UpperEstimationLimit)
        .ToArray();
      double[] classValues;
      double[] thresholds;
      NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(Content.ProblemData, originalOutput, targetClassValues, out classValues, out thresholds);
      var classifier = new SymbolicDiscriminantFunctionClassificationModel(tree, interpreter);
      classifier.SetThresholdsAndClassValues(thresholds, classValues);
      double originalAccuracy = OnlineAccuracyEvaluator.Calculate(targetClassValues, classifier.GetEstimatedClassValues(dataset, rows));

      foreach (ISymbolicExpressionTreeNode node in nodes) {
        var parent = node.Parent;
        constantNode.Value = CalculateReplacementValue(node);
        ISymbolicExpressionTreeNode replacementNode = constantNode;
        SwitchNode(parent, node, replacementNode);
        var newOutput = interpreter.GetSymbolicExpressionTreeValues(tree, dataset, rows)
          .LimitToRange(Content.Model.LowerEstimationLimit, Content.Model.UpperEstimationLimit)
          .ToArray();
        NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(Content.ProblemData, newOutput, targetClassValues, out classValues, out thresholds);
        classifier = new SymbolicDiscriminantFunctionClassificationModel(tree, interpreter);
        classifier.SetThresholdsAndClassValues(thresholds, classValues);
        double newAccuracy = OnlineAccuracyEvaluator.Calculate(targetClassValues, classifier.GetEstimatedClassValues(dataset, rows));

        // impact = 0 if no change
        // impact < 0 if new solution is better
        // impact > 0 if new solution is worse
        impactValues[node] = originalAccuracy - newAccuracy;
        SwitchNode(parent, replacementNode, node);
      }
      return impactValues;
    }

    private double CalculateReplacementValue(ISymbolicExpressionTreeNode node) {
      var start = tempTree.Root.GetSubtree(0);
      while (start.SubtreesCount > 0) start.RemoveSubtree(0);
      start.AddSubtree((ISymbolicExpressionTreeNode)node.Clone());
      var interpreter = Content.Model.Interpreter;
      var rows = Content.ProblemData.TrainingIndizes;
      return interpreter.GetSymbolicExpressionTreeValues(tempTree, Content.ProblemData.Dataset, rows).Median();
    }


    private void SwitchNode(ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode oldBranch, ISymbolicExpressionTreeNode newBranch) {
      for (int i = 0; i < root.SubtreesCount; i++) {
        if (root.GetSubtree(i) == oldBranch) {
          root.RemoveSubtree(i);
          root.InsertSubtree(i, newBranch);
          return;
        }
      }
    }
  }
}
