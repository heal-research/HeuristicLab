#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification.Views {
  public abstract partial class InteractiveSymbolicClassificationSolutionSimplifierViewBase : InteractiveSymbolicDataAnalysisSolutionSimplifierView {
    public new ISymbolicClassificationSolution Content {
      get { return (ISymbolicClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    protected InteractiveSymbolicClassificationSolutionSimplifierViewBase()
      : base(new SymbolicClassificationSolutionImpactValuesCalculator()) {
      InitializeComponent();
      this.Caption = "Interactive Classification Solution Simplifier";
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
  }
}
