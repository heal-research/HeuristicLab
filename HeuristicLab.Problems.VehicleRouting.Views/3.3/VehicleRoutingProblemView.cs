#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.VehicleRouting.Views {
  [View("VehicleRouting Problem View")]
  [Content(typeof(VehicleRoutingProblem), true)]
  public partial class VehicleRoutingProblemView : NamedItemView {
    private VRPImportDialog vrpImportDialog;
    
    public new VehicleRoutingProblem Content {
      get { return (VehicleRoutingProblem)base.Content; }
      set { base.Content = value; }
    }

    public VehicleRoutingProblemView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.CoordinatesParameter.ValueChanged -= new EventHandler(CoordinatesParameter_ValueChanged);
      Content.BestKnownQualityParameter.ValueChanged -= new EventHandler(BestKnownQualityParameter_ValueChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CoordinatesParameter.ValueChanged += new EventHandler(CoordinatesParameter_ValueChanged);
      Content.BestKnownQualityParameter.ValueChanged += new EventHandler(BestKnownQualityParameter_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        parameterCollectionView.Content = null;
        vrpSolutionView.Content = null;
      } else {
        parameterCollectionView.Content = ((IParameterizedNamedItem)Content).Parameters;
        UpdateSolution();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      parameterCollectionView.Enabled = Content != null;
      vrpSolutionView.Enabled = Content != null;
      importButton.Enabled = Content != null && !ReadOnly;
    }

    private void importButton_Click(object sender, EventArgs e) {
      if (vrpImportDialog == null) vrpImportDialog = new VRPImportDialog();

      if (vrpImportDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          switch (vrpImportDialog.Format) {
            case VRPFormat.TSPLib:
              Content.ImportFromTSPLib(vrpImportDialog.VRPFileName);
              break;

            case VRPFormat.Solomon:
              Content.ImportFromSolomon(vrpImportDialog.VRPFileName);
              break;

            case VRPFormat.ORLib:
              Content.ImportFromORLib(vrpImportDialog.VRPFileName);
              break;
          }

          if(!string.IsNullOrEmpty(vrpImportDialog.TourFileName))
            Content.ImportSolution(vrpImportDialog.TourFileName);
         }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    private void CoordinatesParameter_ValueChanged(object sender, EventArgs e) {
      vrpSolutionView.Content.Coordinates = Content.Coordinates;
    }

    private void UpdateSolution() {
      if (Content.BestKnownSolution == null)
        vrpSolutionView.Content = new VRPSolution(Content.Coordinates);
      else {
        //call evaluator
        IValueLookupParameter<DoubleMatrix> distMatrix = new ValueLookupParameter<DoubleMatrix>("DistMatrix",
          Content.DistanceMatrix);

        TourEvaluation eval = VRPEvaluator.Evaluate(
          Content.BestKnownSolution,
          Content.Vehicles,
          Content.DueTime,
          Content.ServiceTime,
          Content.ReadyTime,
          Content.Demand,
          Content.Capacity,
          Content.FleetUsageFactorParameter.Value,
          Content.TimeFactorParameter.Value,
          Content.DistanceFactorParameter.Value,
          Content.OverloadPenaltyParameter.Value,
          Content.TardinessPenaltyParameter.Value,
          Content.Coordinates,
          distMatrix,
          Content.UseDistanceMatrix);

        Content.DistanceMatrix = distMatrix.Value;

        vrpSolutionView.Content = new VRPSolution(Content.Coordinates,
          Content.BestKnownSolution,
          new DoubleValue(eval.Quality),
          new DoubleValue(eval.Distance),
          new DoubleValue(eval.Overload),
          new DoubleValue(eval.Tardiness),
          new DoubleValue(eval.TravelTime),
          new DoubleValue(eval.VehcilesUtilized),
          Content.DistanceMatrix,
          Content.UseDistanceMatrix,
          Content.ReadyTime,
          Content.DueTime,
          Content.ServiceTime);
      }
    }

   void BestKnownQualityParameter_ValueChanged(object sender, EventArgs e) {
      UpdateSolution();
    }
  }
}
