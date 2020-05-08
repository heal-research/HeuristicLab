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

namespace HeuristicLab.Problems.PTSP.Views {
  /// <summary>
  /// The base class for visual representations of a path tour for a PTSP.
  /// </summary>
  [View("pTSP Solution View")]
  [Content(typeof(IProbabilisticTSPSolution), true)]
  public sealed partial class ProbabilisticTSPSolutionView : ItemView {

    public ProbabilisticTSPVisualizer Visualizer { get; set; }

    public new IProbabilisticTSPSolution Content {
      get { return (IProbabilisticTSPSolution)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ProbabilisticTSPSolutionView"/>.
    /// </summary>
    public ProbabilisticTSPSolutionView() {
      InitializeComponent();
      Visualizer = new ProbabilisticTSPVisualizer();
      tspSolutionView.Visualizer = Visualizer;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null)
        Visualizer.Probabilities = null;
      else Visualizer.Probabilities = Content.Probabilities;
      tspSolutionView.Content = Content;
    }
  }
}
