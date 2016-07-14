#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.DataPreprocessing;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Preprocessing Feature Correlation View")]
  [Content(typeof(CorrelationMatrixContent), false)]
  public partial class PreprocessingFeatureCorrelationView : AsynchronousContentView {

    public new CorrelationMatrixContent Content {
      get { return (CorrelationMatrixContent)base.Content; }
      set { base.Content = value; }
    }

    FeatureCorrelationView correlationView;

    public PreprocessingFeatureCorrelationView() {
      InitializeComponent();
      correlationView = new FeatureCorrelationView();
      correlationView.Dock = DockStyle.Fill;
      this.Controls.Add(correlationView);
    }


    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += Data_Changed;
    }

    protected override void DeregisterContentEvents() {
      Content.Changed -= Data_Changed;
      base.DeregisterContentEvents();
    }

    private void Data_Changed(object sender, DataPreprocessingChangedEventArgs e) {
      OnContentChanged();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        correlationView.Content = null;
        return;
      }

      correlationView.Content = Content.ProblemData;
    }
  }
}