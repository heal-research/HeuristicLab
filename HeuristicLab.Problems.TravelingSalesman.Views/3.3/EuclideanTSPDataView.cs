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
using static HeuristicLab.Problems.TravelingSalesman.EuclideanTSPData;

namespace HeuristicLab.Problems.TravelingSalesman.Views {
  [View("Euclidean TSP Data View")]
  [Content(typeof(EuclideanTSPData), IsDefaultView = true)]
  public partial class EuclideanTSPDataView : CoordinatesTSPDataView {

    public new EuclideanTSPData Content {
      get { return (EuclideanTSPData)base.Content; }
      set { base.Content = value; }
    }

    public EuclideanTSPDataView() {
      InitializeComponent();
      foreach (var e in Enum.GetValues(typeof(DistanceRounding)))
        roundingModeComboBox.Items.Add(e);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        roundingModeComboBox.SelectedIndex = -1;
      } else {
        roundingModeComboBox.SelectedItem = Content.Rounding;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      roundingModeComboBox.Enabled = false;
    }
  }
}
