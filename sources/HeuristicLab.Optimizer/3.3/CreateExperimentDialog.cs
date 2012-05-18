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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Optimizer {
  public partial class CreateExperimentDialog : Form {
    private IOptimizer optimizer;
    public IOptimizer Optimizer {
      get { return optimizer; }
      set {
        optimizer = value;
        experiment = null;
        okButton.Enabled = optimizer != null;
        FillOrHideInstanceListView();
      }
    }

    private Experiment experiment;
    public Experiment Experiment {
      get { return experiment; }
    }

    private bool createBatchRun;
    private int repetitions;
    private EventWaitHandle backgroundWorkerWaitHandle = new ManualResetEvent(false);
    private bool suppressListViewEventHandling;

    public CreateExperimentDialog() : this(null) { }
    public CreateExperimentDialog(IOptimizer optimizer) {
      InitializeComponent();
      createBatchRun = createBatchRunCheckBox.Checked;
      repetitions = (int)repetitionsNumericUpDown.Value;
      Optimizer = optimizer;
    }

    private void FillOrHideInstanceListView() {
      if (optimizer != null && optimizer is IAlgorithm) {
        var algorithm = (IAlgorithm)Optimizer;
        if (algorithm.Problem != null) {
          var instanceProviders = ProblemInstanceManager.GetProviders(algorithm.Problem);
          if (instanceProviders.Any()) {
            FillInstanceListView(instanceProviders);
            if (instancesListView.Items.Count > 0) {
              selectAllCheckBox.Visible = true;
              selectNoneCheckBox.Visible = true;
              instancesLabel.Visible = true;
              instancesListView.Visible = true;
              Height = 330;
              return;
            }
          }
        }
      }
      selectAllCheckBox.Visible = false;
      selectNoneCheckBox.Visible = false;
      instancesLabel.Visible = false;
      instancesListView.Visible = false;
      Height = 130;
    }

    private void FillInstanceListView(IEnumerable<IProblemInstanceProvider> instanceProviders) {
      foreach (var provider in instanceProviders) {
        var group = new ListViewGroup(provider.Name, provider.Name);
        group.Tag = provider;
        instancesListView.Groups.Add(group);
        foreach (var d in ProblemInstanceManager.GetDataDescriptors(provider)) {
          var item = new ListViewItem(d.Name, group);
          item.Tag = d;
          instancesListView.Items.Add(item);
        }
      }
      instancesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
      selectAllCheckBox.Checked = true;
    }

    private void createBatchRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      repetitionsNumericUpDown.Enabled = createBatchRunCheckBox.Checked;
      createBatchRun = createBatchRunCheckBox.Checked;
    }
    private void repetitionsNumericUpDown_Validated(object sender, EventArgs e) {
      if (repetitionsNumericUpDown.Text == string.Empty)
        repetitionsNumericUpDown.Text = repetitionsNumericUpDown.Value.ToString();
      repetitions = (int)repetitionsNumericUpDown.Value;
    }
    private void selectAllCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (selectAllCheckBox.Checked) {
        selectNoneCheckBox.Checked = false;
        if (instancesListView.CheckedItems.Count == instancesListView.Items.Count) return;
        try {
          suppressListViewEventHandling = true;
          foreach (var item in instancesListView.Items.OfType<ListViewItem>()) {
            item.Checked = true;
          }
        } finally { suppressListViewEventHandling = false; }
      }
    }
    private void selectNoneCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (selectNoneCheckBox.Checked) {
        selectAllCheckBox.Checked = false;
        if (instancesListView.CheckedItems.Count == 0) return;
        try {
          suppressListViewEventHandling = true;
          foreach (var item in instancesListView.Items.OfType<ListViewItem>()) {
            item.Checked = false;
          }
        } finally { suppressListViewEventHandling = false; }
      }
    }
    private void instancesListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      if (!suppressListViewEventHandling) {
        selectAllCheckBox.Checked = instancesListView.Items.Count == instancesListView.CheckedItems.Count;
        selectNoneCheckBox.Checked = instancesListView.CheckedItems.Count == 0;
      }
    }
    private void okButton_Click(object sender, EventArgs e) {
      SetMode(createExperiment: true);
      experimentCreationBackgroundWorker.RunWorkerAsync(GetSelectedInstances());
      backgroundWorkerWaitHandle.WaitOne();
    }
    private void experimentCreationBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      backgroundWorkerWaitHandle.Set();
      experimentCreationBackgroundWorker.ReportProgress(0, string.Empty);
      var items = (Dictionary<IProblemInstanceProvider, List<IDataDescriptor>>)e.Argument;
      var localExperiment = new Experiment();
      if (items.Count == 0) {
        AddOptimizer((IOptimizer)Optimizer.Clone(), localExperiment);
        experimentCreationBackgroundWorker.ReportProgress(100, string.Empty);
      } else {
        int counter = 0, total = items.SelectMany(x => x.Value).Count();
        foreach (var provider in items.Keys) {
          foreach (var descriptor in items[provider]) {
            var algorithm = (IAlgorithm)Optimizer.Clone();
            ProblemInstanceManager.LoadData(provider, descriptor, (IProblemInstanceConsumer)algorithm.Problem);
            AddOptimizer(algorithm, localExperiment);
            counter++;
            experimentCreationBackgroundWorker.ReportProgress((int)Math.Round(100.0 * counter / total), descriptor.Name);
            if (experimentCreationBackgroundWorker.CancellationPending) {
              e.Cancel = true;
              localExperiment = null;
              break;
            }
          }
        }
      }
      if (localExperiment != null) localExperiment.Prepare(true);
      e.Result = localExperiment;
    }
    private void experimentCreationBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      experimentCreationProgressBar.Value = e.ProgressPercentage;
      progressLabel.Text = (string)e.UserState;
    }
    private void experimentCreationBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      SetMode(createExperiment: false);
      if (e.Error != null) MessageBox.Show(e.Error.Message, "Error occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
      if (!e.Cancelled && e.Error == null) {
        experiment = (Experiment)e.Result;
        DialogResult = System.Windows.Forms.DialogResult.OK;
        Close();
      }
    }
    private void CreateExperimentDialog_FormClosing(object sender, FormClosingEventArgs e) {
      if (experimentCreationBackgroundWorker.IsBusy) {
        if (DialogResult != System.Windows.Forms.DialogResult.OK)
          experimentCreationBackgroundWorker.CancelAsync();
        e.Cancel = true;
      }
    }

    private void AddOptimizer(IOptimizer optimizer, Experiment experiment) {
      if (createBatchRun) {
        var batchRun = new BatchRun();
        batchRun.Repetitions = repetitions;
        batchRun.Optimizer = optimizer;
        experiment.Optimizers.Add(batchRun);
      } else {
        experiment.Optimizers.Add(optimizer);
      }
    }

    private void SetMode(bool createExperiment) {
      createBatchRunCheckBox.Enabled = !createExperiment;
      repetitionsNumericUpDown.Enabled = !createExperiment;
      selectAllCheckBox.Enabled = !createExperiment;
      selectNoneCheckBox.Enabled = !createExperiment;
      instancesListView.Enabled = !createExperiment;
      okButton.Enabled = !createExperiment;
      okButton.Visible = !createExperiment;
      progressLabel.Visible = createExperiment;
      experimentCreationProgressBar.Visible = createExperiment;
    }

    private Dictionary<IProblemInstanceProvider, List<IDataDescriptor>> GetSelectedInstances() {
      var selectedInstances = new Dictionary<IProblemInstanceProvider, List<IDataDescriptor>>();
      foreach (var checkedItem in instancesListView.CheckedItems.OfType<ListViewItem>()) {
        if (!selectedInstances.ContainsKey((IProblemInstanceProvider)checkedItem.Group.Tag))
          selectedInstances.Add((IProblemInstanceProvider)checkedItem.Group.Tag, new List<IDataDescriptor>());
        selectedInstances[(IProblemInstanceProvider)checkedItem.Group.Tag].Add((IDataDescriptor)checkedItem.Tag);
      }
      return selectedInstances;
    }
  }
}
