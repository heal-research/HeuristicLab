#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.Optimization.Views;

namespace HeuristicLab.Problems.Orienteering.Views {
  [View("Orienteering Problem View")]
  [Content(typeof(OrienteeringProblem), true)]
  public partial class OrienteeringProblemView : ProblemView {
    public new OrienteeringProblem Content {
      get { return (OrienteeringProblem)base.Content; }
      set { base.Content = value; }
    }

    public OrienteeringProblemView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.BestKnownSolutionParameter.ValueChanged -= BestKnownSolutionParameter_ValueChanged;
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.BestKnownSolutionParameter.ValueChanged += BestKnownSolutionParameter_ValueChanged;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        orienteeringSolutionView.Content = null;
      } else {
        orienteeringSolutionView.Content = Content.BestKnownSolution;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      orienteeringSolutionView.Enabled = Content != null && !ReadOnly && !Locked;
    }

    private void BestKnownSolutionParameter_ValueChanged(object sender, EventArgs e) {
      orienteeringSolutionView.Content = Content.BestKnownSolution;
    }
  }
}
