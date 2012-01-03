#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.QuadraticAssignment.Views {
  [View("Quadratic Assignment Problem View")]
  [Content(typeof(QuadraticAssignmentProblem), IsDefaultView = true)]
  public sealed partial class QuadraticAssignmentProblemView : ParameterizedNamedItemView {
    public new QuadraticAssignmentProblem Content {
      get { return (QuadraticAssignmentProblem)base.Content; }
      set { base.Content = value; }
    }

    public QuadraticAssignmentProblemView() {
      InitializeComponent();
      importInstanceButton.Image = VSImageLibrary.Open;
      Controls.Remove(parameterCollectionView);
      parameterCollectionView.Dock = DockStyle.Fill;
      problemTabPage.Controls.Add(parameterCollectionView);
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.DistancesParameter.ValueChanged += new EventHandler(DistanceMatrixParameter_ValueChanged);
      Content.WeightsParameter.ValueChanged += new EventHandler(WeightsParameter_ValueChanged);
      Content.BestKnownSolutionParameter.ValueChanged += new EventHandler(BestKnownSolutionParameter_ValueChanged);
    }

    protected override void DeregisterContentEvents() {
      Content.DistancesParameter.ValueChanged -= new EventHandler(DistanceMatrixParameter_ValueChanged);
      Content.WeightsParameter.ValueChanged -= new EventHandler(WeightsParameter_ValueChanged);
      Content.BestKnownSolutionParameter.ValueChanged -= new EventHandler(BestKnownSolutionParameter_ValueChanged);
      base.DeregisterContentEvents();
    }

    private void DistanceMatrixParameter_ValueChanged(object sender, System.EventArgs e) {
      qapView.Distances = Content.Distances;
    }

    private void WeightsParameter_ValueChanged(object sender, System.EventArgs e) {
      qapView.Weights = Content.Weights;
    }

    private void BestKnownSolutionParameter_ValueChanged(object sender, System.EventArgs e) {
      qapView.Assignment = Content.BestKnownSolution;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      instancesComboBox.Items.Clear();
      if (Content != null) {
        foreach (string instance in Content.Instances) {
          instancesComboBox.Items.Add(instance);
        }
        qapView.Distances = Content.Distances;
        qapView.Weights = Content.Weights;
        qapView.Assignment = Content.BestKnownSolution;
      } else {
        qapView.Distances = null;
        qapView.Weights = null;
        qapView.Assignment = null;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      instancesComboBox.Enabled = !ReadOnly && !Locked && Content != null;
      loadInstanceButton.Enabled = !ReadOnly && !Locked && Content != null && instancesComboBox.SelectedItem != null;
      importInstanceButton.Enabled = !ReadOnly && !Locked && Content != null;
    }

    private void instancesComboBox_SelectedValueChanged(object sender, System.EventArgs e) {
      loadInstanceButton.Enabled = instancesComboBox.SelectedItem != null;
    }

    private void loadInstanceButton_Click(object sender, System.EventArgs e) {
      string instance = instancesComboBox.SelectedItem as string;
      try {
        Content.LoadInstanceFromEmbeddedResource(instance);
      } catch (Exception ex) {
        PluginInfrastructure.ErrorHandling.ShowErrorDialog(ex);
      }
    }

    private void importInstanceButton_Click(object sender, System.EventArgs e) {
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        try {
          string datFile = openFileDialog.FileName;
          string directory = Path.GetDirectoryName(datFile);
          string solFile = Path.Combine(directory, Path.GetFileNameWithoutExtension(datFile) + ".sln");
          if (File.Exists(solFile)) {
            Content.LoadInstanceFromFile(datFile, solFile);
          } else {
            Content.LoadInstanceFromFile(datFile);
          }
        } catch (Exception ex) {
          PluginInfrastructure.ErrorHandling.ShowErrorDialog(ex);
        }
      }
    }

    private void QAPLIBInstancesLabel_Click(object sender, System.EventArgs e) {
      System.Diagnostics.Process.Start("http://www.seas.upenn.edu/qaplib/");
    }

    private void QAPLIBInstancesLabel_MouseEnter(object sender, EventArgs e) {
      Cursor = Cursors.Hand;
      QAPLIBInstancesLabel.ForeColor = Color.Red;
      toolTip.SetToolTip(QAPLIBInstancesLabel, "Browse to http://www.seas.upenn.edu/qaplib/");
    }

    private void QAPLIBInstancesLabel_MouseLeave(object sender, EventArgs e) {
      Cursor = Cursors.Default;
      QAPLIBInstancesLabel.ForeColor = Color.Blue;
      toolTip.SetToolTip(QAPLIBInstancesLabel, String.Empty);
    }
  }
}
