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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.Clients.Hive.Views;

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
    private const string JOB_DESCRIPTION = "Description";
    private const string JOB_TASKCOUNT = "Tasks";
    private const string JOB_CALCULATINGTASKCOUNT = "Calculating";
    private const string JOB_FINISHEDTASKCOUNT = "Finished";


    private readonly Color onlineStatusColor = Color.FromArgb(255, 189, 249, 143); // #bdf98f
    private readonly Color onlineStatusColor2 = Color.FromArgb(255, 157, 249, 143); // #9df98f
    private readonly Color statisticsPendingStatusColor = Color.FromArgb(255, 249, 210, 145); // #f9d291
    private readonly Color deletionPendingStatusColor = Color.FromArgb(255, 249, 172, 144); // #f9ac90
    private readonly Color deletionPendingStatusColor2 = Color.FromArgb(255, 249, 149, 143); // #f9958f

    public new Project Content {
      get { return (Project)base.Content; }
      set { base.Content = value; }
    }

    public ProjectJobsView() {
      InitializeComponent();

      removeButton.Enabled = false;
      matrixView.DataGridView.SelectionChanged += DataGridView_SelectionChanged;
    }

    private void DataGridView_SelectionChanged(object sender, EventArgs e) {
      if (matrixView.DataGridView.SelectedRows == null || matrixView.DataGridView.SelectedRows.Count == 0) return;

      bool anyDeletable = false;
      foreach (DataGridViewRow r in matrixView.DataGridView.SelectedRows) {
        if (((string)r.Cells[0].Value) == JobState.Online.ToString()) anyDeletable = true;
      }

      removeButton.Enabled = anyDeletable;
    }

    #region Overrides
    protected override void OnContentChanged() {
      base.OnContentChanged();
      removeButton.Enabled = false;
      UpdateJobs();
    }
    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool enabled = Content != null && !Locked && !ReadOnly;

      refreshButton.Enabled = enabled;
      removeButton.Enabled = false;
      matrixView.Enabled = enabled;
    }
    #endregion Overrides

    #region Event Handlers
    private void ProjectJobsView_Load(object sender, EventArgs e) {
      
    }

    private void refreshButton_Click(object sender, EventArgs e) {
      RefreshJobs();
    }

    private async void removeButton_Click(object sender, EventArgs e) {
      if (matrixView.DataGridView.SelectedRows == null || matrixView.DataGridView.SelectedRows.Count == 0) return;

      var jobNamesToDelete = new List<string>();
      var jobIdsToDelete = new List<Guid>();
      foreach (DataGridViewRow r in matrixView.DataGridView.SelectedRows) {
        if(((string)r.Cells[0].Value) == JobState.Online.ToString()) {
          jobNamesToDelete.Add(" - " + ((string)r.Cells[3].Value));
          jobIdsToDelete.Add(Guid.Parse((string)r.Cells[8].Value));
        }
      }

      if(jobIdsToDelete.Any()) {
        var result = MessageBox.Show("Do you really want to remove following job(s):\n\n"
          + String.Join("\n", jobNamesToDelete),
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question);

        if (result == DialogResult.Yes) {
          await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
            action: () => {
              DeleteJobs(jobIdsToDelete);
            },
            finallyCallback: () => {
              matrixView.DataGridView.ClearSelection();
              removeButton.Enabled = false;
              RefreshJobs();
            });
        }
      } 
    }

    #endregion Event Handlers

    #region Helpers
    private void RefreshJobs() {
      HiveAdminClient.Instance.RefreshJobs();
      UpdateJobs();
    }

    private StringMatrix CreateValueMatrix() {
      if (Content == null || Content.Id == Guid.Empty)
        return new StringMatrix();

      var jobs = HiveAdminClient.Instance.Jobs[Content.Id];
      var resources = HiveAdminClient.Instance.Resources;
      string[,] values = new string[jobs.Count, 10];

      for(int i = 0; i < jobs.Count; i++) {
        var job = jobs.ElementAt(i);
        
        values[i, 0] = job.State.ToString();
        values[i, 1] = job.DateCreated.ToString();
        values[i, 2] = job.OwnerUsername;
        values[i, 3] = job.Name;
        values[i, 4] = job.JobCount.ToString();
        values[i, 5] = job.CalculatingCount.ToString();
        values[i, 6] = job.FinishedCount.ToString();
        values[i, 7] = job.Description;        
        values[i, 8] = job.Id.ToString();
        values[i, 9] = job.OwnerUserId.ToString();
      }
      
      var matrix = new StringMatrix(values);
      matrix.ColumnNames = new string[] { JOB_STATE, JOB_DATECREATED, JOB_OWNER, JOB_NAME, JOB_TASKCOUNT, JOB_CALCULATINGTASKCOUNT, JOB_FINISHEDTASKCOUNT, JOB_DESCRIPTION, JOB_ID, JOB_OWNERID };
      matrix.SortableView = true;
      return matrix;
    }
    
    private void UpdateJobs() {
      if (InvokeRequired) Invoke((Action)UpdateJobs);
      else {
        if(Content != null && Content.Id != null && Content.Id != Guid.Empty) {
          var matrix = CreateValueMatrix();
          matrixView.Content = matrix;
          if(matrix != null) {
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
        } else {
          refreshButton.Enabled = false;
          removeButton.Enabled = false;
          matrixView.Content = null;
        }
      }
    }

    private void DeleteJobs(List<Guid> jobIds) {
      try {
        HiveAdminClient.DeleteJobs(jobIds);
      } catch (AnonymousUserException) {
        ShowHiveInformationDialog();
      }
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
