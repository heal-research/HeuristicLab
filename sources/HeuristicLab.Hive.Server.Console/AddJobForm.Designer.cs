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
  partial class AddJobForm {
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
      this.lblParentJob = new System.Windows.Forms.Label();
      this.cbParJob = new System.Windows.Forms.ComboBox();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.lblNumJobs = new System.Windows.Forms.Label();
      this.tbNumJobs = new System.Windows.Forms.TextBox();
      this.lblError = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lblParentJob
      // 
      this.lblParentJob.AutoSize = true;
      this.lblParentJob.Location = new System.Drawing.Point(9, 36);
      this.lblParentJob.Name = "lblParentJob";
      this.lblParentJob.Size = new System.Drawing.Size(58, 13);
      this.lblParentJob.TabIndex = 1;
      this.lblParentJob.Text = "Parent Job";
      // 
      // cbParJob
      // 
      this.cbParJob.FormattingEnabled = true;
      this.cbParJob.Location = new System.Drawing.Point(117, 33);
      this.cbParJob.Name = "cbParJob";
      this.cbParJob.Size = new System.Drawing.Size(212, 21);
      this.cbParJob.TabIndex = 3;
      // 
      // btnAdd
      // 
      this.btnAdd.Location = new System.Drawing.Point(12, 65);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(75, 23);
      this.btnAdd.TabIndex = 4;
      this.btnAdd.Text = "Add";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(254, 65);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // lblNumJobs
      // 
      this.lblNumJobs.AutoSize = true;
      this.lblNumJobs.Location = new System.Drawing.Point(12, 9);
      this.lblNumJobs.Name = "lblNumJobs";
      this.lblNumJobs.Size = new System.Drawing.Size(78, 13);
      this.lblNumJobs.TabIndex = 6;
      this.lblNumJobs.Text = "Number of jobs";
      // 
      // tbNumJobs
      // 
      this.tbNumJobs.Location = new System.Drawing.Point(117, 6);
      this.tbNumJobs.Name = "tbNumJobs";
      this.tbNumJobs.Size = new System.Drawing.Size(212, 20);
      this.tbNumJobs.TabIndex = 7;
      this.tbNumJobs.Text = "1";
      // 
      // lblError
      // 
      this.lblError.AutoSize = true;
      this.lblError.Location = new System.Drawing.Point(94, 74);
      this.lblError.Name = "lblError";
      this.lblError.Size = new System.Drawing.Size(0, 13);
      this.lblError.TabIndex = 8;
      // 
      // AddJobForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(344, 100);
      this.Controls.Add(this.lblError);
      this.Controls.Add(this.tbNumJobs);
      this.Controls.Add(this.lblNumJobs);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnAdd);
      this.Controls.Add(this.cbParJob);
      this.Controls.Add(this.lblParentJob);
      this.Name = "AddJobForm";
      this.Text = "Add Job";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblParentJob;
    private System.Windows.Forms.ComboBox cbParJob;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label lblNumJobs;
    private System.Windows.Forms.TextBox tbNumJobs;
    private System.Windows.Forms.Label lblError;
  }
}