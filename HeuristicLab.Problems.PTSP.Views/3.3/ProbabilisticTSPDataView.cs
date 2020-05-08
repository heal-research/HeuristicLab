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

using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.PTSP.Views {
  [View("Probabilistic TSP Data View")]
  [Content(typeof(ProbabilisticTSPData), IsDefaultView = true)]
  public partial class ProbabilisticTSPDataView : NamedItemView {

    public ProbabilisticTSPVisualizer Visualizer { get; set; }

    public new ProbabilisticTSPData Content {
      get { return (ProbabilisticTSPData)base.Content; }
      set { base.Content = value; }
    }

    public ProbabilisticTSPDataView() {
      InitializeComponent();
      Visualizer = new ProbabilisticTSPVisualizer();
      tspDataView.Visualizer = Visualizer;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        probabilitiesView.Content = null;
        Visualizer.Probabilities = null;
      } else {
        probabilitiesView.Content = Content.Probabilities;
        Visualizer.Probabilities = Content.Probabilities;
      }
      tspDataView.Content = Content?.TSPData;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      probabilitiesView.Enabled = !ReadOnly && !Locked && Content != null;
      tspDataView.Enabled = !ReadOnly && !Locked && Content != null;
    }
  }
}
