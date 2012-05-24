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
using System.Text;
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
        SetInstanceListViewVisibility();
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
      // do not set the Optimizer property here, because we want to delay instance discovery to the time when the form loads
      this.optimizer = optimizer;
      this.experiment = null;
      okButton.Enabled = optimizer != null;
    }

    #region Event handlers
    private void CreateExperimentDialog_Load(object sender, EventArgs e) {
      SetInstanceListViewVisibility();
    }

    private void CreateExperimentDialog_FormClosing(object sender, FormClosingEventArgs e) {
      if (experimentCreationBackgroundWorker.IsBusy) {
        if (DialogResult != System.Windows.Forms.DialogResult.OK) {
          if (experimentCreationBackgroundWorker.IsBusy) experimentCreationBackgroundWorker.CancelAsync();
          if (instanceDiscoveryBackgroundWorker.IsBusy) instanceDiscoveryBackgroundWorker.CancelAsync();
        }
        e.Cancel = true;
      }
    }

    private void okButton_Click(object sender, EventArgs e) {
      SetMode(locked: true);
      experimentCreationBackgroundWorker.RunWorkerAsync(GetSelectedInstances());
      backgroundWorkerWaitHandle.WaitOne(); // make sure the background worker has started before exiting
    }

    private void instancesListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      if (!suppressListViewEventHandling) {
        selectAllCheckBox.Checked = instancesListView.Items.Count == instancesListView.CheckedItems.Count;
        selectNoneCheckBox.Checked = instancesListView.CheckedItems.Count == 0;
      }
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

    private void createBatchRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      repetitionsNumericUpDown.Enabled = createBatchRunCheckBox.Checked;
      createBatchRun = createBatchRunCheckBox.Checked;
    }

    private void repetitionsNumericUpDown_Validated(object sender, EventArgs e) {
      if (repetitionsNumericUpDown.Text == string.Empty)
        repetitionsNumericUpDown.Text = repetitionsNumericUpDown.Value.ToString();
      repetitions = (int)repetitionsNumericUpDown.Value;
    }
    #endregion

    #region Helpers
    private void SetInstanceListViewVisibility() {
      bool instancesAvailable = optimizer != null
        && optimizer is IAlgorithm
        && ((IAlgorithm)optimizer).Problem != null
        && ProblemInstanceManager.GetProviders(((IAlgorithm)optimizer).Problem).Any();
      selectAllCheckBox.Visible = instancesAvailable;
      selectNoneCheckBox.Visible = instancesAvailable;
      instancesLabel.Visible = instancesAvailable;
      instancesListView.Visible = instancesAvailable;
      if (instancesAvailable) {
        Height = 330;
        FillInstanceListViewAsync();
      } else Height = 130;
    }

    private void FillInstanceListViewAsync() {
      SetMode(locked: true);
      var instanceProviders = ProblemInstanceManager.GetProviders(((IAlgorithm)Optimizer).Problem);
      instanceDiscoveryBackgroundWorker.RunWorkerAsync(instanceProviders);
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

    private void SetMode(bool locked) {
      createBatchRunCheckBox.Enabled = !locked;
      repetitionsNumericUpDown.Enabled = !locked;
      selectAllCheckBox.Enabled = !locked;
      selectNoneCheckBox.Enabled = !locked;
      instancesListView.Enabled = !locked;
      instancesListView.Visible = !locked;
      okButton.Enabled = !locked;
      okButton.Visible = !locked;
      progressLabel.Visible = locked;
      experimentCreationProgressBar.Visible = locked;
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
    #endregion

    #region Background workers
    private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      experimentCreationProgressBar.Value = e.ProgressPercentage;
      progressLabel.Text = (string)e.UserState;
    }

    private void instanceDiscoveryBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      double progress = 0;
      instanceDiscoveryBackgroundWorker.ReportProgress((int)progress, string.Empty);
      var instanceProviders = ((IEnumerable<IProblemInstanceProvider>)e.Argument).ToArray();
      ListViewGroup[] groups = new ListViewGroup[instanceProviders.Length];
      for (int i = 0; i < instanceProviders.Length; i++) {
        var provider = instanceProviders[i];
        groups[i] = new ListViewGroup(provider.Name, provider.Name) { Tag = provider };
      }
      e.Result = groups;
      for (int i = 0; i < groups.Length; i++) {
        var group = groups[i];
        var provider = group.Tag as IProblemInstanceProvider;
        progress = (100.0 * i) / groups.Length;
        instanceDiscoveryBackgroundWorker.ReportProgress((int)progress, provider.Name);
        var descriptors = ProblemInstanceManager.GetDataDescriptors(provider).ToArray();
        for (int j = 0; j < descriptors.Length; j++) {
          #region Check cancellation request
          if (instanceDiscoveryBackgroundWorker.CancellationPending) {
            e.Cancel = true;
            return;
          }
          #endregion
          var d = descriptors[j];
          progress += 1.0 / (descriptors.Length * groups.Length);
          instanceDiscoveryBackgroundWorker.ReportProgress((int)progress, d.Name);
          var item = new ListViewItem(d.Name, group) { Tag = d };
        }
      }
      instanceDiscoveryBackgroundWorker.ReportProgress(100, string.Empty);
    }

    private void instanceDiscoveryBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      try {
        // unfortunately it's not enough to just add the groups, the items need to be added separately
        foreach (var group in (ListViewGroup[])e.Result) {
          instancesListView.Groups.Add(group);
          instancesListView.Items.AddRange(group.Items);
        }
        instancesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        selectAllCheckBox.Checked = true;
      } catch { }
      try {
        SetMode(locked: false);
        if (e.Error != null) MessageBox.Show(e.Error.Message, "Error occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
      } catch { }
    }

    private StringBuilder failedInstances;
    private void experimentCreationBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      backgroundWorkerWaitHandle.Set();
      experimentCreationBackgroundWorker.ReportProgress(0, string.Empty);
      failedInstances = new StringBuilder();
      var items = (Dictionary<IProblemInstanceProvider, List<IDataDescriptor>>)e.Argument;
      var localExperiment = new Experiment();
      if (items.Count == 0) {
        AddOptimizer((IOptimizer)Optimizer.Clone(), localExperiment);
        experimentCreationBackgroundWorker.ReportProgress(100, string.Empty);
      } else {
        int counter = 0, total = items.SelectMany(x => x.Value).Count();
        foreach (var provider in items.Keys) {
          foreach (var descriptor in items[provider]) {
            #region Check cancellation request
            if (experimentCreationBackgroundWorker.CancellationPending) {
              e.Cancel = true;
              localExperiment = null;
              return;
            }
            #endregion
            var algorithm = (IAlgorithm)Optimizer.Clone();
            bool failed = false;
            try {
              ProblemInstanceManager.LoadData(provider, descriptor, (IProblemInstanceConsumer)algorithm.Problem);
            } catch (Exception ex) {
              failedInstances.AppendLine(descriptor.Name + ": " + ex.Message);
              failed = true;
            }
            if (!failed) {
              AddOptimizer(algorithm, localExperiment);
              counter++;
              experimentCreationBackgroundWorker.ReportProgress((int)Math.Round(100.0 * counter / total), descriptor.Name);
            } else experimentCreationBackgroundWorker.ReportProgress((int)Math.Round(100.0 * counter / total), "Loading failed (" + descriptor.Name + ")");
          }
        }
      }
      if (localExperiment != null) localExperiment.Prepare(true);
      e.Result = localExperiment;
    }

    private void experimentCreationBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      try {
        SetMode(locked: false);
        if (e.Error != null) MessageBox.Show(e.Error.Message, "Error occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (failedInstances.Length > 0) MessageBox.Show("Some instances could not be loaded: " + Environment.NewLine + failedInstances.ToString(), "Some instances failed to load", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (!e.Cancelled && e.Error == null) {
          experiment = (Experiment)e.Result;
          DialogResult = System.Windows.Forms.DialogResult.OK;
          Close();
        }
      } catch { }
    }
    #endregion
  }
}
