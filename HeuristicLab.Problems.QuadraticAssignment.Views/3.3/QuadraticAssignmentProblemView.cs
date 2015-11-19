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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization.Views;

namespace HeuristicLab.Problems.QuadraticAssignment.Views {
  [View("Quadratic Assignment Problem View")]
  [Content(typeof(QuadraticAssignmentProblem), IsDefaultView = true)]
  public sealed partial class QuadraticAssignmentProblemView : ProblemView {
    public new QuadraticAssignmentProblem Content {
      get { return (QuadraticAssignmentProblem)base.Content; }
      set { base.Content = value; }
    }

    public QuadraticAssignmentProblemView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.DistancesParameter.ValueChanged += new EventHandler(DistanceMatrixParameter_ValueChanged);
      Content.WeightsParameter.ValueChanged += new EventHandler(WeightsParameter_ValueChanged);
      Content.BestKnownSolutionParameter.ValueChanged += new EventHandler(BestKnownSolutionParameter_ValueChanged);
    }

    protected override void DeregisterContentEvents() {
      Content.DistancesParameter.ValueChanged -= new EventHandler(DistanceMatrixParameter_ValueChanged);
      Content.WeightsParameter.ValueChanged -= new EventHandler(WeightsParameter_ValueChanged);
      Content.BestKnownSolutionParameter.ValueChanged -= new EventHandler(BestKnownSolutionParameter_ValueChanged);
      base.DeregisterContentEvents();
    }

    private void DistanceMatrixParameter_ValueChanged(object sender, EventArgs e) {
      qapView.Distances = Content.Distances;
    }

    private void WeightsParameter_ValueChanged(object sender, EventArgs e) {
      qapView.Weights = Content.Weights;
    }

    private void BestKnownSolutionParameter_ValueChanged(object sender, EventArgs e) {
      qapView.Assignment = Content.BestKnownSolution;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        qapView.Distances = Content.Distances;
        qapView.Weights = Content.Weights;
        qapView.Assignment = Content.BestKnownSolution;
      } else {
        qapView.Distances = null;
        qapView.Weights = null;
        qapView.Assignment = null;
      }
    }
  }
}
