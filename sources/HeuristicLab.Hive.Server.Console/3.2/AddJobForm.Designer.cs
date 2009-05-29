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

namespace HeuristicLab.Hive.Server.ServerConsole {
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
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.lblNumJobs = new System.Windows.Forms.Label();
      this.tbNumJobs = new System.Windows.Forms.TextBox();
      this.lblError = new System.Windows.Forms.Label();
      this.lblProject = new System.Windows.Forms.Label();
      this.cbProject = new System.Windows.Forms.ComboBox();
      this.cbAllGroups = new System.Windows.Forms.CheckBox();
      this.gbGroups = new System.Windows.Forms.GroupBox();
      this.btnRemoveGroup = new System.Windows.Forms.Button();
      this.btnAddGroup = new System.Windows.Forms.Button();
      this.lbGroupsIn = new System.Windows.Forms.ListBox();
      this.lbGroupsOut = new System.Windows.Forms.ListBox();
      this.ofdLoadJob = new System.Windows.Forms.OpenFileDialog();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.btnLoad = new System.Windows.Forms.Button();
      this.gbGroups.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnAdd
      // 
      this.btnAdd.Location = new System.Drawing.Point(6, 249);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(75, 23);
      this.btnAdd.TabIndex = 3;
      this.btnAdd.Text = "Add";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
      // 
      // btnClose
      // 
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Location = new System.Drawing.Point(291, 249);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 4;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
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
      this.tbNumJobs.Size = new System.Drawing.Size(243, 20);
      this.tbNumJobs.TabIndex = 1;
      this.tbNumJobs.Text = "1";
      // 
      // lblError
      // 
      this.lblError.AutoSize = true;
      this.lblError.Location = new System.Drawing.Point(87, 254);
      this.lblError.Name = "lblError";
      this.lblError.Size = new System.Drawing.Size(0, 13);
      this.lblError.TabIndex = 8;
      // 
      // lblProject
      // 
      this.lblProject.AutoSize = true;
      this.lblProject.Location = new System.Drawing.Point(12, 39);
      this.lblProject.Name = "lblProject";
      this.lblProject.Size = new System.Drawing.Size(59, 13);
      this.lblProject.TabIndex = 9;
      this.lblProject.Text = "Job project";
      // 
      // cbProject
      // 
      this.cbProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbProject.FormattingEnabled = true;
      this.cbProject.Location = new System.Drawing.Point(117, 31);
      this.cbProject.Name = "cbProject";
      this.cbProject.Size = new System.Drawing.Size(245, 21);
      this.cbProject.TabIndex = 10;
      // 
      // cbAllGroups
      // 
      this.cbAllGroups.AutoSize = true;
      this.cbAllGroups.Checked = true;
      this.cbAllGroups.CheckState = System.Windows.Forms.CheckState.Checked;
      this.cbAllGroups.Location = new System.Drawing.Point(15, 90);
      this.cbAllGroups.Name = "cbAllGroups";
      this.cbAllGroups.Size = new System.Drawing.Size(91, 17);
      this.cbAllGroups.TabIndex = 15;
      this.cbAllGroups.Text = "use all groups";
      this.cbAllGroups.UseVisualStyleBackColor = true;
      this.cbAllGroups.CheckedChanged += new System.EventHandler(this.cbAllGroups_CheckedChanged);
      // 
      // gbGroups
      // 
      this.gbGroups.Controls.Add(this.btnRemoveGroup);
      this.gbGroups.Controls.Add(this.btnAddGroup);
      this.gbGroups.Controls.Add(this.lbGroupsIn);
      this.gbGroups.Controls.Add(this.lbGroupsOut);
      this.gbGroups.Location = new System.Drawing.Point(10, 115);
      this.gbGroups.Name = "gbGroups";
      this.gbGroups.Size = new System.Drawing.Size(357, 128);
      this.gbGroups.TabIndex = 16;
      this.gbGroups.TabStop = false;
      this.gbGroups.Text = "choose groups";
      // 
      // btnRemoveGroup
      // 
      this.btnRemoveGroup.Enabled = false;
      this.btnRemoveGroup.Location = new System.Drawing.Point(143, 56);
      this.btnRemoveGroup.Name = "btnRemoveGroup";
      this.btnRemoveGroup.Size = new System.Drawing.Size(75, 23);
      this.btnRemoveGroup.TabIndex = 18;
      this.btnRemoveGroup.Text = "<< Remove";
      this.btnRemoveGroup.UseVisualStyleBackColor = true;
      this.btnRemoveGroup.Click += new System.EventHandler(this.btnRemoveGroup_Click);
      // 
      // btnAddGroup
      // 
      this.btnAddGroup.Enabled = false;
      this.btnAddGroup.Location = new System.Drawing.Point(143, 26);
      this.btnAddGroup.Name = "btnAddGroup";
      this.btnAddGroup.Size = new System.Drawing.Size(75, 23);
      this.btnAddGroup.TabIndex = 17;
      this.btnAddGroup.Text = "Add >>";
      this.btnAddGroup.UseVisualStyleBackColor = true;
      this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
      // 
      // lbGroupsIn
      // 
      this.lbGroupsIn.Enabled = false;
      this.lbGroupsIn.FormattingEnabled = true;
      this.lbGroupsIn.Location = new System.Drawing.Point(224, 15);
      this.lbGroupsIn.Name = "lbGroupsIn";
      this.lbGroupsIn.Size = new System.Drawing.Size(128, 108);
      this.lbGroupsIn.TabIndex = 16;
      this.lbGroupsIn.DoubleClick += new System.EventHandler(this.lbGroupsIn_SelectedIndexChanged);
      // 
      // lbGroupsOut
      // 
      this.lbGroupsOut.Enabled = false;
      this.lbGroupsOut.FormattingEnabled = true;
      this.lbGroupsOut.Location = new System.Drawing.Point(6, 15);
      this.lbGroupsOut.Name = "lbGroupsOut";
      this.lbGroupsOut.Size = new System.Drawing.Size(130, 108);
      this.lbGroupsOut.TabIndex = 15;
      this.lbGroupsOut.DoubleClick += new System.EventHandler(this.lbGroupsOut_SelectedIndexChanged);
      // 
      // ofdLoadJob
      // 
      this.ofdLoadJob.FileName = "openFileDialog1";
      // 
      // textBox1
      // 
      this.textBox1.BackColor = System.Drawing.SystemColors.Window;
      this.textBox1.Enabled = false;
      this.textBox1.Location = new System.Drawing.Point(13, 64);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(266, 20);
      this.textBox1.TabIndex = 17;
      // 
      // btnLoad
      // 
      this.btnLoad.Location = new System.Drawing.Point(285, 63);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(75, 23);
      this.btnLoad.TabIndex = 18;
      this.btnLoad.Text = "Load Job";
      this.btnLoad.UseVisualStyleBackColor = true;
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
      // 
      // AddJobForm
      // 
      this.AcceptButton = this.btnAdd;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(372, 280);
      this.Controls.Add(this.btnLoad);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.gbGroups);
      this.Controls.Add(this.cbAllGroups);
      this.Controls.Add(this.cbProject);
      this.Controls.Add(this.lblProject);
      this.Controls.Add(this.lblError);
      this.Controls.Add(this.tbNumJobs);
      this.Controls.Add(this.lblNumJobs);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnAdd);
      this.Name = "AddJobForm";
      this.Text = "Add Job";
      this.gbGroups.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label lblNumJobs;
    private System.Windows.Forms.TextBox tbNumJobs;
    private System.Windows.Forms.Label lblError;
    private System.Windows.Forms.Label lblProject;
    private System.Windows.Forms.ComboBox cbProject;
    private System.Windows.Forms.CheckBox cbAllGroups;
    private System.Windows.Forms.GroupBox gbGroups;
    private System.Windows.Forms.Button btnRemoveGroup;
    private System.Windows.Forms.Button btnAddGroup;
    private System.Windows.Forms.ListBox lbGroupsIn;
    private System.Windows.Forms.ListBox lbGroupsOut;
    private System.Windows.Forms.OpenFileDialog ofdLoadJob;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Button btnLoad;
  }
}