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
namespace HeuristicLab.PluginInfrastructure.Advanced {
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionSetupView));
      this.urlTextBox = new System.Windows.Forms.TextBox();
      this.urlLabel = new System.Windows.Forms.Label();
      this.userLabel = new System.Windows.Forms.Label();
      this.userTextBox = new System.Windows.Forms.TextBox();
      this.passwordLabel = new System.Windows.Forms.Label();
      this.passwordTextBox = new System.Windows.Forms.TextBox();
      this.applyButton = new System.Windows.Forms.Button();
      this.savePasswordCheckbox = new System.Windows.Forms.CheckBox();
      this.cancelButton = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // urlTextBox
      // 
      this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.urlTextBox.Location = new System.Drawing.Point(66, 15);
      this.urlTextBox.Name = "urlTextBox";
      this.urlTextBox.Size = new System.Drawing.Size(410, 20);
      this.urlTextBox.TabIndex = 0;
      this.toolTip.SetToolTip(this.urlTextBox, "URL of HeuristicLab Plugin Deployment Service");
      // 
      // urlLabel
      // 
      this.urlLabel.AutoSize = true;
      this.urlLabel.Location = new System.Drawing.Point(12, 18);
      this.urlLabel.Name = "urlLabel";
      this.urlLabel.Size = new System.Drawing.Size(48, 13);
      this.urlLabel.TabIndex = 1;
      this.urlLabel.Text = "&Address:";
      // 
      // userLabel
      // 
      this.userLabel.AutoSize = true;
      this.userLabel.Location = new System.Drawing.Point(6, 22);
      this.userLabel.Name = "userLabel";
      this.userLabel.Size = new System.Drawing.Size(58, 13);
      this.userLabel.TabIndex = 3;
      this.userLabel.Text = "&Username:";
      // 
      // userTextBox
      // 
      this.userTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.userTextBox.Location = new System.Drawing.Point(94, 19);
      this.userTextBox.Name = "userTextBox";
      this.userTextBox.Size = new System.Drawing.Size(155, 20);
      this.userTextBox.TabIndex = 0;
      this.toolTip.SetToolTip(this.userTextBox, "Username used to connect to the HeuristicLab plugin deployment service");
      // 
      // passwordLabel
      // 
      this.passwordLabel.AutoSize = true;
      this.passwordLabel.Location = new System.Drawing.Point(6, 48);
      this.passwordLabel.Name = "passwordLabel";
      this.passwordLabel.Size = new System.Drawing.Size(56, 13);
      this.passwordLabel.TabIndex = 5;
      this.passwordLabel.Text = "&Password:";
      // 
      // passwordTextBox
      // 
      this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.passwordTextBox.Location = new System.Drawing.Point(94, 45);
      this.passwordTextBox.Name = "passwordTextBox";
      this.passwordTextBox.Size = new System.Drawing.Size(155, 20);
      this.passwordTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.passwordTextBox, "Passwort used to connect to the HeuristicLab plugin deployment service");
      this.passwordTextBox.UseSystemPasswordChar = true;
      // 
      // applyButton
      // 
      this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.applyButton.Location = new System.Drawing.Point(320, 148);
      this.applyButton.Name = "applyButton";
      this.applyButton.Size = new System.Drawing.Size(75, 23);
      this.applyButton.TabIndex = 2;
      this.applyButton.Text = "&OK";
      this.toolTip.SetToolTip(this.applyButton, "Apply and save changes");
      this.applyButton.UseVisualStyleBackColor = true;
      this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
      // 
      // savePasswordCheckbox
      // 
      this.savePasswordCheckbox.AutoSize = true;
      this.savePasswordCheckbox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.savePasswordCheckbox.Location = new System.Drawing.Point(6, 71);
      this.savePasswordCheckbox.Name = "savePasswordCheckbox";
      this.savePasswordCheckbox.Size = new System.Drawing.Size(103, 17);
      this.savePasswordCheckbox.TabIndex = 2;
      this.savePasswordCheckbox.Text = "&Save Password:";
      this.toolTip.SetToolTip(this.savePasswordCheckbox, "Check to save the user credentials to disk");
      this.savePasswordCheckbox.UseVisualStyleBackColor = true;
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(401, 148);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 3;
      this.cancelButton.Text = "&Cancel";
      this.toolTip.SetToolTip(this.cancelButton, "Cancel and revert changes");
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.userTextBox);
      this.groupBox1.Controls.Add(this.userLabel);
      this.groupBox1.Controls.Add(this.savePasswordCheckbox);
      this.groupBox1.Controls.Add(this.passwordTextBox);
      this.groupBox1.Controls.Add(this.passwordLabel);
      this.groupBox1.Location = new System.Drawing.Point(12, 41);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(255, 96);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "User Credentials";
      // 
      // ConnectionSetupView
      // 
      this.AcceptButton = this.applyButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(488, 183);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.applyButton);
      this.Controls.Add(this.urlLabel);
      this.Controls.Add(this.urlTextBox);
      this.Icon = HeuristicLab.PluginInfrastructure.Resources.HeuristicLab;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ConnectionSetupView";
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Edit Connection Settings";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox urlTextBox;
    private System.Windows.Forms.Label urlLabel;
    private System.Windows.Forms.Label userLabel;
    private System.Windows.Forms.TextBox userTextBox;
    private System.Windows.Forms.Label passwordLabel;
    private System.Windows.Forms.TextBox passwordTextBox;
    private System.Windows.Forms.Button applyButton;
    private System.Windows.Forms.CheckBox savePasswordCheckbox;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
