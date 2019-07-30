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

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  partial class HiveJobPermissionView {
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
      this.usernameLabel = new System.Windows.Forms.Label();
      this.permissionLabel = new System.Windows.Forms.Label();
      this.usernameTextBox = new System.Windows.Forms.TextBox();
      this.permissionComboBox = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // storeButton
      // 
      this.toolTip.SetToolTip(this.storeButton, "Store Data");
      // 
      // usernameLabel
      // 
      this.usernameLabel.AutoSize = true;
      this.usernameLabel.Location = new System.Drawing.Point(3, 27);
      this.usernameLabel.Name = "usernameLabel";
      this.usernameLabel.Size = new System.Drawing.Size(58, 13);
      this.usernameLabel.TabIndex = 1;
      this.usernameLabel.Text = "Username:";
      // 
      // permissionLabel
      // 
      this.permissionLabel.AutoSize = true;
      this.permissionLabel.Location = new System.Drawing.Point(4, 57);
      this.permissionLabel.Name = "permissionLabel";
      this.permissionLabel.Size = new System.Drawing.Size(60, 13);
      this.permissionLabel.TabIndex = 2;
      this.permissionLabel.Text = "Permission:";
      // 
      // usernameTextBox
      // 
      this.usernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.usernameTextBox.Location = new System.Drawing.Point(70, 28);
      this.usernameTextBox.Name = "usernameTextBox";
      this.usernameTextBox.Size = new System.Drawing.Size(281, 20);
      this.usernameTextBox.TabIndex = 3;
      this.usernameTextBox.TextChanged += new System.EventHandler(this.usernameTextBox_TextChanged);
      // 
      // permissionComboBox
      // 
      this.permissionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.permissionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.permissionComboBox.FormattingEnabled = true;
      this.permissionComboBox.Location = new System.Drawing.Point(70, 54);
      this.permissionComboBox.Name = "permissionComboBox";
      this.permissionComboBox.Size = new System.Drawing.Size(281, 21);
      this.permissionComboBox.TabIndex = 4;
      this.permissionComboBox.SelectedValueChanged += new System.EventHandler(this.permissionComboBox_SelectedValueChanged);
      // 
      // HiveJobPermissionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.usernameLabel);
      this.Controls.Add(this.permissionComboBox);
      this.Controls.Add(this.permissionLabel);
      this.Controls.Add(this.usernameTextBox);
      this.Name = "HiveJobPermissionView";
      this.Controls.SetChildIndex(this.usernameTextBox, 0);
      this.Controls.SetChildIndex(this.permissionLabel, 0);
      this.Controls.SetChildIndex(this.permissionComboBox, 0);
      this.Controls.SetChildIndex(this.usernameLabel, 0);
      this.Controls.SetChildIndex(this.storeButton, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private System.Windows.Forms.Label usernameLabel;
    private System.Windows.Forms.Label permissionLabel;
    private System.Windows.Forms.TextBox usernameTextBox;
    private System.Windows.Forms.ComboBox permissionComboBox;
  }
}
