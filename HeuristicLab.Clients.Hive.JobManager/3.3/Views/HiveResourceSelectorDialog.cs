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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  public partial class HiveResourceSelectorDialog : Form {
    private readonly object locker = new object();
    private bool updatingProjects = false;

    public Project SelectedProject {
      get { return hiveResourceSelector.SelectedProject; }
    }

    public IEnumerable<Resource> SelectedResources {
      get { return hiveResourceSelector.AssignedResources; }
    }

    private Guid jobId;
    public Guid JobId {
      get { return hiveResourceSelector.JobId; }
      set { jobId = value; }
    }

    // persisted projectId
    private Guid? projectId;
    public Guid? ProjectId {
      get { return hiveResourceSelector.ProjectId; }
      set { projectId = value; }
    }

    // currently selected projectId (initially the persisted projectId)
    private Guid? selectedProjectId;
    public Guid? SelectedProjectId {
      get { return selectedProjectId; }
      set { selectedProjectId = value; }
    }

    // currently selected resourceIds (if null, perform lookup in HiveResourceSelector)
    private IEnumerable<Guid> selectedResourceIds;
    public IEnumerable<Guid> SelectedResourceIds {
      get { return selectedResourceIds; }
      set { selectedResourceIds = value; }
    }



    public HiveResourceSelectorDialog(Guid jobId, Guid projectId) {
      this.jobId = jobId;
      this.projectId = projectId;
      this.selectedProjectId = projectId;
      InitializeComponent();
    }

    #region Overrides
    protected override void OnLoad(EventArgs e) {
      HiveClient.Instance.Refreshing += HiveClient_Instance_Refreshing;
      HiveClient.Instance.Refreshed += HiveClient_Instance_Refreshed;
      base.OnLoad(e);
    }

    protected override void OnClosing(CancelEventArgs e) {
      HiveClient.Instance.Refreshed -= HiveClient_Instance_Refreshed;
      HiveClient.Instance.Refreshing -= HiveClient_Instance_Refreshing;
      base.OnClosing(e);
    }
    #endregion

    #region Event Handlers
    private void HiveClient_Instance_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)HiveClient_Instance_Refreshing, sender, e);
      else {
        refreshButton.Enabled = false;
        okButton.Enabled = false;
        cancelButton.Enabled = false;
        // Progress cannot be shown on dialog (no parent control), thus it is shown on the selector
        Progress.Show(hiveResourceSelector, "Refreshing", ProgressMode.Indeterminate);
      }
    }

    private void HiveClient_Instance_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)HiveClient_Instance_Refreshed, sender, e);
      else {
        Progress.Hide(hiveResourceSelector);
        okButton.Enabled = true;
        cancelButton.Enabled = true;
        refreshButton.Enabled = true;
      }
    }

    private async void HiveResourceSelectorDialog_Load(object sender, EventArgs e) {
      lock (locker) {
        if (updatingProjects) return;
        updatingProjects = true;
      }

      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => UpdateProjects(),
        finallyCallback: () => updatingProjects = false);
    }

    private async void refreshButton_Click(object sender, EventArgs e) {
      lock (locker) {
        if (updatingProjects) return;
        updatingProjects = true;
      }

      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => UpdateProjects(),
        finallyCallback: () => updatingProjects = false);
    }

    private void hiveResourceSelector_SelectedProjectChanged(object sender, EventArgs e) {
      okButton.Enabled = hiveResourceSelector.SelectedProject != null && hiveResourceSelector.AssignedResources.Any();
    }

    private void hiveResourceSelector_SelectedResourcesChanged(object sender, EventArgs e) {
      okButton.Enabled = hiveResourceSelector.AssignedResources.Any();

      if (!hiveResourceSelector.AssignedResources.Any()) {
        errorProvider.SetError(okButton, "Note: currently no resources are assigned");
      } else if (hiveResourceSelector.AssignedCores == 0) {
        errorProvider.SetError(okButton, "Note: currently no resources with cores are assigned");
      } else {
        errorProvider.SetError(okButton, string.Empty);
      }
    }

    private void hiveResourceSelector_ProjectsTreeViewDoubleClicked(object sender, EventArgs e) {
      if (hiveResourceSelector.SelectedProject == null) return;
      if (!hiveResourceSelector.AssignedResources.Any()) return;

      DialogResult = DialogResult.OK;
      Close();
    }
    #endregion

    #region Helpers
    private void UpdateProjects() {
      HiveClient.Instance.RefreshProjectsAndResources();
      hiveResourceSelector.JobId = jobId;
      hiveResourceSelector.ProjectId = projectId;
      hiveResourceSelector.SelectedProjectId = selectedProjectId;
      hiveResourceSelector.SelectedResourceIds = selectedResourceIds;
      hiveResourceSelector.Content = HiveClient.Instance.Projects;
    }

    private void ShowHiveInformationDialog() {
      if (InvokeRequired) Invoke((Action)ShowHiveInformationDialog);
      else {
        using (HiveInformationDialog dialog = new HiveInformationDialog()) {
          dialog.ShowDialog(this);
        }
      }
    }
    #endregion
  }
}
