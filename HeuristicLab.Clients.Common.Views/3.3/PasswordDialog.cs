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
using HeuristicLab.Clients.Common.Properties;

namespace HeuristicLab.Clients.Common {
  public partial class PasswordDialog : Form {
    public PasswordDialog() {
      InitializeComponent();
    }

    private void PasswordDialog_Load(object sender, EventArgs e) {
      usernameTextBox.Text = Settings.Default.UserName;
      passwordTextBox.Text = CryptoService.DecryptString(Settings.Default.Password);
      savePasswordCheckBox.Checked = Settings.Default.SavePassword;
    }

    private void okButton_Click(object sender, EventArgs e) {
      Settings.Default.UserName = usernameTextBox.Text;
      Settings.Default.SavePassword = savePasswordCheckBox.Checked;
      Settings.Default.Password = string.Empty;
      Settings.Default.Save();
      Settings.Default.Password = CryptoService.EncryptString(passwordTextBox.Text);
      if (savePasswordCheckBox.Checked)
        Settings.Default.Save();
    }
  }
}
