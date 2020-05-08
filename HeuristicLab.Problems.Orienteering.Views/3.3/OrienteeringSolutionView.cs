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

using System.ComponentModel;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.TravelingSalesman.Views;

namespace HeuristicLab.Problems.Orienteering.Views {
  [View("OrienteeringSolution View")]
  [Content(typeof(OrienteeringSolution), true)]
  public partial class OrienteeringSolutionView : TSPSolutionView {
    public new OrienteeringVisualizer Visualizer {
      get { return (OrienteeringVisualizer)base.Visualizer; }
      set { base.Visualizer = value; }
    }
    public new OrienteeringSolution Content {
      get { return (OrienteeringSolution)base.Content; }
      set { base.Content = value; }
    }
    public OrienteeringSolutionView() {
      InitializeComponent();
      Visualizer = new OrienteeringVisualizer();
    }

    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= ContentOnPropertyChanged;
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += ContentOnPropertyChanged;
    }

    protected override void ContentOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
      base.ContentOnPropertyChanged(sender, e);
      switch (e.PropertyName) {
        case nameof(Content.Quality):
          qualityValueView.Content = Content.Quality;
          break;
        case nameof(Content.Score):
          scoreValueView.Content = Content.Score;
          break;
      }
    }

    protected override void OnContentChanged() {
      if (Content == null) {
        qualityValueView.Content = null;
        scoreValueView.Content = null;
        Visualizer.Data = null;
        Visualizer.IsFeasible = false;
      } else {
        qualityValueView.Content = Content.Quality;
        scoreValueView.Content = Content.Score;
        Visualizer.Data = Content.OPData;
        Visualizer.IsFeasible = Content.TravelCosts.Value <= Content.OPData.MaximumTravelCosts;
      }
      base.OnContentChanged();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      qualityValueView.Enabled = Content != null && !ReadOnly && !Locked;
      scoreValueView.Enabled = Content != null && !ReadOnly && !Locked;
    }
  }
}
