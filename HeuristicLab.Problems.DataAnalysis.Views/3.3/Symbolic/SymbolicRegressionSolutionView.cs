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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Views.Symbolic {
  [Content(typeof(SymbolicRegressionSolution))]
  [View("Symbolic Regression Solution View")]
  public partial class SymbolicRegressionSolutionView : DataAnalysisSolutionView {
    public SymbolicRegressionSolutionView() {
      InitializeComponent();
    }

    protected new SymbolicRegressionSolution Content {
      get { return (SymbolicRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    private void btn_SimplifyModel_Click(object sender, EventArgs e) {
      InteractiveSymbolicRegressionSolutionSimplifierView view = new InteractiveSymbolicRegressionSolutionSimplifierView();
      view.Content = (SymbolicRegressionSolution)this.Content.Clone();
      view.Show();
    }
  }
}
