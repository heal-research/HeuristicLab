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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.QuadraticAssignment.QAPInstanceService;

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
      reloadInstancesButton.Text = String.Empty;
      reloadInstancesButton.Image = VSImageLibrary.Refresh;
      loadInstanceButton.Image = VSImageLibrary.Internet;
      Controls.Remove(parameterCollectionView);
      parameterCollectionView.Dock = DockStyle.Fill;
      problemTabPage.Controls.Add(parameterCollectionView);
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.DistancesParameter.ValueChanged += new EventHandler(DistanceMatrixParameter_ValueChanged);
      Content.WeightsParameter.ValueChanged += new EventHandler(WeightsParameter_ValueChanged);
      Content.BestKnownSolutionParameter.ValueChanged += new EventHandler(BestKnownSolutionParameter_ValueChanged);
      Content.Instances.ItemsAdded += new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<string>>(Content_Instances_Changed);
      Content.Instances.ItemsMoved += new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<string>>(Content_Instances_Changed);
      Content.Instances.ItemsRemoved += new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<string>>(Content_Instances_Changed);
      Content.Instances.ItemsReplaced += new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<string>>(Content_Instances_Changed);
      Content.Instances.CollectionReset += new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<string>>(Content_Instances_Changed);
    }

    protected override void DeregisterContentEvents() {
      Content.DistancesParameter.ValueChanged -= new EventHandler(DistanceMatrixParameter_ValueChanged);
      Content.WeightsParameter.ValueChanged -= new EventHandler(WeightsParameter_ValueChanged);
      Content.BestKnownSolutionParameter.ValueChanged -= new EventHandler(BestKnownSolutionParameter_ValueChanged);
      Content.Instances.ItemsAdded -= new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<string>>(Content_Instances_Changed);
      Content.Instances.ItemsMoved -= new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<string>>(Content_Instances_Changed);
      Content.Instances.ItemsRemoved -= new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<string>>(Content_Instances_Changed);
      Content.Instances.ItemsReplaced -= new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<string>>(Content_Instances_Changed);
      Content.Instances.CollectionReset -= new Collections.CollectionItemsChangedEventHandler<Collections.IndexedItem<string>>(Content_Instances_Changed);
      base.DeregisterContentEvents();
    }

    private void Content_Instances_Changed(object sender, EventArgs e) {
      instancesComboBox.Items.Clear();
      foreach (string name in Content.Instances)
        instancesComboBox.Items.Add(name);
      if (instancesComboBox.Items.Count > 0)
        instancesComboBox.SelectedIndex = 0;
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
      if (Content != null) {
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
      reloadInstancesButton.Enabled = !ReadOnly && !Locked && Content != null;
      instancesComboBox.Enabled = !ReadOnly && !Locked && Content != null;
      loadInstanceButton.Enabled = !ReadOnly && !Locked && Content != null && !String.IsNullOrEmpty((string)instancesComboBox.SelectedItem);
      importInstanceButton.Enabled = !ReadOnly && !Locked && Content != null;
    }

    private void instancesComboBox_SelectedValueChanged(object sender, System.EventArgs e) {
      SetEnabledStateOfControls();
    }

    private void loadInstanceButton_Click(object sender, System.EventArgs e) {
      ReadOnly = true;
      string instanceStr = instancesComboBox.SelectedItem as string;
      progressBar.Visible = true;
      loadInstanceWorker.RunWorkerAsync(instanceStr);
    }

    private void importFileInstanceButton_Click(object sender, System.EventArgs e) {
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        try {
          Content.LoadInstanceFromFile(openFileDialog.FileName);
        } catch (Exception ex) {
          PluginInfrastructure.ErrorHandling.ShowErrorDialog(ex);
        }
      }
    }

    private void reloadInstancesButton_Click(object sender, EventArgs e) {
      ReadOnly = true;
      progressBar.Visible = true;
      getInstancesWorker.RunWorkerAsync();
    }

    private void loadInstanceWorker_DoWork(object sender, DoWorkEventArgs e) {
      string instance = (string)e.Argument;
      if (String.IsNullOrEmpty(instance)) return;
      using (var client = new QAPClient()) {
        loadInstanceWorker.ReportProgress(10);
        var data = client.GetProblemInstanceData(instance);
        loadInstanceWorker.ReportProgress(60);
        DoubleMatrix weights = new DoubleMatrix(data.Weights.Length, data.Weights.Length);
        DoubleMatrix distances = new DoubleMatrix(data.Weights.Length, data.Weights.Length);
        try {
          for (int i = 0; i < data.Weights.Length; i++)
            for (int j = 0; j < data.Weights.Length; j++) {
              weights[i, j] = data.Weights[i][j];
              distances[i, j] = data.Distances[i][j];
            }
        } catch (IndexOutOfRangeException) {
          MessageBox.Show("The problem data is malformed, the problem could not be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        loadInstanceWorker.ReportProgress(65);
        Content.Name = data.Name;
        Content.Description = data.Description;
        Content.Maximization.Value = data.Maximization;
        Content.Weights = weights;
        Content.Distances = distances;

        Content.BestKnownQualityParameter.Value = null;
        Content.BestKnownSolution = null;
        Content.BestKnownSolutions = new ItemSet<Permutation>();
        var solutions = client.GetBestSolutionsData(instance);
        loadInstanceWorker.ReportProgress(90);
        if (solutions.Any()) {
          Content.BestKnownQualityParameter.Value = new DoubleValue(solutions.First().Quality);
          Content.BestKnownSolution = new Permutation(PermutationTypes.Absolute, solutions.First().Assignment);
          foreach (var solution in solutions) {
            Content.BestKnownSolutions.Add(new Permutation(PermutationTypes.Absolute, solution.Assignment));
          }
        }
        loadInstanceWorker.ReportProgress(100);
      }
    }

    private void getInstancesWorker_DoWork(object sender, DoWorkEventArgs e) {
      using (var client = new QAPClient()) {
        getInstancesWorker.ReportProgress(10);
        var instances = client.GetProblemInstances();
        getInstancesWorker.ReportProgress(85);
        Content.Instances.Replace(instances);
        getInstancesWorker.ReportProgress(100);
      }
    }

    private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      progressBar.Value = e.ProgressPercentage;
    }

    private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      progressBar.Visible = false;
      ReadOnly = false;
      if (e.Error != null)
        PluginInfrastructure.ErrorHandling.ShowErrorDialog(e.Error);
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