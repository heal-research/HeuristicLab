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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Access.Administration {
  [View("User View")]
  [Content(typeof(User), true)]
  public partial class UserView : ItemView {
    public new User Content {
      get { return (User)base.Content; }
      set { base.Content = value; }
    }

    public UserView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content == null) {
        userNameTextBox.Clear();
        fullNameTextBox.Clear();
        emailTextBox.Clear();
        idTextBox.Clear();
        lastActivityTextBox.Clear();
        lastLoginTextBox.Clear();
        refreshableRoleSelectionListView.Content = null;
        refreshableRoleSelectionListView.CurrentUser = null;
      } else {
        userNameTextBox.Text = Content.UserName;
        fullNameTextBox.Text = Content.FullName;
        emailTextBox.Text = Content.Email;
        idTextBox.Text = Content.Id.ToString();
        lastActivityTextBox.Text = Content.LastActivityDate.ToString();
        lastLoginTextBox.Text = Content.LastLoginDate.ToString();
        refreshableRoleSelectionListView.Content = Content.Id != Guid.Empty ? AccessAdministrationClient.Instance : null;
        refreshableRoleSelectionListView.CurrentUser = Content;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool enabled = Content != null && !Locked;
      
      userNameTextBox.ReadOnly = !enabled;
      fullNameTextBox.ReadOnly = !enabled;
      emailTextBox.ReadOnly = !enabled;

      if (Content == null) {
        resetPasswordButton.Enabled = false;
      } else {
        resetPasswordButton.Enabled = Content.Id != Guid.Empty;
      }
    }

    private void fullNameTextBox_TextChanged(object sender, System.EventArgs e) {
      if (Content.FullName != fullNameTextBox.Text)
        Content.FullName = fullNameTextBox.Text;
    }

    private void userNameTextBox_TextChanged(object sender, System.EventArgs e) {
      if (Content.UserName != userNameTextBox.Text)
        Content.UserName = userNameTextBox.Text;
    }

    private void emailTextBox_TextChanged(object sender, System.EventArgs e) {
      if (Content.Email != emailTextBox.Text)
        Content.Email = emailTextBox.Text;
    }

    private void resetPasswordButton_Click(object sender, System.EventArgs e) {
      Action a = new Action(delegate {
        string result = AccessAdministrationClient.CallAccessService<string>(s => s.ResetPassword(Content.Id));
        ShowPassword(result);
      });

      AccessAdministrationClient.Instance.ExecuteActionAsync(a, PluginInfrastructure.ErrorHandling.ShowErrorDialog); ;
    }

    private void ShowPassword(string result) {
      if (InvokeRequired) {
        Invoke(new Action<string>(ShowPassword), result);
      } else {
        using (PasswordDisplayDialog dlg = new PasswordDisplayDialog()) {
          dlg.Password = result;
          dlg.ShowDialog(this);
        }
      }
    }
  }
}
