#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis.Views {
  public partial class InteractiveSymbolicTimeSeriesPrognosisSolutionSimplifierView : InteractiveSymbolicDataAnalysisSolutionSimplifierView {
    public new SymbolicTimeSeriesPrognosisSolution Content {
      get { return (SymbolicTimeSeriesPrognosisSolution)base.Content; }
      set { base.Content = value; }
    }

    public InteractiveSymbolicTimeSeriesPrognosisSolutionSimplifierView()
      : base(new SymbolicRegressionSolutionImpactValuesCalculator()) {
      InitializeComponent();
      this.Caption = "Interactive Time-Series Prognosis Solution Simplifier";
    }

    protected override void UpdateModel(ISymbolicExpressionTree tree) {
      var model = new SymbolicTimeSeriesPrognosisModel(Content.ProblemData.TargetVariable, tree, Content.Model.Interpreter);
      model.Scale(Content.ProblemData);
      Content.Model = model;
    }
  }
}
