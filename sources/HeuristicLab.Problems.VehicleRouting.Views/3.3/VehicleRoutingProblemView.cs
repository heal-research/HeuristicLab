#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization.Views;
using HeuristicLab.Core.Views;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.VehicleRouting.Views {
  [View("VehicleRouting Problem View")]
  [Content(typeof(VehicleRoutingProblem), true)]
  public partial class VehicleRoutingProblemView : NamedItemView {
    public new VehicleRoutingProblem Content {
      get { return (VehicleRoutingProblem)base.Content; }
      set { base.Content = value; }
    }

    public VehicleRoutingProblemView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.CoordinatesParameter.ValueChanged -= new EventHandler(CoordinatesParameter_ValueChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CoordinatesParameter.ValueChanged += new EventHandler(CoordinatesParameter_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        parameterCollectionView.Content = null;
        vrpSolutionView.Content = null;
      } else {
        parameterCollectionView.Content = ((IParameterizedNamedItem)Content).Parameters;
        vrpSolutionView.Content = new VRPSolution(Content.Coordinates);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      parameterCollectionView.Enabled = Content != null;
      vrpSolutionView.Enabled = Content != null;
      importButton.Enabled = importButton2.Enabled = importButton3.Enabled = Content != null && !ReadOnly;
    }

    private void importButton_Click(object sender, EventArgs e) {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Solomon files (*.txt)|*.txt";

      if (dialog.ShowDialog() == DialogResult.OK) {
        Content.ImportFromSolomon(dialog.FileName);
      }
    }

    private void importButton2_Click(object sender, EventArgs e) {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "TSPLib files (*.vrp)|*.vrp";

      if (dialog.ShowDialog() == DialogResult.OK) {
        Content.ImportFromTSPLib(dialog.FileName);
      }
    }

    private void importButton3_Click(object sender, EventArgs e) {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "ORLib files (*.txt)|*.txt";

      if (dialog.ShowDialog() == DialogResult.OK) {
        Content.ImportFromORLib(dialog.FileName);
      }
    }  

    private void CoordinatesParameter_ValueChanged(object sender, EventArgs e) {
      vrpSolutionView.Content.Coordinates = Content.Coordinates;
    }  
  }
}
