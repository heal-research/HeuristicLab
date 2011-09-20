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
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis.Views {
  [Content(typeof(SymbolicTimeSeriesPrognosisSolution), false)]
  [View("SymbolicTimeSeriesPrognosisSolution View")]
  public partial class SymbolicTimeSeriesPrognosisSolutionView : TimeSeriesPrognosisSolutionView {
    public SymbolicTimeSeriesPrognosisSolutionView() {
      InitializeComponent();
    }

    protected new SymbolicTimeSeriesPrognosisSolution Content {
      get { return (SymbolicTimeSeriesPrognosisSolution)base.Content; }
      set { base.Content = value; }
    }

    private void btn_SimplifyModel_Click(object sender, EventArgs e) {
      InteractiveSymbolicTimeSeriesPrognosisSolutionSimplifierView view = new InteractiveSymbolicTimeSeriesPrognosisSolutionSimplifierView();
      view.Content = (SymbolicTimeSeriesPrognosisSolution)this.Content.Clone();
      view.Show();
    }
  }
}
