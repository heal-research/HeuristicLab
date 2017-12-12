#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common.Resources;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using System;
using System.Globalization;

namespace HeuristicLab.Clients.OKB.RunCreation.Views {
  [View("OKB Solution View (single-objective)")]
  [Content(typeof(SingleObjectiveOKBSolution), IsDefaultView = true)]
  public partial class SingleObjectiveOKBSolutionView : ItemView {

    public new SingleObjectiveOKBSolution Content {
      get { return (SingleObjectiveOKBSolution)base.Content; }
      set { base.Content = value; }
    }

    public SingleObjectiveOKBSolutionView() {
      InitializeComponent();
      refreshButton.Text = string.Empty;
      refreshButton.Image = VSImageLibrary.Refresh;
      uploadButton.Text = string.Empty;
      uploadButton.Image = VSImageLibrary.PublishToWeb;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.QualityChanged += ContentOnQualityChanged;
      Content.SolutionChanged += ContentOnSolutionChanged;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.QualityChanged -= ContentOnQualityChanged;
      Content.SolutionChanged -= ContentOnSolutionChanged;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        qualityTextBox.Text = string.Empty;
        solutionViewHost.Content = null;
      } else {
        qualityTextBox.Text = Content.Quality.ToString(CultureInfo.CurrentCulture.NumberFormat);
        solutionViewHost.Content = Content.Solution;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      refreshButton.Enabled = Content != null & !Locked && !ReadOnly && Content.SolutionId != -1;
      uploadButton.Enabled = Content != null && !Locked && !ReadOnly && Content.SolutionId == -1;
    }

    private void ContentOnSolutionChanged(object sender, EventArgs eventArgs) {
      solutionViewHost.Content = Content.Solution;
    }

    private void ContentOnQualityChanged(object sender, EventArgs eventArgs) {
      qualityTextBox.Text = Content.Quality.ToString(CultureInfo.CurrentCulture.NumberFormat);
    }

    private void refreshButton_Click(object sender, System.EventArgs e) {
      Content.Download(Content.SolutionId);
      SetEnabledStateOfControls();
    }

    private void uploadButton_Click(object sender, System.EventArgs e) {
      Content.Upload();
      SetEnabledStateOfControls();
    }
  }
}
