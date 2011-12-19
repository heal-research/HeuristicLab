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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Views;

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
      // the default policy for setting thresholds in classification models is the accuarcy maximizing policy.
      // This is rather slow to calculate and can lead to a very laggy UI in the interactive solution simplifier.
      // However, since we automatically prune sub-trees based on the threshold reaching the maximum accuracy we must
      // also use maximum accuracy threshold calculation here in order to prevent incoherent behavior of the simplifier.
      Content.SetAccuracyMaximizingThresholds();
    }

    protected override Dictionary<ISymbolicExpressionTreeNode, double> CalculateReplacementValues(ISymbolicExpressionTree tree) {
      Dictionary<ISymbolicExpressionTreeNode, double> replacementValues = new Dictionary<ISymbolicExpressionTreeNode, double>();
      foreach (ISymbolicExpressionTreeNode node in tree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPrefix()) {
        replacementValues[node] = CalculateReplacementValue(node, tree);
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

      var targetClassValues = dataset.GetDoubleValues(targetVariable, rows);
      var originalOutput = interpreter.GetSymbolicExpressionTreeValues(tree, dataset, rows)
        .LimitToRange(Content.Model.LowerEstimationLimit, Content.Model.UpperEstimationLimit)
        .ToArray();
      double[] classValues;
      double[] thresholds;
      // normal distribution cut points are used as thresholds here because they are a lot faster to calculate than the accuracy maximizing thresholds
      NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(Content.ProblemData, originalOutput, targetClassValues, out classValues, out thresholds);
      var classifier = new SymbolicDiscriminantFunctionClassificationModel(tree, interpreter);
      classifier.SetThresholdsAndClassValues(thresholds, classValues);
      OnlineCalculatorError errorState;
      double originalAccuracy = OnlineAccuracyCalculator.Calculate(targetClassValues, classifier.GetEstimatedClassValues(dataset, rows), out errorState);
      if (errorState != OnlineCalculatorError.None) originalAccuracy = 0.0;

      foreach (ISymbolicExpressionTreeNode node in nodes) {
        var parent = node.Parent;
        constantNode.Value = CalculateReplacementValue(node, tree);
        ISymbolicExpressionTreeNode replacementNode = constantNode;
        SwitchNode(parent, node, replacementNode);
        var newOutput = interpreter.GetSymbolicExpressionTreeValues(tree, dataset, rows)
          .LimitToRange(Content.Model.LowerEstimationLimit, Content.Model.UpperEstimationLimit)
          .ToArray();
        NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(Content.ProblemData, newOutput, targetClassValues, out classValues, out thresholds);
        classifier = new SymbolicDiscriminantFunctionClassificationModel(tree, interpreter);
        classifier.SetThresholdsAndClassValues(thresholds, classValues);
        double newAccuracy = OnlineAccuracyCalculator.Calculate(targetClassValues, classifier.GetEstimatedClassValues(dataset, rows), out errorState);
        if (errorState != OnlineCalculatorError.None) newAccuracy = 0.0;

        // impact = 0 if no change
        // impact < 0 if new solution is better
        // impact > 0 if new solution is worse
        impactValues[node] = originalAccuracy - newAccuracy;
        SwitchNode(parent, replacementNode, node);
      }
      return impactValues;
    }

    private double CalculateReplacementValue(ISymbolicExpressionTreeNode node, ISymbolicExpressionTree sourceTree) {
      // remove old ADFs
      while (tempTree.Root.SubtreeCount > 1) tempTree.Root.RemoveSubtree(1);
      // clone ADFs of source tree
      for (int i = 1; i < sourceTree.Root.SubtreeCount; i++) {
        tempTree.Root.AddSubtree((ISymbolicExpressionTreeNode)sourceTree.Root.GetSubtree(i).Clone());
      }
      var start = tempTree.Root.GetSubtree(0);
      while (start.SubtreeCount > 0) start.RemoveSubtree(0);
      start.AddSubtree((ISymbolicExpressionTreeNode)node.Clone());
      var interpreter = Content.Model.Interpreter;
      var rows = Content.ProblemData.TrainingIndizes;
      return interpreter.GetSymbolicExpressionTreeValues(tempTree, Content.ProblemData.Dataset, rows).Median();
    }


    private void SwitchNode(ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode oldBranch, ISymbolicExpressionTreeNode newBranch) {
      for (int i = 0; i < root.SubtreeCount; i++) {
        if (root.GetSubtree(i) == oldBranch) {
          root.RemoveSubtree(i);
          root.InsertSubtree(i, newBranch);
          return;
        }
      }
    }

    protected override void btnOptimizeConstants_Click(object sender, EventArgs e) {

    }
  }
}
