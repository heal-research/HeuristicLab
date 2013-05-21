#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Clients.Common;
using CcSettings = HeuristicLab.Clients.Common.Properties;

namespace HeuristicLab.Clients.Access.Views {
  public partial class ChangePasswordDialog : Form {
    public ChangePasswordDialog() {
      InitializeComponent();
    }

    private void cancelButton_Click(object sender, System.EventArgs e) {
      Close();
    }

    private void changePasswordButton_Click(object sender, System.EventArgs e) {
      SaveUserPasswordConfig();
      DisplayProgressBar();
      ExecuteActionAsync(UpdatePassword);
    }

    private void UpdatePassword() {
      UserInformation.Instance.Refresh();

      if (!UserInformation.Instance.UserExists) {
        MessageBox.Show("Couldn't fetch user information from the server." + Environment.NewLine + "Please verify that you have an existing user and that your user name and password is correct. ", "HeuristicLab Access Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
      } else {
        bool result = AccessClient.CallAccessService<bool>(x => x.ChangePassword(UserInformation.Instance.User.Id, oldPasswordTextBox.Text, newPasswordTextBox.Text));
        if (result) {
          MessageBox.Show("Password change successfull.", "HeuristicLab Access Service", MessageBoxButtons.OK, MessageBoxIcon.Information);
          SaveNewUserPasswordConfig();
          Close();
        } else {
          MessageBox.Show("Password change failed. " + Environment.NewLine + "Please check the entered passwords to conform with the passwords standards.", "HeuristicLab Access Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void SaveUserPasswordConfig() {
      CcSettings.Settings.Default.UserName = userNameTextBox.Text;
      CcSettings.Settings.Default.SavePassword = savePasswordCheckBox.Checked;
      CcSettings.Settings.Default.Password = string.Empty;
      CcSettings.Settings.Default.Save();
      CcSettings.Settings.Default.Password = CryptoService.EncryptString(oldPasswordTextBox.Text);
      if (savePasswordCheckBox.Checked)
        CcSettings.Settings.Default.Save();
    }

    private void SaveNewUserPasswordConfig() {
      CcSettings.Settings.Default.Password = CryptoService.EncryptString(newPasswordTextBox.Text);
      if (savePasswordCheckBox.Checked)
        CcSettings.Settings.Default.Save();
    }

    private void DisplayProgressBar() {
      progressBar.Visible = true;
      progressBar.Style = ProgressBarStyle.Marquee;
      progressBar.Value = 0;
    }

    private void HideProgressBar() {
      progressBar.Visible = false;
    }

    private void ChangePasswordDialog_Load(object sender, EventArgs e) {
      userNameTextBox.Text = CcSettings.Settings.Default.UserName;
      oldPasswordTextBox.Text = CryptoService.DecryptString(CcSettings.Settings.Default.Password);
      savePasswordCheckBox.Checked = CcSettings.Settings.Default.SavePassword;
    }

    public void ExecuteActionAsync(Action action) {
      var call = new Func<Exception>(delegate() {
        try {
          action();
        }
        catch (Exception ex) {
          return ex;
        }
        finally {
          HideProgressBar();
        }
        return null;
      });
      call.BeginInvoke(delegate(IAsyncResult result) {
        Exception ex = call.EndInvoke(result);
        if (ex != null) PluginInfrastructure.ErrorHandling.ShowErrorDialog(ex);
      }, null);
    }

    #region validation events
    private void userNameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (userNameTextBox.Text == string.Empty) {
        e.Cancel = true;
        errorProvider.SetError(userNameTextBox, "Please enter a user name");
      }
    }

    private void userNameTextBox_Validated(object sender, EventArgs e) {
      errorProvider.SetError(userNameTextBox, string.Empty);
    }

    private void oldPasswordTextBox_Validated(object sender, EventArgs e) {
      errorProvider.SetError(oldPasswordTextBox, string.Empty);
    }

    private void oldPasswordTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (oldPasswordTextBox.Text == string.Empty) {
        e.Cancel = true;
        errorProvider.SetError(oldPasswordTextBox, "Please enter a password");
      }
    }

    private void newPasswordTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (newPasswordTextBox.Text == string.Empty) {
        e.Cancel = true;
        errorProvider.SetError(newPasswordTextBox, "Please enter a new password");
      }
    }

    private void newPasswordTextBox_Validated(object sender, EventArgs e) {
      errorProvider.SetError(newPasswordTextBox, string.Empty);
    }

    private void retypedNewPasswordtextBox_Validated(object sender, EventArgs e) {
      errorProvider.SetError(retypedNewPasswordtextBox, string.Empty);
    }

    private void retypedNewPasswordtextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (retypedNewPasswordtextBox.Text == string.Empty) {
        e.Cancel = true;
        errorProvider.SetError(retypedNewPasswordtextBox, "Please retype the new password");
      }
    }
    #endregion
  }
}
