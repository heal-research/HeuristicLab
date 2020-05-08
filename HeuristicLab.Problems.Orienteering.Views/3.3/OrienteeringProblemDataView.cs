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

using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.TravelingSalesman.Views;

namespace HeuristicLab.Problems.Orienteering.Views {
  [View("Orienteering Problem Data View")]
  [Content(typeof(OrienteeringProblemData), IsDefaultView = true)]
  public partial class OrienteeringProblemDataView : ItemView {

    public new OrienteeringProblemData Content {
      get { return (OrienteeringProblemData)base.Content; }
      set { base.Content = value; }
    }

    public OrienteeringProblemDataView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        routingDataViewHost.Content = null;
        startingPointTextBox.Text = string.Empty;
        terminalPointTextBox.Text = string.Empty;
        pointVisitingCostsTextBox.Text = string.Empty;
        maximumTravelCostsTextBox.Text = string.Empty;
        scoresArrayView.Content = null;
      } else {
        routingDataViewHost.Content = Content.RoutingData;
        startingPointTextBox.Text = Content.StartingPoint.ToString();
        terminalPointTextBox.Text = Content.TerminalPoint.ToString();
        pointVisitingCostsTextBox.Text = Content.PointVisitingCosts.ToString();
        maximumTravelCostsTextBox.Text = Content.MaximumTravelCosts.ToString();
        scoresArrayView.Content = Content.Scores;
        if (routingDataViewHost.ActiveView is ITSPVisualizerView tspVis)
          tspVis.Visualizer = new OrienteeringVisualizer() {
            Data = Content
          };
      }
    }
  }
}
