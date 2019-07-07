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

using System;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization.Views;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.PTSP.Views {
  /// <summary>
  /// A view for a Traveling Salesman Problem instance.
  /// </summary>
  [View("Probabilistic Traveling Salesman Problem View")]
  [Content(typeof(ProbabilisticTravelingSalesmanProblem), true)]
  public sealed partial class ProbabilisticTravelingSalesmanProblemView : ProblemView {
    public new ProbabilisticTravelingSalesmanProblem Content {
      get { return (ProbabilisticTravelingSalesmanProblem)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ProbabilisticTravelingSalesmanProblemView"/>.
    /// </summary>
    public ProbabilisticTravelingSalesmanProblemView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.CoordinatesParameter.ValueChanged -= CoordinatesParameter_ValueChanged;
      Content.ProbabilitiesParameter.ValueChanged -= ProbabilityParameter_ValueChanged;
      Content.BestKnownSolutionParameter.ValueChanged -= BestKnownSolutionParameter_ValueChanged;
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CoordinatesParameter.ValueChanged += CoordinatesParameter_ValueChanged;
      Content.ProbabilitiesParameter.ValueChanged += ProbabilityParameter_ValueChanged;
      Content.BestKnownSolutionParameter.ValueChanged += BestKnownSolutionParameter_ValueChanged;
    }
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        pathPTSPTourView.Content = null;
      } else {
        pathPTSPTourView.Content = new PathPTSPTour(Content.Coordinates, Content.Probabilities, Content.BestKnownSolution, new DoubleValue(Content.BestKnownQuality));
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      pathPTSPTourView.Enabled = Content != null;
    }

    private void CoordinatesParameter_ValueChanged(object sender, EventArgs e) {
      pathPTSPTourView.Content.Coordinates = Content.Coordinates;
    }
    private void ProbabilityParameter_ValueChanged(object sender, EventArgs e) {
      pathPTSPTourView.Content.Probabilities = Content.Probabilities;
    }
    private void BestKnownSolutionParameter_ValueChanged(object sender, EventArgs e) {
      pathPTSPTourView.Content.Permutation = Content.BestKnownSolution;
      if (Content.BestKnownSolution != null)
        pathPTSPTourView.Content.Quality = new DoubleValue(Content.Evaluate(Content.BestKnownSolution, new MersenneTwister(13)));
      else pathPTSPTourView.Content.Quality = null;
    }
  }
}
