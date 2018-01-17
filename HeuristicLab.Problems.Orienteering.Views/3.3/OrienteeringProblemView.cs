#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      Content.CoordinatesParameter.ValueChanged -= CoordinatesParameter_ValueChanged;
      Content.StartingPointParameter.ValueChanged -= StartingPointParameter_ValueChanged;
      Content.TerminalPointParameter.ValueChanged -= TerminalPointParameter_ValueChanged;
      Content.ScoresParameter.ValueChanged -= ScoresParameter_ValueChanged;
      Content.BestKnownQualityParameter.ValueChanged -= BestKnownQualityParameter_ValueChanged;
      Content.BestKnownSolutionParameter.ValueChanged -= BestKnownSolutionParameter_ValueChanged;
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CoordinatesParameter.ValueChanged += CoordinatesParameter_ValueChanged;
      Content.StartingPointParameter.ValueChanged += StartingPointParameter_ValueChanged;
      Content.TerminalPointParameter.ValueChanged += TerminalPointParameter_ValueChanged;
      Content.ScoresParameter.ValueChanged += ScoresParameter_ValueChanged;
      Content.BestKnownQualityParameter.ValueChanged += BestKnownQualityParameter_ValueChanged;
      Content.BestKnownSolutionParameter.ValueChanged += BestKnownSolutionParameter_ValueChanged;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        orienteeringSolutionView.Content = null;
      } else {
        orienteeringSolutionView.Content = new OrienteeringSolution(Content.BestKnownSolution,
          Content.Coordinates, Content.StartingPointParameter.Value, Content.TerminalPointParameter.Value, Content.Scores);
        if (Content.BestKnownSolution != null) {
          EvaluateBestSolution();
        }
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      orienteeringSolutionView.Enabled = Content != null;
    }

    private void CoordinatesParameter_ValueChanged(object sender, EventArgs e) {
      orienteeringSolutionView.Content.Coordinates = Content.Coordinates;
    }

    private void StartingPointParameter_ValueChanged(object sender, EventArgs e) {
      orienteeringSolutionView.Content.StartingPoint.Value = Content.StartingPoint;
    }

    private void TerminalPointParameter_ValueChanged(object sender, EventArgs e) {
      orienteeringSolutionView.Content.TerminalPoint.Value = Content.TerminalPoint;
    }

    private void ScoresParameter_ValueChanged(object sender, EventArgs e) {
      orienteeringSolutionView.Content.Scores = Content.Scores;
    }

    private void BestKnownQualityParameter_ValueChanged(object sender, EventArgs e) {
      orienteeringSolutionView.Content.Quality = Content.BestKnownQuality;
    }

    private void BestKnownSolutionParameter_ValueChanged(object sender, EventArgs e) {
      orienteeringSolutionView.Content.IntegerVector = Content.BestKnownSolution;
      if (Content.BestKnownSolution != null)
        EvaluateBestSolution();
      else {
        var solution = orienteeringSolutionView.Content;
        solution.Penalty = null;
        solution.Distance = null;
      }
    }

    private void EvaluateBestSolution() {
      var evaluation = Content.Evaluator.Evaluate(Content.BestKnownSolution, Content.Scores, Content.DistanceMatrix,
        Content.MaximumDistance, Content.PointVisitingCosts);
      orienteeringSolutionView.Content.Quality = evaluation.Quality;
      orienteeringSolutionView.Content.Penalty = evaluation.Penalty;
      orienteeringSolutionView.Content.Distance = evaluation.Distance;
    }
  }
}
