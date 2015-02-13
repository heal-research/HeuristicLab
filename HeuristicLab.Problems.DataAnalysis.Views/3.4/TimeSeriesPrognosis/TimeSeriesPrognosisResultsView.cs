#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.Optimization.Views;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("TimeSeriesPrognosisResultsView")]
  [Content(typeof(TimeSeriesPrognosisResults), IsDefaultView = true)]
  public partial class TimeSeriesPrognosisResultsView : ResultCollectionView {
    public new TimeSeriesPrognosisResults Content {
      get { return (TimeSeriesPrognosisResults)base.Content; }
      set { base.Content = value; }
    }

    public override bool Locked {
      get {
        return base.Locked;
      }
      set {
        base.Locked = value;
      }
    }

    public TimeSeriesPrognosisResultsView() {
      InitializeComponent();
      TrainingHorizonErrorProvider.SetIconAlignment(TrainingHorizonTextBox, ErrorIconAlignment.MiddleLeft);
      TrainingHorizonErrorProvider.SetIconPadding(TrainingHorizonTextBox, 2);
      TestHorizonErrorProvider.SetIconAlignment(TestHorizonTextBox, ErrorIconAlignment.MiddleLeft);
      TestHorizonErrorProvider.SetIconPadding(TestHorizonTextBox, 2);
    }

    #region Content Events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.TrainingHorizonChanged += new System.EventHandler(Content_TrainingHorizonChanged);
      Content.TestHorizonChanged += new System.EventHandler(Content_TestHorizonChanged);
    }
    protected override void DeregisterContentEvents() {
      Content.TrainingHorizonChanged -= new System.EventHandler(Content_TrainingHorizonChanged);
      Content.TestHorizonChanged -= new System.EventHandler(Content_TestHorizonChanged);
      base.DeregisterContentEvents();
    }

    private void Content_TrainingHorizonChanged(object sender, System.EventArgs e) {
      TrainingHorizonTextBox.Text = Content.TrainingHorizon.ToString();
    }

    private void Content_TestHorizonChanged(object sender, System.EventArgs e) {
      TestHorizonTextBox.Text = Content.TestHorizon.ToString();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        TrainingHorizonTextBox.Text = string.Empty;
        TestHorizonTextBox.Text = string.Empty;
      } else {
        TrainingHorizonTextBox.Text = Content.TrainingHorizon.ToString();
        TestHorizonTextBox.Text = Content.TestHorizon.ToString();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      //necessary cause this could be triggered by the base ctor when the controls are not created yet
      if (TrainingHorizonTextBox == null || TestHorizonTextBox == null) return;
      TrainingHorizonTextBox.Enabled = Content != null && !Locked;
      TestHorizonTextBox.Enabled = Content != null && !Locked;
    }

    #region Control events
    private void TrainingHorizonTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      int trainingHorizon;
      if (!int.TryParse(TrainingHorizonTextBox.Text, out trainingHorizon)) {
        e.Cancel = true;
        TrainingHorizonErrorProvider.SetError(TrainingHorizonTextBox, "Please enter a numeric value.");
        TrainingHorizonTextBox.SelectAll();
      }
    }

    private void TrainingHorizonTextBox_Validated(object sender, System.EventArgs e) {
      Content.TrainingHorizon = int.Parse(TrainingHorizonTextBox.Text);
      TrainingHorizonErrorProvider.SetError(TrainingHorizonTextBox, string.Empty);
      TrainingHorizonTextBox.Text = Content.TrainingHorizon.ToString();
    }

    private void TrainingHorizonTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        TrainingHorizonLabel.Select();  // select label to validate data

      if (e.KeyCode == Keys.Escape) {
        TrainingHorizonTextBox.Text = Content.TrainingHorizon.ToString();
        TrainingHorizonLabel.Select();  // select label to validate data
      }
    }

    private void TestHorizonTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      int testHorizon;
      if (!int.TryParse(TestHorizonTextBox.Text, out testHorizon)) {
        e.Cancel = true;
        TestHorizonErrorProvider.SetError(TestHorizonTextBox, "Please enter a numeric value.");
        TestHorizonTextBox.SelectAll();
      }
    }

    private void TestHorizonTextBox_Validated(object sender, System.EventArgs e) {
      Content.TestHorizon = int.Parse(TestHorizonTextBox.Text);
      TestHorizonErrorProvider.SetError(TestHorizonTextBox, string.Empty);
      TestHorizonTextBox.Text = Content.TestHorizon.ToString();
    }

    private void TestHorizonTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        TestHorizonLabel.Select();  // select label to validate data

      if (e.KeyCode == Keys.Escape) {
        TestHorizonTextBox.Text = Content.TestHorizon.ToString();
        TestHorizonLabel.Select();  // select label to validate data
      }
    }
    #endregion
  }
}
