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

using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("Hive Job View")]
  [Content(typeof(RefreshableJob), true)]
  public partial class RefreshableHiveJobView : HeuristicLab.Core.Views.ItemView {
    private HiveResourceSelectorDialog hiveResourceSelectorDialog;
    private bool SuppressEvents { get; set; }
    private object runCollectionViewLocker = new object();

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
      Content.HiveTasksChanged += new EventHandler(Content_HiveTasksChanged);
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.Loaded += new EventHandler(Content_Loaded);
      Content.TaskReceived += new EventHandler(Content_TaskReceived);
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().AddOperationProgressToView(this, Content.Progress);
    }

    protected override void DeregisterContentEvents() {
      Content.RefreshAutomaticallyChanged -= new EventHandler(Content_RefreshAutomaticallyChanged);
      Content.JobChanged -= new EventHandler(Content_HiveExperimentChanged);
      Content.IsControllableChanged -= new EventHandler(Content_IsControllableChanged);
      Content.JobStatisticsChanged -= new EventHandler(Content_JobStatisticsChanged);
      Content.ExceptionOccured -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccured);
      Content.StateLogListChanged -= new EventHandler(Content_StateLogListChanged);
      Content.HiveTasksChanged -= new EventHandler(Content_HiveTasksChanged);
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.Loaded -= new EventHandler(Content_Loaded);
      Content.TaskReceived -= new EventHandler(Content_TaskReceived);
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this, false);
      DeregisterHiveExperimentEvents();
      DeregisterHiveTasksEvents();
      base.DeregisterContentEvents();
    }

    private void RegisterHiveExperimentEvents() {
      Content.Job.PropertyChanged += new PropertyChangedEventHandler(HiveExperiment_PropertyChanged);
    }

    private void DeregisterHiveExperimentEvents() {
      Content.Job.PropertyChanged -= new PropertyChangedEventHandler(HiveExperiment_PropertyChanged);
    }

    private void RegisterHiveTasksEvents() {
      Content.HiveTasks.ItemsAdded += new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsAdded);
      Content.HiveTasks.ItemsRemoved += new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsRemoved);
      Content.HiveTasks.CollectionReset += new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_CollectionReset);
    }
    private void DeregisterHiveTasksEvents() {
      Content.HiveTasks.ItemsAdded -= new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsAdded);
      Content.HiveTasks.ItemsRemoved -= new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsRemoved);
      Content.HiveTasks.CollectionReset -= new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_CollectionReset);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      SuppressEvents = true;
      try {
        if (Content == null) {
          nameTextBox.Text = string.Empty;
          executionTimeTextBox.Text = string.Empty;
          resourceNamesTextBox.Text = string.Empty;
          refreshAutomaticallyCheckBox.Checked = false;
          lock (runCollectionViewLocker) {
            runCollectionViewHost.Content = null;
          }
          logView.Content = null;
          jobsTreeView.Content = null;
          hiveExperimentPermissionListView.Content = null;
          stateLogViewHost.Content = null;
        } else {
          nameTextBox.Text = Content.Job.Name;
          executionTimeTextBox.Text = Content.ExecutionTime.ToString();
          resourceNamesTextBox.Text = Content.Job.ResourceNames;
          refreshAutomaticallyCheckBox.Checked = Content.RefreshAutomatically;
          logView.Content = Content.Log;
          lock (runCollectionViewLocker) {
            runCollectionViewHost.Content = GetAllRunsFromJob(Content);
          }
        }
      }
      finally {
        SuppressEvents = false;
      }
      hiveExperimentPermissionListView.Content = null; // has to be filled by refresh button
      Content_JobStatisticsChanged(this, EventArgs.Empty);
      Content_HiveExperimentChanged(this, EventArgs.Empty);
      Content_HiveTasksChanged(this, EventArgs.Empty);
      Content_StateLogListChanged(this, EventArgs.Empty);
      HiveExperiment_PropertyChanged(this, new PropertyChangedEventArgs("Id"));
      SetEnabledStateOfControls();
    }

    protected override void OnLockedChanged() {
      base.OnLockedChanged();
      executionTimeTextBox.Enabled = !Locked;
      jobsTextBox.Enabled = !Locked;
      calculatingTextBox.Enabled = !Locked;
      finishedTextBox.Enabled = !Locked;
      tabControl.Enabled = !Locked;
      nameTextBox.Enabled = !Locked;
      resourceNamesTextBox.Enabled = !Locked;
      searchButton.Enabled = !Locked;
      jobsTreeView.Enabled = !Locked;
      refreshAutomaticallyCheckBox.Enabled = !Locked;
      refreshButton.Enabled = !Locked;
      UnloadButton.Enabled = !Locked;
      startButton.Enabled = !Locked;
      pauseButton.Enabled = !Locked;
      stopButton.Enabled = !Locked;
      resetButton.Enabled = !Locked;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (!Locked) {
        executionTimeTextBox.Enabled = Content != null;
        jobsTextBox.ReadOnly = true;
        calculatingTextBox.ReadOnly = true;
        finishedTextBox.ReadOnly = true;

        if (Content != null) {
          bool alreadyUploaded = Content.Id != Guid.Empty;
          bool jobsLoaded = Content.HiveTasks != null && Content.HiveTasks.All(x => x.Task.Id != Guid.Empty);
          tabControl.Enabled = !Content.IsProgressing;

          this.nameTextBox.ReadOnly = !Content.IsControllable || Content.ExecutionState != ExecutionState.Prepared || alreadyUploaded || Content.IsProgressing;
          this.resourceNamesTextBox.ReadOnly = !Content.IsControllable || Content.ExecutionState != ExecutionState.Prepared || alreadyUploaded || Content.IsProgressing;
          this.searchButton.Enabled = Content.IsControllable && Content.ExecutionState == ExecutionState.Prepared && !alreadyUploaded && !Content.IsProgressing;
          this.jobsTreeView.ReadOnly = !Content.IsControllable || Content.ExecutionState != ExecutionState.Prepared || alreadyUploaded || Content.IsProgressing;

          this.refreshAutomaticallyCheckBox.Enabled = Content.IsControllable && alreadyUploaded && jobsLoaded && Content.ExecutionState == ExecutionState.Started && !Content.IsProgressing;
          this.refreshButton.Enabled = Content.IsDownloadable && alreadyUploaded && !Content.IsProgressing;

          this.UnloadButton.Enabled = Content.HiveTasks != null && Content.HiveTasks.Count > 0 && alreadyUploaded && !Content.IsProgressing;
        }
        SetEnabledStateOfExecutableButtons();
        tabControl_SelectedIndexChanged(this, EventArgs.Empty); // ensure sharing tabpage is disabled
      }
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      if (Content != null) {
        if (Content.RefreshAutomatically)
          Content.StopResultPolling();
      }
      base.OnClosed(e);
    }

    #region Content Events
    void Content_TaskReceived(object sender, EventArgs e) {
      lock (runCollectionViewLocker) {
        runCollectionViewHost.Content = GetAllRunsFromJob(Content);
      }
    }

    private void HiveTasks_ItemsAdded(object sender, CollectionItemsChangedEventArgs<HiveTask> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsAdded), sender, e);
      else {
        SetEnabledStateOfControls();
      }
    }

    private void HiveTasks_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<HiveTask> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_ItemsRemoved), sender, e);
      else {
        SetEnabledStateOfControls();
      }
    }

    private void HiveTasks_CollectionReset(object sender, CollectionItemsChangedEventArgs<HiveTask> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<HiveTask>(HiveTasks_CollectionReset), sender, e);
      else {
        SetEnabledStateOfControls();
      }
    }

    private void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionStateChanged), sender, e);
      else
        SetEnabledStateOfControls();
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
          RegisterHiveTasksEvents();
        } else {
          jobsTreeView.Content = null;
        }
        SetEnabledStateOfControls();
      }
    }

    void Content_Loaded(object sender, EventArgs e) {
      lock (runCollectionViewLocker) {
        runCollectionViewHost.Content = GetAllRunsFromJob(Content);
      }
    }

    private void Content_HiveExperimentChanged(object sender, EventArgs e) {
      if (Content != null && Content.Job != null) {
        RegisterHiveExperimentEvents();
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
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Content_ExceptionOccured), sender, e);
      else {
        //don't show the error dialog when downloading tasks, the HiveClient will throw an exception and the dialog will be shown then
        if (sender.GetType() != typeof(ConcurrentTaskDownloader<ItemTask>) && sender.GetType() != typeof(TaskDownloader)) {
          ErrorHandling.ShowErrorDialog(this, e.Value);
        }
      }
    }
    private void Content_StateLogListChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_StateLogListChanged), sender, e);
      else {
        UpdateStateLogList();
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
    private void searchButton_Click(object sender, EventArgs e) {
      if (hiveResourceSelectorDialog == null)
        hiveResourceSelectorDialog = new HiveResourceSelectorDialog();
      if (hiveResourceSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        StringBuilder sb = new StringBuilder();
        foreach (Resource resource in hiveResourceSelectorDialog.GetSelectedResources()) {
          sb.Append(resource.Name);
          sb.Append(";");
        }
        resourceNamesTextBox.Text = sb.ToString();
        if (Content.Job.ResourceNames != resourceNamesTextBox.Text)
          Content.Job.ResourceNames = resourceNamesTextBox.Text;
      }
    }

    private void startButton_Click(object sender, EventArgs e) {
      if (nameTextBox.Text.Trim() == string.Empty) {
        MessageBox.Show("Please enter a name for the job before uploading it!", "HeuristicLab Hive Job Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } else if (Content.ExecutionState == ExecutionState.Paused) {
        var task = System.Threading.Tasks.Task.Factory.StartNew(ResumeJobAsync, Content);
        task.ContinueWith((t) => {
          Content.Progress.Finish();
          MessageBox.Show("An error occured resuming the job. See the log for more information.", "HeuristicLab Hive Job Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
          Content.Log.LogException(t.Exception);
        }, TaskContinuationOptions.OnlyOnFaulted);
      } else {
        HiveClient.StartJob((Exception ex) => ErrorHandling.ShowErrorDialog(this, "Start failed.", ex), Content, new CancellationToken());
      }
    }

    private void pauseButton_Click(object sender, EventArgs e) {
      var task = System.Threading.Tasks.Task.Factory.StartNew(PauseJobAsync, Content);
      task.ContinueWith((t) => {
        Content.Progress.Finish();
        MessageBox.Show("An error occured pausing the job. See the log for more information.", "HeuristicLab Hive Job Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Content.Log.LogException(t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void stopButton_Click(object sender, EventArgs e) {
      var task = System.Threading.Tasks.Task.Factory.StartNew(StopJobAsync, Content);
      task.ContinueWith((t) => {
        Content.Progress.Finish();
        MessageBox.Show("An error occured stopping the job. See the log for more information.", "HeuristicLab Hive Job Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Content.Log.LogException(t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }
    private void resetButton_Click(object sender, EventArgs e) { }

    private void PauseJobAsync(object job) {
      Content.Progress.Start("Pausing job...");
      HiveClient.PauseJob((RefreshableJob)job);
      Content.Progress.Finish();
    }

    private void StopJobAsync(object job) {
      Content.Progress.Start("Stopping job...");
      HiveClient.StopJob((RefreshableJob)job);
      Content.Progress.Finish();
    }

    private void ResumeJobAsync(object job) {
      Content.Progress.Start("Resuming job...");
      HiveClient.ResumeJob((RefreshableJob)job);
      Content.Progress.Finish();
    }

    private void nameTextBox_Validated(object sender, EventArgs e) {
      if (!SuppressEvents && Content.Job != null && Content.Job.Name != nameTextBox.Text)
        Content.Job.Name = nameTextBox.Text;
    }

    private void resourceNamesTextBox_Validated(object sender, EventArgs e) {
      if (!SuppressEvents && Content.Job != null && Content.Job.ResourceNames != resourceNamesTextBox.Text)
        Content.Job.ResourceNames = resourceNamesTextBox.Text;
    }

    private void refreshAutomaticallyCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content != null && !SuppressEvents) Content.RefreshAutomatically = refreshAutomaticallyCheckBox.Checked;
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
        startButton.Enabled = Content.IsControllable && Content.HiveTasks != null && Content.HiveTasks.Count > 0 && (Content.ExecutionState == ExecutionState.Prepared || Content.ExecutionState == ExecutionState.Paused) && !Content.IsProgressing;
        pauseButton.Enabled = Content.IsControllable && Content.ExecutionState == ExecutionState.Started && !Content.IsProgressing;
        stopButton.Enabled = Content.IsControllable && Content.ExecutionState == ExecutionState.Started && !Content.IsProgressing;
        resetButton.Enabled = false;
      }
    }
    #endregion

    #region Drag & Drop
    private void jobsTreeView_DragOver(object sender, DragEventArgs e) {
      jobsTreeView_DragEnter(sender, e);
    }

    private void jobsTreeView_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      var obj = (IDeepCloneable)e.Data.GetData(Constants.DragDropDataFormat);

      Type objType = obj.GetType();
      if (ItemTask.IsTypeSupported(objType)) {
        if (Content.Id != Guid.Empty) e.Effect = DragDropEffects.None;
        else if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
      }
    }

    private void jobsTreeView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        var obj = (IItem)e.Data.GetData(Constants.DragDropDataFormat);

        IItem newObj = null;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) {
          newObj = (IItem)obj.Clone();
        } else {
          newObj = obj;
        }

        //IOptimizer and IExecutables need some special care
        if (newObj is IOptimizer) {
          ((IOptimizer)newObj).Runs.Clear();
        }
        if (newObj is IExecutable) {
          IExecutable exec = (IExecutable)newObj;
          if (exec.ExecutionState != ExecutionState.Prepared) {
            exec.Prepare();
          }
        }

        ItemTask hiveTask = ItemTask.GetItemTaskForItem(newObj);
        Content.HiveTasks.Add(hiveTask.CreateHiveTask());
      }
    }
    #endregion

    private void tabControl_SelectedIndexChanged(object sender, EventArgs e) {
      if (tabControl.SelectedTab == permissionTabPage) {
        if (!Content.IsSharable) {
          MessageBox.Show("Unable to load permissions. You have insufficient access privileges.", "HeuristicLab Hive Job Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
          tabControl.SelectedTab = tasksTabPage;
        }
      }
    }

    private RunCollection GetAllRunsFromJob(RefreshableJob job) {
      if (job != null) {
        RunCollection runs = new RunCollection() { OptimizerName = job.ItemName };

        foreach (HiveTask subTask in job.HiveTasks) {
          if (subTask is OptimizerHiveTask) {
            OptimizerHiveTask ohTask = subTask as OptimizerHiveTask;
            ohTask.ExecuteReadActionOnItemTask(new Action(delegate() {
              runs.AddRange(ohTask.ItemTask.Item.Runs);
            }));
          }
        }
        return runs;
      } else {
        return null;
      }
    }

    private void UnloadButton_Click(object sender, EventArgs e) {
      Content.Unload();
      runCollectionViewHost.Content = null;
      stateLogViewHost.Content = null;
      hiveExperimentPermissionListView.Content = null;
      jobsTreeView.Content = null;

      SetEnabledStateOfControls();
    }
  }
}