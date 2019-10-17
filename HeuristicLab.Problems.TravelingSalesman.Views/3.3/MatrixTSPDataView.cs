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

namespace HeuristicLab.Problems.TravelingSalesman.Views {
  [View("Matrix TSP Data View")]
  [Content(typeof(MatrixTSPData), IsDefaultView = true)]
  public partial class MatrixTSPDataView : NamedItemView, ITSPVisualizerView {

    private TSPVisualizer visualizer;
    public TSPVisualizer Visualizer {
      get => visualizer;
      set {
        if (visualizer == value) return;
        visualizer = value;
        if (Content != null) {
          visualizer.Coordinates = Content.DisplayCoordinates;
        }
        GenerateImage();
      }
    }

    public new MatrixTSPData Content {
      get { return (MatrixTSPData)base.Content; }
      set { base.Content = value; }
    }

    public MatrixTSPDataView() {
      InitializeComponent();
      visualizer = new TSPVisualizer();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        distanceMatrixView.Content = null;
        coordinatesMatrixView.Content = null;
        coordinatesPictureBox.Image = null;
        Visualizer.Coordinates = null;
      } else {
        distanceMatrixView.Content = Content.Matrix;
        coordinatesMatrixView.Content = Content.DisplayCoordinates;
        Visualizer.Coordinates = Content.DisplayCoordinates;
        GenerateImage();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      distanceMatrixView.Enabled = !ReadOnly && !Locked && Content != null;
      coordinatesMatrixView.Enabled = !ReadOnly && !Locked && Content != null;
    }

    protected virtual void GenerateImage() {
      coordinatesPictureBox.Image = Visualizer.Draw(coordinatesPictureBox.Width, coordinatesPictureBox.Height);
    }
  }
}
