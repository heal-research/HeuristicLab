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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.Orienteering.Views {
  [View("OrienteeringSolution View")]
  [Content(typeof(IOrienteeringSolution), true)]
  public partial class OrienteeringSolutionView : ItemView {
    public OrienteeringVisualizer Visualizer { get; set; }

    public new IOrienteeringSolution Content {
      get { return (IOrienteeringSolution)base.Content; }
      set { base.Content = value; }
    }
    public OrienteeringSolutionView() {
      InitializeComponent();
      Visualizer = new OrienteeringVisualizer();
      tspSolutionView.Visualizer = Visualizer;
    }

    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= ContentOnPropertyChanged;
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += ContentOnPropertyChanged;
    }

    protected virtual void ContentOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
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
        Visualizer.Data = Content.Data;
        Visualizer.IsFeasible = Content.TravelCosts.Value <= Content.Data.MaximumTravelCosts;
      }
      tspSolutionView.Content = Content;
      base.OnContentChanged();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      qualityValueView.Enabled = Content != null && !ReadOnly && !Locked;
      scoreValueView.Enabled = Content != null && !ReadOnly && !Locked;
    }
  }
}
