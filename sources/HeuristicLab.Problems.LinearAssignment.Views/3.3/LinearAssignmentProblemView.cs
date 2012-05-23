#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization.Views;

namespace HeuristicLab.Problems.LinearAssignment.Views {
  [View("LinearAssignmentProblemView")]
  [Content(typeof(LinearAssignmentProblem), IsDefaultView = true)]
  public partial class LinearAssignmentProblemView : ProblemView {
    public new LinearAssignmentProblem Content {
      get { return (LinearAssignmentProblem)base.Content; }
      set { base.Content = value; }
    }

    public LinearAssignmentProblemView() {
      InitializeComponent();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      solveButton.Enabled = Content != null && !ReadOnly && !Locked;
    }

    private void solveButton_Click(object sender, System.EventArgs e) {
      ReadOnly = true;
      Locked = true;
      backgroundWorker.RunWorkerAsync();
    }

    private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      double quality;
      var solution = LinearAssignmentProblemSolver.Solve(Content.Costs, out quality);
      Content.Solution = new IntArray(solution);
      Content.Quality = new DoubleValue(quality);
    }

    private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      Locked = false;
      ReadOnly = false;
    }
  }
}
