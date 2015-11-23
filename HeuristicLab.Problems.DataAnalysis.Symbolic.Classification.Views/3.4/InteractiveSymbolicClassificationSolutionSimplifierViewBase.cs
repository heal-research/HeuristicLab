#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public abstract partial class InteractiveSymbolicClassificationSolutionSimplifierViewBase : InteractiveSymbolicDataAnalysisSolutionSimplifierView {
    private readonly SymbolicClassificationSolutionImpactValuesCalculator calculator;

    public new ISymbolicClassificationSolution Content {
      get { return (ISymbolicClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    public InteractiveSymbolicClassificationSolutionSimplifierViewBase()
      : base() {
      InitializeComponent();
      this.Caption = "Interactive Classification Solution Simplifier";

      calculator = new SymbolicClassificationSolutionImpactValuesCalculator();
    }

    /// <summary>
    /// It is necessary to create new models of an unknown type with new trees in the simplifier.
    /// For this purpose the cloner is used by registering the new tree as already cloned object and invoking the clone mechanism.
    /// This results in a new model of the same type as the old one with an exchanged tree.
    /// </summary>
    /// <param name="tree">The new tree that should be included in the new object</param>
    /// <returns></returns>
    protected ISymbolicClassificationModel CreateModel(ISymbolicExpressionTree tree) {
      var cloner = new Cloner();
      cloner.RegisterClonedObject(Content.Model.SymbolicExpressionTree, tree);

      var model = (ISymbolicClassificationModel)Content.Model.Clone(cloner);
      model.RecalculateModelParameters(Content.ProblemData, Content.ProblemData.TrainingIndices);
      return model;
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
        double impactValue, replacementValue, newQualityForImpactsCalculation;
        calculator.CalculateImpactAndReplacementValues(Content.Model, node, Content.ProblemData, Content.ProblemData.TrainingIndices, out impactValue, out replacementValue, out newQualityForImpactsCalculation);
        impactAndReplacementValues.Add(node, new Tuple<double, double>(impactValue, replacementValue));
      }
      return impactAndReplacementValues;
    }

    protected override void btnOptimizeConstants_Click(object sender, EventArgs e) {

    }
  }
}
