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

using HeuristicLab.Clients.Hive.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  [View("Hive Job Permission")]
  [Content(typeof(JobPermission), true)]
  public partial class HiveJobPermissionView : HiveItemView {
    public new JobPermission Content {
      get { return (JobPermission)base.Content; }
      set { base.Content = value; }
    }

    public HiveJobPermissionView() {
      InitializeComponent();
      permissionComboBox.Items.Add(Permission.Read);
      permissionComboBox.Items.Add(Permission.Full);
      permissionComboBox.SelectedItem = Permission.Read;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        usernameTextBox.Text = string.Empty;
        permissionComboBox.SelectedIndex = 0;
      } else {
        usernameTextBox.Text = Content.GrantedUserName;
        permissionComboBox.SelectedItem = Content.Permission;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (Content == null) {
        usernameTextBox.Enabled = false;
        permissionComboBox.Enabled = false;
      } else {
        usernameTextBox.Enabled = true;
        permissionComboBox.Enabled = true;
      }
    }

    private void usernameTextBox_TextChanged(object sender, System.EventArgs e) {
      if (Content != null) {
        Content.GrantedUserName = usernameTextBox.Text;
      }
    }

    private void permissionComboBox_SelectedValueChanged(object sender, System.EventArgs e) {
      if (Content != null) {
        Content.Permission = (Permission)permissionComboBox.SelectedItem;
      }
    }
  }
}
