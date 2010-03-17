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
      this.urlTextBox = new System.Windows.Forms.TextBox();
      this.urlLabel = new System.Windows.Forms.Label();
      this.userLabel = new System.Windows.Forms.Label();
      this.userTextBox = new System.Windows.Forms.TextBox();
      this.passwordLabel = new System.Windows.Forms.Label();
      this.passwordTextBox = new System.Windows.Forms.TextBox();
      this.applyButton = new System.Windows.Forms.Button();
      this.savePasswordCheckbox = new System.Windows.Forms.CheckBox();
      this.cancelButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // urlTextBox
      // 
      this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.urlTextBox.Location = new System.Drawing.Point(71, 3);
      this.urlTextBox.Name = "urlTextBox";
      this.urlTextBox.Size = new System.Drawing.Size(177, 20);
      this.urlTextBox.TabIndex = 0;
      // 
      // urlLabel
      // 
      this.urlLabel.AutoSize = true;
      this.urlLabel.Location = new System.Drawing.Point(3, 6);
      this.urlLabel.Name = "urlLabel";
      this.urlLabel.Size = new System.Drawing.Size(48, 13);
      this.urlLabel.TabIndex = 1;
      this.urlLabel.Text = "Address:";
      // 
      // userLabel
      // 
      this.userLabel.AutoSize = true;
      this.userLabel.Location = new System.Drawing.Point(3, 32);
      this.userLabel.Name = "userLabel";
      this.userLabel.Size = new System.Drawing.Size(32, 13);
      this.userLabel.TabIndex = 3;
      this.userLabel.Text = "User:";
      // 
      // userTextBox
      // 
      this.userTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.userTextBox.Location = new System.Drawing.Point(71, 29);
      this.userTextBox.Name = "userTextBox";
      this.userTextBox.Size = new System.Drawing.Size(177, 20);
      this.userTextBox.TabIndex = 2;
      // 
      // passwordLabel
      // 
      this.passwordLabel.AutoSize = true;
      this.passwordLabel.Location = new System.Drawing.Point(3, 58);
      this.passwordLabel.Name = "passwordLabel";
      this.passwordLabel.Size = new System.Drawing.Size(56, 13);
      this.passwordLabel.TabIndex = 5;
      this.passwordLabel.Text = "Password:";
      // 
      // passwordTextBox
      // 
      this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.passwordTextBox.Location = new System.Drawing.Point(71, 55);
      this.passwordTextBox.Name = "passwordTextBox";
      this.passwordTextBox.Size = new System.Drawing.Size(177, 20);
      this.passwordTextBox.TabIndex = 4;
      this.passwordTextBox.UseSystemPasswordChar = true;
      // 
      // applyButton
      // 
      this.applyButton.Location = new System.Drawing.Point(6, 104);
      this.applyButton.Name = "applyButton";
      this.applyButton.Size = new System.Drawing.Size(75, 23);
      this.applyButton.TabIndex = 6;
      this.applyButton.Text = "Apply";
      this.applyButton.UseVisualStyleBackColor = true;
      this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
      // 
      // savePasswordCheckbox
      // 
      this.savePasswordCheckbox.AutoSize = true;
      this.savePasswordCheckbox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.savePasswordCheckbox.Location = new System.Drawing.Point(71, 81);
      this.savePasswordCheckbox.Name = "savePasswordCheckbox";
      this.savePasswordCheckbox.Size = new System.Drawing.Size(103, 17);
      this.savePasswordCheckbox.TabIndex = 7;
      this.savePasswordCheckbox.Text = "Save Password:";
      this.savePasswordCheckbox.UseVisualStyleBackColor = true;
      // 
      // cancelButton
      // 
      this.cancelButton.Location = new System.Drawing.Point(87, 104);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 8;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // ConnectionSetupView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.savePasswordCheckbox);
      this.Controls.Add(this.applyButton);
      this.Controls.Add(this.passwordLabel);
      this.Controls.Add(this.passwordTextBox);
      this.Controls.Add(this.userLabel);
      this.Controls.Add(this.userTextBox);
      this.Controls.Add(this.urlLabel);
      this.Controls.Add(this.urlTextBox);
      this.Name = "ConnectionSetupView";
      this.Size = new System.Drawing.Size(251, 132);
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
  }
}
