#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.TravelingSalesman.Views {
  /// <summary>
  /// A view for a Traveling Salesman Problem instance.
  /// </summary>
  [View("Traveling Salesman Problem View")]
  [Content(typeof(TravelingSalesmanProblem), true)]
  public sealed partial class TravelingSalesmanProblemView : NamedItemView {
    private TSPLIBImportDialog tsplibImportDialog;

    public new TravelingSalesmanProblem Content {
      get { return (TravelingSalesmanProblem)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TravelingSalesmanProblemView"/>.
    /// </summary>
    public TravelingSalesmanProblemView() {
      InitializeComponent();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (tsplibImportDialog != null) tsplibImportDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void DeregisterContentEvents() {
      Content.CoordinatesParameter.ValueChanged -= new EventHandler(CoordinatesParameter_ValueChanged);
      Content.BestKnownQualityParameter.ValueChanged -= new EventHandler(BestKnownQualityParameter_ValueChanged);
      Content.BestKnownSolutionParameter.ValueChanged -= new EventHandler(BestKnownSolutionParameter_ValueChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CoordinatesParameter.ValueChanged += new EventHandler(CoordinatesParameter_ValueChanged);
      Content.BestKnownQualityParameter.ValueChanged += new EventHandler(BestKnownQualityParameter_ValueChanged);
      Content.BestKnownSolutionParameter.ValueChanged += new EventHandler(BestKnownSolutionParameter_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        parameterCollectionView.Content = null;
        pathTSPTourView.Content = null;
      } else {
        parameterCollectionView.Content = ((IParameterizedNamedItem)Content).Parameters;
        pathTSPTourView.Content = new PathTSPTour(Content.Coordinates, Content.BestKnownSolution, Content.BestKnownQuality);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      parameterCollectionView.Enabled = Content != null;
      pathTSPTourView.Enabled = Content != null;
      importButton.Enabled = Content != null && !ReadOnly;
    }

    private void importButton_Click(object sender, System.EventArgs e) {
      if (tsplibImportDialog == null) tsplibImportDialog = new TSPLIBImportDialog();

      if (tsplibImportDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          if (tsplibImportDialog.Quality == null)
            Content.ImportFromTSPLIB(tsplibImportDialog.TSPFileName, tsplibImportDialog.TourFileName);
          else
            Content.ImportFromTSPLIB(tsplibImportDialog.TSPFileName, tsplibImportDialog.TourFileName, (double)tsplibImportDialog.Quality);
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    private void CoordinatesParameter_ValueChanged(object sender, EventArgs e) {
      pathTSPTourView.Content.Coordinates = Content.Coordinates;
    }
    private void BestKnownQualityParameter_ValueChanged(object sender, EventArgs e) {
      pathTSPTourView.Content.Quality = Content.BestKnownQuality;
    }
    private void BestKnownSolutionParameter_ValueChanged(object sender, EventArgs e) {
      pathTSPTourView.Content.Permutation = Content.BestKnownSolution;
    }
  }
}
