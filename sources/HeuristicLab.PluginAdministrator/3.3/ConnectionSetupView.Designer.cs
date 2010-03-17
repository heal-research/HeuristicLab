#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
namespace HeuristicLab.PluginAdministrator {
  partial class ConnectionSetupView {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.updateAddressTextBox = new System.Windows.Forms.TextBox();
      this.urlLabel = new System.Windows.Forms.Label();
      this.userLabel = new System.Windows.Forms.Label();
      this.userTextBox = new System.Windows.Forms.TextBox();
      this.passwordLabel = new System.Windows.Forms.Label();
      this.passwordTextBox = new System.Windows.Forms.TextBox();
      this.applyButton = new System.Windows.Forms.Button();
      this.saveCredentialsButton = new System.Windows.Forms.Button();
      this.updateLocationGroupBox = new System.Windows.Forms.GroupBox();
      this.label1 = new System.Windows.Forms.Label();
      this.adminAddressTextBox = new System.Windows.Forms.TextBox();
      this.updateLocationGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // updateAddressTextBox
      // 
      this.updateAddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.updateAddressTextBox.Location = new System.Drawing.Point(126, 19);
      this.updateAddressTextBox.Name = "updateAddressTextBox";
      this.updateAddressTextBox.Size = new System.Drawing.Size(212, 20);
      this.updateAddressTextBox.TabIndex = 0;
      // 
      // urlLabel
      // 
      this.urlLabel.AutoSize = true;
      this.urlLabel.Location = new System.Drawing.Point(36, 22);
      this.urlLabel.Name = "urlLabel";
      this.urlLabel.Size = new System.Drawing.Size(84, 13);
      this.urlLabel.TabIndex = 1;
      this.urlLabel.Text = "Update Service:";
      // 
      // userLabel
      // 
      this.userLabel.AutoSize = true;
      this.userLabel.Location = new System.Drawing.Point(88, 74);
      this.userLabel.Name = "userLabel";
      this.userLabel.Size = new System.Drawing.Size(32, 13);
      this.userLabel.TabIndex = 3;
      this.userLabel.Text = "User:";
      // 
      // userTextBox
      // 
      this.userTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.userTextBox.Location = new System.Drawing.Point(126, 71);
      this.userTextBox.Name = "userTextBox";
      this.userTextBox.Size = new System.Drawing.Size(212, 20);
      this.userTextBox.TabIndex = 2;
      // 
      // passwordLabel
      // 
      this.passwordLabel.AutoSize = true;
      this.passwordLabel.Location = new System.Drawing.Point(64, 100);
      this.passwordLabel.Name = "passwordLabel";
      this.passwordLabel.Size = new System.Drawing.Size(56, 13);
      this.passwordLabel.TabIndex = 5;
      this.passwordLabel.Text = "Password:";
      // 
      // passwordTextBox
      // 
      this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.passwordTextBox.Location = new System.Drawing.Point(126, 97);
      this.passwordTextBox.Name = "passwordTextBox";
      this.passwordTextBox.Size = new System.Drawing.Size(212, 20);
      this.passwordTextBox.TabIndex = 4;
      this.passwordTextBox.UseSystemPasswordChar = true;
      // 
      // applyButton
      // 
      this.applyButton.Location = new System.Drawing.Point(3, 133);
      this.applyButton.Name = "applyButton";
      this.applyButton.Size = new System.Drawing.Size(75, 23);
      this.applyButton.TabIndex = 6;
      this.applyButton.Text = "Apply";
      this.applyButton.UseVisualStyleBackColor = true;
      this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
      // 
      // saveCredentialsButton
      // 
      this.saveCredentialsButton.Location = new System.Drawing.Point(84, 133);
      this.saveCredentialsButton.Name = "saveCredentialsButton";
      this.saveCredentialsButton.Size = new System.Drawing.Size(120, 23);
      this.saveCredentialsButton.TabIndex = 8;
      this.saveCredentialsButton.Text = "Store Credentials";
      this.saveCredentialsButton.UseVisualStyleBackColor = true;
      this.saveCredentialsButton.Click += new System.EventHandler(this.saveCredentialsButton_Click);
      // 
      // updateLocationGroupBox
      // 
      this.updateLocationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.updateLocationGroupBox.Controls.Add(this.adminAddressTextBox);
      this.updateLocationGroupBox.Controls.Add(this.label1);
      this.updateLocationGroupBox.Controls.Add(this.passwordTextBox);
      this.updateLocationGroupBox.Controls.Add(this.updateAddressTextBox);
      this.updateLocationGroupBox.Controls.Add(this.urlLabel);
      this.updateLocationGroupBox.Controls.Add(this.userTextBox);
      this.updateLocationGroupBox.Controls.Add(this.passwordLabel);
      this.updateLocationGroupBox.Controls.Add(this.userLabel);
      this.updateLocationGroupBox.Location = new System.Drawing.Point(3, 3);
      this.updateLocationGroupBox.Name = "updateLocationGroupBox";
      this.updateLocationGroupBox.Size = new System.Drawing.Size(344, 124);
      this.updateLocationGroupBox.TabIndex = 10;
      this.updateLocationGroupBox.TabStop = false;
      this.updateLocationGroupBox.Text = "Update Location";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 48);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(114, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "Administration Service:";
      // 
      // adminAddressTextBox
      // 
      this.adminAddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.adminAddressTextBox.Location = new System.Drawing.Point(126, 45);
      this.adminAddressTextBox.Name = "adminAddressTextBox";
      this.adminAddressTextBox.Size = new System.Drawing.Size(212, 20);
      this.adminAddressTextBox.TabIndex = 6;
      // 
      // ConnectionSetupView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.updateLocationGroupBox);
      this.Controls.Add(this.saveCredentialsButton);
      this.Controls.Add(this.applyButton);
      this.Name = "ConnectionSetupView";
      this.Size = new System.Drawing.Size(350, 161);
      this.updateLocationGroupBox.ResumeLayout(false);
      this.updateLocationGroupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox updateAddressTextBox;
    private System.Windows.Forms.Label urlLabel;
    private System.Windows.Forms.Label userLabel;
    private System.Windows.Forms.TextBox userTextBox;
    private System.Windows.Forms.Label passwordLabel;
    private System.Windows.Forms.TextBox passwordTextBox;
    private System.Windows.Forms.Button applyButton;
    private System.Windows.Forms.Button saveCredentialsButton;
    private System.Windows.Forms.GroupBox updateLocationGroupBox;
    private System.Windows.Forms.TextBox adminAddressTextBox;
    private System.Windows.Forms.Label label1;
  }
}
