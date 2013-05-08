#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis.Views {
  public partial class InteractiveSymbolicTimeSeriesPrognosisSolutionSimplifierView : InteractiveSymbolicDataAnalysisSolutionSimplifierView {
    private readonly ConstantTreeNode constantNode;
    private readonly SymbolicExpressionTree tempTree;

    public new SymbolicTimeSeriesPrognosisSolution Content {
      get { return (SymbolicTimeSeriesPrognosisSolution)base.Content; }
      set { base.Content = value; }
    }

    public InteractiveSymbolicTimeSeriesPrognosisSolutionSimplifierView()
      : base() {
      InitializeComponent();
      this.Caption = "Interactive Time-Series Prognosis Solution Simplifier";

      constantNode = ((ConstantTreeNode)new Constant().CreateTreeNode());
      ISymbolicExpressionTreeNode root = new ProgramRootSymbol().CreateTreeNode();
      ISymbolicExpressionTreeNode start = new StartSymbol().CreateTreeNode();
      root.AddSubtree(start);
      tempTree = new SymbolicExpressionTree(root);
    }

    protected override void UpdateModel(ISymbolicExpressionTree tree) {
      var model = new SymbolicTimeSeriesPrognosisModel(tree, Content.Model.Interpreter);
      model.Scale(Content.ProblemData);
      Content.Model = model;
    }

    protected override Dictionary<ISymbolicExpressionTreeNode, double> CalculateReplacementValues(ISymbolicExpressionTree tree) {
      Dictionary<ISymbolicExpressionTreeNode, double> replacementValues = new Dictionary<ISymbolicExpressionTreeNode, double>();
      foreach (var componentBranch in tree.Root.GetSubtree(0).Subtrees)
        foreach (ISymbolicExpressionTreeNode node in componentBranch.IterateNodesPrefix()) {
          replacementValues[node] = CalculateReplacementValue(node, tree);
        }
      return replacementValues;
    }

    protected override Dictionary<ISymbolicExpressionTreeNode, double> CalculateImpactValues(ISymbolicExpressionTree tree) {
      var interpreter = Content.Model.Interpreter;
      var rows = Content.ProblemData.TrainingIndices;
      var dataset = Content.ProblemData.Dataset;
      var targetVariable = Content.ProblemData.TargetVariable;
      var targetValues = dataset.GetDoubleValues(targetVariable, rows);
      var originalOutput = interpreter.GetSymbolicExpressionTreeValues(tree, dataset, rows).ToArray();

      Dictionary<ISymbolicExpressionTreeNode, double> impactValues = new Dictionary<ISymbolicExpressionTreeNode, double>();
      List<ISymbolicExpressionTreeNode> nodes = tree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPostfix().ToList();
      OnlineCalculatorError errorState;
      double originalR2 = OnlinePearsonsRSquaredCalculator.Calculate(targetValues, originalOutput, out errorState);
      if (errorState != OnlineCalculatorError.None) originalR2 = 0.0;

      foreach (ISymbolicExpressionTreeNode node in nodes) {
        var parent = node.Parent;
        constantNode.Value = CalculateReplacementValue(node, tree);
        ISymbolicExpressionTreeNode replacementNode = constantNode;
        SwitchNode(parent, node, replacementNode);
        var newOutput = interpreter.GetSymbolicExpressionTreeValues(tree, dataset, rows);
        double newR2 = OnlinePearsonsRSquaredCalculator.Calculate(targetValues, newOutput, out errorState);
        if (errorState != OnlineCalculatorError.None) newR2 = 0.0;

        // impact = 0 if no change
        // impact < 0 if new solution is better
        // impact > 0 if new solution is worse
        impactValues[node] = originalR2 - newR2;
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
      var rows = Content.ProblemData.TrainingIndices;
      var allPrognosedValues = interpreter.GetSymbolicExpressionTreeValues(tempTree, Content.ProblemData.Dataset, rows);

      return allPrognosedValues.Median();
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
      throw new NotImplementedException();
    }
  }
}
