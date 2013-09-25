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

namespace HeuristicLab.Clients.Access.Views {
  partial class ChangePasswordDialog {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.oldPasswordTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.newPasswordTextBox = new System.Windows.Forms.TextBox();
      this.retypedNewPasswordtextBox = new System.Windows.Forms.TextBox();
      this.changePasswordButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.userNameTextBox = new System.Windows.Forms.TextBox();
      this.savePasswordCheckBox = new System.Windows.Forms.CheckBox();
      this.label5 = new System.Windows.Forms.Label();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.progressBar = new System.Windows.Forms.ProgressBar();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // oldPasswordTextBox
      // 
      this.oldPasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.oldPasswordTextBox.Location = new System.Drawing.Point(149, 39);
      this.oldPasswordTextBox.Name = "oldPasswordTextBox";
      this.oldPasswordTextBox.PasswordChar = '*';
      this.oldPasswordTextBox.Size = new System.Drawing.Size(232, 20);
      this.oldPasswordTextBox.TabIndex = 0;
      this.oldPasswordTextBox.UseSystemPasswordChar = true;
      this.oldPasswordTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.oldPasswordTextBox_Validating);
      this.oldPasswordTextBox.Validated += new System.EventHandler(this.oldPasswordTextBox_Validated);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 42);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(75, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Old Password:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 68);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(81, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "New Password:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 94);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(116, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Retype new Password:";
      // 
      // newPasswordTextBox
      // 
      this.newPasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.newPasswordTextBox.Location = new System.Drawing.Point(149, 65);
      this.newPasswordTextBox.Name = "newPasswordTextBox";
      this.newPasswordTextBox.PasswordChar = '*';
      this.newPasswordTextBox.Size = new System.Drawing.Size(232, 20);
      this.newPasswordTextBox.TabIndex = 4;
      this.newPasswordTextBox.UseSystemPasswordChar = true;
      this.newPasswordTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.newPasswordTextBox_Validating);
      this.newPasswordTextBox.Validated += new System.EventHandler(this.newPasswordTextBox_Validated);
      // 
      // retypedNewPasswordtextBox
      // 
      this.retypedNewPasswordtextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.retypedNewPasswordtextBox.Location = new System.Drawing.Point(149, 91);
      this.retypedNewPasswordtextBox.Name = "retypedNewPasswordtextBox";
      this.retypedNewPasswordtextBox.PasswordChar = '*';
      this.retypedNewPasswordtextBox.Size = new System.Drawing.Size(232, 20);
      this.retypedNewPasswordtextBox.TabIndex = 5;
      this.retypedNewPasswordtextBox.UseSystemPasswordChar = true;
      this.retypedNewPasswordtextBox.Validating += new System.ComponentModel.CancelEventHandler(this.retypedNewPasswordtextBox_Validating);
      this.retypedNewPasswordtextBox.Validated += new System.EventHandler(this.retypedNewPasswordtextBox_Validated);
      // 
      // changePasswordButton
      // 
      this.changePasswordButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.changePasswordButton.Location = new System.Drawing.Point(192, 127);
      this.changePasswordButton.Name = "changePasswordButton";
      this.changePasswordButton.Size = new System.Drawing.Size(108, 23);
      this.changePasswordButton.TabIndex = 6;
      this.changePasswordButton.Text = "Change Password";
      this.changePasswordButton.UseVisualStyleBackColor = true;
      this.changePasswordButton.Click += new System.EventHandler(this.changePasswordButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(306, 127);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 7;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(12, 16);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(61, 13);
      this.label4.TabIndex = 8;
      this.label4.Text = "User name:";
      // 
      // userNameTextBox
      // 
      this.userNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.userNameTextBox.Location = new System.Drawing.Point(149, 13);
      this.userNameTextBox.Name = "userNameTextBox";
      this.userNameTextBox.Size = new System.Drawing.Size(232, 20);
      this.userNameTextBox.TabIndex = 9;
      this.userNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.userNameTextBox_Validating);
      this.userNameTextBox.Validated += new System.EventHandler(this.userNameTextBox_Validated);
      // 
      // savePasswordCheckBox
      // 
      this.savePasswordCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.savePasswordCheckBox.AutoSize = true;
      this.savePasswordCheckBox.Location = new System.Drawing.Point(149, 132);
      this.savePasswordCheckBox.Name = "savePasswordCheckBox";
      this.savePasswordCheckBox.Size = new System.Drawing.Size(15, 14);
      this.savePasswordCheckBox.TabIndex = 10;
      this.savePasswordCheckBox.UseVisualStyleBackColor = true;
      // 
      // label5
      // 
      this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(13, 132);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(84, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = "Save Password:";
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // progressBar
      // 
      this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar.Location = new System.Drawing.Point(119, 60);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(180, 26);
      this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.progressBar.TabIndex = 12;
      this.progressBar.Visible = false;
      // 
      // ChangePasswordDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(402, 158);
      this.Controls.Add(this.progressBar);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.savePasswordCheckBox);
      this.Controls.Add(this.userNameTextBox);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.changePasswordButton);
      this.Controls.Add(this.retypedNewPasswordtextBox);
      this.Controls.Add(this.newPasswordTextBox);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.oldPasswordTextBox);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ChangePasswordDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Change Password";
      this.Load += new System.EventHandler(this.ChangePasswordDialog_Load);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox oldPasswordTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox newPasswordTextBox;
    private System.Windows.Forms.TextBox retypedNewPasswordtextBox;
    private System.Windows.Forms.Button changePasswordButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox userNameTextBox;
    private System.Windows.Forms.CheckBox savePasswordCheckBox;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.ProgressBar progressBar;
  }
}