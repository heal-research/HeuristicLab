#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Clients.Access;
using HeuristicLab.Clients.Common;
using HeuristicLab.Clients.Hive.Views;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using Tpl = System.Threading.Tasks;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("ProjectView")]
  [Content(typeof(Project), IsDefaultView = true)]
  public partial class ProjectView : ItemView {
    public new Project Content {
      get { return (Project)base.Content; }
      set { base.Content = value; }
    }

    public ProjectView() {
      InitializeComponent();

      AccessClient.Instance.Refreshing += AccessClient_Instance_Refreshing;
      AccessClient.Instance.Refreshed += AccessClient_Instance_Refreshed;
    }

    #region Overrides
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += Content_PropertyChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= Content_PropertyChanged;
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateView();
    }

    private void UpdateView() {
      if (Content == null) {
        idTextBox.Clear();
        nameTextBox.Clear();
        descriptionTextBox.Clear();
        ownerComboBox.SelectedItem = null;
        createdTextBox.Clear();
        startDateTimePicker.Value = DateTime.Now;
        endDateTimePicker.Value = startDateTimePicker.Value;
        indefiniteCheckBox.Checked = false;
      } else {
        idTextBox.Text = Content.Id.ToString();
        nameTextBox.Text = Content.Name;
        descriptionTextBox.Text = Content.Description;
        ownerComboBox.SelectedItem = AccessClient.Instance.UsersAndGroups?.OfType<LightweightUser>().SingleOrDefault(x => x.Id == Content.OwnerUserId);
        createdTextBox.Text = Content.DateCreated.ToString("ddd, dd.MM.yyyy, HH:mm:ss");
        startDateTimePicker.Value = Content.StartDate;
        endDateTimePicker.Value = Content.EndDate.GetValueOrDefault(Content.StartDate);
        indefiniteCheckBox.Checked = !Content.EndDate.HasValue;
      }
    }

    private async Tpl.Task UpdateUsersAsync() {
      await Tpl.Task.Run(() => UpdateUsers());
    }

    private void UpdateUsers() {
      // deregister handler to avoid change of content's owner when data source is updated
      ownerComboBox.SelectedIndexChanged -= ownerComboBox_SelectedIndexChanged;
      try {
        ownerComboBox.DataSource = null;
        SecurityExceptionUtil.TryAndReportSecurityExceptions(AccessClient.Instance.Refresh);
        ownerComboBox.DataSource = AccessClient.Instance.UsersAndGroups.OfType<LightweightUser>().OrderBy(x => x.UserName).ToList();
      } catch (AnonymousUserException) {
        ShowHiveInformationDialog();
      } finally {
        ownerComboBox.SelectedIndexChanged += ownerComboBox_SelectedIndexChanged;
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

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();

      bool enabled = Content != null && !Locked && !ReadOnly;

      nameTextBox.Enabled = enabled;
      descriptionTextBox.Enabled = enabled;
      ownerComboBox.Enabled = enabled;
      refreshButton.Enabled = enabled;
      createdTextBox.Enabled = enabled;
      startDateTimePicker.Enabled = enabled;
      endDateTimePicker.Enabled = enabled && Content.EndDate.HasValue && Content.EndDate > Content.StartDate;
      indefiniteCheckBox.Enabled = enabled;

      if (Content == null) return;

      var parentProject = HiveAdminClient.Instance.Projects.SingleOrDefault(x => x.Id == Content.ParentProjectId);
      if (parentProject != null && parentProject.EndDate.HasValue)
        indefiniteCheckBox.Enabled = false;

      if (Content.Id == Guid.Empty) return; // newly created project
      if (HiveRoles.CheckAdminUserPermissions()) return; // admins can edit any project
      if (HiveAdminClient.Instance.CheckOwnershipOfParentProject(Content, UserInformation.Instance.User.Id)) return; // owner can edit project

      // project was already created and user is neither admin nor owner
      ownerComboBox.Enabled = false;
      startDateTimePicker.Enabled = false;
      endDateTimePicker.Enabled = false;
      indefiniteCheckBox.Enabled = false;
    }
    #endregion

    #region Event Handlers
    private void ProjectView_Load(object sender, EventArgs e) {
      Locked = true;
      try {
        UpdateUsers();
        UpdateView();
      } finally {
        Locked = false;
      }
    }

    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      UpdateView();
    }

    private void nameTextBox_TextChanged(object sender, EventArgs e) {
      if (Content == null || Content.Name == nameTextBox.Text) return;
      Content.Name = nameTextBox.Text;
    }

    private void nameTextBox_Validating(object sender, CancelEventArgs e) {
      if (!string.IsNullOrEmpty(nameTextBox.Text)) return;

      MessageBox.Show("Project must have a name.", "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
      e.Cancel = true;
    }

    private void descriptionTextBox_TextChanged(object sender, EventArgs e) {
      if (Content == null || Content.Description == descriptionTextBox.Text) return;
      Content.Description = descriptionTextBox.Text;
    }

    private void ownerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content == null) return;

      var selectedItem = (LightweightUser)ownerComboBox.SelectedItem;
      var selectedOwnerUserId = selectedItem != null ? selectedItem.Id : Guid.Empty;
      if (Content.OwnerUserId == selectedOwnerUserId) return;

      Content.OwnerUserId = selectedOwnerUserId;
    }

    private async void refreshButton_Click(object sender, EventArgs e) {
      Locked = true;
      try {
        await UpdateUsersAsync();
        UpdateView();
      } finally {
        Locked = false;
      }
    }

    private void startDateTimePicker_ValueChanged(object sender, EventArgs e) {
      if (Content == null || Content.StartDate == startDateTimePicker.Value) return;

      string errorMessage = string.Empty;

      var parentProject = HiveAdminClient.Instance.Projects.SingleOrDefault(x => x.Id == Content.ParentProjectId);
      if (parentProject != null) {
        if (startDateTimePicker.Value < parentProject.StartDate) {
          errorMessage = "Project cannot start before its parent project has started.";
        } else if (startDateTimePicker.Value > parentProject.EndDate) {
          errorMessage = "Project cannot start after its parent project has ended.";
        }
      }

      if (startDateTimePicker.Value > endDateTimePicker.Value) {
        errorMessage = "Project cannot start after it ends.";
      }

      if (!string.IsNullOrEmpty(errorMessage)) {
        MessageBox.Show(errorMessage, "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
        startDateTimePicker.Value = Content.StartDate;
      }

      Content.StartDate = startDateTimePicker.Value;
    }

    private void endDateTimePicker_ValueChanged(object sender, EventArgs e) {
      if (Content == null || Content.EndDate == endDateTimePicker.Value || Content.StartDate == endDateTimePicker.Value) return;

      string errorMessage = string.Empty;

      var parentProject = HiveAdminClient.Instance.Projects.SingleOrDefault(x => x.Id == Content.ParentProjectId);
      if (parentProject != null) {
        if (endDateTimePicker.Value > parentProject.EndDate) {
          errorMessage = "Project cannot end after its parent project has ended.";
        } else if (endDateTimePicker.Value < parentProject.StartDate) {
          errorMessage = "Project cannot end before its parent project has started.";
        }
      }

      if (endDateTimePicker.Value < startDateTimePicker.Value) {
        errorMessage = "Project cannot end after it starts.";
      }

      if (!string.IsNullOrEmpty(errorMessage)) {
        MessageBox.Show(errorMessage, "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
        endDateTimePicker.Value = Content.EndDate.GetValueOrDefault(Content.StartDate);
      }

      Content.EndDate = endDateTimePicker.Value;
    }

    private void indefiniteCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content == null) return;

      var newEndDate = indefiniteCheckBox.Checked ? (DateTime?)null : endDateTimePicker.Value;

      if (Content.EndDate == newEndDate) return;
      Content.EndDate = newEndDate;

      endDateTimePicker.Enabled = !indefiniteCheckBox.Checked;
    }

    private void AccessClient_Instance_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)AccessClient_Instance_Refreshing, sender, e);
        return;
      }

      Progress.Show(this, "Refreshing ...", ProgressMode.Indeterminate);
      SetEnabledStateOfControls();
    }

    private void AccessClient_Instance_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)AccessClient_Instance_Refreshed, sender, e);
        return;
      }

      Progress.Hide(this);
      SetEnabledStateOfControls();
    }
    #endregion
  }
}
