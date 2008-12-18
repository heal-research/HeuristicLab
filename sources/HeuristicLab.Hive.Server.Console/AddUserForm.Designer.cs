#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Hive.Server.Console {
  partial class AddUserForm {
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
      this.lblOne = new System.Windows.Forms.Label();
      this.lblGroup = new System.Windows.Forms.Label();
      this.tbOne = new System.Windows.Forms.TextBox();
      this.cbGroups = new System.Windows.Forms.ComboBox();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.tbPwd = new System.Windows.Forms.TextBox();
      this.lblPassword = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lblOne
      // 
      this.lblOne.AutoSize = true;
      this.lblOne.Location = new System.Drawing.Point(12, 14);
      this.lblOne.Name = "lblOne";
      this.lblOne.Size = new System.Drawing.Size(35, 13);
      this.lblOne.TabIndex = 0;
      this.lblOne.Text = "label1";
      // 
      // lblGroup
      // 
      this.lblGroup.AutoSize = true;
      this.lblGroup.Location = new System.Drawing.Point(12, 71);
      this.lblGroup.Name = "lblGroup";
      this.lblGroup.Size = new System.Drawing.Size(35, 13);
      this.lblGroup.TabIndex = 1;
      this.lblGroup.Text = "label2";
      // 
      // tbOne
      // 
      this.tbOne.Location = new System.Drawing.Point(120, 7);
      this.tbOne.Name = "tbOne";
      this.tbOne.Size = new System.Drawing.Size(212, 20);
      this.tbOne.TabIndex = 2;
      // 
      // cbGroups
      // 
      this.cbGroups.FormattingEnabled = true;
      this.cbGroups.Location = new System.Drawing.Point(120, 68);
      this.cbGroups.Name = "cbGroups";
      this.cbGroups.Size = new System.Drawing.Size(212, 21);
      this.cbGroups.TabIndex = 3;
      // 
      // btnAdd
      // 
      this.btnAdd.Location = new System.Drawing.Point(12, 95);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(75, 23);
      this.btnAdd.TabIndex = 4;
      this.btnAdd.Text = "Add";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(257, 95);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // tbPwd
      // 
      this.tbPwd.Location = new System.Drawing.Point(120, 37);
      this.tbPwd.Name = "tbPwd";
      this.tbPwd.Size = new System.Drawing.Size(212, 20);
      this.tbPwd.TabIndex = 7;
      // 
      // lblPassword
      // 
      this.lblPassword.AutoSize = true;
      this.lblPassword.Location = new System.Drawing.Point(12, 44);
      this.lblPassword.Name = "lblPassword";
      this.lblPassword.Size = new System.Drawing.Size(53, 13);
      this.lblPassword.TabIndex = 6;
      this.lblPassword.Text = "Password";
      // 
      // AddUserForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.ClientSize = new System.Drawing.Size(344, 130);
      this.Controls.Add(this.tbPwd);
      this.Controls.Add(this.lblPassword);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnAdd);
      this.Controls.Add(this.cbGroups);
      this.Controls.Add(this.tbOne);
      this.Controls.Add(this.lblGroup);
      this.Controls.Add(this.lblOne);
      this.Name = "AddUserForm";
      this.Text = "AddNewForm";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblOne;
    private System.Windows.Forms.Label lblGroup;
    private System.Windows.Forms.TextBox tbOne;
    private System.Windows.Forms.ComboBox cbGroups;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.TextBox tbPwd;
    private System.Windows.Forms.Label lblPassword;
  }
}