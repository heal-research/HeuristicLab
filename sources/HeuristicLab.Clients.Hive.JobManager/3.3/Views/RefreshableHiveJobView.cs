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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.Views;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("Hive Job View")]
  [Content(typeof(RefreshableJob), true)]
  public partial class RefreshableHiveJobView : HeuristicLab.Core.Views.ItemView {
    private ProgressView progressView;

    public new RefreshableJob Content {
      get { return (RefreshableJob)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public RefreshableHiveJobView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.RefreshAutomaticallyChanged += new EventHandler(Content_RefreshAutomaticallyChanged);
      Content.JobChanged += new EventHandler(Content_HiveExperimentChanged);
      Content.IsControllableChanged += new EventHandler(Content_IsControllableChanged);
      Content.JobStatisticsChanged += new EventHandler(Content_JobStatisticsChanged);
      Content.ExceptionOccured += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccured);
      Content.StateLogListChanged += new EventHandler(Content_StateLogListChanged);
      Content.IsProgressingChanged += new EventHandler(Content_IsProgressingChanged);
      Content.HiveTasksChanged += new EventHandler(Content_HiveTasksChanged);
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.IsAllowedPrivilegedChanged += new EventHandler(Content_IsAllowedPrivilegedChanged);
      Content.Loaded += new EventHandler(Content_Loaded);
    }

    protected override void DeregisterContentEvents() {
      Content.RefreshAutomaticallyChanged -= new EventHandler(Content_RefreshAutomaticallyChanged);
      Content.JobChanged -= new EventHandler(Content_HiveExperimentChanged);
      Content.IsControllableChanged -= new EventHandler(Content_IsControllableChanged);
      Content.JobStatisticsChanged -= new EventHandler(Content_JobStatisticsChanged);
      Content.ExceptionOccured -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccured);
      Content.StateLogListChanged -= new EventHandler(Content_StateLogListChanged);
      Content.IsProgressingChanged -= new EventHandler(Content_IsProgressingChanged);
      Content.HiveTasksChanged -= new EventHandler(Content_HiveTasksChanged);
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.Loaded -= new EventHandler(Content_Loaded);
      base.DeregisterContentEvents();
    }

    private void RegisterHiveExperimentEvents() {
      Content.Job.PropertyChanged += new PropertyChangedEventHandler(HiveExperiment_PropertyChanged);
    }

    private void DeregisterHiveExperimentEvents() {
      Content.Job.PropertyChanged -= new PropertyChangedEventHandler(HiveExperiment_PropertyChanged);
    }

    private void RegisterHiveJobEvents() {
      Content.HiveTasks.ItemsAdded += new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsAdded);
      Content.HiveTasks.ItemsRemoved += new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsRemoved);
      Content.HiveTasks.CollectionReset += new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_CollectionReset);
    }
    private void DeregisterHiveJobEvents() {
      Content.HiveTasks.ItemsAdded -= new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsAdded);
      Content.HiveTasks.ItemsRemoved -= new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsRemoved);
      Content.HiveTasks.CollectionReset -= new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_CollectionReset);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        nameTextBox.Text = string.Empty;
        executionTimeTextBox.Text = string.Empty;
        resourceNamesTextBox.Text = string.Empty;
        isPrivilegedCheckBox.Checked = false;
        logView.Content = null;
        refreshAutomaticallyCheckBox.Checked = false;
        logView.Content = null;
        runCollectionViewHost.Content = null;
      } else {
        nameTextBox.Text = Content.Job.Name;
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
        resourceNamesTextBox.Text = Content.Job.ResourceNames;
        isPrivilegedCheckBox.Checked = Content.Job.IsPrivileged;
        refreshAutomaticallyCheckBox.Checked = Content.RefreshAutomatically;
        logView.Content = Content.Log;
        runCollectionViewHost.Content = GetAllRunsFromJob(Content);
      }
      hiveExperimentPermissionListView.Content = null; // has to be filled by refresh button
      Content_JobStatisticsChanged(this, EventArgs.Empty);
      Content_HiveExperimentChanged(this, EventArgs.Empty);
      Content_HiveTasksChanged(this, EventArgs.Empty);
      Content_IsProgressingChanged(this, EventArgs.Empty);
      Content_StateLogListChanged(this, EventArgs.Empty);
      HiveExperiment_PropertyChanged(this, new PropertyChangedEventArgs("Id"));
      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      executionTimeTextBox.Enabled = Content != null;
      jobsTextBox.ReadOnly = true;
      calculatingTextBox.ReadOnly = true;
      finishedTextBox.ReadOnly = true;

      if (Content != null) {
        bool alreadyUploaded = Content.Id != Guid.Empty;
        bool jobsLoaded = Content.HiveTasks != null && Content.HiveTasks.All(x => x.Task.Id != Guid.Empty);

        this.nameTextBox.ReadOnly = !Content.IsControllable || Content.ExecutionState != ExecutionState.Prepared || alreadyUploaded;
        this.resourceNamesTextBox.ReadOnly = !Content.IsControllable || Content.ExecutionState != ExecutionState.Prepared || alreadyUploaded;
        this.jobsTreeView.ReadOnly = !Content.IsControllable || Content.ExecutionState != ExecutionState.Prepared || alreadyUploaded;

        this.isPrivilegedCheckBox.Enabled = Content.IsAllowedPrivileged && Content.IsControllable && !(Content.ExecutionState != ExecutionState.Prepared || alreadyUploaded); // TODO: check if user has the rights to do this        
        this.refreshAutomaticallyCheckBox.Enabled = Content.IsControllable && alreadyUploaded && jobsLoaded && Content.ExecutionState == ExecutionState.Started;
        this.refreshButton.Enabled = Content.IsDownloadable && alreadyUploaded;
        this.Locked = !Content.IsControllable || Content.ExecutionState == ExecutionState.Started;
      }
      SetEnabledStateOfExecutableButtons();
      tabControl_SelectedIndexChanged(this, EventArgs.Empty); // ensure sharing tabpage is disabled
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      if (Content != null) {
        if (Content.RefreshAutomatically)
          Content.StopResultPolling();
      }
      base.OnClosed(e);
    }

    #region Content Events
    private void HiveTasks_ItemsAdded(object sender, CollectionItemsChangedEventArgs<HiveTask> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsAdded), sender, e);
      else {
        foreach (HiveTask t in e.Items) {
          t.TaskStateChanged += new EventHandler(HiveTask_TaskStateChanged);
        }
        SetEnabledStateOfControls();
      }
    }

    private void HiveTasks_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<HiveTask> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsRemoved), sender, e);
      else {
        foreach (HiveTask t in e.Items) {
          t.TaskStateChanged -= new EventHandler(HiveTask_TaskStateChanged);
        }
        SetEnabledStateOfControls();
      }
    }

    private void HiveTasks_CollectionReset(object sender, CollectionItemsChangedEventArgs<HiveTask> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_CollectionReset), sender, e);
      else {
        foreach (HiveTask t in e.Items) {
          t.TaskStateChanged -= new EventHandler(HiveTask_TaskStateChanged);
        }
        SetEnabledStateOfControls();
      }
    }

    void HiveTask_TaskStateChanged(object sender, EventArgs e) {
      runCollectionViewHost.Content = GetAllRunsFromJob(Content);
    }

    private void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionStateChanged), sender, e);
      else
        SetEnabledStateOfControls();
    }
    private void Content_Prepared(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Prepared), sender, e);
      else {
        nameTextBox.Enabled = true;
        Locked = false;
        SetEnabledStateOfControls();
      }
    }
    private void Content_Started(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Started), sender, e);
      else {
        nameTextBox.Enabled = false;
        SetEnabledStateOfControls();
      }
    }
    private void Content_Paused(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Paused), sender, e);
      else {
        nameTextBox.Enabled = true;
        SetEnabledStateOfControls();
      }
    }
    private void Content_Stopped(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Stopped), sender, e);
      else {
        nameTextBox.Enabled = true;
        Locked = false;
        SetEnabledStateOfControls();
      }
    }
    private void Content_ExecutionTimeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionTimeChanged), sender, e);
      else
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
    }
    private void Content_RefreshAutomaticallyChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_RefreshAutomaticallyChanged), sender, e);
      else {
        refreshAutomaticallyCheckBox.Checked = Content.RefreshAutomatically;
        SetEnabledStateOfControls();
      }
    }
    private void Content_HiveTasksChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_HiveTasksChanged), sender, e);
      else {
        if (Content != null && Content.HiveTasks != null) {
          jobsTreeView.Content = Content.HiveTasks;
          RegisterHiveJobEvents();
        } else {
          jobsTreeView.Content = null;
        }
        SetEnabledStateOfControls();
      }
    }

    void Content_Loaded(object sender, EventArgs e) {
      runCollectionViewHost.Content = GetAllRunsFromJob(Content);
    }

    private void Content_HiveExperimentChanged(object sender, EventArgs e) {
      if (Content != null && Content.Job != null) {
        RegisterHiveExperimentEvents();
        Content_IsProgressingChanged(sender, e);
      }
    }
    private void Content_IsControllableChanged(object sender, EventArgs e) {
      SetEnabledStateOfControls();
    }
    private void Content_JobStatisticsChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_JobStatisticsChanged), sender, e);
      else {
        if (Content != null) {
          jobsTextBox.Text = (Content.Job.JobCount - Content.Job.CalculatingCount - Content.Job.FinishedCount).ToString();
          calculatingTextBox.Text = Content.Job.CalculatingCount.ToString();
          finishedTextBox.Text = Content.Job.FinishedCount.ToString();
        } else {
          jobsTextBox.Text = "0";
          calculatingTextBox.Text = "0";
          finishedTextBox.Text = "0";
        }
      }
    }
    private void Content_ExceptionOccured(object sender, EventArgs<Exception> e) {
      // don't show exception, it is logged anyway      
    }
    private void Content_StateLogListChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_StateLogListChanged), sender, e);
      else {
        UpdateStateLogList();
      }
    }
    private void Content_IsAllowedPrivilegedChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_IsAllowedPrivilegedChanged), sender, e);
      else {
        SetEnabledStateOfControls();
      }
    }


    private void UpdateStateLogList() {
      if (Content != null && this.Content.Job != null) {
        stateLogViewHost.Content = this.Content.StateLogList;
      } else {
        stateLogViewHost.Content = null;
      }
    }

    private void HiveExperiment_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (this.Content != null && e.PropertyName == "Id") this.hiveExperimentPermissionListView.HiveExperimentId = this.Content.Job.Id;
    }
    #endregion

    #region Control events
    private void startButton_Click(object sender, EventArgs e) {
      if (nameTextBox.Text.Trim() == string.Empty) {
        MessageBox.Show("Please enter a name for the job before uploading it!", "HeuristicLab Hive Job Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } else {
        HiveClient.StartJob((Exception ex) => ErrorHandling.ShowErrorDialog(this, "Start failed.", ex), Content, new CancellationToken());
      }
    }
    private void pauseButton_Click(object sender, EventArgs e) {
      HiveClient.PauseJob(Content);
    }
    private void stopButton_Click(object sender, EventArgs e) {
      HiveClient.StopJob(Content);
    }
    private void resetButton_Click(object sender, EventArgs e) { }

    private void nameTextBox_Validated(object sender, EventArgs e) {
      if (Content.Job.Name != nameTextBox.Text)
        Content.Job.Name = nameTextBox.Text;
    }

    private void resourceNamesTextBox_Validated(object sender, EventArgs e) {
      if (Content.Job.ResourceNames != resourceNamesTextBox.Text)
        Content.Job.ResourceNames = resourceNamesTextBox.Text;
    }

    private void refreshAutomaticallyCheckBox_Validated(object sender, EventArgs e) {
      if (Content != null) Content.RefreshAutomatically = refreshAutomaticallyCheckBox.Checked;
    }

    private void isPrivilegedCheckBox_Validated(object sender, EventArgs e) {
      if (Content != null) Content.Job.IsPrivileged = isPrivilegedCheckBox.Checked;
    }

    private void refreshButton_Click(object sender, EventArgs e) {
      var invoker = new Action<RefreshableJob>(HiveClient.LoadJob);
      invoker.BeginInvoke(Content, (ar) => {
        try {
          invoker.EndInvoke(ar);
        }
        catch (Exception ex) {
          ThreadPool.QueueUserWorkItem(delegate(object exception) { ErrorHandling.ShowErrorDialog(this, (Exception)exception); }, ex);
        }
      }, null);
    }

    private void refreshPermissionsButton_Click(object sender, EventArgs e) {
      if (this.Content.Job.Id == Guid.Empty) {
        MessageBox.Show("You have to upload the Job first before you can share it.", "HeuristicLab Hive Job Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } else {
        hiveExperimentPermissionListView.Content = HiveClient.GetJobPermissions(this.Content.Job.Id);
      }
    }


    #endregion

    #region Helpers
    private void SetEnabledStateOfExecutableButtons() {
      if (Content == null) {
        startButton.Enabled = pauseButton.Enabled = stopButton.Enabled = resetButton.Enabled = false;
      } else {
        startButton.Enabled = Content.IsControllable && Content.HiveTasks != null && Content.HiveTasks.Count > 0 && Content.ExecutionState == ExecutionState.Prepared;
        pauseButton.Enabled = Content.IsControllable && Content.ExecutionState == ExecutionState.Started;
        stopButton.Enabled = Content.IsControllable && Content.ExecutionState == ExecutionState.Started;
        resetButton.Enabled = false;
      }
    }
    #endregion

    #region Progress reporting
    private void Content_IsProgressingChanged(object sender, EventArgs e) {
      if (this.InvokeRequired) {
        Invoke(new EventHandler(Content_IsProgressingChanged), sender, e);
      } else {
        if (Content != null && Content.IsProgressing) {
          SetProgressView();
        } else {
          FinishProgressView();
        }
      }
    }

    private void SetProgressView() {
      if (progressView == null) {
        progressView = new ProgressView(this, Content.Progress);
      }
      progressView.Progress = Content.Progress;
    }

    private void FinishProgressView() {
      if (progressView != null) {
        progressView.Finish();
        progressView = null;
        SetEnabledStateOfControls();
      }
    }
    #endregion

    #region Drag & Drop
    private void jobsTreeView_DragOver(object sender, DragEventArgs e) {
      jobsTreeView_DragEnter(sender, e);
    }

    private void jobsTreeView_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      var obj = e.Data.GetData(Constants.DragDropDataFormat);
      if (obj is IOptimizer) {
        if (Content.Id != Guid.Empty) e.Effect = DragDropEffects.None;
        else if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
      }
    }

    private void jobsTreeView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        var obj = e.Data.GetData(Constants.DragDropDataFormat);

        var optimizer = obj as IOptimizer;
        if (optimizer != null) {
          IOptimizer newOptimizer = null;
          if (e.Effect.HasFlag(DragDropEffects.Copy)) {
            newOptimizer = (IOptimizer)optimizer.Clone();
          } else {
            newOptimizer = optimizer;
          }
          if (newOptimizer.ExecutionState != ExecutionState.Prepared) {
            newOptimizer.Prepare();
          }

          Content.HiveTasks.Add(new OptimizerHiveTask(newOptimizer));
        }
      }
    }
    #endregion

    private void tabControl_SelectedIndexChanged(object sender, EventArgs e) {
      if (tabControl.SelectedTab == permissionTabPage) {
        if (!Content.IsSharable) {
          MessageBox.Show("Unable to load tab. You have insufficient access privileges.");
          tabControl.SelectedTab = tasksTabPage;
        }
      }
    }

    private RunCollection GetAllRunsFromJob(RefreshableJob job) {
      if (job != null) {
        RunCollection runs = new RunCollection();

        foreach (HiveTask subTask in job.HiveTasks) {
          if (subTask is OptimizerHiveTask) {
            OptimizerHiveTask ohTask = subTask as OptimizerHiveTask;
            runs.AddRange(ohTask.ItemTask.Item.Runs);
          }
        }
        return runs;
      } else {
        return null;
      }
    }
  }
}
