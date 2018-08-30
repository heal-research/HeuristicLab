#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Clients.Hive.Views;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("ProjectView")]
  [Content(typeof(Project), IsDefaultView = true)]
  public partial class ProjectView : ItemView {
    private readonly object locker = new object();

    private Guid persistedOwnerUserId;

    public new Project Content {
      get { return (Project)base.Content; }
      set { base.Content = value; persistedOwnerUserId = Content != null ? Content.OwnerUserId : Guid.Empty; }
    }

    public ProjectView() {
      InitializeComponent();

      AccessClient.Instance.Refreshing += AccessClient_Instance_Refreshing;
      AccessClient.Instance.Refreshed += AccessClient_Instance_Refreshed;
    }

    #region Overrides
    protected override void OnClosing(FormClosingEventArgs e) {
      AccessClient.Instance.Refreshed -= AccessClient_Instance_Refreshed;
      AccessClient.Instance.Refreshing -= AccessClient_Instance_Refreshing;
      base.OnClosing(e);
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += Content_PropertyChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= Content_PropertyChanged;
      base.DeregisterContentEvents();
    }

    protected void RegisterControlEvents() {
      nameTextBox.TextChanged += nameTextBox_TextChanged;
      nameTextBox.Validating += nameTextBox_Validating;
      descriptionTextBox.TextChanged += descriptionTextBox_TextChanged;
      ownerComboBox.SelectedIndexChanged += ownerComboBox_SelectedIndexChanged;
      startDateTimePicker.ValueChanged += startDateTimePicker_ValueChanged;
      endDateTimePicker.ValueChanged += endDateTimePicker_ValueChanged;
      indefiniteCheckBox.CheckedChanged += indefiniteCheckBox_CheckedChanged;
    }

    protected void DeregisterControlEvents() {
      nameTextBox.TextChanged -= nameTextBox_TextChanged;
      nameTextBox.Validating -= nameTextBox_Validating;
      descriptionTextBox.TextChanged -= descriptionTextBox_TextChanged;
      ownerComboBox.SelectedIndexChanged -= ownerComboBox_SelectedIndexChanged;
      startDateTimePicker.ValueChanged -= startDateTimePicker_ValueChanged;
      endDateTimePicker.ValueChanged -= endDateTimePicker_ValueChanged;
      indefiniteCheckBox.CheckedChanged -= indefiniteCheckBox_CheckedChanged;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      DeregisterControlEvents();
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

        ownerComboBox.SelectedIndexChanged -= ownerComboBox_SelectedIndexChanged;
        var users = AccessClient.Instance.UsersAndGroups.OfType<LightweightUser>();
        if (!Content.ParentProjectId.HasValue) users = users.Where(x => x.Roles.Select(y => y.Name).Contains(HiveRoles.Administrator));
        ownerComboBox.DataSource = users.ToList();
        ownerComboBox.SelectedItem = users.FirstOrDefault(x => x.Id == Content.OwnerUserId);
        ownerComboBox.SelectedIndexChanged += ownerComboBox_SelectedIndexChanged;

        createdTextBox.Text = Content.DateCreated.ToString("ddd, dd.MM.yyyy, HH:mm:ss");
        startDateTimePicker.Value = Content.StartDate;

        indefiniteCheckBox.Checked = !Content.EndDate.HasValue;
        if (!indefiniteCheckBox.Checked) endDateTimePicker.Value = Content.EndDate.Value;
        else endDateTimePicker.Value = Content.StartDate;
        endDateTimePicker.Enabled = !indefiniteCheckBox.Checked;
      }
      RegisterControlEvents();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool enabled = Content != null && !Locked && !ReadOnly;
      nameTextBox.Enabled = enabled;
      descriptionTextBox.Enabled = enabled;
      refreshButton.Enabled = enabled;
      ownerComboBox.Enabled = enabled;
      createdTextBox.Enabled = enabled;
      startDateTimePicker.Enabled = enabled;
      endDateTimePicker.Enabled = enabled && Content.EndDate.HasValue;
      indefiniteCheckBox.Enabled = enabled;

      if (Content != null) {
        var parentProject = HiveAdminClient.Instance.GetAvailableProjectAncestors(Content.Id).LastOrDefault();
        if (parentProject == null || parentProject.EndDate.HasValue) {
          indefiniteCheckBox.Enabled = false;
        }

        if (!IsAdmin() && !HiveAdminClient.Instance.CheckOwnershipOfParentProject(Content, UserInformation.Instance.User.Id)) {
          ownerComboBox.Enabled = false;
          startDateTimePicker.Enabled = false;
          endDateTimePicker.Enabled = false;
          indefiniteCheckBox.Enabled = false;
        }
      }
    }
    #endregion

    #region Event Handlers
    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (InvokeRequired) Invoke((Action<object, PropertyChangedEventArgs>)Content_PropertyChanged, sender, e);
      else OnContentChanged();
    }

    private void AccessClient_Instance_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)AccessClient_Instance_Refreshing, sender, e);
      else {
        var mainForm = MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>();
        mainForm.AddOperationProgressToView(this, "Refreshing ...");
        SetEnabledStateOfControls();
      }
    }

    private void AccessClient_Instance_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)AccessClient_Instance_Refreshed, sender, e);
      else {
        var mainForm = MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>();
        mainForm.RemoveOperationProgressFromView(this);
        SetEnabledStateOfControls();
      }
    }

    private async void ProjectView_Load(object sender, EventArgs e) {
      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => UpdateUsers(),
        finallyCallback: () => {
          ownerComboBox.SelectedIndexChanged -= ownerComboBox_SelectedIndexChanged;
          var users = AccessClient.Instance.UsersAndGroups.OfType<LightweightUser>();
          if (Content != null && !Content.ParentProjectId.HasValue) users = users.Where(x => x.Roles.Select(y => y.Name).Contains(HiveRoles.Administrator));
          ownerComboBox.DataSource = users.ToList();
          ownerComboBox.SelectedIndexChanged += ownerComboBox_SelectedIndexChanged;
        });
    }

    private async void refreshButton_Click(object sender, EventArgs e) {
      lock (locker) {
        if (!refreshButton.Enabled) return;
        refreshButton.Enabled = false;
      }

      await SecurityExceptionUtil.TryAsyncAndReportSecurityExceptions(
        action: () => UpdateUsers(),
        finallyCallback: () => {
          ownerComboBox.SelectedIndexChanged -= ownerComboBox_SelectedIndexChanged;
          var users = AccessClient.Instance.UsersAndGroups.OfType<LightweightUser>();
          if (Content != null && !Content.ParentProjectId.HasValue) users = users.Where(x => x.Roles.Select(y => y.Name).Contains(HiveRoles.Administrator));
          ownerComboBox.DataSource = users.ToList();
          ownerComboBox.SelectedItem = users.FirstOrDefault(x => x.Id == persistedOwnerUserId);
          ownerComboBox.SelectedIndexChanged += ownerComboBox_SelectedIndexChanged;
          refreshButton.Enabled = true;
        });
    }

    private void nameTextBox_Validating(object sender, CancelEventArgs e) {
      if (string.IsNullOrEmpty(nameTextBox.Text)) {
        MessageBox.Show(
          "Project must have a name.",
          "HeuristicLab Hive Administrator",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        e.Cancel = true;
      }
    }

    private void nameTextBox_TextChanged(object sender, EventArgs e) {
      if (Content != null && Content.Name != nameTextBox.Text) {
        DeregisterContentEvents();
        Content.Name = nameTextBox.Text;
        RegisterContentEvents();
      }
    }

    private void descriptionTextBox_TextChanged(object sender, EventArgs e) {
      if (Content != null && Content.Description != descriptionTextBox.Text) {
        DeregisterContentEvents();
        Content.Description = descriptionTextBox.Text;
        RegisterContentEvents();
      }
    }

    private void ownerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      var selectedItem = (LightweightUser)ownerComboBox.SelectedItem;
      var selectedOwnerUserId = selectedItem != null ? selectedItem.Id : Guid.Empty;
      if (Content != null && Content.OwnerUserId != selectedOwnerUserId) {
        DeregisterContentEvents();
        Content.OwnerUserId = selectedOwnerUserId;
        RegisterContentEvents();
      }
    }

    private void startDateTimePicker_ValueChanged(object sender, EventArgs e) {
      if (Content == null) return;
      startDateTimePicker.ValueChanged -= startDateTimePicker_ValueChanged;

      if (!IsAdmin()) {
        var parentProject = HiveAdminClient.Instance.GetAvailableProjectAncestors(Content.Id).LastOrDefault();
        if (parentProject != null) {
          if (startDateTimePicker.Value < parentProject.StartDate)
            startDateTimePicker.Value = parentProject.StartDate;
        } else {
          startDateTimePicker.Value = Content.StartDate;
        }
      }

      if (!Content.EndDate.HasValue || startDateTimePicker.Value > Content.EndDate)
        endDateTimePicker.Value = startDateTimePicker.Value;
      if (Content.StartDate != startDateTimePicker.Value) {
        DeregisterContentEvents();
        Content.StartDate = startDateTimePicker.Value;
        RegisterContentEvents();
      }

      startDateTimePicker.ValueChanged += startDateTimePicker_ValueChanged;
    }

    private void endDateTimePicker_ValueChanged(object sender, EventArgs e) {
      if (Content == null) return;
      endDateTimePicker.ValueChanged -= endDateTimePicker_ValueChanged;

      if (!IsAdmin()) {
        var parentProject = HiveAdminClient.Instance.GetAvailableProjectAncestors(Content.Id).LastOrDefault();
        if (parentProject != null) {
          if (parentProject.EndDate.HasValue && endDateTimePicker.Value > parentProject.EndDate.Value) {
            endDateTimePicker.Value = parentProject.EndDate.Value;
          }
        } else if (Content.EndDate.HasValue) {
          endDateTimePicker.Value = Content.EndDate.Value;
        }
      }

      if (endDateTimePicker.Value < startDateTimePicker.Value)
        endDateTimePicker.Value = startDateTimePicker.Value;
      if (Content.EndDate != endDateTimePicker.Value) {
        DeregisterContentEvents();
        Content.EndDate = endDateTimePicker.Value;
        RegisterContentEvents();
      }

      endDateTimePicker.ValueChanged += endDateTimePicker_ValueChanged;
    }

    private void indefiniteCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content == null) return;

      var newEndDate = indefiniteCheckBox.Checked ? (DateTime?)null : endDateTimePicker.Value;
      endDateTimePicker.Enabled = !indefiniteCheckBox.Checked;
      if (Content.EndDate != newEndDate) {
        DeregisterContentEvents();
        Content.EndDate = newEndDate;
        RegisterContentEvents();
      }
    }
    #endregion

    #region Helpers
    private void UpdateUsers() {
      try {
        AccessClient.Instance.Refresh();
      } catch (AnonymousUserException) {
        ShowHiveInformationDialog();
      }
    }

    private bool IsAdmin() {
      return HiveRoles.CheckAdminUserPermissions();
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
