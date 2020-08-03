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
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting.Views {
  [View("VRP Evaluation View")]
  [Content(typeof(VRPEvaluation), IsDefaultView = true)]
  public partial class VRPEvaluationView : ItemView {
    public new VRPEvaluation Content {
      get => (VRPEvaluation)base.Content;
      set => base.Content = value;
    }
    public VRPEvaluationView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      qualityTextBox.Text = Content?.Quality.ToString() ?? string.Empty;
      distanceTextBox.Text = Content?.Distance.ToString() ?? string.Empty;
      vehiclesTextBox.Text = Content?.VehicleUtilization.ToString() ?? string.Empty;
      penaltyTextBox.Text = Content?.Penalty.ToString() ?? string.Empty;
      isFeasibleCcheckBox.Checked = Content?.IsFeasible ?? false;
    }
  }
}
