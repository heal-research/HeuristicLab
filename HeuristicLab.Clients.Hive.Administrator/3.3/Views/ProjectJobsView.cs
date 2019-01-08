#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.Clients.Hive.Views;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("ProjectView")]
  [Content(typeof(Project), IsDefaultView = false)]
  public partial class ProjectJobsView : ItemView {
    private const string JOB_ID = "Id";
    private const string JOB_NAME = "Name";
    private const string JOB_OWNER = "Owner";
    private const string JOB_OWNERID = "Owner Id";
    private const string JOB_DATECREATED = "Date Created";
    private const string JOB_STATE = "State";
    private const string JOB_EXECUTIONSTATE = "Execution State";
    private const string JOB_EXECUTIONTIME = "Execution Time";
    private const string JOB_DESCRIPTION = "Description";
    private const string JOB_TASKCOUNT = "Tasks";
    private const string JOB_WAITINGTASKCOUNT = "Waiting";
    private const string JOB_CALCULATINGTASKCOUNT = "Calculating";
    private const string JOB_FINISHEDTASKCOUNT = "Finished";

    private readonly Color onlineStatusColor = Color.FromArgb(255, 189, 249, 143); // #bdf98f
    private readonly Color onlineStatusColor2 = Color.FromArgb(255, 157, 249, 143); // #9df98f
    private readonly Color statisticsPendingStatusColor = Color.FromArgb(255, 249, 210, 145); // #f9d291
    private readonly Color deletionPendingStatusColor = Color.FromArgb(255, 249, 172, 144); // #f9ac90
    private readonly Color deletionPendingStatusColor2 = Color.FromArgb(255, 249, 149, 143); // #f9958f

    private IProgress progress;
    public IProgress Progress {
      get { return progress; }
      set {
        this.progress = value;
        OnIsProgressingChanged();
      }
    }

    public new Project Content {
      get { return (Project)base.Content; }
      set { base.Content = value; }
    }

    public ProjectJobsView() {
      InitializeComponent();
      progress = new Progress();

      removeButton.Enabled = false;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      matrixView.DataGridView.SelectionChanged += DataGridView_SelectionChanged;
      MainForm.Progress.Show(this, progress);
    }

    protected override void DeregisterContentEvents() {
      matrixView.DataGridView.SelectionChanged -= DataGridView_SelectionChanged;
      MainForm.Progress.Hide(this, false);
      base.DeregisterContentEvents();
    }

    private void DataGridView_SelectionChanged(object sender, EventArgs e) {
      SetEnabledStateOfControls();
    }

    #region Overrides
    protected override void OnContentChanged() {
      base.OnContentChanged();
      removeButton.Enabled = false;
      UpdateJobs();
      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool enabled = Content != null && !Locked && !ReadOnly;

      matrixView.Enabled = enabled;

      // Buttons (start/resume, pause, stop, remove)
      refreshButton.Enabled = startButton.Enabled = pauseButton.Enabled = stopButton.Enabled = removeButton.Enabled = false;

      if (enabled && progress.ProgressState != ProgressState.Started) {
        var jobs = GetSelectedJobs().ToList();
        if (jobs.Any()) {

          startButton.Enabled = jobs.All(x =>
            !x.IsProgressing && HiveAdminClient.Instance.Tasks.ContainsKey(x.Id) && HiveAdminClient.Instance.Tasks[x.Id].Count > 0
            && x.Job.ProjectId != Guid.Empty //&& x.Job.ResourceIds != null && x.Job.ResourceIds.Any()
            && (x.ExecutionState == ExecutionState.Prepared || x.ExecutionState == ExecutionState.Paused));
          pauseButton.Enabled = jobs.All(x => !x.IsProgressing && x.ExecutionState == ExecutionState.Started);
          stopButton.Enabled = jobs.All(x => !x.IsProgressing && (x.ExecutionState == ExecutionState.Started || x.ExecutionState == ExecutionState.Paused));
          removeButton.Enabled = jobs.All(x => !x.IsProgressing && x.Job.State == JobState.Online);
        }
      }

      // refresh Button
      if (Content != null && !Locked && progress.ProgressState != ProgressState.Started) {
        refreshButton.Enabled = true;
      }
    }

    #endregion Overrides

    #region Event Handlers
    private void ProjectJobsView_Load(object sender, EventArgs e) {

    }

    private void refreshButton_Click(object sender, EventArgs e) {
      progress.Start("Refreshing jobs...", ProgressMode.Indeterminate);
      SetEnabledStateOfControls();
      var task = System.Threading.Tasks.Task.Factory.StartNew(RefreshJobsAsync);

      task.ContinueWith((t) => {
        progress.Finish();
        SetEnabledStateOfControls();
      });
    }

    private void removeButton_Click(object sender, EventArgs e) {
      var jobs = GetSelectedJobs();

      if (jobs.Any()) {
        var result = MessageBox.Show("Do you really want to remove following job(s):\n\n"
                                     + String.Join("\n", jobs.Select(x => x.Job.Name)),
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question);

        if (result == DialogResult.Yes) {
          progress.Start("Removing job(s)...", ProgressMode.Indeterminate);
          SetEnabledStateOfControls();
          var task = System.Threading.Tasks.Task.Factory.StartNew(RemoveJobsAsync, jobs);

          task.ContinueWith((t) => {
            RefreshJobs();
            progress.Finish();
            SetEnabledStateOfControls();
          }, TaskContinuationOptions.NotOnFaulted);

          task.ContinueWith((t) => {
            RefreshJobs();
            progress.Finish();
            SetEnabledStateOfControls();
            MessageBox.Show("An error occured removing the job(s).", "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }, TaskContinuationOptions.OnlyOnFaulted);
        }
      }
    }

    private void startButton_Click(object sender, EventArgs e) {
      var jobs = GetSelectedJobs();

      if (jobs.Any()) {
        var result = MessageBox.Show("Do you really want to resume following job(s):\n\n"
                                     + String.Join("\n", jobs.Select(x => x.Job.Name)),
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question);

        if (result == DialogResult.Yes) {
          progress.Start("Resuming job(s)...", ProgressMode.Indeterminate);
          SetEnabledStateOfControls();
          var task = System.Threading.Tasks.Task.Factory.StartNew(ResumeJobsAsync, jobs);

          task.ContinueWith((t) => {
            RefreshJobs();
            progress.Finish();
            SetEnabledStateOfControls();
          }, TaskContinuationOptions.NotOnFaulted);

          task.ContinueWith((t) => {
            RefreshJobs();
            progress.Finish();
            SetEnabledStateOfControls();
            MessageBox.Show("An error occured resuming the job(s).", "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }, TaskContinuationOptions.OnlyOnFaulted);
        }
      }
    }

    private void pauseButton_Click(object sender, EventArgs e) {
      var jobs = GetSelectedJobs();

      if (jobs.Any()) {
        var result = MessageBox.Show("Do you really want to pause following job(s):\n\n"
                                     + String.Join("\n", jobs.Select(x => x.Job.Name)),
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question);

        if (result == DialogResult.Yes) {
          progress.Start("Pausing job(s)...");
          SetEnabledStateOfControls();
          var task = System.Threading.Tasks.Task.Factory.StartNew(PauseJobsAsync, jobs);

          task.ContinueWith((t) => {
            RefreshJobs();
            progress.Finish();
            SetEnabledStateOfControls();
          }, TaskContinuationOptions.NotOnFaulted);

          task.ContinueWith((t) => {
            RefreshJobs();
            progress.Finish();
            SetEnabledStateOfControls();
            MessageBox.Show("An error occured pausing the job(s).", "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }, TaskContinuationOptions.OnlyOnFaulted);
        }
      }
    }

    private void stopButton_Click(object sender, EventArgs e) {
      var jobs = GetSelectedJobs();

      if (jobs.Any()) {
        var result = MessageBox.Show("Do you really want to stop following job(s):\n\n"
                                     + String.Join("\n", jobs.Select(x => x.Job.Name)),
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question);

        if (result == DialogResult.Yes) {
          progress.Start("Stopping job(s)...", ProgressMode.Indeterminate);
          SetEnabledStateOfControls();
          var task = System.Threading.Tasks.Task.Factory.StartNew(StopJobsAsync, jobs);

          task.ContinueWith((t) => {
            RefreshJobs();
            progress.Finish();
            SetEnabledStateOfControls();
          }, TaskContinuationOptions.NotOnFaulted);

          task.ContinueWith((t) => {
            RefreshJobs();
            progress.Finish();
            SetEnabledStateOfControls();
            MessageBox.Show("An error occured stopping the job(s).", "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }, TaskContinuationOptions.OnlyOnFaulted);
        }
      }
    }

    public event EventHandler IsProgressingChanged;
    private void OnIsProgressingChanged() {
      var handler = IsProgressingChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion Event Handlers

    #region Helpers

    private IEnumerable<RefreshableJob> GetSelectedJobs() {
      if (Content == null || matrixView.DataGridView.SelectedRows == null || matrixView.DataGridView.SelectedRows.Count == 0)
        return Enumerable.Empty<RefreshableJob>();

      var jobs = new List<RefreshableJob>();
      foreach (DataGridViewRow r in matrixView.DataGridView.SelectedRows) {
        if (((string)r.Cells[0].Value) == JobState.Online.ToString()) {
          jobs.Add(HiveAdminClient.Instance.Jobs[Content.Id].FirstOrDefault(x => x.Id == Guid.Parse((string)r.Cells[11].Value)));
        }
      }

      return jobs;
    }

    private void RefreshJobs() {
      HiveAdminClient.Instance.RefreshJobs();
      UpdateJobs();
      SetEnabledStateOfControls();
    }

    private StringMatrix CreateValueMatrix() {
      if (Content == null || Content.Id == Guid.Empty)
        return new StringMatrix();

      var jobs = HiveAdminClient.Instance.Jobs[Content.Id];
      var resources = HiveAdminClient.Instance.Resources;
      string[,] values = new string[jobs.Count, 13];

      for (int i = 0; i < jobs.Count; i++) {
        var job = jobs.ElementAt(i);
        values[i, 0] = job.Job.State.ToString();
        values[i, 1] = job.ExecutionState.ToString();
        values[i, 2] = job.ExecutionTime.ToString();
        values[i, 3] = job.Job.DateCreated.ToString();
        values[i, 4] = job.Job.OwnerUsername;
        values[i, 5] = job.Job.Name;
        values[i, 6] = job.Job.JobCount.ToString();
        values[i, 7] = (job.Job.JobCount - job.Job.CalculatingCount - job.Job.FinishedCount).ToString();
        values[i, 8] = job.Job.CalculatingCount.ToString();
        values[i, 9] = job.Job.FinishedCount.ToString();
        values[i, 10] = job.Job.Description;
        values[i, 11] = job.Job.Id.ToString();
        values[i, 12] = job.Job.OwnerUserId.ToString();
      }

      var matrix = new StringMatrix(values);
      matrix.ColumnNames = new string[] { JOB_STATE, JOB_EXECUTIONSTATE, JOB_EXECUTIONTIME, JOB_DATECREATED, JOB_OWNER, JOB_NAME, JOB_TASKCOUNT, JOB_WAITINGTASKCOUNT, JOB_CALCULATINGTASKCOUNT, JOB_FINISHEDTASKCOUNT, JOB_DESCRIPTION, JOB_ID, JOB_OWNERID };
      matrix.SortableView = true;
      return matrix;
    }

    private void UpdateJobs() {
      if (InvokeRequired) Invoke((Action)UpdateJobs);
      else {
        if (Content != null && Content.Id != null && Content.Id != Guid.Empty) {
          var matrix = CreateValueMatrix();
          matrixView.Content = matrix;
          if (matrix != null) {
            foreach (DataGridViewRow row in matrixView.DataGridView.Rows) {
              string val = ((string)row.Cells[0].Value);
              if (val == JobState.Online.ToString()) {
                row.DefaultCellStyle.BackColor = onlineStatusColor;
              } else if (val == JobState.StatisticsPending.ToString()) {
                row.DefaultCellStyle.BackColor = statisticsPendingStatusColor;
              } else if (val == JobState.DeletionPending.ToString()) {
                row.DefaultCellStyle.BackColor = deletionPendingStatusColor;
              }
            }

            matrixView.DataGridView.AutoResizeColumns();
            matrixView.DataGridView.Columns[0].MinimumWidth = 90;
            matrixView.DataGridView.Columns[1].MinimumWidth = 108;
          }
        }
      }
    }

    private void RefreshJobsAsync() {
      HiveAdminClient.Instance.RefreshJobs();
      UpdateJobs();
    }

    private void ResumeJobsAsync(object jobs) {
      var jobList = (IEnumerable<RefreshableJob>)jobs;
      foreach (var job in jobList) {
        progress.Message = "Resuming job \"" + job.Job.Name + "\"...";
        HiveAdminClient.ResumeJob(job);
      }
    }

    private void PauseJobsAsync(object jobs) {
      var jobList = (IEnumerable<RefreshableJob>)jobs;
      foreach (var job in jobList) {
        progress.Message = "Pausing job \"" + job.Job.Name + "\"...";
        HiveAdminClient.PauseJob(job);
      }
    }

    private void StopJobsAsync(object jobs) {
      var jobList = (IEnumerable<RefreshableJob>)jobs;
      foreach (var job in jobList) {
        progress.Message = "Stopping job \"" + job.Job.Name + "\"...";
        HiveAdminClient.StopJob(job);
      }
    }

    private void RemoveJobsAsync(object jobs) {
      var jobList = (IEnumerable<RefreshableJob>)jobs;
      progress.Start("", ProgressMode.Indeterminate);
      foreach (var job in jobList) {
        progress.Message = "Removing job \"" + job.Job.Name + "\"...";
        HiveAdminClient.RemoveJob(job);
      }
      progress.Finish();
    }

    private void ShowHiveInformationDialog() {
      if (InvokeRequired) Invoke((Action)ShowHiveInformationDialog);
      else {
        using (HiveInformationDialog dialog = new HiveInformationDialog()) {
          dialog.ShowDialog(this);
        }
      }
    }
    #endregion Helpers
  }
}
