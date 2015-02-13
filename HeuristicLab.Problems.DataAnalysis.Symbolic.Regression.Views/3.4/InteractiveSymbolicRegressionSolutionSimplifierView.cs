#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.DataAnalysis.Symbolic.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression.Views {
  public partial class InteractiveSymbolicRegressionSolutionSimplifierView : InteractiveSymbolicDataAnalysisSolutionSimplifierView {
    private readonly SymbolicRegressionSolutionImpactValuesCalculator calculator;

    public new SymbolicRegressionSolution Content {
      get { return (SymbolicRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    public InteractiveSymbolicRegressionSolutionSimplifierView()
      : base() {
      InitializeComponent();
      this.Caption = "Interactive Regression Solution Simplifier";
      calculator = new SymbolicRegressionSolutionImpactValuesCalculator();
    }

    protected override void UpdateModel(ISymbolicExpressionTree tree) {
      var model = new SymbolicRegressionModel(tree, Content.Model.Interpreter, Content.Model.LowerEstimationLimit, Content.Model.UpperEstimationLimit);
      model.Scale(Content.ProblemData);
      Content.Model = model;
    }

    protected override Dictionary<ISymbolicExpressionTreeNode, double> CalculateReplacementValues(ISymbolicExpressionTree tree) {
      return tree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPrefix().ToDictionary(
        n => n,
        n => calculator.CalculateReplacementValue(Content.Model, n, Content.ProblemData, Content.ProblemData.TrainingIndices)
        );
    }

    protected override Dictionary<ISymbolicExpressionTreeNode, double> CalculateImpactValues(ISymbolicExpressionTree tree) {
      var values = CalculateImpactAndReplacementValues(tree);
      return values.ToDictionary(x => x.Key, x => x.Value.Item1);
    }

    protected override Dictionary<ISymbolicExpressionTreeNode, Tuple<double, double>> CalculateImpactAndReplacementValues(ISymbolicExpressionTree tree) {
      var impactAndReplacementValues = new Dictionary<ISymbolicExpressionTreeNode, Tuple<double, double>>();
      foreach (var node in tree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPrefix()) {
        double impactValue, replacementValue;
        calculator.CalculateImpactAndReplacementValues(Content.Model, node, Content.ProblemData, Content.ProblemData.TrainingIndices, out impactValue, out replacementValue);
        impactAndReplacementValues.Add(node, new Tuple<double, double>(impactValue, replacementValue));
      }
      return impactAndReplacementValues;
    }

    protected override void btnOptimizeConstants_Click(object sender, EventArgs e) {
      var model = Content.Model;
      SymbolicRegressionConstantOptimizationEvaluator.OptimizeConstants(model.Interpreter, model.SymbolicExpressionTree, Content.ProblemData, Content.ProblemData.TrainingIndices,
        applyLinearScaling: true, maxIterations: 50, upperEstimationLimit: model.UpperEstimationLimit, lowerEstimationLimit: model.LowerEstimationLimit);
      UpdateModel(Content.Model.SymbolicExpressionTree);
    }
  }
}
